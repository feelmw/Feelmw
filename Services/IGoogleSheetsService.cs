namespace FeelmwLogistika.Blazor.Services;

public interface IGoogleSheetsService
{
    bool IsConfigured { get; }

    Task<IReadOnlyList<IReadOnlyList<string>>> ReadSheetAsync(string workbookKey, string sheetName, CancellationToken cancellationToken = default);

    Task SaveSheetAsync(string workbookKey, string sheetName, IReadOnlyList<IReadOnlyList<string>> rows, CancellationToken cancellationToken = default);

    Task AddRowAsync(string workbookKey, string sheetName, IReadOnlyList<string> row, CancellationToken cancellationToken = default);

    Task UpdateRowAsync(string workbookKey, string sheetName, int rowIndex, IReadOnlyList<string> row, CancellationToken cancellationToken = default);

    Task DeleteRowAsync(string workbookKey, string sheetName, int rowIndex, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<IReadOnlyList<string>>> RefreshSheetAsync(string workbookKey, string sheetName, CancellationToken cancellationToken = default);
}
