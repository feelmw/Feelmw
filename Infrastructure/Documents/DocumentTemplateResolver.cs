namespace FeelmwLogistika.Blazor.Infrastructure.Documents;

internal static class DocumentTemplateResolver
{
    public static async Task<MemoryStream> LoadTemplateAsync(HttpClient httpClient, string templatePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(templatePath))
        {
            throw new FileNotFoundException("Ez da Word txantiloiaren ruta zehaztu.");
        }

        await using Stream response = await httpClient.GetStreamAsync(templatePath, cancellationToken);
        MemoryStream stream = new();
        await response.CopyToAsync(stream, cancellationToken);
        stream.Position = 0;
        return stream;
    }

    public static async Task<MemoryStream> LoadTemplateAsync(string templatePath, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        throw new NotSupportedException("GitHub Pages bertsioan txantiloiak HttpClient bidez kargatu behar dira.");
    }

    public static string SafeFileName(string value, string fallback)
    {
        string raw = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        foreach (char invalid in Path.GetInvalidFileNameChars())
        {
            raw = raw.Replace(invalid, '_');
        }

        return string.IsNullOrWhiteSpace(raw) ? fallback : raw;
    }
}
