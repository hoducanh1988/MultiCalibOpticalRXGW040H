using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCalibOpticalRXGW040H.Function
{
    public class globalData
    {
        public static defaultsetting initSetting = new defaultsetting();
        public static List<bosainfo> listBosaInfo = new List<bosainfo>();

        public static testinginfo testingDataDut1 = new testinginfo();
        public static testinginfo testingDataDut2 = new testinginfo();
        public static testinginfo testingDataDut3 = new testinginfo();
        public static testinginfo testingDataDut4 = new testinginfo();

        public static string loginUser = "";
        public static string loginPass = "";

        public static mainwindowinfo mainWindowINFO = new mainwindowinfo();
    }
}
