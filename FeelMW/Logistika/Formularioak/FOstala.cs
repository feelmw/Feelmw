using FeelmwLogistika.Logistika.DatuModeloak;
using FeelmwLogistika.Logistika.ExelDB;
using FeelmwLogistika.Plangintza;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FeelmwLogistika.Formularioak
{
    public partial class FOstala : Form
    {
        public Ostalak Ost;
        private List<Ostalak> LisOst = new List<Ostalak>();
        private readonly Ostalak? ostalaEditatzen;
        private bool EsKlasikoa => string.Equals(FMenuNagusia.Mota, "Klasikoa", StringComparison.Ordinal)
            || ostalaEditatzen?.EsKlasikoa == true;

        public FOstala()
        {
            InitializeComponent();

            cbxOstala.Focus();
            txtLokali.Text = "Feelmw";

            this.DoubleBuffered = true;
            AppEstiloa.Aplikatu(this);

            btnGorde.MouseEnter += (s, e) => btnGorde.BackColor = AppEstiloa.UrdinaHover;
            btnGorde.MouseLeave += (s, e) => btnGorde.BackColor = AppEstiloa.Urdina;

            btnGehitu.MouseEnter += (s, e) => btnGehitu.BackColor = AppEstiloa.BerdeaHover;
            btnGehitu.MouseLeave += (s, e) => btnGehitu.BackColor = AppEstiloa.Berdea;

            btnIrten.MouseEnter += (s, e) => btnIrten.BackColor = AppEstiloa.GorriaHover;
            btnIrten.MouseLeave += (s, e) => btnIrten.BackColor = AppEstiloa.Gorria;

            AplicarModoKlasikoa();
        }

        public FOstala(Ostalak ostala) : this()
        {
            ostalaEditatzen = ostala;
            btnGehitu.Visible = false;
            lblIzenburua.Text = "Ostatua editatu";
        }

        private void FOstala_Load(object sender, EventArgs e)
        {
            try
            {
                this.LisOst = DatuakIrakurri.OstalakListaratu();
            }
            catch (Exception ex)
            {
                this.LisOst = new List<Ostalak>();
                MessageBox.Show("Ezin izan dira ostatuak kargatu: " + ex.Message);
            }

            if (this.LisOst.Count > 0)
            {
                this.LisOst.RemoveAt(0);
            }

            foreach (Ostalak ost in LisOst)
            {
                cbxOstala.Items.Add(ost.OstalaIzena);
            }

            if (ostalaEditatzen != null)
            {
                OstalaKargatu(ostalaEditatzen);
            }

            AplicarModoKlasikoa();
        }

        private void btnIrten_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGorde_Click(object sender, EventArgs e)
        {
            Ost = OstalaSortu();
            LogistikaPlangintzaZubia.OstalaGorde(Ost);

            this.Close();
        }

        private void cbxOstala_SelectedValueChanged(object sender, EventArgs e)
        {
            foreach (Ostalak ostala in LisOst)
            {
                if (ostala.OstalaIzena == cbxOstala.Text)
                {
                    txtBonoa.Text = ostala.Bonoa;
                    txtHelbidea.Text = ostala.Helbidea;
                    txtIn.Text = ostala.Checkin;
                    txtOut.Text = ostala.Checkout;
                    txtDoku.Text = ostala.Dokumentazioa;
                    txtHarrera.Text = ostala.Harrera;
                    chkLuggage.Checked = ostala.Luggage;
                    txtLuggage.Text = ostala.LuggageKuota;
                    chkToallak.Checked = ostala.Toailak;
                    chkIzarak.Checked = ostala.Izarak;
                    txtInst.Text = ostala.Instalazioak;
                }
            }
        }

        private void btnGehitu_Click(object sender, EventArgs e)
        {
            Ost = OstalaSortu();
            LogistikaPlangintzaZubia.OstalaGorde(Ost);

            if (string.IsNullOrWhiteSpace(Ost.OstalaIzena))
            {
                MessageBox.Show("Ostatuak izen bat behar du.");
                return;
            }

            if (!LisOst.Any(o => o.OstalaIzena.Equals(Ost.OstalaIzena, StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    DatuakIrakurri.OstalaGehitu(Ost);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ezin izan da ostatua Driveko exelean gorde: " + ex.Message);
                    return;
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Izen hori duen ostal bat dagoeneko existitzen da. Mesedez, ziurtatu datuak zuzenak direla.");
            }
        }
        private Ostalak OstalaSortu()
        {
            string lokalizatzailea = EsKlasikoa ? string.Empty : txtLokali.Text;
            int gauak = EsKlasikoa ? 0 : (int)nudGauak.Value;
            string datak = EsKlasikoa ? string.Empty : datHasiera.Value.ToShortDateString() + " - " + datAmaiera.Value.ToShortDateString();
            string gelak = EsKlasikoa ? string.Empty : txtGelak.Text;

            Ostalak ostala = new Ostalak(
                cbxOstala.Text,
                txtBonoa.Text,
                txtHelbidea.Text,
                lokalizatzailea,
                gauak,
                datak,
                gelak,
                txtIn.Text,
                txtOut.Text,
                txtDoku.Text,
                txtHarrera.Text,
                txtGosaria.Text,
                txtBazkaria.Text,
                txtAfaria.Text,
                chkToallak.Checked,
                chkIzarak.Checked,
                chkFidantza.Checked,
                txtFidantza.Text,
                chkLuggage.Checked,
                txtLuggage.Text,
                txtInst.Text,
                EsKlasikoa
            );

            return ostala;
        }

        private void AplicarModoKlasikoa()
        {
            bool erakutsi = !EsKlasikoa;

            lblGauak.Visible = erakutsi;
            lblData.Visible = erakutsi;
            lblLokali.Visible = erakutsi;
            lblGelak.Visible = erakutsi;
            nudGauak.Visible = erakutsi;
            txtGelak.Visible = erakutsi;
            txtLokali.Visible = erakutsi;
            datHasiera.Visible = erakutsi;
            datAmaiera.Visible = erakutsi;
        }

        private void OstalaKargatu(Ostalak ostala)
        {
            cbxOstala.Text = ostala.OstalaIzena;
            txtBonoa.Text = ostala.Bonoa;
            txtHelbidea.Text = ostala.Helbidea;
            txtLokali.Text = ostala.Lokalizatzailea;
            nudGauak.Value = Math.Min(Math.Max(ostala.Gauak, (int)nudGauak.Minimum), (int)nudGauak.Maximum);
            DatakKargatu(ostala.Datak);
            txtGelak.Text = ostala.Gelak;
            txtIn.Text = ostala.Checkin;
            txtOut.Text = ostala.Checkout;
            txtDoku.Text = ostala.Dokumentazioa;
            txtHarrera.Text = ostala.Harrera;
            txtGosaria.Text = ostala.Gosariates;
            txtBazkaria.Text = ostala.Bazkariates;
            txtAfaria.Text = ostala.Afariates;
            chkToallak.Checked = ostala.Toailak;
            chkIzarak.Checked = ostala.Izarak;
            chkFidantza.Checked = ostala.Fidantza;
            txtFidantza.Text = ostala.FidantzaKuota;
            chkLuggage.Checked = ostala.Luggage;
            txtLuggage.Text = ostala.LuggageKuota;
            txtInst.Text = ostala.Instalazioak;
        }

        private void DatakKargatu(string datak)
        {
            if (string.IsNullOrWhiteSpace(datak))
            {
                return;
            }

            string[] zatiak = datak.Split(" - ");
            if (zatiak.Length > 0 && DateTime.TryParse(zatiak[0], out DateTime hasiera))
            {
                datHasiera.Value = hasiera;
            }

            if (zatiak.Length > 1 && DateTime.TryParse(zatiak[1], out DateTime amaiera))
            {
                datAmaiera.Value = amaiera;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbxOstala_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
