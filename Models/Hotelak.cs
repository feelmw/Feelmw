namespace FeelmwLogistika.Blazor.Models;

public class Hotelak
{
    public string Hiria { get; set; } = "";
    public string Izena { get; set; } = "";
    public string HelbideaUrl { get; set; } = "";

    public Hotelak()
    {
    }

    public Hotelak(string h, string i, string u)
    {
        Hiria = h;
        Izena = i;
        HelbideaUrl = u;
    }
}
