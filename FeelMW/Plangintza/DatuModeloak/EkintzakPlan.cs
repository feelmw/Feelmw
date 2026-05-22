using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeelmwLogistika.Plangintza.DatuModeloak
{
    public class EkintzakPlan
    {
        private TimeSpan ordua;
        private string mota;
        private string deskribapena;

        public TimeSpan Ordua { get => ordua; set => ordua = value; }
        public string Mota { get => mota; set => mota = value; }
        public string Deskribapena { get => deskribapena; set => deskribapena = value; }

        public EkintzakPlan(TimeSpan o, string m, string d)
        {
            this.ordua = o;
            this.mota = m;
            this.deskribapena = d;
        }

        public EkintzakPlan(string m, string d)
        {
            this.mota = m;
            this.deskribapena = d;
        }
    }
}
