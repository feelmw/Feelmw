namespace FeelmwLogistika.Blazor.Models;

public class LogisPlanZubia
{
    public string Izena { get; set; } = "";
    public IReadOnlyList<Ostalak> Ostalak { get; init; } = [];
    public IReadOnlyList<Bidaiak> Bidaiak { get; init; } = [];
}
