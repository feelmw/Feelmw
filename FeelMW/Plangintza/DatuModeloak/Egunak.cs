using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeelmwLogistika.Plangintza.DatuModeloak
{
    public class Egunak
    {
        private DateTime data;
        private string goiza;
        private string arratsaldea;
        private string gaua;
        private List<EkintzakPlan> ekintzak;

        public DateTime Data { get => data; set => data = value; }
        public string Goiza { get => goiza; set => goiza = value; }
        public string Arratsaldea { get => arratsaldea; set => arratsaldea = value; }
        public string Gaua { get => gaua; set => gaua = value; }
        public List<EkintzakPlan> Ekintzak { get => ekintzak; set => ekintzak = value; }

        public Egunak(DateTime d, string g, string a, string gu,List<EkintzakPlan> ekin)
        {
            this.data = d;
            this.goiza = g;
            this.arratsaldea = a;
            this.gaua = gu;
            this.ekintzak = ekin;
        }
    }
}