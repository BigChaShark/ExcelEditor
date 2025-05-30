﻿using ExcelEditor.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using static IndexModel;

namespace ExcelEditor.Pages
{
    public static class ExcelManager
    {
        #region Read Excels
        public static List<UserModel> ReadUsersFromExcel(string filePath)
        {
            DateTime currentDate = DateTime.Now;
            var db = new SaveoneKoratMarketContext();
            var usersTemp = db.UserOfflines.Where(x => x.CreateDate == DateOnly.FromDateTime(currentDate)).Select(x => new { x.UserOfflineId, x.Status }).ToList();
            var users = new List<UserModel>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var allWorksheets = package.Workbook.Worksheets.ToList();
                for (int sheetIndex = 0; sheetIndex < allWorksheets.Count; sheetIndex++)
                {
                    var sheet = allWorksheets[sheetIndex];
                    int rowCount = sheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        if (string.IsNullOrEmpty(sheet.Cells[row, 3].Text) || string.IsNullOrEmpty(sheet.Cells[row, 4].Text))
                            continue;
                        var user = new UserModel();
                        int subZoneID = GetLogZone.GetLogzone(sheet.Cells[row, 4].Text);
                        int zoneID = GetLogZone.GetMarketZone(sheet.Cells[row, 4].Text);
                        var matchInDB = usersTemp.FirstOrDefault(x => x.UserOfflineId == sheet.Cells[row, 3].Text + zoneID);
                        if (matchInDB != null && matchInDB.Status == 1)
                        {
                            user.UserStatus = matchInDB.Status;
                            user.UserLogNames.Add("Already RS Today");
                            user.UserLogIDs.Add(0);
                        }
                        else
                        {
                            user.UserStatus = 0;
                        }
                        var matchInSheet = users.FirstOrDefault(x => x.UserOfflineID == sheet.Cells[row, 3].Text + zoneID);
                        if (matchInSheet != null)
                        {
                            continue;
                        }
                        user.UserOfflineID = sheet.Cells[row, 3].Text + zoneID;
                        user.Mobile = sheet.Cells[row, 3].Text;
                        user.Zone = zoneID;
                        user.SubZone = subZoneID;
                        user.UserName = sheet.Cells[row, 2].Text;
                        user.SheetIndex = sheetIndex + 1; // เริ่มจาก 1 แทน 0
                        user.CreatDate = DateOnly.FromDateTime(currentDate);
                        user.CreatDateTime = currentDate;
                        if (int.TryParse(sheet.Cells[row, 5].Text, out int logNum))
                            user.LogNum = logNum;
                        if (int.TryParse(sheet.Cells[row, 6].Text == null ? "0" : sheet.Cells[row, 6].Text, out int fullLogeQty))
                            user.FullLogeQty = fullLogeQty;
                        if (int.TryParse(sheet.Cells[row, 7].Text == null ? "0" : sheet.Cells[row, 7].Text, out int electricityQty))
                            user.ElectricityQty = electricityQty;
                        if (int.TryParse(sheet.Cells[row, 8].Text == null ? "0" : sheet.Cells[row, 8].Text, out int electronicQty))
                            user.ElectronicQty = electronicQty;
                        users.Add(user);
                    }
                }

            }

            return users;
        }



        public static List<UserSummaryModel> ReadUploadedSummary(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var users = new List<UserSummaryModel>();
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        users.Add(new UserSummaryModel
                        {
                            UserName = worksheet.Cells[row, 2].Text,
                            Mobile = worksheet.Cells[row, 3].Text,
                            Message = worksheet.Cells[row, 4].Text
                        });
                    }
                }
            }
            return users;
        }

        public static List<UserModel> ReadUploadedTransection(IFormFile file)
        {
            DateTime currentDate = DateTime.Now;
            var db = new SaveoneKoratMarketContext();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var usersTemp = db.UserOfflines.Where(x => x.CreateDate == DateOnly.FromDateTime(currentDate)).Select(x => new { x.UserOfflineId, x.Status }).ToList();
            var users = new List<UserModel>();
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var allWorksheets = package.Workbook.Worksheets.ToList();
                    for (int sheetIndex = 0; sheetIndex < allWorksheets.Count; sheetIndex++)
                    {
                        var sheet = allWorksheets[sheetIndex];
                        int rowCount = sheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (string.IsNullOrEmpty(sheet.Cells[row, 3].Text) || string.IsNullOrEmpty(sheet.Cells[row, 4].Text))
                                continue;
                            var user = new UserModel();
                            int subZoneID = GetLogZone.GetLogzone(sheet.Cells[row, 4].Text);
                            int zoneID = GetLogZone.GetMarketZone(sheet.Cells[row, 4].Text);
                            var matchInSheet = users.FirstOrDefault(x => x.UserOfflineID == sheet.Cells[row, 3].Text + zoneID);
                            if (matchInSheet != null)
                            {
                                continue;
                            }
                            user.UserOfflineID = sheet.Cells[row, 3].Text + zoneID;
                            user.Mobile = sheet.Cells[row, 3].Text;
                            user.Zone = zoneID;
                            user.SubZone = subZoneID;
                            user.UserName = sheet.Cells[row, 2].Text;
                            user.SheetIndex = sheetIndex + 1;
                            user.CreatDate = DateOnly.FromDateTime(currentDate);
                            user.CreatDateTime = currentDate;
                            if (int.TryParse(sheet.Cells[row, 5].Text, out int logNum))
                                user.LogNum = logNum;
                            if (int.TryParse(sheet.Cells[row, 6].Text == null ? "0" : sheet.Cells[row, 6].Text, out int fullLogeQty))
                                user.FullLogeQty = fullLogeQty;
                            if (int.TryParse(sheet.Cells[row, 7].Text == null ? "0" : sheet.Cells[row, 7].Text, out int electricityQty))
                                user.ElectricityQty = electricityQty;
                            if (int.TryParse(sheet.Cells[row, 8].Text == null ? "0" : sheet.Cells[row, 8].Text, out int electronicQty))
                                user.ElectronicQty = electronicQty;
                            user.UserLogNames = sheet.Cells[row, 9].Text.Split(',').ToList();
                            user.UserLogIDs = sheet.Cells[row, 10].Text.Split(',').Select(int.Parse).ToList();
                            user.ElectricityID = int.TryParse(sheet.Cells[row, 13].Text, out int electricityID) ? electricityID : 0;
                            user.ElectronicID = int.TryParse(sheet.Cells[row, 14].Text, out int electronicID) ? electronicID : 0;
                            user.LogeAmount = decimal.TryParse(sheet.Cells[row, 15].Text, out decimal logeAmount) ? logeAmount : 0;
                            user.ElectricityAmount = decimal.TryParse(sheet.Cells[row, 16].Text, out decimal electricityAmount) ? electricityAmount : 0;
                            user.ElectronicAmount = decimal.TryParse(sheet.Cells[row, 17].Text, out decimal electronicAmount) ? electronicAmount : 0;
                            user.TotalAmount = decimal.TryParse(sheet.Cells[row, 18].Text, out decimal totalAmount) ? totalAmount : 0;
                            user.CreatDate = DateOnly.TryParse(sheet.Cells[row, 19].Text, out DateOnly creatDate) ? creatDate : DateOnly.FromDateTime(currentDate);
                            user.CreatDateTime = DateTime.TryParse(sheet.Cells[row, 20].Text, out DateTime creatDateTime) ? creatDateTime : currentDate;
                            users.Add(user);
                        }
                    }
                }
            }
            return users;
        }
        #endregion

        #region Fill Data to Excel
        public static void FillUserLogIDsFromLogStore(string filePath, List<UserModel> x)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var allWorksheets = package.Workbook.Worksheets.ToList();

                foreach (var sheet in allWorksheets)
                {
                    sheet.Cells[1, 9].Value = "LogIDs";
                    sheet.Cells[1, 10].Value = "LogNames";
                    sheet.Cells[1, 11].Value = "ZoneID";
                    sheet.Cells[1, 12].Value = "SubZoneID";
                    sheet.Cells[1, 13].Value = "ElectricityID";
                    sheet.Cells[1, 14].Value = "ElectronicID";
                    sheet.Cells[1, 15].Value = "LogeAmount";
                    sheet.Cells[1, 16].Value = "ElectricityAmount";
                    sheet.Cells[1, 17].Value = "ElectronicAmount";
                    sheet.Cells[1, 18].Value = "TotalAmount";
                    sheet.Cells[1, 19].Value = "CreatDate";
                    sheet.Cells[1, 20].Value = "CreatDateTime";
                    var rowCount = sheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        if (string.IsNullOrEmpty(sheet.Cells[row, 3].Text))
                            continue;

                        int marketID = GetLogZone.GetMarketZone(sheet.Cells[row, 4].Text);
                        string id = sheet.Cells[row, 3].Text + marketID;

                        var matched = x.FirstOrDefault(x => x.UserOfflineID == id);

                        if (matched != null && matched.UserLogIDs.Any())
                        {
                            string userLogNamesString = string.Join(",", matched.UserLogNames);
                            string userLogIDsString = string.Join(",", matched.UserLogIDs);
                            sheet.Cells[row, 9].Value = userLogIDsString;
                            sheet.Cells[row, 10].Value = userLogNamesString;
                            sheet.Cells[row, 11].Value = matched.Zone;
                            sheet.Cells[row, 12].Value = matched.SubZone;
                            sheet.Cells[row, 13].Value = matched.ElectricityID;
                            sheet.Cells[row, 14].Value = matched.ElectronicID;
                            sheet.Cells[row, 15].Value = matched.LogeAmount;
                            sheet.Cells[row, 16].Value = matched.ElectricityAmount;
                            sheet.Cells[row, 17].Value = matched.ElectronicAmount;
                            sheet.Cells[row, 18].Value = matched.TotalAmount;
                            sheet.Cells[row, 19].Value = matched.CreatDate;
                            sheet.Cells[row, 20].Value = matched.CreatDateTime;
                        }
                    }
                }
                package.Save();
            }
        }
        #endregion

    }
}
