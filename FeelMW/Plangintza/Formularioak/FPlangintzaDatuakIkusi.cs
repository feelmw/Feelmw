using FeelmwLogistika.Plangintza.DatuModeloak;
using FeelmwLogistika.Plangintza.ExelDB;

namespace FeelmwLogistika.Plangintza.Formularioak
{
    public partial class FPlangintzaDatuakIkusi : Form
    {
        private readonly List<Bidaiak> bidaiak;
        private readonly List<ExelaSortu.HotelDatuak> hotelak;
        private readonly List<ExelaSortu.EgunLaburpena> egunak;
        private readonly List<ExelaSortu.EkintzaDatuak> ekintzak;
        private readonly Action<Bidaiak> bidaiaEditatu;
        private readonly ComboBox cbxMota;
        private readonly DataGridView dgvDatuak;

        public FPlangintzaDatuakIkusi(
            List<Bidaiak> bidaiak,
            List<ExelaSortu.HotelDatuak> hotelak,
            List<ExelaSortu.EgunLaburpena> egunak,
            List<ExelaSortu.EkintzaDatuak> ekintzak,
            Action<Bidaiak> bidaiaEditatu)
        {
            this.bidaiak = bidaiak;
            this.hotelak = hotelak;
            this.egunak = egunak;
            this.ekintzak = ekintzak;
            this.bidaiaEditatu = bidaiaEditatu;

            BackColor = AppEstiloa.Fondoa;
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;
            DoubleBuffered = true;

            Label lblIzenburua = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold),
                ForeColor = AppEstiloa.TestuNagusia,
                Location = new Point(49, 18),
                Text = "Plangintza datuak"
            };

            Panel edukia = new Panel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = AppEstiloa.Txartela,
                Location = new Point(49, 92),
                Padding = new Padding(24),
                Size = new Size(1080, 620)
            };

            Label lblMota = new Label
            {
                AutoSize = true,
                Location = new Point(24, 24),
                Text = "Mota"
            };

            cbxMota = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(24, 50),
                Size = new Size(220, 31)
            };
            cbxMota.Items.AddRange(new object[] { "Guztiak", "Hotelak", "Egunak", "Ekintzak" });
            cbxMota.SelectedIndex = 0;
            cbxMota.SelectedIndexChanged += (s, e) => DatuakKargatu();

            Button btnEditatu = BotoiaSortu("Editatu", AppEstiloa.Urdina, new Point(596, 48));
            btnEditatu.Click += btnEditatu_Click;

            Button btnEzabatu = BotoiaSortu("Ezabatu", AppEstiloa.Gorria, new Point(736, 48));
            btnEzabatu.Click += btnEzabatu_Click;

            Button btnIrten = BotoiaSortu("Irten", AppEstiloa.Gorria, new Point(876, 48));
            btnIrten.Click += (s, e) => Close();

            dgvDatuak = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                Location = new Point(24, 106),
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Size = new Size(1030, 486)
            };
            ZutabeakEzarri("Guztiak");

            edukia.Controls.Add(lblMota);
            edukia.Controls.Add(cbxMota);
            edukia.Controls.Add(btnEditatu);
            edukia.Controls.Add(btnEzabatu);
            edukia.Controls.Add(btnIrten);
            edukia.Controls.Add(dgvDatuak);
            Controls.Add(lblIzenburua);
            Controls.Add(edukia);

            AppEstiloa.Aplikatu(this);
            DatuakKargatu();
        }

        private Button BotoiaSortu(string testua, Color kolorea, Point kokapena)
        {
            Color hoverKolorea = kolorea == AppEstiloa.Gorria ? AppEstiloa.GorriaHover : AppEstiloa.UrdinaHover;
            return AppEstiloa.BotoiaSortu(testua, kolorea, hoverKolorea, kokapena, new Size(120, 36));
        }

        private void DatuakKargatu()
        {
            dgvDatuak.Rows.Clear();
            string mota = cbxMota.SelectedItem?.ToString() ?? "Guztiak";
            ZutabeakEzarri(mota);

            if (mota == "Guztiak" || mota == "Hotelak")
            {
                foreach (Bidaiak bidaia in bidaiak)
                {
                    ExelaSortu.HotelDatuak hotela = HotelDatuakSortu(bidaia);
                    DataGridViewRow row = RowSortu(hotela.Izena, hotela.EgunKop.ToString(), hotela.Data, hotela.Hiria);
                    row.Tag = new DatuFila(bidaia, hotela);
                    dgvDatuak.Rows.Add(row);
                }
            }

            if (mota == "Guztiak" || mota == "Egunak")
            {
                foreach (Bidaiak bidaia in bidaiak)
                {
                    foreach (ExelaSortu.EgunLaburpena eguna in bidaia.EgunLaburpenak)
                    {
                        DataGridViewRow row = RowSortu(
                            "Eguna",
                            eguna.Eguna.ToString(),
                            eguna.Data,
                            $"G: {eguna.Goisa} | A: {eguna.Arratsaldea} | Ga: {eguna.Gaua}"
                        );
                        row.Tag = new DatuFila(bidaia, eguna);
                        dgvDatuak.Rows.Add(row);
                    }
                }
            }

            if (mota == "Guztiak" || mota == "Ekintzak")
            {
                foreach (Bidaiak bidaia in bidaiak)
                {
                    foreach (ExelaSortu.EkintzaDatuak ekintza in bidaia.EkintzaDatuak)
                    {
                        DataGridViewRow row = mota == "Ekintzak"
                            ? RowSortu(ekintza.Deskribapena, ekintza.Eguna, ekintza.Ordua, ekintza.Mota)
                            : RowSortu("Ekintza", ekintza.Eguna, ekintza.Ordua, $"{ekintza.Mota}: {ekintza.Deskribapena}");
                        row.Tag = new DatuFila(bidaia, ekintza);
                        dgvDatuak.Rows.Add(row);
                    }
                }
            }
        }

        private ExelaSortu.HotelDatuak HotelDatuakSortu(Bidaiak bidaia)
        {
            return new ExelaSortu.HotelDatuak(
                bidaia.HotelHautatua?.Hiria ?? "",
                bidaia.HotelHautatua?.Izena ?? "",
                bidaia.HotelHautatua?.HelbideaUrl ?? "",
                bidaia.EgunKop,
                bidaia.EgunLaburpenak.FirstOrDefault()?.Data ?? ""
            );
        }

        private void ZutabeakEzarri(string mota)
        {
            dgvDatuak.Columns.Clear();

            if (mota == "Hotelak")
            {
                dgvDatuak.Columns.Add("HotelIzena", "Hotel izena");
                dgvDatuak.Columns.Add("EgunKop", "Egun kopurua");
                dgvDatuak.Columns.Add("Data", "Data");
                dgvDatuak.Columns.Add("Hiria", "Hiria");
            }
            else if (mota == "Ekintzak")
            {
                dgvDatuak.Columns.Add("Aktibitatea", "Aktibitatea");
                dgvDatuak.Columns.Add("Eguna", "Eguna");
                dgvDatuak.Columns.Add("Ordua", "Ordua");
                dgvDatuak.Columns.Add("Mota", "Mota");
            }
            else
            {
                dgvDatuak.Columns.Add("Mota", "Mota");
                dgvDatuak.Columns.Add("Eguna", "Eguna");
                dgvDatuak.Columns.Add("DataOrdua", "Data / Ordua");
                dgvDatuak.Columns.Add("Xehetasuna", "Xehetasuna");
            }
        }

        private DataGridViewRow RowSortu(string mota, string eguna, string dataOrdua, string xehetasuna)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dgvDatuak, mota, eguna, dataOrdua, xehetasuna);
            return row;
        }

        private object? HautatutakoObjektua()
        {
            if (dgvDatuak.SelectedRows.Count == 0)
            {
                MessageBox.Show("Aukeratu elementu bat lehenengo.");
                return null;
            }

            return dgvDatuak.SelectedRows[0].Tag;
        }

        private void btnEditatu_Click(object? sender, EventArgs e)
        {
            if (HautatutakoObjektua() is not DatuFila fila)
            {
                return;
            }

            bidaiaEditatu(fila.Bidaia);
            Close();
        }

        private void btnEzabatu_Click(object? sender, EventArgs e)
        {
            if (HautatutakoObjektua() is not DatuFila fila)
            {
                return;
            }

            DialogResult erantzuna = MessageBox.Show("Aukeratutako elementua ezabatu nahi duzu?", "Ezabatu", MessageBoxButtons.YesNo);
            if (erantzuna != DialogResult.Yes)
            {
                return;
            }

            if (fila.Datua is ExelaSortu.HotelDatuak)
            {
                HotelarekinLotutakoGuztiaEzabatu(fila.Bidaia);
            }
            else if (fila.Datua is ExelaSortu.EgunLaburpena eguna)
            {
                EgunaEtaBereEkintzakEzabatu(fila.Bidaia, eguna);
            }
            else if (fila.Datua is ExelaSortu.EkintzaDatuak ekintza)
            {
                fila.Bidaia.EkintzaDatuak.Remove(ekintza);
                ekintzak.Remove(ekintza);
            }

            DatuakKargatu();
        }

        private void HotelarekinLotutakoGuztiaEzabatu(Bidaiak bidaia)
        {
            bidaiak.Remove(bidaia);
            ExelaSortu.HotelDatuak? hotela = hotelak.FirstOrDefault(h => h.Izena == bidaia.HotelHautatua?.Izena);
            if (hotela != null)
            {
                hotelak.Remove(hotela);
            }

            foreach (ExelaSortu.EgunLaburpena eguna in bidaia.EgunLaburpenak.ToList())
            {
                egunak.Remove(eguna);
            }

            foreach (ExelaSortu.EkintzaDatuak ekintza in bidaia.EkintzaDatuak.ToList())
            {
                ekintzak.Remove(ekintza);
            }
        }

        private void EgunaEtaBereEkintzakEzabatu(Bidaiak bidaia, ExelaSortu.EgunLaburpena eguna)
        {
            bidaia.EgunLaburpenak.Remove(eguna);
            egunak.Remove(eguna);

            List<ExelaSortu.EkintzaDatuak> lotutakoEkintzak = bidaia.EkintzaDatuak
                .Where(e => e.Eguna == eguna.Data
                    || e.Eguna == $"{eguna.Eguna}. eguna"
                    || e.Eguna == $"Eguna{eguna.Eguna}")
                .ToList();

            foreach (ExelaSortu.EkintzaDatuak ekintza in lotutakoEkintzak)
            {
                bidaia.EkintzaDatuak.Remove(ekintza);
                ekintzak.Remove(ekintza);
            }
        }

        private class DatuFila
        {
            public DatuFila(Bidaiak bidaia, object datua)
            {
                Bidaia = bidaia;
                Datua = datua;
            }

            public Bidaiak Bidaia { get; }
            public object Datua { get; }
        }
    }
}
