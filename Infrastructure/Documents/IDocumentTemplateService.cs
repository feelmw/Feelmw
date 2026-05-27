namespace FeelmwLogistika.Blazor.Infrastructure.Documents;

public interface IDocumentTemplateService
{
    Task CreateDocumentFromTemplateAsync(CancellationToken cancellationToken = default);
    Task EditDocumentAsync(CancellationToken cancellationToken = default);
}
