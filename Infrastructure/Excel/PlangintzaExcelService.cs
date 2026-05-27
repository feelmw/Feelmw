using ClosedXML.Excel;
using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Infrastructure.Excel;

public sealed class PlangintzaExcelService : IPlangintzaExcelService
{
    public byte[] CreateWorkbook(HotelDatuak hotela, int egunKop, IEnumerable<EgunLaburpena> egunak, IEnumerable<EkintzaDatuak> ekintzak)
    {
        using XLWorkbook workbook = new();
        IXLWorksheet laburpenaSheet = workbook.Worksheets.Add("Laburpena");
        IXLWorksheet hotelakSheet = workbook.Worksheets.Add("Hotelak");
        IXLWorksheet egunakSheet = workbook.Worksheets.Add("Egunak");
        IXLWorksheet ekintzakSheet = workbook.Worksheets.Add("Ekintzak");

        LaburpenaIdatzi(laburpenaSheet, hotela.Izena, egunKop);
        HotelakIdatzi(hotelakSheet, hotela);
        EgunakIdatzi(egunakSheet, egunak);
        EkintzakIdatzi(ekintzakSheet, ekintzak);

        return ToBytes(workbook);
    }

    public byte[] CreateWorkbook(IEnumerable<Bidaiak> bidaiak)
    {
        List<Bidaiak> bidaiakList = bidaiak.ToList();

        using XLWorkbook workbook = new();
        IXLWorksheet laburpenaSheet = workbook.Worksheets.Add("Laburpena");
        IXLWorksheet hotelakSheet = workbook.Worksheets.Add("Hotelak");
        IXLWorksheet egunakSheet = workbook.Worksheets.Add("Egunak");
        IXLWorksheet ekintzakSheet = workbook.Worksheets.Add("Ekintzak");

        LaburpenaIdatzi(laburpenaSheet, "Plangintza", bidaiakList.Sum(b => b.EgunKop));
        HotelakIdatzi(hotelakSheet, bidaiakList);
        EgunakIdatzi(egunakSheet, bidaiakList);
        EkintzakIdatzi(ekintzakSheet, bidaiakList);

        return ToBytes(workbook);
    }

    public async Task SaveWorkbookAsync(string path, IEnumerable<Bidaiak> bidaiak, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        throw new NotSupportedException("GitHub Pages bertsioan Excel fitxategiak nabigatzailean deskargatzen dira.");
    }

    public PlangintzaDatuak ReadWorkbook(Stream stream)
    {
        using XLWorkbook workbook = new(stream);

        if (workbook.Worksheets.TryGetWorksheet("Laburpena", out IXLWorksheet? laburpenaSheet)
            && workbook.Worksheets.TryGetWorksheet("Hotelak", out IXLWorksheet? hotelakSheet)
            && workbook.Worksheets.TryGetWorksheet("Egunak", out IXLWorksheet? egunakSheet)
            && workbook.Worksheets.TryGetWorksheet("Ekintzak", out IXLWorksheet? ekintzakSheet))
        {
            return EgituraBerriaIrakurri(laburpenaSheet, hotelakSheet, egunakSheet, ekintzakSheet);
        }

        return EgituraZaharraIrakurri(workbook.Worksheets.First());
    }

    private static PlangintzaDatuak EgituraBerriaIrakurri(IXLWorksheet laburpenaSheet, IXLWorksheet hotelakSheet, IXLWorksheet egunakSheet, IXLWorksheet ekintzakSheet)
    {
        PlangintzaDatuak datuak = new()
        {
            Ostala = CeldaTestua(laburpenaSheet.Cell(1, 2)),
            EgunKop = Math.Max(1, CeldaEnteroa(laburpenaSheet.Cell(2, 2), 1))
        };

        if (TaulaAnitzaDa(hotelakSheet))
        {
            datuak.Bidaiak = BidaiakIrakurri(hotelakSheet, egunakSheet, ekintzakSheet);
            Bidaiak? lehenBidaia = datuak.Bidaiak.FirstOrDefault();
            datuak.Hotela = lehenBidaia is null ? null : HotelaDatuakSortu(lehenBidaia);
            datuak.Ostala = lehenBidaia?.HotelHautatua?.Izena ?? datuak.Ostala;
            datuak.EgunKop = lehenBidaia?.EgunKop ?? 1;
            datuak.Egunak = datuak.Bidaiak.SelectMany(b => b.EgunLaburpenak).ToList();
            datuak.Ekintzak = datuak.Bidaiak.SelectMany(b => b.EkintzaDatuak).ToList();
            datuak.Data = datuak.Egunak.FirstOrDefault()?.Data ?? "";
            return datuak;
        }

        datuak.Hotela = HotelaIrakurri(hotelakSheet, datuak.EgunKop);
        datuak.Ostala = datuak.Hotela?.Izena ?? datuak.Ostala;

        IXLRange? egunakRange = egunakSheet.RangeUsed();
        if (egunakRange is not null)
        {
            foreach (IXLRangeRow row in egunakRange.RowsUsed().Skip(1))
            {
                datuak.Egunak.Add(new EgunLaburpena(
                    CeldaEnteroa(row.Cell(1), datuak.Egunak.Count + 1),
                    CeldaTestua(row.Cell(2)),
                    CeldaTestua(row.Cell(3)),
                    CeldaTestua(row.Cell(4)),
                    CeldaTestua(row.Cell(5))));
            }
        }

        datuak.Data = datuak.Egunak.FirstOrDefault()?.Data ?? "";

        IXLRange? ekintzakRange = ekintzakSheet.RangeUsed();
        if (ekintzakRange is not null)
        {
            foreach (IXLRangeRow row in ekintzakRange.RowsUsed().Skip(1))
            {
                datuak.Ekintzak.Add(new EkintzaDatuak(
                    CeldaTestua(row.Cell(1)),
                    CeldaTestua(row.Cell(2)),
                    CeldaTestua(row.Cell(3)),
                    CeldaTestua(row.Cell(4))));
            }
        }

        return datuak;
    }

    private static List<Bidaiak> BidaiakIrakurri(IXLWorksheet hotelakSheet, IXLWorksheet egunakSheet, IXLWorksheet ekintzakSheet)
    {
        Dictionary<int, Hotelak> hotelak = [];
        Dictionary<int, int> egunKop = [];
        Dictionary<int, List<EgunLaburpena>> egunak = [];
        Dictionary<int, List<EkintzaDatuak>> ekintzak = [];

        foreach (IXLRow row in hotelakSheet.RowsUsed().Skip(1))
        {
            int zatia = CeldaEnteroa(row.Cell(1), hotelak.Count + 1);
            hotelak[zatia] = new Hotelak(
                CeldaTestua(row.Cell(2)),
                CeldaTestua(row.Cell(3)),
                CeldaTestua(row.Cell(4)));
            egunKop[zatia] = Math.Max(1, CeldaEnteroa(row.Cell(5), 1));
        }

        foreach (IXLRow row in egunakSheet.RowsUsed().Skip(1))
        {
            int zatia = CeldaEnteroa(row.Cell(1), 1);
            if (!egunak.ContainsKey(zatia))
            {
                egunak[zatia] = [];
            }

            egunak[zatia].Add(new EgunLaburpena(
                CeldaEnteroa(row.Cell(3), egunak[zatia].Count + 1),
                CeldaTestua(row.Cell(4)),
                CeldaTestua(row.Cell(5)),
                CeldaTestua(row.Cell(6)),
                CeldaTestua(row.Cell(7))));
        }

        foreach (IXLRow row in ekintzakSheet.RowsUsed().Skip(1))
        {
            int zatia = CeldaEnteroa(row.Cell(1), 1);
            if (!ekintzak.ContainsKey(zatia))
            {
                ekintzak[zatia] = [];
            }

            ekintzak[zatia].Add(new EkintzaDatuak(
                CeldaTestua(row.Cell(3)),
                CeldaTestua(row.Cell(4)),
                CeldaTestua(row.Cell(5)),
                CeldaTestua(row.Cell(6))));
        }

        return hotelak
            .OrderBy(h => h.Key)
            .Select(h => new Bidaiak(
                h.Value,
                egunKop.TryGetValue(h.Key, out int kop) ? kop : Math.Max(1, egunak.GetValueOrDefault(h.Key)?.Count ?? 1),
                egunak.GetValueOrDefault(h.Key) ?? [],
                ekintzak.GetValueOrDefault(h.Key) ?? []))
            .ToList();
    }

    private static PlangintzaDatuak EgituraZaharraIrakurri(IXLWorksheet sheet)
    {
        IXLRange? range = sheet.RangeUsed();
        PlangintzaDatuak datuak = new();

        if (range is null)
        {
            return datuak;
        }

        IXLRangeRow row = GoiburuaDa(CeldaTestua(range.FirstRowUsed().Cell(1)))
            ? range.RowsUsed().Skip(1).FirstOrDefault() ?? range.FirstRowUsed()
            : range.FirstRowUsed();

        datuak.Ostala = CeldaTestua(row.Cell(1));
        datuak.EgunKop = Math.Max(1, CeldaEnteroa(row.Cell(2), 1));
        datuak.Data = CeldaTestua(row.Cell(3));
        datuak.Egunak.Add(new EgunLaburpena(
            1,
            datuak.Data,
            CeldaTestua(row.Cell(4)),
            CeldaTestua(row.Cell(5)),
            CeldaTestua(row.Cell(6))));

        string ordua = CeldaTestua(row.Cell(7));
        string mota = CeldaTestua(row.Cell(8));
        string deskribapena = CeldaTestua(row.Cell(9));
        if (!string.IsNullOrWhiteSpace(ordua)
            || !string.IsNullOrWhiteSpace(mota)
            || !string.IsNullOrWhiteSpace(deskribapena))
        {
            datuak.Ekintzak.Add(new EkintzaDatuak(datuak.Data, ordua, mota, deskribapena));
        }

        datuak.Hotela = new HotelDatuak("", datuak.Ostala, "", datuak.EgunKop, datuak.Data);
        return datuak;
    }

    private static HotelDatuak HotelaDatuakSortu(Bidaiak bidaia)
    {
        Hotelak? hotela = bidaia.HotelHautatua;
        return new HotelDatuak(
            hotela?.Hiria ?? "",
            hotela?.Izena ?? "",
            hotela?.HelbideaUrl ?? "",
            bidaia.EgunKop,
            bidaia.EgunLaburpenak.FirstOrDefault()?.Data ?? "");
    }

    private static HotelDatuak HotelaIrakurri(IXLWorksheet sheet, int egunKop)
    {
        IXLRange? range = sheet.RangeUsed();
        if (range is null)
        {
            return new HotelDatuak("", "", "", egunKop, "");
        }

        IXLRangeRow? row = range.RowsUsed().Skip(1).FirstOrDefault();
        if (row is null)
        {
            return new HotelDatuak("", "", "", egunKop, "");
        }

        return new HotelDatuak(
            CeldaTestua(row.Cell(1)),
            CeldaTestua(row.Cell(2)),
            CeldaTestua(row.Cell(3)),
            CeldaEnteroa(row.Cell(4), egunKop),
            CeldaTestua(row.Cell(5)));
    }

    private static void LaburpenaIdatzi(IXLWorksheet sheet, string ostala, int egunKop)
    {
        sheet.Cell(1, 1).Value = "Ostala";
        sheet.Cell(1, 2).Value = ostala;
        sheet.Cell(2, 1).Value = "Egun kopurua";
        sheet.Cell(2, 2).Value = egunKop;
        sheet.Columns().AdjustToContents();
    }

    private static void HotelakIdatzi(IXLWorksheet sheet, HotelDatuak hotela)
    {
        sheet.Cell(1, 1).Value = "Hiria";
        sheet.Cell(1, 2).Value = "Hotel izena";
        sheet.Cell(1, 3).Value = "Helbidea / URL";
        sheet.Cell(1, 4).Value = "Egun kopurua";
        sheet.Cell(1, 5).Value = "Data";
        sheet.Cell(2, 1).Value = hotela.Hiria;
        sheet.Cell(2, 2).Value = hotela.Izena;
        sheet.Cell(2, 3).Value = hotela.HelbideaUrl;
        sheet.Cell(2, 4).Value = hotela.EgunKop;
        sheet.Cell(2, 5).Value = hotela.Data;
        sheet.Columns().AdjustToContents();
    }

    private static void HotelakIdatzi(IXLWorksheet sheet, List<Bidaiak> bidaiak)
    {
        sheet.Cell(1, 1).Value = "Bidaia zatia";
        sheet.Cell(1, 2).Value = "Hiria";
        sheet.Cell(1, 3).Value = "Hotel izena";
        sheet.Cell(1, 4).Value = "Helbidea / URL";
        sheet.Cell(1, 5).Value = "Egun kopurua";
        sheet.Cell(1, 6).Value = "Data";

        int row = 2;
        for (int i = 0; i < bidaiak.Count; i++)
        {
            Bidaiak bidaia = bidaiak[i];
            Hotelak? hotela = bidaia.HotelHautatua;
            sheet.Cell(row, 1).Value = i + 1;
            sheet.Cell(row, 2).Value = hotela?.Hiria ?? "";
            sheet.Cell(row, 3).Value = hotela?.Izena ?? "";
            sheet.Cell(row, 4).Value = hotela?.HelbideaUrl ?? "";
            sheet.Cell(row, 5).Value = bidaia.EgunKop;
            sheet.Cell(row, 6).Value = bidaia.EgunLaburpenak.FirstOrDefault()?.Data ?? "";
            row++;
        }

        sheet.Columns().AdjustToContents();
    }

    private static void EgunakIdatzi(IXLWorksheet sheet, IEnumerable<EgunLaburpena> egunak)
    {
        sheet.Cell(1, 1).Value = "Eguna";
        sheet.Cell(1, 2).Value = "Data";
        sheet.Cell(1, 3).Value = "Goisa";
        sheet.Cell(1, 4).Value = "Arratsaldea";
        sheet.Cell(1, 5).Value = "Gaua";

        int row = 2;
        foreach (EgunLaburpena eguna in egunak)
        {
            sheet.Cell(row, 1).Value = eguna.Eguna;
            sheet.Cell(row, 2).Value = eguna.Data;
            sheet.Cell(row, 3).Value = eguna.Goisa;
            sheet.Cell(row, 4).Value = eguna.Arratsaldea;
            sheet.Cell(row, 5).Value = eguna.Gaua;
            row++;
        }

        sheet.Columns().AdjustToContents();
    }

    private static void EgunakIdatzi(IXLWorksheet sheet, List<Bidaiak> bidaiak)
    {
        sheet.Cell(1, 1).Value = "Bidaia zatia";
        sheet.Cell(1, 2).Value = "Hotela";
        sheet.Cell(1, 3).Value = "Eguna";
        sheet.Cell(1, 4).Value = "Data";
        sheet.Cell(1, 5).Value = "Goisa";
        sheet.Cell(1, 6).Value = "Arratsaldea";
        sheet.Cell(1, 7).Value = "Gaua";

        int row = 2;
        for (int i = 0; i < bidaiak.Count; i++)
        {
            Bidaiak bidaia = bidaiak[i];
            foreach (EgunLaburpena eguna in bidaia.EgunLaburpenak)
            {
                sheet.Cell(row, 1).Value = i + 1;
                sheet.Cell(row, 2).Value = bidaia.HotelHautatua?.Izena ?? "";
                sheet.Cell(row, 3).Value = eguna.Eguna;
                sheet.Cell(row, 4).Value = eguna.Data;
                sheet.Cell(row, 5).Value = eguna.Goisa;
                sheet.Cell(row, 6).Value = eguna.Arratsaldea;
                sheet.Cell(row, 7).Value = eguna.Gaua;
                row++;
            }
        }

        sheet.Columns().AdjustToContents();
    }

    private static void EkintzakIdatzi(IXLWorksheet sheet, IEnumerable<EkintzaDatuak> ekintzak)
    {
        sheet.Cell(1, 1).Value = "Eguna";
        sheet.Cell(1, 2).Value = "Ordua";
        sheet.Cell(1, 3).Value = "Mota";
        sheet.Cell(1, 4).Value = "Deskribapena";

        int row = 2;
        foreach (EkintzaDatuak ekintza in ekintzak)
        {
            sheet.Cell(row, 1).Value = ekintza.Eguna;
            sheet.Cell(row, 2).Value = ekintza.Ordua;
            sheet.Cell(row, 3).Value = ekintza.Mota;
            sheet.Cell(row, 4).Value = ekintza.Deskribapena;
            row++;
        }

        sheet.Columns().AdjustToContents();
    }

    private static void EkintzakIdatzi(IXLWorksheet sheet, List<Bidaiak> bidaiak)
    {
        sheet.Cell(1, 1).Value = "Bidaia zatia";
        sheet.Cell(1, 2).Value = "Hotela";
        sheet.Cell(1, 3).Value = "Eguna";
        sheet.Cell(1, 4).Value = "Ordua";
        sheet.Cell(1, 5).Value = "Mota";
        sheet.Cell(1, 6).Value = "Deskribapena";

        int row = 2;
        for (int i = 0; i < bidaiak.Count; i++)
        {
            Bidaiak bidaia = bidaiak[i];
            foreach (EkintzaDatuak ekintza in bidaia.EkintzaDatuak)
            {
                sheet.Cell(row, 1).Value = i + 1;
                sheet.Cell(row, 2).Value = bidaia.HotelHautatua?.Izena ?? "";
                sheet.Cell(row, 3).Value = ekintza.Eguna;
                sheet.Cell(row, 4).Value = ekintza.Ordua;
                sheet.Cell(row, 5).Value = ekintza.Mota;
                sheet.Cell(row, 6).Value = ekintza.Deskribapena;
                row++;
            }
        }

        sheet.Columns().AdjustToContents();
    }

    private static bool TaulaAnitzaDa(IXLWorksheet sheet) => string.Equals(CeldaTestua(sheet.Cell(1, 1)), "Bidaia zatia", StringComparison.OrdinalIgnoreCase);

    private static int CeldaEnteroa(IXLCell cell, int lehenetsia)
    {
        if (cell.TryGetValue(out int balioa))
        {
            return balioa;
        }

        string testua = CeldaTestua(cell);
        return int.TryParse(testua, out balioa) ? balioa : lehenetsia;
    }

    private static string CeldaTestua(IXLCell cell) => cell.GetFormattedString() ?? "";

    private static bool GoiburuaDa(string balioa)
    {
        balioa = (balioa ?? "").Trim();
        return string.Equals(balioa, "Ostala", StringComparison.OrdinalIgnoreCase)
            || string.Equals(balioa, "Hotela", StringComparison.OrdinalIgnoreCase)
            || string.Equals(balioa, "Hotel izena", StringComparison.OrdinalIgnoreCase);
    }

    private static byte[] ToBytes(XLWorkbook workbook)
    {
        using MemoryStream stream = new();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
