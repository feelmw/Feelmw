using DocumentFormat.OpenXml.Office2010.ExcelAc;
using FeelmwLogistika.Logistika.DatuModeloak;
using FeelmwLogistika.Plangintza;
using FeelmwLogistika.Plangintza.DatuModeloak;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FeelmwLogistika.Plangintza.Formularioak
{
    public partial class FPlangintza : Form
    {
        public static string IdEditatuDokumentua;
        public static string DokIzena;
        public static string Mota = "";
        private Panel panelak;
        private List<Hotelak> LisHot = new List<Hotelak>();
        private Hotelak? hotelaHautatua;
        private List<EkintzakPlan> LisEki = new List<EkintzakPlan>();
        private TableLayoutPanel tlpEgunLaburpenak;
        private TableLayoutPanel tlpEkintzak;
        private Button btnEkintzaGehitu;
        private Button btnMotaBerria;
        private Button btnDatuakIkusi;
        private Button btnZerrendaraGehitu;
        private readonly List<EgunekoLaburpenaKontrolak> egunekoLaburpenak = new List<EgunekoLaburpenaKontrolak>();
        private readonly List<EkintzaKontrolak> ekintzaKontrolak = new List<EkintzaKontrolak>();
        private readonly List<ExelDB.ExelaSortu.HotelDatuak> gordetakoHotelak = new List<ExelDB.ExelaSortu.HotelDatuak>();
        private readonly List<ExelDB.ExelaSortu.EgunLaburpena> gordetakoEgunak = new List<ExelDB.ExelaSortu.EgunLaburpena>();
        private readonly List<ExelDB.ExelaSortu.EkintzaDatuak> gordetakoEkintzak = new List<ExelDB.ExelaSortu.EkintzaDatuak>();
        private readonly List<Bidaiak> gordetakoBidaiak = new List<Bidaiak>();
        private Bidaiak? bidaiaEditatzen;
        public FPlangintza(Panel panelak)
        {
            InitializeComponent();
            this.panelak = panelak;
            AppEstiloa.Aplikatu(this);
            EgunLaburpenakPrestatu();
            EkintzakPrestatu();
            DatuakIkusiBotoiaPrestatu();
            ZerrendaraGehituBotoiaPrestatu();

            btnGorde.MouseEnter += (s, e) => btnGorde.BackColor = AppEstiloa.UrdinaHover;
            btnGorde.MouseLeave += (s, e) => btnGorde.BackColor = AppEstiloa.Urdina;
            btnSortu.MouseEnter += (s, e) => btnSortu.BackColor = AppEstiloa.UrdinaHover;
            btnSortu.MouseLeave += (s, e) => btnSortu.BackColor = AppEstiloa.Urdina;
            btnIrten.MouseEnter += (s, e) => btnIrten.BackColor = AppEstiloa.GorriaHover;
            btnIrten.MouseLeave += (s, e) => btnIrten.BackColor = AppEstiloa.Gorria;
            btnGorde.Click += btnGorde_Click;
            cbxOstala.SelectedValueChanged += (s, e) => HotelaHautatuaGorde();
            cbxOstala.SelectedIndexChanged += (s, e) => HotelaHautatuaGorde();
            nudEgunKop.ValueChanged += (s, e) =>
            {
                EgunLaburpenakSortu();
                EkintzenEgunakEguneratu();
            };
            txtData.TextChanged += (s, e) => EkintzenEgunakEguneratu();
        }

        private void btnIrten_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGorde_Click(object sender, EventArgs e)
        {
            if (gordetakoBidaiak.Count == 0)
            {
                UnekoDatuakZerrendetaraGorde();
            }

            string izenLehenetsia = string.IsNullOrWhiteSpace(DokIzena) ? "MyFeelPlangintza" : DokIzena;
            string izena = Interaction.InputBox(
                "Idatzi dokumentuaren izena:",
                "Plangintza berria",
                izenLehenetsia
            );

            if (string.IsNullOrWhiteSpace(izena))
            {
                MessageBox.Show("Dokumentuak izen bat behar du.");
                return;
            }

            try
            {
                ExelDB.ExelaSortu.Gorde(
                    izena,
                    gordetakoBidaiak
                );

                MessageBox.Show("Plangintza exela sortuta.");
                Close();
            }
            catch
            {
                MessageBox.Show("Errorea exela sortzean, datuak ez dira gorde.");
            }
        }

        private void FPlangintza_Load(object sender, EventArgs e)
        {
            try
            {
                LisHot = ExelDB.ListakKargatu.OstalakListaratu();
            }
            catch (Exception ex)
            {
                LisHot = new List<Hotelak>();
                MessageBox.Show("Ezin izan dira hotelak kargatu: " + ex.Message);
            }

            foreach (Hotelak hot in LisHot)
            {
                cbxOstala.Items.Add(hot.Izena);
            }

            try
            {
                LisEki = ExelDB.ListakKargatu.EkintzakListaratu();
            }
            catch (Exception ex)
            {
                LisEki = new List<EkintzakPlan>();
                MessageBox.Show("Ezin izan dira ekintza motak kargatu: " + ex.Message);
            }

            foreach (EkintzakPlan eki in LisEki)
            {
                cbxMota.Items.Add(eki.Mota);
            }
            EkintzenMotakEguneratu();

            if (!string.IsNullOrWhiteSpace(IdEditatuDokumentua))
            {
                PlangintzaDatuakKargatu();
            }
            else
            {
                KargatuPlangintzaOstaletik();
            }
        }

        private void cbxMota_SelectedValueChanged(object sender, EventArgs e)
        {
            foreach (EkintzakPlan eki in LisEki)
            {
                if (eki.Mota == cbxMota.Text)
                {
                    txtDeskribapena.Text = eki.Deskribapena;
                }
            }
        }

        private void PlangintzaDatuakKargatu()
        {
            if (!File.Exists(IdEditatuDokumentua))
            {
                MessageBox.Show("Ez da aurkitu aukeratutako plangintza dokumentua.");
                return;
            }

            ExelDB.ExelaSortu.PlangintzaDatuak datuak = ExelDB.ExelaSortu.Irakurri(IdEditatuDokumentua);

            cbxOstala.Text = datuak.Ostala;
            if (datuak.Hotela != null)
            {
                gordetakoHotelak.Clear();
                gordetakoHotelak.Add(datuak.Hotela);
            }
            nudEgunKop.Value = Math.Min(Math.Max(datuak.EgunKop, (int)nudEgunKop.Minimum), (int)nudEgunKop.Maximum);
            txtData.Text = datuak.Data;
            EgunLaburpenakKargatu(datuak.Egunak);
            EkintzakKargatu(datuak.Ekintzak);

            if (datuak.Hotela != null)
            {
                Hotelak hotela = new Hotelak(datuak.Hotela.Hiria, datuak.Hotela.Izena, datuak.Hotela.HelbideaUrl);
                gordetakoBidaiak.Clear();
                gordetakoBidaiak.Add(new Bidaiak(hotela, datuak.EgunKop, datuak.Egunak, datuak.Ekintzak));
            }
        }

        private void KargatuPlangintzaOstaletik()
        {
            LogistikaPlangintzaZubia.KargaEmaitza emaitza = LogistikaPlangintzaZubia.KargatuPlangintzaOstaletik(LisHot);

            foreach (string abisua in emaitza.Abisuak.Distinct())
            {
                MessageBox.Show(abisua);
            }

            if (emaitza.Bidaiak.Count == 0)
            {
                return;
            }

            foreach (Bidaiak bidaia in emaitza.Bidaiak)
            {
                if (!gordetakoBidaiak.Any(b => IzenBerdina(b.HotelHautatua?.Izena, bidaia.HotelHautatua?.Izena)))
                {
                    gordetakoBidaiak.Add(bidaia);
                    gordetakoHotelak.Add(HotelDatuakSortu(bidaia));
                    gordetakoEgunak.AddRange(bidaia.EgunLaburpenak);
                    gordetakoEkintzak.AddRange(bidaia.EkintzaDatuak);
                }
            }

            if (emaitza.AzkenBidaia == null)
            {
                return;
            }

            Bidaiak azkenBidaia = emaitza.AzkenBidaia;
            if (azkenBidaia.HotelHautatua != null)
            {
                cbxOstala.Text = azkenBidaia.HotelHautatua.Izena;
                hotelaHautatua = azkenBidaia.HotelHautatua;
            }

            if (azkenBidaia.EgunKop <= 0)
            {
                MessageBox.Show("Logistikako ostatuak ez dauka Gauak balio erabilgarririk; Plangintzan egun kopurua eskuz bete beharko da.");
            }

            nudEgunKop.Value = Math.Min(Math.Max(azkenBidaia.EgunKop, (int)nudEgunKop.Minimum), (int)nudEgunKop.Maximum);
            EgunLaburpenakKargatu(azkenBidaia.EgunLaburpenak);
            EkintzakKargatu(azkenBidaia.EkintzaDatuak);
        }

        private ExelDB.ExelaSortu.HotelDatuak HotelDatuakSortu(Bidaiak bidaia)
        {
            Hotelak? hotela = bidaia.HotelHautatua;
            return new ExelDB.ExelaSortu.HotelDatuak(
                hotela?.Hiria ?? "",
                hotela?.Izena ?? "",
                hotela?.HelbideaUrl ?? "",
                bidaia.EgunKop,
                bidaia.EgunLaburpenak.FirstOrDefault()?.Data ?? ""
            );
        }

        private bool IzenBerdina(string? lehenengoa, string? bigarrena)
        {
            return string.Equals(
                (lehenengoa ?? "").Trim(),
                (bigarrena ?? "").Trim(),
                StringComparison.OrdinalIgnoreCase
            );
        }

        private void EgunLaburpenakPrestatu()
        {
            lblData.Visible = false;
            lblGoizak.Visible = false;
            lblArratsaldea.Visible = false;
            lblGaua.Visible = false;
            txtData.Visible = false;
            txtGoisak.Visible = false;
            txtArratsaldea.Visible = false;
            txtGaua.Visible = false;

            nudEgunKop.Minimum = 1;
            nudEgunKop.Value = Math.Max(nudEgunKop.Value, nudEgunKop.Minimum);

            tlpEgunLaburpenak = new TableLayoutPanel
            {
                AutoScroll = true,
                BackColor = Color.Transparent,
                ColumnCount = 5,
                Location = new Point(45, 275),
                Name = "tlpEgunLaburpenak",
                Size = new Size(763, 140),
                TabIndex = 30
            };
            tlpEgunLaburpenak.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tlpEgunLaburpenak.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tlpEgunLaburpenak.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpEgunLaburpenak.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpEgunLaburpenak.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));

            panel2.Controls.Add(tlpEgunLaburpenak);
            EgunLaburpenakSortu();
        }

        private void EkintzakPrestatu()
        {
            lblBazkaria.Visible = false;
            lblMota.Visible = false;
            lblDeskribapena.Visible = false;
            dateTimePicker1.Visible = false;
            cbxMota.Visible = false;
            txtDeskribapena.Visible = false;

            lblEkintza.Text = "Ekintzak";
            lblEkintza.Location = new Point(45, 430);

            tlpEkintzak = new TableLayoutPanel
            {
                AutoScroll = true,
                BackColor = Color.Transparent,
                ColumnCount = 5,
                Location = new Point(45, 462),
                Name = "tlpEkintzak",
                Size = new Size(820, 150),
                TabIndex = 31
            };
            tlpEkintzak.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135F));
            tlpEkintzak.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115F));
            tlpEkintzak.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190F));
            tlpEkintzak.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpEkintzak.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42F));

            btnEkintzaGehitu = new Button
            {
                BackColor = AppEstiloa.Urdina,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(884, 462),
                Name = "btnEkintzaGehitu",
                Size = new Size(118, 36),
                Text = "+ Ekintza",
                UseVisualStyleBackColor = false
            };
            btnEkintzaGehitu.FlatAppearance.BorderSize = 0;
            btnEkintzaGehitu.Click += (s, e) => EkintzaLerroaGehitu();

            btnMotaBerria = new Button
            {
                BackColor = AppEstiloa.Berdea,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(884, 505),
                Name = "btnMotaBerria",
                Size = new Size(118, 36),
                Text = "+ Mota",
                UseVisualStyleBackColor = false
            };
            btnMotaBerria.FlatAppearance.BorderSize = 0;
            btnMotaBerria.Click += (s, e) => MotaBerriaGehitu();

            panel2.Controls.Add(tlpEkintzak);
            panel2.Controls.Add(btnEkintzaGehitu);
            panel2.Controls.Add(btnMotaBerria);
            EkintzenTaulaMarraztu();
            EkintzaLerroaGehitu();
        }

        private void MotaBerriaGehitu()
        {
            string mota = Interaction.InputBox(
                "Idatzi ekintza mota berria:",
                "Ekintza berria",
                ""
            );

            if (string.IsNullOrWhiteSpace(mota))
            {
                return;
            }

            if (LisEki.Any(e => e.Mota.Equals(mota, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Mota hori dagoeneko existitzen da.");
                return;
            }

            string deskribapena = Interaction.InputBox(
                "Idatzi ekintzaren deskribapena:",
                "Ekintza berria",
                ""
            );

            EkintzakPlan ekintza = new EkintzakPlan(mota, deskribapena);

            try
            {
                ExelDB.ListakKargatu.EkintzaGehitu(ekintza);
                LisEki.Add(ekintza);
                cbxMota.Items.Add(ekintza.Mota);
                EkintzenMotakEguneratu();

                if (ekintzaKontrolak.Count > 0)
                {
                    EkintzaKontrolak azkena = ekintzaKontrolak.Last();
                    azkena.Mota.Text = ekintza.Mota;
                    azkena.Deskribapena.Text = ekintza.Deskribapena;
                }

                MessageBox.Show("Ekintza berria Driveko exelean gorde da.");
            }
            catch
            {
                MessageBox.Show("Errorea ekintza berria Driveko exelean gordetzean.");
            }
        }

        private void ZerrendaraGehituBotoiaPrestatu()
        {
            btnZerrendaraGehitu = new Button
            {
                BackColor = AppEstiloa.Berdea,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(210, 675),
                Name = "btnZerrendaraGehitu",
                Size = new Size(145, 50),
                Text = "➕ Zerrendara",
                UseVisualStyleBackColor = false
            };
            btnZerrendaraGehitu.FlatAppearance.BorderSize = 0;
            btnZerrendaraGehitu.MouseEnter += (s, e) => btnZerrendaraGehitu.BackColor = AppEstiloa.BerdeaHover;
            btnZerrendaraGehitu.MouseLeave += (s, e) => btnZerrendaraGehitu.BackColor = AppEstiloa.Berdea;
            btnZerrendaraGehitu.Click += (s, e) =>
            {
                UnekoDatuakZerrendetaraGorde();
                FormularioaGarbitu();
                MessageBox.Show("Datuak zerrendan gorde dira.");
            };

            panel2.Controls.Add(btnZerrendaraGehitu);
            btnZerrendaraGehitu.BringToFront();
        }

        private void DatuakIkusiBotoiaPrestatu()
        {
            btnDatuakIkusi = new Button
            {
                BackColor = AppEstiloa.Urdina,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(45, 675),
                Name = "btnDatuakIkusi",
                Size = new Size(145, 50),
                Text = "📋 Datuak ikusi",
                UseVisualStyleBackColor = false
            };
            btnDatuakIkusi.FlatAppearance.BorderSize = 0;
            btnDatuakIkusi.MouseEnter += (s, e) => btnDatuakIkusi.BackColor = AppEstiloa.UrdinaHover;
            btnDatuakIkusi.MouseLeave += (s, e) => btnDatuakIkusi.BackColor = AppEstiloa.Urdina;
            btnDatuakIkusi.Click += (s, e) => DatuakIkusiIreki();

            panel2.Controls.Add(btnDatuakIkusi);
            btnDatuakIkusi.BringToFront();
        }

        private void DatuakIkusiIreki()
        {
            FPlangintzaDatuakIkusi form = new FPlangintzaDatuakIkusi(
                gordetakoBidaiak,
                gordetakoHotelak,
                gordetakoEgunak,
                gordetakoEkintzak,
                KargatuBidaiaEditatzeko
            );
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            panelak.Controls.Add(form);
            form.BringToFront();
            form.Show();
        }

        public void KargatuBidaiaEditatzeko(Bidaiak bidaia)
        {
            if (bidaia.HotelHautatua != null)
            {
                cbxOstala.Text = bidaia.HotelHautatua.Izena;
                hotelaHautatua = bidaia.HotelHautatua;
            }

            bidaiaEditatzen = bidaia;
            nudEgunKop.Value = Math.Min(Math.Max(bidaia.EgunKop, (int)nudEgunKop.Minimum), (int)nudEgunKop.Maximum);
            EgunLaburpenakKargatu(bidaia.EgunLaburpenak);
            EkintzakKargatu(bidaia.EkintzaDatuak);
        }

        private void UnekoDatuakZerrendetaraGorde()
        {
            if (bidaiaEditatzen != null)
            {
                GordetakoBidaiaKendu(bidaiaEditatzen);
            }

            List<ExelDB.ExelaSortu.EgunLaburpena> egunakBerria = new List<ExelDB.ExelaSortu.EgunLaburpena>();
            List<ExelDB.ExelaSortu.EkintzaDatuak> ekintzakBerria = new List<ExelDB.ExelaSortu.EkintzaDatuak>();
            ExelDB.ExelaSortu.HotelDatuak hotelBerria = HotelDatuakSortu();
            gordetakoHotelak.Add(hotelBerria);

            List<string> egunak = EgunAukerak();
            for (int i = 0; i < egunekoLaburpenak.Count; i++)
            {
                EgunekoLaburpenaKontrolak laburpena = egunekoLaburpenak[i];
                string data = string.IsNullOrWhiteSpace(laburpena.Data.Text)
                    ? (i < egunak.Count ? egunak[i] : $"{i + 1}. eguna")
                    : laburpena.Data.Text;

                ExelDB.ExelaSortu.EgunLaburpena eguna = new ExelDB.ExelaSortu.EgunLaburpena(
                    i + 1,
                    data,
                    laburpena.Goisa.Text,
                    laburpena.Arratsaldea.Text,
                    laburpena.Gaua.Text
                );
                egunakBerria.Add(eguna);
                gordetakoEgunak.Add(eguna);
            }

            foreach (EkintzaKontrolak ekintza in ekintzaKontrolak)
            {
                if (string.IsNullOrWhiteSpace(ekintza.Mota.Text)
                    && string.IsNullOrWhiteSpace(ekintza.Deskribapena.Text))
                {
                    continue;
                }

                ExelDB.ExelaSortu.EkintzaDatuak ekintzaBerria = new ExelDB.ExelaSortu.EkintzaDatuak(
                    ekintza.Eguna.Text,
                    ekintza.Ordua.Value.ToShortTimeString(),
                    ekintza.Mota.Text,
                    ekintza.Deskribapena.Text
                );
                ekintzakBerria.Add(ekintzaBerria);
                gordetakoEkintzak.Add(ekintzaBerria);
            }

            Bidaiak bidaiaBerria = new Bidaiak(
                hotelaHautatua ?? new Hotelak(hotelBerria.Hiria, hotelBerria.Izena, hotelBerria.HelbideaUrl),
                (int)nudEgunKop.Value,
                egunakBerria,
                ekintzakBerria
            );
            gordetakoBidaiak.Add(bidaiaBerria);
            LogistikaPlangintzaZubia.BidaiaGorde(bidaiaBerria);
            bidaiaEditatzen = null;
        }

        private void GordetakoBidaiaKendu(Bidaiak bidaia)
        {
            gordetakoBidaiak.Remove(bidaia);
            LogistikaPlangintzaZubia.BidaiaKendu(bidaia);
            ExelDB.ExelaSortu.HotelDatuak? hotela = gordetakoHotelak.FirstOrDefault(h => h.Izena == bidaia.HotelHautatua?.Izena);
            if (hotela != null)
            {
                gordetakoHotelak.Remove(hotela);
            }

            foreach (ExelDB.ExelaSortu.EgunLaburpena eguna in bidaia.EgunLaburpenak)
            {
                gordetakoEgunak.Remove(eguna);
            }

            foreach (ExelDB.ExelaSortu.EkintzaDatuak ekintza in bidaia.EkintzaDatuak)
            {
                gordetakoEkintzak.Remove(ekintza);
            }
        }

        private void FormularioaGarbitu()
        {
            cbxOstala.SelectedIndex = -1;
            cbxOstala.Text = "";
            hotelaHautatua = null;
            bidaiaEditatzen = null;
            txtData.Clear();

            foreach (EgunekoLaburpenaKontrolak laburpena in egunekoLaburpenak)
            {
                laburpena.Data.Clear();
                laburpena.Goisa.Clear();
                laburpena.Arratsaldea.Clear();
                laburpena.Gaua.Clear();
            }

            ekintzaKontrolak.Clear();
            EkintzaLerroaGehitu();
        }

        private ExelDB.ExelaSortu.HotelDatuak HotelDatuakSortu()
        {
            Hotelak? hotela = hotelaHautatua ?? LisHot.FirstOrDefault(h => h.Izena == cbxOstala.Text);
            string data = EgunAukerak().FirstOrDefault() ?? "";

            if (hotela == null)
            {
                return new ExelDB.ExelaSortu.HotelDatuak(
                    "",
                    cbxOstala.Text,
                    "",
                    (int)nudEgunKop.Value,
                    data
                );
            }

            return new ExelDB.ExelaSortu.HotelDatuak(
                hotela.Hiria,
                hotela.Izena,
                hotela.HelbideaUrl,
                (int)nudEgunKop.Value,
                data
            );
        }

        private ExelDB.ExelaSortu.HotelDatuak HotelDatuakGordetzeko()
        {
            return gordetakoHotelak.LastOrDefault() ?? HotelDatuakSortu();
        }

        private void HotelaHautatuaGorde()
        {
            hotelaHautatua = LisHot.FirstOrDefault(h => h.Izena == cbxOstala.Text);
        }

        private void EkintzenTaulaMarraztu()
        {
            tlpEkintzak.Controls.Clear();
            tlpEkintzak.RowStyles.Clear();
            tlpEkintzak.RowCount = ekintzaKontrolak.Count + 1;

            GehituEkintzaGoiburua("Eguna", 0);
            GehituEkintzaGoiburua("Ordua", 1);
            GehituEkintzaGoiburua("Mota", 2);
            GehituEkintzaGoiburua("Deskribapena", 3);

            for (int i = 0; i < ekintzaKontrolak.Count; i++)
            {
                int row = i + 1;
                EkintzaKontrolak ekintza = ekintzaKontrolak[i];
                tlpEkintzak.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
                tlpEkintzak.Controls.Add(ekintza.Eguna, 0, row);
                tlpEkintzak.Controls.Add(ekintza.Ordua, 1, row);
                tlpEkintzak.Controls.Add(ekintza.Mota, 2, row);
                tlpEkintzak.Controls.Add(ekintza.Deskribapena, 3, row);
                tlpEkintzak.Controls.Add(ekintza.Ezabatu, 4, row);
            }

            AppEstiloa.KontrolakAplikatu(tlpEkintzak.Controls);
        }

        private void GehituEkintzaGoiburua(string testua, int column)
        {
            Label label = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                Text = testua,
                TextAlign = ContentAlignment.MiddleLeft
            };
            tlpEkintzak.Controls.Add(label, column, 0);
        }

        private void EkintzaLerroaGehitu()
        {
            EkintzaKontrolak ekintza = new EkintzaKontrolak(
                EgunComboSortu(),
                OrduPickerSortu(),
                MotaComboSortu(),
                EkintzaTextBoxSortu(),
                EzabatuBotoiaSortu()
            );

            ekintza.Mota.SelectedValueChanged += (s, e) => EkintzaDeskribapenaBete(ekintza);
            ekintza.Ezabatu.Click += (s, e) => EkintzaLerroaEzabatu(ekintza);
            ekintzaKontrolak.Add(ekintza);
            EkintzenTaulaMarraztu();
        }

        private ComboBox EgunComboSortu()
        {
            ComboBox combo = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(3)
            };
            combo.Items.AddRange(EgunAukerak().Cast<object>().ToArray());
            if (combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }
            return combo;
        }

        private DateTimePicker OrduPickerSortu()
        {
            return new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Time,
                Margin = new Padding(3),
                ShowUpDown = true
            };
        }

        private ComboBox MotaComboSortu()
        {
            ComboBox combo = new ComboBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };
            combo.Items.AddRange(LisEki.Select(e => e.Mota).Cast<object>().ToArray());
            return combo;
        }

        private TextBox EkintzaTextBoxSortu()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(3),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private Button EzabatuBotoiaSortu()
        {
            Button button = new Button
            {
                BackColor = AppEstiloa.Gorria,
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(3),
                Text = "X",
                UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void EkintzaLerroaEzabatu(EkintzaKontrolak ekintza)
        {
            if (ekintzaKontrolak.Count == 1)
            {
                ekintza.Eguna.SelectedIndex = ekintza.Eguna.Items.Count > 0 ? 0 : -1;
                ekintza.Ordua.Value = DateTime.Now;
                ekintza.Mota.Text = "";
                ekintza.Deskribapena.Text = "";
                return;
            }

            ekintzaKontrolak.Remove(ekintza);
            EkintzenTaulaMarraztu();
        }

        private void EkintzaDeskribapenaBete(EkintzaKontrolak ekintza)
        {
            EkintzakPlan? hautatua = LisEki.FirstOrDefault(e => e.Mota == ekintza.Mota.Text);
            if (hautatua != null)
            {
                ekintza.Deskribapena.Text = hautatua.Deskribapena;
            }
        }

        private void EkintzenMotakEguneratu()
        {
            foreach (EkintzaKontrolak ekintza in ekintzaKontrolak)
            {
                string aukeratua = ekintza.Mota.Text;
                ekintza.Mota.Items.Clear();
                ekintza.Mota.Items.AddRange(LisEki.Select(e => e.Mota).Cast<object>().ToArray());
                ekintza.Mota.Text = aukeratua;
            }
        }

        private void EkintzenEgunakEguneratu()
        {
            List<string> aukerak = EgunAukerak();
            foreach (EkintzaKontrolak ekintza in ekintzaKontrolak)
            {
                string aurrekoa = ekintza.Eguna.Text;
                ekintza.Eguna.Items.Clear();
                ekintza.Eguna.Items.AddRange(aukerak.Cast<object>().ToArray());
                if (aukerak.Contains(aurrekoa))
                {
                    ekintza.Eguna.Text = aurrekoa;
                }
                else if (ekintza.Eguna.Items.Count > 0)
                {
                    ekintza.Eguna.SelectedIndex = 0;
                }
            }
        }

        private List<string> EgunAukerak()
        {
            List<string> aukerak = new List<string>();
            int egunKop = (int)Math.Max(1, nudEgunKop.Value);

            foreach (EgunekoLaburpenaKontrolak laburpena in egunekoLaburpenak)
            {
                if (!string.IsNullOrWhiteSpace(laburpena.Data.Text))
                {
                    aukerak.Add(laburpena.Data.Text);
                }
            }

            if (aukerak.Count > 0)
            {
                for (int i = aukerak.Count + 1; i <= egunKop; i++)
                {
                    aukerak.Add($"{i}. eguna");
                }

                return aukerak.Take(egunKop).ToList();
            }

            if (DateTime.TryParse(txtData.Text, out DateTime hasiera))
            {
                for (int i = 0; i < egunKop; i++)
                {
                    aukerak.Add(hasiera.AddDays(i).ToShortDateString());
                }
            }
            else
            {
                for (int i = 1; i <= egunKop; i++)
                {
                    aukerak.Add($"{i}. eguna");
                }
            }

            return aukerak;
        }

        private void EgunLaburpenakSortu()
        {
            List<(string Data, string Goisa, string Arratsaldea, string Gaua)> aurrekoBalioak = egunekoLaburpenak
                .Select(e => (e.Data.Text, e.Goisa.Text, e.Arratsaldea.Text, e.Gaua.Text))
                .ToList();

            egunekoLaburpenak.Clear();
            tlpEgunLaburpenak.Controls.Clear();
            tlpEgunLaburpenak.RowStyles.Clear();
            tlpEgunLaburpenak.RowCount = (int)nudEgunKop.Value + 1;

            GehituGoiburua("", 0, 0);
            GehituGoiburua("Data", 1, 0);
            GehituGoiburua("Goisa", 2, 0);
            GehituGoiburua("Arratsaldea", 3, 0);
            GehituGoiburua("Gaua", 4, 0);

            for (int i = 0; i < nudEgunKop.Value; i++)
            {
                int row = i + 1;
                tlpEgunLaburpenak.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));

                Label lblEguna = new Label
                {
                    AutoSize = false,
                    Dock = DockStyle.Fill,
                    Text = $"{row}. eguna",
                    TextAlign = ContentAlignment.MiddleLeft
                };

                TextBox txtDataEguna = LaburpenTextBoxSortu();
                TextBox txtGoisa = LaburpenTextBoxSortu();
                TextBox txtArratsaldea = LaburpenTextBoxSortu();
                TextBox txtGaua = LaburpenTextBoxSortu();
                txtDataEguna.TextChanged += (s, e) => EkintzenEgunakEguneratu();

                if (i < aurrekoBalioak.Count)
                {
                    txtDataEguna.Text = aurrekoBalioak[i].Data;
                    txtGoisa.Text = aurrekoBalioak[i].Goisa;
                    txtArratsaldea.Text = aurrekoBalioak[i].Arratsaldea;
                    txtGaua.Text = aurrekoBalioak[i].Gaua;
                }

                tlpEgunLaburpenak.Controls.Add(lblEguna, 0, row);
                tlpEgunLaburpenak.Controls.Add(txtDataEguna, 1, row);
                tlpEgunLaburpenak.Controls.Add(txtGoisa, 2, row);
                tlpEgunLaburpenak.Controls.Add(txtArratsaldea, 3, row);
                tlpEgunLaburpenak.Controls.Add(txtGaua, 4, row);
                egunekoLaburpenak.Add(new EgunekoLaburpenaKontrolak(txtDataEguna, txtGoisa, txtArratsaldea, txtGaua));
            }

            AppEstiloa.KontrolakAplikatu(tlpEgunLaburpenak.Controls);
        }

        private void GehituGoiburua(string testua, int column, int row)
        {
            Label label = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                Text = testua,
                TextAlign = ContentAlignment.MiddleLeft
            };
            tlpEgunLaburpenak.Controls.Add(label, column, row);
        }

        private TextBox LaburpenTextBoxSortu()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(3),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void EgunLaburpenakKargatu(List<ExelDB.ExelaSortu.EgunLaburpena> egunak)
        {
            if (egunekoLaburpenak.Count == 0)
            {
                EgunLaburpenakSortu();
            }

            for (int i = 0; i < egunak.Count && i < egunekoLaburpenak.Count; i++)
            {
                egunekoLaburpenak[i].Data.Text = egunak[i].Data;
                egunekoLaburpenak[i].Goisa.Text = egunak[i].Goisa;
                egunekoLaburpenak[i].Arratsaldea.Text = egunak[i].Arratsaldea;
                egunekoLaburpenak[i].Gaua.Text = egunak[i].Gaua;
            }
        }

        private void EkintzakKargatu(List<ExelDB.ExelaSortu.EkintzaDatuak> ekintzak)
        {
            ekintzaKontrolak.Clear();

            foreach (ExelDB.ExelaSortu.EkintzaDatuak ekintzaDatuak in ekintzak)
            {
                EkintzaKontrolak ekintza = new EkintzaKontrolak(
                    EgunComboSortu(),
                    OrduPickerSortu(),
                    MotaComboSortu(),
                    EkintzaTextBoxSortu(),
                    EzabatuBotoiaSortu()
                );

                ekintza.Mota.SelectedValueChanged += (s, e) => EkintzaDeskribapenaBete(ekintza);
                ekintza.Ezabatu.Click += (s, e) => EkintzaLerroaEzabatu(ekintza);
                ekintza.Eguna.Text = ekintzaDatuak.Eguna;
                if (DateTime.TryParse(ekintzaDatuak.Ordua, out DateTime ordua))
                {
                    ekintza.Ordua.Value = ordua;
                }
                ekintza.Mota.Text = ekintzaDatuak.Mota;
                ekintza.Deskribapena.Text = ekintzaDatuak.Deskribapena;
                ekintzaKontrolak.Add(ekintza);
            }

            if (ekintzaKontrolak.Count == 0)
            {
                EkintzaLerroaGehitu();
                return;
            }

            EkintzenTaulaMarraztu();
        }

        private class EgunekoLaburpenaKontrolak
        {
            public EgunekoLaburpenaKontrolak(TextBox data, TextBox goisa, TextBox arratsaldea, TextBox gaua)
            {
                Data = data;
                Goisa = goisa;
                Arratsaldea = arratsaldea;
                Gaua = gaua;
            }

            public TextBox Data { get; }
            public TextBox Goisa { get; }
            public TextBox Arratsaldea { get; }
            public TextBox Gaua { get; }
        }

        private class EkintzaKontrolak
        {
            public EkintzaKontrolak(ComboBox eguna, DateTimePicker ordua, ComboBox mota, TextBox deskribapena, Button ezabatu)
            {
                Eguna = eguna;
                Ordua = ordua;
                Mota = mota;
                Deskribapena = deskribapena;
                Ezabatu = ezabatu;
            }

            public ComboBox Eguna { get; }
            public DateTimePicker Ordua { get; }
            public ComboBox Mota { get; }
            public TextBox Deskribapena { get; }
            public Button Ezabatu { get; }
        }
    }
}
