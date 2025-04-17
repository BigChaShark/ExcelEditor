using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using ExcelEditer;

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
    }

    public class Market
    {
        public int LogID { get; set; }
        public bool IsReserve { get; set; } = false;
        public bool IsCorner { get; set; } = false;
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

        BookingSystem bookingSystem = new BookingSystem(2, 2, 4);
        var users = ReadUsersFromExcel(originalFilePath);
        bookingSystem.ShowAllLogs();
        bookingSystem.ReserveLogs(users);
        bookingSystem.ShowUnreservedLogs();
        bookingSystem.ShowAllUsers(users);
        //ProcessExcelFile(originalFilePath);
        FillUserLogIDsFromLogStore(originalFilePath, users);
        //var newFileName = Path.GetFileNameWithoutExtension(UploadedFile.FileName) + "Success" + Path.GetExtension(UploadedFile.FileName);
        //var newFilePath = Path.Combine(uploadDir, newFileName);

        //System.IO.File.Move(originalFilePath, newFilePath);

        TempData["FilePath"] = originalFilePath;
        TempData["Success"] = true;

        return Page();
    }
    public IActionResult OnGetDownloadFile()
    {
        var filePath = TempData["FilePath"] as string;
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
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) // เริ่มจากแถวที่ 2  
            {
                var user = new UserModel();
                //if (int.TryParse(worksheet.Cells[row, 3].Text, out int userId))
                //    user.UserID = userId;
                user.UserID = worksheet.Cells[row, 3].Text;
                if (int.TryParse(worksheet.Cells[row, 5].Text, out int logNum))
                    user.LogNum = logNum;
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
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                string id = worksheet.Cells[row, 3].Text; // Name อยู่ column 5

                var matched = x.FirstOrDefault(x => x.UserID == id);

                if (matched != null && matched.UserLogIDs.Any())
                {
                    string userLogIDsString = string.Join(",", matched.UserLogIDs);
                    worksheet.Cells[row, 8].Value = userLogIDsString; // เติมใน column 8
                }
            }

            package.Save(); // เซฟไฟล์
        }
    }
    public class BookingSystem
    {
        private List<Market> markets = new List<Market>();
        private int logsPerRow;
        private int columnsPerRow;
        private int totalRows;

        public BookingSystem(int totalRows, int columnsPerRow, int logsPerColumn)
        {
            this.logsPerRow = columnsPerRow * logsPerColumn;
            this.columnsPerRow = columnsPerRow;
            this.totalRows = totalRows;
            for (int r = 1; r <= totalRows; r++)
            {
                for (int c = 1; c <= logsPerRow; c++)
                {
                    int logID = r * 100 + c;
                    bool isCorner = (c == 1 || c == logsPerRow || c == logsPerRow / columnsPerRow || c == logsPerRow / columnsPerRow * (columnsPerRow - 1) + 1);
                    markets.Add(new Market { LogID = logID, IsCorner = isCorner });
                }
            }
        }

        public void ShowAllLogs()
        {
            var groupedLogs = markets.GroupBy(m => m.LogID / 100).OrderBy(g => g.Key);
            foreach (var row in groupedLogs)
            {
                Console.WriteLine($"Row {row.Key}: {string.Join(", ", row.Select(m => m.LogID + (m.IsCorner ? " (Corner)" : "")))}");
            }
        }

        public void ReserveLogs(List<UserModel> users)
        {
            Random random = new Random();
            var shuffledUsers = users.OrderBy(x => random.Next()).ToList();
            int currentRow = 1;
            foreach (var user in shuffledUsers)
            {
                if (user.UserStatus == 1) continue;
                if (ReserveLogsForUserInRow(user, user.LogNum, currentRow))
                {
                    currentRow += 1;
                }
                else
                {
                    for (int i = 1; i <= totalRows; i++)
                    {
                        if (ReserveLogsForUserInRow(user, user.LogNum, i))
                        {
                            currentRow += 1;
                        }
                    }
                }
                if (currentRow > totalRows)
                {
                    currentRow = 1;
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

            var availableLogs = markets
                .Where(m => !m.IsReserve && (m.LogID / 100) == row)
                .Select(m => m.LogID)
                .ToList();

            var selectedLogs = GetFirstConsecutiveLogs(availableLogs, logCount);
            if (selectedLogs != null)
            {
                user.UserLogIDs.AddRange(selectedLogs);
                MarkLogsAsReserved(selectedLogs);
                user.UserStatus = 1;
                Console.WriteLine($"UserID {user.UserID} RS {logCount} log SUC...: {string.Join(", ", selectedLogs)}");
                return true;
            }
            Console.WriteLine($"UserID {user.UserID} Can't RS on Row {row}");
            return false;
        }

        private List<int> GetFirstConsecutiveLogs(List<int> availableLogs, int logCount)
        {
            for (int i = 0; i <= availableLogs.Count - logCount; i++)
            {
                var subset = availableLogs.Skip(i).Take(logCount).ToList();
                if (IsConsecutive(subset) && CountCornerLogs(subset) <= 1)
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
            return logIDs.Count(logID => markets.First(m => m.LogID == logID).IsCorner);
        }

        private void MarkLogsAsReserved(List<int> logIDs)
        {
            foreach (var logID in logIDs)
            {
                markets.First(m => m.LogID == logID).IsReserve = true;
            }
        }

        public void ShowUnreservedLogs()
        {
            var unreservedLogs = markets.Where(m => !m.IsReserve).Select(m => m.LogID).ToList();
            Console.WriteLine($"Log No RS : {string.Join(", ", unreservedLogs)}");
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
