using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MultiCalibOpticalRXGW040H.Function;

namespace MultiCalibOpticalRXGW040H {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        DispatcherTimer timer = null;

        private void setStartupLocation() {
            double scaleX = 1;
            double scaleY = 1;
            //double scaleX = 0.75;
            //double scaleY = 0.8;
            this.Height = SystemParameters.WorkArea.Height * scaleY;
            this.Width = SystemParameters.WorkArea.Width * scaleX;
            this.Top = (SystemParameters.WorkArea.Height * (1 - scaleY)) / 2;
            this.Left = (SystemParameters.WorkArea.Width * (1 - scaleX)) / 2;
        }


        public MainWindow() {
            InitializeComponent();
            setStartupLocation();
            this.DataContext = globalData.mainWindowINFO;

            //Binding data
            this.border_DUT01.DataContext = globalData.testingDataDut1;
            this.border_DUT02.DataContext = globalData.testingDataDut2;
            this.border_DUT03.DataContext = globalData.testingDataDut3;
            this.border_DUT04.DataContext = globalData.testingDataDut4;

            //Load bosa report
            baseFunction.loadBosaReport();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += ((sender, e) => {
                if (globalData.testingDataDut1.BUTTONENABLE == false) {
                    this._dut1scrollViewer.ScrollToEnd();
                }
                if (globalData.testingDataDut2.BUTTONENABLE == false) {
                    this._dut2scrollViewer.ScrollToEnd();
                }
                if (globalData.testingDataDut3.BUTTONENABLE == false) {
                    this._dut3scrollViewer.ScrollToEnd();
                }
                if (globalData.testingDataDut4.BUTTONENABLE == false) {
                    this._dut4scrollViewer.ScrollToEnd();
                }
            });
            timer.Start();

        }


        private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
            //if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            Label l = sender as Label;
            switch (l.Content.ToString()) {
                case "X": {
                        this.Close();
                        break;
                    }
                default: {
                        break;
                    }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            MenuItem mi = sender as MenuItem;
            switch (mi.Header) {
                case "Open Folder LogTest": {
                        string _Str = string.Format("{0}LogTest", AppDomain.CurrentDomain.BaseDirectory);
                        Process.Start(_Str);
                        break;
                    }
                case "Open Folder LogDetail": {
                        string _Str = string.Format("{0}LogDetail", AppDomain.CurrentDomain.BaseDirectory);
                        Process.Start(_Str);
                        break;
                    }
                case "Setting": {
                        this.Opacity = 0.3;
                        settingWindow sw = new settingWindow();
                        sw.ShowDialog();
                        this.Opacity = 1;
                        break;
                    }
                case "Exit": {
                        this.Close();
                        break;
                    }
                case "About": {
                        this.Opacity = 0.3;
                        About about = new About();
                        about.ShowDialog();
                        this.Opacity = 1;
                        break;
                    }
                case "View": {
                        this.Opacity = 0.3;
                        viewWindow vw = new viewWindow();
                        vw.ShowDialog();
                        this.Opacity = 1;
                        break;
                    }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string buttonName = b.Name;
            string _index = buttonName.Substring(buttonName.Length - 1, 1);

            this._resetDisplay(_index); //manual

            this.Opacity = 0.3;
            inputBosaWindow wb = new inputBosaWindow(_index);
            wb.ShowDialog();
            this.Opacity = 1;

            //***BEGIN -----------------------------------------//
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                //Start count time
                System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
                st.Start();

                string _name = buttonName;
                testinginfo testtmp = null;
                baseFunction.get_Testing_Info_By_Name(_name, ref testtmp);

                string _BosaSN = "";
                testtmp.SYSTEMLOG += string.Format("Input Bosa Serial...\r\n...{0}\r\n", testtmp.BOSASERIAL);
                _BosaSN = testtmp.BOSASERIAL;
                if (_BosaSN == "--") return;

                //Get Bosa Information from Bosa Serial
                bosainfo bosaInfo = new bosainfo();

                testtmp.SYSTEMLOG += string.Format("Get Bosa information...\r\n");
                bosaInfo = this._getDataByBosaSN(_BosaSN);
                if (bosaInfo == null) {
                    testtmp.ERRORCODE = "(Mã Lỗi: COT-BS-0001)";
                    testtmp.SYSTEMLOG += string.Format("...FAIL. {0}. Bosa SN is not existed\r\n", testtmp.ERRORCODE);
                    testtmp.TOTALRESULT = Parameters.testStatus.FAIL.ToString();
                    goto END;
                }
                testtmp.SYSTEMLOG += string.Format("...Ith = {0} mA\r\n", bosaInfo.Ith);
                testtmp.SYSTEMLOG += string.Format("...Vbr = {0} V\r\n", bosaInfo.Vbr);
                testtmp.SYSTEMLOG += string.Format("...PASS\r\n");

                //Variables
                variables vari = new variables();
                vari.OLT_Power = _getOltPower(_index);
                vari.Vbr = double.Parse(bosaInfo.Vbr);

                //Calib RX
                testtmp.TOTALRESULT = Parameters.testStatus.Wait.ToString();
                testtmp.BUTTONCONTENT = "STOP"; testtmp.BUTTONENABLE = false;
                bool _result = RunAll(testtmp, bosaInfo, vari);
                testtmp.TOTALRESULT = _result == false ? Parameters.testStatus.FAIL.ToString() : Parameters.testStatus.PASS.ToString();

                END:
                testtmp.SYSTEMLOG += string.Format("\r\n----------------------------\r\nTotal Judged={0}\r\n", testtmp.TOTALRESULT);
                testtmp.BUTTONCONTENT = "START"; testtmp.BUTTONENABLE = true;

                //Stop count time
                st.Stop();
                testtmp.SYSTEMLOG += string.Format("Total time = {0} seconds\r\n", st.ElapsedMilliseconds / 1000);
                testtmp.TOTALTIME += (st.ElapsedMilliseconds / 1000).ToString();

                //save log
                LogFile.Savelogtest(testtmp);
                LogFile.Savelogdetail(testtmp);
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            }));
            t.IsBackground = true;
            t.Start();
            //***END -------------------------------------------//

        }


        //----RUN ALL
        //****************************************************************************************************
        //****************************************************************************************************
        //****************************************************************************************************
        bool RunAll(testinginfo _testtemp, bosainfo _bosainfo, variables _vari) {
            System.Diagnostics.Stopwatch pt = new System.Diagnostics.Stopwatch();
            pt.Start();
            bool _result = false;
            string _message = "";

            GW ontDevice = null;
            FVA3150 instrument = null;

            switch (globalData.initSetting.ONTTYPE) {
                case "GW040H": {
                        ontDevice = new GW040H(_testtemp.COMPORT);
                        break;
                    }
                case "GW020BoB": {
                        ontDevice = new GW020BoB(_testtemp.COMPORT);
                        break;
                    }
                default: return false;
            }

            //Connect to Instrument
            _testtemp.SYSTEMLOG += string.Format("Connect to FVA3150 {0}...\r\n", _testtemp.GPIB);
            instrument = new FVA3150(_testtemp.GPIB);
            if (instrument.Open(out _message) == false) {
                _testtemp.SYSTEMLOG += "...FAIL" + "\r\n";
                _testtemp.SYSTEMLOG += _message + "\r\n";
                goto END;
            }
            _testtemp.SYSTEMLOG += "...PASS" + "\r\n";

            //login to ONT
            if (ontDevice.loginToONT(_testtemp) == false) goto END;

            //Get MAC Address
            _testtemp.MACADDRESS = ontDevice.getMACAddress(_testtemp);
            if (_testtemp.MACADDRESS == string.Empty) { _testtemp.ERRORCODE = "(Mã Lỗi: COT-GM-0001)"; goto END; }

            //Set Vapd + Slope
            if (ontDevice.setVapdAndSlope(_bosainfo, _testtemp, instrument, _vari) == false) goto END;
            pt.Stop();
            _testtemp.SYSTEMLOG += string.Format("Set Vadp + Slope time = {0} ms\r\n", pt.ElapsedMilliseconds);

            //Overload + Sensitivity
            if (ontDevice.overloadSensitivity(_bosainfo, _testtemp, instrument, _vari) == false) goto END;
            pt.Stop();
            _testtemp.SYSTEMLOG += string.Format("Overload + Sensitivity time = {0} ms\r\n", pt.ElapsedMilliseconds);

            //RX DDMI Calibration
            if (ontDevice.calibDDMI(_bosainfo, _testtemp, instrument, _vari) == false) goto END;
            pt.Stop();
            _testtemp.SYSTEMLOG += string.Format("RX DDMI Calibration time = {0} ms\r\n", pt.ElapsedMilliseconds);

            //RX DDMI Curve
            if (ontDevice.curveDDMI(_bosainfo, _testtemp, instrument, _vari) == false) goto END;
            pt.Stop();
            _testtemp.SYSTEMLOG += string.Format("RX DDMI Curve time = {0} ms\r\n", pt.ElapsedMilliseconds);

            //LOS Calibration
            bool _flag = false;
            if (ontDevice.calibLOS(_bosainfo, _testtemp, instrument, _vari, ref _flag) == false) goto END;
            pt.Stop();
            _testtemp.SYSTEMLOG += string.Format("LOS Calibration time = {0} ms\r\n", pt.ElapsedMilliseconds);

            //LOS Check
            if (ontDevice.checkLOS(_flag, _bosainfo, _testtemp, instrument, _vari) == false) goto END;
            pt.Stop();
            _testtemp.SYSTEMLOG += string.Format("LOS check time = {0} ms\r\n", pt.ElapsedMilliseconds);

            //Write Flash
            if (ontDevice.writeFlash(_bosainfo, _testtemp) == false) goto END;
            pt.Stop();
            _testtemp.SYSTEMLOG += string.Format("Write flash time = {0} ms\r\n", pt.ElapsedMilliseconds);

            _result = true;

            END:
            try { ontDevice.Close(); } catch { }
            return _result;
        }


        //----SUB FUNCTION
        //****************************************************************************************************
        //****************************************************************************************************
        //****************************************************************************************************
        bool _resetDisplay(string index) {
            switch (index) {
                case "1": {
                        globalData.testingDataDut1.Initialization();
                        break;
                    }
                case "2": {
                        globalData.testingDataDut2.Initialization();
                        break;
                    }
                case "3": {
                        globalData.testingDataDut3.Initialization();
                        break;
                    }
                case "4": {
                        globalData.testingDataDut4.Initialization();
                        break;
                    }
            }
            return true;
        }

        string _getOltPower(string index) {
            string _ret = "";
            switch (index) {
                case "1": {
                        _ret = globalData.initSetting.OLTPOWER1;
                        break;
                    }
                case "2": {
                        _ret = globalData.initSetting.OLTPOWER2;
                        break;
                    }
                case "3": {
                        _ret = globalData.initSetting.OLTPOWER3;
                        break;
                    }
                case "4": {
                        _ret = globalData.initSetting.OLTPOWER4;
                        break;
                    }
            }
            return _ret;
        }

        bosainfo _getDataByBosaSN(string _bosaSN) {
            if (globalData.listBosaInfo.Count == 0) return null;
            bosainfo tmp = null;
            foreach (var item in globalData.listBosaInfo) {
                if (item.BosaSN == _bosaSN) {
                    tmp = item;
                    break;
                }
            }
            return tmp;
        }
        //****************************************************************************************************
    }
}
