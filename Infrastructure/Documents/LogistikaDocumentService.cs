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
        DocumentHeaderData headerData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using MemoryStream template = await DocumentTemplateResolver.LoadTemplateAsync(httpClient, templatePath, cancellationToken);
            return CreateDocument(template, ostalak, ekintzak, garraioak, nombreUsuario, headerData);
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
        string nombreUsuario,
        DocumentHeaderData headerData)
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

                ReemplazarGoikoTaula(mainPart, body, headerData);

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

                NormalizarPosicionTablas(body);
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
            { $"{{{{OST_OSTALA_{index}}}}}", o.OstalaIzena },
            { $"{{{{OST_BONOA_{index}}}}}", o.Bonoa },
            { $"{{{{OST_HELBIDEA_{index}}}}}", o.Helbidea },
            { $"{{{{OST_LOKALIZATZAILEA_{index}}}}}", o.Lokalizatzailea },
            { $"{{{{OST_GAUAK_{index}}}}}", o.Gauak.ToString() },
            { $"{{{{OST_DATAK_{index}}}}}", o.Datak },
            { $"{{{{OST_GELAK_{index}}}}}", o.Gelak },
            { $"{{{{OST_CHECKIN_{index}}}}}", o.Checkin },
            { $"{{{{OST_CHECKOUT_{index}}}}}", o.Checkout },
            { $"{{{{OST_DOC_{index}}}}}", o.Dokumentazioa },
            { $"{{{{OST_HARRERA_{index}}}}}", o.Harrera },
            { $"{{{{OST_GOSARIA_{index}}}}}", o.Gosaria },
            { $"{{{{OST_BAZKARIA_{index}}}}}", o.Bazkaria },
            { $"{{{{OST_AFARIA_{index}}}}}", o.Afaria },
            { $"{{{{OST_TOALLAK_{index}}}}}", o.Toailak ? "Bai" : "Ez" },
            { $"{{{{OST_IZARAK_{index}}}}}", o.Izarak ? "Bai" : "Ez" },
            { $"{{{{OST_FIDANTZA_{index}}}}}", o.Fidantza },
            { $"{{{{OST_LUGGAGE_{index}}}}}", o.Luggage },
            { $"{{{{OST_INST_{index}}}}}", o.Instalazioak }
        };

        OrdezkatuMarkak(body, mapa);
    }

    private static void ReemplazarEkintza(Body body, int index, Ekintzak e)
    {
        Dictionary<string, string> mapa = new()
        {
            { $"{{{{EKI_EKINTZA_{index}}}}}", e.EkintzaIzena },
            { $"{{{{EKI_BONOA_{index}}}}}", e.Bonoa },
            { $"{{{{EKI_EGUNA_{index}}}}}", e.Iraupena },
            { $"{{{{EKI_IRAUPENA_{index}}}}}", e.Iraupena },
            { $"{{{{EKI_LOKALIZATZAILEA_{index}}}}}", e.Lokali },
            { $"{{{{EKI_KONTAKTUA_{index}}}}}", e.Kontaktua },
            { $"{{{{EKI_ELKARTOKIA_{index}}}}}", e.Elkartokia },
            { $"{{{{EKI_EGINBEHARREKOA_{index}}}}}", e.Iristean },
            { $"{{{{EKI_ERAMANM_{index}}}}}", e.EramanM },
            { $"{{{{EKI_BERTAKOM_{index}}}}}", e.BertanM },
            { $"{{{{EKI_ALDA_{index}}}}}", e.Aldagela ? "Bai" : "Ez" },
            { $"{{{{EKI_KOMU_{index}}}}}", e.Komuna ? "Bai" : "Ez" },
            { $"{{{{EKI_EGONLEKUA_{index}}}}}", e.Egonlekua },
            { $"{{{{EKI_INFO_{index}}}}}", e.Informazioa }
        };

        OrdezkatuMarkak(body, mapa);
    }

    private static void ReemplazarGarraio(Body body, int index, Garraioak g)
    {
        Dictionary<string, string> mapa = new()
        {
            { $"{{{{GAR_GARRAIOA_{index}}}}}", g.GarraioaIzena },
            { $"{{{{GAR_EGUNA_{index}}}}}", g.Eguna },
            { $"{{{{GAR_ORDUTEGIA_{index}}}}}", g.Ordutegia },
            { $"{{{{GAR_LOKALIZATZAILEA_{index}}}}}", g.Lokalizatzailea },
            { $"{{{{GAR_KONTAKTUA_{index}}}}}", g.Kontaktua },
            { $"{{{{GAR_ELKARTOKIA_{index}}}}}", g.Elkargunea },
            { $"{{{{GAR_EGINBEHARRAK_{index}}}}}", g.Eginbeharrak },
            { $"{{{{GAR_INFO_{index}}}}}", g.Informazioa }
        };

        OrdezkatuMarkak(body, mapa);
    }

    private static void OrdezkatuMarkak(Body body, Dictionary<string, string> mapa)
    {
        DocumentPlaceholderReplacer.Replace(body, mapa.Select(item => new DocumentPlaceholderValue(item.Key, item.Value)));
    }

    private static void NormalizarPosicionTablas(Body body)
    {
        foreach (Table table in body.Descendants<Table>())
        {
            TableProperties? properties = table.GetFirstChild<TableProperties>();
            if (properties is null)
            {
                continue;
            }

            properties.RemoveAllChildren<TablePositionProperties>();
            ReordenarJustificacionTabla(properties);
        }
    }

    private static void ReordenarJustificacionTabla(TableProperties properties)
    {
        TableJustification? justification = properties.GetFirstChild<TableJustification>();
        if (justification is null)
        {
            return;
        }

        TableWidth? width = properties.GetFirstChild<TableWidth>();
        if (width is not null && justification.PreviousSibling() == width)
        {
            return;
        }

        TableStyle? style = properties.GetFirstChild<TableStyle>();
        justification.Remove();
        if (width is not null)
        {
            properties.InsertAfter(justification, width);
        }
        else if (style is not null)
        {
            properties.InsertAfter(justification, style);
        }
        else
        {
            properties.PrependChild(justification);
        }
    }

    private static void ReemplazarGoikoTaula(MainDocumentPart mainPart, Body body, DocumentHeaderData headerData)
    {
        IReadOnlyList<DocumentPlaceholderValue> replacements = DocumentHeaderPlaceholders.From(headerData);
        DocumentPlaceholderReplacer.Replace(body, replacements);
        foreach (HeaderPart headerPart in mainPart.HeaderParts)
        {
            if (headerPart.Header is not null)
            {
                DocumentPlaceholderReplacer.Replace(headerPart.Header, replacements);
            }
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
