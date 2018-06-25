using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCalibOpticalRXGW040H.Function {
    public class LogFile {
        private static Object lockthis = new Object();
        private static string _logtest = System.AppDomain.CurrentDomain.BaseDirectory + "LogTest\\";
        private static string _logdetail = System.AppDomain.CurrentDomain.BaseDirectory + "LogDetail\\";

        static LogFile() {
            if (Directory.Exists(_logtest) == false) Directory.CreateDirectory(_logtest);
            if (Directory.Exists(_logdetail) == false) Directory.CreateDirectory(_logdetail);
        }

        public static bool Savelogtest(testinginfo _testinfo) {
            lock (lockthis) {
                try {
                    string _file = DateTime.Now.ToString("yyyyMMdd");
                    string _title = "DateTime,MacAddress,BosaSN,setAPDandSlope,OverloadSensitivity,CalibDDMI,CurveDDMI,CalibLOS,CheckLOS,WriteFlash,ErrorCode,TotalResult";

                    string _content = "";
                    _content += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ",";
                    _content += _testinfo.MACADDRESS + ",";
                    _content += _testinfo.BOSASERIAL + ",";
                    _content += _testinfo.VAPDRESULT + ",";
                    _content += _testinfo.OVERLOADRESULT + ",";
                    _content += _testinfo.CALIBDDMIRESULT + ",";
                    _content += _testinfo.CURVEDDMIRESULT + ",";

                    _content += _testinfo.CALIBLOSRESULT + ",";
                    _content += _testinfo.CHECKLOSRESULT + ",";
                    _content += _testinfo.WRITEFLASHRESULT + ",";

                    _content += _testinfo.ERRORCODE.Replace("Mã Lỗi", "") + ",";
                    _content += _testinfo.TOTALRESULT + ",";

                    if (File.Exists(string.Format("{0}\\{1}.csv", _logtest, _file)) == false) {
                        StreamWriter st = new StreamWriter(string.Format("{0}\\{1}.csv", _logtest, _file), true);
                        st.WriteLine(_title);
                        st.WriteLine(_content);
                        st.Dispose();
                    }
                    else {
                        StreamWriter st = new StreamWriter(string.Format("{0}\\{1}.csv", _logtest, _file), true);
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
                    string _file = DateTime.Now.ToString("yyyyMMdd");
                    StreamWriter st = new StreamWriter(string.Format("{0}\\{1}.txt", _logdetail, _file), true);
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
