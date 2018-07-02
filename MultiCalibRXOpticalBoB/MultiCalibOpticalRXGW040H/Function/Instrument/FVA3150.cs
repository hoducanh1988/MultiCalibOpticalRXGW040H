using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ivi.Visa.Interop;
using NationalInstruments.VisaNS;


namespace MultiCalibOpticalRXGW040H.Function {
    public class FVA3150 {

        Object thislock = new Object();
        private MessageBasedSession mbSession;
        private string GPIBAddress = "";

        public FVA3150(string _address) {
            this.GPIBAddress = _address;
        }

        public bool Open(out string message) {
            message = "";
            try {
                mbSession = (MessageBasedSession)NationalInstruments.VisaNS.ResourceManager.GetLocalManager().Open(this.GPIBAddress); //att_handle là địa chỉ GPIB của máy Attenuation
                return true;
            } catch (Exception ex) {
                message = ex.ToString();
                return false;
            }
        }

        public bool Write(string cmd) {
            try {
                mbSession.Write(cmd);
                return true;
            } catch {
                return false;
            }
        }



        public void set_attenuation_level(string attnuation_level) {
            string set_att_command = ":INPUT:ATT " + (attnuation_level);
            mbSession.Write("ATT " + attnuation_level);
        }
    }
}
