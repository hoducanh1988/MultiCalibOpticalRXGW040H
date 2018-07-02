using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCalibOpticalRXGW040H.Function
{
    public abstract class GW : Protocol.Serial
    {
        protected int Delay_suyhao = 3000;
        protected int Delay_modem = 300;
        public GW(string _portname) : base(_portname) { }

        public abstract bool Login(out string message);

        //Calib Quang------------------------------//
        public abstract bool loginToONT(testinginfo _testinfo);
        public abstract string getMACAddress(testinginfo _testinfo);
        public abstract bool setVapdAndSlope(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR);
        public abstract bool overloadSensitivity(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR);
        public abstract bool calibDDMI(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR);
        public abstract bool curveDDMI(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR);
        public abstract bool calibLOS(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR,ref bool _flag);
        public abstract bool checkLOS(bool _flag, bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR);
        public abstract bool writeFlash(bosainfo _bosainfo, testinginfo _testinfo);
        //Calib Quang------------------------------//

    }
}
