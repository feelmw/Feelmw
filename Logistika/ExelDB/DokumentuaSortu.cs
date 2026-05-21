using DocumentFormat.OpenXml.Packaging;
using FeelmwLogistika.Logistika.DatuModeloak;
using WordText = DocumentFormat.OpenXml.Wordprocessing.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Windows.Forms;

namespace FeelmwLogistika.Logistika.ExelDB
{
    public class DokumentuaSortu
    {
        public static void Sortu(List<Ostalak> ostalak, List<Ekintzak> ekintzak, List<Garraioak> garraioak, string nombreUsuario)
        {
            int z1 = 0, z2 = 0;

            string plantillaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plantilla.docx");

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string outputPath = Path.Combine(desktopPath, $"{nombreUsuario}_MyfeelDoc.docx");

            if (!File.Exists(plantillaPath))
            {
                MessageBox.Show("Ez da aurkitu Word txantiloia: Plantilla.docx");
                return;
            }

            File.Copy(plantillaPath, outputPath, true);

            using (WordprocessingDocument doc =
                WordprocessingDocument.Open(outputPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                if (ostalak.Count > 0 && ostalak.All(o => o.EsKlasikoa))
                {
                    OstalaKlasikoaErrenkadakKendu(body);
                }

                for (int i = 0; i < ostalak.Count; i++)
                {
                    var o = ostalak[i];
                    Reemplazar(body, i + 1, o);
                }

                for(int i = 0; i < ekintzak.Count; i++)
                {
                    var e = ekintzak[i];
                    ReemplazarEkintza(body, i + 1, e);
                }

                for(int i = 0; i < garraioak.Count; i++)
                {
                    var g = garraioak[i];
                    ReemplazarGarraio(body, i + 1, g);
                }

                var tablas = body.Descendants<Table>().ToList();

                if (tablas.Count < 6)
                {
                    doc.MainDocumentPart.Document.Save();
                    return;
                }

                Table tablaOstalak1 = tablas[1];
                Table tablaOstalak2 = tablas[2];
                Table tablaEkintzak1 = tablas[3];
                Table tablaEkintzak2 = tablas[4];
                Table tablaGarraioak1 = tablas[5];
                z1 = ostalak.Count;
                if (z1 == 0)
                {
                    tablaOstalak1.Remove();
                    tablaOstalak2.Remove();
                }
                else if(z1 > 3)
                {
                    z2 = z1 - 3;
                    z1 = 3;
                    EliminarColumnasSobrantes(tablaOstalak1, z1);
                    EliminarColumnasSobrantes(tablaOstalak2, z2);
                }
                else
                {
                    EliminarColumnasSobrantes(tablaOstalak1, z1);
                    tablaOstalak2.Remove();
                }
                    
                z1 = ekintzak.Count;
                if (z1 == 0) 
                { 
                    tablaEkintzak1.Remove();
                    tablaEkintzak2.Remove();
                }
                else if(z1 > 3)
                {
                    z2 = z1 - 3;
                    z1 = 3;
                    EliminarColumnasSobrantes(tablaEkintzak1, z1);
                    EliminarColumnasSobrantes(tablaEkintzak2, z2);
                }
                else
                {
                    EliminarColumnasSobrantes(tablaEkintzak1, z1);
                    tablaEkintzak2.Remove();
                }  

                z1 = garraioak.Count;
                if (z1 == 0)
                {
                    tablaGarraioak1.Remove();
                }
                else 
                { 
                    EliminarColumnasSobrantes(tablaGarraioak1, z1);
                }

                doc.MainDocumentPart.Document.Save();
            }
        }
        private static void Reemplazar(Body body, int index, Ostalak o)
        {
            Dictionary<string, string> mapa = new()
    {
        { $"{{{{Ost_Ostala_{index}}}}}", o.OstalaIzena },
        { $"{{{{ost_bonoa_{index}}}}}", o.Bonoa },
        { $"{{{{ost_helbidea_{index}}}}}", o.Helbidea },
        { $"{{{{ost_lokalizatzailea_{index}}}}}", o.XehetasunOsagarriakErakutsi ? o.Lokalizatzailea : string.Empty },
        { $"{{{{ost_gauak_{index}}}}}", o.XehetasunOsagarriakErakutsi ? o.Gauak.ToString() : string.Empty },
        { $"{{{{ost_datak_{index}}}}}", o.XehetasunOsagarriakErakutsi ? o.Datak : string.Empty },
        { $"{{{{ost_gelak_{index}}}}}", o.XehetasunOsagarriakErakutsi ? o.Gelak : string.Empty },
        { $"{{{{ost_checkin_{index}}}}}", o.Checkin },
        { $"{{{{ost_checkout_{index}}}}}", o.Checkout },
        { $"{{{{ost_doc_{index}}}}}", o.Dokumentazioa },
        { $"{{{{ost_harrera_{index}}}}}", o.Harrera },
        { $"{{{{ost_toallak_{index}}}}}", o.Toailak ? "Bai" : "Ez" },
        { $"{{{{ost_izarak_{index}}}}}", o.Izarak ? "Bai" : "Ez" },
        { $"{{{{ost_fidantza_{index}}}}}", o.Fidantza ? o.FidantzaKuota : "Ez" },
        { $"{{{{ost_luggage_{index}}}}}", o.Luggage ? o.LuggageKuota : "Ez" },
        { $"{{{{ost_inst_{index}}}}}", o.Instalazioak }
    };

            foreach (var text in body.Descendants<WordText>())
            {
                foreach (var item in mapa)
                {
                    if (text.Text.Contains(item.Key))
                    {
                        text.Text = text.Text.Replace(item.Key, item.Value);
                    }
                }
            }
        }

        private static void OstalaKlasikoaErrenkadakKendu(Body body)
        {
            string[] ezkutatuBeharrekoak =
            {
                "ost_lokalizatzailea_",
                "ost_gauak_",
                "ost_datak_",
                "ost_gelak_"
            };

            foreach (var row in body.Descendants<TableRow>().ToList())
            {
                string testua = string.Concat(row.Descendants<WordText>().Select(t => t.Text));
                if (ezkutatuBeharrekoak.Any(testua.Contains))
                {
                    row.Remove();
                }
            }
        }

        private  static void ReemplazarEkintza(Body body, int index, Ekintzak e)
        {
            Dictionary<string, string> mapa = new()
            {
                { $"{{{{eki_ekintza_{index}}}}}", e.EkintzaIzena },
                { $"{{{{eki_bonoa_{index}}}}}", e.Bonoa },
                { $"{{{{eki_eguna_{index}}}}}", e.Iraupena },
                { $"{{{{eki_iraupena_{index}}}}}", e.Iraupena },
                { $"{{{{eki_lokalizatzailea_{index}}}}}", " " },
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

            foreach (var text in body.Descendants<WordText>())
            {
                foreach (var item in mapa)
                {
                    if (text.Text.Contains(item.Key))
                    {
                        text.Text = text.Text.Replace(item.Key, item.Value);
                    }
                }
            }
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

            foreach (var text in body.Descendants<WordText>())
            {
                foreach (var item in mapa)
                {
                    if (text.Text.Contains(item.Key))
                    {
                        text.Text = text.Text.Replace(item.Key, item.Value);
                    }
                }
            }
        }
        private static void EliminarColumnasSobrantes(Table tabla, int objetosUsados)
        {
            int maxObjetos = 3;

            // +1 porque la primera columna es la de títulos
            for (int colIndex = maxObjetos; colIndex > objetosUsados; colIndex--)
            {
                foreach (TableRow row in tabla.Elements<TableRow>())
                {
                    var cells = row.Elements<TableCell>().ToList();

                    if (cells.Count > colIndex)
                    {
                        cells[colIndex].Remove();
                    }
                }
            }
        }

    }

}
