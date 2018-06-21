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
        public double OLTPOWER1 {
            get { return Properties.Settings.Default.oltPower1; }
            set {
                Properties.Settings.Default.oltPower1 = value;
                OnPropertyChanged(nameof(OLTPOWER1));
            }
        }
        public double OLTPOWER2 {
            get { return Properties.Settings.Default.oltPower2; }
            set {
                Properties.Settings.Default.oltPower2 = value;
                OnPropertyChanged(nameof(OLTPOWER2));
            }
        }
        public double OLTPOWER3 {
            get { return Properties.Settings.Default.oltPower3; }
            set {
                Properties.Settings.Default.oltPower3 = value;
                OnPropertyChanged(nameof(OLTPOWER3));
            }
        }
        public double OLTPOWER4 {
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
        public double SLOPEUP {
            get { return Properties.Settings.Default.slopeUp; }
            set {
                Properties.Settings.Default.slopeUp = value;
                OnPropertyChanged(nameof(SLOPEUP));
            }
        }
        public double SLOPEDOWN {
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

            SLOPEUP = 0.14;
            SLOPEDOWN = 0.11;
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

        public mainwindowinfo() {
            OPACITY = 1;
        }
    }

    public class bosainfo {
        public string BosaSN { get; set; }
        public string Ith { get; set; }
        public string Vbr { get; set; }
    }


}
