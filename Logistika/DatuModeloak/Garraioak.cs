using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeelmwLogistika.Logistika.DatuModeloak
{
    public class Garraioak
    {
        private string garraioaIzena;
        private string eguna;
        private string ordutegia;
        private string lokalizatzailea;
        private string kontaktua;
        private string elkargunea;
        private string eginbeharrak;
        private string informazioa;

        // GET - SET

        public string GarraioaIzena { get => garraioaIzena; set => garraioaIzena = value; }
        public string Eguna { get => eguna; set => eguna = value; }
        public string Ordutegia { get => ordutegia; set => ordutegia = value; }
        public string Lokalizatzailea { get => lokalizatzailea; set => lokalizatzailea = value; }
        public string Kontaktua { get => kontaktua; set => kontaktua = value; }
        public string Elkargunea { get => elkargunea; set => elkargunea = value; }
        public string Eginbeharrak { get => eginbeharrak; set => eginbeharrak = value; }
        public string Informazioa { get => informazioa; set => informazioa = value; }

        // CONSTRUCTOR
        public Garraioak(string g, string e, string o, string l, string k, string el, string eg, string i)
        {
            garraioaIzena = g;
            eguna = e;
            ordutegia = o;
            lokalizatzailea = l;
            kontaktua = k;
            elkargunea = el;
            eginbeharrak = eg;
            informazioa = i;
        }
    }
}
