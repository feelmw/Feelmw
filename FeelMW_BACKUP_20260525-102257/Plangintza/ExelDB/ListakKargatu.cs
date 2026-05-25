using FeelmwLogistika.Logistika.DatuModeloak;
using FeelmwLogistika.Logistika.ExelDB;
using FeelmwLogistika.Plangintza.DatuModeloak;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace FeelmwLogistika.Plangintza.ExelDB
{
    public class ListakKargatu
    {
        private static string DatuBaseID = "1KggU6fnTW5lOldbP6Vp8PJJO2t7RE9hJAnurnunTzgg";

        public static List<Hotelak> OstalakListaratu()
        {
            List<Hotelak> LisOst = new List<Hotelak>();

            Baimenak.Autentikazioa();

            var spreadsheetRequest = Baimenak.service.Spreadsheets.Get(DatuBaseID);

            spreadsheetRequest.Ranges = new List<string>
            {
                "Ostalak!A:C"
            };

            spreadsheetRequest.IncludeGridData = true;

            var spreadsheet = spreadsheetRequest.Execute();

            if (spreadsheet.Sheets == null || spreadsheet.Sheets.Count == 0
                || spreadsheet.Sheets[0].Data == null || spreadsheet.Sheets[0].Data.Count == 0)
            {
                return LisOst;
            }

            var sheet = spreadsheet.Sheets[0];

            var rows = sheet.Data[0].RowData;

            if (rows != null && rows.Count > 0)
            {
                foreach (var row in rows)
                {
                    if (row.Values == null || row.Values.Count == 0)
                        continue;

                    var cells = row.Values;

                    string hiria =
                        cells.Count > 0
                        ? cells[0].FormattedValue ?? ""
                        : "";

                    string ostala =
                        cells.Count > 1
                        ? cells[1].FormattedValue ?? ""
                        : "";

                    if (string.IsNullOrWhiteSpace(ostala) || GoiburuaDa(ostala, "ostala", "hotela", "hotel izena", "izena"))
                        continue;

                    string url = "";

                    if (cells.Count > 2)
                    {
                        var cell = cells[2];

                        url = cell.Hyperlink
                              ?? cell.FormattedValue
                              ?? "";
                    }

                    Hotelak osta = new Hotelak(hiria, ostala, url);

                    LisOst.Add(osta);
                }
            }
            else
            {
                MessageBox.Show("Ez dago daturik 😶");
            }

            return LisOst;
        }

        public static List<EkintzakPlan> EkintzakListaratu()
        {
            List<EkintzakPlan> LisEkintzak = new List<EkintzakPlan>();
            EkintzakPlan ekintza;

            string range = "Ekintzak!A:L";
            Baimenak.Autentikazioa();

            var request = Baimenak.service.Spreadsheets.Values.Get(DatuBaseID, range);
            ValueRange response = request.Execute();

            IList<IList<object>> values = response.Values;

            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (RowHutsa(row) || GoiburuaDa(RowBalioa(row, 0), "ekintza", "mota", "izena"))
                        continue;
                    var cells = row;
                    string mota = RowBalioa(cells, 0);
                    if (string.IsNullOrWhiteSpace(mota))
                        continue;

                    string deskripzioa = RowBalioa(cells, 1);
                    ekintza= new EkintzakPlan(mota, deskripzioa);
                    LisEkintzak.Add(ekintza);
                }
            }
            else
            {
                MessageBox.Show("Ez dago daturik 😶");
            }
            return LisEkintzak;
        }

        public static void EkintzaGehitu(EkintzakPlan ekintza)
        {
            string range = "Ekintzak!A:B";
            Baimenak.Autentikazioa();

            ValueRange valueRange = new ValueRange
            {
                Values = new List<IList<object>>
                {
                    new List<object>
                    {
                        ekintza.Mota,
                        ekintza.Deskribapena
                    }
                }
            };

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

        private static bool RowHutsa(params string[] balioak)
        {
            return balioak.All(string.IsNullOrWhiteSpace);
        }

        private static bool GoiburuaDa(string lehenZutabea, params string[] aukerak)
        {
            string balioa = (lehenZutabea ?? "").Trim();
            return aukerak.Any(a => string.Equals(balioa, a, StringComparison.OrdinalIgnoreCase));
        }
    }
}
