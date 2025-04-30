namespace ExcelEditor.Pages
{
    public static class GetLogZone
    {
        public static int GetLogzone(string name) =>
                    name.Contains("GA-GJ") ? 43 :
                    name.Contains("GL-GT") ? 45 :
                    name.Contains("GW-GZ") ? 46 :
                    name.Contains("R01B-R01S") ? 49 :
                    name.Contains("R01A") ? 50 : 0;

        public static int GetMarketZone(string name) =>
                   name.Contains("GA-GJ") ? 1 :
                   name.Contains("GL-GT") ? 1 :
                   name.Contains("GW-GZ") ? 1 :
                   name.Contains("R01B-R01S") ? 2 :
                   name.Contains("R01A") ? 2 : 0;
    }
    
 }
