namespace FeelmwLogistika.Blazor.Infrastructure.Documents;

public sealed class DocumentTemplateService : IDocumentTemplateService
{
    public Task CreateDocumentFromTemplateAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task EditDocumentAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
