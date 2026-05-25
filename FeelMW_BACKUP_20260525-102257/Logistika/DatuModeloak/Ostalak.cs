using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using FeelmwLogistika;

namespace FeelmwLogistika.Logistika.DatuModeloak
{
    public class Ostalak
    {
            // ATRIBUTOS

            private string ostalaIzena = "";
            private string bonoa = "";
            private string helbidea = "";
            private string lokalizatzailea = BalioLehenetsiak.Lokalizatzailea;
            private int gauak;
            private string datak = "";
            private string gelak = "";
            private string checkin = "";
            private string checkout = "";
            private string dokumentazioa = "";
            private string harrera = "";

            private string gosariates = "";
            private string bazkariates = "";
            private string afariates = "";

            private bool toailak;
            private bool izarak;
            private bool fidantza;
            private string fidantzaKuota = "";
            private bool luggage;
            private string luggageKuota = "";

            private string instalazioak = "";
            private bool esKlasikoa;

            // GET - SET

            public string OstalaIzena { get => ostalaIzena; set => ostalaIzena = value ?? ""; }
            public string Bonoa { get => bonoa; set => bonoa = value ?? ""; }
            public string Helbidea { get => helbidea; set => helbidea = value ?? ""; }
            public string Lokalizatzailea { get => lokalizatzailea; set => lokalizatzailea = value ?? ""; }
            public int Gauak { get => gauak; set => gauak = value; }
            public string Datak { get => datak; set => datak = value ?? ""; }
            public string Gelak { get => gelak; set => gelak = value ?? ""; }

            public string Checkin { get => checkin; set => checkin = value ?? ""; }
            public string Checkout { get => checkout; set => checkout = value ?? ""; }

            public string Dokumentazioa { get => dokumentazioa; set => dokumentazioa = value ?? ""; }
            public string Harrera { get => harrera; set => harrera = value ?? ""; }
            public string Gosariates { get => gosariates; set => gosariates = value ?? ""; }
            public string Bazkariates { get => bazkariates; set => bazkariates = value ?? ""; }
            public string Afariates { get => afariates; set => afariates = value ?? ""; }
            public bool Toailak { get => toailak; set => toailak = value; }
            public bool Izarak { get => izarak; set => izarak = value; }
            public bool Fidantza { get => fidantza; set => fidantza = value; }
            public string FidantzaKuota { get => fidantzaKuota; set => fidantzaKuota = value ?? ""; }
            public bool Luggage { get => luggage; set => luggage = value; }
            public string LuggageKuota { get => luggageKuota; set => luggageKuota = value ?? ""; }
            public string Instalazioak { get => instalazioak; set => instalazioak = value ?? ""; }
            public bool EsKlasikoa { get => esKlasikoa; set => esKlasikoa = value; }
            public bool XehetasunOsagarriakErakutsi => !esKlasikoa;

        // CONSTRUCTOR

        public Ostalak(string oi, string b, string h, string ci, string co, string doc, string ha, bool t, bool i, bool lu, string luK, string ins)
        {
            OstalaIzena = oi;
            Bonoa = b;
            Helbidea = h;
            Checkin = ci;
            Checkout = co;
            Dokumentazioa = doc;
            Harrera = ha;
            Lokalizatzailea = BalioLehenetsiak.Lokalizatzailea;
            Gauak = BalioLehenetsiak.Gauak;
            datak = "";
            gelak = "";
            toailak = t;
            izarak = i;
            luggage = lu;
            LuggageKuota = luK;
            Instalazioak = ins;
            esKlasikoa = false;
        }
        public Ostalak(string oi, string b, string h, string l, int g, string d, string ge, string ci, string co, string doc, string ha, string gosK, string bazK, string afaK, bool t, bool i, bool f, string fK, bool lu, string luK, string ins, bool esKlasikoa = false)
            {
                OstalaIzena = oi;
                Bonoa = b;
                Helbidea = h;
                this.esKlasikoa = esKlasikoa;
                Lokalizatzailea = l;
                Gauak = g;
                Datak = d;
                Gelak = ge;
                Checkin = ci;
                Checkout = co;
                Dokumentazioa = doc;
                Harrera = ha;
                Gosariates = gosK;
                Bazkariates = bazK;
                Afariates = afaK;
                toailak = t;
                izarak = i;
                fidantza = f;
                FidantzaKuota = fK;
                luggage = lu;
                LuggageKuota = luK;
                Instalazioak = ins;
            }
        }
    }
