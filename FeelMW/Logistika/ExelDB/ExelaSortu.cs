using ClosedXML.Excel;
using FeelmwLogistika.Logistika.DatuModeloak;
using System;
using System.Collections.Generic;
using System.IO;

namespace FeelmwLogistika.Logistika.ExelDB
{
    class Exelak
    {
        // Exela sortu bin\Debug\netX karpetan
        public static string ExelBerriaSortu(string izena)
        {
            string ruta = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                $"Logistika - {izena}.xlsx"
            );

            using (var workbook = new XLWorkbook())
            {
                // Sheetak sortu
                workbook.Worksheets.Add("Ostalak");
                workbook.Worksheets.Add("Ekintzak");
                workbook.Worksheets.Add("Garraioak");

                workbook.SaveAs(ruta);
            }

            return ruta;
        }

        public static void OstalakIdatzi(string ruta, List<Ostalak> lista)
        {
            using (var workbook = new XLWorkbook(ruta))
            {
                var sheet = workbook.Worksheet("Ostalak");

                int row = 1;

                foreach (var o in lista)
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
                    sheet.Cell(row, 12).Value = o.Gosariates;
                    sheet.Cell(row, 13).Value = o.Bazkariates;
                    sheet.Cell(row, 14).Value = o.Afariates;
                    sheet.Cell(row, 15).Value = DatuakIrakurri.BoolToBaiEz(o.Toailak);
                    sheet.Cell(row, 16).Value = DatuakIrakurri.BoolToBaiEz(o.Izarak);
                    sheet.Cell(row, 17).Value = DatuakIrakurri.BoolToBaiEz(o.Fidantza);
                    sheet.Cell(row, 18).Value = o.FidantzaKuota;
                    sheet.Cell(row, 19).Value = DatuakIrakurri.BoolToBaiEz(o.Luggage);
                    sheet.Cell(row, 20).Value = o.LuggageKuota;
                    sheet.Cell(row, 21).Value = o.Instalazioak;

                    row++;
                }

                workbook.Save();
            }
        }

        public static void EkintzakIdatzi(string ruta, List<Ekintzak> lista)
        {
            using (var workbook = new XLWorkbook(ruta))
            {
                var sheet = workbook.Worksheet("Ekintzak");

                int row = 1;

                foreach (var e in lista)
                {
                    sheet.Cell(row, 1).Value = e.EkintzaIzena;
                    sheet.Cell(row, 2).Value = e.Bonoa;
                    sheet.Cell(row, 3).Value = e.Iraupena;
                    sheet.Cell(row, 4).Value = e.Kontaktua;
                    sheet.Cell(row, 5).Value = e.Elkartokia;
                    sheet.Cell(row, 6).Value = e.Iristean;
                    sheet.Cell(row, 7).Value = e.EramanM;
                    sheet.Cell(row, 8).Value = e.BertanM;
                    sheet.Cell(row, 9).Value = DatuakIrakurri.BoolToBaiEz(e.Aldagela);
                    sheet.Cell(row, 10).Value = DatuakIrakurri.BoolToBaiEz(e.Komuna);
                    sheet.Cell(row, 11).Value = e.Egonlekua;
                    sheet.Cell(row, 12).Value = e.Informazioa;
                    sheet.Cell(row, 13).Value = e.Lokali;

                    row++;
                }

                workbook.Save();
            }
        }

        public static void GarraioakIdatzi(string ruta, List<Garraioak> lista)
        {
            using (var workbook = new XLWorkbook(ruta))
            {
                var sheet = workbook.Worksheet("Garraioak");

                int row = 1;

                foreach (var g in lista)
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

                workbook.Save();
            }
        }
    }
}
