using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Infrastructure.Excel;

public interface IPlangintzaExcelService
{
    byte[] CreateWorkbook(HotelDatuak hotela, int egunKop, IEnumerable<EgunLaburpena> egunak, IEnumerable<EkintzaDatuak> ekintzak);
    byte[] CreateWorkbook(IEnumerable<Bidaiak> bidaiak);
    Task SaveWorkbookAsync(string path, IEnumerable<Bidaiak> bidaiak, CancellationToken cancellationToken = default);
    PlangintzaDatuak ReadWorkbook(Stream stream);
}
