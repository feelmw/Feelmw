using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public interface ILogistikaDbService
{
    Task<IReadOnlyList<Ostalak>> ReadOstalakAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Ekintzak>> ReadEkintzakAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Garraioak>> ReadGarraioakAsync(CancellationToken cancellationToken = default);
    Task AddOstalaAsync(Ostalak ostala, CancellationToken cancellationToken = default);
    Task AddEkintzaAsync(Ekintzak ekintza, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IReadOnlyList<string>>> ReadSheetAsync(string sheetName, CancellationToken cancellationToken = default);
    Task SaveSheetAsync(string sheetName, IReadOnlyList<IReadOnlyList<string>> rows, CancellationToken cancellationToken = default);
}
