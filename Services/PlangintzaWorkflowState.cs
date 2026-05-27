using System.Text.RegularExpressions;
using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public sealed class PlangintzaWorkflowState
{
    public string DokumentuIzena { get; set; } = "MyFeelPlangintza";
    public string Ikastetxea { get; set; } = "";
    public bool Editatzen { get; set; }
    public List<Bidaiak> Bidaiak { get; } = [];
    public byte[]? AzkenExcelBytes { get; set; }
    public string? AzkenExcelFileName { get; set; }
    public byte[]? AzkenDokumentuaBytes { get; set; }
    public string? AzkenDokumentuaFileName { get; set; }
    public List<string> Mezuak { get; } = [];

    public void Clear()
    {
        Bidaiak.Clear();
        AzkenExcelBytes = null;
        AzkenExcelFileName = null;
        AzkenDokumentuaBytes = null;
        AzkenDokumentuaFileName = null;
        Mezuak.Clear();
        Editatzen = false;
    }

    public void StartBlank(string dokumentuIzena)
    {
        Clear();
        DokumentuIzena = string.IsNullOrWhiteSpace(dokumentuIzena) ? "MyFeelPlangintza" : dokumentuIzena.Trim();
    }

    public void LoadForEdit(string dokumentuIzena, IEnumerable<Bidaiak> bidaiak, bool hasiHutsik)
    {
        Clear();
        DokumentuIzena = string.IsNullOrWhiteSpace(dokumentuIzena) ? "MyFeelPlangintza" : dokumentuIzena.Trim();
        Editatzen = true;
        Bidaiak.AddRange(bidaiak);
        if (hasiHutsik)
        {
            Mezuak.Add("Dokumentua kargatu da; formulario berria hutsik dago editatzeko fluxuaren arabera.");
        }
    }

    public void LoadFromLogistika(IEnumerable<Ostalak> ostalak, IEnumerable<Hotelak> hotelak)
    {
        foreach (Ostalak ostala in ostalak)
        {
            Bidaiak? bidaia = CreateFromOstala(ostala, hotelak, out string? abisua);
            if (bidaia is not null)
            {
                UpsertBidaia(bidaia);
            }
            else if (!string.IsNullOrWhiteSpace(abisua))
            {
                Mezuak.Add(abisua);
            }
        }
    }

    public void UpsertBidaia(Bidaiak bidaia)
    {
        if (bidaia.HotelHautatua is null || string.IsNullOrWhiteSpace(bidaia.HotelHautatua.Izena))
        {
            return;
        }

        string hotelIzena = Normalize(bidaia.HotelHautatua.Izena);
        int index = Bidaiak.FindIndex(b => Normalize(b.HotelHautatua?.Izena) == hotelIzena);
        if (index >= 0)
        {
            Bidaiak[index] = bidaia;
        }
        else
        {
            Bidaiak.Add(bidaia);
        }
    }

    public static string DownloadHref(byte[]? bytes)
    {
        return bytes is null || bytes.Length == 0
            ? "#"
            : $"data:application/octet-stream;base64,{Convert.ToBase64String(bytes)}";
    }

    private static Bidaiak? CreateFromOstala(Ostalak? ostala, IEnumerable<Hotelak> hotelak, out string? abisua)
    {
        abisua = null;
        if (ostala is null || string.IsNullOrWhiteSpace(ostala.OstalaIzena))
        {
            abisua = "Logistikako ostatua ez dago beteta; ezin da Plangintzara pasa.";
            return null;
        }

        Hotelak hotela = HotelForPlangintza(ostala, FindHotelByName(hotelak, ostala.OstalaIzena));
        int gauak = Math.Max(0, ostala.Gauak);
        int egunKop = Math.Max(1, gauak + 1);
        List<EgunLaburpena> egunak = [];
        for (int i = 1; i <= egunKop; i++)
        {
            egunak.Add(new EgunLaburpena(i, $"Eguna{i}", "", "", ""));
        }

        return new Bidaiak(hotela, egunKop, egunak, []);
    }

    private static Hotelak HotelForPlangintza(Ostalak ostala, Hotelak? plangintzaHotela)
    {
        string izena = string.IsNullOrWhiteSpace(plangintzaHotela?.Izena) ? ostala.OstalaIzena : plangintzaHotela.Izena;
        string helbideaUrl = string.IsNullOrWhiteSpace(plangintzaHotela?.HelbideaUrl) ? ostala.Helbidea : plangintzaHotela.HelbideaUrl;
        return new Hotelak(plangintzaHotela?.Hiria ?? "", izena, helbideaUrl);
    }

    private static Hotelak? FindHotelByName(IEnumerable<Hotelak> hotelak, string? ostalaIzena)
    {
        if (string.IsNullOrWhiteSpace(ostalaIzena))
        {
            return null;
        }

        string izenNormalizatua = Normalize(ostalaIzena);
        List<Hotelak> hotelZerrenda = hotelak.Where(h => !string.IsNullOrWhiteSpace(h.Izena)).ToList();
        Hotelak? exact = hotelZerrenda.FirstOrDefault(h => Normalize(h.Izena) == izenNormalizatua);
        if (exact is not null)
        {
            return exact;
        }

        List<Hotelak> partial = hotelZerrenda.Where(h => PartialSafe(izenNormalizatua, Normalize(h.Izena))).ToList();
        return partial.Count == 1 ? partial[0] : null;
    }

    private static string Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "" : Regex.Replace(value.Trim(), @"\s+", " ").ToUpperInvariant();
    }

    private static bool PartialSafe(string ostala, string hotela)
    {
        return ostala.Length >= 6 && hotela.Length >= 6 && (hotela.Contains(ostala) || ostala.Contains(hotela));
    }
}
