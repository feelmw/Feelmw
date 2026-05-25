using System.Windows.Forms;

namespace FeelmwLogistika.Formularioak
{
    partial class FEkintza
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
            chkAldagela = new CheckBox();
            lblEramanM = new Label();
            lblElkartokia = new Label();
            lblIraupena = new Label();
            cbxEkintza = new ComboBox();
            txtIraupena = new TextBox();
            txtBertanM = new TextBox();
            txtEramanM = new TextBox();
            txtElkartokia = new TextBox();
            chkKomuna = new CheckBox();
            lblBertanM = new Label();
            lblEkintza = new Label();
            panel3 = new Panel();
            lblLokali = new Label();
            txtLokali = new TextBox();
            lblIristean = new Label();
            txtIristean = new TextBox();
            lblEgonlekua = new Label();
            txtEgonlekua = new TextBox();
            btnIrten = new Button();
            btnGehitu = new Button();
            btnGorde = new Button();
            lblBonoa = new Label();
            txtBonoa = new TextBox();
            lblInfo = new Label();
            txtInfo = new TextBox();
            lblKontaktua = new Label();
            txtKontaktua = new TextBox();
            panel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // lblIzenburua
            // 
            lblIzenburua.AutoSize = true;
            lblIzenburua.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold);
            lblIzenburua.ForeColor = Color.FromArgb(45, 45, 48);
            lblIzenburua.Location = new Point(49, 12);
            lblIzenburua.Name = "lblIzenburua";
            lblIzenburua.Size = new Size(372, 54);
            lblIzenburua.TabIndex = 2;
            lblIzenburua.Text = "Ekintzen kudeaketa";
            // 
            // panel1
            // 
            panel1.Controls.Add(tableLayoutPanel1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(30);
            panel1.Size = new Size(1187, 880);
            panel1.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(panel2, 0, 0);
            tableLayoutPanel1.Controls.Add(panel3, 1, 0);
            tableLayoutPanel1.Location = new Point(49, 65);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(10);
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(1091, 782);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.BackColor = Color.White;
            panel2.Controls.Add(chkAldagela);
            panel2.Controls.Add(lblEramanM);
            panel2.Controls.Add(lblElkartokia);
            panel2.Controls.Add(lblIraupena);
            panel2.Controls.Add(cbxEkintza);
            panel2.Controls.Add(txtIraupena);
            panel2.Controls.Add(txtBertanM);
            panel2.Controls.Add(txtEramanM);
            panel2.Controls.Add(txtElkartokia);
            panel2.Controls.Add(chkKomuna);
            panel2.Controls.Add(lblBertanM);
            panel2.Controls.Add(lblEkintza);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(13, 13);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(20);
            panel2.Size = new Size(529, 756);
            panel2.TabIndex = 0;
            // 
            // chkAldagela
            // 
            chkAldagela.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chkAldagela.AutoSize = true;
            chkAldagela.Location = new Point(65, 630);
            chkAldagela.Name = "chkAldagela";
            chkAldagela.Size = new Size(91, 24);
            chkAldagela.TabIndex = 5;
            chkAldagela.Text = "Aldagela";
            chkAldagela.UseVisualStyleBackColor = true;
            // 
            // lblEramanM
            // 
            lblEramanM.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblEramanM.AutoSize = true;
            lblEramanM.Location = new Point(31, 495);
            lblEramanM.Name = "lblEramanM";
            lblEramanM.Size = new Size(197, 20);
            lblEramanM.TabIndex = 33;
            lblEramanM.Text = "Eraman beharreko materiala";
            // 
            // lblElkartokia
            // 
            lblElkartokia.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblElkartokia.AutoSize = true;
            lblElkartokia.Location = new Point(31, 257);
            lblElkartokia.Name = "lblElkartokia";
            lblElkartokia.Size = new Size(74, 20);
            lblElkartokia.TabIndex = 32;
            lblElkartokia.Text = "Elkartokia";
            // 
            // lblIraupena
            // 
            lblIraupena.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblIraupena.AutoSize = true;
            lblIraupena.Location = new Point(31, 138);
            lblIraupena.Name = "lblIraupena";
            lblIraupena.Size = new Size(67, 20);
            lblIraupena.TabIndex = 31;
            lblIraupena.Text = "Iraupena";
            // 
            // cbxEkintza
            // 
            cbxEkintza.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cbxEkintza.FlatStyle = FlatStyle.Flat;
            cbxEkintza.Font = new Font("Segoe UI", 10F);
            cbxEkintza.FormattingEnabled = true;
            cbxEkintza.Location = new Point(31, 42);
            cbxEkintza.Name = "cbxEkintza";
            cbxEkintza.Size = new Size(460, 31);
            cbxEkintza.TabIndex = 0;
            cbxEkintza.SelectedValueChanged += cbxEkintza_SelectedValueChanged;
            // 
            // txtIraupena
            // 
            txtIraupena.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtIraupena.BorderStyle = BorderStyle.FixedSingle;
            txtIraupena.Font = new Font("Segoe UI", 10F);
            txtIraupena.Location = new Point(30, 161);
            txtIraupena.Name = "txtIraupena";
            txtIraupena.Size = new Size(461, 30);
            txtIraupena.TabIndex = 1;
            // 
            // txtBertanM
            // 
            txtBertanM.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtBertanM.BorderStyle = BorderStyle.FixedSingle;
            txtBertanM.Font = new Font("Segoe UI", 10F);
            txtBertanM.Location = new Point(31, 399);
            txtBertanM.Name = "txtBertanM";
            txtBertanM.Size = new Size(460, 30);
            txtBertanM.TabIndex = 3;
            // 
            // txtEramanM
            // 
            txtEramanM.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtEramanM.BorderStyle = BorderStyle.FixedSingle;
            txtEramanM.Font = new Font("Segoe UI", 10F);
            txtEramanM.Location = new Point(31, 518);
            txtEramanM.Name = "txtEramanM";
            txtEramanM.Size = new Size(460, 30);
            txtEramanM.TabIndex = 4;
            // 
            // txtElkartokia
            // 
            txtElkartokia.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtElkartokia.BorderStyle = BorderStyle.FixedSingle;
            txtElkartokia.Font = new Font("Segoe UI", 10F);
            txtElkartokia.Location = new Point(31, 280);
            txtElkartokia.Name = "txtElkartokia";
            txtElkartokia.Size = new Size(460, 30);
            txtElkartokia.TabIndex = 2;
            // 
            // chkKomuna
            // 
            chkKomuna.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chkKomuna.AutoSize = true;
            chkKomuna.Location = new Point(331, 630);
            chkKomuna.Name = "chkKomuna";
            chkKomuna.Size = new Size(93, 24);
            chkKomuna.TabIndex = 6;
            chkKomuna.Text = "Komunak";
            chkKomuna.UseVisualStyleBackColor = true;
            // 
            // lblBertanM
            // 
            lblBertanM.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblBertanM.AutoSize = true;
            lblBertanM.Location = new Point(31, 376);
            lblBertanM.Name = "lblBertanM";
            lblBertanM.Size = new Size(193, 20);
            lblBertanM.TabIndex = 3;
            lblBertanM.Text = "Bertan emandako materiala";
            // 
            // lblEkintza
            // 
            lblEkintza.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblEkintza.AutoSize = true;
            lblEkintza.Location = new Point(31, 19);
            lblEkintza.Name = "lblEkintza";
            lblEkintza.Size = new Size(56, 20);
            lblEkintza.TabIndex = 1;
            lblEkintza.Text = "Ekintza";
            // 
            // panel3
            // 
            panel3.BackColor = Color.White;
            panel3.Controls.Add(lblLokali);
            panel3.Controls.Add(txtLokali);
            panel3.Controls.Add(lblIristean);
            panel3.Controls.Add(txtIristean);
            panel3.Controls.Add(lblEgonlekua);
            panel3.Controls.Add(txtEgonlekua);
            panel3.Controls.Add(btnIrten);
            panel3.Controls.Add(btnGehitu);
            panel3.Controls.Add(btnGorde);
            panel3.Controls.Add(lblBonoa);
            panel3.Controls.Add(txtBonoa);
            panel3.Controls.Add(lblInfo);
            panel3.Controls.Add(txtInfo);
            panel3.Controls.Add(lblKontaktua);
            panel3.Controls.Add(txtKontaktua);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(548, 13);
            panel3.Name = "panel3";
            panel3.Padding = new Padding(20);
            panel3.Size = new Size(530, 756);
            panel3.TabIndex = 1;
            //
            // lblLokali
            //
            lblLokali.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblLokali.AutoSize = true;
            lblLokali.Location = new Point(28, 138);
            lblLokali.Name = "lblLokali";
            lblLokali.Size = new Size(111, 20);
            lblLokali.TabIndex = 33;
            lblLokali.Text = "Lokalizatzailea";
            //
            // txtLokali
            //
            txtLokali.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLokali.BorderStyle = BorderStyle.FixedSingle;
            txtLokali.Font = new Font("Segoe UI", 10F);
            txtLokali.Location = new Point(28, 161);
            txtLokali.Name = "txtLokali";
            txtLokali.Size = new Size(461, 30);
            txtLokali.TabIndex = 8;
            txtLokali.Text = "Feelmw";
            //
            // lblIristean
            //
            lblIristean.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblIristean.AutoSize = true;
            lblIristean.Location = new Point(28, 257);
            lblIristean.Name = "lblIristean";
            lblIristean.Size = new Size(57, 20);
            lblIristean.TabIndex = 31;
            lblIristean.Text = "Iristean";
            // 
            // txtIristean
            // 
            txtIristean.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtIristean.BorderStyle = BorderStyle.FixedSingle;
            txtIristean.Font = new Font("Segoe UI", 10F);
            txtIristean.Location = new Point(28, 280);
            txtIristean.Name = "txtIristean";
            txtIristean.Size = new Size(461, 30);
            txtIristean.TabIndex = 9;
            // 
            // lblEgonlekua
            // 
            lblEgonlekua.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblEgonlekua.AutoSize = true;
            lblEgonlekua.Location = new Point(28, 590);
            lblEgonlekua.Name = "lblEgonlekua";
            lblEgonlekua.Size = new Size(78, 20);
            lblEgonlekua.TabIndex = 32;
            lblEgonlekua.Text = "Egonlekua";
            // 
            // txtEgonlekua
            // 
            txtEgonlekua.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtEgonlekua.BorderStyle = BorderStyle.FixedSingle;
            txtEgonlekua.Font = new Font("Segoe UI", 10F);
            txtEgonlekua.Location = new Point(28, 613);
            txtEgonlekua.Name = "txtEgonlekua";
            txtEgonlekua.Size = new Size(461, 30);
            txtEgonlekua.TabIndex = 12;
            // 
            // btnIrten
            // 
            btnIrten.Anchor = AnchorStyles.Bottom;
            btnIrten.BackColor = Color.FromArgb(220, 53, 69);
            btnIrten.Cursor = Cursors.Hand;
            btnIrten.FlatAppearance.BorderSize = 0;
            btnIrten.FlatStyle = FlatStyle.Flat;
            btnIrten.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnIrten.ForeColor = Color.White;
            btnIrten.Location = new Point(375, 658);
            btnIrten.Name = "btnIrten";
            btnIrten.Size = new Size(118, 33);
            btnIrten.TabIndex = 15;
            btnIrten.Text = "🚪 Irten";
            btnIrten.UseVisualStyleBackColor = false;
            btnIrten.Click += btnIrten_Click;
            // 
            // btnGehitu
            // 
            btnGehitu.Anchor = AnchorStyles.Bottom;
            btnGehitu.BackColor = Color.FromArgb(40, 167, 69);
            btnGehitu.Cursor = Cursors.Hand;
            btnGehitu.FlatAppearance.BorderSize = 0;
            btnGehitu.FlatStyle = FlatStyle.Flat;
            btnGehitu.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnGehitu.ForeColor = Color.White;
            btnGehitu.Location = new Point(183, 658);
            btnGehitu.Name = "btnGehitu";
            btnGehitu.Size = new Size(151, 33);
            btnGehitu.TabIndex = 14;
            btnGehitu.Text = "💾 Berria Gehitu";
            btnGehitu.UseVisualStyleBackColor = false;
            btnGehitu.Click += btnGehitu_Click;
            // 
            // btnGorde
            // 
            btnGorde.Anchor = AnchorStyles.Bottom;
            btnGorde.BackColor = Color.FromArgb(0, 120, 215);
            btnGorde.Cursor = Cursors.Hand;
            btnGorde.FlatAppearance.BorderSize = 0;
            btnGorde.FlatStyle = FlatStyle.Flat;
            btnGorde.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnGorde.ForeColor = Color.White;
            btnGorde.Location = new Point(23, 658);
            btnGorde.Name = "btnGorde";
            btnGorde.Size = new Size(118, 33);
            btnGorde.TabIndex = 13;
            btnGorde.Text = "💾 Gorde";
            btnGorde.UseVisualStyleBackColor = false;
            btnGorde.Click += btnGorde_Click;
            // 
            // lblBonoa
            // 
            lblBonoa.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblBonoa.AutoSize = true;
            lblBonoa.Location = new Point(28, 19);
            lblBonoa.Name = "lblBonoa";
            lblBonoa.Size = new Size(52, 20);
            lblBonoa.TabIndex = 3;
            lblBonoa.Text = "Bonoa";
            // 
            // txtBonoa
            // 
            txtBonoa.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtBonoa.BorderStyle = BorderStyle.FixedSingle;
            txtBonoa.Font = new Font("Segoe UI", 10F);
            txtBonoa.Location = new Point(28, 42);
            txtBonoa.Name = "txtBonoa";
            txtBonoa.Size = new Size(461, 30);
            txtBonoa.TabIndex = 7;
            // 
            // lblInfo
            // 
            lblInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(28, 376);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(89, 20);
            lblInfo.TabIndex = 3;
            lblInfo.Text = "Informazioa";
            // 
            // txtInfo
            // 
            txtInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtInfo.BorderStyle = BorderStyle.FixedSingle;
            txtInfo.Font = new Font("Segoe UI", 10F);
            txtInfo.Location = new Point(28, 399);
            txtInfo.Name = "txtInfo";
            txtInfo.Size = new Size(460, 30);
            txtInfo.TabIndex = 10;
            // 
            // lblKontaktua
            // 
            lblKontaktua.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblKontaktua.AutoSize = true;
            lblKontaktua.Location = new Point(28, 495);
            lblKontaktua.Name = "lblKontaktua";
            lblKontaktua.Size = new Size(76, 20);
            lblKontaktua.TabIndex = 3;
            lblKontaktua.Text = "Kontaktua";
            // 
            // txtKontaktua
            // 
            txtKontaktua.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtKontaktua.BorderStyle = BorderStyle.FixedSingle;
            txtKontaktua.Font = new Font("Segoe UI", 10F);
            txtKontaktua.Location = new Point(28, 518);
            txtKontaktua.Name = "txtKontaktua";
            txtKontaktua.Size = new Size(461, 30);
            txtKontaktua.TabIndex = 11;
            // 
            // FEkintza
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1187, 880);
            Controls.Add(lblIzenburua);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FEkintza";
            Text = "FEkintza";
            Load += FEkintza_Load;
            panel1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblIzenburua;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel2;
        private ComboBox cbxEkintza;
        private TextBox txtBertanM;
        private TextBox txtEramanM;
        private TextBox txtElkartokia;
        private CheckBox chkKomuna;
        private Label lblBertanM;
        private Label lblEkintza;
        private Label lblInfo;
        private TextBox txtInfo;
        private Panel panel3;
        private Button btnIrten;
        private Button btnGehitu;
        private Button btnGorde;
        private Label lblBonoa;
        private TextBox txtBonoa;
        private Label lblKontaktua;
        private TextBox txtKontaktua;
        private Label lblEramanM;
        private Label lblElkartokia;
        private Label lblIraupena;
        private TextBox txtIraupena;
        private CheckBox chkAldagela;
        private Label lblIristean;
        private TextBox txtIristean;
        private Label lblEgonlekua;
        private TextBox txtEgonlekua;
        private Label lblLokali;
        private TextBox txtLokali;
    }
}
