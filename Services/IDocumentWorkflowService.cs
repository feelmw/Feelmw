namespace FeelmwLogistika.Blazor.Services;

public interface IDocumentWorkflowService
{
    Task CreateNewDocumentAsync(CancellationToken cancellationToken = default);
    Task OpenExistingDocumentAsync(CancellationToken cancellationToken = default);
    Task CreateIntermediateExcelAsync(CancellationToken cancellationToken = default);
    Task CreateFinalDocumentFromTemplateAsync(CancellationToken cancellationToken = default);
}
