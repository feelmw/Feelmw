using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Infrastructure.Excel;

public interface ILogistikaExcelService
{
    byte[] CreateWorkbook(LogistikaDatuak datuak);
    byte[] CreateWorkbook(IEnumerable<Ostalak> ostalak, IEnumerable<Ekintzak> ekintzak, IEnumerable<Garraioak> garraioak);
    Task SaveWorkbookAsync(string path, LogistikaDatuak datuak, CancellationToken cancellationToken = default);
}
