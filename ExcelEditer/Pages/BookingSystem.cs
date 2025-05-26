using ExcelEditor.Models;
using static IndexModel;
using System.Text.RegularExpressions;
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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Expressions;

namespace ExcelEditor.Pages
{
    public class BookingSystem
    {
        private List<LogeModel> logeMain = new List<LogeModel>();
        private int totalRows;
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
            var loge = db.LogeTempOfflines
                .Include(x => x.Loge)
                .ThenInclude(l => l.LogeGroup)
                .Where(x => x.OpenDateInt == DateOnly.FromDateTime(nextDate))
                .OrderBy(x => x.Loge.LogeGroup.SubZoneId).ThenBy(x => x.Loge.LogeGroup.GroupSeqNo).ThenBy(x => x.LogeIndex)
                .ToList();
            if (loge != null)
            {
                for (int i = 0; i < loge.Count; i++)
                {
                    var seqNum = loge[i].Loge.LogeGroup.GroupSeqNo;
                    var name = loge[i].LogeName;
                    var zone = loge[i].Loge.LogeGroup.SubZoneId;
                    var isReserve = loge[i].Status;
                    var p = (i > 0) ? loge[i - 1].Loge.LogeGroup.GroupSeqNo : -99;
                    var n = (i < loge.Count - 1) ? loge[i + 1].Loge.LogeGroup.GroupSeqNo : -99;
                    bool isCorner = p == n ? false : true;
                    logeMain.Add(new LogeModel
                    {
                        Row = seqNum,
                        IsCorner = isCorner,
                        LogeName = name,
                        MaxRow = loge.Where(x => x.Loge.LogeGroup.SubZoneId == zone).Max(x => x.Loge.LogeGroup.GroupSeqNo),
                        IsReserve = isReserve,
                        LogeID = loge[i].LogeId,
                        LogeSeqNum = seqNum,
                        LogeZone = zone ?? 0,
                        LogeIndex = loge[i].LogeIndex,
                        Column = splitInt(name),
                        OpenDateInt = loge[i].OpenDateInt,
                    });
                }
                int splitInt(string name)
                {
                    var match = Regex.Match(name, @"\d+");
                    if (match.Success)
                    {
                        return int.Parse(match.Value);
                    }
                    return 0;
                }
                //int index = 1;
                //foreach (var item in logeMain)
                //{
                //    Console.WriteLine($"{index} : C {item.Column}:{item.LogeName} {(item.IsCorner ? " (Corner)" : "")} IsZone : {item.LogeZone} LogID : {item.LogeID}");
                //    index++;
                //}
                //var loge49 = logeMain.Where(x => x.LogeZone == 49).ToList();
                //int index2 = 1;
                //foreach (var item in loge49)
                //{
                //    Console.WriteLine($"CH {index2} : Row {item.Row}:{item.LogeName} {(item.IsCorner ? " (Corner)" : "")} IsZone : {item.LogeZone} LogID : {item.LogeID}");
                //    index2++;
                //}
            }
        }
        public void ReserveLogs(List<UserModel> users)
        {
            DateTime currentDate = DateTime.Now;
            Random random = new Random();
            //var shuffledUsers = users.OrderByDescending(m => m.LogNum).ThenBy(x => random.Next()).ToList();
            var shuffledUsers = users.OrderBy(m => m.SheetIndex).ThenBy(x => random.Next()).ToList();
            int currentRow = 0;
            foreach (var user in shuffledUsers)
            {
                var logeTemp = logeMain;
                if (user.UserStatus == 1) continue;
                //if (currentRows.ContainsKey(user.zone))
                //{
                //    currentRow = currentRows[user.zone];
                //totalRows = logeMain.Where(x => x.LogeZone == user.zone).Max(m => m.MaxRow);
                //}
                //else
                //{
                //    Console.WriteLine($"Zone not found at User ID : {user.UserID}");
                //}
                
                if (user.SubZone == 49)
                {
                    if (currentDate.TimeOfDay < new TimeSpan(0, 50, 0))
                    {
                        logeTemp = logeTemp.Where(m => m.LogeSeqNum <= 5 && m.IsReserve == 0 && m.LogeZone == user.SubZone).OrderBy(o => o.LogeIndex).ToList();
                    }
                    if (currentDate.TimeOfDay >= new TimeSpan(0, 50, 0) && currentDate.TimeOfDay < new TimeSpan(0, 52, 0))
                    {
                        var logeZone1 = logeTemp.Where(m => m.LogeSeqNum <= 5 && m.IsReserve == 0 && m.LogeZone == user.SubZone).OrderBy(o => o.LogeIndex).ToList();
                        var logeZone2 = logeTemp.Where(m => m.LogeSeqNum > 5 && m.IsReserve == 0 && m.LogeZone == user.SubZone).OrderBy(o => o.LogeIndex).ToList();
                        logeTemp = logeZone1.Concat(logeZone2).ToList();
                    }
                    if (currentDate.TimeOfDay >= new TimeSpan(0, 52, 0))
                    {
                        logeTemp = logeTemp.Where(m => m.LogeSeqNum > 5 && m.IsReserve == 0 && m.LogeZone == user.SubZone).OrderBy(o => o.LogeIndex).ToList().ToList();
                    }
                }
                else
                {
                    logeTemp = logeTemp.Where(m => m.IsReserve == 0 && m.LogeZone == user.SubZone).OrderBy(o => o.LogeIndex).ToList();
                }
                //var logTest = logeTemp.Select(s => s.LogeName).ToList();
                //Console.WriteLine($"{string.Join(",", logTest)}");
                //totalRows = logeTemp.Where(m => m.IsReserve == 0 && m.LogeZone == user.SubZone).Select(m => (int?)m.LogeIndex).Max() ?? 0;
                var indexNow = logeTemp.FirstOrDefault();
                //Console.WriteLine($"Now Index : {indexNow.LogeIndex}");
                //currentRow = logeMain.Where(m => m.IsReserve == 0 && m.LogeZone == user.zone).OrderBy(m => m.LogeIndex).Select(m => m.Row).FirstOrDefault();
                //currentRow = indexNow != null ? indexNow.Row : 1;
                if (indexNow == null)
                {
                    Console.WriteLine($"UserID {user.UserOfflineID} No available loge");
                    continue;
                }
                for (int i = 0; i <= logeTemp.Count - 1; i++)
                {
                    //currentRow = logeTemp[i].Row;
                    if (ReserveLogsForUserInRow(user, logeTemp[i]))
                    {
                        //currentRow += 1;
                        //SetRow(user.zone);
                        break;
                    }
                }
                //if (ReserveLogsForUserInRow(user, currentRow))
                //{
                //    //currentRow += 1;
                //    //SetRow(user.zone);
                //}
                //else
                //{
                //    Console.WriteLine("In loop");
                //    if (indexNow == null)
                //    {
                //        Console.WriteLine($"UserID {user.UserOfflineID} No available loge");
                //        continue;
                //    }
                //    for (int i = 0; i <= logeTemp.Count - 1; i++)
                //    {
                //        currentRow = logeTemp[i].Row;
                //        if (ReserveLogsForUserInRow(user, currentRow))
                //        {
                //            //currentRow += 1;
                //            //SetRow(user.zone);
                //            break;
                //        }
                //    }
            }
        }

        private bool ReserveLogsForUserInRow(UserModel user, LogeModel loge)
        {
            switch (user.Zone)
            {
                case 3:
                case 5:
                    if (user.LogNum < 1 || user.LogNum > 5)
                    {
                        Console.WriteLine($"UserID {user.UserOfflineID} loge Error");
                        return false;
                    }
                break;
                case 0:
                    Console.WriteLine($"UserID {user.UserOfflineID} loge Error");
                    return false;
                default:
                    if (user.LogNum < 1 || user.LogNum > 3)
                    {
                        Console.WriteLine($"UserID {user.UserOfflineID} loge Error");
                        return false;
                    }
                    break;
            }
            var logeTemp = logeMain;
            var availableLogs = new List<int>();
            if (user.SubZone==7)
            {
               availableLogs = logeTemp
               .Where(m => m.IsReserve == 0 && m.LogeZone == user.SubZone).OrderBy(m => m.LogeIndex)
               .Select(m => m.LogeIndex)
               .ToList();
            }
            else
            {
                availableLogs = logeTemp
                .Where(m => m.IsReserve == 0 && m.Row == loge.Row && m.LogeID >= loge.LogeID).OrderBy(m => m.LogeID)
                .Select(m => m.LogeID)
                .ToList();
            }
            
            if (availableLogs.Count > 0)
            {
                var selectedLogs = GetFirstConsecutiveLogs(availableLogs, user.LogNum, user.SubZone);
                if (selectedLogs != null)
                {
                    var names = new List<string>();
                    if (user.SubZone == 7)
                    {
                        names = logeMain.Where(m => selectedLogs.Contains(m.LogeIndex) && m.LogeZone == user.SubZone).OrderBy(o => o.LogeIndex).Select(m => m.LogeName).ToList();
                    }
                    else
                    {
                        names = logeMain.Where(m => selectedLogs.Contains(m.LogeID) && m.Row == loge.Row).Select(m => m.LogeName).ToList();
                    }
                    user.UserLogIDs.AddRange(selectedLogs);
                    user.UserLogNames.AddRange(names);
                    MarkLogeAsReserved(selectedLogs,user.SubZone);
                    PriceCalSystem.TotalPrice(user);
                    user.UserStatus = 1;
                    Console.WriteLine($"UserID {user.UserOfflineID} RS {user.LogNum} log SUC...: {string.Join(", ", names)}");
                    return true;
                }
                Console.WriteLine($"UserID {user.UserOfflineID} Can't RS on Row {loge.Row}");
                return false;
            }
            else
            {
                return false;
            }

        }
        private List<int> GetFirstConsecutiveLogs(List<int> availableLogs, int? logCount, int zone)
        {
            if (zone==7)
            {
                for (int i = 0; i <= availableLogs.Count - logCount; i++)
                {
                    if (logCount==2 && availableLogs[i] % 2 == 0)
                    {
                        continue;
                    }
                    var subset = availableLogs.Skip(i).Take((int)logCount).ToList();
                    if (subset.Count == logCount && IsConsecutive(subset))
                    {
                        return subset;
                    }
                }
                return null;
            }
            else
            {
                var subset = availableLogs.Take((int)logCount).ToList();
                if (subset.Count == logCount && IsConsecutive(subset))
                {
                    return subset;
                }
                return null;
                //for (int i = 0; i <= availableLogs.Count - logCount; i++)
                //{
                //    var subset = availableLogs.Skip(i).Take((int)logCount).ToList();
                //    if (subset.Count == logCount && IsConsecutive(subset))
                //    {
                //        return subset;
                //    }
                //}
                //return null;
            }

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
            return logIDs.Count(logID => logeMain.First(m => m.LogeID == logID).IsCorner);
        }

        private void MarkLogeAsReserved(List<int> logIDs , int zone)
        {
            foreach (var logID in logIDs)
            {
                if(zone == 7)
                {
                    logeMain.First(m => m.LogeIndex == logID && m.LogeZone == zone).IsReserve = 1;
                }
                else {
                    logeMain.First(m => m.LogeID == logID).IsReserve = 1;
                }
                    
            }
        }
        public void UpdateDB(List<UserModel> users)
        {
            UpdateLoges();
            UpdateUserOffline(users);
        }
        public void UpdateLoges()
        {
            using (var db = new SaveoneKoratMarketContext())
            {
                foreach (var loge in logeMain)
                {
                    var targetLoge = db.LogeTempOfflines.FirstOrDefault(x => x.LogeId == loge.LogeID && x.OpenDateInt == loge.OpenDateInt);
                    if (targetLoge != null)
                    {
                        targetLoge.Status = loge.IsReserve;
                    }
                }
                db.SaveChanges();
            }
        }
        public void UpdateUserOffline(List<UserModel> users)
        {
            using (var db = new SaveoneKoratMarketContext())
            {
                DateTime currentDate = DateTime.Now;
                var userCurrent = db.UserOfflines.Where(x => x.CreateDate == DateOnly.FromDateTime(currentDate));
                var userToAdd = new List<UserOffline>();
                foreach (var user in users)
                {
                    var isSame = userCurrent.FirstOrDefault(x => (x.UserOfflineId == user.UserOfflineID));
                    if (isSame == null)
                    {
                        var userOffline = new UserOffline
                        {
                            UserOfflineId = user.UserOfflineID,
                            Name = user.UserName,
                            Mobile = user.Mobile,
                            Status = (int)user.UserStatus,
                            LogeQty = (int)user.LogNum,
                            ZoneId = user.Zone,
                            SubZoneId = user.SubZone,
                            LogeName = string.Join(", ", user.UserLogNames),
                            LogeId = string.Join(", ", user.UserLogIDs),
                            LogeAmount = user.LogeAmount,
                            ElectricityAmount = user.ElectricityAmount,
                            ElectronicAmount = user.ElectronicAmount,
                            TotalAmount = user.TotalAmount,
                            CreateDate = DateOnly.FromDateTime(currentDate),
                            CreateDateTime = currentDate,
                            CreateBy = 113,
                        };
                        userToAdd.Add(userOffline);
                    }
                    else
                    {
                        if (isSame.Status == 0)
                        {
                            isSame.Status = (int)user.UserStatus;
                            isSame.LogeQty = (int)user.LogNum;
                            isSame.ZoneId = user.Zone;
                            isSame.SubZoneId = user.SubZone;
                            isSame.LogeName = string.Join(", ", user.UserLogNames);
                            isSame.LogeId = string.Join(", ", user.UserLogIDs);
                            db.SaveChanges();
                        }
                    }

                }
                if (userToAdd.Count > 0)
                {
                    db.UserOfflines.AddRange(userToAdd);
                    db.SaveChanges();
                }
                foreach (var user in userToAdd)
                {
                    Console.WriteLine($"UserID: {user.UserOfflineId}, Logs Reserved: {user.LogeName} , Status : {user.Status}");
                }
            }
        }
        public void ShowAllAvailableLogs()
        {
            //var allMarkets = logeMain.Where(x => x.IsReserve == 0).ToList();
            //foreach (var market in allMarkets)
            //{
            //    Console.WriteLine($"LogID: {market.LogeID}, LogName: {market.LogeName}, IsReserved: {market.IsReserve}");
            //}
        }
        public void ShowAllUsers(List<UserModel> users)
        {
            //foreach (var user in users)
            //{
            //    Console.WriteLine($"UserID: {user.UserID}, Logs Reserved: {string.Join(", ", user.UserLogNames)}");
            //}
        }
    }
}
