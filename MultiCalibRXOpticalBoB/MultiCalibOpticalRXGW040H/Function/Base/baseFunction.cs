using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiCalibOpticalRXGW040H.Function
{
    public class baseFunction
    {

        /// <summary>
        /// LIỆT KÊ TÊN TẤT CẢ CÁC CỔNG SERIAL PORT ĐANG KẾT NỐI VÀO MÁY TÍNH
        /// </summary>
        /// <returns></returns>
        public static List<string> get_Array_Of_SerialPort() {
            try {
                // Get a list of serial port names.
                //string[] ports = SerialPort.GetPortNames();
                List<string> list = new List<string>();
                list.Add("-");
                for (int i = 1; i < 100; i++) {
                    list.Add(string.Format("COM{0}", i));
                }
                //foreach (var item in ports) {
                //    list.Add(item);
                //}
                return list;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// KIỂM TRA SỐ BOSA SERIAL NUMBER NHẬP VÀO CÓ ĐÚNG ĐỊNH DẠNG HAY KHÔNG
        /// </summary>
        /// <param name="_bosaSN"></param>
        /// <returns></returns>
        public static bool bosa_SerialNumber_Is_Correct(string _bosaSN) {
            try {
                //Kiểm tra số lượng kí tự trên tem SN Bosa 
                int lent = globalData.initSetting.BOSASNLEN;
                return _bosaSN.Length == lent;

            }
            catch {
                return false;
            }
        }

        static bool _loadBosaReport() {
            if (globalData.initSetting.BOSAREPORTFILE.Trim().Length == 0) return false;
            //Thread t = new Thread(new ThreadStart(() => {
                //Load data from excel to dataGrid
                DataTable dt = new DataTable();
                dt = BosaReport.readData();

                //Import data from dataGrid to Sql Server (using Sql Bulk)
                int counter = 0;
                globalData.listBosaInfo = new List<bosainfo>();
                for (int i = 0; i < dt.Rows.Count; i++) {
                    string _bosaSN = "", _Ith = "", _Vbr = "";
                    _bosaSN = dt.Rows[i].ItemArray[0].ToString().Trim();
                    if (_bosaSN.Length > 0 && bosa_SerialNumber_Is_Correct(_bosaSN) == true) {
                        _Ith = dt.Rows[i].ItemArray[1].ToString().Trim();
                        _Vbr = dt.Rows[i].ItemArray[5].ToString().Trim();

                        bosainfo _bs = new bosainfo() { BosaSN = _bosaSN, Ith = _Ith, Vbr = _Vbr };
                        globalData.listBosaInfo.Add(_bs);
                        counter++;
                    }
                }
            //}));
            //t.IsBackground = true;
            //t.Start();
            return true;
        }

        /// <summary>
        /// LOAD DU LIEU BOSA TU FILE VAO LIST
        /// </summary>
        /// <returns></returns>
        public static bool loadBosaReport() {
            waitWindow w = null;
            //load bosa report data
            Thread t = new Thread(new ThreadStart(() => {
                //Show waiting form
                App.Current.Dispatcher.Invoke(new Action(() => {
                    w = new waitWindow();
                    w.Show();
                }));

                //load bosa report
                Thread s = new Thread(new ThreadStart(() => {
                    if (globalData.initSetting.BOSAREPORTFILE.Length > 0) {
                        _loadBosaReport();
                    }
                }));
                s.IsBackground = true;
                s.Start();

                while (s.IsAlive) Thread.Sleep(100);

                App.Current.Dispatcher.Invoke(new Action(() => {
                    w.Close();
                }));

            }));
            t.IsBackground = true;
            t.Start();
            return true;
        }


        /// <summary>
        /// LẤY VÀ KHỞI TẠO THÔNG TIN TESTINGINFO THEO TÊN NÚT NHẤN
        /// </summary>
        /// <param name="_btnname"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static bool get_Testing_Info_By_Name(string _btnname, ref testinginfo tf) {
            try {
                tf = new testinginfo();
                switch (_btnname) {
                    case "btnStart1": {
                            tf = globalData.testingDataDut1;
                            tf.COMPORT = globalData.initSetting.DEBUG1;
                            tf.GPIB = globalData.initSetting.GPIB1;
                            break;
                        }
                    case "btnStart2": {
                            tf = globalData.testingDataDut2;
                            tf.COMPORT = globalData.initSetting.DEBUG2;
                            tf.GPIB = globalData.initSetting.GPIB2;
                            break;
                        }
                    case "btnStart3": {
                            tf = globalData.testingDataDut3;
                            tf.COMPORT = globalData.initSetting.DEBUG3;
                            tf.GPIB = globalData.initSetting.GPIB3;
                            break;
                        }
                    case "btnStart4": {
                            tf = globalData.testingDataDut4;
                            tf.COMPORT = globalData.initSetting.DEBUG4;
                            tf.GPIB = globalData.initSetting.GPIB4;
                            break;
                        }
                }
                //tf.Initialization();
                //tf.ONTINDEX = _btnname.Substring(_btnname.Length - 1, 1);
                return true;
            }
            catch {
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dBm"></param>
        /// <returns></returns>
        public static double convert_dBm_To_uW(string _dBm) {
            double p = double.Parse(_dBm);
            double uW = 0;
            uW = Math.Pow(10, p / 10) * 1000;
            return uW;
        }

        /// <summary>
        /// CHUYỂN ĐỔI DỮ LIỆU CHUẨN NRZ3 => DOUBLE
        /// </summary>
        /// <param name="_data">-8.780000E+001/-8.780000E-001</param>
        /// <returns></returns>
        public static double convert_NRZ3_To_Double(string _data) {
            try {
                _data = _data.Trim().Replace("\r", "").Replace("\n", "");
                string[] buffer = _data.Split('E');
                double fNum = double.Parse(buffer[0]);
                double lNum = double.Parse(buffer[1].Trim());
                double number = fNum * Math.Pow(10, lNum);
                return (double)Math.Round((decimal)number, 2);
            }
            catch {
                return double.MinValue;
            }
        }

        /// <summary>
        /// GEN MÃ GPON SERIAL TỪ ĐỊA CHỈ MAC
        /// </summary>
        /// <param name="MAC"></param>
        /// <returns></returns>
        public static string GEN_SERIAL_ONT(string MAC) {
            try {
                string low_MAC = MAC.Substring(6, 6);
                string origalByteString = Convert.ToString(HexToBin(low_MAC)[0], 2).PadLeft(8, '0');
                string VNPT_SERIAL_ONT = null;

                origalByteString = origalByteString + "" + Convert.ToString(HexToBin(low_MAC)[1], 2).PadLeft(8, '0');
                origalByteString = origalByteString + "" + Convert.ToString(HexToBin(low_MAC)[2], 2).PadLeft(8, '0');
                //----HEX to BIN Cach 2-------
                string value = low_MAC;
                var s = String.Join("", low_MAC.Select(x => Convert.ToString(Convert.ToInt32(x + "", 16), 2).PadLeft(4, '0')));
                //----HEX to BIN Cach 2-------
                string shiftByteString = "";
                shiftByteString = origalByteString.Substring(1, origalByteString.Length - 1) + origalByteString[0];

                if (MAC.Contains("A06518")) {
                    VNPT_SERIAL_ONT = "VNPT" + "00" + BinToHex(shiftByteString); //"'00' --> dải MAC đang được đăng ký, sau này nếu đăng ký thêm dải mới thì giá trị này sẽ thành '01'"
                }
                else if (MAC.Contains("A4F4C2")) //Dải mác mới của VNPT. Hòa Add: 16/03/2017
                {
                    VNPT_SERIAL_ONT = "VNPT" + "01" + BinToHex(shiftByteString);
                }
                return VNPT_SERIAL_ONT;
            }
            catch {
                return "ERROR";
            }
        }

        private static string BinToHex(string bin) {
            string output = "";
            try {
                int rest = bin.Length % 4;
                bin = bin.PadLeft(rest, '0'); //pad the length out to by divideable by 4

                for (int i = 0; i <= bin.Length - 4; i += 4) {
                    output += string.Format("{0:X}", Convert.ToByte(bin.Substring(i, 4), 2));
                }

                return output;
            }
            catch {
                return "ERROR";
            }
        }

        private static Byte[] HexToBin(string pHexString) {
            if (String.IsNullOrEmpty(pHexString))
                return new Byte[0];

            if (pHexString.Length % 2 != 0)
                throw new Exception("Hexstring must have an even length");

            Byte[] bin = new Byte[pHexString.Length / 2];
            int o = 0;
            int i = 0;
            for (; i < pHexString.Length; i += 2, o++) {
                switch (pHexString[i]) {
                    case '0': bin[o] = 0x00; break;
                    case '1': bin[o] = 0x10; break;
                    case '2': bin[o] = 0x20; break;
                    case '3': bin[o] = 0x30; break;
                    case '4': bin[o] = 0x40; break;
                    case '5': bin[o] = 0x50; break;
                    case '6': bin[o] = 0x60; break;
                    case '7': bin[o] = 0x70; break;
                    case '8': bin[o] = 0x80; break;
                    case '9': bin[o] = 0x90; break;
                    case 'A': bin[o] = 0xa0; break;
                    case 'a': bin[o] = 0xa0; break;
                    case 'B': bin[o] = 0xb0; break;
                    case 'b': bin[o] = 0xb0; break;
                    case 'C': bin[o] = 0xc0; break;
                    case 'c': bin[o] = 0xc0; break;
                    case 'D': bin[o] = 0xd0; break;
                    case 'd': bin[o] = 0xd0; break;
                    case 'E': bin[o] = 0xe0; break;
                    case 'e': bin[o] = 0xe0; break;
                    case 'F': bin[o] = 0xf0; break;
                    case 'f': bin[o] = 0xf0; break;
                    default: throw new Exception("Invalid character found during hex decode");
                }

                switch (pHexString[i + 1]) {
                    case '0': bin[o] |= 0x00; break;
                    case '1': bin[o] |= 0x01; break;
                    case '2': bin[o] |= 0x02; break;
                    case '3': bin[o] |= 0x03; break;
                    case '4': bin[o] |= 0x04; break;
                    case '5': bin[o] |= 0x05; break;
                    case '6': bin[o] |= 0x06; break;
                    case '7': bin[o] |= 0x07; break;
                    case '8': bin[o] |= 0x08; break;
                    case '9': bin[o] |= 0x09; break;
                    case 'A': bin[o] |= 0x0a; break;
                    case 'a': bin[o] |= 0x0a; break;
                    case 'B': bin[o] |= 0x0b; break;
                    case 'b': bin[o] |= 0x0b; break;
                    case 'C': bin[o] |= 0x0c; break;
                    case 'c': bin[o] |= 0x0c; break;
                    case 'D': bin[o] |= 0x0d; break;
                    case 'd': bin[o] |= 0x0d; break;
                    case 'E': bin[o] |= 0x0e; break;
                    case 'e': bin[o] |= 0x0e; break;
                    case 'F': bin[o] |= 0x0f; break;
                    case 'f': bin[o] |= 0x0f; break;
                    default: throw new Exception("Invalid character found during hex decode");
                }
            }
            return bin;
        }

    }
}
