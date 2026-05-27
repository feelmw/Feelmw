namespace FeelmwLogistika.Blazor.Infrastructure.Excel;

public sealed class ExcelInfrastructureService : IExcelInfrastructureService
{
    public Task CreateIntermediateWorkbookAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task ReadWorkbookAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
