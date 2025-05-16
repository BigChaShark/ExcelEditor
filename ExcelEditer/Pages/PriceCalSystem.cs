using ExcelEditor.Models;
using static IndexModel;

namespace ExcelEditor.Pages
{
    public class PriceCalSystem
    {
        public static void TotalPrice(UserModel user)
        {
            decimal logeAmount = GetLogeAmount(user);
            decimal electricityAmount = GetElectricityAmount(user);
            decimal electronicAmount = GetElectronicAmount(user);
            decimal total = logeAmount + electricityAmount + electronicAmount;
            user.LogeAmount = logeAmount;
            user.ElectricityAmount = electricityAmount;
            user.ElectronicAmount = electronicAmount;
            user.TotalAmount = total;
        }
        public static decimal GetElectricityAmount(UserModel user)
        {
            var db = new SaveoneKoratMarketContext();
            if (user.ElectricityQty == 0)
                return 0;
            var electricityRate = db.ReservationLogeElectricityTypes.Where(x => x.Description.Contains(user.ElectricityQty.ToString()))
                .Select(s => s.Price)
                .FirstOrDefault();
            if (electricityRate != null) {
                return (decimal)electricityRate;
            }
            return 0;
        }
        public static decimal GetElectronicAmount(UserModel user)
        {
            var db = new SaveoneKoratMarketContext();
            if (user.ElectronicQty == 0)
                return 0;
            var electronicRate = db.ReservationLogeElectronicTypes.Where(x => x.Description.Contains(user.ElectronicQty.ToString()))
                .Select(s => s.Price)
                .FirstOrDefault();
            if (electronicRate != null)
            {
                return (decimal)electronicRate;
            }
            return 0;
        }
        public static decimal GetLogeAmount(UserModel user)
        {
            decimal logeCost = 0;
            string day = user.CreatDate.DayOfWeek.ToString();
            var db = new SaveoneKoratMarketContext();
            var logeCostPerDays = db.LogeCostPerDays.Where(x => (x.SubZoneId == user.SubZone) && (x.Day == day)).FirstOrDefault();
            if (logeCostPerDays == null)
                return 0;
            int hm = int.Parse(DateTime.Now.ToString("HHmm"));
            if (hm <= 1500)
                logeCost = logeCostPerDays.Cost * (decimal)user.LogNum;
            else if (hm > 1500)
                logeCost = logeCostPerDays.Cost2.HasValue ? logeCostPerDays.Cost2.Value : logeCostPerDays.Cost * (decimal)user.LogNum;

            decimal fullLogeCost = GetFullogeAmount(user , logeCost);
            logeCost += fullLogeCost;
            return logeCost;
        }
        public static decimal GetFullogeAmount(UserModel user , decimal logeCostNow)
        {
            decimal total = 0;
            if (user.LogNum==user.FullLogeQty)
            {
                total = logeCostNow;
                return total;
            }
            else
            {
                total = 50*user.FullLogeQty;
                return total;
            }
        }
    }
}
