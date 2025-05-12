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
using Microsoft.Extensions.FileSystemGlobbing;
using Newtonsoft.Json;
using static ExcelEditor.Pages.SMSManager;

public class IndexModel : PageModel
{
    private readonly SMSManager _smsManager;

    public IndexModel(SMSManager smsManager)
    {
        _smsManager = smsManager;
    }
    [BindProperty]
    public IFormFile UploadedFile { get; set; }
    public IFormFile UploadedSummaryFile { get; set; }

    public bool ShowDownloadButton { get; set; } = false;
    public bool ShowSummaryButton { get; set; } = false;

    public class UserModel
    {
        public string? UserID { get; set; }
        public string Mobile { get; set; }
        public string? UserName { get; set; }
        public int? LogNum { get; set; }
        public int SubZone { get; set; }
        public int Zone { get; set; }
        public int? UserStatus { get; set; } = 0;
        public int SheetIndex { get; set; } = 0;
        public List<int> UserLogIDs { get; set; } = new List<int>();
        public List<string> UserLogNames { get; set; } = new List<string>();
        public DateOnly CreatDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateTime CreatDateTime { get; set; } = DateTime.Now;
    }

    public class UserSummaryModel
    {
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public string Message {get; set;}
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
        TempData.Clear();
        ViewData.Clear();
        HttpContext.Session.Clear();
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
                            .Where(x => (x.Loge.LogeGroup.SubZoneId == 1  || x.Loge.LogeGroup.SubZoneId == 6
                                      || x.Loge.LogeGroup.SubZoneId == 38 || x.Loge.LogeGroup.SubZoneId == 2
                                      || x.Loge.LogeGroup.SubZoneId == 3  || x.Loge.LogeGroup.SubZoneId == 4
                                      || x.Loge.LogeGroup.SubZoneId == 5  || x.Loge.LogeGroup.SubZoneId == 21
                                      || x.Loge.LogeGroup.SubZoneId == 34 || x.Loge.LogeGroup.SubZoneId == 41
                                      || x.Loge.LogeGroup.SubZoneId == 7  || x.Loge.LogeGroup.SubZoneId == 16
                                      || x.Loge.LogeGroup.SubZoneId == 48 || x.Loge.LogeGroup.SubZoneId == 43
                              /**/    || x.Loge.LogeGroup.SubZoneId == 45 || x.Loge.LogeGroup.SubZoneId == 46
                                      || x.Loge.LogeGroup.SubZoneId == 49 || x.Loge.LogeGroup.SubZoneId == 50) &&
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

    public async Task<IActionResult> OnPostUploadSummaryAsync()
    {
        ShowSummaryButton = false;
        if (UploadedSummaryFile == null || UploadedSummaryFile.Length == 0)
        {
            ModelState.AddModelError("UploadedSummaryFile", "Please upload a valid summary file.");
            return Page();
        }
        var summaryUsers = ExcelManager.ReadUploadedSummary(UploadedSummaryFile);
        var usersTemp = ExcelManager.SaveSummaryUsersToTempFile(summaryUsers);
        TempData["TempSummaryUser"] = usersTemp;
        ShowSummaryButton = true;
        return Page();
    }

    public async Task<IActionResult> OnPostProcessExcelAsync()
    {
        ShowDownloadButton = false;
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

        if (Directory.Exists(uploadDir))
        {
            foreach (var file in Directory.GetFiles(uploadDir))
            {
                System.IO.File.Delete(file);
            }
        }
        HttpContext.Session.Clear();
        TempData.Clear();

        var originalFilePath = Path.Combine(uploadDir, UploadedFile.FileName);
        using (var stream = new FileStream(originalFilePath, FileMode.Create))
        {
            await UploadedFile.CopyToAsync(stream);
        }

        BookingSystem bookingSystem = new BookingSystem();
        var users = ExcelManager.ReadUsersFromExcel(originalFilePath);
        bookingSystem.ReserveLogs(users);
        bookingSystem.ShowAllUsers(users);
        bookingSystem.ShowAllAvailableLogs();
        bookingSystem.UpdateDB(users);
        var usersTemp = ExcelManager.SaveUsersToTempFile(users);
        ExcelManager.FillUserLogIDsFromLogStore(originalFilePath, users);
        HttpContext.Session.SetString("FilePath", originalFilePath);
        TempData["FilePath"] = originalFilePath;
        TempData["TempUserFile"] = usersTemp;
        TempData["Success"] = true;
        ShowDownloadButton = true;

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

    public IActionResult OnPostDownloadSummaryExcel(string tempFileName)
    {
        var users = ExcelManager.LoadUsersFromTempFile(tempFileName);

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("User Summary");

            worksheet.Cells[1, 1].Value = "ลำดับ";
            worksheet.Cells[1, 2].Value = "UserName";
            worksheet.Cells[1, 3].Value = "UserMobile";
            worksheet.Cells[1, 4].Value = "UserLoge";

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                worksheet.Cells[i + 2, 1].Value = i + 1;
                worksheet.Cells[i + 2, 2].Value = user.UserName;
                worksheet.Cells[i + 2, 3].Value = user.Mobile;
                if (user.UserLogIDs.Count == 0)
                {
                    worksheet.Cells[i + 2, 4].Value = $"{user.UserName} จองไม่สำเร็จค่ะ";
                    continue;
                }
                if (user.UserLogNames.Contains("Already RS Today"))
                {
                    worksheet.Cells[i + 2, 4].Value = $"{user.UserName} ได้มีการจองล็อคแล้วค่ะ";
                    continue;
                }
                string userLogIDsString = $"{user.UserName} ได้จองล็อค {string.Join(",", user.UserLogNames)} สำเร็จค่ะ";
                worksheet.Cells[i + 2, 4].Value = userLogIDsString;
            }
            
            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UserSummary.xlsx");
        }
    }
    public IActionResult OnPostSendSMS(string tempFile ,string tempFileName)
    {
        string message = "";
        if (tempFileName == "TempUserFile")
        {
            var users = ExcelManager.LoadUsersFromTempFile(tempFile);
            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                if (user.UserLogIDs.Count == 0)
                {
                    message = $"{user.UserName} จองไม่สำเร็จค่ะ";
                    _smsManager.SendSMS(user.Mobile, message);
                    continue;
                }
                if (user.UserLogNames.Contains("Already RS Today"))
                {
                    message = $"{user.UserName} ได้มีการจองล็อคแล้วค่ะ";
                    _smsManager.SendSMS(user.Mobile, message);
                    continue;
                }
                string userLogIDsString = $"{user.UserName} ได้จองล็อค {string.Join(",", user.UserLogNames)} สำเร็จค่ะ";
                message = userLogIDsString;
                _smsManager.SendSMS(user.Mobile, message);
            }
            return new OkResult();
        }
        else if (tempFileName == "TempSummaryUser")
        {
            var users = ExcelManager.LoadUsersFromSummaryTemp(tempFile);
            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                message = user.Message;
                _smsManager.SendSMS(user.Mobile, message);
            }
            return new OkResult();
        }
        return Content("Can't send sms");
    }
}
