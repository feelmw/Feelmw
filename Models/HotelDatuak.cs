namespace FeelmwLogistika.Blazor.Models;

public class HotelDatuak
{
    public string Hiria { get; set; } = "";
    public string Izena { get; set; } = "";
    public string HelbideaUrl { get; set; } = "";
    public int EgunKop { get; set; }
    public string Data { get; set; } = "";

    public HotelDatuak()
    {
    }

    public HotelDatuak(string hiria, string izena, string helbideaUrl, int egunKop, string data)
    {
        Hiria = hiria;
        Izena = izena;
        HelbideaUrl = helbideaUrl;
        EgunKop = egunKop;
        Data = data;
    }
}
