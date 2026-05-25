using FeelmwLogistika.Formularioak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using FeelmwLogistika.Logistika.ExelDB;
using FeelmwLogistika.Logistika.DatuModeloak;
using FeelmwLogistika.Plangintza;

namespace FeelmwLogistika
{
    public partial class FMenuNagusia : Form
    {
        private Panel panela;
        public static string? IdEditatuDokumentua;
        public static string? DokIzena;
        public static string Mota = "";
        private List<Ostalak> LisOst = new List<Ostalak>();
        private List<Ekintzak> LisEki = new List<Ekintzak>();
        private List<Garraioak> LisGar = new List<Garraioak>();
        private void FMenuNagusia_Load(object sender, EventArgs e)
        {
            if (IdEditatuDokumentua != null)
            {
                (LisOst, LisEki, LisGar) = DatuakIrakurri.DatuakKargatu();
            }
        }

        public FMenuNagusia(Panel panela)
        {
            LisOst.Clear();
            LisGar.Clear();
            LisEki.Clear();

            InitializeComponent();
            this.panela = panela;

            this.DoubleBuffered = true;
            AppEstiloa.Aplikatu(this);

            AppEstiloa.BotoiNeutro(btnOstala);
            AppEstiloa.BotoiNeutro(btnEkintza);
            AppEstiloa.BotoiNeutro(btnGarraioa);
            AppEstiloa.BotoiNeutro(btnIkudi);
            AppEstiloa.BotoiGorde(btnGorde);
            AppEstiloa.BotoiNagusia(btnSortu);
            AppEstiloa.BotoiIrten(btnIrten);

        }

        private void btnOstala_Click(object sender, EventArgs e)
        {
            FOstala fo = new FOstala();
            fo.TopLevel = false;
            fo.Dock = DockStyle.Fill;
            panela.Controls.Add(fo);
            fo.BringToFront();
            fo.Show();

            fo.FormClosed += (s, args) =>
            {
                Ostalak? ost = fo.Ost;
                if (ost != null)
                {
                    LisOst.Add(ost);
                    LogistikaPlangintzaZubia.OstalaGorde(ost);
                }

                panela.Controls.Remove(fo);
            };
        }

        private void btnEkintza_Click(object sender, EventArgs e)
        {
            FEkintza fe = new FEkintza();
            fe.TopLevel = false;
            fe.Dock = DockStyle.Fill;
            panela.Controls.Add(fe);
            fe.BringToFront();
            fe.Show();

            fe.FormClosed += (s, args) =>
            {
                Ekintzak? ekintza = fe.Eki;
                if (ekintza != null)
                {
                    LisEki.Add(ekintza);
                }

                panela.Controls.Remove(fe);
            };
        }

        private void btnGarraioa_Click(object sender, EventArgs e)
        {
            FGarraioa fg = new FGarraioa();
            fg.TopLevel = false;
            fg.Dock = DockStyle.Fill;
            panela.Controls.Add(fg);
            fg.BringToFront();
            fg.Show();

            fg.FormClosed += (s, args) =>
            {
                Garraioak? garraioa = fg.Gar;
                if (garraioa != null)
                {
                    LisGar.Add(garraioa);
                }

                panela.Controls.Remove(fg);
            };
        }

        private void btnIrten_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            this.Close();
        }

        private void btnGorde_Click(object sender, EventArgs e)
        {
            string nombre;
            if (IdEditatuDokumentua == null)
            {
                nombre = Microsoft.VisualBasic.Interaction.InputBox(
                "Idatzi dokumentuaren izena:",
                "Dokumentu berria",
                "MyFeelLogistika"
                );
            }
            else
            {
                nombre = Microsoft.VisualBasic.Interaction.InputBox(
                "Idatzi dokumentuaren izena:",
                "Dokumentu berria",
                DokIzena ?? ""
                );
            }


            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Dokumentuak izen bat behar du 😅");
                return;
            }

            try
            {
                GordeLogistikaEtaPlangintza(nombre);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea exela sortzean, datuak ez dira gorde: " + ex.Message);
            }
        }

        private void GordeLogistikaEtaPlangintza(string nombre)
        {
            string sheetId = Exelak.ExelBerriaSortu(nombre);
            if (LisOst != null)
            {
                LogistikaPlangintzaZubia.OstalakGorde(LisOst);
                Exelak.OstalakIdatzi(sheetId, LisOst);
            }
            if (LisEki != null)
            {
                Exelak.EkintzakIdatzi(sheetId, LisEki);
            }
            if (LisGar != null)
            {
                Exelak.GarraioakIdatzi(sheetId, LisGar);
            }

            (bool plangintzaSortuta, string plangintzaMezua) = LogistikaPlangintzaZubia.SortuPlangintzaDokumentua(nombre);
            string mezua = plangintzaSortuta
                ? "Logistika eta Plangintza excelak sortuta."
                : $"Logistika exela sortuta.\n{plangintzaMezua}";
            MessageBox.Show(mezua);
        }

        private void btnSortu_Click(object sender, EventArgs e)
        {
            string nombre;
                if(IdEditatuDokumentua == null)
                {
                    nombre = Microsoft.VisualBasic.Interaction.InputBox(
                    "Idatzi dokumentuaren izena:",
                    "Dokumentu berria",
                    ""
                    );
                }
            else
            {
                nombre = Microsoft.VisualBasic.Interaction.InputBox(
               "Idatzi dokumentuaren izena:",
               "Dokumentu berria",
               DokIzena ?? ""
               );
            }
                
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Dokumentuak izen bat behar du 😅");
                return;
            }

            DokumentuaSortu.Sortu(LisOst, LisEki, LisGar, nombre);

            if (File.Exists(IdEditatuDokumentua))
            {
                File.Delete(IdEditatuDokumentua);
            }
            MessageBox.Show("Dokumentua mahi gainean sortu da. ");
            this.Close();
        }

        private void btnIkusi_Click(object sender, EventArgs e)
        {
            FDatuakIkusi fd = new FDatuakIkusi(LisOst, LisEki, LisGar, panela);
            fd.TopLevel = false;
            fd.Dock = DockStyle.Fill;
            panela.Controls.Add(fd);
            fd.BringToFront();
            fd.Show();

        }

        private void LimpiarFormulario()
        {
            LisOst.Clear();
            LisEki.Clear();
            LisGar.Clear();
        }
    }
}
