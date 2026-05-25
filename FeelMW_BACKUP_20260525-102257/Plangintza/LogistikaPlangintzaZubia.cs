using FeelmwLogistika.Logistika.DatuModeloak;
using FeelmwLogistika.Plangintza.DatuModeloak;
using FeelmwLogistika.Plangintza.ExelDB;
using System.Text.RegularExpressions;

namespace FeelmwLogistika.Plangintza
{
    public static class LogistikaPlangintzaZubia
    {
        private static readonly List<Ostalak> logistikaOstalak = new List<Ostalak>();
        private static readonly List<Bidaiak> sortutakoBidaiak = new List<Bidaiak>();

        public static IReadOnlyList<Bidaiak> SortutakoBidaiak => sortutakoBidaiak;

        public static void Garbitu()
        {
            logistikaOstalak.Clear();
            sortutakoBidaiak.Clear();
        }

        public static void OstalaGorde(Ostalak? ostala)
        {
            if (ostala == null || string.IsNullOrWhiteSpace(ostala.OstalaIzena))
            {
                return;
            }

            int index = logistikaOstalak.FindIndex(o => IzenNormalizatua(o.OstalaIzena) == IzenNormalizatua(ostala.OstalaIzena));
            if (index >= 0)
            {
                logistikaOstalak[index] = ostala;
            }
            else
            {
                logistikaOstalak.Add(ostala);
            }
        }

        public static void OstalakGorde(IEnumerable<Ostalak>? ostalak)
        {
            if (ostalak == null)
            {
                return;
            }

            foreach (Ostalak ostala in ostalak)
            {
                OstalaGorde(ostala);
            }
        }

        public static void BidaiaGorde(Bidaiak? bidaia)
        {
            if (bidaia?.HotelHautatua == null || string.IsNullOrWhiteSpace(bidaia.HotelHautatua.Izena))
            {
                return;
            }

            string hotelIzena = IzenNormalizatua(bidaia.HotelHautatua.Izena);
            sortutakoBidaiak.RemoveAll(b => IzenNormalizatua(b.HotelHautatua?.Izena) == hotelIzena);
            sortutakoBidaiak.Add(bidaia);
        }

        public static void BidaiaKendu(Bidaiak? bidaia)
        {
            if (bidaia?.HotelHautatua == null)
            {
                return;
            }

            string hotelIzena = IzenNormalizatua(bidaia.HotelHautatua.Izena);
            sortutakoBidaiak.RemoveAll(b => IzenNormalizatua(b.HotelHautatua?.Izena) == hotelIzena);
        }

        public static Hotelak? BuscarHotelPorNombre(IEnumerable<Hotelak>? hotelak, string? ostalaIzena)
        {
            if (hotelak == null || string.IsNullOrWhiteSpace(ostalaIzena))
            {
                return null;
            }

            string izenNormalizatua = IzenNormalizatua(ostalaIzena);
            List<Hotelak> hotelZerrenda = hotelak
                .Where(h => !string.IsNullOrWhiteSpace(h.Izena))
                .ToList();

            Hotelak? zehatza = hotelZerrenda.FirstOrDefault(h => IzenNormalizatua(h.Izena) == izenNormalizatua);
            if (zehatza != null)
            {
                return zehatza;
            }

            List<Hotelak> partzialak = hotelZerrenda
                .Where(h => IzenPartzialSegurua(izenNormalizatua, IzenNormalizatua(h.Izena)))
                .ToList();

            return partzialak.Count == 1 ? partzialak[0] : null;
        }

        public static KargaEmaitza KargatuPlangintzaOstaletik(IEnumerable<Hotelak>? hotelak)
        {
            KargaEmaitza emaitza = new KargaEmaitza();

            if (hotelak == null || !hotelak.Any())
            {
                emaitza.Abisuak.Add("Plangintzako hotel zerrenda hutsik dago; ezin da Logistikako ostatua automatikoki lotu.");
                return emaitza;
            }

            foreach (Ostalak ostala in logistikaOstalak)
            {
                Bidaiak? bidaia = SortuEdoEguneratuBidaia(ostala, hotelak, out string? abisua);
                if (bidaia != null)
                {
                    emaitza.Bidaiak.Add(bidaia);
                    emaitza.AzkenBidaia = bidaia;
                }
                else if (!string.IsNullOrWhiteSpace(abisua))
                {
                    emaitza.Abisuak.Add(abisua);
                }
            }

            return emaitza;
        }

        public static Bidaiak? SortuEdoEguneratuBidaia(Ostalak? ostala, IEnumerable<Hotelak>? hotelak, out string? abisua)
        {
            abisua = null;
            if (ostala == null || string.IsNullOrWhiteSpace(ostala.OstalaIzena))
            {
                abisua = "Logistikako ostatua ez dago beteta; ezin da Plangintzara pasa.";
                return null;
            }

            Hotelak? hotela = BuscarHotelPorNombre(hotelak, ostala.OstalaIzena);
            if (hotela == null)
            {
                abisua = HotelLoturaAbisua(hotelak, ostala.OstalaIzena);
                return null;
            }

            Bidaiak bidaia = BidaiaSortu(ostala, hotela);
            BidaiaGorde(bidaia);
            return bidaia;
        }

        public static (bool Sortuta, string Mezua) SortuPlangintzaDokumentua(string izena)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(izena))
                {
                    return (false, "Plangintza ezin da sortu: dokumentuaren izena hutsik dago.");
                }

                if (sortutakoBidaiak.Count == 0 && logistikaOstalak.Count > 0)
                {
                    List<Hotelak> hotelak = ListakKargatu.OstalakListaratu();
                    KargatuPlangintzaOstaletik(hotelak);
                }

                if (sortutakoBidaiak.Count == 0)
                {
                    return (false, "Plangintza ez da sortu: ez dago Logistikatik lotutako hotelik.");
                }

                foreach (Bidaiak bidaia in sortutakoBidaiak)
                {
                    if (!BidaiaOsatua(bidaia, out string mezua))
                    {
                        return (false, mezua);
                    }
                }

                ExelaSortu.Gorde(izena, sortutakoBidaiak.ToList());
                return (true, "Plangintza exela sortuta.");
            }
            catch (Exception ex)
            {
                return (false, $"Plangintza ez da sortu: {ex.Message}");
            }
        }

        private static Bidaiak BidaiaSortu(Ostalak ostala, Hotelak hotela)
        {
            int gauak = Math.Max(1, ostala.Gauak);
            int egunKop = Math.Max(2, gauak + 1);
            List<ExelaSortu.EgunLaburpena> egunak = new List<ExelaSortu.EgunLaburpena>();
            for (int i = 1; i <= egunKop; i++)
            {
                egunak.Add(new ExelaSortu.EgunLaburpena(i, "", "", "", ""));
            }

            return new Bidaiak(
                hotela,
                egunKop,
                egunak,
                new List<ExelaSortu.EkintzaDatuak>()
            );
        }

        private static bool BidaiaOsatua(Bidaiak bidaia, out string mezua)
        {
            if (bidaia.HotelHautatua == null || string.IsNullOrWhiteSpace(bidaia.HotelHautatua.Izena))
            {
                mezua = "Plangintza ez da sortu: bidaiak ez dauka hotelik aukeratuta.";
                return false;
            }

            if (bidaia.EgunKop <= 0)
            {
                mezua = $"Plangintza ez da sortu: '{bidaia.HotelHautatua.Izena}' hotelaren Gauak/EgunKop balioa falta da edo 0 da.";
                return false;
            }

            mezua = "";
            return true;
        }

        private static string IzenNormalizatua(string? izena)
        {
            if (string.IsNullOrWhiteSpace(izena))
            {
                return "";
            }

            return Regex.Replace(izena.Trim(), @"\s+", " ").ToUpperInvariant();
        }

        private static bool IzenPartzialSegurua(string ostala, string hotela)
        {
            if (ostala.Length < 6 || hotela.Length < 6)
            {
                return false;
            }

            return hotela.Contains(ostala) || ostala.Contains(hotela);
        }

        private static string HotelLoturaAbisua(IEnumerable<Hotelak>? hotelak, string ostalaIzena)
        {
            string izenNormalizatua = IzenNormalizatua(ostalaIzena);
            int antzekoak = hotelak?
                .Count(h => IzenPartzialSegurua(izenNormalizatua, IzenNormalizatua(h.Izena))) ?? 0;

            if (antzekoak > 1)
            {
                return $"Plangintzan '{ostalaIzena}' ostatuarentzako lotura bat baino gehiago aurkitu da. Aukeratu eskuz cbxOstala/cbxHotela eremuan.";
            }

            return $"Ez da aurkitu Plangintzan '{ostalaIzena}' izeneko hotel baliokiderik. Aukeratu eskuz cbxOstala/cbxHotela eremuan.";
        }

        public class KargaEmaitza
        {
            public List<Bidaiak> Bidaiak { get; } = new List<Bidaiak>();
            public Bidaiak? AzkenBidaia { get; set; }
            public List<string> Abisuak { get; } = new List<string>();
        }
    }
}
