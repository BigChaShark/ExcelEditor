using ExcelEditor.Models;
using static IndexModel;

namespace ExcelEditor.Pages
{
    public class ReservationManager
    {
        public static void createReservationLoge(UserModel member)
        {
            using (var context = new SaveoneKoratMarketContext())
            {
                DateTime currentDate = DateTime.Now;
                long memberId = context.Members.Where(x => x.Mobile == member.Mobile).Select(s => s.Id).FirstOrDefault();
                string memberCode = context.Members.Where(x => x.Mobile == member.Mobile).Select(s => s.Code).FirstOrDefault();
                int reservationLogeStatus = 2;
                int reservationstatus = 1;
                int zoneId = member.Zone;
                int subZoneId = member.SubZone;
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        string tranCode = memberCode + currentDate.ToString("yyyyMMddHHmmss");
                        var checkreservation = context.ReservationLoges.Where(x => x.MemberId == memberId && x.Status == reservationLogeStatus
                        && x.CreateDate.Year == currentDate.Year && x.CreateDate.Month == currentDate.Month && x.CreateDate.Day == currentDate.Day
                        && x.ZoneId == zoneId && (x.SubZoneId == subZoneId)
                        ).FirstOrDefault();
                        if (checkreservation == null)
                        {
                            ReservationLoge newReservationLoge = new ReservationLoge()
                            {
                                CreateDate = currentDate,
                                DiscountAmount = 0,
                                ElectricityAmount = member.ElectricityAmount,
                                ElectronicAmount = member.ElectronicAmount,
                                FineAmount = 0,
                                FullAreaAmount = 0,
                                LogeAmount = member.LogeAmount,
                                LogeName = string.Join(":", member.UserLogNames),
                                LogeQty = (int)member.LogNum,
                                MemberId = memberId,
                                Status = reservationLogeStatus,
                                ZoneId = zoneId,
                                SubZoneId = subZoneId,
                                ReservationStatus = reservationstatus
                            };
                            context.ReservationLoges.Add(newReservationLoge);
                            context.SaveChanges();
                            dbContextTransaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Error creating reservation loge: " + ex.Message);
                    }
                }
            }
        }
        public static void createReservationLogeDetail(UserModel member)
        {
            using (var context = new SaveoneKoratMarketContext())
            {
                DateTime currentDate = DateTime.Now;
                long memberId = context.Members.Where(x => x.Mobile == member.Mobile).Select(s => s.Id).FirstOrDefault();
                int reservationLogeStatus = 2;
                int zoneId = member.Zone;
                int subZoneId = member.SubZone;
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var checkreservation = context.ReservationLoges.Where(x => x.MemberId == memberId && x.Status == reservationLogeStatus
                        && x.CreateDate.Year == currentDate.Year && x.CreateDate.Month == currentDate.Month && x.CreateDate.Day == currentDate.Day
                        && x.ZoneId == zoneId && (x.SubZoneId == subZoneId)
                        ).FirstOrDefault();
                        if (checkreservation != null)
                        {
                            foreach (var logeId in member.UserLogIDs)
                            {
                                ReservationLogeDetail newReservationLogeDetail = new ReservationLogeDetail()
                                {
                                    ReservationLogeId = checkreservation.Id,
                                    LogeId = logeId,
                                    ReservationDate = long.Parse(currentDate.ToString("yyyyMMdd")),
                                    TimeStamp = currentDate
                                };
                                context.ReservationLogeDetails.Add(newReservationLogeDetail);
                                context.SaveChanges();
                                dbContextTransaction.Commit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Error creating reservation loge detail: " + ex.Message);
                    }
                }
            }
        }
    }
}
