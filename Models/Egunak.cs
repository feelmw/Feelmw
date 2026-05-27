namespace FeelmwLogistika.Blazor.Models;

public class Egunak
{
    public DateTime Data { get; set; } = DateTime.Today;
    public string Goiza { get; set; } = "";
    public string Arratsaldea { get; set; } = "";
    public string Gaua { get; set; } = "";
    public List<EkintzakPlan> Ekintzak { get; set; } = [];

    public Egunak()
    {
    }

    public Egunak(DateTime d, string g, string a, string gu, List<EkintzakPlan> ekin)
    {
        Data = d;
        Goiza = g;
        Arratsaldea = a;
        Gaua = gu;
        Ekintzak = ekin;
    }
}
