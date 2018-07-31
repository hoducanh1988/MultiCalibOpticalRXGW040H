using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MultiCalibOpticalRXGW040H.Function {
    public class LogFile {
        private static Object lockthis = new Object();

        private static string _dir_logtest = System.AppDomain.CurrentDomain.BaseDirectory + "LogTest";
        private static string _dir_logdetail = System.AppDomain.CurrentDomain.BaseDirectory + "LogDetail";
        private static string _dir_040H_logtest = System.AppDomain.CurrentDomain.BaseDirectory + "LogTest\\040H";
        private static string _dir_040H_logdetail = System.AppDomain.CurrentDomain.BaseDirectory + "LogDetail\\040H";
        private static string _dir_020BoB_logtest = System.AppDomain.CurrentDomain.BaseDirectory + "LogTest\\020BoB";
        private static string _dir_020BoB_logdetail = System.AppDomain.CurrentDomain.BaseDirectory + "LogDetail\\020BoB";



        static LogFile() {
            if (Directory.Exists(_dir_logtest) == false) Directory.CreateDirectory(_dir_logtest);
            if (Directory.Exists(_dir_logdetail) == false) Directory.CreateDirectory(_dir_logdetail);
            Thread.Sleep(100);

            if (Directory.Exists(_dir_040H_logtest) == false) Directory.CreateDirectory(_dir_040H_logtest);
            if (Directory.Exists(_dir_040H_logdetail) == false) Directory.CreateDirectory(_dir_040H_logdetail);
            if (Directory.Exists(_dir_020BoB_logtest) == false) Directory.CreateDirectory(_dir_020BoB_logtest);
            if (Directory.Exists(_dir_020BoB_logdetail) == false) Directory.CreateDirectory(_dir_020BoB_logdetail);
        }

        public static bool Savelogtest(testinginfo _testinfo) {
            lock (lockthis) {
                try {
                    string _dir = _testinfo.ONTTYPE == "GW040H" ? _dir_040H_logtest : _dir_020BoB_logtest;
                    string _file = DateTime.Now.ToString("yyyyMMdd");
                    string _title = _testinfo.ONTTYPE == "GW040H" ? "DateTime,MacAddress,BosaSN,setAPDandSlope,OverloadSensitivity,CalibDDMI,CurveDDMI,CalibLOS,CheckLOS,WriteFlash,ErrorCode,TotalResult" : "DateTime,MacAddress,BosaSN,CalibDDMI,CalibLOS,ErrorCode,TotalResult";
                    
                    string _content = "";
                    _content += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ",";
                    _content += _testinfo.MACADDRESS + ",";
                    _content += _testinfo.BOSASERIAL + ",";

                    //GW040H
                    if (_testinfo.ONTTYPE == "GW040H") {
                        _content += _testinfo.VAPDRESULT + ",";
                        _content += _testinfo.OVERLOADRESULT + ",";
                        _content += _testinfo.CALIBDDMIRESULT + ",";
                        _content += _testinfo.CURVEDDMIRESULT + ",";

                        _content += _testinfo.CALIBLOSRESULT + ",";
                        _content += _testinfo.CHECKLOSRESULT + ",";
                        _content += _testinfo.WRITEFLASHRESULT + ",";

                        _content += _testinfo.ERRORCODE.Replace("Mã Lỗi", "") + ",";
                        _content += _testinfo.TOTALRESULT + ",";
                    }
                    //GW020BoB
                    else {
                        _content += _testinfo.CALIBDDMIRESULT + ",";
                        _content += _testinfo.CALIBLOSRESULT + ",";
                        _content += _testinfo.ERRORCODE.Replace("Mã Lỗi", "") + ",";
                        _content += _testinfo.TOTALRESULT + ",";
                    }
                    
                    if (File.Exists(string.Format("{0}\\{1}.csv", _dir, _file)) == false) {
                        StreamWriter st = new StreamWriter(string.Format("{0}\\{1}.csv", _dir, _file), true);
                        st.WriteLine(_title);
                        st.WriteLine(_content);
                        st.Dispose();
                    }
                    else {
                        StreamWriter st = new StreamWriter(string.Format("{0}\\{1}.csv", _dir, _file), true);
                        st.WriteLine(_content);
                        st.Dispose();
                    }
                    return true;
                }
                catch {
                    return false;
                }
            }
        }

        public static bool Savelogdetail (testinginfo _testinfo) {
            lock (lockthis) {
                try {
                    string _dir = _testinfo.ONTTYPE == "GW040H" ? _dir_040H_logdetail : _dir_020BoB_logdetail;
                    string _file = DateTime.Now.ToString("yyyyMMdd");
                    StreamWriter st = new StreamWriter(string.Format("{0}\\{1}.txt", _dir, _file), true);
                    st.WriteLine(_testinfo.SYSTEMLOG);
                    st.Dispose();
                    return true;
                } catch {
                    return false;
                }
            }
        }
    }
}
