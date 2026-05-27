namespace FeelmwLogistika.Blazor.Models;

public class Bidaiak
{
    public List<Hotelak> Hotela { get; set; } = [];
    public List<Egunak> Eguna { get; set; } = [];
    public int EgunKop { get; set; }
    public Hotelak? HotelHautatua { get; set; }
    public List<EgunLaburpena> EgunLaburpenak { get; set; } = [];
    public List<EkintzaDatuak> EkintzaDatuak { get; set; } = [];

    public Bidaiak()
    {
    }

    public Bidaiak(List<Hotelak> lisHot, int e)
    {
        Hotela = lisHot;
        EgunKop = e;
    }

    public Bidaiak(List<Hotelak> lisHot, List<Egunak> lisEgu)
    {
        Hotela = lisHot;
        Eguna = lisEgu;
        EgunKop = lisEgu.Count;
    }

    public Bidaiak(Hotelak? hotela, int egunKop, List<EgunLaburpena> egunak, List<EkintzaDatuak> ekintzak)
    {
        HotelHautatua = hotela;
        EgunKop = egunKop;
        EgunLaburpenak = egunak;
        EkintzaDatuak = ekintzak;
        Hotela = hotela is null ? [] : [hotela];
    }
}
