using FeelmwLogistika.Logistika.DatuModeloak;
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
    public partial class FGarraioa : Form
    {
        public Garraioak Gar;
        private readonly Garraioak? garraioaEditatzen;
        public FGarraioa()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            AppEstiloa.Aplikatu(this);

            btnGorde.MouseEnter += (s, e) => btnGorde.BackColor = AppEstiloa.UrdinaHover;
            btnGorde.MouseLeave += (s, e) => btnGorde.BackColor = AppEstiloa.Urdina;

            btnIrten.MouseEnter += (s, e) => btnIrten.BackColor = AppEstiloa.GorriaHover;
            btnIrten.MouseLeave += (s, e) => btnIrten.BackColor = AppEstiloa.Gorria;
        }

        public FGarraioa(Garraioak garraioa) : this()
        {
            garraioaEditatzen = garraioa;
            lblIzenburua.Text = "Garraioa editatu";
        }

        private void FGarraioa_Load(object sender, EventArgs e)
        {
            if (garraioaEditatzen != null)
            {
                GarraioaKargatu(garraioaEditatzen);
            }
        }

        private void btnIrten_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGorde_Click(object sender, EventArgs e)
        {
            Gar = new Garraioak(
                txtGarraioa.Text,
                dtpEguna.Value.ToShortDateString(),
                txtOrdutegia.Text,
                txtLokali.Text,
                txtkontaktua.Text,
                txtElkartokia.Text,
                txtEginbeharrak.Text,
                txtInfo.Text
                );

            this.Close();
        }

        private void GarraioaKargatu(Garraioak garraioa)
        {
            txtGarraioa.Text = garraioa.GarraioaIzena;
            if (DateTime.TryParse(garraioa.Eguna, out DateTime eguna))
            {
                dtpEguna.Value = eguna;
            }
            txtOrdutegia.Text = garraioa.Ordutegia;
            txtLokali.Text = garraioa.Lokalizatzailea;
            txtkontaktua.Text = garraioa.Kontaktua;
            txtElkartokia.Text = garraioa.Elkargunea;
            txtEginbeharrak.Text = garraioa.Eginbeharrak;
            txtInfo.Text = garraioa.Informazioa;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
