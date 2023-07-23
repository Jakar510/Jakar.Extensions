using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Jakar.AppLogger.Common;


public static class Routes
{
    public const string REGISTER = "Api/Register";
    public const string VERIFY   = "Api/Verify";
    public const string REFRESH  = "Api/Refresh";
    public const string LOG      = "Api/Log";



    public static class Sessions
    {
        public const string START = "Api/Sessions/Start";
        public const string END   = "Api/Sessions/End";
    }
}
