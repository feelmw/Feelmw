using ClosedXML.Excel;
using FeelmwLogistika.Blazor.Models;
using Microsoft.JSInterop;

namespace FeelmwLogistika.Blazor.Services;

public sealed class PlangintzaListakService(HttpClient httpClient, IJSRuntime jsRuntime) : IPlangintzaListakService
{
    private static readonly SemaphoreSlim FileLock = new(1, 1);
    private static readonly string[] AllowedSheets = ["Ostalak", "Ekintzak"];
    private const string StorageKey = "FeelMW.Plangintza.xlsx";
    private byte[]? workbookBytes;

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
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            using XLWorkbook workbook = await OpenWorkbookAsync(cancellationToken);
            if (!workbook.Worksheets.TryGetWorksheet("Ostalak", out IXLWorksheet? sheet) || sheet.RangeUsed() is null)
            {
                return [];
            }

            IXLRange range = sheet.RangeUsed()!;
            Dictionary<string, int> headers = ReadHeaders(sheet);
            bool hasHeader = HasAnyHeader(headers, "izena", "ostala", "hotela");
            List<Hotelak> result = [];
            foreach (IXLRangeRow row in hasHeader ? range.RowsUsed().Skip(1) : range.RowsUsed())
            {
                string izena = hasHeader
                    ? ReadByHeader(row, headers, "izena", "ostala", "hotela")
                    : ReadCell(row, 2);
                if (string.IsNullOrWhiteSpace(izena))
                {
                    continue;
                }

                result.Add(new Hotelak(
                    hasHeader ? ReadByHeader(row, headers, "hiria") : ReadCell(row, 1),
                    izena,
                    hasHeader ? ReadByHeader(row, headers, "helbideaurl", "helbidea url", "url") : ReadCell(row, 3)));
            }

            return result;
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<IReadOnlyList<EkintzakPlan>> ReadEkintzakAsync(CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            using XLWorkbook workbook = await OpenWorkbookAsync(cancellationToken);
            if (!workbook.Worksheets.TryGetWorksheet("Ekintzak", out IXLWorksheet? sheet) || sheet.RangeUsed() is null)
            {
                return [];
            }

            IXLRange range = sheet.RangeUsed()!;
            Dictionary<string, int> headers = ReadHeaders(sheet);
            bool hasHeader = HasAnyHeader(headers, "izena", "mota", "ekintza");
            List<EkintzakPlan> result = [];
            foreach (IXLRangeRow row in hasHeader ? range.RowsUsed().Skip(1) : range.RowsUsed())
            {
                string izena = hasHeader
                    ? ReadByHeader(row, headers, "izena", "mota", "ekintza")
                    : ReadCell(row, 1);
                if (string.IsNullOrWhiteSpace(izena))
                {
                    continue;
                }

                result.Add(new EkintzakPlan(
                    izena,
                    hasHeader ? ReadByHeader(row, headers, "deskribapena", "descripcion", "azalpena") : ReadCell(row, 2)));
            }

            return result;
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task AddEkintzaAsync(EkintzakPlan ekintza, CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            using XLWorkbook workbook = await OpenWorkbookAsync(cancellationToken);
            IXLWorksheet sheet = workbook.Worksheets.TryGetWorksheet("Ekintzak", out IXLWorksheet? existing)
                ? existing
                : workbook.Worksheets.Add("Ekintzak");

            EnsureEkintzakHeader(sheet);
            Dictionary<string, int> headers = ReadHeaders(sheet);
            int row = NextRow(sheet);
            WriteByHeader(sheet, row, headers, "Izena", ekintza.Mota);
            WriteByHeader(sheet, row, headers, "Deskribapena", ekintza.Deskribapena);
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
                : await httpClient.GetByteArrayAsync("data/Plangintza.xlsx", cancellationToken);
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

    private static Dictionary<string, int> ReadHeaders(IXLWorksheet sheet)
    {
        Dictionary<string, int> headers = new(StringComparer.OrdinalIgnoreCase);
        IXLRange? range = sheet.RangeUsed();
        if (range is null)
        {
            return headers;
        }

        IXLRangeRow firstRow = range.FirstRowUsed();
        foreach (IXLCell cell in firstRow.CellsUsed())
        {
            string header = Normalize(cell.GetFormattedString());
            if (!string.IsNullOrWhiteSpace(header) && !headers.ContainsKey(header))
            {
                headers[header] = cell.Address.ColumnNumber;
            }
        }

        return headers;
    }

    private static string ReadByHeader(IXLRangeRow row, IReadOnlyDictionary<string, int> headers, params string[] aliases)
    {
        foreach (string alias in aliases)
        {
            if (headers.TryGetValue(Normalize(alias), out int column))
            {
                IXLCell cell = row.Cell(column);
                if (cell.HasHyperlink)
                {
                    string? url = cell.GetHyperlink().ExternalAddress?.ToString();
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        return url;
                    }
                }

                return cell.GetFormattedString();
            }
        }

        return "";
    }

    private static string ReadCell(IXLRangeRow row, int column)
    {
        IXLCell cell = row.Cell(column);
        if (cell.HasHyperlink)
        {
            string? url = cell.GetHyperlink().ExternalAddress?.ToString();
            if (!string.IsNullOrWhiteSpace(url))
            {
                return url;
            }
        }

        return cell.GetFormattedString();
    }

    private static bool HasAnyHeader(IReadOnlyDictionary<string, int> headers, params string[] aliases)
    {
        return aliases.Any(alias => headers.ContainsKey(Normalize(alias)));
    }

    private static void WriteByHeader(IXLWorksheet sheet, int row, Dictionary<string, int> headers, string header, string value)
    {
        string key = Normalize(header);
        if (!headers.TryGetValue(key, out int column))
        {
            column = headers.Count + 1;
            headers[key] = column;
            sheet.Cell(1, column).Value = header;
        }

        sheet.Cell(row, column).Value = value ?? "";
    }

    private static void EnsureEkintzakHeader(IXLWorksheet sheet)
    {
        if (sheet.RangeUsed() is not null && ReadHeaders(sheet).Count > 0)
        {
            return;
        }

        sheet.Cell(1, 1).Value = "Izena";
        sheet.Cell(1, 2).Value = "Deskribapena";
    }

    private static int NextRow(IXLWorksheet sheet)
    {
        IXLRange? range = sheet.RangeUsed();
        return range is null ? 1 : range.LastRowUsed().RowNumber() + 1;
    }

    private static void EnsureAllowedSheet(string sheetName)
    {
        if (!AllowedSheets.Contains(sheetName, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"{sheetName} orria ez dago baimenduta.");
        }
    }

    private static string Normalize(string value)
    {
        return new string((value ?? "")
            .Trim()
            .ToLowerInvariant()
            .Where(char.IsLetterOrDigit)
            .ToArray());
    }
}
