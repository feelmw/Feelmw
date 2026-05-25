using FeelmwLogistika.Logistika.DatuModeloak;

namespace FeelmwLogistika.Formularioak
{
    public partial class FDatuakIkusi : Form
    {
        private readonly List<Ostalak> lisOst;
        private readonly List<Ekintzak> lisEki;
        private readonly List<Garraioak> lisGar;
        private readonly Panel panela;
        private readonly DataGridView dgvDatuak;
        private readonly ComboBox cbxMota;

        public FDatuakIkusi(List<Ostalak> lisOst, List<Ekintzak> lisEki, List<Garraioak> lisGar, Panel panela)
        {
            this.lisOst = lisOst;
            this.lisEki = lisEki;
            this.lisGar = lisGar;
            this.panela = panela;

            BackColor = AppEstiloa.Fondoa;
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;
            DoubleBuffered = true;

            Label lblIzenburua = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                Location = new Point(49, 18),
                Text = "Datuen kudeaketa"
            };

            Panel edukia = new Panel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = AppEstiloa.Txartela,
                BorderStyle = BorderStyle.None,
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
            cbxMota.Items.AddRange(new object[] { "Guztiak", "Ostalak", "Ekintzak", "Garraioak" });
            cbxMota.SelectedIndex = 0;
            cbxMota.SelectedIndexChanged += (s, e) => DatuakKargatu();

            Button btnEditatu = BotoiaSortu("✏️ Editatu", AppEstiloa.Urdina, new Point(596, 48));
            btnEditatu.Click += btnEditatu_Click;

            Button btnEzabatu = BotoiaSortu("🗑️ Ezabatu", AppEstiloa.Gorria, new Point(736, 48));
            btnEzabatu.Click += btnEzabatu_Click;

            Button btnIrten = BotoiaSortu("🚪 Irten", AppEstiloa.Gorria, new Point(876, 48));
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
            dgvDatuak.Columns.Add("Mota", "Mota");
            dgvDatuak.Columns.Add("Izena", "Izena");
            dgvDatuak.Columns.Add("Xehetasuna", "Xehetasuna");

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

            if (mota == "Guztiak" || mota == "Ostalak")
            {
                foreach (Ostalak ostala in lisOst)
                {
                    DataGridViewRow row = RowSortu("Ostala", ostala.OstalaIzena, ostala.Datak);
                    row.Tag = ostala;
                    dgvDatuak.Rows.Add(row);
                }
            }

            if (mota == "Guztiak" || mota == "Ekintzak")
            {
                foreach (Ekintzak ekintza in lisEki)
                {
                    DataGridViewRow row = RowSortu("Ekintza", ekintza.EkintzaIzena, ekintza.Iraupena);
                    row.Tag = ekintza;
                    dgvDatuak.Rows.Add(row);
                }
            }

            if (mota == "Guztiak" || mota == "Garraioak")
            {
                foreach (Garraioak garraioa in lisGar)
                {
                    DataGridViewRow row = RowSortu("Garraioa", garraioa.GarraioaIzena, garraioa.Eguna);
                    row.Tag = garraioa;
                    dgvDatuak.Rows.Add(row);
                }
            }
        }

        private DataGridViewRow RowSortu(string mota, string izena, string xehetasuna)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dgvDatuak, mota, izena, xehetasuna);
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

        private void btnEzabatu_Click(object? sender, EventArgs e)
        {
            object? objektua = HautatutakoObjektua();
            if (objektua == null)
            {
                return;
            }

            DialogResult erantzuna = MessageBox.Show("Aukeratutako elementua ezabatu nahi duzu?", "Ezabatu", MessageBoxButtons.YesNo);
            if (erantzuna != DialogResult.Yes)
            {
                return;
            }

            if (objektua is Ostalak ostala)
            {
                lisOst.Remove(ostala);
            }
            else if (objektua is Ekintzak ekintza)
            {
                lisEki.Remove(ekintza);
            }
            else if (objektua is Garraioak garraioa)
            {
                lisGar.Remove(garraioa);
            }

            DatuakKargatu();
        }

        private void btnEditatu_Click(object? sender, EventArgs e)
        {
            object? objektua = HautatutakoObjektua();
            if (objektua == null)
            {
                return;
            }

            if (objektua is Ostalak ostala)
            {
                FOstala form = new FOstala(ostala);
                FormularioaIreki(form, () =>
                {
                    if (form.Ost != null)
                    {
                        OstalaEguneratu(ostala, form.Ost);
                    }
                });
            }
            else if (objektua is Ekintzak ekintza)
            {
                FEkintza form = new FEkintza(ekintza);
                FormularioaIreki(form, () =>
                {
                    if (form.Eki != null)
                    {
                        EkintzaEguneratu(ekintza, form.Eki);
                    }
                });
            }
            else if (objektua is Garraioak garraioa)
            {
                FGarraioa form = new FGarraioa(garraioa);
                FormularioaIreki(form, () =>
                {
                    if (form.Gar != null)
                    {
                        GarraioaEguneratu(garraioa, form.Gar);
                    }
                });
            }
        }

        private void FormularioaIreki(Form form, Action itxitakoan)
        {
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            panela.Controls.Add(form);
            form.BringToFront();
            form.Show();
            form.FormClosed += (s, e) =>
            {
                itxitakoan();
                panela.Controls.Remove(form);
                Close();
            };
        }

        private void OstalaEguneratu(Ostalak helburua, Ostalak berria)
        {
            helburua.OstalaIzena = berria.OstalaIzena;
            helburua.Bonoa = berria.Bonoa;
            helburua.Helbidea = berria.Helbidea;
            helburua.Lokalizatzailea = berria.Lokalizatzailea;
            helburua.Gauak = berria.Gauak;
            helburua.Datak = berria.Datak;
            helburua.Gelak = berria.Gelak;
            helburua.Checkin = berria.Checkin;
            helburua.Checkout = berria.Checkout;
            helburua.Dokumentazioa = berria.Dokumentazioa;
            helburua.Harrera = berria.Harrera;
            helburua.Gosariates = berria.Gosariates;
            helburua.Bazkariates = berria.Bazkariates;
            helburua.Afariates = berria.Afariates;
            helburua.Toailak = berria.Toailak;
            helburua.Izarak = berria.Izarak;
            helburua.Fidantza = berria.Fidantza;
            helburua.FidantzaKuota = berria.FidantzaKuota;
            helburua.Luggage = berria.Luggage;
            helburua.LuggageKuota = berria.LuggageKuota;
            helburua.Instalazioak = berria.Instalazioak;
        }

        private void EkintzaEguneratu(Ekintzak helburua, Ekintzak berria)
        {
            helburua.EkintzaIzena = berria.EkintzaIzena;
            helburua.Bonoa = berria.Bonoa;
            helburua.Iraupena = berria.Iraupena;
            helburua.Kontaktua = berria.Kontaktua;
            helburua.Elkartokia = berria.Elkartokia;
            helburua.Iristean = berria.Iristean;
            helburua.EramanM = berria.EramanM;
            helburua.BertanM = berria.BertanM;
            helburua.Aldagela = berria.Aldagela;
            helburua.Komuna = berria.Komuna;
            helburua.Egonlekua = berria.Egonlekua;
            helburua.Informazioa = berria.Informazioa;
            helburua.Lokali = berria.Lokali;
        }

        private void GarraioaEguneratu(Garraioak helburua, Garraioak berria)
        {
            helburua.GarraioaIzena = berria.GarraioaIzena;
            helburua.Eguna = berria.Eguna;
            helburua.Ordutegia = berria.Ordutegia;
            helburua.Lokalizatzailea = berria.Lokalizatzailea;
            helburua.Kontaktua = berria.Kontaktua;
            helburua.Elkargunea = berria.Elkargunea;
            helburua.Eginbeharrak = berria.Eginbeharrak;
            helburua.Informazioa = berria.Informazioa;
        }
    }
}
