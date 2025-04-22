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
        public List<int> UserLogIDs { get; set; } = new List<int>();
        public List<string> UserLogNames { get; set; } = new List<string>();
    }

    public class Market
    {
        public int LogeIndex { get; set; }
        public int LogeID { get; set; }
        public string LogeName { get; set; }
        public int LogeZone { get; set; }
        public int LogeSeqNum { get; set; }
        public int IsReserve { get; set; } // 1 = not reserve , 0 = reserve
        public bool IsCorner { get; set; } = false;
        public int Row { get; set; }
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
                            .Where(x => (x.Loge.LogeGroup.SubZoneId == 43 || x.Loge.LogeGroup.SubZoneId == 45 || x.Loge.LogeGroup.SubZoneId == 46 || x.Loge.LogeGroup.SubZoneId == 50) &&
                                        x.OpenCase == openCase && x.Status == 1)
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
                                    Status = 0
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
        BookingSystem bookingSystem = new BookingSystem();
        var users = ReadUsersFromExcel(originalFilePath);
        bookingSystem.ShowAllLogs();
        bookingSystem.ReserveLogs(users);
        bookingSystem.UpdateLogsToDB();
        bookingSystem.ShowAllUsers(users);
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
    //public IActionResult OnGetDownloadFile()
    //{
    //    var filePath = TempData["FilePath"] as string;
    //    if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
    //    {
    //        return NotFound();
    //    }

    //    var fileBytes = System.IO.File.ReadAllBytes(filePath);
    //    var fileName = Path.GetFileName(filePath);
    //    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    //}
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
            var worksheet = package.Workbook.Worksheets[1];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) // เริ่มจากแถวที่ 2  
            {
                var user = new UserModel();
                //if (int.TryParse(worksheet.Cells[row, 3].Text, out int userId))
                //    user.UserID = userId;
                user.UserID = worksheet.Cells[row, 3].Text;
                if (int.TryParse(worksheet.Cells[row, 6].Text, out int logNum))
                    user.LogNum = logNum;
                if (int.TryParse(worksheet.Cells[row, 5].Text, out int zone))
                    user.zone = zone;
                //if (int.TryParse(worksheet.Cells[row, 3].Text, out int zone))
                //    user.zone = zone;
                users.Add(user);
            }
        }

        return users;
    }

    //private void ProcessExcelFile(string filePath)
    //{
    //    OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
    //    List<int> logIDs = new List<int> { 101, 102, 103, 104, 105 }; // LogID ที่ต้องแจกจ่าย  
    //    using (var package = new ExcelPackage(new FileInfo(filePath)))
    //    {
    //        var worksheet = package.Workbook.Worksheets[0]; // ใช้ชีตแรก  
    //        int rowCount = worksheet.Dimension.Rows;
    //        int logIndex = 0; // ใช้ในการเข้าถึง LogID  

    //        for (int row = 2; row <= rowCount; row++) // เริ่มจากแถวที่ 2  
    //        {
    //            if (worksheet.Cells[row, 3].Value == null) continue;

    //            int logNum = Convert.ToInt32(worksheet.Cells[row, 3].Value);
    //            List<int> assignedLogs = new List<int>();

    //            for (int i = 0; i < logNum && logIndex < logIDs.Count; i++)
    //            {
    //                assignedLogs.Add(logIDs[logIndex]); // แจก LogID ให้ User  
    //                logIndex++;
    //            }

    //            if (assignedLogs.Count > 0)
    //            {
    //                worksheet.Cells[row, 4].Value = string.Join(",", assignedLogs); // อัปเดตค่า UserLog  
    //            }
    //        }

    //        package.Save(); // บันทึกการเปลี่ยนแปลง  
    //    }
    //}
    public void FillUserLogIDsFromLogStore(string filePath, List<UserModel> x)
    {
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[1];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                string id = worksheet.Cells[row, 3].Text;

                var matched = x.FirstOrDefault(x => x.UserID == id);

                if (matched != null && matched.UserLogIDs.Any())
                {
                    string userLogIDsString = string.Join(",", matched.UserLogNames);
                    worksheet.Cells[row, 9].Value = userLogIDsString; // เติมใน column 8
                }
            }

            package.Save(); // เซฟไฟล์
        }
    }
    public class BookingSystem
    {
        private List<Market> markets43 = new List<Market>();
        private List<Market> markets45 = new List<Market>();
        private List<Market> markets46 = new List<Market>();
        private List<Market> marketsMain = new List<Market>();
        private int totalRows;
        private int totalRows43;
        private int totalRows45;
        private int totalRows46;

        public BookingSystem()
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
            var db = new SaveoneKoratMarketContext();
            var loge43 = db.LogeTempMasters
                .Include(x => x.Loge)
                .ThenInclude(l => l.LogeGroup)
                .Where(x => x.Loge.LogeGroup.SubZoneId == 43 && x.OpenCase == openCase)
                .OrderBy(x => x.Loge.LogeGroup.GroupSeqNo)
                .ToList();
            var loge45 = db.LogeTempMasters
                    .Include(x => x.Loge)
                    .ThenInclude(l => l.LogeGroup)
                    .Where(x => x.Loge.LogeGroup.SubZoneId == 45 && x.OpenCase == openCase)
                    .OrderBy(x => x.Loge.LogeGroup.GroupSeqNo)
                    .ToList();
            var loge46 = db.LogeTempMasters.Include(x => x.Loge)
                    .ThenInclude(l => l.LogeGroup)
                    .Where(x => x.Loge.LogeGroup.SubZoneId == 46 && x.OpenCase == openCase)
                    .OrderBy(x => x.Loge.LogeGroup.GroupSeqNo)
                    .ToList();
            if (loge43 != null) 
            {
                var groupIndexes = loge43.Select((item, index) => new { item, index }).GroupBy(x => x.item.Loge.LogeGroup.GroupSeqNo).Select(g => new
                {
                    GroupSeqNo = g.Key,
                    FirstIndex = g.First().index,
                    LastIndex = g.Last().index
                }).ToList();
                int index = 0;
                foreach (var item in loge43)
                {
                    var seqNum = item.Loge.LogeGroup.GroupSeqNo;
                    var name = item.LogeName;
                    var zone = item.Loge.LogeGroup.SubZoneId;
                    var isReserve = item.Status;
                    bool isCorner = groupIndexes.Any(g => g.FirstIndex == index || g.LastIndex == index);
                    markets43.Add(new Market
                    {
                        Row = seqNum,
                        IsCorner = isCorner,
                        LogeName = name,
                        LogeIndex = index,
                        IsReserve = isReserve,
                        LogeID = item.LogeId,
                    });
                    index += 1;
                }
                var maxGroupSeqNo = loge43.Select(x => x.Loge.LogeGroup.GroupSeqNo).Max();
                totalRows43 = maxGroupSeqNo;
                foreach (var item in markets43)
                {
                    Console.WriteLine($"Row {item.Row}: {item.LogeName} {(item.IsCorner ? " (Corner)" : "")} IsRS {item.IsReserve}");
                }
            }
            if (loge45 != null)
            {
                var groupIndexes = loge45.Select((item, index) => new { item, index }).GroupBy(x => x.item.Loge.LogeGroup.GroupSeqNo).Select(g => new
                {
                    GroupSeqNo = g.Key,
                    FirstIndex = g.First().index,
                    LastIndex = g.Last().index
                }).ToList();
                int index = 0;
                foreach (var item in loge45)
                {
                    var seqNum = item.Loge.LogeGroup.GroupSeqNo;
                    var name = item.LogeName;
                    var zone = item.Loge.LogeGroup.SubZoneId;
                    var isReserve = item.Status;
                    bool isCorner = groupIndexes.Any(g => g.FirstIndex == index || g.LastIndex == index);
                    markets45.Add(new Market 
                    { 
                        Row = seqNum, 
                        IsCorner = isCorner, 
                        LogeName = name, 
                        LogeIndex = index , 
                        IsReserve = isReserve, 
                        LogeID = item.LogeId,
                    });
                    index += 1;
                }
                var maxGroupSeqNo = loge45.Select(x => x.Loge.LogeGroup.GroupSeqNo).Max();
                totalRows45 = maxGroupSeqNo;
                foreach (var item in markets45)
                {
                    Console.WriteLine($"Row {item.Row}: {item.LogeName} {(item.IsCorner ? " (Corner)" : "")} IsRS {item.IsReserve}");
                }
            }
            if (loge46 != null)
            {
                var groupIndexes = loge46.Select((item, index) => new { item, index }).GroupBy(x => x.item.Loge.LogeGroup.GroupSeqNo).Select(g => new
                {
                    GroupSeqNo = g.Key,
                    FirstIndex = g.First().index,
                    LastIndex = g.Last().index
                }).ToList();
                int index = 0;
                foreach (var item in loge46)
                {
                    var seqNum = item.Loge.LogeGroup.GroupSeqNo;
                    var name = item.LogeName;
                    var zone = item.Loge.LogeGroup.SubZoneId;
                    var isReserve = item.Status;
                    bool isCorner = groupIndexes.Any(g => g.FirstIndex == index || g.LastIndex == index);
                    markets46.Add(new Market
                    {
                        Row = seqNum,
                        IsCorner = isCorner,
                        LogeName = name,
                        LogeIndex = index,
                        IsReserve = isReserve,
                        LogeID = item.LogeId,
                    });
                    index += 1;
                }
                var maxGroupSeqNo = loge46.Select(x => x.Loge.LogeGroup.GroupSeqNo).Max();
                totalRows46 = maxGroupSeqNo;
                //foreach (var item in markets46)
                //{
                //    Console.WriteLine($"Row {item.Row}: {item.LogeName} {(item.IsCorner ? " (Corner)" : "")}");
                //}
            }
            

        }

        public void ShowAllLogs()
        {
            //var groupedLogs = marketsMain.GroupBy(m => m.LogID / 100).OrderBy(g => g.Key);
            //foreach (var row in groupedLogs)
            //{
            //    Console.WriteLine($"Row {row.Key}: {string.Join(", ", row.Select(m => m.LogID + (m.IsCorner ? " (Corner)" : "")))}");
            //}
            //foreach (var item in marketsMain)
            //{
            //    Console.WriteLine($"Row {item.Row}: {item.LogeName} {(item.IsCorner ? " (Corner)" : "")}");
            //}
        }

        public void ReserveLogs(List<UserModel> users)
        {
            Random random = new Random();
            var shuffledUsers = users.OrderBy(x => random.Next()).ToList();
            int currentRow = 1;
            int currentRow43 = 1;
            int currentRow45 = 1;
            int currentRow46 = 1;
            foreach (var user in shuffledUsers)
            {
                if (user.UserStatus == 1) continue;
                if (user.zone == 43)
                {
                    marketsMain = markets43;
                    totalRows = totalRows43;
                    currentRow = currentRow43;
                }
                else if (user.zone == 45)
                {
                    marketsMain = markets45;
                    totalRows = totalRows45;
                    currentRow = currentRow45;
                }
                else if (user.zone == 46)
                {
                    marketsMain = markets46;
                    totalRows = totalRows46;
                    currentRow = currentRow46;
                }
                else
                {
                    Console.WriteLine($"UserID {user.UserID} zone not found");
                    continue;
                }
                if (ReserveLogsForUserInRow(user, user.LogNum, currentRow))
                {
                    currentRow += 1;
                    if (user.zone == 43) currentRow43 = currentRow;
                    else if (user.zone == 45) currentRow45 = currentRow;
                    else if (user.zone == 46) currentRow46 = currentRow;
                }
                else
                {
                    for (int i = 1; i <= totalRows; i++)
                    {
                        if (ReserveLogsForUserInRow(user, user.LogNum, i))
                        {
                            currentRow += 1;
                            if (user.zone == 43) currentRow43 = currentRow;
                            else if (user.zone == 45) currentRow45 = currentRow;
                            else if (user.zone == 46) currentRow46 = currentRow;
                            break;
                        }
                    }
                }
                if (currentRow > totalRows)
                {
                    currentRow = 1;
                    if (user.zone == 43) currentRow43 = currentRow;
                    else if (user.zone == 45) currentRow45 = currentRow;
                    else if (user.zone == 46) currentRow46 = currentRow;
                }
            }
        }

        private bool ReserveLogsForUserInRow(UserModel user, int logCount, int row)
        {
            if (logCount < 1 || logCount > 3)
            {
                Console.WriteLine($"UserID {user.UserID} Must <= 3 log");
                return false;
            }

            var availableLogs = marketsMain
                .Where(m => m.IsReserve == 1 && m.Row == row)
                .Select(m => m.LogeIndex)
                .ToList();
            if (availableLogs.Count>0)
            {
                var selectedLogs = GetFirstConsecutiveLogs(availableLogs, logCount);
                if (selectedLogs != null)
                {
                    var names = marketsMain.Where(m => selectedLogs.Contains(m.LogeIndex)).Select(m => m.LogeName).ToList();
                    user.UserLogIDs.AddRange(selectedLogs);
                    user.UserLogNames.AddRange(names);
                    MarkLogsAsReserved(selectedLogs, user.zone);
                    user.UserStatus = 1;
                    Console.WriteLine($"UserID {user.UserID} RS {logCount} log SUC...: {string.Join(", ", selectedLogs)}");
                    return true;
                }
                Console.WriteLine($"UserID {user.UserID} Can't RS on Row {row}");
                return false;
            }
            else
            {
                return false;
            }

        }

        private List<int> GetFirstConsecutiveLogs(List<int> availableLogs, int logCount)
        {
            for (int i = 0; i <= availableLogs.Count - logCount; i++)
            {
                var subset = availableLogs.Skip(i).Take(logCount).ToList();
                if (subset.Count==logCount && IsConsecutive(subset) && CountCornerLogs(subset) <= 1)
                {
                    return subset;
                }
            }
            return null;
        }

        private bool IsConsecutive(List<int> logIDs)
        {
            logIDs.Sort();
            for (int i = 1; i < logIDs.Count; i++)
            {
                if (logIDs[i] != logIDs[i - 1] + 1) return false;
            }
            return true;
        }

        private int CountCornerLogs(List<int> logIDs)
        {
            return logIDs.Count(logID => marketsMain.First(m => m.LogeIndex == logID).IsCorner);
        }

        private void MarkLogsAsReserved(List<int> logIDs , int zone)
        {
            int zoneID = zone;
            foreach (var logID in logIDs)
            {
                UpdateLogs(zoneID, logID);
            }
        }

        public void UpdateLogs(int zone , int id)
        {
            if (zone == 43) {
                marketsMain.First(m => m.LogeIndex == id).IsReserve = 0;
                markets43.First(m => m.LogeIndex == id).IsReserve = 0;
            }
            else if (zone == 45) 
            { 
                marketsMain.First(m => m.LogeIndex == id).IsReserve = 0;
                markets45.First(m => m.LogeIndex == id).IsReserve = 0;
            }  
            else if (zone == 46)
            {
                marketsMain.First(m => m.LogeIndex == id).IsReserve = 0;
                markets46.First(m => m.LogeIndex == id).IsReserve = 0;
            }
        }

        public void UpdateLogsToDB()
        {
            //var combinedMarkets = markets43.Where(m => m.IsReserve == 0)
            //    .Concat(markets45.Where(m => m.IsReserve == 0))
            //    .Concat(markets46.Where(m => m.IsReserve == 0))
            //    .ToList();
            //foreach (var market in combinedMarkets)
            //{
            //    Console.WriteLine($"LogID: {market.LogeIndex}, LogeName: {market.LogeName}, IsReserve: {market.IsReserve}");
            //}
            //using (var db = new SaveoneKoratMarketContext())
            //{
            //    // ดึง LogeId จาก combinedMarkets
            //    var logeIdsToUpdate = combinedMarkets.Select(m => m.LogeID).ToList();

            //    // ค้นหา LogeTempOffline ที่มี LogeId ตรงกับ combinedMarkets และ Status == 0
            //    var logeTempOfflinesToUpdate = db.LogeTempOfflines.Where(lto => logeIdsToUpdate.Contains(lto.LogeId) && lto.Status == 0).ToList();

            //    // อัปเดตค่า Status เป็น 1
            //    foreach (var logeTempOffline in logeTempOfflinesToUpdate)
            //    {
            //        logeTempOffline.Status = 1;
            //    }

            //    // บันทึกการเปลี่ยนแปลงลงในฐานข้อมูล
            //    db.SaveChanges();
            //}
            //var unreservedLogs = marketsMain.Where(m => m.IsReserve == 1).Select(m => m.LogID).ToList();
            //Console.WriteLine($"Log No RS : {string.Join(", ", unreservedLogs)}");
        }

        public void ShowAllUsers(List<UserModel> users)
        {
            foreach (var user in users)
            {
                Console.WriteLine($"UserID: {user.UserID}, Logs Reserved: {string.Join(", ", user.UserLogIDs)}");
            }
        }
    }
}
