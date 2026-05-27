using ClosedXML.Excel;
using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public sealed class LogistikaDataService(ILogistikaDbService logistikaDbService) : ILogistikaDataService
{
    public async Task<LogistikaDatuak> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Ostalak> ostalak = await ReadOstalakAsync(cancellationToken);
        IReadOnlyList<Ekintzak> ekintzak = await ReadEkintzakAsync(cancellationToken);
        IReadOnlyList<Garraioak> garraioak = await ReadGarraioakAsync(cancellationToken);

        return new LogistikaDatuak
        {
            Ostalak = ostalak.ToList(),
            Ekintzak = ekintzak.ToList(),
            Garraioak = garraioak.ToList()
        };
    }

    public Task<IReadOnlyList<Ostalak>> ReadOstalakAsync(CancellationToken cancellationToken = default)
    {
        return logistikaDbService.ReadOstalakAsync(cancellationToken);
    }

    public Task<IReadOnlyList<Ekintzak>> ReadEkintzakAsync(CancellationToken cancellationToken = default)
    {
        return logistikaDbService.ReadEkintzakAsync(cancellationToken);
    }

    public Task<IReadOnlyList<Garraioak>> ReadGarraioakAsync(CancellationToken cancellationToken = default)
    {
        return logistikaDbService.ReadGarraioakAsync(cancellationToken);
    }

    public Task AddOstalaAsync(Ostalak ostala, CancellationToken cancellationToken = default)
    {
        return logistikaDbService.AddOstalaAsync(ostala, cancellationToken);
    }

    public Task AddEkintzaAsync(Ekintzak ekintza, CancellationToken cancellationToken = default)
    {
        return logistikaDbService.AddEkintzaAsync(ekintza, cancellationToken);
    }

    public Task<LogistikaDatuak> ReadExcelAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using XLWorkbook workbook = new(stream);

        return Task.FromResult(new LogistikaDatuak
        {
            Ostalak = ReadOstalakWorksheet(workbook).ToList(),
            Ekintzak = ReadEkintzakWorksheet(workbook).ToList(),
            Garraioak = ReadGarraioakWorksheet(workbook).ToList()
        });
    }

    private static IEnumerable<Ostalak> ReadOstalakWorksheet(XLWorkbook workbook)
    {
        if (!workbook.Worksheets.TryGetWorksheet("Ostalak", out IXLWorksheet? sheet) || sheet.RangeUsed() is null)
        {
            yield break;
        }

        foreach (IXLRangeRow row in sheet.RangeUsed()!.RowsUsed())
        {
            if (ExcelRowHutsa(row, 17) || GoiburuaDa(Gelaxka(row, 1), "ostala", "izena", "hotela"))
            {
                continue;
            }

            bool formatuBerria = OstalaFormatuBerriaDa(row);
            string ostala = Gelaxka(row, 1);
            if (string.IsNullOrWhiteSpace(ostala))
            {
                continue;
            }

            yield return new Ostalak(
                ostala,
                Gelaxka(row, 2),
                Gelaxka(row, 3),
                formatuBerria ? Gelaxka(row, 4) : BalioLehenetsiak.Lokalizatzailea,
                int.TryParse(formatuBerria ? Gelaxka(row, 5) : BalioLehenetsiak.Gauak.ToString(), out int gauakKop) ? gauakKop : 0,
                formatuBerria ? Gelaxka(row, 6) : "",
                formatuBerria ? Gelaxka(row, 7) : "",
                formatuBerria ? Gelaxka(row, 8) : Gelaxka(row, 4),
                formatuBerria ? Gelaxka(row, 9) : Gelaxka(row, 5),
                formatuBerria ? Gelaxka(row, 10) : Gelaxka(row, 6),
                formatuBerria ? Gelaxka(row, 11) : Gelaxka(row, 8),
                formatuBerria ? Gelaxka(row, 12) : Gelaxka(row, 12),
                formatuBerria ? Gelaxka(row, 13) : Gelaxka(row, 13),
                formatuBerria ? Gelaxka(row, 14) : Gelaxka(row, 14),
                Parseatu(formatuBerria ? Gelaxka(row, 15) : Gelaxka(row, 9)),
                Parseatu(formatuBerria ? Gelaxka(row, 16) : Gelaxka(row, 10)),
                ResolveFidantza(row, formatuBerria),
                ResolveLuggage(row, formatuBerria),
                formatuBerria ? ResolveInstalazioak(row) : Gelaxka(row, 11));
        }
    }

    private static IEnumerable<Ekintzak> ReadEkintzakWorksheet(XLWorkbook workbook)
    {
        if (!workbook.Worksheets.TryGetWorksheet("Ekintzak", out IXLWorksheet? sheet) || sheet.RangeUsed() is null)
        {
            yield break;
        }

        foreach (IXLRangeRow row in sheet.RangeUsed()!.RowsUsed())
        {
            if (ExcelRowHutsa(row, 13) || GoiburuaDa(Gelaxka(row, 1), "ekintza", "izena"))
            {
                continue;
            }

            string ekintza = Gelaxka(row, 1);
            if (string.IsNullOrWhiteSpace(ekintza))
            {
                continue;
            }

            yield return new Ekintzak(
                ekintza,
                Gelaxka(row, 2),
                Gelaxka(row, 3),
                Gelaxka(row, 4),
                Gelaxka(row, 5),
                Gelaxka(row, 6),
                Gelaxka(row, 7),
                Gelaxka(row, 8),
                Parseatu(Gelaxka(row, 9)),
                Parseatu(Gelaxka(row, 10)),
                Gelaxka(row, 11),
                Gelaxka(row, 12),
                Gelaxka(row, 13));
        }
    }

    private static IEnumerable<Garraioak> ReadGarraioakWorksheet(XLWorkbook workbook)
    {
        if (!workbook.Worksheets.TryGetWorksheet("Garraioak", out IXLWorksheet? sheet) || sheet.RangeUsed() is null)
        {
            yield break;
        }

        foreach (IXLRangeRow row in sheet.RangeUsed()!.RowsUsed())
        {
            if (ExcelRowHutsa(row, 8) || GoiburuaDa(Gelaxka(row, 1), "garraioa", "izena"))
            {
                continue;
            }

            string garraioa = Gelaxka(row, 1);
            if (string.IsNullOrWhiteSpace(garraioa))
            {
                continue;
            }

            yield return new Garraioak(
                garraioa,
                Gelaxka(row, 2),
                Gelaxka(row, 3),
                Gelaxka(row, 4),
                Gelaxka(row, 5),
                Gelaxka(row, 6),
                Gelaxka(row, 7),
                Gelaxka(row, 8));
        }
    }

    private static string Gelaxka(IXLRangeRow row, int zutabea) => row.Cell(zutabea).GetFormattedString() ?? "";

    private static bool ExcelRowHutsa(IXLRangeRow row, int zutabeKop)
    {
        for (int i = 1; i <= zutabeKop; i++)
        {
            if (!string.IsNullOrWhiteSpace(Gelaxka(row, i)))
            {
                return false;
            }
        }

        return true;
    }

    private static bool GoiburuaDa(string lehenZutabea, params string[] aukerak)
    {
        string balioa = (lehenZutabea ?? "").Trim();
        return aukerak.Any(a => string.Equals(balioa, a, StringComparison.OrdinalIgnoreCase));
    }

    private static bool OstalaFormatuBerriaDa(IXLRangeRow row)
    {
        if (int.TryParse(Gelaxka(row, 5), out _)
            || Gelaxka(row, 6).Contains(" - ")
            || string.Equals(Gelaxka(row, 4), BalioLehenetsiak.Lokalizatzailea, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        for (int zutabea = 17; zutabea <= 19; zutabea++)
        {
            if (!string.IsNullOrWhiteSpace(Gelaxka(row, zutabea)))
            {
                return true;
            }
        }

        return false;
    }

    private static string ResolveInstalazioak(IXLRangeRow row)
    {
        return Gelaxka(row, 19);
    }

    private static string ResolveFidantza(IXLRangeRow row, bool formatuBerria)
    {
        return formatuBerria ? Gelaxka(row, 17) : Gelaxka(row, 15);
    }

    private static string ResolveLuggage(IXLRangeRow row, bool formatuBerria)
    {
        return formatuBerria ? Gelaxka(row, 18) : Gelaxka(row, 7);
    }

    private static bool Parseatu(string balioa)
    {
        return string.Equals(balioa, "Bai", StringComparison.OrdinalIgnoreCase)
            || string.Equals(balioa, "true", StringComparison.OrdinalIgnoreCase);
    }
}
