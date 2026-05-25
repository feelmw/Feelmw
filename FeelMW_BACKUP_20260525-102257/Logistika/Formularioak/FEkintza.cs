using FeelmwLogistika.Logistika.DatuModeloak;
using FeelmwLogistika.Logistika.ExelDB;
using FeelmwLogistika;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FeelmwLogistika.Formularioak
{
    public partial class FEkintza : Form
    {
        public Ekintzak? Eki;
        public List<Ekintzak> LisEki = new List<Ekintzak>();
        private readonly Ekintzak? ekintzaEditatzen;
        public FEkintza()
        {
            InitializeComponent();

            DoubleBuffered = true;
            AppEstiloa.Aplikatu(this);

            AppEstiloa.BotoiGorde(btnGorde);
            AppEstiloa.BotoiGehitu(btnGehitu);
            AppEstiloa.BotoiIrten(btnIrten);

            txtLokali.Text = BalioLehenetsiak.Lokalizatzailea;
        }

        public FEkintza(Ekintzak ekintza) : this()
        {
            ekintzaEditatzen = ekintza;
            btnGehitu.Visible = false;
            lblIzenburua.Text = "Ekintza editatu";
        }

        private void btnIrten_Click(object sender, EventArgs e)
        {
            if (ekintzaEditatzen == null)
            {
                LimpiarFormulario();
            }

            this.Close();
        }

        private void btnGorde_Click(object sender, EventArgs e)
        {
            Eki = EkintzaSortu();

            this.Close();
        }

        private void FEkintza_Load(object sender, EventArgs e)
        {
            if (ekintzaEditatzen == null)
            {
                LimpiarFormulario();
            }

            try
            {
                this.LisEki = DatuakIrakurri.EkintzakListaratu();
            }
            catch (Exception ex)
            {
                this.LisEki = new List<Ekintzak>();
                MessageBox.Show("Ezin izan dira ekintzak kargatu: " + ex.Message);
            }

            if (LisEki.Count > 0)
            {
                LisEki.RemoveAt(0);
            }

            foreach (Ekintzak eki in LisEki)
            {
                cbxEkintza.Items.Add(eki.EkintzaIzena);
            }

            if (ekintzaEditatzen != null)
            {
                EkintzaKargatu(ekintzaEditatzen);
            }

            cbxEkintza.Focus();
        }

        private void LimpiarFormulario()
        {
            Eki = null;
            cbxEkintza.Items.Clear();
            cbxEkintza.SelectedIndex = -1;
            cbxEkintza.Text = "";
            txtBonoa.Clear();
            txtIraupena.Clear();
            txtKontaktua.Clear();
            txtElkartokia.Clear();
            txtIristean.Clear();
            txtEramanM.Clear();
            txtBertanM.Clear();
            chkAldagela.Checked = false;
            chkKomuna.Checked = false;
            txtEgonlekua.Clear();
            txtInfo.Clear();
            txtLokali.Text = BalioLehenetsiak.Lokalizatzailea;
        }

        private Ekintzak EkintzaSortu()
        {
            Ekintzak ekin = new Ekintzak(
                cbxEkintza.Text,
                txtBonoa.Text,
                txtIraupena.Text,
                txtKontaktua.Text,
                txtElkartokia.Text,
                txtIristean.Text,
                txtEramanM.Text,
                txtBertanM.Text,
                chkAldagela.Checked,
                chkKomuna.Checked,
                txtEgonlekua.Text,
                txtInfo.Text,
                LokaliBalioa()
            );
            return ekin;
        }

        private string LokaliBalioa()
        {
            return string.IsNullOrWhiteSpace(txtLokali.Text) ? BalioLehenetsiak.Lokalizatzailea : txtLokali.Text;
        }

        private void btnGehitu_Click(object sender, EventArgs e)
        {
            Eki = EkintzaSortu();

            if (string.IsNullOrWhiteSpace(Eki.EkintzaIzena))
            {
                MessageBox.Show("Ekintzak izen bat behar du.");
                return;
            }

            if (!LisEki.Any(e => e.EkintzaIzena.Equals(Eki.EkintzaIzena, StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    DatuakIrakurri.EkintzaGehitu(Eki);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ezin izan da ekintza Driveko exelean gorde: " + ex.Message);
                    return;
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Izen hori duen ekintza bat dagoeneko existitzen da. Mesedez, ziurtatu datuak zuzenak direla.");
            }
        }

        private void cbxEkintza_SelectedValueChanged(object sender, EventArgs e)
        {

            foreach (Ekintzak ekintza in LisEki)
            {
                if (ekintza.EkintzaIzena == cbxEkintza.Text)
                {
                    txtBonoa.Text = ekintza.Bonoa;
                    txtIraupena.Text = ekintza.Iraupena;
                    txtIristean.Text = ekintza.Iristean;
                    txtInfo.Text = ekintza.Informazioa;
                    txtBertanM.Text = ekintza.BertanM;
                    txtEramanM.Text = ekintza.EramanM;
                    txtKontaktua.Text = ekintza.Kontaktua;
                    txtElkartokia.Text = ekintza.Elkartokia;
                    txtEgonlekua.Text = ekintza.Egonlekua;
                    txtLokali.Text = ekintza.Lokali;

                    chkAldagela.Checked = ekintza.Aldagela;
                    chkKomuna.Checked = ekintza.Komuna;
                }
            }
        }

        private void EkintzaKargatu(Ekintzak ekintza)
        {
            cbxEkintza.Text = ekintza.EkintzaIzena;
            txtBonoa.Text = ekintza.Bonoa;
            txtIraupena.Text = ekintza.Iraupena;
            txtKontaktua.Text = ekintza.Kontaktua;
            txtElkartokia.Text = ekintza.Elkartokia;
            txtIristean.Text = ekintza.Iristean;
            txtEramanM.Text = ekintza.EramanM;
            txtBertanM.Text = ekintza.BertanM;
            chkAldagela.Checked = ekintza.Aldagela;
            chkKomuna.Checked = ekintza.Komuna;
            txtEgonlekua.Text = ekintza.Egonlekua;
            txtInfo.Text = ekintza.Informazioa;
            txtLokali.Text = ekintza.Lokali;
        }
    }
}
