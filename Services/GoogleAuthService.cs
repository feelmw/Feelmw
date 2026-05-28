using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace FeelmwLogistika.Blazor.Services;

public sealed class GoogleAuthService(IConfiguration configuration, IJSRuntime jsRuntime) : IGoogleAuthService
{
    private readonly string clientId = configuration["GoogleAuth:ClientId"] ?? "";

    public event Action? AuthStateChanged;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(clientId);
    public bool IsAuthenticated => CurrentUser is not null;
    public string AllowedDomain { get; } = configuration["GoogleAuth:AllowedDomain"] ?? "feelmw.com";
    public GoogleUserSession? CurrentUser { get; private set; }
    public string? ErrorMessage { get; private set; }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            ErrorMessage = "Google saioa ez dago konfiguratuta.";
            Notify();
            return;
        }

        try
        {
            await jsRuntime.InvokeVoidAsync("feelmwAuth.initialize", cancellationToken, clientId);
        }
        catch (JSException)
        {
            ErrorMessage = "Google saioa ezin izan da kargatu.";
            Notify();
        }
    }

    public Task SetCredentialAsync(string idToken, CancellationToken cancellationToken = default)
    {
        GoogleUserSession session;
        try
        {
            session = ParseSession(idToken);
        }
        catch (InvalidOperationException exception)
        {
            CurrentUser = null;
            ErrorMessage = exception.Message;
            Notify();
            return Task.CompletedTask;
        }
        if (!IsAllowedEmail(session.Email))
        {
            CurrentUser = null;
            ErrorMessage = $"Kontu hau ez dago baimenduta. Erabili @{AllowedDomain} kontu bat.";
            Notify();
            return Task.CompletedTask;
        }

        CurrentUser = session;
        ErrorMessage = null;
        Notify();
        return Task.CompletedTask;
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        string? email = CurrentUser?.Email;
        CurrentUser = null;
        ErrorMessage = null;
        await jsRuntime.InvokeVoidAsync("feelmwAuth.signOut", cancellationToken, email);
        Notify();
    }

    public Task<string> GetRequiredIdTokenAsync(CancellationToken cancellationToken = default)
    {
        if (CurrentUser is null || string.IsNullOrWhiteSpace(CurrentUser.IdToken))
        {
            throw new InvalidOperationException("Google saioa hasi behar da.");
        }

        return Task.FromResult(CurrentUser.IdToken);
    }

    private bool IsAllowedEmail(string email)
    {
        return email.EndsWith($"@{AllowedDomain}", StringComparison.OrdinalIgnoreCase);
    }

    private static GoogleUserSession ParseSession(string idToken)
    {
        string[] parts = idToken.Split('.');
        if (parts.Length < 2)
        {
            throw new InvalidOperationException("Google token baliogabea.");
        }

        using JsonDocument payload = JsonDocument.Parse(Base64UrlDecode(parts[1]));
        JsonElement root = payload.RootElement;
        string email = ReadString(root, "email");
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException("Google tokenak ez du emailik.");
        }

        return new GoogleUserSession
        {
            Email = email,
            Name = ReadString(root, "name"),
            Picture = ReadString(root, "picture"),
            IdToken = idToken
        };
    }

    private static string ReadString(JsonElement root, string propertyName)
    {
        return root.TryGetProperty(propertyName, out JsonElement value) ? value.GetString() ?? "" : "";
    }

    private static byte[] Base64UrlDecode(string value)
    {
        string padded = value.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + (4 - padded.Length % 4) % 4, '=');
        return Convert.FromBase64String(padded);
    }

    private void Notify()
    {
        AuthStateChanged?.Invoke();
    }
}
