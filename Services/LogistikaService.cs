using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public sealed class LogistikaService : ILogistikaService
{
    public Task<IReadOnlyList<Ostalak>> GetOstalakAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Ostalak> result = [];
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<Ekintzak>> GetEkintzakAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Ekintzak> result = [];
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<Garraioak>> GetGarraioakAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Garraioak> result = [];
        return Task.FromResult(result);
    }
}
