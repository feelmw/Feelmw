using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FeelmwLogistika.Blazor.Models;
using WordText = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace FeelmwLogistika.Blazor.Infrastructure.Documents;

public sealed class PlangintzaDocumentService(HttpClient httpClient) : IPlangintzaDocumentService
{
    private sealed class EgunDokumentua
    {
        public EgunLaburpena Eguna { get; set; } = null!;
        public List<EkintzaDatuak> Ekintzak { get; set; } = [];
    }

    public async Task<DocumentGenerationResult> CreateDocumentFromTemplatePathAsync(
        string templatePath,
        IEnumerable<Bidaiak> bidaiak,
        string izena,
        string ikastetxea,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using MemoryStream template = await DocumentTemplateResolver.LoadTemplateAsync(httpClient, templatePath, cancellationToken);
            return CreateDocument(template, bidaiak, izena, ikastetxea);
        }
        catch (Exception ex)
        {
            return new DocumentGenerationResult { Errors = [$"Errorea dokumentua sortzean: {ex.Message}"] };
        }
    }

    public DocumentGenerationResult CreateDocument(Stream templateStream, IEnumerable<Bidaiak> bidaiak, string izena, string ikastetxea)
    {
        DocumentGenerationResult result = new()
        {
            FileName = $"Plangintza - {DocumentTemplateResolver.SafeFileName(izena, "Plangintza")}.docx"
        };

        try
        {
            List<Bidaiak> bidaiakList = bidaiak.ToList();
            if (bidaiakList.Count == 0)
            {
                result.Errors.Add("Ez dago Plangintzako daturik dokumentua sortzeko.");
                return result;
            }

            using MemoryStream output = new();
            templateStream.CopyTo(output);
            output.Position = 0;

            using (WordprocessingDocument doc = WordprocessingDocument.Open(output, true))
            {
                MainDocumentPart? mainPart = doc.MainDocumentPart;
                Body? body = mainPart?.Document.Body;
                if (mainPart is null || body is null)
                {
                    result.Errors.Add("Word txantiloiak ez dauka dokumentuaren gorputza erabilgarri.");
                    return result;
                }

                List<HotelDatuak> hotelak = bidaiakList
                    .Select(HotelaSortu)
                    .Where(h => !string.IsNullOrWhiteSpace(h.Izena) || !string.IsNullOrWhiteSpace(h.Hiria))
                    .ToList();
                List<EgunLaburpena> egunak = EgunakOrdenatu(bidaiakList.SelectMany(b => b.EgunLaburpenak)).ToList();
                List<EkintzaDatuak> ekintzak = bidaiakList.SelectMany(b => b.EkintzaDatuak).ToList();
                List<EgunDokumentua> egunDokumentuak = EgunDokumentuakSortu(egunak, ekintzak, result.Warnings);

                GehituAbisuaGehiegiBada(body, result.Warnings, "{{OSTATUA_IZENA_HYPERLINK}}", hotelak.Count, "ostatu");
                GehituAbisuaGehiegiBada(body, result.Warnings, "{{EGUNA_DATA}}", egunak.Count, "egun");

                Ordezkatu(body, "{{IKASTETXEA}}", [ikastetxea]);
                Ordezkatu(body, "{{HIRIA}}", hotelak.Select(h => h.Hiria).ToList());
                OrdezkatuHyperlink(mainPart, body, "{{OSTATUA_IZENA_HYPERLINK}}", hotelak);
                Ordezkatu(body, "{{EGUNA_DATA}}", egunak.Select(e => e.Data).ToList());
                Ordezkatu(body, "{{EGUNA_GOIZA}}", egunak.Select(e => e.Goisa).ToList());
                Ordezkatu(body, "{{EGUNA_ARRATSALDEA}}", egunak.Select(e => e.Arratsaldea).ToList());
                Ordezkatu(body, "{{EGUNA_GAUA}}", egunak.Select(e => e.Gaua).ToList());
                EgunXehetasunakOrdezkatu(body, egunDokumentuak, result.Warnings);
                EliminarFilasConMarkagailuakSobratuak(body);
                GarbituMarkagailuSobratuak(body);
                EliminarEgituraHutsak(body);

                mainPart.Document.Save();
            }

            result.Content = output.ToArray();
            return result;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Errorea dokumentua sortzean: {ex.Message}");
            result.Content = [];
            return result;
        }
    }

    private static HotelDatuak HotelaSortu(Bidaiak bidaia)
    {
        Hotelak? hotela = bidaia.HotelHautatua;
        return new HotelDatuak(
            hotela?.Hiria ?? "",
            hotela?.Izena ?? "",
            hotela?.HelbideaUrl ?? "",
            bidaia.EgunKop,
            bidaia.EgunLaburpenak.FirstOrDefault()?.Data ?? "");
    }

    private static IEnumerable<EgunLaburpena> EgunakOrdenatu(IEnumerable<EgunLaburpena> egunak)
    {
        return egunak
            .Select((eguna, index) => new { Eguna = eguna, Index = index, Data = DataOrdenatzeko(eguna.Data) })
            .OrderBy(item => item.Data ?? DateTime.MaxValue)
            .ThenBy(item => item.Index)
            .Select(item => item.Eguna);
    }

    private static List<EgunDokumentua> EgunDokumentuakSortu(IReadOnlyList<EgunLaburpena> egunak, IReadOnlyList<EkintzaDatuak> ekintzak, List<string> abisuak)
    {
        Dictionary<string, EgunDokumentua> egunakMap = [];
        List<EgunDokumentua> emaitza = [];

        for (int i = 0; i < egunak.Count; i++)
        {
            EgunDokumentua egunDokumentua = new() { Eguna = egunak[i] };
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

        foreach (EkintzaDatuak ekintza in ekintzak)
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
        List<EgunDokumentua> egunakEdukiDutenak = egunak.Where(e => e.Ekintzak.Count > 0).ToList();
        EzabatuEgunBlokeHutsak(body, egunak);

        List<Paragraph> paragraphs = body.Descendants<Paragraph>().ToList();
        List<int> hasierak = EgunBlokeHasierak(paragraphs);

        if (egunakEdukiDutenak.Count > hasierak.Count)
        {
            abisuak.Add($"Txantiloiak {hasierak.Count} egun xehetasun bloke ditu, baina {egunakEdukiDutenak.Count} egun daude. Soberako egunak ez dira dokumentuan sartuko.");
        }

        for (int i = 0; i < hasierak.Count; i++)
        {
            int hasiera = hasierak[i];
            int amaiera = i + 1 < hasierak.Count ? hasierak[i + 1] : paragraphs.Count;
            List<Paragraph> blokea = paragraphs.Skip(hasiera).Take(amaiera - hasiera).ToList();

            if (i >= egunakEdukiDutenak.Count)
            {
                EzabatuParagrafoBlokea(body, blokea);
                continue;
            }

            EgunDokumentua eguna = egunakEdukiDutenak[i];
            int lekuak = Math.Min(MarkaKopurua(blokea, "{{EGUNA_ORDUA}}"), MarkaKopurua(blokea, "{{EGUNA_DESKRIBAPENA}}"));

            if (eguna.Ekintzak.Count > lekuak)
            {
                abisuak.Add($"{eguna.Eguna.Data} egunean {lekuak} ekintza leku daude, baina {eguna.Ekintzak.Count} ekintza. Soberakoak ez dira dokumentuan sartuko.");
            }

            List<EkintzaDatuak> ekintzak = eguna.Ekintzak.Take(lekuak).ToList();
            OrdezkatuParagrafoetan(blokea, "{{EGUNA_IZENBURUA}}", [eguna.Eguna.Data]);
            OrdezkatuParagrafoetan(blokea, "{{EGUNA_ORDUA}}", ekintzak.Select(e => e.Ordua).ToList());
            OrdezkatuParagrafoetan(blokea, "{{EGUNA_DESKRIBAPENA}}", ekintzak.Select(e => e.Deskribapena).ToList());
        }
    }

    private static void OrdezkatuHyperlink(MainDocumentPart mainPart, Body body, string marka, IReadOnlyList<HotelDatuak> hotelak)
    {
        int index = 0;
        foreach (Paragraph paragraph in body.Descendants<Paragraph>().ToList())
        {
            string testua = ParagrafoTestua(paragraph);
            if (!testua.Contains(marka))
            {
                continue;
            }

            HotelDatuak? hotela = index < hotelak.Count ? hotelak[index] : null;
            string izena = hotela?.Izena ?? "";
            string url = hotela?.HelbideaUrl ?? "";
            index++;

            if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            {
                ParagrafoTestuaEzarri(paragraph, testua.Replace(marka, izena));
                continue;
            }

            HyperlinkRelationship relation = mainPart.AddHyperlinkRelationship(uri, true);
            string aurretik = testua[..testua.IndexOf(marka, StringComparison.Ordinal)];
            string ondoren = testua[(aurretik.Length + marka.Length)..];
            paragraph.RemoveAllChildren<Run>();
            paragraph.RemoveAllChildren<Hyperlink>();
            if (!string.IsNullOrEmpty(aurretik))
            {
                paragraph.AppendChild(new Run(new WordText(aurretik) { Space = SpaceProcessingModeValues.Preserve }));
            }

            Run linkRun = new(new RunProperties(new RunStyle { Val = "Hyperlink" }), new WordText(izena));
            paragraph.AppendChild(new Hyperlink(linkRun) { Id = relation.Id, History = true });

            if (!string.IsNullOrEmpty(ondoren))
            {
                paragraph.AppendChild(new Run(new WordText(ondoren) { Space = SpaceProcessingModeValues.Preserve }));
            }
        }
    }

    private static void EzabatuEgunBlokeHutsak(Body body, IReadOnlyList<EgunDokumentua> egunak)
    {
        List<Paragraph> paragraphs = body.Descendants<Paragraph>().ToList();
        List<int> hasierak = EgunBlokeHasierak(paragraphs);

        for (int i = Math.Min(egunak.Count, hasierak.Count) - 1; i >= 0; i--)
        {
            if (egunak[i].Ekintzak.Count > 0)
            {
                continue;
            }

            int hasiera = hasierak[i];
            int amaiera = i + 1 < hasierak.Count ? hasierak[i + 1] : paragraphs.Count;
            EzabatuParagrafoBlokea(body, paragraphs.Skip(hasiera).Take(amaiera - hasiera).ToList());
        }
    }

    private static void EliminarFilasConMarkagailuakSobratuak(Body body)
    {
        foreach (TableRow row in body.Descendants<TableRow>().ToList())
        {
            if (ParagrafoakTestua(row.Descendants<Paragraph>()).Contains("{{"))
            {
                row.Remove();
            }
        }
    }

    private static void GarbituMarkagailuSobratuak(Body body)
    {
        Regex regex = new(@"\{\{[^}]+\}\}");
        foreach (Paragraph paragraph in body.Descendants<Paragraph>())
        {
            string testua = ParagrafoTestua(paragraph);
            if (testua.Contains("{{"))
            {
                ParagrafoTestuaEzarri(paragraph, regex.Replace(testua, ""));
            }
        }
    }

    private static void EliminarEgituraHutsak(Body body)
    {
        foreach (Table table in body.Descendants<Table>().ToList())
        {
            foreach (TableRow row in table.Elements<TableRow>().ToList())
            {
                if (string.IsNullOrWhiteSpace(ParagrafoakTestua(row.Descendants<Paragraph>())))
                {
                    row.Remove();
                }
            }

            if (!table.Elements<TableRow>().Any())
            {
                table.Remove();
            }
        }
    }

    private static List<int> EgunBlokeHasierak(IReadOnlyList<Paragraph> paragraphs)
    {
        return paragraphs
            .Select((paragraph, index) => new { paragraph, index })
            .Where(item => ParagrafoTestua(item.paragraph).Contains("{{EGUNA_IZENBURUA}}"))
            .Select(item => item.index)
            .ToList();
    }

    private static void EzabatuParagrafoBlokea(Body body, IReadOnlyList<Paragraph> blokea)
    {
        foreach (OpenXmlElement element in blokea.Select(p => ElementuaEzabatzeko(body, p)).OfType<OpenXmlElement>().Distinct().ToList())
        {
            element.Remove();
        }
    }

    private static OpenXmlElement? ElementuaEzabatzeko(Body body, OpenXmlElement element)
    {
        TableRow? row = element.Ancestors<TableRow>().FirstOrDefault();
        return row ?? GorputzekoElementua(body, element);
    }

    private static OpenXmlElement? GorputzekoElementua(Body body, OpenXmlElement element)
    {
        OpenXmlElement unekoa = element;
        while (unekoa.Parent is not null && unekoa.Parent != body)
        {
            unekoa = unekoa.Parent;
        }

        return unekoa.Parent == body ? unekoa : null;
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

    private static void GehituAbisuaGehiegiBada(Body body, List<string> abisuak, string marka, int datuKop, string izena)
    {
        int markaKop = body.Descendants<Paragraph>().Sum(paragraph => AgerraldiKopurua(ParagrafoTestua(paragraph), marka));
        if (datuKop > markaKop)
        {
            abisuak.Add($"Txantiloiak {markaKop} {izena} leku ditu, baina {datuKop} datu daude. Soberakoak ez dira dokumentuan sartuko.");
        }
    }

    private static string EgunaGakoa(string eguna) => (eguna ?? "").Trim().ToUpperInvariant();

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

        Dictionary<string, int> hilabeteak = new(StringComparer.OrdinalIgnoreCase)
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
        if (!hilabeteak.TryGetValue(hilabetea, out int hilabeteZenbakia)
            || eguna < 1
            || eguna > DateTime.DaysInMonth(DateTime.Today.Year, hilabeteZenbakia))
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

    private static int MarkaKopurua(IReadOnlyList<Paragraph> paragraphs, string marka) => paragraphs.Sum(paragraph => AgerraldiKopurua(ParagrafoTestua(paragraph), marka));

    private static string ParagrafoakTestua(IEnumerable<Paragraph> paragraphs) => string.Concat(paragraphs.Select(ParagrafoTestua));

    private static string ParagrafoTestua(Paragraph paragraph) => string.Concat(paragraph.Descendants<WordText>().Select(text => text.Text));

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
        return index < 0 ? testua : testua[..index] + ordezkoa + testua[(index + bilatu.Length)..];
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
