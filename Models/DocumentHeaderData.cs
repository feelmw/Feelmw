namespace FeelmwLogistika.Blazor.Models;

public class DocumentHeaderData
{
    public string Maila { get; set; } = "";
    public string Ikastetxea { get; set; } = "";
    public string Datak { get; set; } = "";
    public string Ibilbidea { get; set; } = "";
    public string Irakasleak { get; set; } = "";
    public string Begiraleak { get; set; } = "";
    public string Ikasleak { get; set; } = "";
    public string Gidariak { get; set; } = "";
    public string PolizaIkasleak { get; set; } = "";
    public string PolizaBegiraleak { get; set; } = "";

    public string IkastetxeaOsatua => string.IsNullOrWhiteSpace(Maila) || string.IsNullOrWhiteSpace(Ikastetxea)
        ? ""
        : $"{Maila.Trim()} {Ikastetxea.Trim()}";

    public DocumentHeaderData Clone()
    {
        return new DocumentHeaderData
        {
            Maila = Maila,
            Ikastetxea = Ikastetxea,
            Datak = Datak,
            Ibilbidea = Ibilbidea,
            Irakasleak = Irakasleak,
            Begiraleak = Begiraleak,
            Ikasleak = Ikasleak,
            Gidariak = Gidariak,
            PolizaIkasleak = PolizaIkasleak,
            PolizaBegiraleak = PolizaBegiraleak
        };
    }
}
