namespace FeelmwLogistika.Blazor.Services;

public interface IGoogleAuthService
{
    event Action? AuthStateChanged;

    bool IsConfigured { get; }
    bool IsAuthenticated { get; }
    string AllowedDomain { get; }
    GoogleUserSession? CurrentUser { get; }
    string? ErrorMessage { get; }

    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task SetCredentialAsync(string idToken, CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
    Task<string> GetRequiredIdTokenAsync(CancellationToken cancellationToken = default);
    Task<string> GetRequiredAccessTokenAsync(string scopes, CancellationToken cancellationToken = default);
}
