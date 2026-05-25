using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using ClosedXML.Excel;
using FeelmwLogistika;
using FeelmwLogistika.Logistika.DatuModeloak;

namespace FeelmwLogistika.Logistika.ExelDB
{
    public class DatuakIrakurri
    {
        // Datu base bezela erabiltzean exelan Id-a

        private static string DatuBaseID = "1Ad_LvrFsS9-GTxjQy8yAJMGs2a-jyqXQr04LOdpRZt8";

        public static List<SheetInfo> SheetsListaratu()
        {
            List<SheetInfo> lista = new List<SheetInfo>();

            // Ruta de la carpeta bin
            string carpeta = AppDomain.CurrentDomain.BaseDirectory;

            // Buscar todos los .xlsx
            string[] archivos = Directory.GetFiles(carpeta, "*.xlsx");

            foreach (string archivo in archivos)
            {
                lista.Add(new SheetInfo
                {
                    Id = archivo, // guardamos la ruta completa
                    Nombre = Path.GetFileNameWithoutExtension(archivo)
                });
            }

            return lista;
        }

        public static List<Ostalak> OstalakListaratu()
        {
            List<Ostalak> LisOst = new List<Ostalak>();
            Ostalak osta;

            // Drive eskema: A:Q (17 zutabe). Excel lokalak 21 zutabe erabiltzen ditu.
            string range = "Ostalak!A:Q";
            Baimenak.Autentikazioa();

            var request = Baimenak.service.Spreadsheets.Values.Get(DatuBaseID, range);
            ValueRange response = request.Execute();

            IList<IList<object>> values = response.Values;

            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (RowHutsa(row) || GoiburuaDa(RowBalioa(row, 0), "ostala", "izena", "hotela"))
                    {
                        continue;
                    }
                    string ostala = RowBalioa(row, 0);
                    if (string.IsNullOrWhiteSpace(ostala))
                    {
                        continue;
                    }

                    string bonoa = RowBalioa(row, 1);
                    string helbidea = RowBalioa(row, 2);
                    string checin = RowBalioa(row, 3);
                    string checkout = RowBalioa(row, 4);
                    string doku = RowBalioa(row, 5);
                    string luga = RowBalioa(row, 6);
                    string lugapre = RowBalioa(row, 7);
                    string harreta = RowBalioa(row, 8);
                    string toailak = RowBalioa(row, 9);
                    string izarak = RowBalioa(row, 10);
                    string instalazioak = RowBalioa(row, 11);
                    string gosaria = RowBalioa(row, 12);
                    string bazkaria = RowBalioa(row, 13);
                    string afaria = RowBalioa(row, 14);
                    string fidantza = RowBalioa(row, 15);
                    string fidantzaKuota = RowBalioa(row, 16);

                    osta = new Ostalak(
                        ostala,
                        bonoa,
                        helbidea,
                        BalioLehenetsiak.Lokalizatzailea,
                        BalioLehenetsiak.Gauak,
                        "",
                        "",
                        checin,
                        checkout,
                        doku,
                        harreta,
                        gosaria,
                        bazkaria,
                        afaria,
                        Parseatu(toailak),
                        Parseatu(izarak),
                        Parseatu(fidantza),
                        fidantzaKuota,
                        Parseatu(luga),
                        lugapre,
                        instalazioak
                    );
                    LisOst.Add(osta);
                }
            }
            else
            {
                MessageBox.Show("Ez dago daturik 😶");
            }

            return LisOst;
        }

        public static void OstalaGehitu(Ostalak ost)
        {
            // Drive eskema: A:Q (17 zutabe). Excel lokalak 21 zutabe erabiltzen ditu.
            string range = "Ostalak!A:Q";

            Baimenak.Autentikazioa();

            var valueRange = new ValueRange();

            var objectList = new List<object>()
            {
                ost.OstalaIzena,
                ost.Bonoa,
                ost.Helbidea,
                ost.Checkin,
                ost.Checkout,
                ost.Dokumentazioa,
                BoolToBaiEz(ost.Luggage),
                ost.LuggageKuota,
                ost.Harrera,
                BoolToBaiEz(ost.Toailak),
                BoolToBaiEz(ost.Izarak),
                ost.Instalazioak,
                ost.Gosariates,
                ost.Bazkariates,
                ost.Afariates,
                BoolToBaiEz(ost.Fidantza),
                ost.FidantzaKuota
            };

            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = Baimenak.service.Spreadsheets.Values.Append(
                valueRange,
                DatuBaseID,
                range
            );

            appendRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            appendRequest.Execute();
        }

        public static List<Ekintzak> EkintzakListaratu()
        {
            List<Ekintzak> LisEki = new List<Ekintzak>();
            Ekintzak eki;
            string range = "Ekintzak!A:L";
            Baimenak.Autentikazioa();
            var request = Baimenak.service.Spreadsheets.Values.Get(DatuBaseID, range);
            ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (RowHutsa(row) || GoiburuaDa(RowBalioa(row, 0), "ekintza", "izena"))
                        continue;
                    string ekintza = row.Count > 0 ? row[0].ToString() ?? "" : "";
                    if (string.IsNullOrWhiteSpace(ekintza))
                        continue;

                    string bonoa = row.Count > 1 ? row[1].ToString() ?? "" : "";
                    string iraupena = row.Count > 2 ? row[2].ToString() ?? "" : "";
                    string kontaktua = row.Count > 3 ? row[3].ToString() ?? "" : "";
                    string elkartokia = row.Count > 4 ? row[4].ToString() ?? "" : "";
                    string iristean = row.Count > 5 ? row[5].ToString() ?? "" : "";
                    string eramanM = row.Count > 6 ? row[6].ToString() ?? "" : "";
                    string bertanM = row.Count > 7 ? row[7].ToString() ?? "" : "";
                    string aldagela = row.Count > 8 ? row[8].ToString() ?? "" : "";
                    string komuna = row.Count > 9 ? row[9].ToString() ?? "" : "";
                    string egonlekua = row.Count > 10 ? row[10].ToString() ?? "" : "";
                    string informazioa = row.Count > 11 ? row[11].ToString() ?? "" : "";
                    eki = new Ekintzak(ekintza, bonoa, iraupena, kontaktua, elkartokia, iristean, eramanM, bertanM, Parseatu(aldagela), Parseatu(komuna), egonlekua, informazioa);
                    LisEki.Add(eki);
                }
            }
            else
            {
                MessageBox.Show("Ez dago daturik 😶");
            }
            return LisEki;
        }

        public static void EkintzaGehitu(Ekintzak eki)
        {
            string range = "Ekintzak!A:L";
            Baimenak.Autentikazioa();
            var valueRange = new ValueRange();
            var objectList = new List<object>()
            {
                eki.EkintzaIzena,
                eki.Bonoa,
                eki.Iraupena,
                eki.Kontaktua,
                eki.Elkartokia,
                eki.Iristean,
                eki.EramanM,
                eki.BertanM,
                BoolToBaiEz(eki.Aldagela),
                BoolToBaiEz(eki.Komuna),
                eki.Egonlekua,
                eki.Informazioa
            };
            valueRange.Values = new List<IList<object>> { objectList };
            var appendRequest = Baimenak.service.Spreadsheets.Values.Append(
                valueRange,
                DatuBaseID,
                range
            );
            appendRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            appendRequest.Execute();
        }

        private static string RowBalioa(IList<object> row, int index)
        {
            return row.Count > index ? row[index].ToString() ?? "" : "";
        }

        private static bool RowHutsa(IList<object>? row)
        {
            return row == null || row.Count == 0 || row.All(c => string.IsNullOrWhiteSpace(c?.ToString()));
        }

        private static string Gelaxka(IXLRangeRow row, int zutabea)
        {
            return row.Cell(zutabea).GetFormattedString() ?? "";
        }

        private static bool ExcelRowHutsa(IXLRangeRow row, int zutabeKop)
        {
            for (int i = 1; i <= zutabeKop; i++)
            {
                if (!string.IsNullOrWhiteSpace(Gelaxka(row, i)))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool GoiburuaDa(string lehenZutabea, params string[] aukerak)
        {
            string balioa = (lehenZutabea ?? "").Trim();
            return aukerak.Any(a => string.Equals(balioa, a, StringComparison.OrdinalIgnoreCase));
        }

        private static bool OstalaFormatuBerriaDa(IXLRangeRow row)
        {
            if (int.TryParse(Gelaxka(row, 5), out _)
                || Gelaxka(row, 6).Contains(" - ")
                || string.Equals(Gelaxka(row, 4), BalioLehenetsiak.Lokalizatzailea, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            for (int zutabea = 17; zutabea <= 21; zutabea++)
            {
                if (!string.IsNullOrWhiteSpace(Gelaxka(row, zutabea)))
                {
                    return true;
                }
            }

            return false;
        }

        public static string BoolToBaiEz(bool balioa)
        {
            return balioa ? "Bai" : "Ez";
        }

        public static bool Parseatu(string balioa)
        {
            if (balioa != null)
            {
                if (balioa.Equals("Bai", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else if (balioa.Equals("Ez", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return false;
        }

        public static (List<Ostalak>, List<Ekintzak>, List<Garraioak>) DatuakKargatu()
        {
            List<Ostalak> LisOst = new List<Ostalak>();
            List<Ekintzak> LisEki = new List<Ekintzak>();
            List<Garraioak> LisGar = new List<Garraioak>();

            Ostalak Ost;
            Ekintzak Eki;
            Garraioak Gar;

            // =========================
            // 📌 OSTALAK
            // =========================
            string ruta = FMenuNagusia.IdEditatuDokumentua ?? "";

            if (File.Exists(ruta))
            {
                using (var workbook = new XLWorkbook(ruta))
                {
                    if (workbook.Worksheets.TryGetWorksheet("Ostalak", out var hoja))
                    {
                        var rango = hoja.RangeUsed();
                        if (rango != null)
                        {
                            foreach (var row in rango.Rows())
                            {
                                if (ExcelRowHutsa(row, 21) || GoiburuaDa(Gelaxka(row, 1), "ostala", "izena", "hotela"))
                                {
                                    continue;
                                }

                                bool formatuBerria = OstalaFormatuBerriaDa(row);
                                string ostala = Gelaxka(row, 1);
                                if (string.IsNullOrWhiteSpace(ostala))
                                {
                                    continue;
                                }

                                string bonoa = Gelaxka(row, 2);
                                string helbidea = Gelaxka(row, 3);
                                string lokali = formatuBerria ? Gelaxka(row, 4) : BalioLehenetsiak.Lokalizatzailea;
                                string gauak = formatuBerria ? Gelaxka(row, 5) : BalioLehenetsiak.Gauak.ToString();
                                string data = formatuBerria ? Gelaxka(row, 6) : "";
                                string gelak = formatuBerria ? Gelaxka(row, 7) : "";
                                string checin = formatuBerria ? Gelaxka(row, 8) : Gelaxka(row, 4);
                                string checkout = formatuBerria ? Gelaxka(row, 9) : Gelaxka(row, 5);
                                string doku = formatuBerria ? Gelaxka(row, 10) : Gelaxka(row, 6);
                                string harreta = formatuBerria ? Gelaxka(row, 11) : Gelaxka(row, 9);
                                string gosaria = formatuBerria ? Gelaxka(row, 12) : Gelaxka(row, 13);
                                string bazkaria = formatuBerria ? Gelaxka(row, 13) : Gelaxka(row, 14);
                                string afaria = formatuBerria ? Gelaxka(row, 14) : Gelaxka(row, 15);
                                string toailak = formatuBerria ? Gelaxka(row, 15) : Gelaxka(row, 10);
                                string izarak = formatuBerria ? Gelaxka(row, 16) : Gelaxka(row, 11);
                                string fidantza = formatuBerria ? Gelaxka(row, 17) : Gelaxka(row, 16);
                                string fidantzaKuota = formatuBerria ? Gelaxka(row, 18) : "";
                                string luggage = formatuBerria ? Gelaxka(row, 19) : Gelaxka(row, 7);
                                string luggageKuota = formatuBerria ? Gelaxka(row, 20) : Gelaxka(row, 8);
                                string instalazioak = formatuBerria ? Gelaxka(row, 21) : Gelaxka(row, 12);

                                Ost = new Ostalak(
                                   ostala,
                                   bonoa,
                                   helbidea,
                                   lokali,
                                   int.TryParse(gauak, out int gauakKop) ? gauakKop : 0,
                                   data,
                                   gelak,
                                   checin,
                                   checkout,
                                   doku,
                                   harreta,
                                   gosaria,
                                   bazkaria,
                                   afaria,
                                   Parseatu(toailak),
                                   Parseatu(izarak),
                                   Parseatu(fidantza),
                                   fidantzaKuota,
                                   Parseatu(luggage),
                                   luggageKuota,
                                   instalazioak
                               );

                                LisOst.Add(Ost);
                            }
                        }
                    }
                }
            }

            // ========================= 
            // 📌 EKINTZAK 
            // =========================
            if (File.Exists(ruta))
            {
                using (var workbook = new XLWorkbook(ruta))
                {
                    if (workbook.Worksheets.TryGetWorksheet("Ekintzak", out var hoja))
                    {
                        var rango = hoja.RangeUsed();
                        if (rango != null)
                        {
                            foreach (var row in rango.Rows())
                            {
                                if (ExcelRowHutsa(row, 13) || GoiburuaDa(Gelaxka(row, 1), "ekintza", "izena"))
                                {
                                    continue;
                                }

                                string ekintza = Gelaxka(row, 1);
                            if (string.IsNullOrWhiteSpace(ekintza))
                            {
                                continue;
                            }

                            string bonoa = Gelaxka(row, 2);
                            string iraupena = Gelaxka(row, 3);
                            string kontaktua = Gelaxka(row, 4);
                            string elkartokia = Gelaxka(row, 5);
                            string iristean = Gelaxka(row, 6);
                            string eramanM = Gelaxka(row, 7);
                            string bertanM = Gelaxka(row, 8);
                            string aldagela = Gelaxka(row, 9);
                            string komuna = Gelaxka(row, 10);
                            string egonlekua = Gelaxka(row, 11);
                            string informazioa = Gelaxka(row, 12);
                            string lokali = Gelaxka(row, 13);

                            Eki = new Ekintzak(
                                ekintza,
                                bonoa,
                                iraupena,
                                kontaktua,
                                elkartokia,
                                iristean,
                                eramanM,
                                bertanM,
                                Parseatu(aldagela),
                                Parseatu(komuna),
                                egonlekua,
                                informazioa,
                                lokali
                            );

                                LisEki.Add(Eki);
                            }
                        }
                    }
                }
            }
            // ========================= 
            // 📌 GARRAIOAK 
            // =========================
            if (File.Exists(ruta))
            {
                using (var workbook = new XLWorkbook(ruta))
                {
                    if (workbook.Worksheets.TryGetWorksheet("Garraioak", out var hoja))
                    {
                        var rango = hoja.RangeUsed();
                        if (rango != null)
                        {
                            foreach (var row in rango.Rows())
                            {
                                if (ExcelRowHutsa(row, 8) || GoiburuaDa(Gelaxka(row, 1), "garraioa", "izena"))
                                {
                                    continue;
                                }

                                string garraioa = Gelaxka(row, 1);
                            if (string.IsNullOrWhiteSpace(garraioa))
                            {
                                continue;
                            }

                            string eguna = Gelaxka(row, 2);
                            string ordutegia = Gelaxka(row, 3);
                            string lokali = Gelaxka(row, 4);
                            string kontaktua = Gelaxka(row, 5);
                            string elkartokia = Gelaxka(row, 6);
                            string eginbeharrak = Gelaxka(row, 7);
                            string info = Gelaxka(row, 8);

                            Gar = new Garraioak(
                                garraioa,
                                eguna,
                                ordutegia,
                                lokali,
                                kontaktua,
                                elkartokia,
                                eginbeharrak,
                                info
                            );

                                LisGar.Add(Gar);
                            }
                        }
                    }
                }
            }
            return (LisOst, LisEki, LisGar);
        }
    }
}
