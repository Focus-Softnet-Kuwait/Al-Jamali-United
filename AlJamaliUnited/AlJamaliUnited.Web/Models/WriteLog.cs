using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AlJamaliUnited.Web.Models
{
    public class WriteLog
    {
        public static void writeLog(string content)
        {

            StreamWriter objSw = null;

            try
            {
                string sFilePath = @"C:\Windows\Temp\" + DateTime.Now.ToString("ddMM") + "SBLog.txt";
                objSw = new StreamWriter(sFilePath, true);
                objSw.WriteLine(DateTime.Now.ToString() + " " + content + Environment.NewLine);
            }
            catch (Exception)
            {

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