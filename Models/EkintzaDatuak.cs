namespace FeelmwLogistika.Blazor.Models;

public class EkintzaDatuak
{
    public string Eguna { get; set; } = "";
    public string Ordua { get; set; } = "";
    public string Mota { get; set; } = "";
    public string Deskribapena { get; set; } = "";

    public EkintzaDatuak()
    {
    }

    public EkintzaDatuak(string eguna, string ordua, string mota, string deskribapena)
    {
        Eguna = eguna;
        Ordua = ordua;
        Mota = mota;
        Deskribapena = deskribapena;
    }
}
