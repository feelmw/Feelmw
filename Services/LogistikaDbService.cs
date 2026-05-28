using ClosedXML.Excel;
using FeelmwLogistika.Blazor.Models;
using Microsoft.JSInterop;

namespace FeelmwLogistika.Blazor.Services;

public sealed class LogistikaDbService(HttpClient httpClient, IJSRuntime jsRuntime, IGoogleSheetsService googleSheetsService) : ILogistikaDbService
{
    private static readonly SemaphoreSlim FileLock = new(1, 1);
    private static readonly string[] AllowedSheets = ["Ostalak", "Ekintzak", "Garraioak"];
    private static readonly string[] DefaultLogistikaOstalakHeaders = ["Ostala", "Bonoa", "Helbidea", "Checkin", "Checkout", "Doku", "Harreta ordutegia", "Toallak barne", "Izarak barne", "Instalazioak", "Gosaria", "Bazkaria", "Afaria"];
    private static readonly string[] DefaultLogistikaEkintzakHeaders = ["Ekintza", "Bonoa", "Iraupena", "Kontaktua", "Elkartokia", "Iristean", "EramanM", "BertanM", "Aldageal", "Komunak", "Egonlekua", "Informazioa"];
    private const string StorageKey = "FeelMW.Logistika.xlsx";
    private byte[]? workbookBytes;

    public async Task<IReadOnlyList<Ostalak>> ReadOstalakAsync(CancellationToken cancellationToken = default)
    {
        if (googleSheetsService.IsConfigured)
        {
            return ReadOstalakRows(await googleSheetsService.ReadSheetAsync("Logistika", "Ostalak", cancellationToken)).ToList();
        }

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            using XLWorkbook workbook = await OpenWorkbookAsync(cancellationToken);
            if (!workbook.Worksheets.TryGetWorksheet("Ostalak", out IXLWorksheet? sheet) || sheet.RangeUsed() is null)
            {
                return [];
            }

            return ReadOstalak(sheet).ToList();
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<IReadOnlyList<Ekintzak>> ReadEkintzakAsync(CancellationToken cancellationToken = default)
    {
        if (googleSheetsService.IsConfigured)
        {
            return ReadEkintzakRows(await googleSheetsService.ReadSheetAsync("Logistika", "Ekintzak", cancellationToken)).ToList();
        }

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            using XLWorkbook workbook = await OpenWorkbookAsync(cancellationToken);
            if (!workbook.Worksheets.TryGetWorksheet("Ekintzak", out IXLWorksheet? sheet) || sheet.RangeUsed() is null)
            {
                return [];
            }

            return ReadEkintzak(sheet).ToList();
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<IReadOnlyList<Garraioak>> ReadGarraioakAsync(CancellationToken cancellationToken = default)
    {
        if (googleSheetsService.IsConfigured)
        {
            return ReadGarraioakRows(await googleSheetsService.ReadSheetAsync("Logistika", "Garraioak", cancellationToken)).ToList();
        }

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            using XLWorkbook workbook = await OpenWorkbookAsync(cancellationToken);
            if (!workbook.Worksheets.TryGetWorksheet("Garraioak", out IXLWorksheet? sheet) || sheet.RangeUsed() is null)
            {
                return [];
            }

            return ReadGarraioak(sheet).ToList();
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task AddOstalaAsync(Ostalak ostala, CancellationToken cancellationToken = default)
    {
        if (googleSheetsService.IsConfigured)
        {
            IReadOnlyList<IReadOnlyList<string>> rows = await googleSheetsService.ReadSheetAsync("Logistika", "Ostalak", cancellationToken);
            IReadOnlyList<string> headers = rows.Count > 0 ? rows[0] : DefaultLogistikaOstalakHeaders;
            await googleSheetsService.AddRowAsync("Logistika", "Ostalak", headers.Select(header => OstalaValueForHeader(NormalizeHeader(header), ostala)).ToList(), cancellationToken);
            return;
        }

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            using XLWorkbook workbook = await OpenWorkbookAsync(cancellationToken);
            IXLWorksheet sheet = workbook.Worksheets.TryGetWorksheet("Ostalak", out IXLWorksheet? existing)
                ? existing
                : workbook.Worksheets.Add("Ostalak");

            int row = NextRow(sheet);
            if (HasHeader(sheet, "ostala", "ostalaizena", "ostala izena", "izena", "hotela"))
            {
                WriteOstalaByHeader(sheet, row, ostala);
            }
            else
            {
                WriteRow(sheet, row,
                [
                    ostala.OstalaIzena,
                    ostala.Bonoa,
                    ostala.Helbidea,
                    ostala.Lokalizatzailea,
                    ostala.Gauak.ToString(),
                    ostala.Datak,
                    ostala.Gelak,
                    ostala.Checkin,
                    ostala.Checkout,
                    ostala.Dokumentazioa,
                    ostala.Harrera,
                    ostala.Gosaria,
                    ostala.Bazkaria,
                    ostala.Afaria,
                    BoolToBaiEz(ostala.Toailak),
                    BoolToBaiEz(ostala.Izarak),
                    ostala.Fidantza,
                    ostala.Luggage,
                    ostala.Instalazioak
                ]);
            }
            await SaveWorkbookAsync(workbook);
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task AddEkintzaAsync(Ekintzak ekintza, CancellationToken cancellationToken = default)
    {
        if (googleSheetsService.IsConfigured)
        {
            IReadOnlyList<IReadOnlyList<string>> rows = await googleSheetsService.ReadSheetAsync("Logistika", "Ekintzak", cancellationToken);
            IReadOnlyList<string> headers = rows.Count > 0 ? rows[0] : DefaultLogistikaEkintzakHeaders;
            await googleSheetsService.AddRowAsync("Logistika", "Ekintzak", headers.Select(header => EkintzaValueForHeader(NormalizeHeader(header), ekintza)).ToList(), cancellationToken);
            return;
        }

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            using XLWorkbook workbook = await OpenWorkbookAsync(cancellationToken);
            IXLWorksheet sheet = workbook.Worksheets.TryGetWorksheet("Ekintzak", out IXLWorksheet? existing)
                ? existing
                : workbook.Worksheets.Add("Ekintzak");

            int row = NextRow(sheet);
            if (HasHeader(sheet, "ekintza", "ekintzaizena", "ekintza izena", "izena"))
            {
                WriteEkintzaByHeader(sheet, row, ekintza);
            }
            else
            {
                WriteRow(sheet, row,
                [
                    ekintza.EkintzaIzena,
                    ekintza.Bonoa,
                    ekintza.Iraupena,
                    ekintza.Kontaktua,
                    ekintza.Elkartokia,
                    ekintza.Iristean,
                    ekintza.EramanM,
                    ekintza.BertanM,
                    BoolToBaiEz(ekintza.Aldagela),
                    BoolToBaiEz(ekintza.Komuna),
                    ekintza.Egonlekua,
                    ekintza.Informazioa,
                    ekintza.Lokali
                ]);
            }
            await SaveWorkbookAsync(workbook);
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<IReadOnlyList<IReadOnlyList<string>>> ReadSheetAsync(string sheetName, CancellationToken cancellationToken = default)
    {
        EnsureAllowedSheet(sheetName);
        if (googleSheetsService.IsConfigured)
        {
            return await googleSheetsService.ReadSheetAsync("Logistika", sheetName, cancellationToken);
        }

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            using XLWorkbook workbook = await OpenWorkbookAsync(cancellationToken);
            if (!workbook.Worksheets.TryGetWorksheet(sheetName, out IXLWorksheet? sheet) || sheet.RangeUsed() is null)
            {
                return [];
            }

            IXLRange range = sheet.RangeUsed()!;
            List<IReadOnlyList<string>> rows = [];
            foreach (IXLRangeRow row in range.RowsUsed())
            {
                List<string> values = [];
                for (int col = 1; col <= range.ColumnCount(); col++)
                {
                    values.Add(row.Cell(col).GetFormattedString() ?? "");
                }

                rows.Add(values);
            }

            return rows;
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task SaveSheetAsync(string sheetName, IReadOnlyList<IReadOnlyList<string>> rows, CancellationToken cancellationToken = default)
    {
        EnsureAllowedSheet(sheetName);
        if (googleSheetsService.IsConfigured)
        {
            await googleSheetsService.SaveSheetAsync("Logistika", sheetName, rows, cancellationToken);
            return;
        }

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            using XLWorkbook workbook = await OpenWorkbookAsync(cancellationToken);
            IXLWorksheet sheet = workbook.Worksheets.TryGetWorksheet(sheetName, out IXLWorksheet? existing)
                ? existing
                : workbook.Worksheets.Add(sheetName);

            sheet.Clear();
            for (int row = 0; row < rows.Count; row++)
            {
                for (int col = 0; col < rows[row].Count; col++)
                {
                    sheet.Cell(row + 1, col + 1).Value = rows[row][col];
                }
            }

            await SaveWorkbookAsync(workbook);
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<byte[]> ExportWorkbookAsync(CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            using XLWorkbook workbook = await OpenWorkbookAsync(cancellationToken);
            using MemoryStream stream = new();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        finally
        {
            FileLock.Release();
        }
    }

    private async Task<XLWorkbook> OpenWorkbookAsync(CancellationToken cancellationToken)
    {
        if (workbookBytes is null)
        {
            string? saved = await ReadStoredWorkbookAsync();
            workbookBytes = !string.IsNullOrWhiteSpace(saved)
                ? Convert.FromBase64String(saved)
                : await httpClient.GetByteArrayAsync("data/Logistika.xlsx", cancellationToken);
        }

        return new XLWorkbook(new MemoryStream(workbookBytes, writable: false));
    }

    private async Task SaveWorkbookAsync(XLWorkbook workbook)
    {
        using MemoryStream stream = new();
        workbook.SaveAs(stream);
        workbookBytes = stream.ToArray();
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, Convert.ToBase64String(workbookBytes));
    }

    private async Task<string?> ReadStoredWorkbookAsync()
    {
        try
        {
            return await jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        }
        catch (JSException)
        {
            return null;
        }
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
        bool formatuBerria = OstalaFormatuBerriaDa(row);
        return new Ostalak(
            RowCell(row, 1),
            RowCell(row, 2),
            RowCell(row, 3),
            formatuBerria ? RowCell(row, 4) : BalioLehenetsiak.Lokalizatzailea,
            int.TryParse(formatuBerria ? RowCell(row, 5) : BalioLehenetsiak.Gauak.ToString(), out int gauakKop) ? gauakKop : 0,
            formatuBerria ? RowCell(row, 6) : "",
            formatuBerria ? RowCell(row, 7) : "",
            formatuBerria ? RowCell(row, 8) : RowCell(row, 4),
            formatuBerria ? RowCell(row, 9) : RowCell(row, 5),
            formatuBerria ? RowCell(row, 10) : RowCell(row, 6),
            formatuBerria ? RowCell(row, 11) : RowCell(row, 8),
            formatuBerria ? RowCell(row, 12) : RowCell(row, 12),
            formatuBerria ? RowCell(row, 13) : RowCell(row, 13),
            formatuBerria ? RowCell(row, 14) : RowCell(row, 14),
            Parseatu(formatuBerria ? RowCell(row, 15) : RowCell(row, 9)),
            Parseatu(formatuBerria ? RowCell(row, 16) : RowCell(row, 10)),
            formatuBerria ? RowCell(row, 17) : RowCell(row, 15),
            formatuBerria ? RowCell(row, 18) : RowCell(row, 7),
            formatuBerria ? RowCell(row, 19) : RowCell(row, 11));
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
        return new Garraioak(
            RowCell(row, 1),
            RowCell(row, 2),
            RowCell(row, 3),
            RowCell(row, 4),
            RowCell(row, 5),
            RowCell(row, 6),
            RowCell(row, 7),
            RowCell(row, 8));
    }

    private static IEnumerable<Ostalak> ReadOstalak(IXLWorksheet sheet)
    {
        List<IXLRangeRow> rows = sheet.RangeUsed()!.RowsUsed().ToList();
        if (rows.Count == 0)
        {
            yield break;
        }

        bool hasHeader = HeaderDa(Gelaxka(rows[0], 1), "ostala", "ostalaizena", "ostala izena", "izena", "hotela");
        Dictionary<string, int> headers = hasHeader ? HeaderMap(rows[0]) : [];

        foreach (IXLRangeRow row in rows.Skip(hasHeader ? 1 : 0))
        {
            if (RowHutsa(row))
            {
                continue;
            }

            Ostalak item = hasHeader ? ReadOstalaByHeader(row, headers) : ReadOstalaByPosition(row);
            string ostala = item.OstalaIzena;
            if (string.IsNullOrWhiteSpace(ostala))
            {
                continue;
            }

            yield return item;
        }
    }

    private static Ostalak ReadOstalaByHeader(IXLRangeRow row, IReadOnlyDictionary<string, int> headers)
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

    private static Ostalak ReadOstalaByPosition(IXLRangeRow row)
    {
        bool formatuBerria = OstalaFormatuBerriaDa(row);
        return new Ostalak(
            Gelaxka(row, 1),
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

    private static IEnumerable<Ekintzak> ReadEkintzak(IXLWorksheet sheet)
    {
        List<IXLRangeRow> rows = sheet.RangeUsed()!.RowsUsed().ToList();
        if (rows.Count == 0)
        {
            yield break;
        }

        bool hasHeader = HeaderDa(Gelaxka(rows[0], 1), "ekintza", "ekintzaizena", "ekintza izena", "izena");
        Dictionary<string, int> headers = hasHeader ? HeaderMap(rows[0]) : [];

        foreach (IXLRangeRow row in rows.Skip(hasHeader ? 1 : 0))
        {
            if (RowHutsa(row))
            {
                continue;
            }

            Ekintzak item = hasHeader ? ReadEkintzaByHeader(row, headers) : ReadEkintzaByPosition(row);
            string ekintza = item.EkintzaIzena;
            if (string.IsNullOrWhiteSpace(ekintza))
            {
                continue;
            }

            yield return item;
        }
    }

    private static Ekintzak ReadEkintzaByHeader(IXLRangeRow row, IReadOnlyDictionary<string, int> headers)
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

    private static Ekintzak ReadEkintzaByPosition(IXLRangeRow row)
    {
        return new Ekintzak(
            Gelaxka(row, 1),
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

    private static IEnumerable<Garraioak> ReadGarraioak(IXLWorksheet sheet)
    {
        List<IXLRangeRow> rows = sheet.RangeUsed()!.RowsUsed().ToList();
        if (rows.Count == 0)
        {
            yield break;
        }

        bool hasHeader = HeaderDa(Gelaxka(rows[0], 1), "garraioa", "garraioaizena", "garraioa izena", "izena");
        Dictionary<string, int> headers = hasHeader ? HeaderMap(rows[0]) : [];

        foreach (IXLRangeRow row in rows.Skip(hasHeader ? 1 : 0))
        {
            if (RowHutsa(row))
            {
                continue;
            }

            Garraioak item = hasHeader ? ReadGarraioaByHeader(row, headers) : ReadGarraioaByPosition(row);
            string garraioa = item.GarraioaIzena;
            if (string.IsNullOrWhiteSpace(garraioa))
            {
                continue;
            }

            yield return item;
        }
    }

    private static Garraioak ReadGarraioaByHeader(IXLRangeRow row, IReadOnlyDictionary<string, int> headers)
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

    private static Garraioak ReadGarraioaByPosition(IXLRangeRow row)
    {
        return new Garraioak(
            Gelaxka(row, 1),
            Gelaxka(row, 2),
            Gelaxka(row, 3),
            Gelaxka(row, 4),
            Gelaxka(row, 5),
            Gelaxka(row, 6),
            Gelaxka(row, 7),
            Gelaxka(row, 8));
    }

    private static int NextRow(IXLWorksheet sheet)
    {
        return sheet.LastRowUsed()?.RowNumber() + 1 ?? 1;
    }

    private static void WriteRow(IXLWorksheet sheet, int row, IReadOnlyList<string> values)
    {
        for (int i = 0; i < values.Count; i++)
        {
            sheet.Cell(row, i + 1).Value = values[i];
        }
    }

    private static void WriteOstalaByHeader(IXLWorksheet sheet, int row, Ostalak ostala)
    {
        foreach (IXLCell headerCell in sheet.Row(1).CellsUsed())
        {
            string header = NormalizeHeader(headerCell.GetFormattedString());
            sheet.Cell(row, headerCell.Address.ColumnNumber).Value = OstalaValueForHeader(header, ostala);
        }
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

    private static void WriteEkintzaByHeader(IXLWorksheet sheet, int row, Ekintzak ekintza)
    {
        foreach (IXLCell headerCell in sheet.Row(1).CellsUsed())
        {
            string header = NormalizeHeader(headerCell.GetFormattedString());
            sheet.Cell(row, headerCell.Address.ColumnNumber).Value = EkintzaValueForHeader(header, ekintza);
        }
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

    private static string Gelaxka(IXLRangeRow row, int zutabea) => row.Cell(zutabea).GetFormattedString() ?? "";

    private static string RowCell(IReadOnlyList<string> row, int zutabea)
    {
        int index = zutabea - 1;
        return index >= 0 && index < row.Count ? row[index] ?? "" : "";
    }

    private static Dictionary<string, int> HeaderMap(IXLRangeRow headerRow)
    {
        Dictionary<string, int> headers = [];
        foreach (IXLCell cell in headerRow.CellsUsed())
        {
            string key = NormalizeHeader(cell.GetFormattedString());
            if (!string.IsNullOrWhiteSpace(key) && !headers.ContainsKey(key))
            {
                headers.Add(key, cell.Address.ColumnNumber);
            }
        }

        return headers;
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

    private static string HeaderValue(IXLRangeRow row, IReadOnlyDictionary<string, int> headers, params string[] names)
    {
        foreach (string name in names)
        {
            if (headers.TryGetValue(NormalizeHeader(name), out int column))
            {
                return Gelaxka(row, column);
            }
        }

        return "";
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

    private static string NormalizeHeader(string? value)
    {
        return (value ?? "")
            .Trim()
            .Replace(" ", "", StringComparison.Ordinal)
            .Replace("_", "", StringComparison.Ordinal)
            .Replace("-", "", StringComparison.Ordinal)
            .ToLowerInvariant();
    }

    private static int ParseInt(string value)
    {
        return int.TryParse(value, out int result) ? result : BalioLehenetsiak.Gauak;
    }

    private static bool RowHutsa(IXLRangeRow row) => row.CellsUsed().All(c => string.IsNullOrWhiteSpace(c.GetFormattedString()));

    private static bool RowHutsa(IReadOnlyList<string> row) => row.All(string.IsNullOrWhiteSpace);

    private static bool GoiburuaDa(string lehenZutabea, params string[] aukerak)
    {
        string balioa = (lehenZutabea ?? "").Trim();
        return aukerak.Any(a => string.Equals(balioa, a, StringComparison.OrdinalIgnoreCase));
    }

    private static bool HeaderDa(string lehenZutabea, params string[] aukerak)
    {
        string balioa = NormalizeHeader(lehenZutabea);
        return aukerak.Any(a => string.Equals(balioa, NormalizeHeader(a), StringComparison.OrdinalIgnoreCase));
    }

    private static bool HasHeader(IXLWorksheet sheet, params string[] aukerak)
    {
        return HeaderDa(sheet.Cell(1, 1).GetFormattedString(), aukerak);
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

    private static bool OstalaFormatuBerriaDa(IReadOnlyList<string> row)
    {
        if (int.TryParse(RowCell(row, 5), out _)
            || RowCell(row, 6).Contains(" - ")
            || string.Equals(RowCell(row, 4), BalioLehenetsiak.Lokalizatzailea, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        for (int zutabea = 17; zutabea <= 19; zutabea++)
        {
            if (!string.IsNullOrWhiteSpace(RowCell(row, zutabea)))
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

    private static void EnsureAllowedSheet(string sheetName)
    {
        if (!AllowedSheets.Contains(sheetName, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Orria ez dago baimenduta.");
        }
    }

    private static string BoolToBaiEz(bool balioa) => balioa ? "Bai" : "Ez";

    private static bool Parseatu(string balioa)
    {
        return string.Equals(balioa, "Bai", StringComparison.OrdinalIgnoreCase)
            || string.Equals(balioa, "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(balioa, "yes", StringComparison.OrdinalIgnoreCase)
            || string.Equals(balioa, "si", StringComparison.OrdinalIgnoreCase)
            || string.Equals(balioa, "sí", StringComparison.OrdinalIgnoreCase);
    }

}
