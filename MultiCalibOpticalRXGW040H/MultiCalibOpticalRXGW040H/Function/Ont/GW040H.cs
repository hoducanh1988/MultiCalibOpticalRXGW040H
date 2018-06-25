using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiCalibOpticalRXGW040H.Function
{
    public class GW040H : GW
    {
        public GW040H(string _portname) : base(_portname) { }

      

        public override string getMACAddress(testinginfo _testinfo) {
            try {
                _testinfo.SYSTEMLOG += string.Format("Get mac address...\r\n");
                base.Write("ifconfig\n");
                Thread.Sleep(300);
                string _tmpStr = base.Read();
                _tmpStr = _tmpStr.Replace("\r", "").Replace("\n", "").Trim();
                string[] buffer = _tmpStr.Split(new string[] { "HWaddr" }, StringSplitOptions.None);
                _tmpStr = buffer[1].Trim();
                string mac = _tmpStr.Substring(0, 17).Replace(":", "");
                _testinfo.SYSTEMLOG += string.Format("...PASS. {0}\r\n", mac);
                return mac;
            }
            catch (Exception ex) {
                _testinfo.ERRORCODE = "(Mã Lỗi: COT-GM-0001)";
                _testinfo.SYSTEMLOG += string.Format("...FAIL. {0}. {1}\r\n", _testinfo.ERRORCODE, ex.ToString());
                return string.Empty;
            }
        }

        public override bool Login(out string message) {
            message = "";
            try {
                bool _flag = false;
                int index = 0;
                int max = 20;
                while (!_flag) {
                    //Gửi lệnh Enter để ONT về trạng thái đăng nhập
                    message += "Gửi lệnh Enter để truy nhập vào login...\r\n";
                    base.WriteLine("\r\n");
                    Thread.Sleep(250);
                    string data = "";
                    data = base.Read();
                    message += string.Format("Feedback:=> {0}\r\n", data);
                    if (data.Replace("\r", "").Replace("\n", "").Trim().Contains("#")) return true;
                    while (!data.Contains("tc login:")) {
                        data += base.Read();
                        message += string.Format("Feedback:=> {0}\r\n", data);
                        Thread.Sleep(500);
                        if (index >= max) break;
                        else index++;
                    }
                    if (index >= max) break;

                    //Gửi thông tin User
                    base.Write(globalData.initSetting.ONTUSER + "\n");
                    message += "Gửi thông tin user: " + globalData.initSetting.ONTUSER + "...\r\n";

                    //Chờ ONT xác nhận User
                    while (!data.Contains("Password:")) {
                        data += base.Read();
                        message += string.Format("Feedback:=> {0}\r\n", data);
                        Thread.Sleep(500);
                        if (index >= max) break;
                        else index++;
                    }
                    if (index >= max) break;

                    //Gửi thông tin Password
                    base.Write(globalData.initSetting.ONTPASS + "\n");
                    message += "Gửi thông tin password: " + globalData.initSetting.ONTPASS + "...\r\n";

                    //Chờ ONT xác nhận Password
                    while (!data.Contains("root login  on `console'")) {
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

        public override bool loginToONT(testinginfo _testinfo) {
            _testinfo.SYSTEMLOG += string.Format("Verifying type of ONT...\r\n...{0}\r\n", globalData.initSetting.ONTTYPE);
            bool _result = false;
            string _message = "";

            _testinfo.SYSTEMLOG += string.Format("Open comport of ONT {0}...\r\n", _testinfo.COMPORT);
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

        public override bool setVapdAndSlope(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            try {
                FVA.Write("INP:WAVE 1490 NM");
                Thread.Sleep(Delay_suyhao);

                _testinfo.SYSTEMLOG += "STEP 1: SET APD voltage & Slope\r\n";
                _testinfo.VAPDRESULT = Parameters.testStatus.Wait.ToString();

                base.Read();
                base.WriteLine("echo set_flash_register 0x" + VAR.APD_00 + VAR.APD_40 + " 0x30 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                _testinfo.SYSTEMLOG += string.Format("{0}\r\n", base.Read());

                base.WriteLine("echo set_flash_register 0x" + VAR.APD_80 + VAR.APD_C0 + " 0x34 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                _testinfo.SYSTEMLOG += string.Format("{0}\r\n", base.Read());

                VAR.Slope_Up = Convert.ToInt32((Convert.ToDouble(VAR.Slope_Up) * 100)).ToString("X");
                VAR.Slope_Down = Convert.ToInt32((Convert.ToDouble(VAR.Slope_Down) * 100)).ToString("X");

                base.WriteLine("echo set_flash_register_APD 0x00" + VAR.Slope_Up + " 0x00" + VAR.Slope_Down + " 0x" + (Convert.ToInt32((VAR.Vbr - 3) * 100)).ToString("X") + " >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                _testinfo.SYSTEMLOG += string.Format("{0}\r\n", base.Read());

                string APD_DAC = "";
                if (VAR.Vbr - 3 >= 30.2 && VAR.Vbr - 3 <= 37.6) {
                    APD_DAC = Convert.ToInt32((Math.Round((((VAR.Vbr - 3) - 30.2) / (37.6 - 30.2)) * 64))).ToString("X");
                }
                else if (VAR.Vbr - 3 >= 37.6 && VAR.Vbr - 3 <= 44.9) {
                    APD_DAC = Convert.ToInt32((Math.Round((((VAR.Vbr - 3) - 37.6) / (44.9 - 37.6)) * 64) + 64)).ToString("X");
                }
                if (VAR.Vbr - 3 >= 44.9 && VAR.Vbr - 3 <= 50.9) {
                    APD_DAC = Convert.ToInt32((Math.Round((((VAR.Vbr - 3) - 44.9) / (50.9 - 44.9)) * 64) + 128)).ToString("X");
                }
                base.WriteLine("echo APD_DAC 0x" + APD_DAC + " >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                base.WriteLine("echo DDMI_check_8472 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                base.WriteLine("echo apd_ctrl >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                _testinfo.SYSTEMLOG += string.Format("{0}\r\n", base.Read());

                _testinfo.SYSTEMLOG += string.Format("{0}\r\n", "Set APD & Slope: PASS");
                _testinfo.VAPDRESULT = Parameters.testStatus.PASS.ToString();
                return true;
            }
            catch (Exception ex) {
                _testinfo.VAPDRESULT = Parameters.testStatus.FAIL.ToString();
                _testinfo.SYSTEMLOG += ex.ToString() + "\r\n";
                return false;
            }
        }

        public override bool overloadSensitivity(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            try {
                string str = "";
                bool ret1 = false, ret2 = false;
                //------------------------------------------------------------------------------------------//
                _testinfo.SYSTEMLOG += "STEP 2: RX Power Overload test & Sensivity test\r\n";
                _testinfo.OVERLOADRESULT = Parameters.testStatus.PASS.ToString();
                _testinfo.SYSTEMLOG += "- Thiết lập suy hao để ONT nhận mức Power: -8dBm\r\n";
                VAR.Att = -8 - (Convert.ToDouble(VAR.OLT_Power));
                FVA.Write("ATT " + VAR.Att.ToString() + " DB");
                Thread.Sleep(Delay_suyhao);

                for (int j = 0; j < 2; j++) {
                    base.WriteLine("echo GPON_BER 6 >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem * 2);
                    str = base.Read();
                    _testinfo.SYSTEMLOG += str + "\r\n";
                   
                    for (int n = 0; n < str.Split('\n').Length; n++) {
                        if (str.Contains("Pattern Aligned")) {
                            Thread.Sleep(Delay_modem * 3);
                            base.WriteLine("echo err_cnt >/proc/pon_phy/debug");
                            Thread.Sleep(Delay_modem * 2);
                            str = base.Read();
                            _testinfo.SYSTEMLOG += str + "\r\n";

                            for (int m = 0; m < str.Split('\n').Length; m++) {
                                if (str.Contains("0x0")) {
                                    ret1 = true;
                                    _testinfo.SYSTEMLOG += "Test Overload -8dBm: PASS\r\n";
                                    break;
                                }
                                else {
                                    ret1 = false;
                                    _testinfo.SYSTEMLOG += "Test Overload -8dBm: FAIL\r\n";
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    if (ret1 == true)
                        break;
                }
                if (ret1 == false) goto END;

                //------------------------------------------------------------------------------------------//
                _testinfo.SYSTEMLOG += "- Thiết lập suy hao để ONT nhận mức Power: -28dBm\r\n";
                VAR.Att = -28 - (Convert.ToDouble(VAR.OLT_Power));
                FVA.Write("ATT " + VAR.Att.ToString() + " DB");
                Thread.Sleep(Delay_suyhao);

                for (int j = 0; j < 2; j++) {
                    base.WriteLine("echo GPON_BER 6 >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem * 2);
                    str = base.Read();
                    _testinfo.SYSTEMLOG += str + "\r\n";

                    for (int n = 0; n < str.Split('\n').Length; n++) {
                        if (str.Contains("Pattern Aligned")) {
                            Thread.Sleep(Delay_modem * 3);
                            base.WriteLine("echo err_cnt >/proc/pon_phy/debug");
                            Thread.Sleep(Delay_modem * 2);
                            str = base.Read();
                            _testinfo.SYSTEMLOG += str + "\r\n";

                            for (int m = 0; m < str.Split('\n').Length; m++) {
                                if (str.Contains("0x0")) {
                                    ret2 = true;
                                    _testinfo.SYSTEMLOG += "Test Sensitivity -28dBm: PASS\r\n";
                                    break;
                                }
                                else {
                                    ret2 = false;
                                    _testinfo.SYSTEMLOG += "Test Sensitivity -28dBm: FAIL\r\n";
                                    break;
                                }
                            }
                            break;
                        }

                        else {
                            ret2 = false;
                            _testinfo.SYSTEMLOG += "Test Sensitivity -28dBm FAIL. Kết quả Log không chứa Pattern Aligned\r\n";
                            break;
                        }
                    }
                    if (ret2 == true)
                        break;
                    else
                        continue;
                }
                goto END;
                //------------------------------------------------------------------------------------------//
                END:
                _testinfo.OVERLOADRESULT = ret1 && ret2 == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return ret1 && ret2;
            }
            catch (Exception ex) {
                _testinfo.OVERLOADRESULT = Parameters.testStatus.FAIL.ToString();
                _testinfo.SYSTEMLOG += ex.ToString() + "\r\n";
                return false;
            }
        }

        public override bool calibDDMI(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            try {
                string str = "";
                Thread.Sleep(Delay_modem);
                _testinfo.SYSTEMLOG += "STEP 3: RX DDMI Calibration\r\n";
                _testinfo.CALIBDDMIRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "- Thiết lập suy hao để ONT nhận mức Power: -30dBm\r\n";
                VAR.Att = -30 - (Convert.ToDouble(VAR.OLT_Power));
                FVA.Write("ATT " + VAR.Att.ToString() + " DB");
                Thread.Sleep(Delay_suyhao);
                base.WriteLine("echo set_flash_register_DDMI_RxPower 0x0064 0x58 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem * 4);
                str = base.Read();
                _testinfo.SYSTEMLOG += str + "\r\n";
                _testinfo.SYSTEMLOG += "- Thiết lập suy hao để ONT nhận mức Power: -20dBm\r\n";
                VAR.Att = -20 - (Convert.ToDouble(VAR.OLT_Power));
                FVA.Write("ATT " + VAR.Att.ToString() + " DB");
                Thread.Sleep(Delay_suyhao);
                base.WriteLine("echo set_flash_register_DDMI_RxPower 0x03e8 0x54 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem * 4);
                str = base.Read();
                _testinfo.SYSTEMLOG += str + "\r\n";
                _testinfo.SYSTEMLOG += "- Thiết lập suy hao để ONT nhận mức Power: -10dBm\r\n";
                VAR.Att = -10 - (Convert.ToDouble(VAR.OLT_Power));
                FVA.Write("ATT " + VAR.Att.ToString() + " DB");
                Thread.Sleep(Delay_suyhao);
                base.WriteLine("echo set_flash_register_DDMI_RxPower 0x2710 0x50 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem * 4);
                str = base.Read();
                _testinfo.SYSTEMLOG += str + "\r\n";
                _testinfo.CALIBDDMIRESULT = Parameters.testStatus.PASS.ToString();
                _testinfo.SYSTEMLOG += "DDMI Calibration thành công.\r\n";
                return true;
            }
            catch (Exception ex) {
                _testinfo.CALIBDDMIRESULT = Parameters.testStatus.FAIL.ToString();
                _testinfo.SYSTEMLOG += ex.ToString() + "\r\n";
                return false;
            }
        }

        public override bool curveDDMI(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            try {
                //----------------------------------BƯỚC 4: RX DDMI curve check ------------------------------------
                string str = "";
                bool ret1 = false, ret2 = false;
                _testinfo.SYSTEMLOG += "STEP 4: RX DDMI curve check\r\n";
                _testinfo.CURVEDDMIRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "- Thiết lập suy hao để ONT nhận mức Power: -15dBm\r\n";
                VAR.Att = -15 - (Convert.ToDouble(VAR.OLT_Power));
                FVA.Write("ATT " + VAR.Att.ToString() + " DB");
                Thread.Sleep(Delay_suyhao);
                for (int i = 0; i < 2; i++) {
                    base.WriteLine("echo DDMI_check_8472 >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem);
                    str = base.Read();
                    _testinfo.SYSTEMLOG += str + "\r\n";
                    for (int n = 0; n < str.Split('\n').Length; n++) {
                        if (str.Split('\n')[n].Contains("Rx Power")) {
                            str = str.Split('\n')[n].Split('=')[1];
                            double RX_DDMI_Temp = Convert.ToDouble(str);
                            RX_DDMI_Temp = (Math.Log10(RX_DDMI_Temp / 10000)) * 10;
                            _testinfo.SYSTEMLOG += "RX DDMI = " + RX_DDMI_Temp + "\r\n";
                            if (RX_DDMI_Temp > -16.5 && RX_DDMI_Temp < -13.5) {
                                _testinfo.SYSTEMLOG += "Check DDMI -15dBm: PASS\r\n";
                                ret1 = true;
                                break;
                            }
                            else {
                                _testinfo.SYSTEMLOG += "Check DDMI -15dBm: FAIL\r\n";
                                ret1 = false;
                                break;
                            }
                        }
                        else if ((n == str.Split('\n').Length - 1) && !str.Split('\n')[n].Contains("Rx Power")) {
                            _testinfo.SYSTEMLOG += "Check DDMI -15dBm: FAIL\r\n";
                            ret1 = false;
                            break;
                        }
                    }
                    if (ret1 == true) {
                        break;
                    }
                }

                _testinfo.SYSTEMLOG += "- Thiết lập suy hao để ONT nhận mức Power: -25dBm\r\n";
                VAR.Att = -25 - (Convert.ToDouble(VAR.OLT_Power));
                FVA.Write("ATT " + VAR.Att.ToString() + " DB");
                Thread.Sleep(Delay_suyhao);
                for (int i = 0; i < 2; i++) {
                    base.WriteLine("echo DDMI_check_8472 >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem);
                    str = base.Read();
                    _testinfo.SYSTEMLOG += str + "\r\n";
                    for (int n = 0; n < str.Split('\n').Length; n++) {
                        if (str.Split('\n')[n].Contains("Rx Power")) {
                            str = str.Split('\n')[n].Split('=')[1];
                            double RX_DDMI_Temp = Convert.ToDouble(str);
                            RX_DDMI_Temp = (Math.Log10(RX_DDMI_Temp / 10000)) * 10;
                            _testinfo.SYSTEMLOG += "RX DDMI = " + RX_DDMI_Temp + "\r\n";
                            if (RX_DDMI_Temp > -26.5 && RX_DDMI_Temp < -23.5) {
                                _testinfo.SYSTEMLOG += "Check DDMI -25dBm: PASS\r\n";
                                ret2 = true;
                                break;
                            }
                            else {
                                _testinfo.SYSTEMLOG += "Check DDMI -25dBm: FAIL\r\n";
                                ret2 = false;
                                break;
                            }
                        }
                        else if ((n == str.Split('\n').Length - 1) && !str.Split('\n')[n].Contains("Rx Power")) {
                            _testinfo.SYSTEMLOG += "Check DDMI -25dBm: FAIL\r\n";
                            ret2 = false;
                            break;
                        }
                    }
                    if (ret2 == true) {
                        break;
                    }
                }

                _testinfo.CURVEDDMIRESULT = ret1 && ret2 == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return ret1 && ret2;
            }
            catch (Exception ex) {
                _testinfo.CURVEDDMIRESULT = Parameters.testStatus.FAIL.ToString();
                _testinfo.SYSTEMLOG += ex.ToString() + "\r\n";
                return false;
            }
        }

        public override bool calibLOS(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR, ref bool _flag) {
            try {
                string str = "";
                _testinfo.SYSTEMLOG += "STEP 5: LOS Calibration\r\n";
                _testinfo.CALIBLOSRESULT = Parameters.testStatus.Wait.ToString();
                base.WriteLine("echo apd_ctrl >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                base.WriteLine("echo LOS_calibration 7F 00 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                str = base.Read();
                _testinfo.SYSTEMLOG += str + "\r\n";
                _testinfo.SYSTEMLOG += "- Thiết lập suy hao để ONT nhận mức Power: -34dBm\r\n";
                VAR.Att = -34 - (Convert.ToDouble(VAR.OLT_Power));
                FVA.Write("ATT " + VAR.Att.ToString() + " DB");
                Thread.Sleep(Delay_suyhao);
                base.WriteLine("echo cal_LOS >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem * 4);
                str = base.Read();
                _testinfo.SYSTEMLOG += str + "\r\n";
                _testinfo.SYSTEMLOG += "- Thiết lập suy hao để ONT nhận mức Power: -30dBm\r\n";
                VAR.Att = -30 - (Convert.ToDouble(VAR.OLT_Power));
                FVA.Write("ATT " + VAR.Att.ToString() + " DB");
                Thread.Sleep(Delay_suyhao);
                base.WriteLine("echo cal_SD >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem * 4);
                str = base.Read();
                _testinfo.SYSTEMLOG += str + "\r\n";
                base.WriteLine("echo set_flash_register_LOS >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem * 2);
                str = base.Read();
                if (str.Contains("0x7e")) {
                    _testinfo.SYSTEMLOG += str + "\r\n";
                    _testinfo.SYSTEMLOG += "- Thiết lập suy hao để ONT nhận mức Power: -36dBm\r\n";
                    VAR.Att = -36 - (Convert.ToDouble(VAR.OLT_Power));
                    FVA.Write("ATT " + VAR.Att.ToString() + " DB");
                    Thread.Sleep(Delay_suyhao);
                    base.WriteLine("echo cal_LOS >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem * 4);
                    str = base.Read();
                    _testinfo.SYSTEMLOG += str + "\r\n";
                    _testinfo.SYSTEMLOG += "- Thiết lập suy hao để ONT nhận mức Power: -32dBm\r\n";
                    VAR.Att = -32 - (Convert.ToDouble(VAR.OLT_Power));
                    FVA.Write("ATT " + VAR.Att.ToString() + " DB");
                    Thread.Sleep(Delay_suyhao);
                    base.WriteLine("echo cal_SD >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem * 4);
                    str = base.Read();
                    _testinfo.SYSTEMLOG += str + "\r\n";
                    base.WriteLine("echo set_flash_register_LOS >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem * 2);
                    str = base.Read();
                    if (str.Contains("0x7e")) {
                        _testinfo.SYSTEMLOG += str + "\r\n";
                        _testinfo.SYSTEMLOG += "[FAIL] Board gặp trạng thái 7E\r\n";
                        _testinfo.CALIBLOSRESULT = Parameters.testStatus.FAIL.ToString();
                    }
                    else {
                        _testinfo.SYSTEMLOG += str + "\r\n";
                        _testinfo.SYSTEMLOG += "[OK] Thực hiện xong LOS Calibration\r\n";
                        _testinfo.CALIBLOSRESULT = Parameters.testStatus.PASS.ToString();
                    }
                    _flag = true;
                }
                else {
                    _testinfo.CALIBLOSRESULT = Parameters.testStatus.PASS.ToString();
                    _flag = false;
                }
                return _testinfo.CALIBLOSRESULT == Parameters.testStatus.PASS.ToString();
            }
            catch (Exception ex) {
                _testinfo.CALIBLOSRESULT = Parameters.testStatus.FAIL.ToString();
                _testinfo.SYSTEMLOG += ex.ToString() + "\r\n";
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_flag">True = -37, -31 // -35, -30</param>
        /// <param name="_bosainfo"></param>
        /// <param name="_testinfo"></param>
        /// <param name="FVA"></param>
        /// <param name="VAR"></param>
        /// <returns></returns>
        public override bool checkLOS(bool _flag, bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            try {
                string str = "";
                bool ret1 = false, ret2 = false;
                double value1 = _flag == true ? -37 : -35, value2 = _flag == true ? -31 : -30;
                _testinfo.SYSTEMLOG += "STEP 6: LOS Check\r\n";
                _testinfo.CHECKLOSRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += string.Format("- LOS Check: Thiết lập suy hao để ONT nhận mức Power: {0}dBm\r\n", value1);

                VAR.Att = value1 - (Convert.ToDouble(VAR.OLT_Power));
                FVA.Write("ATT " + VAR.Att.ToString() + " DB");
                Thread.Sleep(Delay_suyhao);
                base.WriteLine("echo DDMI_check_8472 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                base.WriteLine("echo show_BoB_information >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                str = base.Read();
                _testinfo.SYSTEMLOG += str + "\r\n";
                for (int n = 0; n < str.Split('\n').Length; n++) {
                    if (str.Split('\n')[n].Contains("LOS =")) {
                        str = str.Split('\n')[n].Split('=')[1];
                        if (str.Contains("1")) {
                            _testinfo.SYSTEMLOG += string.Format("Check LOS ở {0}dBm: PASS\r\n", value1);
                            ret1 = true;
                            break;
                        }
                        else {
                            _testinfo.SYSTEMLOG += string.Format("Check LOS ở {0}dBm: FAIL\r\n", value1);
                            ret1 = false;
                            break;
                        }
                    }
                }

                if (ret1 == true) {
                    _testinfo.SYSTEMLOG += string.Format("- LOS Check: Thiết lập suy hao để ONT nhận mức Power: {0}dBm\r\n", value2);
                    VAR.Att = value2 - (Convert.ToDouble(VAR.OLT_Power));
                    FVA.Write("ATT " + VAR.Att.ToString() + " DB");
                    Thread.Sleep(Delay_suyhao);
                    base.WriteLine("echo DDMI_check_8472 >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem);
                    base.WriteLine("echo show_BoB_information >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem);
                    str = base.Read();
                    _testinfo.SYSTEMLOG += str + "\r\n";
                    for (int n = 0; n < str.Split('\n').Length; n++) {
                        if (str.Split('\n')[n].Contains("LOS =")) {
                            str = str.Split('\n')[n].Split('=')[1];

                            if (str.Contains("0")) {
                                _testinfo.SYSTEMLOG += string.Format("Check LOS ở {0}dBm: PASS", value2);
                                ret2 = true;
                                break;
                            }
                            else {
                                _testinfo.SYSTEMLOG += string.Format("Check LOS ở {0}dBm: FAIL", value2);
                                ret2 = false;
                                break;
                            }
                        }
                    }
                }
                _testinfo.CHECKLOSRESULT = ret1 && ret2 == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return ret1 && ret2;
            }
            catch (Exception ex) {
                _testinfo.CHECKLOSRESULT = Parameters.testStatus.FAIL.ToString();
                _testinfo.SYSTEMLOG += ex.ToString() + "\r\n";
                return false;
            }
        }

        public override bool writeFlash(bosainfo _bosainfo, testinginfo _testinfo) {
            try {
                string str = "";
                _testinfo.SYSTEMLOG += "STEP 7: Save data into Flash\r\n";
                _testinfo.WRITEFLASHRESULT = Parameters.testStatus.Wait.ToString();
                base.WriteLine("echo set_flash_register 0x07050701 0x94 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem * 2);
                str = base.Read();
                _testinfo.SYSTEMLOG += str + "\r\n";
                base.WriteLine("echo save_flash_matrix >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem * 2);
                str = base.Read();
                _testinfo.SYSTEMLOG += str + "\r\n";
                base.WriteLine("mtd writeflash /tmp/7570_bob.conf 160 656896 reservearea"); //dual band
                Thread.Sleep(Delay_modem + 1000);
                str = base.Read();
                _testinfo.SYSTEMLOG += str + "\r\n";
                for (int m = 0; m < 3; m++) {
                    base.WriteLine("echo flash_dump >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem * 5);
                    str = base.Read();
                    _testinfo.SYSTEMLOG += str + "\r\n";
                    if (!str.Contains("0x07050701")) {
                        _testinfo.WRITEFLASHRESULT = Parameters.testStatus.FAIL.ToString();
                    }
                    else {
                        _testinfo.WRITEFLASHRESULT = Parameters.testStatus.PASS.ToString();
                        _testinfo.SYSTEMLOG += "Hoàn thành quá trình Calibration.\r\n";
                        break;
                    }
                }

                if (_testinfo.WRITEFLASHRESULT == Parameters.testStatus.FAIL.ToString()) {
                    _testinfo.SYSTEMLOG += "Không Write được vào Flash\r\n";
                }
                return _testinfo.WRITEFLASHRESULT == Parameters.testStatus.PASS.ToString();
            }
            catch {
                return false;
            }
        }
    }
}
