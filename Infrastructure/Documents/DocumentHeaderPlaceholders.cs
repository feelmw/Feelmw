using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Infrastructure.Documents;

internal static class DocumentHeaderPlaceholders
{
    public static IReadOnlyList<DocumentPlaceholderValue> From(DocumentHeaderData headerData)
    {
        return
        [
            new("{{IKASTETXEA}}", headerData.IkastetxeaOsatua, Required: true),
            new("{{DATAK}}", headerData.Datak, Required: true),
            new("{{IBILBIDEA}}", headerData.Ibilbidea, Required: true),
            new("{{IRAKASLEAK}}", headerData.Irakasleak, Required: true),
            new("{{BEGIRALEAK}}", headerData.Begiraleak, Required: true),
            new("{{IKASLEAK}}", headerData.Ikasleak, Required: true),
            new("{{GIDARIAK}}", headerData.Gidariak, Required: true),
            new("{{POLIZA_IKASLEAK}}", headerData.PolizaIkasleak, Required: true),
            new("{{POLIZA_BEGIRALEAK}}", headerData.PolizaBegiraleak, Required: true)
        ];
    }
}
