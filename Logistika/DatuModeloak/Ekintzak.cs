using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeelmwLogistika.Logistika.DatuModeloak
{
    public class Ekintzak
    {
        // Atributuak
        private string ekintzaIzena;
        private string bonoa;
        private string iraupena;
        private string kontaktua;
        private string elkartokia;
        private string iristean;
        private string eramanM;
        private string bertanM;

        private bool aldagela;
        private bool komuna;

        private string egonlekua;
        private string informazioa;

        // GET - SET
        public string EkintzaIzena { get => ekintzaIzena; set => ekintzaIzena = value; }
        public string Bonoa { get => bonoa; set => bonoa = value; }
        public string Iraupena { get => iraupena; set => iraupena = value; }
        public string Kontaktua { get => kontaktua; set => kontaktua = value; }
        public string Elkartokia { get => elkartokia; set => elkartokia = value; }
        public string Iristean { get => iristean; set => iristean = value; }
        public string EramanM { get => eramanM; set => eramanM = value; }
        public string BertanM { get => bertanM; set => bertanM = value; }

        public bool Aldagela { get => aldagela; set => aldagela = value; }
        public bool Komuna { get => komuna; set => komuna = value; }

        public string Egonlekua { get => egonlekua; set => egonlekua = value; }
        public string Informazioa { get => informazioa; set => informazioa = value; }

        // CONSTRUCTOR

        public Ekintzak(
            string e, string b, string i, string k, string el, string ir, string er, string be, bool a, bool ko, string eg, string info)
        {
            ekintzaIzena = e;
            bonoa = b;
            iraupena = i;
            kontaktua = k;
            elkartokia = el;
            iristean = ir;
            eramanM = er;
            bertanM = be;

            aldagela = a;
            komuna = ko;

            egonlekua = eg;
            informazioa = info;
        }
    }
}
