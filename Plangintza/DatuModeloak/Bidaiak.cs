using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeelmwLogistika.Plangintza.ExelDB;

namespace FeelmwLogistika.Plangintza.DatuModeloak
{
    public class Bidaiak
    {
        private List<Hotelak> hotela;
        private List<Egunak> eguna;
        private int egunKop;
        private Hotelak? hotelHautatua;
        private List<ExelaSortu.EgunLaburpena> egunLaburpenak = new List<ExelaSortu.EgunLaburpena>();
        private List<ExelaSortu.EkintzaDatuak> ekintzaDatuak = new List<ExelaSortu.EkintzaDatuak>();

        public List<Hotelak> Hotela { get => hotela; set => hotela = value; }
        public List<Egunak> Eguna { get => eguna; set => eguna = value; }
        public int EgunKop { get => egunKop; set => egunKop = value; }
        public Hotelak? HotelHautatua { get => hotelHautatua; set => hotelHautatua = value; }
        public List<ExelaSortu.EgunLaburpena> EgunLaburpenak { get => egunLaburpenak; set => egunLaburpenak = value; }
        public List<ExelaSortu.EkintzaDatuak> EkintzaDatuak { get => ekintzaDatuak; set => ekintzaDatuak = value; }

        public Bidaiak(List<Hotelak> LisHot, int e) 
        {
            this.hotela = LisHot;
            this.egunKop = e;
        }
        public Bidaiak(List<Hotelak> LisHot, List<Egunak> LisEgu)
        {
            this.hotela = LisHot;
            this.eguna = LisEgu;
            this.egunKop = LisEgu.Count();
        }

        public Bidaiak(Hotelak? hotela, int egunKop, List<ExelaSortu.EgunLaburpena> egunak, List<ExelaSortu.EkintzaDatuak> ekintzak)
        {
            this.hotelHautatua = hotela;
            this.egunKop = egunKop;
            this.egunLaburpenak = egunak;
            this.ekintzaDatuak = ekintzak;
            this.hotela = hotela == null ? new List<Hotelak>() : new List<Hotelak> { hotela };
            this.eguna = new List<Egunak>();
        }
    }
}
