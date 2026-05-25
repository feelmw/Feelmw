using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FeelmwLogistika.Plangintza.DatuModeloak;
using System.Text.RegularExpressions;
using WordText = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace FeelmwLogistika.Plangintza.ExelDB
{
    public static class DokumentuaSortu
    {
        public class DokumentuEmaitza
        {
            public string Ruta { get; set; } = "";
            public List<string> Abisuak { get; } = new List<string>();
        }

        private class EgunDokumentua
        {
            public ExelaSortu.EgunLaburpena Eguna { get; set; } = null!;
            public List<ExelaSortu.EkintzaDatuak> Ekintzak { get; set; } = new List<ExelaSortu.EkintzaDatuak>();
        }

        public static DokumentuEmaitza Sortu(List<Bidaiak> bidaiak, string izena, string ikastetxea)
        {
            if (bidaiak == null || bidaiak.Count == 0)
            {
                throw new InvalidOperationException("Ez dago Plangintzako daturik dokumentua sortzeko.");
            }

            string plantillaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plangintza.docx");
            if (!File.Exists(plantillaPath))
            {
                throw new FileNotFoundException("Ez da aurkitu Word txantiloia: Plangintza.docx", plantillaPath);
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputPath = Path.Combine(desktopPath, $"Plangintza - {izena}.docx");
            File.Copy(plantillaPath, outputPath, true);

            using WordprocessingDocument doc = WordprocessingDocument.Open(outputPath, true);
            MainDocumentPart? mainPart = doc.MainDocumentPart;
            Body? body = mainPart?.Document.Body;
            if (mainPart == null || body == null)
            {
                throw new InvalidOperationException("Word txantiloiak ez dauka dokumentuaren gorputza erabilgarri.");
            }

            List<ExelaSortu.HotelDatuak> hotelak = bidaiak
                .Select(b => HotelaSortu(b))
                .Where(h => !string.IsNullOrWhiteSpace(h.Izena) || !string.IsNullOrWhiteSpace(h.Hiria))
                .ToList();
            List<ExelaSortu.EgunLaburpena> egunak = EgunakOrdenatu(bidaiak.SelectMany(b => b.EgunLaburpenak)).ToList();
            List<ExelaSortu.EkintzaDatuak> ekintzak = bidaiak.SelectMany(b => b.EkintzaDatuak).ToList();
            DokumentuEmaitza emaitza = new DokumentuEmaitza { Ruta = outputPath };
            List<EgunDokumentua> egunDokumentuak = EgunDokumentuakSortu(egunak, ekintzak, emaitza.Abisuak);

            GehituAbisuaGehiegiBada(body, emaitza.Abisuak, "{{OSTATUA_IZENA_HYPERLINK}}", hotelak.Count, "ostatu");
            GehituAbisuaGehiegiBada(body, emaitza.Abisuak, "{{EGUNA_DATA}}", egunak.Count, "egun");

            Ordezkatu(body, "{{IKASTETXEA}}", new[] { ikastetxea });
            Ordezkatu(body, "{{HIRIA}}", hotelak.Select(h => h.Hiria).ToList());
            OrdezkatuHyperlink(mainPart, body, "{{OSTATUA_IZENA_HYPERLINK}}", hotelak);
            Ordezkatu(body, "{{EGUNA_DATA}}", egunak.Select(e => e.Data).ToList());
            Ordezkatu(body, "{{EGUNA_GOIZA}}", egunak.Select(e => e.Goisa).ToList());
            Ordezkatu(body, "{{EGUNA_ARRATSALDEA}}", egunak.Select(e => e.Arratsaldea).ToList());
            Ordezkatu(body, "{{EGUNA_GAUA}}", egunak.Select(e => e.Gaua).ToList());
            EgunXehetasunakOrdezkatu(body, egunDokumentuak, emaitza.Abisuak);
            GarbituMarkagailuSobratuak(body);

            mainPart.Document.Save();
            return emaitza;
        }

        private static ExelaSortu.HotelDatuak HotelaSortu(Bidaiak bidaia)
        {
            Hotelak? hotela = bidaia.HotelHautatua;
            return new ExelaSortu.HotelDatuak(
                hotela?.Hiria ?? "",
                hotela?.Izena ?? "",
                hotela?.HelbideaUrl ?? "",
                bidaia.EgunKop,
                bidaia.EgunLaburpenak.FirstOrDefault()?.Data ?? ""
            );
        }

        private static IEnumerable<ExelaSortu.EgunLaburpena> EgunakOrdenatu(IEnumerable<ExelaSortu.EgunLaburpena> egunak)
        {
            return egunak
                .Select((eguna, index) => new { Eguna = eguna, Index = index, Data = DataOrdenatzeko(eguna.Data) })
                .OrderBy(item => item.Data ?? DateTime.MaxValue)
                .ThenBy(item => item.Index)
                .Select(item => item.Eguna);
        }

        private static List<EgunDokumentua> EgunDokumentuakSortu(
            IReadOnlyList<ExelaSortu.EgunLaburpena> egunak,
            IReadOnlyList<ExelaSortu.EkintzaDatuak> ekintzak,
            List<string> abisuak)
        {
            Dictionary<string, EgunDokumentua> egunakMap = new Dictionary<string, EgunDokumentua>();
            List<EgunDokumentua> emaitza = new List<EgunDokumentua>();

            for (int i = 0; i < egunak.Count; i++)
            {
                EgunDokumentua egunDokumentua = new EgunDokumentua
                {
                    Eguna = egunak[i]
                };
                emaitza.Add(egunDokumentua);
                string gakoa = EgunaGakoa(egunak[i].Data);
                if (!string.IsNullOrWhiteSpace(gakoa) && !egunakMap.ContainsKey(gakoa))
                {
                    egunakMap.Add(gakoa, egunDokumentua);
                }

                string zenbakiGakoa = EgunaGakoa($"{egunak[i].Eguna}. eguna");
                if (!egunakMap.ContainsKey(zenbakiGakoa))
                {
                    egunakMap.Add(zenbakiGakoa, egunDokumentua);
                }
            }

            foreach (ExelaSortu.EkintzaDatuak ekintza in ekintzak)
            {
                string gakoa = EgunaGakoa(ekintza.Eguna);
                if (string.IsNullOrWhiteSpace(gakoa) || !egunakMap.TryGetValue(gakoa, out EgunDokumentua? eguna))
                {
                    abisuak.Add($"Ekintza batek ez dauka egun baliodunik eta ez da dokumentuan sartu: {ekintza.Deskribapena}");
                    continue;
                }

                eguna.Ekintzak.Add(ekintza);
            }

            foreach (EgunDokumentua eguna in emaitza)
            {
                eguna.Ekintzak = eguna.Ekintzak
                    .OrderBy(e => OrduaOrdenatzeko(e.Ordua) ?? TimeSpan.MaxValue)
                    .ThenBy(e => e.Deskribapena)
                    .ToList();
            }

            return emaitza;
        }

        private static string EgunaGakoa(string eguna)
        {
            return (eguna ?? "").Trim().ToUpperInvariant();
        }

        private static DateTime? DataOrdenatzeko(string data)
        {
            if (DateTime.TryParse(data, out DateTime parsed))
            {
                return parsed.Date;
            }

            Match match = Regex.Match(data ?? "", @"(?<hilabetea>\D+?)\s*(?<eguna>\d{1,2})", RegexOptions.IgnoreCase);
            if (!match.Success || !int.TryParse(match.Groups["eguna"].Value, out int eguna))
            {
                return null;
            }

            Dictionary<string, int> hilabeteak = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["urtarrilak"] = 1,
                ["otsailak"] = 2,
                ["martxoak"] = 3,
                ["apirilak"] = 4,
                ["maiatzak"] = 5,
                ["ekainak"] = 6,
                ["uztailak"] = 7,
                ["abuztuak"] = 8,
                ["irailak"] = 9,
                ["urriak"] = 10,
                ["azaroak"] = 11,
                ["abenduak"] = 12
            };

            string hilabetea = match.Groups["hilabetea"].Value.Trim();
            if (!hilabeteak.TryGetValue(hilabetea, out int hilabeteZenbakia))
            {
                return null;
            }

            if (eguna < 1 || eguna > DateTime.DaysInMonth(DateTime.Today.Year, hilabeteZenbakia))
            {
                return null;
            }

            return new DateTime(DateTime.Today.Year, hilabeteZenbakia, eguna);
        }

        private static TimeSpan? OrduaOrdenatzeko(string ordua)
        {
            if (TimeSpan.TryParse(ordua, out TimeSpan time))
            {
                return time;
            }

            return DateTime.TryParse(ordua, out DateTime dateTime) ? dateTime.TimeOfDay : null;
        }

        private static void Ordezkatu(Body body, string marka, IReadOnlyList<string> balioak)
        {
            int index = 0;
            foreach (Paragraph paragraph in body.Descendants<Paragraph>().ToList())
            {
                string testua = ParagrafoTestua(paragraph);
                if (!testua.Contains(marka))
                {
                    continue;
                }

                while (testua.Contains(marka))
                {
                    string balioa = index < balioak.Count ? balioak[index] ?? "" : "";
                    testua = LehenengoAldizOrdezkatu(testua, marka, balioa);
                    index++;
                }

                ParagrafoTestuaEzarri(paragraph, testua);
            }
        }

        private static void EgunXehetasunakOrdezkatu(Body body, IReadOnlyList<EgunDokumentua> egunak, List<string> abisuak)
        {
            List<Paragraph> paragraphs = body.Descendants<Paragraph>().ToList();
            List<int> hasierak = paragraphs
                .Select((paragraph, index) => new { paragraph, index })
                .Where(item => ParagrafoTestua(item.paragraph).Contains("{{EGUNA_IZENBURUA}}"))
                .Select(item => item.index)
                .ToList();

            if (egunak.Count > hasierak.Count)
            {
                abisuak.Add($"Txantiloiak {hasierak.Count} egun xehetasun bloke ditu, baina {egunak.Count} egun daude. Soberako egunak ez dira dokumentuan sartuko.");
            }

            for (int i = 0; i < hasierak.Count; i++)
            {
                int hasiera = hasierak[i];
                int amaiera = i + 1 < hasierak.Count ? hasierak[i + 1] : paragraphs.Count;
                List<Paragraph> blokea = paragraphs.Skip(hasiera).Take(amaiera - hasiera).ToList();

                if (i >= egunak.Count)
                {
                    OrdezkatuParagrafoetan(blokea, "{{EGUNA_IZENBURUA}}", Array.Empty<string>());
                    OrdezkatuParagrafoetan(blokea, "{{EGUNA_ORDUA}}", Array.Empty<string>());
                    OrdezkatuParagrafoetan(blokea, "{{EGUNA_DESKRIBAPENA}}", Array.Empty<string>());
                    continue;
                }

                EgunDokumentua eguna = egunak[i];
                int orduLekuak = MarkaKopurua(blokea, "{{EGUNA_ORDUA}}");
                int deskribapenLekuak = MarkaKopurua(blokea, "{{EGUNA_DESKRIBAPENA}}");
                int lekuak = Math.Min(orduLekuak, deskribapenLekuak);

                if (eguna.Ekintzak.Count > lekuak)
                {
                    abisuak.Add($"{eguna.Eguna.Data} egunean {lekuak} ekintza leku daude, baina {eguna.Ekintzak.Count} ekintza. Soberakoak ez dira dokumentuan sartuko.");
                }

                List<ExelaSortu.EkintzaDatuak> ekintzak = eguna.Ekintzak.Take(lekuak).ToList();
                OrdezkatuParagrafoetan(blokea, "{{EGUNA_IZENBURUA}}", new[] { eguna.Eguna.Data });
                OrdezkatuParagrafoetan(blokea, "{{EGUNA_ORDUA}}", ekintzak.Select(e => e.Ordua).ToList());
                OrdezkatuParagrafoetan(blokea, "{{EGUNA_DESKRIBAPENA}}", ekintzak.Select(e => e.Deskribapena).ToList());
            }
        }

        private static void OrdezkatuParagrafoetan(IReadOnlyList<Paragraph> paragraphs, string marka, IReadOnlyList<string> balioak)
        {
            int index = 0;
            foreach (Paragraph paragraph in paragraphs)
            {
                string testua = ParagrafoTestua(paragraph);
                if (!testua.Contains(marka))
                {
                    continue;
                }

                while (testua.Contains(marka))
                {
                    string balioa = index < balioak.Count ? balioak[index] ?? "" : "";
                    testua = LehenengoAldizOrdezkatu(testua, marka, balioa);
                    index++;
                }

                ParagrafoTestuaEzarri(paragraph, testua);
            }
        }

        private static int MarkaKopurua(IReadOnlyList<Paragraph> paragraphs, string marka)
        {
            return paragraphs.Sum(paragraph => AgerraldiKopurua(ParagrafoTestua(paragraph), marka));
        }

        private static void GehituAbisuaGehiegiBada(Body body, List<string> abisuak, string marka, int datuKop, string izena)
        {
            int markaKop = body.Descendants<Paragraph>().Sum(paragraph => AgerraldiKopurua(ParagrafoTestua(paragraph), marka));
            if (datuKop > markaKop)
            {
                abisuak.Add($"Txantiloiak {markaKop} {izena} leku ditu, baina {datuKop} datu daude. Soberakoak ez dira dokumentuan sartuko.");
            }
        }

        private static void OrdezkatuHyperlink(MainDocumentPart mainPart, Body body, string marka, IReadOnlyList<ExelaSortu.HotelDatuak> hotelak)
        {
            int index = 0;
            foreach (Paragraph paragraph in body.Descendants<Paragraph>().ToList())
            {
                string testua = ParagrafoTestua(paragraph);
                if (!testua.Contains(marka))
                {
                    continue;
                }

                ExelaSortu.HotelDatuak? hotela = index < hotelak.Count ? hotelak[index] : null;
                string izena = hotela?.Izena ?? "";
                string url = hotela?.HelbideaUrl ?? "";
                index++;

                if (testua.Trim() != marka || string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
                {
                    ParagrafoTestuaEzarri(paragraph, testua.Replace(marka, izena));
                    continue;
                }

                HyperlinkRelationship relation = mainPart.AddHyperlinkRelationship(uri, true);
                paragraph.RemoveAllChildren<Run>();
                Run linkRun = new Run(
                    new RunProperties(new RunStyle { Val = "Hyperlink" }),
                    new WordText(izena));

                Hyperlink hyperlink = new Hyperlink(linkRun)
                {
                    Id = relation.Id,
                    History = true
                };

                paragraph.AppendChild(hyperlink);
            }
        }

        private static void GarbituMarkagailuSobratuak(Body body)
        {
            Regex regex = new Regex(@"\{\{[^}]+\}\}");
            foreach (Paragraph paragraph in body.Descendants<Paragraph>())
            {
                string testua = ParagrafoTestua(paragraph);
                if (testua.Contains("{{"))
                {
                    ParagrafoTestuaEzarri(paragraph, regex.Replace(testua, ""));
                }
            }
        }

        private static string ParagrafoTestua(Paragraph paragraph)
        {
            return string.Concat(paragraph.Descendants<WordText>().Select(text => text.Text));
        }

        private static void ParagrafoTestuaEzarri(Paragraph paragraph, string testua)
        {
            List<WordText> zatiak = paragraph.Descendants<WordText>().ToList();
            if (zatiak.Count == 0)
            {
                paragraph.AppendChild(new Run(new WordText(testua)));
                return;
            }

            zatiak[0].Text = testua;
            zatiak[0].Space = SpaceProcessingModeValues.Preserve;
            foreach (WordText zatia in zatiak.Skip(1))
            {
                zatia.Text = "";
            }
        }

        private static string LehenengoAldizOrdezkatu(string testua, string bilatu, string ordezkoa)
        {
            int index = testua.IndexOf(bilatu, StringComparison.Ordinal);
            return index < 0
                ? testua
                : testua.Substring(0, index) + ordezkoa + testua.Substring(index + bilatu.Length);
        }

        private static int AgerraldiKopurua(string testua, string bilatu)
        {
            int count = 0;
            int index = 0;
            while ((index = testua.IndexOf(bilatu, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += bilatu.Length;
            }

            return count;
        }
    }
}
