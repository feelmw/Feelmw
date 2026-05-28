namespace FeelmwLogistika.Blazor.Services;

public sealed class GoogleUserSession
{
    public string Email { get; init; } = "";
    public string Name { get; init; } = "";
    public string Picture { get; init; } = "";
    public string IdToken { get; init; } = "";
}
