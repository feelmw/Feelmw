using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public sealed class PlangintzaListakService(IGoogleSheetsService googleSheetsService) : IPlangintzaListakService
{
    private const string Workbook = "Plangintza";
    private static readonly string[] AllowedSheets = ["Ostalak", "Ekintzak"];
    private static readonly string[] DefaultEkintzakHeaders = ["Izena", "Deskribapena"];

    public async Task<PlangintzaListak> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Hotelak> ostalak = await ReadOstalakAsync(cancellationToken);
        IReadOnlyList<EkintzakPlan> ekintzak = await ReadEkintzakAsync(cancellationToken);

        return new PlangintzaListak
        {
            Ostalak = ostalak.ToList(),
            Ekintzak = ekintzak.ToList()
        };
    }

    public async Task<IReadOnlyList<Hotelak>> ReadOstalakAsync(CancellationToken cancellationToken = default)
    {
        return ReadOstalakRows(await ReadSheetAsync("Ostalak", cancellationToken)).ToList();
    }

    public async Task<IReadOnlyList<EkintzakPlan>> ReadEkintzakAsync(CancellationToken cancellationToken = default)
    {
        return ReadEkintzakRows(await ReadSheetAsync("Ekintzak", cancellationToken)).ToList();
    }

    public async Task AddEkintzaAsync(EkintzakPlan ekintza, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<IReadOnlyList<string>> rows = await ReadSheetAsync("Ekintzak", cancellationToken);
        IReadOnlyList<string> headers = rows.Count > 0 ? rows[0] : DefaultEkintzakHeaders;
        await googleSheetsService.AddRowAsync(Workbook, "Ekintzak", headers.Select(header => EkintzaValueForHeader(header, ekintza)).ToList(), cancellationToken);
    }

    public Task<IReadOnlyList<IReadOnlyList<string>>> ReadSheetAsync(string sheetName, CancellationToken cancellationToken = default)
    {
        EnsureReady(sheetName);
        return googleSheetsService.ReadSheetAsync(Workbook, sheetName, cancellationToken);
    }

    public Task SaveSheetAsync(string sheetName, IReadOnlyList<IReadOnlyList<string>> rows, CancellationToken cancellationToken = default)
    {
        EnsureReady(sheetName);
        return googleSheetsService.SaveSheetAsync(Workbook, sheetName, rows, cancellationToken);
    }

    private static IEnumerable<Hotelak> ReadOstalakRows(IReadOnlyList<IReadOnlyList<string>> rows)
    {
        if (rows.Count == 0)
        {
            yield break;
        }

        Dictionary<string, int> headers = ReadHeaders(rows[0]);
        bool hasHeader = HasAnyHeader(headers, "izena", "ostala", "hotela");
        foreach (IReadOnlyList<string> row in rows.Skip(hasHeader ? 1 : 0))
        {
            string izena = hasHeader ? ReadByHeader(row, headers, "izena", "ostala", "hotela") : ReadCell(row, 2);
            if (string.IsNullOrWhiteSpace(izena))
            {
                continue;
            }

            yield return new Hotelak(
                hasHeader ? ReadByHeader(row, headers, "hiria") : ReadCell(row, 1),
                izena,
                hasHeader ? ReadByHeader(row, headers, "helbideaurl", "helbidea url", "url") : ReadCell(row, 3));
        }
    }

    private static IEnumerable<EkintzakPlan> ReadEkintzakRows(IReadOnlyList<IReadOnlyList<string>> rows)
    {
        if (rows.Count == 0)
        {
            yield break;
        }

        Dictionary<string, int> headers = ReadHeaders(rows[0]);
        bool hasHeader = HasAnyHeader(headers, "izena", "mota", "ekintza");
        foreach (IReadOnlyList<string> row in rows.Skip(hasHeader ? 1 : 0))
        {
            string izena = hasHeader ? ReadByHeader(row, headers, "izena", "mota", "ekintza") : ReadCell(row, 1);
            if (string.IsNullOrWhiteSpace(izena))
            {
                continue;
            }

            yield return new EkintzakPlan(
                izena,
                hasHeader ? ReadByHeader(row, headers, "deskribapena", "descripcion", "azalpena") : ReadCell(row, 2));
        }
    }

    private static Dictionary<string, int> ReadHeaders(IReadOnlyList<string> headerRow)
    {
        Dictionary<string, int> headers = new(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < headerRow.Count; i++)
        {
            string header = Normalize(headerRow[i]);
            if (!string.IsNullOrWhiteSpace(header) && !headers.ContainsKey(header))
            {
                headers[header] = i + 1;
            }
        }

        return headers;
    }

    private static string ReadByHeader(IReadOnlyList<string> row, IReadOnlyDictionary<string, int> headers, params string[] aliases)
    {
        foreach (string alias in aliases)
        {
            if (headers.TryGetValue(Normalize(alias), out int column))
            {
                return ReadCell(row, column);
            }
        }

        return "";
    }

    private static string ReadCell(IReadOnlyList<string> row, int column)
    {
        int index = column - 1;
        return index >= 0 && index < row.Count ? row[index] ?? "" : "";
    }

    private static string EkintzaValueForHeader(string header, EkintzakPlan ekintza)
    {
        return Normalize(header) switch
        {
            "izena" or "mota" or "ekintza" => ekintza.Mota,
            "deskribapena" or "descripcion" or "azalpena" => ekintza.Deskribapena,
            _ => ""
        };
    }

    private static bool HasAnyHeader(IReadOnlyDictionary<string, int> headers, params string[] aliases)
    {
        return aliases.Any(alias => headers.ContainsKey(Normalize(alias)));
    }

    private static string Normalize(string value)
    {
        return new string((value ?? "")
            .Trim()
            .ToLowerInvariant()
            .Where(char.IsLetterOrDigit)
            .ToArray());
    }

    private void EnsureReady(string sheetName)
    {
        if (!AllowedSheets.Contains(sheetName, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"{sheetName} orria ez dago baimenduta.");
        }

        if (!googleSheetsService.IsConfigured)
        {
            throw new InvalidOperationException("Driveko datu-basea ez dago konfiguratuta.");
        }
    }
}
