using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public sealed class PlangintzaService : IPlangintzaService
{
    public Task<IReadOnlyList<Hotelak>> GetHotelakAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Hotelak> result = [];
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<Bidaiak>> GetBidaiakAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Bidaiak> result = [];
        return Task.FromResult(result);
    }
}
