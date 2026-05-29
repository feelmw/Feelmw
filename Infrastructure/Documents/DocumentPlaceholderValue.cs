namespace FeelmwLogistika.Blazor.Infrastructure.Documents;

public readonly record struct DocumentPlaceholderValue(string Marker, string? Value, bool Required = false);
