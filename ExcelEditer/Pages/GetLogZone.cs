namespace ExcelEditor.Pages
{
    public static class GetLogZone
    {
        public static int GetLogzone(string name) =>
                    name.Contains("UWXYZ") ? 1 :
                    name.Contains("ST") ? 6 :
                    name.Contains("AB") ? 38 :
                    name.Contains("7-11") ? 2 :
                    name.Contains("วะวาบ") ? 3 :
                    name.Contains("บนถนน") ? 4 :
                    name.Contains("Lotus") ? 5 :
                    name.Contains("กลางเดิ่น") ? 21 :
                    name.Contains("MC") ? 34 :
                    name.Contains("หลังคาสูง") ? 41 :
                    name.Contains("MU") ? 7 :
                    name.Contains("MS") ? 16 :
                    name.Contains("MA-MB") ? 48 :
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
                    name.Contains("R01A") ? 2 :
                    name.Contains("UWXYZ") ? 3 :
                    name.Contains("ST") ? 3 :
                    name.Contains("AB") ? 3 :
                    name.Contains("7-11") ? 4 :
                    name.Contains("วะวาบ") ? 4 :
                    name.Contains("บนถนน") ? 4 :
                    name.Contains("Lotus") ? 4 :
                    name.Contains("กลางเดิ่น") ? 4 :
                    name.Contains("หลังคาสูง") ? 4 :
                    name.Contains("MU") ? 5 :
                    name.Contains("MS") ? 5 :
                    name.Contains("MA-MB") ? 5 :
                    name.Contains("MC") ? 5 : 0;
    }
    
 }
