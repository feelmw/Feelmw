namespace FeelmwLogistika.Blazor.Infrastructure.Excel;

public interface IExcelInfrastructureService
{
    Task CreateIntermediateWorkbookAsync(CancellationToken cancellationToken = default);
    Task ReadWorkbookAsync(CancellationToken cancellationToken = default);
}
