using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCalibOpticalRXGW040H.Function
{
    public class GW020BoB : GW
    {
        public GW020BoB(string _portname) : base(_portname) { }

        public override bool calibDDMI(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            throw new NotImplementedException();
        }

        public override bool calibLOS(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR, ref bool _flag) {
            throw new NotImplementedException();
        }

        public override bool checkLOS(bool _flag, bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            throw new NotImplementedException();
        }

        public override bool curveDDMI(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            throw new NotImplementedException();
        }

        public override string getMACAddress(testinginfo _testinfo) {
            throw new NotImplementedException();
        }

        public override bool Login(out string message) {
            throw new NotImplementedException();
        }

        public override bool loginToONT(testinginfo _testinfo) {
            throw new NotImplementedException();
        }

        public override bool overloadSensitivity(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            throw new NotImplementedException();
        }

        public override bool setVapdAndSlope(bosainfo _bosainfo, testinginfo _testinfo, FVA3150 FVA, variables VAR) {
            throw new NotImplementedException();
        }

        public override bool writeFlash(bosainfo _bosainfo, testinginfo _testinfo) {
            throw new NotImplementedException();
        }
    }
}
