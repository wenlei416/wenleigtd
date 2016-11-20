using System.Reflection;
using log4net;

namespace GTD.Util
{
    public class LogHelper
    {
        public static void WriteLog(string txt)
        {
            ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(txt);
        }
    }
}