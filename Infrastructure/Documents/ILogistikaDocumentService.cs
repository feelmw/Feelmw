using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Infrastructure.Documents;

public interface ILogistikaDocumentService
{
    DocumentGenerationResult CreateDocument(
        Stream templateStream,
        IEnumerable<Ostalak> ostalak,
        IEnumerable<Ekintzak> ekintzak,
        IEnumerable<Garraioak> garraioak,
        string nombreUsuario,
        DocumentHeaderData headerData);

    Task<DocumentGenerationResult> CreateDocumentFromTemplatePathAsync(
        string templatePath,
        IEnumerable<Ostalak> ostalak,
        IEnumerable<Ekintzak> ekintzak,
        IEnumerable<Garraioak> garraioak,
        string nombreUsuario,
        DocumentHeaderData headerData,
        CancellationToken cancellationToken = default);
}
