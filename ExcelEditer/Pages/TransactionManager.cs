using ExcelEditor.Models;
using static IndexModel;

namespace ExcelEditor.Pages
{
    public class TransactionManager
    {
        void createTransactionId(UserModel member, DateTime currentDate)
        {
            //Test 0981565947 must chk with status if == 1
            var db = new SaveoneKoratMarketContext();
            //int userId = 113; // Auto
            int reservationLogeStatus = 2; 
            int zoneId = member.Zone;
            int subZoneId = member.SubZone;
            int[] zone = { 43, 45 , 46 , 49 , 50 };
            //GoBKK 43 45 46 49 50 // Gate 14 Type 21
            int PaymentGatewayId = zone.Contains(subZoneId) ? 14 : 9; // Default => 13 SCB QrCode  // PaymentGateway.Id => 14	, PaymentGateway.Description => GoMoney Wallet
            int PaymentTypeId = zone.Contains(subZoneId) ? 21 : 11; // Default => 18 SCB QrCode || 21 GoMoney Wallet
            //Other GatewayId 9 TypeId 11
            long memberId = db.Members.Where(x => x.Mobile == member.Mobile).Select(s => s.Id).FirstOrDefault();
            string memberCode = db.Members.Where(x => x.Mobile == member.Mobile).Select(s => s.Code).FirstOrDefault();
            using (var context = new SaveoneKoratMarketContext())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        string tranCode = memberCode + currentDate.ToString("yyyyMMddHHmmss");
                        var reservation = context.ReservationLoges.Where(x => x.MemberId == memberId && x.Status == reservationLogeStatus
                        && x.CreateDate.Year == currentDate.Year && x.CreateDate.Month == currentDate.Month && x.CreateDate.Day == currentDate.Day
                        && x.ZoneId == zoneId && (x.SubZoneId == subZoneId)
                        ).FirstOrDefault();
                        if (reservation != null)
                        {
                            Transaction newTransaction = new Transaction()
                            {
                                AmountToPay = member.TotalAmount,
                                CreateDate = member.CreatDateTime,
                                MemberId = memberId,
                                PaymentDate = null,
                                PaymentEndPointUrl = null,
                                PaymentGatewayId = PaymentGatewayId, // Default => 13 SCB QrCode || 14 GoMoney Wallet
                                PaymentReferenceCode = null,
                                PaymentResponseCode = null,
                                PaymentResponseMessage = null,
                                PaymentTypeId = PaymentTypeId, // Default => 18 SCB QrCode || 21 GoMoney Wallet
                                RcptNo = null,
                                ReservationLogeElectricityTypeId = member.ElectricityID,
                                ReservationLogeElectronicTypeId = member.ElectronicID,
                                ReservationLogeTypeId = int.Parse(member.LogNum.ToString() + member.FullLogeQty.ToString()),
                                ReservationLogeId = reservation.Id,
                                ReservationsDate = null,
                                TransactionStatusId = 3,
                                TranCode = tranCode,
                                LastUpdate = currentDate,
                                ReservationSubZoneId = subZoneId,
                                TransactionSubZoneId = subZoneId,
                                BypassPaymentStatusId = 1,
                                IsGuest = 0, // always
                                IsChangeZone = 0,
                            };

                            context.Transactions.Add(newTransaction);
                            context.SaveChanges();
                            dbContextTransaction.Commit();
                        }
                    }
                    catch (Exception err)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Error creating reservation loge: " + err.Message);
                    }
                }
            }         
        }
    }
}
