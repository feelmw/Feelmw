namespace FeelmwLogistika.Blazor.Models;

public class DocumentHeaderData
{
    public string Ikastetxea { get; set; } = "";
    public string Datak { get; set; } = "";
    public string Ibilbidea { get; set; } = "";
    public string Irakasleak { get; set; } = "";
    public string Begiraleak { get; set; } = "";
    public string Ikasleak { get; set; } = "";
    public string Gidariak { get; set; } = "";

    public DocumentHeaderData Clone()
    {
        return new DocumentHeaderData
        {
            Ikastetxea = Ikastetxea,
            Datak = Datak,
            Ibilbidea = Ibilbidea,
            Irakasleak = Irakasleak,
            Begiraleak = Begiraleak,
            Ikasleak = Ikasleak,
            Gidariak = Gidariak
        };
    }
}
