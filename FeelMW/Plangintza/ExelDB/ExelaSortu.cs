using ClosedXML.Excel;
using FeelmwLogistika.Plangintza.DatuModeloak;

namespace FeelmwLogistika.Plangintza.ExelDB
{
    public class ExelaSortu
    {
        public class HotelDatuak
        {
            public HotelDatuak(string hiria, string izena, string helbideaUrl, int egunKop, string data)
            {
                Hiria = hiria;
                Izena = izena;
                HelbideaUrl = helbideaUrl;
                EgunKop = egunKop;
                Data = data;
            }

            public string Hiria { get; }
            public string Izena { get; }
            public string HelbideaUrl { get; }
            public int EgunKop { get; }
            public string Data { get; }
        }

        public class EgunLaburpena
        {
            public EgunLaburpena(int eguna, string data, string goisa, string arratsaldea, string gaua)
            {
                Eguna = eguna;
                Data = data;
                Goisa = goisa;
                Arratsaldea = arratsaldea;
                Gaua = gaua;
            }

            public int Eguna { get; }
            public string Data { get; }
            public string Goisa { get; }
            public string Arratsaldea { get; }
            public string Gaua { get; }
        }

        public class EkintzaDatuak
        {
            public EkintzaDatuak(string eguna, string ordua, string mota, string deskribapena)
            {
                Eguna = eguna;
                Ordua = ordua;
                Mota = mota;
                Deskribapena = deskribapena;
            }

            public string Eguna { get; }
            public string Ordua { get; }
            public string Mota { get; }
            public string Deskribapena { get; }
        }

        public class PlangintzaDatuak
        {
            public string Ostala { get; set; } = "";
            public HotelDatuak? Hotela { get; set; }
            public int EgunKop { get; set; } = 1;
            public string Data { get; set; } = "";
            public List<EgunLaburpena> Egunak { get; set; } = new List<EgunLaburpena>();
            public List<EkintzaDatuak> Ekintzak { get; set; } = new List<EkintzaDatuak>();
            public List<Bidaiak> Bidaiak { get; set; } = new List<Bidaiak>();
        }

        public static string Gorde(
            string izena,
            HotelDatuak hotela,
            int egunKop,
            List<EgunLaburpena> egunak,
            List<EkintzaDatuak> ekintzak)
        {
            string ruta = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                $"Plangintza - {izena}.xlsx"
            );

            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }

            using XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet laburpenaSheet = workbook.Worksheets.Add("Laburpena");
            IXLWorksheet hotelakSheet = workbook.Worksheets.Add("Hotelak");
            IXLWorksheet egunakSheet = workbook.Worksheets.Add("Egunak");
            IXLWorksheet ekintzakSheet = workbook.Worksheets.Add("Ekintzak");

            LaburpenaIdatzi(laburpenaSheet, hotela.Izena, egunKop);
            HotelakIdatzi(hotelakSheet, hotela);
            EgunakIdatzi(egunakSheet, egunak);
            EkintzakIdatzi(ekintzakSheet, ekintzak);

            workbook.SaveAs(ruta);
            return ruta;
        }

        public static string Gorde(string izena, List<Bidaiak> bidaiak)
        {
            string ruta = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                $"Plangintza - {izena}.xlsx"
            );

            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }

            using XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet laburpenaSheet = workbook.Worksheets.Add("Laburpena");
            IXLWorksheet hotelakSheet = workbook.Worksheets.Add("Hotelak");
            IXLWorksheet egunakSheet = workbook.Worksheets.Add("Egunak");
            IXLWorksheet ekintzakSheet = workbook.Worksheets.Add("Ekintzak");

            LaburpenaIdatzi(laburpenaSheet, "Plangintza", bidaiak.Sum(b => b.EgunKop));
            HotelakIdatzi(hotelakSheet, bidaiak);
            EgunakIdatzi(egunakSheet, bidaiak);
            EkintzakIdatzi(ekintzakSheet, bidaiak);

            workbook.SaveAs(ruta);
            return ruta;
        }

        public static PlangintzaDatuak Irakurri(string ruta)
        {
            using XLWorkbook workbook = new XLWorkbook(ruta);

            if (workbook.Worksheets.TryGetWorksheet("Laburpena", out var laburpenaSheet)
                && workbook.Worksheets.TryGetWorksheet("Hotelak", out var hotelakSheet)
                && workbook.Worksheets.TryGetWorksheet("Egunak", out var egunakSheet)
                && workbook.Worksheets.TryGetWorksheet("Ekintzak", out var ekintzakSheet))
            {
                return EgituraBerriaIrakurri(laburpenaSheet, hotelakSheet, egunakSheet, ekintzakSheet);
            }

            return EgituraZaharraIrakurri(workbook.Worksheets.First());
        }

        private static PlangintzaDatuak EgituraBerriaIrakurri(
            IXLWorksheet laburpenaSheet,
            IXLWorksheet hotelakSheet,
            IXLWorksheet egunakSheet,
            IXLWorksheet ekintzakSheet)
        {
            PlangintzaDatuak datuak = new PlangintzaDatuak
            {
                Ostala = CeldaTestua(laburpenaSheet.Cell(1, 2)),
                EgunKop = Math.Max(1, CeldaEnteroa(laburpenaSheet.Cell(2, 2), 1))
            };

            if (TaulaAnitzaDa(hotelakSheet))
            {
                datuak.Bidaiak = BidaiakIrakurri(hotelakSheet, egunakSheet, ekintzakSheet);
                Bidaiak? lehenBidaia = datuak.Bidaiak.FirstOrDefault();
                datuak.Hotela = lehenBidaia == null ? null : HotelaDatuakSortu(lehenBidaia);
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
            if (egunakRange != null)
            {
                foreach (IXLRangeRow row in egunakRange.RowsUsed().Skip(1))
                {
                    datuak.Egunak.Add(new EgunLaburpena(
                        CeldaEnteroa(row.Cell(1), datuak.Egunak.Count + 1),
                        CeldaTestua(row.Cell(2)),
                        CeldaTestua(row.Cell(3)),
                        CeldaTestua(row.Cell(4)),
                        CeldaTestua(row.Cell(5))
                    ));
                }
            }

            datuak.Data = datuak.Egunak.FirstOrDefault()?.Data ?? "";

            IXLRange? ekintzakRange = ekintzakSheet.RangeUsed();
            if (ekintzakRange != null)
            {
                foreach (IXLRangeRow row in ekintzakRange.RowsUsed().Skip(1))
                {
                    datuak.Ekintzak.Add(new EkintzaDatuak(
                        CeldaTestua(row.Cell(1)),
                        CeldaTestua(row.Cell(2)),
                        CeldaTestua(row.Cell(3)),
                        CeldaTestua(row.Cell(4))
                    ));
                }
            }

            return datuak;
        }

        private static bool TaulaAnitzaDa(IXLWorksheet sheet)
        {
            return string.Equals(CeldaTestua(sheet.Cell(1, 1)), "Bidaia zatia", StringComparison.OrdinalIgnoreCase);
        }

        private static List<Bidaiak> BidaiakIrakurri(IXLWorksheet hotelakSheet, IXLWorksheet egunakSheet, IXLWorksheet ekintzakSheet)
        {
            Dictionary<int, Hotelak> hotelak = new Dictionary<int, Hotelak>();
            Dictionary<int, int> egunKop = new Dictionary<int, int>();
            Dictionary<int, List<EgunLaburpena>> egunak = new Dictionary<int, List<EgunLaburpena>>();
            Dictionary<int, List<EkintzaDatuak>> ekintzak = new Dictionary<int, List<EkintzaDatuak>>();

            foreach (var row in hotelakSheet.RowsUsed().Skip(1))
            {
                int zatia = CeldaEnteroa(row.Cell(1), hotelak.Count + 1);
                hotelak[zatia] = new Hotelak(
                    CeldaTestua(row.Cell(2)),
                    CeldaTestua(row.Cell(3)),
                    CeldaTestua(row.Cell(4))
                );
                egunKop[zatia] = Math.Max(1, CeldaEnteroa(row.Cell(5), 1));
            }

            foreach (var row in egunakSheet.RowsUsed().Skip(1))
            {
                int zatia = CeldaEnteroa(row.Cell(1), 1);
                if (!egunak.ContainsKey(zatia))
                {
                    egunak[zatia] = new List<EgunLaburpena>();
                }

                egunak[zatia].Add(new EgunLaburpena(
                    CeldaEnteroa(row.Cell(3), egunak[zatia].Count + 1),
                    CeldaTestua(row.Cell(4)),
                    CeldaTestua(row.Cell(5)),
                    CeldaTestua(row.Cell(6)),
                    CeldaTestua(row.Cell(7))
                ));
            }

            foreach (var row in ekintzakSheet.RowsUsed().Skip(1))
            {
                int zatia = CeldaEnteroa(row.Cell(1), 1);
                if (!ekintzak.ContainsKey(zatia))
                {
                    ekintzak[zatia] = new List<EkintzaDatuak>();
                }

                ekintzak[zatia].Add(new EkintzaDatuak(
                    CeldaTestua(row.Cell(3)),
                    CeldaTestua(row.Cell(4)),
                    CeldaTestua(row.Cell(5)),
                    CeldaTestua(row.Cell(6))
                ));
            }

            return hotelak
                .OrderBy(h => h.Key)
                .Select(h => new Bidaiak(
                    h.Value,
                    egunKop.TryGetValue(h.Key, out int kop) ? kop : Math.Max(1, egunak.GetValueOrDefault(h.Key)?.Count ?? 1),
                    egunak.GetValueOrDefault(h.Key) ?? new List<EgunLaburpena>(),
                    ekintzak.GetValueOrDefault(h.Key) ?? new List<EkintzaDatuak>()
                ))
                .ToList();
        }

        private static HotelDatuak HotelaDatuakSortu(Bidaiak bidaia)
        {
            Hotelak? hotela = bidaia.HotelHautatua;
            return new HotelDatuak(
                hotela?.Hiria ?? "",
                hotela?.Izena ?? "",
                hotela?.HelbideaUrl ?? "",
                bidaia.EgunKop,
                bidaia.EgunLaburpenak.FirstOrDefault()?.Data ?? ""
            );
        }

        private static PlangintzaDatuak EgituraZaharraIrakurri(IXLWorksheet sheet)
        {
            IXLRange? range = sheet.RangeUsed();
            PlangintzaDatuak datuak = new PlangintzaDatuak();

            if (range == null)
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
                CeldaTestua(row.Cell(6))
            ));
            string ordua = CeldaTestua(row.Cell(7));
            string mota = CeldaTestua(row.Cell(8));
            string deskribapena = CeldaTestua(row.Cell(9));
            if (!string.IsNullOrWhiteSpace(ordua)
                || !string.IsNullOrWhiteSpace(mota)
                || !string.IsNullOrWhiteSpace(deskribapena))
            {
                datuak.Ekintzak.Add(new EkintzaDatuak(
                    datuak.Data,
                    ordua,
                    mota,
                    deskribapena
                ));
            }
            datuak.Hotela = new HotelDatuak("", datuak.Ostala, "", datuak.EgunKop, datuak.Data);

            return datuak;
        }

        private static HotelDatuak HotelaIrakurri(IXLWorksheet sheet, int egunKop)
        {
            IXLRange? range = sheet.RangeUsed();
            if (range == null)
            {
                return new HotelDatuak("", "", "", egunKop, "");
            }

            IXLRangeRow? row = range.RowsUsed().Skip(1).FirstOrDefault();
            if (row == null)
            {
                return new HotelDatuak("", "", "", egunKop, "");
            }

            return new HotelDatuak(
                CeldaTestua(row.Cell(1)),
                CeldaTestua(row.Cell(2)),
                CeldaTestua(row.Cell(3)),
                CeldaEnteroa(row.Cell(4), egunKop),
                CeldaTestua(row.Cell(5))
            );
        }

        private static void LaburpenaIdatzi(IXLWorksheet sheet, string ostala, int egunKop)
        {
            sheet.Cell(1, 1).Value = "Ostala";
            sheet.Cell(1, 2).Value = ostala;
            sheet.Cell(2, 1).Value = "Egun kopurua";
            sheet.Cell(2, 2).Value = egunKop;
            sheet.Columns().AdjustToContents();
        }

        private static int CeldaEnteroa(IXLCell cell, int lehenetsia)
        {
            if (cell.TryGetValue(out int balioa))
            {
                return balioa;
            }

            string testua = CeldaTestua(cell);
            return int.TryParse(testua, out balioa) ? balioa : lehenetsia;
        }

        private static string CeldaTestua(IXLCell cell)
        {
            return cell.GetFormattedString() ?? "";
        }

        private static bool GoiburuaDa(string balioa)
        {
            balioa = (balioa ?? "").Trim();
            return string.Equals(balioa, "Ostala", StringComparison.OrdinalIgnoreCase)
                || string.Equals(balioa, "Hotela", StringComparison.OrdinalIgnoreCase)
                || string.Equals(balioa, "Hotel izena", StringComparison.OrdinalIgnoreCase);
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

        private static void EgunakIdatzi(IXLWorksheet sheet, List<EgunLaburpena> egunak)
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

        private static void EkintzakIdatzi(IXLWorksheet sheet, List<EkintzaDatuak> ekintzak)
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
    }
}
