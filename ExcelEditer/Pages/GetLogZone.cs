namespace ExcelEditor.Pages
{
    public static class GetLogZone
    {
        public static int GetLogzone(string name) =>
                    name.Contains("U1-Z2") ? 1 :
                    name.Contains("S1-T2") ? 6 :
                    name.Contains("A1-B2") ? 38 :
                    name.Contains("TA-TC") ? 2 :
                    name.Contains("TD-TN") ? 3 :
                    name.Contains("TDS-TGS") ? 4 :
                    name.Contains("TAL-TCL") ? 5 :
                    name.Contains("TH-TJ") ? 21 :
                    name.Contains("TYF-TZF") ? 22 :
                    name.Contains("TV-TZ") ? 41 :
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
                    name.Contains("U1-Z2") ? 3 :
                    name.Contains("S1-T2") ? 3 :
                    name.Contains("A1-B2") ? 3 :
                    name.Contains("TA-TC") ? 4 :
                    name.Contains("TD-TN") ? 4 :
                    name.Contains("TDS-TGS") ? 4 :
                    name.Contains("TAL-TCL") ? 4 :
                    name.Contains("TH-TJ") ? 4 :
                    name.Contains("TYF-TZF") ? 4 :
                    name.Contains("TV-TZ") ? 4 :
                    name.Contains("MU") ? 5 :
                    name.Contains("MS") ? 5 :
                    name.Contains("MA-MB") ? 5 : 0;
    }
    
 }
