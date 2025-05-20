namespace ExcelEditor.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Data.OleDb;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Xml.Linq;
    using System.Threading;
    using System.Net.Http;
    using System.Resources;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Microsoft.Extensions.Options;

    public class SMSManager
    {
        public SMSManager()
        {
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
        private readonly SMSSetting _setting;

        public SMSManager(IOptions<SMSSetting> setting)
        {
            _setting = setting.Value;
        }

        public class Result
        {
            public string Message { get; set; }
            public bool Success { get; set; }
        }
        
        public Result SendSMS(string mobileNumber, string message)
        {
            bool suc = false;
            string msg = "";

            try
            {
                string postData = "ACCOUNT=" + _setting.SMSAccount + "&PASSWORD=" + _setting.SMSPassword + "&MOBILE=" + mobileNumber + "&MESSAGE=" + message;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://u8.sc4msg.com/SendMessage");
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                //req.Timeout = 800;
                byte[] data = Encoding.UTF8.GetBytes(postData);
                req.ContentLength = data.Length;

                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
                string returnData = sr.ReadToEnd();

                int status = 1;
                string messageId = "", taskId = "", drdnStatus = "";
                foreach (var item in returnData.Split('\n'))
                {
                    string[] result = item.Split('=');
                    switch (result[0].ToUpper())
                    {
                        case "STATUS":
                            status = int.Parse(result[1].ToString());
                            break;
                        case "MESSAGE_ID":
                            messageId = result[1].ToString();
                            break;
                        case "TASK_ID":
                            taskId = result[1].ToString();
                            break;
                    }
                }

                if (status == 0)
                {
                    status = 1;
                    suc = true;
                    msg = "Send SMS to " + mobileNumber + " Success";
                    //drdnStatus = "Delivered";
                }
                else
                {
                    switch (status)
                    {
                        case 500:
                            msg = "Service Error or Invalid Request Parameter"; break;
                        case 501:
                            msg = "Incomplete Request"; break;
                        case 502:
                            msg = "Authentication Failed(Invalid Account or Password)"; break;
                        case 503:
                            msg = "Data Format Error"; break;
                        case 504:
                            msg = "Insufficient Balance"; break;
                        case 505:
                            msg = "Invalid Mobile No."; break;
                        case 506:
                            msg = "Invalid Activation Key"; break;
                        case 507:
                            msg = "Sender Not Allowed"; break;
                    }
                    drdnStatus = msg;
                }

               

            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return new Result() { Success = suc, Message = msg };
        }
    }
}
    
