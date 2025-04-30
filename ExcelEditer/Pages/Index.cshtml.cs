using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using ExcelEditor;
using ExcelEditor.Models;
using static IndexModel;
using System.Text.RegularExpressions;
using ExcelEditor.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public IFormFile UploadedFile { get; set; }

    public class UserModel
    {
        public string UserID { get; set; }
        public int LogNum { get; set; }
        public int zone { get; set; }
        public int UserStatus { get; set; } = 0;
        public int SheetIndex { get; set; } = 0;
        public List<int> UserLogIDs { get; set; } = new List<int>();
        public List<string> UserLogNames { get; set; } = new List<string>();
    }

    public class LogeModel
    {
        public int MaxRow { get; set; }
        public int LogeID { get; set; }
        public int LogeIndex { get; set; }
        public string LogeName { get; set; }
        public int LogeZone { get; set; }
        public int LogeSeqNum { get; set; }
        public int IsReserve { get; set; } // 1 = not reserve , 0 = reserve
        public bool IsCorner { get; set; } = false;
        public int Row { get; set; }
        public int Column { get; set; }
        public DateOnly OpenDateInt { get; set; }
    }

    public void OnGet()
    {
        using (var db = new SaveoneKoratMarketContext())
        {
            DateTime currentDate = DateTime.Now;
            DateTime nextDate = currentDate.AddDays(2);

            int openCase = 0;
            switch (nextDate.DayOfWeek.ToString().ToLower())
            {
                case "monday": openCase = 1; break;
                case "tuesday": openCase = 2; break;
                case "wednesday": openCase = 3; break;
                case "thursday": openCase = 4; break;
                case "friday": openCase = 5; break;
                case "saturday": openCase = 6; break;
                case "sunday": openCase = 7; break;
                default: break;
            }

            var logeTempCurrent = db.LogeTempOfflines.Where(x => x.OpenDateInt.Day == nextDate.Day && x.OpenDateInt.Month == nextDate.Month && x.OpenDateInt.Year == nextDate.Year).ToList();

            using (var context = new SaveoneKoratMarketContext())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        List<LogeTempOffline> logeTemps = new List<LogeTempOffline>();
                        var logeTempMasters = context.LogeTempMasters
                            .Where(x => (x.Loge.LogeGroup.SubZoneId == 43 || x.Loge.LogeGroup.SubZoneId == 45 || x.Loge.LogeGroup.SubZoneId == 46 || x.Loge.LogeGroup.SubZoneId == 49 || x.Loge.LogeGroup.SubZoneId == 50) &&
                                        x.OpenCase == openCase && x.Status == 1 )
                            .ToList();

                        foreach (var item in logeTempMasters)
                        {
                            var isDuplicate = logeTempCurrent.FirstOrDefault(x => x.LogeId == item.LogeId);
                            if (isDuplicate == null)
                                logeTemps.Add(new LogeTempOffline()
                                {
                                    IsConner = item.IsConner,
                                    LogeId = item.LogeId,
                                    LogeIndex = item.LogeIndex,
                                    LogeName = item.LogeName,
                                    LogeTypeId = item.LogeTypeId,
                                    OpenDateInt = DateOnly.FromDateTime(nextDate),
                                    Status = 0,
                                });
                        }
                        if (logeTemps.Count > 0)
                        {
                            context.LogeTempOfflines.AddRange(logeTemps);
                            if (context.SaveChanges() >= 0)
                                dbContextTransaction.Commit();
                            else
                            {
                                dbContextTransaction.Rollback();
                                context.Dispose();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        context.Dispose();

                    }
                }
            }
        }
    }

    public async Task<IActionResult> OnPostProcessExcelAsync()
    {
        if (UploadedFile == null || UploadedFile.Length == 0)
        {
            ModelState.AddModelError("UploadedFile", "Please upload a valid Excel file.");
            return Page();
        }
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadDir))
        {
            Directory.CreateDirectory(uploadDir);
        }
        var originalFilePath = Path.Combine(uploadDir, UploadedFile.FileName);

        using (var stream = new FileStream(originalFilePath, FileMode.Create))
        {
            await UploadedFile.CopyToAsync(stream);
        }
        BookingSystem bookingSystem = new BookingSystem();
        var users = ReadUsersFromExcel(originalFilePath);
        bookingSystem.ReserveLogs(users);
        bookingSystem.ShowAllUsers(users);
        bookingSystem.ShowAllAvailableLogs();
        bookingSystem.UpdateDB();
        //ProcessExcelFile(originalFilePath);
        FillUserLogIDsFromLogStore(originalFilePath, users);
        //var newFileName = Path.GetFileNameWithoutExtension(UploadedFile.FileName) + "Success" + Path.GetExtension(UploadedFile.FileName);
        //var newFilePath = Path.Combine(uploadDir, newFileName);
        //System.IO.File.Move(originalFilePath, newFilePath);
        HttpContext.Session.SetString("FilePath", originalFilePath);
        TempData["FilePath"] = originalFilePath;
        TempData["Success"] = true;

        return Page();
    }
    public IActionResult OnGetDownloadFile()
    {
        var filePath = HttpContext.Session.GetString("FilePath");
        if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
        {
            return NotFound();
        }
        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        var fileName = Path.GetFileName(filePath);
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
    public List<UserModel> ReadUsersFromExcel(string filePath)
    {
        var users = new List<UserModel>();
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var allWorksheets = package.Workbook.Worksheets.ToList();
            //var selectedWorksheets = allWorksheets.Skip(1);

            //foreach (var sheet in allWorksheets)
            //{
            //    int rowCount = sheet.Dimension.Rows;

            //    for (int row = 2; row <= rowCount; row++)
            //    {
            //        if (string.IsNullOrEmpty(sheet.Cells[row, 3].Text))
            //            continue;
            //        var user = new UserModel();
            //        int zoneID = GetLogZone.GetLogzone(sheet.Cells[row, 4].Text);
            //        int marketID = GetLogZone.GetMarketZone(sheet.Cells[row, 4].Text);
            //        user.UserID = sheet.Cells[row, 3].Text + marketID;
            //        user.zone = zoneID;
            //        if (int.TryParse(sheet.Cells[row, 5].Text, out int logNum))
            //            user.LogNum = logNum;
            //        users.Add(user);
            //    }
            //}
            for (int sheetIndex = 0; sheetIndex < allWorksheets.Count; sheetIndex++)
            {
                var sheet = allWorksheets[sheetIndex];
                int rowCount = sheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    if (string.IsNullOrEmpty(sheet.Cells[row, 3].Text) || string.IsNullOrEmpty(sheet.Cells[row, 4].Text))
                        continue;

                    var user = new UserModel();
                    int zoneID = GetLogZone.GetLogzone(sheet.Cells[row, 4].Text);
                    int marketID = GetLogZone.GetMarketZone(sheet.Cells[row, 4].Text);

                    user.UserID = sheet.Cells[row, 3].Text + marketID;
                    user.zone = zoneID;
                    user.SheetIndex = sheetIndex + 1; // เริ่มจาก 1 แทน 0

                    if (int.TryParse(sheet.Cells[row, 5].Text, out int logNum))
                        user.LogNum = logNum;

                    users.Add(user);
                }
            }

        }

        return users;
    }
    public void FillUserLogIDsFromLogStore(string filePath, List<UserModel> x)
    {
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var allWorksheets = package.Workbook.Worksheets.ToList();

            foreach (var sheet in allWorksheets)
            {
                var rowCount = sheet.Dimension.Rows;
                for (int row = 2; row <= rowCount; row++)
                {
                    if (string.IsNullOrEmpty(sheet.Cells[row, 3].Text))
                        continue;
                    int marketID = GetLogZone.GetMarketZone(sheet.Cells[row, 4].Text);
                    string id = sheet.Cells[row, 3].Text + marketID;

                    var matched = x.FirstOrDefault(x => x.UserID == id);

                    if (matched != null && matched.UserLogIDs.Any())
                    {
                        string userLogIDsString = string.Join(",", matched.UserLogNames);
                        sheet.Cells[row, 8].Value = userLogIDsString; // เติมใน column 8
                    }
                }
            }
            package.Save(); 
        }
    }
    
}
