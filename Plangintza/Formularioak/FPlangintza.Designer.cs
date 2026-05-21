using System.Windows.Forms;

namespace FeelmwLogistika.Plangintza.Formularioak
{
    partial class FPlangintza
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblIzenburua = new Label();
            panel1 = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            panel2 = new Panel();
            cbxMota = new ComboBox();
            dateTimePicker1 = new DateTimePicker();
            txtData = new TextBox();
            lblData = new Label();
            btnSortu = new Button();
            lblEkintza = new Label();
            btnIrten = new Button();
            nudEgunKop = new NumericUpDown();
            btnGorde = new Button();
            lblMota = new Label();
            lblBazkaria = new Label();
            lblGaua = new Label();
            cbxOstala = new ComboBox();
            txtGaua = new TextBox();
            txtArratsaldea = new TextBox();
            txtGoisak = new TextBox();
            lblLaburpena = new Label();
            lblGoizak = new Label();
            lblEgunKop = new Label();
            lblArratsaldea = new Label();
            lblOstala = new Label();
            lblDeskribapena = new Label();
            txtDeskribapena = new TextBox();
            panel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudEgunKop).BeginInit();
            SuspendLayout();
            // 
            // lblIzenburua
            // 
            lblIzenburua.AutoSize = true;
            lblIzenburua.Font = new Font("Segoe UI", 22F);
            lblIzenburua.Location = new Point(79, 55);
            lblIzenburua.Name = "lblIzenburua";
            lblIzenburua.Size = new Size(342, 50);
            lblIzenburua.TabIndex = 4;
            lblIzenburua.Text = "Eguneko kudeaketa";
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(245, 246, 250);
            panel1.Controls.Add(lblIzenburua);
            panel1.Controls.Add(tableLayoutPanel1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(30);
            panel1.Size = new Size(1178, 905);
            panel1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(panel2, 0, 0);
            tableLayoutPanel1.Location = new Point(49, 65);
            tableLayoutPanel1.Margin = new Padding(10);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(10);
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1065, 778);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.BackColor = Color.White;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(cbxMota);
            panel2.Controls.Add(dateTimePicker1);
            panel2.Controls.Add(txtData);
            panel2.Controls.Add(lblData);
            panel2.Controls.Add(btnSortu);
            panel2.Controls.Add(lblEkintza);
            panel2.Controls.Add(btnIrten);
            panel2.Controls.Add(nudEgunKop);
            panel2.Controls.Add(btnGorde);
            panel2.Controls.Add(lblMota);
            panel2.Controls.Add(lblBazkaria);
            panel2.Controls.Add(lblGaua);
            panel2.Controls.Add(cbxOstala);
            panel2.Controls.Add(txtGaua);
            panel2.Controls.Add(txtArratsaldea);
            panel2.Controls.Add(txtGoisak);
            panel2.Controls.Add(lblLaburpena);
            panel2.Controls.Add(lblGoizak);
            panel2.Controls.Add(lblEgunKop);
            panel2.Controls.Add(lblArratsaldea);
            panel2.Controls.Add(lblOstala);
            panel2.Controls.Add(lblDeskribapena);
            panel2.Controls.Add(txtDeskribapena);
            panel2.Location = new Point(13, 13);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(15);
            panel2.Size = new Size(1039, 752);
            panel2.TabIndex = 5;
            // 
            // cbxMota
            // 
            cbxMota.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cbxMota.BackColor = Color.FromArgb(250, 250, 250);
            cbxMota.FlatStyle = FlatStyle.Flat;
            cbxMota.FormattingEnabled = true;
            cbxMota.Location = new Point(159, 512);
            cbxMota.Name = "cbxMota";
            cbxMota.Size = new Size(649, 31);
            cbxMota.TabIndex = 7;
            cbxMota.SelectedValueChanged += cbxMota_SelectedValueChanged;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Format = DateTimePickerFormat.Time;
            dateTimePicker1.Location = new Point(159, 462);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(649, 30);
            dateTimePicker1.TabIndex = 6;
            // 
            // txtData
            // 
            txtData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtData.BackColor = Color.FromArgb(250, 250, 250);
            txtData.BorderStyle = BorderStyle.FixedSingle;
            txtData.Location = new Point(45, 192);
            txtData.Name = "txtData";
            txtData.Size = new Size(762, 30);
            txtData.TabIndex = 2;
            // 
            // lblData
            // 
            lblData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblData.AutoSize = true;
            lblData.Location = new Point(46, 169);
            lblData.Name = "lblData";
            lblData.Size = new Size(46, 23);
            lblData.TabIndex = 29;
            lblData.Text = "Data";
            // 
            // btnSortu
            // 
            btnSortu.Anchor = AnchorStyles.Bottom;
            btnSortu.BackColor = Color.FromArgb(0, 120, 215);
            btnSortu.Cursor = Cursors.Hand;
            btnSortu.FlatAppearance.BorderSize = 0;
            btnSortu.FlatStyle = FlatStyle.Flat;
            btnSortu.Font = new Font("Segoe UI Semibold", 10F);
            btnSortu.ForeColor = Color.White;
            btnSortu.Location = new Point(377, 675);
            btnSortu.Name = "btnSortu";
            btnSortu.Size = new Size(150, 50);
            btnSortu.TabIndex = 9;
            btnSortu.Text = "📄 Sortu dok.";
            btnSortu.UseVisualStyleBackColor = false;
            // 
            // lblEkintza
            // 
            lblEkintza.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblEkintza.AutoSize = true;
            lblEkintza.Location = new Point(45, 427);
            lblEkintza.Name = "lblEkintza";
            lblEkintza.Size = new Size(64, 23);
            lblEkintza.TabIndex = 28;
            lblEkintza.Text = "Ekintza";
            // 
            // btnIrten
            // 
            btnIrten.Anchor = AnchorStyles.Bottom;
            btnIrten.BackColor = Color.FromArgb(220, 53, 69);
            btnIrten.Cursor = Cursors.Hand;
            btnIrten.FlatAppearance.BorderSize = 0;
            btnIrten.FlatStyle = FlatStyle.Flat;
            btnIrten.Font = new Font("Segoe UI", 11F);
            btnIrten.ForeColor = SystemColors.ButtonHighlight;
            btnIrten.Location = new Point(704, 675);
            btnIrten.Name = "btnIrten";
            btnIrten.Size = new Size(127, 50);
            btnIrten.TabIndex = 11;
            btnIrten.Text = "<- Irten";
            btnIrten.UseVisualStyleBackColor = false;
            btnIrten.Click += btnIrten_Click;
            // 
            // nudEgunKop
            // 
            nudEgunKop.BackColor = Color.FromArgb(250, 250, 250);
            nudEgunKop.BorderStyle = BorderStyle.FixedSingle;
            nudEgunKop.Location = new Point(46, 125);
            nudEgunKop.Name = "nudEgunKop";
            nudEgunKop.Size = new Size(762, 30);
            nudEgunKop.TabIndex = 1;
            // 
            // btnGorde
            // 
            btnGorde.Anchor = AnchorStyles.Bottom;
            btnGorde.BackColor = Color.FromArgb(0, 120, 215);
            btnGorde.Cursor = Cursors.Hand;
            btnGorde.FlatAppearance.BorderSize = 0;
            btnGorde.FlatStyle = FlatStyle.Flat;
            btnGorde.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGorde.ForeColor = SystemColors.ButtonHighlight;
            btnGorde.Location = new Point(544, 675);
            btnGorde.Name = "btnGorde";
            btnGorde.Size = new Size(127, 50);
            btnGorde.TabIndex = 10;
            btnGorde.Text = "📄 Gorde";
            btnGorde.UseVisualStyleBackColor = false;
            // 
            // lblMota
            // 
            lblMota.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblMota.AutoSize = true;
            lblMota.Location = new Point(50, 512);
            lblMota.Name = "lblMota";
            lblMota.Size = new Size(50, 23);
            lblMota.TabIndex = 26;
            lblMota.Text = "Mota";
            // 
            // lblBazkaria
            // 
            lblBazkaria.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblBazkaria.AutoSize = true;
            lblBazkaria.Location = new Point(50, 462);
            lblBazkaria.Name = "lblBazkaria";
            lblBazkaria.Size = new Size(58, 23);
            lblBazkaria.TabIndex = 25;
            lblBazkaria.Text = "Ordua";
            // 
            // lblGaua
            // 
            lblGaua.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblGaua.AutoSize = true;
            lblGaua.Location = new Point(50, 385);
            lblGaua.Name = "lblGaua";
            lblGaua.Size = new Size(50, 23);
            lblGaua.TabIndex = 24;
            lblGaua.Text = "Gaua";
            // 
            // cbxOstala
            // 
            cbxOstala.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cbxOstala.BackColor = Color.FromArgb(250, 250, 250);
            cbxOstala.FlatStyle = FlatStyle.Flat;
            cbxOstala.FormattingEnabled = true;
            cbxOstala.Location = new Point(46, 57);
            cbxOstala.Name = "cbxOstala";
            cbxOstala.Size = new Size(762, 31);
            cbxOstala.TabIndex = 0;
            // 
            // txtGaua
            // 
            txtGaua.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtGaua.BackColor = Color.FromArgb(250, 250, 250);
            txtGaua.BorderStyle = BorderStyle.FixedSingle;
            txtGaua.Location = new Point(159, 383);
            txtGaua.Name = "txtGaua";
            txtGaua.Size = new Size(649, 30);
            txtGaua.TabIndex = 5;
            // 
            // txtArratsaldea
            // 
            txtArratsaldea.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtArratsaldea.BackColor = Color.FromArgb(250, 250, 250);
            txtArratsaldea.BorderStyle = BorderStyle.FixedSingle;
            txtArratsaldea.Location = new Point(159, 330);
            txtArratsaldea.Name = "txtArratsaldea";
            txtArratsaldea.Size = new Size(649, 30);
            txtArratsaldea.TabIndex = 4;
            // 
            // txtGoisak
            // 
            txtGoisak.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtGoisak.BackColor = Color.FromArgb(250, 250, 250);
            txtGoisak.BorderStyle = BorderStyle.FixedSingle;
            txtGoisak.Location = new Point(159, 275);
            txtGoisak.Name = "txtGoizak";
            txtGoisak.Size = new Size(649, 30);
            txtGoisak.TabIndex = 3;
            // 
            // lblLaburpena
            // 
            lblLaburpena.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblLaburpena.AutoSize = true;
            lblLaburpena.Location = new Point(46, 244);
            lblLaburpena.Name = "lblLaburpena";
            lblLaburpena.Size = new Size(91, 23);
            lblLaburpena.TabIndex = 3;
            lblLaburpena.Text = "Laburpena";
            // 
            // lblGoizak
            // 
            lblGoizak.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblGoizak.AutoSize = true;
            lblGoizak.Location = new Point(50, 277);
            lblGoizak.Name = "lblGoizak";
            lblGoizak.Size = new Size(61, 23);
            lblGoizak.TabIndex = 3;
            lblGoizak.Text = "Goizak";
            // 
            // lblEgunKop
            // 
            lblEgunKop.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblEgunKop.AutoSize = true;
            lblEgunKop.Location = new Point(46, 102);
            lblEgunKop.Name = "lblEgunKop";
            lblEgunKop.Size = new Size(117, 23);
            lblEgunKop.TabIndex = 3;
            lblEgunKop.Text = "Egun kopurua";
            // 
            // lblArratsaldea
            // 
            lblArratsaldea.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblArratsaldea.AutoSize = true;
            lblArratsaldea.Location = new Point(50, 332);
            lblArratsaldea.Name = "lblArratsaldea";
            lblArratsaldea.Size = new Size(96, 23);
            lblArratsaldea.TabIndex = 3;
            lblArratsaldea.Text = "Arratsaldea";
            // 
            // lblOstala
            // 
            lblOstala.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblOstala.AutoSize = true;
            lblOstala.Location = new Point(46, 34);
            lblOstala.Name = "lblOstala";
            lblOstala.Size = new Size(58, 23);
            lblOstala.TabIndex = 1;
            lblOstala.Text = "Ostala";
            // 
            // lblDeskribapena
            // 
            lblDeskribapena.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblDeskribapena.AutoSize = true;
            lblDeskribapena.Location = new Point(50, 562);
            lblDeskribapena.Name = "lblDeskribapena";
            lblDeskribapena.Size = new Size(113, 23);
            lblDeskribapena.TabIndex = 3;
            lblDeskribapena.Text = "Deskribapena";
            // 
            // txtDeskribapena
            // 
            txtDeskribapena.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtDeskribapena.BackColor = Color.FromArgb(250, 250, 250);
            txtDeskribapena.BorderStyle = BorderStyle.FixedSingle;
            txtDeskribapena.Location = new Point(169, 562);
            txtDeskribapena.Name = "txtDeskribapena";
            txtDeskribapena.Size = new Size(639, 30);
            txtDeskribapena.TabIndex = 8;
            // 
            // FPlangintza
            // 
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 246, 250);
            ClientSize = new Size(1178, 905);
            Controls.Add(panel1);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FPlangintza";
            Text = "FPlangintza";
            WindowState = FormWindowState.Maximized;
            Load += FPlangintza_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudEgunKop).EndInit();
            ResumeLayout(false);
        }


        #endregion
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lblIzenburua;
        private Panel panel2;
        private NumericUpDown nudEgunKop;
        private Label lblMota;
        private Label lblBazkaria;
        private Label lblGaua;
        private ComboBox cbxOstala;
        private TextBox txtGaua;
        private TextBox txtArratsaldea;
        private TextBox txtGoisak;
        private Label lblLaburpena;
        private Label lblGoizak;
        private Label lblEgunKop;
        private Label lblArratsaldea;
        private Label lblOstala;
        private Label lblDeskribapena;
        private TextBox txtDeskribapena;
        private Label lblEkintza;
        private Button btnSortu;
        private Button btnIrten;
        private Button btnGorde;
        private TextBox txtData;
        private Label lblData;
        private ComboBox cbxMota;
        private DateTimePicker dateTimePicker1;
    }
}