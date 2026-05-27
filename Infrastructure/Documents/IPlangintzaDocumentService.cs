using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Infrastructure.Documents;

public interface IPlangintzaDocumentService
{
    DocumentGenerationResult CreateDocument(
        Stream templateStream,
        IEnumerable<Bidaiak> bidaiak,
        string izena,
        string ikastetxea);

    Task<DocumentGenerationResult> CreateDocumentFromTemplatePathAsync(
        string templatePath,
        IEnumerable<Bidaiak> bidaiak,
        string izena,
        string ikastetxea,
        CancellationToken cancellationToken = default);
}
