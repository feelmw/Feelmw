using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeelmwLogistika.Plangintza.DatuModeloak
{
    public class Hotelak
    {
        private string hiria;
        private string izena;
        private string helbideaUrl;

        public string Hiria { get => hiria; set => hiria = value; }
        public string Izena { get => izena; set => izena = value; }
        public string HelbideaUrl { get => helbideaUrl; set => helbideaUrl = value; }

        public Hotelak(string h, string i, string u)
        {
            this.hiria = h;
            this.izena = i;
            this.helbideaUrl = u;
        }
    }
}
