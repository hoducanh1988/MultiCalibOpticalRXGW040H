using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCalibOpticalRXGW040H.Function
{
    public class defaultsetting : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //CAU HINH ONT //----------------------------------------
        public string ONTTYPE {
            get { return Properties.Settings.Default.ontType; }
            set {
                Properties.Settings.Default.ontType = value;
                OnPropertyChanged(nameof(ONTTYPE));
            }
        }
        public string ONTUSER {
            get { return Properties.Settings.Default.ontUser; }
            set {
                Properties.Settings.Default.ontUser = value;
                OnPropertyChanged(nameof(ONTUSER));
            }
        }
        public string ONTPASS {
            get { return Properties.Settings.Default.ontPass; }
            set {
                Properties.Settings.Default.ontPass = value;
                OnPropertyChanged(nameof(ONTPASS));
            }
        }
        //-------------------------------------------------------


        //CAU HINH INSTRUMENT //---------------------------------
        public string GPIB1 {
            get { return Properties.Settings.Default.GPIB1; }
            set {
                Properties.Settings.Default.GPIB1 = value;
                OnPropertyChanged(nameof(GPIB1));
            }
        }
        public string GPIB2 {
            get { return Properties.Settings.Default.GPIB2; }
            set {
                Properties.Settings.Default.GPIB2 = value;
                OnPropertyChanged(nameof(GPIB2));
            }
        }
        public string GPIB3 {
            get { return Properties.Settings.Default.GPIB3; }
            set {
                Properties.Settings.Default.GPIB3 = value;
                OnPropertyChanged(nameof(GPIB3));
            }
        }
        public string GPIB4 {
            get { return Properties.Settings.Default.GPIB4; }
            set {
                Properties.Settings.Default.GPIB4 = value;
                OnPropertyChanged(nameof(GPIB4));
            }
        }
        //-------------------------------------------------------

        //CAU HINH DEBUG //--------------------------------------
        public string DEBUG1 {
            get { return Properties.Settings.Default.debug1; }
            set {
                Properties.Settings.Default.debug1 = value;
                OnPropertyChanged(nameof(DEBUG1));
            }
        }
        public string DEBUG2 {
            get { return Properties.Settings.Default.debug2; }
            set {
                Properties.Settings.Default.debug2 = value;
                OnPropertyChanged(nameof(DEBUG2));
            }
        }
        public string DEBUG3 {
            get { return Properties.Settings.Default.debug3; }
            set {
                Properties.Settings.Default.debug3 = value;
                OnPropertyChanged(nameof(DEBUG3));
            }
        }
        public string DEBUG4 {
            get { return Properties.Settings.Default.debug4; }
            set {
                Properties.Settings.Default.debug4 = value;
                OnPropertyChanged(nameof(DEBUG4));
            }
        }
        //-------------------------------------------------------

        //CAU HINH SUY HAO //------------------------------------
        public string OLTPOWER1 {
            get { return Properties.Settings.Default.oltPower1; }
            set {
                Properties.Settings.Default.oltPower1 = value;
                OnPropertyChanged(nameof(OLTPOWER1));
            }
        }
        public string OLTPOWER2 {
            get { return Properties.Settings.Default.oltPower2; }
            set {
                Properties.Settings.Default.oltPower2 = value;
                OnPropertyChanged(nameof(OLTPOWER2));
            }
        }
        public string OLTPOWER3 {
            get { return Properties.Settings.Default.oltPower3; }
            set {
                Properties.Settings.Default.oltPower3 = value;
                OnPropertyChanged(nameof(OLTPOWER3));
            }
        }
        public string OLTPOWER4 {
            get { return Properties.Settings.Default.oltPower4; }
            set {
                Properties.Settings.Default.oltPower4 = value;
                OnPropertyChanged(nameof(OLTPOWER4));
            }
        }
        //-------------------------------------------------------

        //CAU HINH BOSA //---------------------------------------
        public string BOSATYPE {
            get { return Properties.Settings.Default.bosaType; }
            set {
                Properties.Settings.Default.bosaType = value;
                OnPropertyChanged(nameof(BOSATYPE));
            }
        }
        public int BOSASNLEN {
            get { return Properties.Settings.Default.bosaSNLength; }
            set {
                Properties.Settings.Default.bosaSNLength = value;
                OnPropertyChanged(nameof(BOSASNLEN));
            }
        }
        public string SLOPEUP {
            get { return Properties.Settings.Default.slopeUp; }
            set {
                Properties.Settings.Default.slopeUp = value;
                OnPropertyChanged(nameof(SLOPEUP));
            }
        }
        public string SLOPEDOWN {
            get { return Properties.Settings.Default.slopeDown; }
            set {
                Properties.Settings.Default.slopeDown = value;
                OnPropertyChanged(nameof(SLOPEDOWN));
            }
        }
        public string VAPD00 {
            get { return Properties.Settings.Default.vApd_00; }
            set {
                Properties.Settings.Default.vApd_00 = value;
                OnPropertyChanged(nameof(VAPD00));
            }
        }
        public string VAPD40 {
            get { return Properties.Settings.Default.vApd_40; }
            set {
                Properties.Settings.Default.vApd_40 = value;
                OnPropertyChanged(nameof(VAPD40));
            }
        }
        public string VAPD80 {
            get { return Properties.Settings.Default.vApd_80; }
            set {
                Properties.Settings.Default.vApd_80 = value;
                OnPropertyChanged(nameof(VAPD80));
            }
        }
        public string VAPDC0 {
            get { return Properties.Settings.Default.vApd_C0; }
            set {
                Properties.Settings.Default.vApd_C0 = value;
                OnPropertyChanged(nameof(VAPDC0));
            }
        }
        public string BOSAREPORTFILE {
            get { return Properties.Settings.Default.bosaReportFile; }
            set {
                Properties.Settings.Default.bosaReportFile = value;
                OnPropertyChanged(nameof(BOSAREPORTFILE));
            }
        }
        //-------------------------------------------------------

        public void setDefault() {

            ONTTYPE = "GW040H";
            ONTUSER = "admin";
            ONTPASS = "ttcn@77CN";

            //GPIB1 = "GPIB0::1::INSTR";
            //GPIB2 = "GPIB0::2::INSTR";
            //GPIB3 = "GPIB0::3::INSTR";
            //GPIB4 = "GPIB0::4::INSTR";

            //DEBUG1 = "COM1";
            //DEBUG2 = "COM2";
            //DEBUG3 = "COM3";
            //DEBUG4 = "COM4";

            //OLTPOWER1 = 0;
            //OLTPOWER2 = 0;
            //OLTPOWER3 = 0;
            //OLTPOWER4 = 0;

            BOSATYPE = "Mentech";

            SLOPEUP = "0.14";
            SLOPEDOWN = "0.11";
            VAPD00 = "012E";
            VAPD40 = "0178";
            VAPD80 = "01C1";
            VAPDC0 = "0203";
            //BOSAREPORTFILE = "";
        }
    }
    public class testinginfo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // COMMON ------------------------------//
        string _ontindex;
        public string ONTINDEX {
            get { return _ontindex; }
            set {
                _ontindex = value;
                OnPropertyChanged(nameof(ONTINDEX));
            }
        }
        string _comport;
        public string COMPORT {
            get { return _comport; }
            set {
                _comport = value;
                OnPropertyChanged(nameof(COMPORT));
            }
        }
        string _gpib;
        public string GPIB {
            get { return _gpib; }
            set {
                _gpib = value;
                OnPropertyChanged(nameof(GPIB));
            }
        }
        string _macaddress;
        public string MACADDRESS {
            get { return _macaddress; }
            set {
                _macaddress = value;
                OnPropertyChanged(nameof(MACADDRESS));
            }
        }
        string _bosaserial;
        public string BOSASERIAL {
            get { return _bosaserial; }
            set {
                _bosaserial = value;
                OnPropertyChanged(nameof(BOSASERIAL));
            }
        }
        //-------------------------------------//

        // RESULT -----------------------------//
        string _vapdresult;
        public string VAPDRESULT {
            get { return _vapdresult; }
            set {
                _vapdresult = value;
                OnPropertyChanged(nameof(VAPDRESULT));
            }
        }
        string _overloadresult;
        public string OVERLOADRESULT {
            get { return _overloadresult; }
            set {
                _overloadresult = value;
                OnPropertyChanged(nameof(OVERLOADRESULT));
            }
        }
        string _calibddmiresult;
        public string CALIBDDMIRESULT {
            get { return _calibddmiresult; }
            set {
                _calibddmiresult = value;
                OnPropertyChanged(nameof(CALIBDDMIRESULT));
            }
        }
        string _curveddmiresult;
        public string CURVEDDMIRESULT {
            get { return _curveddmiresult; }
            set {
                _curveddmiresult = value;
                OnPropertyChanged(nameof(CURVEDDMIRESULT));
            }
        }
        string _caliblosresult;
        public string CALIBLOSRESULT {
            get { return _caliblosresult; }
            set {
                _caliblosresult = value;
                OnPropertyChanged(nameof(CALIBLOSRESULT));
            }
        }
        string _checklosresult;
        public string CHECKLOSRESULT {
            get { return _checklosresult; }
            set {
                _checklosresult = value;
                OnPropertyChanged(nameof(CHECKLOSRESULT));
            }
        }
        string _writeflashresult;
        public string WRITEFLASHRESULT {
            get { return _writeflashresult; }
            set {
                _writeflashresult = value;
                OnPropertyChanged(nameof(WRITEFLASHRESULT));
            }
        }
        string _totalresult;
        public string TOTALRESULT {
            get { return _totalresult; }
            set {
                _totalresult = value;
                OnPropertyChanged(nameof(TOTALRESULT));
            }
        }
        //-------------------------------------//

        // TOTAL ------------------------------//
        string _systemlog;
        public string SYSTEMLOG {
            get { return _systemlog; }
            set {
                _systemlog = value;
                OnPropertyChanged(nameof(SYSTEMLOG));
            }
        }
        string _errorcode;
        public string ERRORCODE {
            get { return _errorcode; }
            set {
                _errorcode = value;
                OnPropertyChanged(nameof(ERRORCODE));
            }
        }
        string _ontlog;
        public string ONTLOG {
            get { return _ontlog; }
            set {
                _ontlog = value;
                OnPropertyChanged(nameof(ONTLOG));
            }
        }
        string _buttoncontent;
        public string BUTTONCONTENT {
            get { return _buttoncontent; }
            set {
                _buttoncontent = value;
                OnPropertyChanged(nameof(BUTTONCONTENT));
            }
        }
        bool _buttonenable;
        public bool BUTTONENABLE {
            get { return _buttonenable; }
            set {
                _buttonenable = value;
                OnPropertyChanged(nameof(BUTTONENABLE));
            }
        }
        string _totaltime;
        public string TOTALTIME {
            get { return _totaltime; }
            set {
                _totaltime = value;
                OnPropertyChanged(nameof(TOTALTIME));
            }
        }
        //-------------------------------------//

        public testinginfo() {
            Initialization();
        }

        public void Initialization() {
            this.TOTALTIME = "0";
            this.MACADDRESS = "--";
            this.BOSASERIAL = "--";
            this.VAPDRESULT = Parameters.testStatus.NONE.ToString();
            this.OVERLOADRESULT = Parameters.testStatus.NONE.ToString();
            this.CALIBDDMIRESULT = Parameters.testStatus.NONE.ToString();
            this.CURVEDDMIRESULT = Parameters.testStatus.NONE.ToString();
            this.CALIBLOSRESULT = Parameters.testStatus.NONE.ToString();
            this.CHECKLOSRESULT = Parameters.testStatus.NONE.ToString();
            this.WRITEFLASHRESULT = Parameters.testStatus.NONE.ToString();
            this.TOTALRESULT = Parameters.testStatus.NONE.ToString();
            this.SYSTEMLOG = "";
            this.ONTLOG = "";
            this.ERRORCODE = "";
            this.BUTTONCONTENT = "START";
            this.BUTTONENABLE = true;
        }

    }

    public class mainwindowinfo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        double _opacity = 1;
        public double OPACITY {
            get { return _opacity; }
            set {
                _opacity = value;
                OnPropertyChanged(nameof(OPACITY));
            }
        }
        string _windowtitle;
        public string WINDOWTITLE {
            get { return _windowtitle; }
            set {
                _windowtitle = value;
                OnPropertyChanged(nameof(WINDOWTITLE));
            }
        }

        public mainwindowinfo() {
            OPACITY = 1;
            WINDOWTITLE = string.Format("PHẦN MỀM CALIBRATION RX QUANG ONT {0}", globalData.initSetting.ONTTYPE);
        }
    }

    public class bosainfo {
        public string BosaSN { get; set; }
        public string Ith { get; set; }
        public string Vbr { get; set; }
    }

    public class variables {

        public variables() {
            OLT_Power = "";
            APD_00 = globalData.initSetting.VAPD00;
            APD_40 = globalData.initSetting.VAPD40;
            APD_80 = globalData.initSetting.VAPD80;
            APD_C0 = globalData.initSetting.VAPDC0;
            Slope_Up = globalData.initSetting.SLOPEUP;
            Slope_Down = globalData.initSetting.SLOPEDOWN;
            Vbr = 0;
            Att = 0;
        }

        public string OLT_Power { get; set; }
        public string APD_00 { get; set; }
        public string APD_40 { get; set; }
        public string APD_80 { get; set; }
        public string APD_C0 { get; set; }
        public string Slope_Up { get; set; }
        public string Slope_Down { get; set; }
        public double Vbr { get; set; }
        public double Att { get; set; }
    }
}
