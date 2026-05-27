namespace FeelmwLogistika.Blazor.Models;

public class EkintzakPlan
{
    public TimeSpan Ordua { get; set; }
    public string Mota { get; set; } = "";
    public string Deskribapena { get; set; } = "";

    public EkintzakPlan()
    {
    }

    public EkintzakPlan(TimeSpan o, string m, string d)
    {
        Ordua = o;
        Mota = m;
        Deskribapena = d;
    }

    public EkintzakPlan(string m, string d)
    {
        Mota = m;
        Deskribapena = d;
    }
}
