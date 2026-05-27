namespace FeelmwLogistika.Blazor.Models;

public class Garraioak
{
    public string GarraioaIzena { get; set; } = "";
    public string Eguna { get; set; } = "";
    public string Ordutegia { get; set; } = "";
    public string Lokalizatzailea { get; set; } = BalioLehenetsiak.Lokalizatzailea;
    public string Kontaktua { get; set; } = "";
    public string Elkargunea { get; set; } = "";
    public string Eginbeharrak { get; set; } = "";
    public string Informazioa { get; set; } = "";

    public Garraioak()
    {
    }

    public Garraioak(string g, string e, string o, string l, string k, string el, string eg, string i)
    {
        GarraioaIzena = g;
        Eguna = e;
        Ordutegia = o;
        Lokalizatzailea = string.IsNullOrWhiteSpace(l) ? BalioLehenetsiak.Lokalizatzailea : l;
        Kontaktua = k;
        Elkargunea = el;
        Eginbeharrak = eg;
        Informazioa = i;
    }
}
