using ClosedXML.Excel;
using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Infrastructure.Excel;

public sealed class LogistikaExcelService : ILogistikaExcelService
{
    public byte[] CreateWorkbook(LogistikaDatuak datuak)
    {
        return CreateWorkbook(datuak.Ostalak, datuak.Ekintzak, datuak.Garraioak);
    }

    public byte[] CreateWorkbook(IEnumerable<Ostalak> ostalak, IEnumerable<Ekintzak> ekintzak, IEnumerable<Garraioak> garraioak)
    {
        using XLWorkbook workbook = new();
        WriteOstalak(workbook.Worksheets.Add("Ostalak"), ostalak);
        WriteEkintzak(workbook.Worksheets.Add("Ekintzak"), ekintzak);
        WriteGarraioak(workbook.Worksheets.Add("Garraioak"), garraioak);
        return ToBytes(workbook);
    }

    public async Task SaveWorkbookAsync(string path, LogistikaDatuak datuak, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        throw new NotSupportedException("GitHub Pages bertsioan Excel fitxategiak nabigatzailean deskargatzen dira.");
    }

    private static void WriteOstalak(IXLWorksheet sheet, IEnumerable<Ostalak> ostalak)
    {
        string[] headers =
        [
            "Ostala",
            "Bonoa",
            "Helbidea",
            "Lokalizatzailea",
            "Gauak",
            "Datak",
            "Gelak",
            "Checkin",
            "Checkout",
            "Dokumentazioa",
            "Harrera",
            "Gosaria",
            "Bazkaria",
            "Afaria",
            "Toailak",
            "Izarak",
            "Fidantza prezioa",
            "Luggage prezioa",
            "Instalazioak"
        ];

        WriteHeaders(sheet, headers);

        int row = 2;
        foreach (Ostalak o in ostalak)
        {
            sheet.Cell(row, 1).Value = o.OstalaIzena;
            sheet.Cell(row, 2).Value = o.Bonoa;
            sheet.Cell(row, 3).Value = o.Helbidea;
            sheet.Cell(row, 4).Value = o.Lokalizatzailea;
            sheet.Cell(row, 5).Value = o.Gauak;
            sheet.Cell(row, 6).Value = o.Datak;
            sheet.Cell(row, 7).Value = o.Gelak;
            sheet.Cell(row, 8).Value = o.Checkin;
            sheet.Cell(row, 9).Value = o.Checkout;
            sheet.Cell(row, 10).Value = o.Dokumentazioa;
            sheet.Cell(row, 11).Value = o.Harrera;
            sheet.Cell(row, 12).Value = o.Gosaria;
            sheet.Cell(row, 13).Value = o.Bazkaria;
            sheet.Cell(row, 14).Value = o.Afaria;
            sheet.Cell(row, 15).Value = BoolToBaiEz(o.Toailak);
            sheet.Cell(row, 16).Value = BoolToBaiEz(o.Izarak);
            sheet.Cell(row, 17).Value = o.Fidantza;
            sheet.Cell(row, 18).Value = o.Luggage;
            sheet.Cell(row, 19).Value = o.Instalazioak;
            row++;
        }

        sheet.Columns().AdjustToContents();
    }

    private static void WriteEkintzak(IXLWorksheet sheet, IEnumerable<Ekintzak> ekintzak)
    {
        int row = 1;
        foreach (Ekintzak e in ekintzak)
        {
            sheet.Cell(row, 1).Value = e.EkintzaIzena;
            sheet.Cell(row, 2).Value = e.Bonoa;
            sheet.Cell(row, 3).Value = e.Iraupena;
            sheet.Cell(row, 4).Value = e.Kontaktua;
            sheet.Cell(row, 5).Value = e.Elkartokia;
            sheet.Cell(row, 6).Value = e.Iristean;
            sheet.Cell(row, 7).Value = e.EramanM;
            sheet.Cell(row, 8).Value = e.BertanM;
            sheet.Cell(row, 9).Value = BoolToBaiEz(e.Aldagela);
            sheet.Cell(row, 10).Value = BoolToBaiEz(e.Komuna);
            sheet.Cell(row, 11).Value = e.Egonlekua;
            sheet.Cell(row, 12).Value = e.Informazioa;
            sheet.Cell(row, 13).Value = e.Lokali;
            row++;
        }

        sheet.Columns().AdjustToContents();
    }

    private static void WriteGarraioak(IXLWorksheet sheet, IEnumerable<Garraioak> garraioak)
    {
        int row = 1;
        foreach (Garraioak g in garraioak)
        {
            sheet.Cell(row, 1).Value = g.GarraioaIzena;
            sheet.Cell(row, 2).Value = g.Eguna;
            sheet.Cell(row, 3).Value = g.Ordutegia;
            sheet.Cell(row, 4).Value = g.Lokalizatzailea;
            sheet.Cell(row, 5).Value = g.Kontaktua;
            sheet.Cell(row, 6).Value = g.Elkargunea;
            sheet.Cell(row, 7).Value = g.Eginbeharrak;
            sheet.Cell(row, 8).Value = g.Informazioa;
            row++;
        }

        sheet.Columns().AdjustToContents();
    }

    private static void WriteHeaders(IXLWorksheet sheet, IReadOnlyList<string> headers)
    {
        for (int i = 0; i < headers.Count; i++)
        {
            sheet.Cell(1, i + 1).Value = headers[i];
        }
    }

    private static byte[] ToBytes(XLWorkbook workbook)
    {
        using MemoryStream stream = new();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static string BoolToBaiEz(bool value) => value ? "Bai" : "Ez";
}
