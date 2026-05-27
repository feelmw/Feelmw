using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public interface ILogistikaService
{
    Task<IReadOnlyList<Ostalak>> GetOstalakAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Ekintzak>> GetEkintzakAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Garraioak>> GetGarraioakAsync(CancellationToken cancellationToken = default);
}
