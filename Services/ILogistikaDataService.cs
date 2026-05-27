using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public interface ILogistikaDataService
{
    Task<LogistikaDatuak> ReadAllAsync(CancellationToken cancellationToken = default);
    Task<LogistikaDatuak> ReadExcelAsync(Stream stream, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Ostalak>> ReadOstalakAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Ekintzak>> ReadEkintzakAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Garraioak>> ReadGarraioakAsync(CancellationToken cancellationToken = default);
    Task AddOstalaAsync(Ostalak ostala, CancellationToken cancellationToken = default);
    Task AddEkintzaAsync(Ekintzak ekintza, CancellationToken cancellationToken = default);
}
