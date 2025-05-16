using ExcelEditor.Models;
using static IndexModel;

namespace ExcelEditor.Pages
{
    public class TransactionManager
    {
        void createTransactionId(UserModel member, DateTime currentDate)
        {
            var db = new SaveoneKoratMarketContext();
            int userId = 113; // Auto
            #region Transaction Payment
            int PaymentGatewayId = 14; // Default => 13 SCB QrCode  // PaymentGateway.Id => 14	, PaymentGateway.Description => GoMoney Wallet
            int PaymentTypeId = 21; // Default => 18 SCB QrCode || 21 GoMoney Wallet

            long tranId = 0;
            #endregion
            int zoneId = member.Zone;
            int subZoneId = member.SubZone;
            long memberId = db.Members.Where(x => x.Mobile == member.Mobile).Select(s => s.Id).FirstOrDefault();
            string memberCode = db.Members.Where(x => x.Mobile == member.Mobile).Select(s => s.Code).FirstOrDefault();
            int isComplete = 0;
            string errormsg = "";
            using (var context = new SaveoneKoratMarketContext())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        string tranCode = memberCode + currentDate.ToString("yyyyMMddHHmmss");
                        var checkreservation = context.ReservationLoges.Where(x => x.MemberId == memberId /*&& x.Status == reservationLogeStatus*/
                        && x.CreateDate.Year == currentDate.Year && x.CreateDate.Month == currentDate.Month && x.CreateDate.Day == currentDate.Day
                        && x.ZoneId == zoneId && (x.SubZoneId == subZoneId)
                        ).FirstOrDefault();
                        if (checkreservation == null)
                        {
                            ReservationLoge newReservationLoge = new ReservationLoge()
                            {
                                CreateDate = currentDate,
                                DiscountAmount = 0,
                                //ElectricityAmount = eletricAmount,
                                //ElectronicAmount = hardwareAmount,
                                FineAmount = 0,
                                FullAreaAmount = 0,
                                //LogeAmount = logeAmount,
                                LogeName = "",
                                LogeQty = (int)member.LogNum,
                                MemberId = memberId,
                                // Status = reservationLogeStatus,
                                ZoneId = zoneId,
                                SubZoneId = subZoneId,
                                //ReservationStatus = reservationstatus
                            };
                            context.ReservationLoges.Add(newReservationLoge);
                            context.SaveChanges();

                            Transaction newTransaction = new Transaction()
                            {
                                //AmountToPay = logeAmount + hardwareAmount + eletricAmount,
                                CreateDate = currentDate,
                                MemberId = memberId,
                                PaymentDate = null,
                                PaymentEndPointUrl = null,
                                PaymentGatewayId = PaymentGatewayId, // Default => 13 SCB QrCode || 14 GoMoney Wallet
                                PaymentReferenceCode = null,
                                PaymentResponseCode = null,
                                PaymentResponseMessage = null,
                                PaymentTypeId = PaymentTypeId, // Default => 18 SCB QrCode || 21 GoMoney Wallet
                                RcptNo = null,
                                //ReservationLogeElectricityTypeId = electricQty,
                                //ReservationLogeElectronicTypeId = hardwareQty,
                                //ReservationLogeTypeId = int.Parse(logeQty.ToString() + "0"),
                                ReservationLogeId = newReservationLoge.Id,
                                ReservationsDate = null,
                                TransactionStatusId = 19,
                                TranCode = tranCode,
                                LastUpdate = currentDate,
                                ReservationSubZoneId = subZoneId,
                                TransactionSubZoneId = subZoneId,
                                BypassPaymentStatusId = 1,
                                IsGuest = 0, // always
                                //isChangeZone = isChangeZone
                            };

                            context.Transactions.Add(newTransaction);
                            context.SaveChanges();

                            tranId = newTransaction.TranId;
                            dbContextTransaction.Commit();
                            isComplete = 1;
                        }
                        else
                        {
                            //if (q.AmountToPay == checkreservation.TotalAmount)
                            //{
                            //    Transaction existTransaction = checkreservation.Transactions.FirstOrDefault();
                            //    tranId = existTransaction.TranId;
                            //    isComplete = 1;
                            //}
                        }
                    }
                    catch (Exception err)
                    {
                        if (err.Message != null) Console.WriteLine("IOException source: {0}", err.Message);
                        dbContextTransaction.Rollback();
                        isComplete = -1;
                        errormsg = "IOException source: " + err.Message;
                    }
                }
            }

            //if (isComplete == 1 && tranId > 0)
            //{
            //    //q.Status = 3;
            //    //q.TranId = tranId;
            //    SaveoneKoratMarketContext.SaveChanges();
            //}
            //else
            //{
            //    q.TranBillNo = "Error! :" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + " @" + isComplete + ": " + errormsg;
            //    SaveoneKoratMarketContext.SaveChanges();
            //}

            //else
            //{
            //    //WebMessage.AlertMessage("Error Code T0");
            //    string message = "Error Code T0";
            //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "myalert", "alert(' " + message + " ');", true);
            //    return;
            //    //Response.Redirect("Login.aspx", true);
            //}
            //}
        }
    }
}
