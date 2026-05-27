using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public interface IPlangintzaListakService
{
    Task<PlangintzaListak> ReadAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Hotelak>> ReadOstalakAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EkintzakPlan>> ReadEkintzakAsync(CancellationToken cancellationToken = default);
    Task AddEkintzaAsync(EkintzakPlan ekintza, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IReadOnlyList<string>>> ReadSheetAsync(string sheetName, CancellationToken cancellationToken = default);
    Task SaveSheetAsync(string sheetName, IReadOnlyList<IReadOnlyList<string>> rows, CancellationToken cancellationToken = default);
    Task<byte[]> ExportWorkbookAsync(CancellationToken cancellationToken = default);
}
