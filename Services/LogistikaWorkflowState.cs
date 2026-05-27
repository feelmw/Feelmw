using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public sealed class LogistikaWorkflowState
{
    public string Mota { get; set; } = "";
    public string DokumentuIzena { get; set; } = "MyFeelLogistika";
    public string? IdEditatuDokumentua { get; set; }
    public bool Editatzen { get; set; }
    public List<Ostalak> Ostalak { get; } = [];
    public List<Ekintzak> Ekintzak { get; } = [];
    public List<Garraioak> Garraioak { get; } = [];
    public byte[]? AzkenExcelBytes { get; set; }
    public string? AzkenExcelFileName { get; set; }
    public byte[]? AzkenDokumentuaBytes { get; set; }
    public string? AzkenDokumentuaFileName { get; set; }
    public List<string> AzkenMezuak { get; } = [];

    public void StartNew(string mota, string dokumentuIzena)
    {
        ClearData();
        Mota = mota;
        DokumentuIzena = string.IsNullOrWhiteSpace(dokumentuIzena) ? "MyFeelLogistika" : dokumentuIzena.Trim();
        Editatzen = false;
        IdEditatuDokumentua = null;
    }

    public void StartEditing(string dokumentuIzena, IEnumerable<Ostalak> ostalak, IEnumerable<Ekintzak> ekintzak, IEnumerable<Garraioak> garraioak)
    {
        ClearData();
        DokumentuIzena = string.IsNullOrWhiteSpace(dokumentuIzena) ? "MyFeelLogistika" : dokumentuIzena.Trim();
        Editatzen = true;
        Ostalak.AddRange(ostalak);
        Ekintzak.AddRange(ekintzak);
        Garraioak.AddRange(garraioak);
    }

    public void ClearData()
    {
        Ostalak.Clear();
        Ekintzak.Clear();
        Garraioak.Clear();
        AzkenExcelBytes = null;
        AzkenExcelFileName = null;
        AzkenDokumentuaBytes = null;
        AzkenDokumentuaFileName = null;
        AzkenMezuak.Clear();
    }

    public LogistikaDatuak ToDatuak()
    {
        return new LogistikaDatuak
        {
            Ostalak = Ostalak.ToList(),
            Ekintzak = Ekintzak.ToList(),
            Garraioak = Garraioak.ToList()
        };
    }

    public static string DownloadHref(byte[]? bytes)
    {
        return bytes is null || bytes.Length == 0
            ? "#"
            : $"data:application/octet-stream;base64,{Convert.ToBase64String(bytes)}";
    }
}
