using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FeelmwLogistika.Blazor.Models;
using WordText = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace FeelmwLogistika.Blazor.Infrastructure.Documents;

public sealed class LogistikaDocumentService(HttpClient httpClient) : ILogistikaDocumentService
{
    public async Task<DocumentGenerationResult> CreateDocumentFromTemplatePathAsync(
        string templatePath,
        IEnumerable<Ostalak> ostalak,
        IEnumerable<Ekintzak> ekintzak,
        IEnumerable<Garraioak> garraioak,
        string nombreUsuario,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using MemoryStream template = await DocumentTemplateResolver.LoadTemplateAsync(httpClient, templatePath, cancellationToken);
            return CreateDocument(template, ostalak, ekintzak, garraioak, nombreUsuario);
        }
        catch (Exception ex)
        {
            return new DocumentGenerationResult { Errors = [$"Errorea dokumentua sortzean: {ex.Message}"] };
        }
    }

    public DocumentGenerationResult CreateDocument(
        Stream templateStream,
        IEnumerable<Ostalak> ostalak,
        IEnumerable<Ekintzak> ekintzak,
        IEnumerable<Garraioak> garraioak,
        string nombreUsuario)
    {
        DocumentGenerationResult result = new()
        {
            FileName = $"{DocumentTemplateResolver.SafeFileName(nombreUsuario, "FeelMW")}_MyfeelDoc.docx"
        };

        try
        {
            List<Ostalak> ostalakList = ostalak.ToList();
            List<Ekintzak> ekintzakList = ekintzak.ToList();
            List<Garraioak> garraioakList = garraioak.ToList();
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

                for (int i = 0; i < ostalakList.Count; i++)
                {
                    ReemplazarOstala(body, i + 1, ostalakList[i]);
                }

                for (int i = 0; i < ekintzakList.Count; i++)
                {
                    ReemplazarEkintza(body, i + 1, ekintzakList[i]);
                }

                for (int i = 0; i < garraioakList.Count; i++)
                {
                    ReemplazarGarraio(body, i + 1, garraioakList[i]);
                }

                List<Table> tablas = body.Descendants<Table>().ToList();
                if (tablas.Count >= 6)
                {
                    Table tablaOstalak1 = tablas[1];
                    Table tablaOstalak2 = tablas[2];
                    Table tablaEkintzak1 = tablas[3];
                    Table tablaEkintzak2 = tablas[4];
                    Table tablaGarraioak1 = tablas[5];

                    EliminarFilasComidaHutsak(tablaOstalak1, ostalakList.Take(3).ToList());
                    EliminarFilasComidaHutsak(tablaOstalak2, ostalakList.Skip(3).Take(3).ToList());
                    AjustarTablasDobles(tablaOstalak1, tablaOstalak2, ostalakList.Count);
                    AjustarTablasDobles(tablaEkintzak1, tablaEkintzak2, ekintzakList.Count);

                    if (garraioakList.Count == 0)
                    {
                        tablaGarraioak1.Remove();
                    }
                    else
                    {
                        EliminarColumnasSobrantes(tablaGarraioak1, garraioakList.Count);
                    }
                }
                else
                {
                    result.Warnings.Add("Txantiloiak ez dauka espero zen taula kopurua; markagailuak ordezkatu dira baina ez da taulen garbiketa osoa egin.");
                }

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

    private static void AjustarTablasDobles(Table primera, Table segunda, int count)
    {
        if (count == 0)
        {
            primera.Remove();
            segunda.Remove();
        }
        else if (count > 3)
        {
            EliminarColumnasSobrantes(primera, 3);
            EliminarColumnasSobrantes(segunda, count - 3);
        }
        else
        {
            EliminarColumnasSobrantes(primera, count);
            segunda.Remove();
        }
    }

    private static void ReemplazarOstala(Body body, int index, Ostalak o)
    {
        Dictionary<string, string> mapa = new()
        {
            { $"{{{{Ost_Ostala_{index}}}}}", o.OstalaIzena },
            { $"{{{{ost_bonoa_{index}}}}}", o.Bonoa },
            { $"{{{{ost_helbidea_{index}}}}}", o.Helbidea },
            { $"{{{{ost_lokalizatzailea_{index}}}}}", o.Lokalizatzailea },
            { $"{{{{ost_gauak_{index}}}}}", o.Gauak.ToString() },
            { $"{{{{ost_datak_{index}}}}}", o.Datak },
            { $"{{{{ost_gelak_{index}}}}}", o.Gelak },
            { $"{{{{ost_checkin_{index}}}}}", o.Checkin },
            { $"{{{{ost_checkout_{index}}}}}", o.Checkout },
            { $"{{{{ost_doc_{index}}}}}", o.Dokumentazioa },
            { $"{{{{ost_harrera_{index}}}}}", o.Harrera },
            { $"{{{{ost_gosaria_{index}}}}}", o.Gosaria },
            { $"{{{{ost_bazkaria_{index}}}}}", o.Bazkaria },
            { $"{{{{ost_afaria_{index}}}}}", o.Afaria },
            { $"{{{{ost_toallak_{index}}}}}", o.Toailak ? "Bai" : "Ez" },
            { $"{{{{ost_izarak_{index}}}}}", o.Izarak ? "Bai" : "Ez" },
            { $"{{{{ost_fidantza_{index}}}}}", o.Fidantza },
            { $"{{{{ost_luggage_{index}}}}}", o.Luggage },
            { $"{{{{ost_inst_{index}}}}}", o.Instalazioak }
        };

        OrdezkatuMarkak(body, mapa);
    }

    private static void ReemplazarEkintza(Body body, int index, Ekintzak e)
    {
        Dictionary<string, string> mapa = new()
        {
            { $"{{{{eki_ekintza_{index}}}}}", e.EkintzaIzena },
            { $"{{{{eki_bonoa_{index}}}}}", e.Bonoa },
            { $"{{{{eki_eguna_{index}}}}}", e.Iraupena },
            { $"{{{{eki_iraupena_{index}}}}}", e.Iraupena },
            { $"{{{{eki_lokalizatzailea_{index}}}}}", e.Lokali },
            { $"{{{{eki_kontaktua_{index}}}}}", e.Kontaktua },
            { $"{{{{eki_elkartokia_{index}}}}}", e.Elkartokia },
            { $"{{{{eki_eginbeharrekoa_{index}}}}}", e.Iristean },
            { $"{{{{eki_eramanM_{index}}}}}", e.EramanM },
            { $"{{{{eki_bertakoM_{index}}}}}", e.BertanM },
            { $"{{{{eki_alda_{index}}}}}", e.Aldagela ? "Bai" : "Ez" },
            { $"{{{{eki_komu_{index}}}}}", e.Komuna ? "Bai" : "Ez" },
            { $"{{{{eki_egonlekua_{index}}}}}", e.Egonlekua },
            { $"{{{{eki_info_{index}}}}}", e.Informazioa }
        };

        OrdezkatuMarkak(body, mapa);
    }

    private static void ReemplazarGarraio(Body body, int index, Garraioak g)
    {
        Dictionary<string, string> mapa = new()
        {
            { $"{{{{gar_garraioa_{index}}}}}", g.GarraioaIzena },
            { $"{{{{gar_eguna_{index}}}}}", g.Eguna },
            { $"{{{{gar_ordutegia_{index}}}}}", g.Ordutegia },
            { $"{{{{gar_lokalizatzailea_{index}}}}}", g.Lokalizatzailea },
            { $"{{{{gar_kontaktua_{index}}}}}", g.Kontaktua },
            { $"{{{{gar_elkartokia_{index}}}}}", g.Elkargunea },
            { $"{{{{gar_eginbeharrak_{index}}}}}", g.Eginbeharrak },
            { $"{{{{gar_info_{index}}}}}", g.Informazioa }
        };

        OrdezkatuMarkak(body, mapa);
    }

    private static void OrdezkatuMarkak(Body body, Dictionary<string, string> mapa)
    {
        foreach (Paragraph paragraph in body.Descendants<Paragraph>())
        {
            string testua = string.Concat(paragraph.Descendants<WordText>().Select(t => t.Text));
            if (!mapa.Keys.Any(testua.Contains))
            {
                continue;
            }

            foreach (KeyValuePair<string, string> item in mapa)
            {
                testua = testua.Replace(item.Key, item.Value);
            }

            ParagrafoTestuaEzarri(paragraph, testua);
        }
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
        foreach (WordText zatia in zatiak.Skip(1))
        {
            zatia.Text = "";
        }
    }

    private static void EliminarColumnasSobrantes(Table tabla, int objetosUsados)
    {
        const int maxObjetos = 3;
        for (int colIndex = maxObjetos; colIndex > objetosUsados; colIndex--)
        {
            foreach (TableRow row in tabla.Elements<TableRow>())
            {
                List<TableCell> cells = row.Elements<TableCell>().ToList();
                if (cells.Count > colIndex)
                {
                    cells[colIndex].Remove();
                }
            }
        }
    }

    private static void EliminarFilasComidaHutsak(Table tabla, List<Ostalak> ostalak)
    {
        if (tabla.Parent is null || ostalak.Count == 0)
        {
            return;
        }

        foreach (TableRow row in tabla.Elements<TableRow>().ToList())
        {
            string testua = string.Concat(row.Descendants<WordText>().Select(t => t.Text)).ToLowerInvariant();
            if (testua.Contains("gosaria") && ostalak.All(o => string.IsNullOrWhiteSpace(o.Gosaria)))
            {
                row.Remove();
            }
            else if (testua.Contains("bazkaria") && ostalak.All(o => string.IsNullOrWhiteSpace(o.Bazkaria)))
            {
                row.Remove();
            }
            else if (testua.Contains("afaria") && ostalak.All(o => string.IsNullOrWhiteSpace(o.Afaria)))
            {
                row.Remove();
            }
        }
    }
}
