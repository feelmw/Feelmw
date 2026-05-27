namespace FeelmwLogistika.Blazor.Models;

public class Ekintzak
{
    public string EkintzaIzena { get; set; } = "";
    public string Bonoa { get; set; } = "";
    public string Iraupena { get; set; } = "";
    public string Kontaktua { get; set; } = "";
    public string Elkartokia { get; set; } = "";
    public string Iristean { get; set; } = "";
    public string EramanM { get; set; } = "";
    public string BertanM { get; set; } = "";
    public bool Aldagela { get; set; }
    public bool Komuna { get; set; }
    public string Egonlekua { get; set; } = "";
    public string Informazioa { get; set; } = "";
    public string Lokali { get; set; } = BalioLehenetsiak.Lokalizatzailea;

    public Ekintzak()
    {
    }

    public Ekintzak(string e, string b, string i, string k, string el, string ir, string er, string be, bool a, bool ko, string eg, string info, string lo = BalioLehenetsiak.Lokalizatzailea)
    {
        EkintzaIzena = e;
        Bonoa = b;
        Iraupena = i;
        Kontaktua = k;
        Elkartokia = el;
        Iristean = ir;
        EramanM = er;
        BertanM = be;
        Aldagela = a;
        Komuna = ko;
        Egonlekua = eg;
        Informazioa = info;
        Lokali = string.IsNullOrWhiteSpace(lo) ? BalioLehenetsiak.Lokalizatzailea : lo;
    }
}
