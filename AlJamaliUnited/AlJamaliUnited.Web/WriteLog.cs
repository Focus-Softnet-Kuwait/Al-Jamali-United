﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace AlJamaliUnited.Web
{
    public class WriteLog
    {
        public static string PublishFolder = ConfigurationManager.AppSettings["PublishFolder"].ToString();
        public static void writeLog(string content)
        {
            StreamWriter objSw = null;
            try
            {
                string logFolder = Path.Combine(@"C:\inetpub\wwwroot\AlJamaliUnited", PublishFolder, "Log");
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                string logFilePath = Path.Combine(logFolder, "AlJamaliUnited-" + DateTime.Now.ToString("ddMMyyyy") + ".txt");

                objSw = new StreamWriter(logFilePath, true);
                objSw.WriteLine(DateTime.Now.ToString() + " " + content + Environment.NewLine);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (objSw != null)
                {
                    objSw.Flush();
                    objSw.Dispose();
                }
            }
        }
    }
}