namespace FeelmwLogistika.Blazor.Models;

public class SheetInfo
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";

    public override string ToString() => Nombre;
}
