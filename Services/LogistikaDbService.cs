using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public sealed class LogistikaDbService(IGoogleSheetsService googleSheetsService) : ILogistikaDbService
{
    private const string Workbook = "Logistika";
    private static readonly string[] AllowedSheets = ["Ostalak", "Ekintzak", "Garraioak"];
    private static readonly string[] DefaultOstalakHeaders = ["Ostala", "Bonoa", "Helbidea", "Lokalizatzailea", "Gauak", "Datak", "Gelak", "Checkin", "Checkout", "Doku", "Harreta ordutegia", "Toallak barne", "Izarak barne", "Instalazioak", "Gosaria", "Bazkaria", "Afaria", "Fidantza prezioa", "Luggage prezioa"];
    private static readonly string[] DefaultEkintzakHeaders = ["Ekintza", "Bonoa", "Iraupena", "Kontaktua", "Elkartokia", "Iristean", "EramanM", "BertanM", "Aldageal", "Komunak", "Egonlekua", "Informazioa"];

    public async Task<IReadOnlyList<Ostalak>> ReadOstalakAsync(CancellationToken cancellationToken = default)
    {
        return ReadOstalakRows(await ReadSheetAsync("Ostalak", cancellationToken)).ToList();
    }

    public async Task<IReadOnlyList<Ekintzak>> ReadEkintzakAsync(CancellationToken cancellationToken = default)
    {
        return ReadEkintzakRows(await ReadSheetAsync("Ekintzak", cancellationToken)).ToList();
    }

    public async Task<IReadOnlyList<Garraioak>> ReadGarraioakAsync(CancellationToken cancellationToken = default)
    {
        return ReadGarraioakRows(await ReadSheetAsync("Garraioak", cancellationToken)).ToList();
    }

    public async Task AddOstalaAsync(Ostalak ostala, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<IReadOnlyList<string>> rows = await ReadSheetAsync("Ostalak", cancellationToken);
        List<string> headers = rows.Count > 0 ? rows[0].ToList() : DefaultOstalakHeaders.ToList();
        EnsureOstalaHeaders(headers);

        List<IReadOnlyList<string>> rowsToSave = [headers];
        rowsToSave.AddRange(rows.Skip(1).Select(row => NormalizeRow(row, headers.Count)));
        rowsToSave.Add(headers.Select(header => OstalaValueForHeader(NormalizeHeader(header), ostala)).ToList());

        await SaveSheetAsync("Ostalak", rowsToSave, cancellationToken);
    }

    public async Task AddEkintzaAsync(Ekintzak ekintza, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<IReadOnlyList<string>> rows = await ReadSheetAsync("Ekintzak", cancellationToken);
        IReadOnlyList<string> headers = rows.Count > 0 ? rows[0] : DefaultEkintzakHeaders;
        await googleSheetsService.AddRowAsync(Workbook, "Ekintzak", headers.Select(header => EkintzaValueForHeader(NormalizeHeader(header), ekintza)).ToList(), cancellationToken);
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

    private static IEnumerable<Ostalak> ReadOstalakRows(IReadOnlyList<IReadOnlyList<string>> rows)
    {
        if (rows.Count == 0)
        {
            yield break;
        }

        bool hasHeader = HeaderDa(RowCell(rows[0], 1), "ostala", "ostalaizena", "ostala izena", "izena", "hotela");
        Dictionary<string, int> headers = hasHeader ? HeaderMap(rows[0]) : [];
        foreach (IReadOnlyList<string> row in rows.Skip(hasHeader ? 1 : 0))
        {
            if (RowHutsa(row))
            {
                continue;
            }

            Ostalak item = hasHeader ? ReadOstalaByHeader(row, headers) : ReadOstalaByPosition(row);
            if (!string.IsNullOrWhiteSpace(item.OstalaIzena))
            {
                yield return item;
            }
        }
    }

    private static Ostalak ReadOstalaByHeader(IReadOnlyList<string> row, IReadOnlyDictionary<string, int> headers)
    {
        return new Ostalak(
            HeaderValue(row, headers, "ostalaizena", "ostala", "izena", "hotela"),
            HeaderValue(row, headers, "bonoa"),
            HeaderValue(row, headers, "helbidea"),
            HeaderValue(row, headers, "lokalizatzailea", "lokali"),
            ParseInt(HeaderValue(row, headers, "gauak")),
            HeaderValue(row, headers, "datak"),
            HeaderValue(row, headers, "gelak"),
            HeaderValue(row, headers, "sarrera", "checkin", "check in"),
            HeaderValue(row, headers, "irteera", "checkout", "check out"),
            HeaderValue(row, headers, "dokumentazioa", "doc", "doku"),
            HeaderValue(row, headers, "harrera", "harreta", "harreta ordutegia", "harrera ordutegia"),
            HeaderValue(row, headers, "gosaria", "gosariates"),
            HeaderValue(row, headers, "bazkaria", "bazkariates"),
            HeaderValue(row, headers, "afaria", "afariates"),
            Parseatu(HeaderValue(row, headers, "toailak", "toallak", "toallak barne", "toailak barne")),
            Parseatu(HeaderValue(row, headers, "izarak", "izarak barne")),
            HeaderValue(row, headers, "fidantza prezioa", "fidantza kuota", "fidantzaquota"),
            HeaderValue(row, headers, "maleten prezioa", "luggage prezioa", "luggage kuota", "luggagekuota", "luggage quota"),
            HeaderValue(row, headers, "instalazioak", "inst"));
    }

    private static Ostalak ReadOstalaByPosition(IReadOnlyList<string> row)
    {
        return new Ostalak(
            RowCell(row, 1),
            RowCell(row, 2),
            RowCell(row, 3),
            BalioLehenetsiak.Lokalizatzailea,
            BalioLehenetsiak.Gauak,
            "",
            "",
            RowCell(row, 4),
            RowCell(row, 5),
            RowCell(row, 6),
            RowCell(row, 7),
            RowCell(row, 11),
            RowCell(row, 12),
            RowCell(row, 13),
            Parseatu(RowCell(row, 8)),
            Parseatu(RowCell(row, 9)),
            "",
            "",
            RowCell(row, 10));
    }

    private static IEnumerable<Ekintzak> ReadEkintzakRows(IReadOnlyList<IReadOnlyList<string>> rows)
    {
        if (rows.Count == 0)
        {
            yield break;
        }

        bool hasHeader = HeaderDa(RowCell(rows[0], 1), "ekintza", "ekintzaizena", "ekintza izena", "izena");
        Dictionary<string, int> headers = hasHeader ? HeaderMap(rows[0]) : [];
        foreach (IReadOnlyList<string> row in rows.Skip(hasHeader ? 1 : 0))
        {
            if (RowHutsa(row))
            {
                continue;
            }

            Ekintzak item = hasHeader ? ReadEkintzaByHeader(row, headers) : ReadEkintzaByPosition(row);
            if (!string.IsNullOrWhiteSpace(item.EkintzaIzena))
            {
                yield return item;
            }
        }
    }

    private static Ekintzak ReadEkintzaByHeader(IReadOnlyList<string> row, IReadOnlyDictionary<string, int> headers)
    {
        return new Ekintzak(
            HeaderValue(row, headers, "ekintzaizena", "ekintza", "izena"),
            HeaderValue(row, headers, "bonoa"),
            HeaderValue(row, headers, "iraupena"),
            HeaderValue(row, headers, "kontaktua"),
            HeaderValue(row, headers, "elkartokia"),
            HeaderValue(row, headers, "iristean"),
            HeaderValue(row, headers, "eramanm"),
            HeaderValue(row, headers, "bertanm"),
            Parseatu(HeaderValue(row, headers, "aldagela", "aldageal")),
            Parseatu(HeaderValue(row, headers, "komuna", "komunak")),
            HeaderValue(row, headers, "egonlekua"),
            HeaderValue(row, headers, "informazioa"),
            HeaderValue(row, headers, "lokali", "lokalizatzailea"));
    }

    private static Ekintzak ReadEkintzaByPosition(IReadOnlyList<string> row)
    {
        return new Ekintzak(
            RowCell(row, 1),
            RowCell(row, 2),
            RowCell(row, 3),
            RowCell(row, 4),
            RowCell(row, 5),
            RowCell(row, 6),
            RowCell(row, 7),
            RowCell(row, 8),
            Parseatu(RowCell(row, 9)),
            Parseatu(RowCell(row, 10)),
            RowCell(row, 11),
            RowCell(row, 12),
            RowCell(row, 13));
    }

    private static IEnumerable<Garraioak> ReadGarraioakRows(IReadOnlyList<IReadOnlyList<string>> rows)
    {
        if (rows.Count == 0)
        {
            yield break;
        }

        bool hasHeader = HeaderDa(RowCell(rows[0], 1), "garraioa", "garraioaizena", "garraioa izena", "izena");
        Dictionary<string, int> headers = hasHeader ? HeaderMap(rows[0]) : [];
        foreach (IReadOnlyList<string> row in rows.Skip(hasHeader ? 1 : 0))
        {
            if (RowHutsa(row))
            {
                continue;
            }

            Garraioak item = hasHeader ? ReadGarraioaByHeader(row, headers) : ReadGarraioaByPosition(row);
            if (!string.IsNullOrWhiteSpace(item.GarraioaIzena))
            {
                yield return item;
            }
        }
    }

    private static Garraioak ReadGarraioaByHeader(IReadOnlyList<string> row, IReadOnlyDictionary<string, int> headers)
    {
        return new Garraioak(
            HeaderValue(row, headers, "garraioaizena", "garraioa", "izena"),
            HeaderValue(row, headers, "eguna"),
            HeaderValue(row, headers, "ordutegia"),
            HeaderValue(row, headers, "lokalizatzailea", "lokali"),
            HeaderValue(row, headers, "kontaktua"),
            HeaderValue(row, headers, "elkargunea"),
            HeaderValue(row, headers, "eginbeharrak"),
            HeaderValue(row, headers, "informazioa"));
    }

    private static Garraioak ReadGarraioaByPosition(IReadOnlyList<string> row)
    {
        return new Garraioak(RowCell(row, 1), RowCell(row, 2), RowCell(row, 3), RowCell(row, 4), RowCell(row, 5), RowCell(row, 6), RowCell(row, 7), RowCell(row, 8));
    }

    private static string OstalaValueForHeader(string header, Ostalak ostala)
    {
        return header switch
        {
            "ostala" or "ostalaizena" or "izena" or "hotela" => ostala.OstalaIzena,
            "bonoa" => ostala.Bonoa,
            "helbidea" => ostala.Helbidea,
            "lokalizatzailea" or "lokali" => ostala.Lokalizatzailea,
            "gauak" => ostala.Gauak.ToString(),
            "datak" => ostala.Datak,
            "gelak" => ostala.Gelak,
            "sarrera" or "checkin" => ostala.Checkin,
            "irteera" or "checkout" => ostala.Checkout,
            "dokumentazioa" or "doc" or "doku" => ostala.Dokumentazioa,
            "harrera" or "harreta" or "harretaordutegia" or "harreraordutegia" => ostala.Harrera,
            "gosaria" or "gosariates" => ostala.Gosaria,
            "bazkaria" or "bazkariates" => ostala.Bazkaria,
            "afaria" or "afariates" => ostala.Afaria,
            "toailak" or "toallak" or "toallakbarne" or "toailakbarne" => BoolToBaiEz(ostala.Toailak),
            "izarak" or "izarakbarne" => BoolToBaiEz(ostala.Izarak),
            "fidantzaprezioa" or "fidantzakuota" => ostala.Fidantza,
            "maletenprezioa" or "luggageprezioa" or "luggagekuota" or "luggagequota" => ostala.Luggage,
            "instalazioak" or "inst" => ostala.Instalazioak,
            _ => ""
        };
    }

    private static string EkintzaValueForHeader(string header, Ekintzak ekintza)
    {
        return header switch
        {
            "ekintza" or "ekintzaizena" or "izena" => ekintza.EkintzaIzena,
            "bonoa" => ekintza.Bonoa,
            "iraupena" => ekintza.Iraupena,
            "kontaktua" => ekintza.Kontaktua,
            "elkartokia" => ekintza.Elkartokia,
            "iristean" => ekintza.Iristean,
            "eramanm" => ekintza.EramanM,
            "bertanm" => ekintza.BertanM,
            "aldagela" or "aldageal" => BoolToBaiEz(ekintza.Aldagela),
            "komuna" or "komunak" => BoolToBaiEz(ekintza.Komuna),
            "egonlekua" => ekintza.Egonlekua,
            "informazioa" => ekintza.Informazioa,
            "lokali" or "lokalizatzailea" => ekintza.Lokali,
            _ => ""
        };
    }

    private static Dictionary<string, int> HeaderMap(IReadOnlyList<string> headerRow)
    {
        Dictionary<string, int> headers = [];
        for (int i = 0; i < headerRow.Count; i++)
        {
            string key = NormalizeHeader(headerRow[i]);
            if (!string.IsNullOrWhiteSpace(key) && !headers.ContainsKey(key))
            {
                headers.Add(key, i + 1);
            }
        }

        return headers;
    }

    private static string HeaderValue(IReadOnlyList<string> row, IReadOnlyDictionary<string, int> headers, params string[] names)
    {
        foreach (string name in names)
        {
            if (headers.TryGetValue(NormalizeHeader(name), out int column))
            {
                return RowCell(row, column);
            }
        }

        return "";
    }

    private static string RowCell(IReadOnlyList<string> row, int zutabea)
    {
        int index = zutabea - 1;
        return index >= 0 && index < row.Count ? row[index] ?? "" : "";
    }

    private static string NormalizeHeader(string? value)
    {
        return (value ?? "")
            .Trim()
            .Replace(" ", "", StringComparison.Ordinal)
            .Replace("_", "", StringComparison.Ordinal)
            .Replace("-", "", StringComparison.Ordinal)
            .ToLowerInvariant();
    }

    private static bool HeaderDa(string lehenZutabea, params string[] aukerak)
    {
        string balioa = NormalizeHeader(lehenZutabea);
        return aukerak.Any(a => string.Equals(balioa, NormalizeHeader(a), StringComparison.OrdinalIgnoreCase));
    }

    private static bool RowHutsa(IReadOnlyList<string> row) => row.All(string.IsNullOrWhiteSpace);
    private static int ParseInt(string value) => int.TryParse(value, out int result) ? result : BalioLehenetsiak.Gauak;
    private static string BoolToBaiEz(bool balioa) => balioa ? "Bai" : "Ez";

    private static List<string> NormalizeRow(IReadOnlyList<string> row, int columns)
    {
        List<string> values = row.Take(columns).ToList();
        while (values.Count < columns)
        {
            values.Add("");
        }

        return values;
    }

    private static void EnsureOstalaHeaders(List<string> headers)
    {
        foreach (string header in DefaultOstalakHeaders)
        {
            if (!headers.Any(item => string.Equals(NormalizeHeader(item), NormalizeHeader(header), StringComparison.OrdinalIgnoreCase)))
            {
                headers.Add(header);
            }
        }
    }

    private static bool Parseatu(string balioa)
    {
        return string.Equals(balioa?.Trim(), "Bai", StringComparison.OrdinalIgnoreCase)
            || string.Equals(balioa?.Trim(), "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(balioa?.Trim(), "yes", StringComparison.OrdinalIgnoreCase)
            || string.Equals(balioa?.Trim(), "si", StringComparison.OrdinalIgnoreCase)
            || string.Equals(balioa?.Trim(), "sí", StringComparison.OrdinalIgnoreCase);
    }

    private void EnsureReady(string sheetName)
    {
        if (!AllowedSheets.Contains(sheetName, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Orria ez dago baimenduta.");
        }

        if (!googleSheetsService.IsConfigured)
        {
            throw new InvalidOperationException("Driveko datu-basea ez dago konfiguratuta.");
        }
    }
}
