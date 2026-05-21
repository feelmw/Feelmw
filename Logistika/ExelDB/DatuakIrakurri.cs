using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using ClosedXML.Excel;
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

            string range = "Ostalak!A:L";
            Baimenak.Autentikazioa();

            var request = Baimenak.service.Spreadsheets.Values.Get(DatuBaseID, range);
            ValueRange response = request.Execute();

            IList<IList<object>> values = response.Values;

            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (row == null || row.Count == 0)
                    {
                        continue; // Salta filas vacías
                    }
                    string ostala = row.Count > 0 ? row[0].ToString() ?? "" : "";
                    string bonoa = row.Count > 1 ? row[1].ToString() ?? "" : "";
                    string helbidea = row.Count > 2 ? row[2].ToString() ?? "" : "";
                    string checin = row.Count > 3 ? row[3].ToString() ?? "" : "";
                    string checkout = row.Count > 4 ? row[4].ToString() ?? "" : "";
                    string doku = row.Count > 5 ? row[5].ToString() ?? "" : "";
                    string luga = row.Count > 6 ? row[6].ToString() ?? "" : "";
                    string lugapre = row.Count > 7 ? row[7].ToString() ?? "" : "";
                    string harreta = row.Count > 8 ? row[8].ToString() ?? "" : "";
                    string toailak = row.Count > 9 ? row[9].ToString() ?? "" : "";
                    string izarak = row.Count > 10 ? row[10].ToString() ?? "" : "";
                    string instalazioak = row.Count > 11 ? row[11].ToString() ?? "" : "";

                    osta = new Ostalak(ostala, bonoa, helbidea, checin, checkout, doku, harreta, Parseatu(toailak), Parseatu(izarak), Parseatu(luga), lugapre, instalazioak);
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
            string range = "Ostalak!A:L";

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
                ost.Instalazioak
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
                    if (row == null || row.Count == 0)
                    {
                        continue; // Salta filas vacías
                    }
                    string ekintza = row.Count > 0 ? row[0].ToString() ?? "" : "";
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
            string ruta = FMenuNagusia.IdEditatuDokumentua;

            if (File.Exists(ruta))
            {
                using (var workbook = new XLWorkbook(ruta))
                {
                    if (workbook.Worksheets.TryGetWorksheet("Ostalak", out IXLWorksheet hoja))
                    {
                        var rango = hoja.RangeUsed();
                        if (rango != null)
                        {
                            foreach (var row in rango.Rows())
                            {
                                string ostala = row.Cell(1).GetValue<string>();
                                string bonoa = row.Cell(2).GetValue<string>();
                                string helbidea = row.Cell(3).GetValue<string>();
                                string lokali = row.Cell(4).GetValue<string>();
                                string gauak = row.Cell(5).GetValue<string>();
                                string data = row.Cell(6).GetValue<string>();
                                string gelak = row.Cell(7).GetValue<string>();
                                string checin = row.Cell(8).GetValue<string>();
                                string checkout = row.Cell(9).GetValue<string>();
                                string doku = row.Cell(10).GetValue<string>();
                                string harreta = row.Cell(11).GetValue<string>();
                                string gosaria = row.Cell(12).GetValue<string>();
                                string bazkaria = row.Cell(13).GetValue<string>();
                                string afaria = row.Cell(14).GetValue<string>();
                                bool esKlasikoa = string.IsNullOrWhiteSpace(lokali)
                                    && string.IsNullOrWhiteSpace(gauak)
                                    && string.IsNullOrWhiteSpace(data)
                                    && string.IsNullOrWhiteSpace(gelak);

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
                                   Parseatu(row.Cell(15).GetValue<string>()),
                                   Parseatu(row.Cell(16).GetValue<string>()),
                                   Parseatu(row.Cell(17).GetValue<string>()),
                                   row.Cell(18).GetValue<string>(),
                                   Parseatu(row.Cell(19).GetValue<string>()),
                                   row.Cell(20).GetValue<string>(),
                                   row.Cell(21).GetValue<string>(),
                                   esKlasikoa
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
                    if (workbook.Worksheets.TryGetWorksheet("Ekintzak", out IXLWorksheet hoja))
                    {
                        var rango = hoja.RangeUsed();
                        if (rango != null)
                        {
                            foreach (var row in rango.Rows())
                            {
                                string ekintza = row.Cell(1).GetValue<string>();
                            string bonoa = row.Cell(2).GetValue<string>();
                            string iraupena = row.Cell(3).GetValue<string>();
                            string kontaktua = row.Cell(4).GetValue<string>();
                            string elkartokia = row.Cell(5).GetValue<string>();
                            string iristean = row.Cell(6).GetValue<string>();
                            string eramanM = row.Cell(7).GetValue<string>();
                            string bertanM = row.Cell(8).GetValue<string>();
                            string aldagela = row.Cell(9).GetValue<string>();
                            string komuna = row.Cell(10).GetValue<string>();
                            string egonlekua = row.Cell(11).GetValue<string>();
                            string informazioa = row.Cell(12).GetValue<string>();

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
                                informazioa
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
                    if (workbook.Worksheets.TryGetWorksheet("Garraioak", out IXLWorksheet hoja))
                    {
                        var rango = hoja.RangeUsed();
                        if (rango != null)
                        {
                            foreach (var row in rango.Rows())
                            {
                                string garraioa = row.Cell(1).GetValue<string>();
                            string eguna = row.Cell(2).GetValue<string>();
                            string ordutegia = row.Cell(3).GetValue<string>();
                            string lokali = row.Cell(4).GetValue<string>();
                            string kontaktua = row.Cell(5).GetValue<string>();
                            string elkartokia = row.Cell(6).GetValue<string>();
                            string eginbeharrak = row.Cell(7).GetValue<string>();
                            string info = row.Cell(8).GetValue<string>();

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
