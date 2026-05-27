namespace FeelmwLogistika.Blazor.Models;

public class PlangintzaDatuak
{
    public string Ostala { get; set; } = "";
    public HotelDatuak? Hotela { get; set; }
    public int EgunKop { get; set; } = 1;
    public string Data { get; set; } = "";
    public List<EgunLaburpena> Egunak { get; set; } = [];
    public List<EkintzaDatuak> Ekintzak { get; set; } = [];
    public List<Bidaiak> Bidaiak { get; set; } = [];
}
