using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public interface IPlangintzaService
{
    Task<IReadOnlyList<Hotelak>> GetHotelakAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Bidaiak>> GetBidaiakAsync(CancellationToken cancellationToken = default);
}
