namespace FeelmwLogistika.Blazor.Models;

public class EgunLaburpena
{
    public int Eguna { get; set; }
    public string Data { get; set; } = "";
    public string Goisa { get; set; } = "";
    public string Arratsaldea { get; set; } = "";
    public string Gaua { get; set; } = "";

    public EgunLaburpena()
    {
    }

    public EgunLaburpena(int eguna, string data, string goisa, string arratsaldea, string gaua)
    {
        Eguna = eguna;
        Data = data;
        Goisa = goisa;
        Arratsaldea = arratsaldea;
        Gaua = gaua;
    }
}
