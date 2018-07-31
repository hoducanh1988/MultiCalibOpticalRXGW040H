using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiCalibOpticalRXGW040H.Function {
    public class GW020BoB : GW {

        int delay = 100;

        public GW020BoB(string _portname) : base(_portname) { }

        //OK
        public override string getMACAddress(testinginfo _testinfo) {
            //Get MAC Address
            string _mac = "";
            try {
                _testinfo.SYSTEMLOG += string.Format("Get mac address...\r\n");
                base.Write("ifconfig br0\n");
                Thread.Sleep(300);
                string _tmpStr = base.Read();
                _tmpStr = _tmpStr.Replace("\r", "").Replace("\n", "").Trim();
                string[] buffer = _tmpStr.Split(new string[] { "HWaddr" }, StringSplitOptions.None);
                _tmpStr = buffer[1].Trim();
                _mac = _tmpStr.Substring(0, 17).Replace(":", "");
                _testinfo.SYSTEMLOG += string.Format("...PASS. {0}\r\n", _mac);
            }
            catch (Exception ex) {
                _testinfo.ERRORCODE = "(Mã Lỗi: COT-GM-0001)";
                _testinfo.SYSTEMLOG += string.Format("...FAIL. {0}. {1}\r\n", _testinfo.ERRORCODE, ex.ToString());
            }

            //Write Pass
            base.WriteLine("laser msg --set 7b 1 ff"); //Chuỗi lệnh Write Password
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 7c 1 ff"); //
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 7d 1 ff"); //
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 7e 1 ff"); //
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 7f 1 2"); //
            Thread.Sleep(100);
            base.WriteLine("laser msg --set c4 1 0"); //
            Thread.Sleep(100);
            base.WriteLine("laser msg --set c7 1 0"); //Lệnh Enable Edit EEPROM
            Thread.Sleep(100);

            base.WriteLine("laser msg --set 6e 1 44"); //2 lệnh Soft Reset
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 6e 1 04"); //
            Thread.Sleep(100);

            return _mac;
        }

        //OK
        public override bool Login(out string message) {
            message = "";
            try {
                bool _flag = false;
                int index = 0;
                int max = 20;
                while (!_flag) {
                    //Gửi lệnh Enter để ONT về trạng thái đăng nhập
                    message += "Gửi lệnh Enter để truy nhập vào login...\r\n";
                    base.WriteLine("");
                    Thread.Sleep(200);
                    string data = "";
                    data = base.Read();
                    message += string.Format("Feedback:=> {0}\r\n", data);
                    if (data.Replace("\r", "").Replace("\n", "").Trim().Contains(">")) return true;
                    while (!data.Contains("Login:")) {
                        data += base.Read();
                        message += string.Format("Feedback:=> {0}\r\n", data);
                        Thread.Sleep(200);
                        if (index >= max) break;
                        else index++;
                    }
                    if (index >= max) break;

                    //Gửi thông tin User
                    base.Write(globalData.initSetting.ONTUSER + "\n");
                    message += "Gửi thông tin user: " + globalData.initSetting.ONTUSER + "...\r\n";

                    //Chờ ONT xác nhận User
                    data = "";
                    while (!data.Contains("Password:")) {
                        data += base.Read();
                        message += string.Format("Feedback:=> {0}\r\n", data);
                        Thread.Sleep(100);
                        if (index >= max) break;
                        else index++;
                    }
                    if (index >= max) break;

                    //Gửi thông tin Password
                    base.Write(globalData.initSetting.ONTPASS + "\n");
                    message += "Gửi thông tin password: " + globalData.initSetting.ONTPASS + "...\r\n";

                    //Chờ ONT xác nhận Password
                    while (!data.Contains(">")) {
                        data += base.Read();
                        message += string.Format("Feedback:=> {0}\r\n", data);
                        Thread.Sleep(500);
                        if (index >= max) break;
                        else index++;
                    }
                    if (index >= max) break;
                    else _flag = true;
                }
                return _flag;
            }
            catch (Exception ex) {
                message += ex.ToString() + "\r\n";
                return false;
            }
        }

        //OK
        public override bool loginToONT(testinginfo _testinfo) {
            _testinfo.SYSTEMLOG += string.Format("Verifying type of ONT...\r\n...{0}\r\n", globalData.initSetting.ONTTYPE);
            bool _result = false;
            string _message = "";

            _testinfo.SYSTEMLOG += string.Format("Open comport of ONT...{0}\r\n", _testinfo.COMPORT);
            if (!base.Open(out _message)) {
                _testinfo.ERRORCODE = "(Mã Lỗi: COT-LI-0001)";
                _testinfo.SYSTEMLOG += string.Format("...{0}, {1}\r\n", _testinfo.ERRORCODE, _message);
                return false;
            }
            _testinfo.SYSTEMLOG += "...PASS\r\n";

            _testinfo.SYSTEMLOG += "Login to ONT...\r\n";
            _result = this.Login(out _message);

            if (_result == false) _testinfo.ERRORCODE = "(Mã Lỗi: COT-LI-0002)";
            _testinfo.SYSTEMLOG += string.Format("...{0}, {1}\r\n", _testinfo.ERRORCODE, _message);
            _testinfo.SYSTEMLOG += _result == true ? "PASS\r\n" : "FAIL\r\n";

            return _result;
        }

        //OK
        public override bool calibDDMI(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {

            double ADC_RX_Point1 = 0;
            double ADC_RX_Point2 = 0;
            double ADC_RX_Point3 = 0;

            double RX_Power1 = 0.5;
            double RX_Power2 = 1;
            double RX_Power3 = 100;

            string Slope_P2 = "";
            string Slope_P1 = "";
            string Offset = "";

            string value = "";
            string value_register = "";
            bool Calibration_RX_Result = false;
            string Power_DDMI = "";

            string attenuation_level_1 = (-33 - double.Parse(VAR.OLT_Power)).ToString();
            string attenuation_level_2 = (-30 - double.Parse(VAR.OLT_Power)).ToString();
            string attenuation_level_3 = (-10 - double.Parse(VAR.OLT_Power)).ToString();

            _testinfo.CALIBDDMIRESULT = Parameters.testStatus.Wait.ToString();

            value = base.Read();
            try {

                for (int i = 0; i < 3; i++) {
                    _testinfo.SYSTEMLOG += "Đã thiết lập xong máy suy hao.\r\n";
                    _testinfo.SYSTEMLOG += "OLT phát công suất = -33dBm ~ 0.5uW\r\n";
                    // Thiết lập suy hao mức 1 cho máy điều chỉnh suy hao
                    FVA.set_attenuation_level(attenuation_level_1);
                    _testinfo.SYSTEMLOG += string.Format("Đang thiết lập mức suy hao = {0}\r\n", attenuation_level_1);
                    Thread.Sleep(3000);
                    // Đọc giá trị thanh ghi ADC RX Power:F0-F1
                    // Đổi Hexa -> Decimal, lưu vào biến ADC_RX_Point1
                    //ADC_RX_Point1 = double.Parse(Read_ADC_value_RX("F0"));
                    //MessageBox.Show("Đọc ADC_RX 1");

                    base.Read();
                    base.WriteLine("laser msg --get F0 2");
                    Thread.Sleep(100);
                    value = base.Read();
                    _testinfo.SYSTEMLOG += value + "\r\n";
                    for (int j = 0; j < value.Split('\n').Length; j++) {
                        if (value.Split('\n')[j].Contains("I2C")) {
                            value_register = value.Split('\n')[j].Split(':')[1].Trim();
                            ADC_RX_Point1 = Convert.ToDouble(HexToDec(value_register).ToString());
                            _testinfo.SYSTEMLOG += "ADC_RX_Point1 = " + ADC_RX_Point1 + "\r\n";
                        }
                    }
                    _testinfo.SYSTEMLOG += "OLT phát công suất = -30dBm ~ 1uW\r\n";
                    // Thiết lập suy hao mức 2 cho máy điều chỉnh suy hao
                    FVA.set_attenuation_level(attenuation_level_2);
                    _testinfo.SYSTEMLOG += "Đang thiết lập mức suy hao = " + attenuation_level_2 + "\r\n";
                    Thread.Sleep(3000);
                    // Đọc giá trị thanh ghi ADC RX Power:F0-F1
                    // Đổi Hexa -> Decimal, lưu vào biến ADC_RX_Point2
                    //ADC_RX_Point2 = double.Parse(Read_ADC_value_RX("F0"));
                    //MessageBox.Show("Đọc ADC_RX 2");
                    base.Read();
                    base.WriteLine("laser msg --get F0 2");
                    Thread.Sleep(100);
                    value = base.Read();
                    _testinfo.SYSTEMLOG += value + "\r\n";
                    for (int j = 0; j < value.Split('\n').Length; j++) {
                        if (value.Split('\n')[j].Contains("I2C")) {
                            value_register = value.Split('\n')[j].Split(':')[1].Trim();
                            ADC_RX_Point2 = Convert.ToDouble(HexToDec(value_register).ToString());
                            _testinfo.SYSTEMLOG += "ADC_RX_Point2 = " + ADC_RX_Point2 + "\r\n";
                        }
                    }

                    _testinfo.SYSTEMLOG += "OLT phát công suất = -10dBm ~ 100uW\r\n";
                    // Thiết lập suy hao mức 3 cho máy điều chỉnh suy hao
                    FVA.set_attenuation_level(attenuation_level_3);
                    _testinfo.SYSTEMLOG += "Đang thiết lập mức suy hao = " + attenuation_level_3 + "\r\n";
                    Thread.Sleep(3000);
                    // Đọc giá trị thanh ghi ADC RX Power:F0-F1
                    // Đổi Hexa -> Decimal, lưu vào biến ADC_RX_Point3
                    //ADC_RX_Point3 = double.Parse(Read_ADC_value_RX("F0"));
                    //MessageBox.Show("Đọc ADC_RX 3");

                    base.Read();
                    base.WriteLine("laser msg --get F0 2");
                    Thread.Sleep(100);
                    value = base.Read();
                    _testinfo.SYSTEMLOG += value + "\r\n";
                    for (int j = 0; j < value.Split('\n').Length; j++) {
                        if (value.Split('\n')[j].Contains("I2C")) {
                            value_register = value.Split('\n')[j].Split(':')[1].Trim();
                            ADC_RX_Point3 = Convert.ToDouble(HexToDec(value_register).ToString());
                            _testinfo.SYSTEMLOG += "ADC_RX_Point3 = " + ADC_RX_Point3 + "\r\n";
                        }
                    }

                    //Tính Slope-P2; Slope-P1; Offset
                    Slope_P2 = Calculate_Slope_Offset_Polynomial(2, ADC_RX_Point1, ADC_RX_Point2, ADC_RX_Point3, RX_Power1, RX_Power2, RX_Power3);
                    _testinfo.SYSTEMLOG += "Slope_2 DEC =" + Slope_P2 + "\r\n";
                    Slope_P2 = Convert_SlopeP2_RxToHex(float.Parse(Slope_P2));

                    Slope_P1 = Calculate_Slope_Offset_Polynomial(1, ADC_RX_Point1, ADC_RX_Point2, ADC_RX_Point3, RX_Power1, RX_Power2, RX_Power3);
                    _testinfo.SYSTEMLOG += "Slope_1 DEC =" + Slope_P1 + "\r\n";
                    Slope_P1 = Convert_SlopeP1_RxToHex(float.Parse(Slope_P1));

                    Offset = Calculate_Slope_Offset_Polynomial(0, ADC_RX_Point1, ADC_RX_Point2, ADC_RX_Point3, RX_Power1, RX_Power2, RX_Power3);
                    _testinfo.SYSTEMLOG += "Offset DEC =" + Offset + "\r\n";
                    Offset = Convert_Offset_RxToHex(float.Parse(Offset));
                    _testinfo.SYSTEMLOG += "--\r\n";
                    _testinfo.SYSTEMLOG += "Slope_2 =" + Slope_P2 + "\r\n";
                    _testinfo.SYSTEMLOG += "Slope_1 =" + Slope_P1 + "\r\n";
                    _testinfo.SYSTEMLOG += "Offset =" + Offset + "\r\n";

                    if (Slope_P2 != "NONE" && Slope_P1 != "NONE" && Offset != "NONE") {
                        // Code Write Slope, Offset đến GN25L98
                        _testinfo.SYSTEMLOG += "Thực hiện Write Slope & Offset to EEPROM\r\n";
                        //MessageBox.Show(Slope_P2.Substring(0, 2));
                        Write_To_Register_COM("e2", Slope_P2.Substring(0, 2));
                        Thread.Sleep(delay);
                        //MessageBox.Show(Slope_P2.Substring(2, 2));
                        Write_To_Register_COM("e3", Slope_P2.Substring(2, 2));
                        Thread.Sleep(delay);
                        //MessageBox.Show(Offset.Substring(0, 2));
                        Write_To_Register_COM("e4", Slope_P1.Substring(0, 2));
                        Thread.Sleep(delay);
                        //MessageBox.Show(Offset.Substring(2, 2));
                        Write_To_Register_COM("e5", Slope_P1.Substring(2, 2));
                        Thread.Sleep(delay);
                        //MessageBox.Show(Slope_P2.Substring(0, 2));
                        Write_To_Register_COM("e6", Offset.Substring(0, 2));
                        Thread.Sleep(delay);
                        //MessageBox.Show(Offset.Substring(2, 2));
                        Write_To_Register_COM("e7", Offset.Substring(2, 2));
                        Thread.Sleep(delay);
                        base.Read();
                        base.WriteLine("laser power --rxread");
                        Thread.Sleep(100);
                        Power_DDMI = base.Read().Split('=')[2].Trim().Substring(0, 5);
                        _testinfo.SYSTEMLOG += "Power_DDMI = " + Power_DDMI + "\r\n";
                        if (Convert.ToDouble(Power_DDMI) > -11 && Convert.ToDouble(Power_DDMI) < -9) {
                            _testinfo.SYSTEMLOG += "[OK] Hoàn thành Calibration RX Power.\r\n";
                            Calibration_RX_Result = true;
                            _testinfo.CALIBDDMIRESULT = Parameters.testStatus.PASS.ToString();
                        }
                        else {
                            _testinfo.SYSTEMLOG += "[FAIL] Lỗi Calibration RX Power.\r\n";
                            Calibration_RX_Result = false;
                            _testinfo.CALIBDDMIRESULT = Parameters.testStatus.FAIL.ToString();
                        }
                        break;
                    }
                    else {
                        continue;
                    }
                }
            }
            catch (Exception ex) {
                _testinfo.SYSTEMLOG += ex.ToString() + "\r\n";
                _testinfo.SYSTEMLOG += "[ERROR] Lỗi trong quá trình RX Calibration.\r\n";
                //MessageBox.Show(Ex.ToString());
                Calibration_RX_Result = false;
                _testinfo.CALIBDDMIRESULT = Parameters.testStatus.FAIL.ToString();
            }
            return Calibration_RX_Result;
        }

        //OK
        public override bool calibLOS(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR, ref bool _flag) {
            string result1_LOS = "";
            string Reg_value_LOS = "";
            string Reg_value_bin_LOS = "";
            string LOS_bit = "";
            string LOS_Level = "";
            //string LOS_Level_Hex = "";
            string LOS_SLEEP_CTRL = "";
            bool Tuning_LOS_Level_Result = false;
            bool Check_LOS_Result = false;
            bool Check_SD_Result = false;
            string attenuation_level = (-36 - double.Parse(VAR.OLT_Power)).ToString();

            _testinfo.CALIBLOSRESULT = Parameters.testStatus.Wait.ToString();
            _testinfo.SYSTEMLOG += "Đã thiết lập xong máy suy hao.\r\n";

            FVA.set_attenuation_level(attenuation_level);
            _testinfo.SYSTEMLOG += "Đang thiết lập mức suy hao = " + attenuation_level + "\r\n";
            Thread.Sleep(2000);
            //ONT.Read();
            //base.WriteLine("laser msg --set bc 1 1c"); 
            base.WriteLine("laser msg --set bc 1 15"); //06.07.2018 đổi từ 1c sang 15 => nhầm code
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 6e 1 44");
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 6e 1 04");
            Thread.Sleep(100);

            base.Read();
            base.WriteLine("laser msg --get bc 1");
            Thread.Sleep(100);
            result1_LOS = base.Read();

            _testinfo.SYSTEMLOG += "result1_LOS: " + result1_LOS + "\r\n";
            //Hien_Thi.Hienthi.SetText(rtb, result1_LOS);

            for (int i = 0; i < result1_LOS.Split('\n').Length; i++) {
                if (result1_LOS.Split('\n')[i].Contains("I2C")) {
                    Reg_value_LOS = result1_LOS.Split('\n')[i].Split(':')[1].Trim();
                    Reg_value_bin_LOS = hex2bin(Reg_value_LOS);
                }
            }
            LOS_SLEEP_CTRL = Reg_value_bin_LOS.Substring(0, 1);
            LOS_Level = Reg_value_bin_LOS.Substring(1, 7);
            //Hien_Thi.Hienthi.SetText(rtb, "LOS_Level_bin = " + LOS_Level);
            base.Read();
            base.WriteLine("laser msg --get 6e 1");
            Thread.Sleep(100);
            result1_LOS = base.Read();
            //Hien_Thi.Hienthi.SetText(rtb, result1_LOS);

            for (int i = 0; i < result1_LOS.Split('\n').Length; i++) {
                if (result1_LOS.Split('\n')[i].Contains("I2C")) {
                    Reg_value_LOS = result1_LOS.Split('\n')[i].Split(':')[1].Trim();
                    Reg_value_bin_LOS = hex2bin(Reg_value_LOS);
                }
            }
            LOS_bit = Reg_value_bin_LOS.Substring(6, 1);
            //Hien_Thi.Hienthi.SetText(rtb, "LOS_bit = " + LOS_bit);

            if (LOS_bit == "1") {
                for (int m = 0; m < 78; m++) {
                    _testinfo.SYSTEMLOG += "...\r\n";
                    _testinfo.SYSTEMLOG += "Điều chỉnh giảm LOS Level\r\n";
                    LOS_Level = (Convert.ToInt32((LOS_Level), 2) - Convert.ToInt32("1", 2)).ToString("X");
                    _testinfo.SYSTEMLOG += "LOS_Level_new = " + LOS_Level + "\r\n";
                    base.WriteLine("laser msg --set bc 1 " + LOS_Level);
                    Thread.Sleep(100);
                    base.Read();
                    base.WriteLine("laser msg --get bc 1");
                    Thread.Sleep(100);
                    result1_LOS = base.Read();
                    //Hien_Thi.Hienthi.SetText(rtb, result1_LOS);

                    for (int i = 0; i < result1_LOS.Split('\n').Length; i++) {
                        if (result1_LOS.Split('\n')[i].Contains("I2C")) {
                            Reg_value_LOS = result1_LOS.Split('\n')[i].Split(':')[1].Trim();
                            Reg_value_bin_LOS = hex2bin(Reg_value_LOS);
                        }
                    }
                    LOS_SLEEP_CTRL = Reg_value_bin_LOS.Substring(0, 1);
                    LOS_Level = Reg_value_bin_LOS.Substring(1, 7);
                    //Hien_Thi.Hienthi.SetText(rtb, "LOS_Level_bin = " + LOS_Level);

                    base.Read();
                    base.WriteLine("laser msg --get 6e 1");
                    Thread.Sleep(100);
                    result1_LOS = base.Read();
                    //Hien_Thi.Hienthi.SetText(rtb, result1_LOS);

                    for (int i = 0; i < result1_LOS.Split('\n').Length; i++) {
                        if (result1_LOS.Split('\n')[i].Contains("I2C")) {
                            Reg_value_LOS = result1_LOS.Split('\n')[i].Split(':')[1].Trim();
                            Reg_value_bin_LOS = hex2bin(Reg_value_LOS);
                        }
                    }
                    LOS_bit = Reg_value_bin_LOS.Substring(6, 1);
                    _testinfo.SYSTEMLOG += "LOS_bit = " + LOS_bit + "\r\n";

                    if (LOS_bit == "0") {
                        _testinfo.SYSTEMLOG += "Phát hiện SD. Tiếp tục điều chỉnh tăng LOS Level\r\n";
                        for (int n = 0; n < 58; n++) {
                            _testinfo.SYSTEMLOG += "...\r\n";
                            _testinfo.SYSTEMLOG += "Tiếp tục điều chỉnh tăng LOS Level\r\n";
                            LOS_Level = (Convert.ToInt32((LOS_Level), 2) + Convert.ToInt32("1", 2)).ToString("X");
                            _testinfo.SYSTEMLOG += "LOS_Level_new = " + LOS_Level + "\r\n";
                            base.WriteLine("laser msg --set bc 1 " + LOS_Level);
                            Thread.Sleep(100);

                            base.Read();
                            base.WriteLine("laser msg --get bc 1");
                            Thread.Sleep(100);
                            result1_LOS = base.Read();
                            //Hien_Thi.Hienthi.SetText(rtb, result1_LOS);

                            for (int i = 0; i < result1_LOS.Split('\n').Length; i++) {
                                if (result1_LOS.Split('\n')[i].Contains("I2C")) {
                                    Reg_value_LOS = result1_LOS.Split('\n')[i].Split(':')[1].Trim();
                                    Reg_value_bin_LOS = hex2bin(Reg_value_LOS);
                                }
                            }
                            LOS_SLEEP_CTRL = Reg_value_bin_LOS.Substring(0, 1);
                            LOS_Level = Reg_value_bin_LOS.Substring(1, 7);
                            //Hien_Thi.Hienthi.SetText(rtb, "LOS_Level_bin = " + LOS_Level);

                            base.Read();
                            base.WriteLine("laser msg --get 6e 1");
                            Thread.Sleep(100);
                            result1_LOS = base.Read();
                            //Hien_Thi.Hienthi.SetText(rtb, result1_LOS);

                            for (int i = 0; i < result1_LOS.Split('\n').Length; i++) {
                                if (result1_LOS.Split('\n')[i].Contains("I2C")) {
                                    Reg_value_LOS = result1_LOS.Split('\n')[i].Split(':')[1].Trim();
                                    Reg_value_bin_LOS = hex2bin(Reg_value_LOS);
                                }
                            }
                            LOS_bit = Reg_value_bin_LOS.Substring(6, 1);
                            _testinfo.SYSTEMLOG += "LOS_bit = " + LOS_bit + "\r\n";

                            if (LOS_bit == "1") {
                                _testinfo.SYSTEMLOG += "[OK] Hoàn thành Tuning LOS Level.\r\n";
                                Tuning_LOS_Level_Result = true;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            else if (LOS_bit == "0") {
                for (int m = 0; m < 78; m++) {
                    _testinfo.SYSTEMLOG += "...\r\n";
                    _testinfo.SYSTEMLOG += "Điều chỉnh tăng LOS Level\r\n";
                    LOS_Level = (Convert.ToInt32((LOS_Level), 2) + Convert.ToInt32("1", 2)).ToString("X");
                    _testinfo.SYSTEMLOG += "LOS_Level_new = " + LOS_Level + "\r\n";
                    base.WriteLine("laser msg --set bc 1 " + LOS_Level);
                    Thread.Sleep(100);

                    base.Read();
                    base.WriteLine("laser msg --get bc 1");
                    Thread.Sleep(100);
                    result1_LOS = base.Read();
                    //Hien_Thi.Hienthi.SetText(rtb, result1_LOS);

                    for (int i = 0; i < result1_LOS.Split('\n').Length; i++) {
                        if (result1_LOS.Split('\n')[i].Contains("I2C")) {
                            Reg_value_LOS = result1_LOS.Split('\n')[i].Split(':')[1].Trim();
                            Reg_value_bin_LOS = hex2bin(Reg_value_LOS);
                        }
                    }
                    LOS_SLEEP_CTRL = Reg_value_bin_LOS.Substring(0, 1);
                    LOS_Level = Reg_value_bin_LOS.Substring(1, 7);
                    //Hien_Thi.Hienthi.SetText(rtb, "LOS_Level_bin = " + LOS_Level);

                    base.Read();
                    base.WriteLine("laser msg --get 6e 1");
                    Thread.Sleep(100);
                    result1_LOS = base.Read();
                    //Hien_Thi.Hienthi.SetText(rtb, result1_LOS);

                    for (int i = 0; i < result1_LOS.Split('\n').Length; i++) {
                        if (result1_LOS.Split('\n')[i].Contains("I2C")) {
                            Reg_value_LOS = result1_LOS.Split('\n')[i].Split(':')[1].Trim();
                            Reg_value_bin_LOS = hex2bin(Reg_value_LOS);
                        }
                    }
                    LOS_bit = Reg_value_bin_LOS.Substring(6, 1);
                    _testinfo.SYSTEMLOG += "LOS_bit = " + LOS_bit + "\r\n";

                    if (LOS_bit == "1") {
                        _testinfo.SYSTEMLOG += "[OK] Hoàn thành Tuning LOS Level.\r\n";
                        Tuning_LOS_Level_Result = true;
                        break;
                    }
                }
            }


            _testinfo.SYSTEMLOG += string.Format("- Check LOS: -36dBm\r\n");
            FVA.set_attenuation_level((-36 - double.Parse(VAR.OLT_Power)).ToString());
            Thread.Sleep(1000);
            base.Read();
            base.WriteLine("laser msg --get 6e 1");
            Thread.Sleep(100);
            result1_LOS = base.Read();
            for (int i = 0; i < result1_LOS.Split('\n').Length; i++) {
                if (result1_LOS.Split('\n')[i].Contains("I2C")) {
                    Reg_value_LOS = result1_LOS.Split('\n')[i].Split(':')[1].Trim();
                    Reg_value_bin_LOS = hex2bin(Reg_value_LOS);
                }
            }
            LOS_bit = Reg_value_bin_LOS.Substring(6, 1);
            _testinfo.SYSTEMLOG += "LOS_bit = " + LOS_bit + "\r\n";

            if (LOS_bit == "1") {
                _testinfo.SYSTEMLOG += "Check LOS: PASS\r\n";
                Check_LOS_Result = true;
            }

            _testinfo.SYSTEMLOG += "- Check LOS: -33dBm\r\n";
            FVA.set_attenuation_level((-33 - double.Parse(VAR.OLT_Power)).ToString());
            Thread.Sleep(1000);
            base.Read();
            base.WriteLine("laser msg --get 6e 1");
            Thread.Sleep(100);
            result1_LOS = base.Read();
            for (int i = 0; i < result1_LOS.Split('\n').Length; i++) {
                if (result1_LOS.Split('\n')[i].Contains("I2C")) {
                    Reg_value_LOS = result1_LOS.Split('\n')[i].Split(':')[1].Trim();
                    Reg_value_bin_LOS = hex2bin(Reg_value_LOS);
                }
            }
            LOS_bit = Reg_value_bin_LOS.Substring(6, 1);
            _testinfo.SYSTEMLOG += "LOS_bit = " + LOS_bit + "\r\n";

            if (LOS_bit == "0") {
                _testinfo.SYSTEMLOG += "Check SD: PASS\r\n";
                Check_SD_Result = true;
            }

            bool ret = Tuning_LOS_Level_Result && Check_LOS_Result && Check_SD_Result;
            _testinfo.CALIBLOSRESULT = ret == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
            return ret;
        }


        #region No Use
        //No used
        public override bool checkLOS(bool _flag, bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            throw new NotImplementedException();
        }

        //No used
        public override bool curveDDMI(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            throw new NotImplementedException();
        }

        //No used
        public override bool overloadSensitivity(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            throw new NotImplementedException();
        }

        //No used
        public override bool setVapdAndSlope(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            throw new NotImplementedException();
        }

        //No used
        public override bool writeFlash(bosainfo _bosainfo, testinginfo _testinfo) {
            throw new NotImplementedException();
        }

        #endregion

        #region Support Function 

        private string HexToDec(string Hex) {
            int decValue = int.Parse(Hex, System.Globalization.NumberStyles.HexNumber);
            return decValue.ToString();

        }

        private void Write_To_Register_COM(string register_address, string value) {
            base.WriteLine("laser msg --set " + register_address + " 1 " + value);
            Thread.Sleep(100);
        }

        // Hàm convert offset_Rx dạng thập phân sang dạng Hex
        private string Convert_Offset_RxToHex(float offset_Rx_f) {
            float offset_Rx_Dec;
            string offset_Rx_Hex = "";
            int offset_Rx_Int;
            // code conver sang Hex
            offset_Rx_Dec = (offset_Rx_f / 0.1f) * float.Parse((Math.Pow(2, 12)).ToString());
            offset_Rx_Int = (int)Math.Round((double)offset_Rx_Dec);
            //MessageBox.Show(offset_Rx_Int.ToString());
            //MessageBox.Show(offset_Rx_Dec.ToString());
            if (offset_Rx_Int < 0) {
                offset_Rx_Int = 65536 + offset_Rx_Int;
                offset_Rx_Hex = DecToHex(offset_Rx_Int);
                return offset_Rx_Hex;
            }
            else if (offset_Rx_Int > 0) {
                offset_Rx_Hex = DecToHex(offset_Rx_Int);
                return offset_Rx_Hex;
            }
            else {
                return "0000";
            }

        }

        // Hàm chuyển đổi kiểu Decimal sang Hexa
        private string DecToHex(int dec) {
            string hex = "";
            hex = dec.ToString("X4");
            if (hex.Length > 4) {
                hex = hex.Substring(hex.Length - 4, 4);
            }
            return hex;
        }

        // Hàm tính Slope, Offset: 
        // index = 0: Offset
        // index = 1: Slope_P1
        // index = 2: Slope_P2
        private string Calculate_Slope_Offset_Polynomial(int index, double x1, double x2, double x3, double y1, double y2, double y3) {
            int i, j, k;
            double[] x = { x1, x2, x3 };
            double[] y = { y1, y2, y3 };
            double Slope_P1 = 0;
            double Slope_P2 = 0;
            double Offset = 0;

            int n = 2; // n is the degree of Polynomial
            int N = 3;

            double[] X = new double[2 * n + 1];
            for (i = 0; i < 2 * n + 1; i++) {
                X[i] = 0;
                for (j = 0; j < N; j++)
                    X[i] = X[i] + Math.Pow(x[j], i);        //consecutive positions of the array will store N,sigma(xi),sigma(xi^2),sigma(xi^3)....sigma(xi^2n)
            }

            double[,] B = new double[n + 1, n + 2];
            double[] a = new double[n + 1];
            for (i = 0; i <= n; i++)
                for (j = 0; j <= n; j++)
                    B[i, j] = X[i + j];            //Build the Normal matrix by storing the corresponding coefficients at the right positions except the last column of the matrix
            double[] Y = new double[n + 1];
            for (i = 0; i < n + 1; i++) {
                Y[i] = 0;
                for (j = 0; j < N; j++)
                    Y[i] = Y[i] + Math.Pow(x[j], i) * y[j];        //consecutive positions will store sigma(yi),sigma(xi*yi),sigma(xi^2*yi)...sigma(xi^n*yi)
            }
            for (i = 0; i <= n; i++)
                B[i, n + 1] = Y[i];                //load the values of Y as the last column of B(Normal Matrix but augmented)
            n = n + 1;                //n is made n+1 because the Gaussian Elimination part below was for n equations, but here n is the degree of polynomial and for n degree we get n+1 equations

            for (i = 0; i < n; i++)                    //From now Gaussian Elimination starts(can be ignored) to solve the set of linear equations (Pivotisation)
                for (k = i + 1; k < n; k++)
                    if (B[i, i] < B[k, i])
                        for (j = 0; j <= n; j++) {
                            double temp = B[i, j];
                            B[i, j] = B[k, j];
                            B[k, j] = temp;
                        }

            for (i = 0; i < n - 1; i++)            //loop to perform the gauss elimination
                for (k = i + 1; k < n; k++) {
                    double t = B[k, i] / B[i, i];
                    for (j = 0; j <= n; j++)
                        B[k, j] = B[k, j] - t * B[i, j];    //make the elements below the pivot elements equal to zero or elimnate the variables
                }
            for (i = n - 1; i >= 0; i--)                //back-substitution
            {                        //x is an array whose values correspond to the values of x,y,z..
                a[i] = B[i, n];                //make the variable to be calculated equal to the rhs of the last equation
                for (j = 0; j < n; j++)
                    if (j != i)            //then subtract all the lhs values except the coefficient of the variable whose value                                   is being calculated
                        a[i] = a[i] - B[i, j] * a[j];
                a[i] = a[i] / B[i, i];            //now finally divide the rhs by the coefficient of the variable to be calculated
            }

            Slope_P1 = a[1];
            Slope_P2 = a[2];
            Offset = a[0];

            if (index == 0) {
                return Offset.ToString();
            }

            else if (index == 1) {
                return Slope_P1.ToString();
            }

            else if (index == 2) {
                return Slope_P2.ToString();
            }
            else {
                return "NONE";
            }
            //MessageBox.Show("Slope P1 = " + Slope_P1.ToString() + ", Slope P2 = " + Slope_P2.ToString() + ", Offset = " + Offset.ToString());
        }

        // Hàm convert slope_P1_Rx dạng thập phân sang dạng Hex
        private string Convert_SlopeP1_RxToHex(float slopeP1_Rx_f) {
            float slopeP1_Rx_Dec;
            string slopeP1_Rx_Hex = "";
            int slopeP1_Rx_Int = 0;
            // code convert sang Hex
            //if (slopeP1_Rx_f < 0)
            //{
            //    return "NONE";
            //}
            //else if (slopeP1_Rx_f > 0)
            //{
            slopeP1_Rx_Dec = (slopeP1_Rx_f / 0.1f) * float.Parse((Math.Pow(2, 13)).ToString());
            slopeP1_Rx_Int = (int)Math.Round((double)slopeP1_Rx_Dec);
            slopeP1_Rx_Hex = DecToHex(slopeP1_Rx_Int);
            return slopeP1_Rx_Hex;
            //}

            //else
            //{
            //    return "0000";
            //}


        }

        // Hàm convert slope_P2_Rx dạng thập phân sang dạng Hex
        private string Convert_SlopeP2_RxToHex(float slopeP2_Rx_f) {
            float slopeP2_Rx_Dec;
            string slopeP2_Rx_Hex = "";
            int slopeP2_Rx_Int = 0;
            // code conver sang Hex
            //if (slopeP2_Rx_f < 0)
            //{
            //    return "NONE";
            //}
            //else if (slopeP2_Rx_f > 0)
            //{
            slopeP2_Rx_Dec = (slopeP2_Rx_f / 0.1f) * float.Parse((Math.Pow(2, 30)).ToString());
            slopeP2_Rx_Int = (int)Math.Round((double)slopeP2_Rx_Dec);
            slopeP2_Rx_Hex = DecToHex(slopeP2_Rx_Int);
            return slopeP2_Rx_Hex;
            //}
            //else
            //{
            //    return "0000";
            //}

        }

        private string hex2bin(string value) {
            return Convert.ToString(Convert.ToInt32(value, 16), 2).PadLeft(value.Length * 4, '0');
        }

        #endregion
    }
}
