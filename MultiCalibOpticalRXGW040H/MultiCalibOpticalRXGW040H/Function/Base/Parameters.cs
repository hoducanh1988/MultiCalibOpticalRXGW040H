using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCalibOpticalRXGW040H.Function
{
    public class Parameters {

        public static List<string> ListOntType = new List<string>() { "GW020BoB", "GW040H" };
        public static List<string> ListBosaVendor = new List<string>() { "Mentech", "Hisense", "Ezconn" };
        public static List<string> ListComport = null;

        public enum testStatus { NONE = 0, Wait = 1, PASS = 2, FAIL = 3 };

        static Parameters() {
            ListComport = new List<string>();
           for (int i = 1; i < 100; i++) {
                ListComport.Add(string.Format("COM{0}", i));
            }
        }

      
    }
}
