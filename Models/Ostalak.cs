namespace FeelmwLogistika.Blazor.Models;

public class Ostalak
{
    public string OstalaIzena { get; set; } = "";
    public string Bonoa { get; set; } = "";
    public string Helbidea { get; set; } = "";
    public string Lokalizatzailea { get; set; } = BalioLehenetsiak.Lokalizatzailea;
    public int Gauak { get; set; } = BalioLehenetsiak.Gauak;
    public string Datak { get; set; } = "";
    public string Gelak { get; set; } = "";
    public string Checkin { get; set; } = "";
    public string Checkout { get; set; } = "";
    public string Dokumentazioa { get; set; } = "";
    public string Harrera { get; set; } = "";
    public string Gosaria { get; set; } = "";
    public string Bazkaria { get; set; } = "";
    public string Afaria { get; set; } = "";
    public bool Toailak { get; set; }
    public bool Izarak { get; set; }
    public string Fidantza { get; set; } = "";
    public string Luggage { get; set; } = "";
    public string Instalazioak { get; set; } = "";

    public Ostalak()
    {
    }

    public Ostalak(string oi, string b, string h, string ci, string co, string doc, string ha, bool t, bool i, string lu, string ins)
    {
        OstalaIzena = oi;
        Bonoa = b;
        Helbidea = h;
        Checkin = ci;
        Checkout = co;
        Dokumentazioa = doc;
        Harrera = ha;
        Toailak = t;
        Izarak = i;
        Luggage = lu;
        Instalazioak = ins;
    }

    public Ostalak(string oi, string b, string h, string l, int g, string d, string ge, string ci, string co, string doc, string ha, string gos, string baz, string afa, bool t, bool i, string f, string lu, string ins)
    {
        OstalaIzena = oi;
        Bonoa = b;
        Helbidea = h;
        Lokalizatzailea = string.IsNullOrWhiteSpace(l) ? BalioLehenetsiak.Lokalizatzailea : l;
        Gauak = g;
        Datak = d;
        Gelak = ge;
        Checkin = ci;
        Checkout = co;
        Dokumentazioa = doc;
        Harrera = ha;
        Gosaria = gos;
        Bazkaria = baz;
        Afaria = afa;
        Toailak = t;
        Izarak = i;
        Fidantza = f;
        Luggage = lu;
        Instalazioak = ins;
    }
}
