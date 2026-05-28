using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace FeelmwLogistika.Blazor.Services;

public sealed class GoogleSheetsService(HttpClient httpClient, IConfiguration configuration) : IGoogleSheetsService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly string baseUrl = configuration["GoogleSheetsApi:BaseUrl"] ?? "";

    public bool IsConfigured => Uri.TryCreate(baseUrl, UriKind.Absolute, out _);

    public Task<IReadOnlyList<IReadOnlyList<string>>> ReadSheetAsync(string workbookKey, string sheetName, CancellationToken cancellationToken = default)
    {
        return SendRowsRequestAsync(new GoogleSheetsRequest("read", workbookKey, sheetName), cancellationToken);
    }

    public Task SaveSheetAsync(string workbookKey, string sheetName, IReadOnlyList<IReadOnlyList<string>> rows, CancellationToken cancellationToken = default)
    {
        return SendRequestAsync(new GoogleSheetsRequest("save", workbookKey, sheetName, Rows: rows), cancellationToken);
    }

    public Task AddRowAsync(string workbookKey, string sheetName, IReadOnlyList<string> row, CancellationToken cancellationToken = default)
    {
        return SendRequestAsync(new GoogleSheetsRequest("add", workbookKey, sheetName, Row: row), cancellationToken);
    }

    public Task UpdateRowAsync(string workbookKey, string sheetName, int rowIndex, IReadOnlyList<string> row, CancellationToken cancellationToken = default)
    {
        return SendRequestAsync(new GoogleSheetsRequest("update", workbookKey, sheetName, RowIndex: rowIndex, Row: row), cancellationToken);
    }

    public Task DeleteRowAsync(string workbookKey, string sheetName, int rowIndex, CancellationToken cancellationToken = default)
    {
        return SendRequestAsync(new GoogleSheetsRequest("delete", workbookKey, sheetName, RowIndex: rowIndex), cancellationToken);
    }

    public Task<IReadOnlyList<IReadOnlyList<string>>> RefreshSheetAsync(string workbookKey, string sheetName, CancellationToken cancellationToken = default)
    {
        return ReadSheetAsync(workbookKey, sheetName, cancellationToken);
    }

    private async Task<IReadOnlyList<IReadOnlyList<string>>> SendRowsRequestAsync(GoogleSheetsRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await PostAsync(request, cancellationToken);
        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        response.EnsureSuccessStatusCode();

        if (TryReadRows(json, out IReadOnlyList<IReadOnlyList<string>> rows))
        {
            return rows;
        }

        GoogleSheetsResponse? envelope = JsonSerializer.Deserialize<GoogleSheetsResponse>(json, JsonOptions);
        if (envelope?.Success == false)
        {
            throw new InvalidOperationException(envelope.Message ?? "Google Sheets API errorea.");
        }

        return envelope?.Rows ?? [];
    }

    private async Task SendRequestAsync(GoogleSheetsRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await PostAsync(request, cancellationToken);
        GoogleSheetsResponse? envelope = await response.Content.ReadFromJsonAsync<GoogleSheetsResponse>(JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        if (envelope?.Success == false)
        {
            throw new InvalidOperationException(envelope.Message ?? "Google Sheets API errorea.");
        }
    }

    private async Task<HttpResponseMessage> PostAsync(GoogleSheetsRequest request, CancellationToken cancellationToken)
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException("Google Sheets API ez dago konfiguratuta.");
        }

        string json = JsonSerializer.Serialize(request, JsonOptions);
        using StringContent content = new(json, Encoding.UTF8, "text/plain");
        return await httpClient.PostAsync(baseUrl, content, cancellationToken);
    }

    private static bool TryReadRows(string json, out IReadOnlyList<IReadOnlyList<string>> rows)
    {
        try
        {
            IReadOnlyList<IReadOnlyList<string>>? directRows = JsonSerializer.Deserialize<IReadOnlyList<IReadOnlyList<string>>>(json, JsonOptions);
            rows = directRows ?? [];
            return directRows is not null;
        }
        catch (JsonException)
        {
            rows = [];
            return false;
        }
    }

    private sealed record GoogleSheetsRequest(
        string Action,
        string Workbook,
        string Sheet,
        IReadOnlyList<IReadOnlyList<string>>? Rows = null,
        IReadOnlyList<string>? Row = null,
        int? RowIndex = null);

    private sealed class GoogleSheetsResponse
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public IReadOnlyList<IReadOnlyList<string>> Rows { get; set; } = [];
    }
}
