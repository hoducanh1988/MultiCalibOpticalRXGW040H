using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCalibOpticalRXGW040H.Function.Ont
{
    public abstract class GW : Protocol.Serial
    {
        protected int Delay_modem = 300;
        public GW(string _portname) : base(_portname) { }
    }
}
