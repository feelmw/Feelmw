namespace FeelmwLogistika.Blazor.Services;

public sealed class DocumentWorkflowService : IDocumentWorkflowService
{
    public Task CreateNewDocumentAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task OpenExistingDocumentAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task CreateIntermediateExcelAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task CreateFinalDocumentFromTemplateAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
