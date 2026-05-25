using FeelmwLogistika.Logistika.DatuModeloak;
using FeelmwLogistika.Logistika.ExelDB;
using FeelmwLogistika.Plangintza;
using FeelmwLogistika.Plangintza.Formularioak;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FeelmwLogistika
{
    public partial class FLogina : Form
    {
        private List<SheetInfo> sheets = new List<SheetInfo>();
        private string motaHelburua = "";
        public static string DokumentazioaMota = "";

        private void FLogina_Load(object sender, EventArgs e)
        {
            HasierakoMenuaErakutsi();
        }

        public FLogina()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            AppEstiloa.Aplikatu(this);

            AppEstiloa.BotoiNagusia(btnLogistika);
            AppEstiloa.BotoiNagusia(btnEditatu);
            AppEstiloa.BotoiNagusia(btnPlangintza);
            AppEstiloa.BotoiNagusia(btnDokumentazioa);
            btnDokumentazioa.Click += btnDokumentazioa_Click;

            AppEstiloa.BotoiIrten(btnIrten);
            AppEstiloa.BotoiNagusia(btnJarraitu);

        }

        private void btnLogistika_Click(object sender, EventArgs e)
        {
            MotaAukeraErakutsi("Logistika");
        }

        private void btnDokumentazioa_Click(object? sender, EventArgs e)
        {
            MotaAukeraErakutsi("Dokumentazioa");
        }

        private void btnPlangintza_Click(object sender, EventArgs e)
        {
            MotaAukeraErakutsi("Plangintza");
        }

        private void MotaAukeraErakutsi(string helburua)
        {
            motaHelburua = helburua;
            lblMota.Visible = true;
            cbxMota.Visible = true;
            btnJarraitu.Visible = true;
            btnLogistika.Visible = false;
            btnPlangintza.Visible = false;
            btnDokumentazioa.Visible = false;
            btnEditatu.Visible = false;
            btnIrten.Visible = true;
            cbxMota.SelectedIndex = -1;
            cbxMota.Focus();
            lblMota.BringToFront();
            cbxMota.BringToFront();
            btnJarraitu.BringToFront();
            btnIrten.BringToFront();
        }

        private void btnJarraitu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(motaHelburua) || cbxMota.SelectedItem == null)
            {
                MessageBox.Show("Aukeratu bidaia mota jarraitu aurretik.");
                return;
            }

            string mota = cbxMota.SelectedItem.ToString() ?? "";
            string helburua = motaHelburua;
            motaHelburua = "";

            if (helburua == "Logistika")
            {
                FMenuNagusia.Mota = mota;
                FPlangintza.Mota = "";
                LogistikaBerriaIreki();
            }
            else if (helburua == "Plangintza")
            {
                FMenuNagusia.Mota = "";
                FPlangintza.Mota = mota;
                PlangintzaBerriaIreki();
            }
            else if (helburua == "Dokumentazioa")
            {
                FMenuNagusia.Mota = "";
                FPlangintza.Mota = "";
                DokumentazioaMota = mota;
                HasierakoMenuaErakutsi();
            }
        }

        private void LogistikaBerriaIreki()
        {
            LogistikaPlangintzaZubia.Garbitu();
            FMenuNagusia.IdEditatuDokumentua = null;
            FMenuNagusia.DokIzena = null;
            FPlangintza.IdEditatuDokumentua = null;
            FPlangintza.DokIzena = null;
            FMenuNagusia fmn = new FMenuNagusia(panelak);
            FormularioaIreki(fmn);
        }

        private void btnIrten_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(motaHelburua))
            {
                HasierakoMenuaErakutsi();
            }
            else if (btnEditatu.Text == "📄 Editatu dok.")
            {
                this.Close();
            }
            else
            {
                btnLogistika.Visible = true;
                btnPlangintza.Visible = true;
                btnDokumentazioa.Visible = true;
                lblDoku.Visible = false;
                cbxDoku.Visible = false;
                lblMota.Visible = false;
                cbxMota.Visible = false;
                btnJarraitu.Visible = false;
                btnEditatu.Text = "📄 Editatu dok.";
                FMenuNagusia.IdEditatuDokumentua = null;
                FMenuNagusia.DokIzena = null;
                FMenuNagusia.Mota = "";
                FPlangintza.IdEditatuDokumentua = null;
                FPlangintza.DokIzena = null;
                FPlangintza.Mota = "";
                motaHelburua = "";
            }

        }

        private void btnEditatu_Click(object sender, EventArgs e)
        {
            if (btnEditatu.Text == "📄 Editatu dok.")
            {
                btnLogistika.Visible = false;
                btnPlangintza.Visible = false;
                btnDokumentazioa.Visible = false;
                lblDoku.Visible = true;
                cbxDoku.Visible = true;
                cbxDoku.Focus();
                sheets = DatuakIrakurri.SheetsListaratu();

                cbxDoku.Items.Clear();

                foreach (SheetInfo s in sheets)
                {
                    cbxDoku.Items.Add(s);
                }

                btnEditatu.Text = "📄 Editatu";
            }
            else
            {
                SheetInfo? aukeratua = cbxDoku.SelectedItem as SheetInfo;
                if (aukeratua == null)
                {
                    MessageBox.Show("Aukeratu editatu nahi duzun dokumentua lehenengo 😅");
                    return;
                }

                DokumentuaAukeratu(aukeratua);

                lblDoku.Visible = false;
                cbxDoku.Visible = false;

                if (LogistikaDokumentuaDa(aukeratua.Nombre))
                {
                    LogistikaEditatuIreki();
                }
                else if (PlangintzaDokumentuaDa(aukeratua.Nombre))
                {
                    PlangintzaEditatuIreki();
                }
                else
                {
                    MessageBox.Show("Dokumentuaren izenak 'Logistika -' edo 'Plangintza -' hasi behar du.");
                    lblDoku.Visible = true;
                    cbxDoku.Visible = true;
                    return;
                }

                btnEditatu.Text = "📄 Editatu dok.";
            }
        }

        private void cbxDoku_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (SheetInfo s in sheets)
            {
                if (s.Nombre == cbxDoku.Text)
                {
                    DokumentuaAukeratu(s);
                }
            }


        }

        private void PlangintzaBerriaIreki()
        {
            FMenuNagusia.IdEditatuDokumentua = null;
            FMenuNagusia.DokIzena = null;
            FMenuNagusia.Mota = "";
            FPlangintza.IdEditatuDokumentua = null;
            FPlangintza.DokIzena = null;
            FPlangintza.Mota = "";
            FPlangintza fp = new FPlangintza(panelak);
            FormularioaIreki(fp);
        }

        private void LogistikaEditatuIreki()
        {
            FMenuNagusia fmn = new FMenuNagusia(panelak);
            FormularioaIreki(fmn);
        }

        private void PlangintzaEditatuIreki()
        {
            FPlangintza fp = new FPlangintza(panelak);
            FormularioaIreki(fp);
        }

        private void FormularioaIreki(Form form)
        {
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            panelak.Controls.Add(form);
            form.BringToFront();
            form.Show();

            lblMyfeel.Visible = false;
            lblMota.Visible = false;
            cbxMota.Visible = false;
            btnJarraitu.Visible = false;
            btnLogistika.Visible = false;
            btnEditatu.Visible = false;
            btnIrten.Visible = false;
            btnPlangintza.Visible = false;
            btnDokumentazioa.Visible = false;

            form.FormClosed += (s, args) =>
            {
                panelak.Controls.Remove(form);
                HasierakoMenuaErakutsi();
            };
        }

        private void HasierakoMenuaErakutsi()
        {
            motaHelburua = "";
            lblMyfeel.Visible = true;
            lblMota.Visible = false;
            cbxMota.Visible = false;
            btnJarraitu.Visible = false;
            lblDoku.Visible = false;
            cbxDoku.Visible = false;
            btnLogistika.Visible = true;
            btnEditatu.Visible = true;
            btnIrten.Visible = true;
            btnPlangintza.Visible = true;
            btnDokumentazioa.Visible = true;
            btnEditatu.Text = "📄 Editatu dok.";
            cbxMota.SelectedIndex = -1;
        }

        private void DokumentuaAukeratu(SheetInfo sheet)
        {
            FMenuNagusia.IdEditatuDokumentua = null;
            FMenuNagusia.DokIzena = null;
            FMenuNagusia.Mota = "";
            FPlangintza.IdEditatuDokumentua = null;
            FPlangintza.DokIzena = null;
            FPlangintza.Mota = "";

            if (LogistikaDokumentuaDa(sheet.Nombre))
            {
                FMenuNagusia.IdEditatuDokumentua = sheet.Id;
                FMenuNagusia.DokIzena = DokumentuIzenaGarbitu(sheet.Nombre, "Logistika");
            }
            else if (PlangintzaDokumentuaDa(sheet.Nombre))
            {
                FPlangintza.IdEditatuDokumentua = sheet.Id;
                FPlangintza.DokIzena = DokumentuIzenaGarbitu(sheet.Nombre, "Plangintza");
            }
        }

        private bool LogistikaDokumentuaDa(string izena)
        {
            return izena.StartsWith("Logistika -", StringComparison.OrdinalIgnoreCase)
                || izena.StartsWith("Logistoka -", StringComparison.OrdinalIgnoreCase);
        }

        private bool PlangintzaDokumentuaDa(string izena)
        {
            return izena.StartsWith("Plangintza -", StringComparison.OrdinalIgnoreCase);
        }

        private string DokumentuIzenaGarbitu(string izena, string mota)
        {
            string prefix = mota + " -";
            return izena.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                ? izena.Substring(prefix.Length).Trim()
                : izena;
        }
    }
}
