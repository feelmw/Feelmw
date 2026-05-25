namespace FeelmwLogistika
{
    partial class FLogina
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FLogina));
            lblMyfeel = new Label();
            btnEditatu = new Button();
            btnLogistika = new Button();
            btnIrten = new Button();
            cbxMota = new ComboBox();
            lblMota = new Label();
            panelak = new Panel();
            btnDokumentazioa = new Button();
            btnPlangintza = new Button();
            lblDoku = new Label();
            cbxDoku = new ComboBox();
            btnJarraitu = new Button();
            panelak.SuspendLayout();
            SuspendLayout();
            // 
            // lblMyfeel
            // 
            lblMyfeel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblMyfeel.AutoSize = true;
            lblMyfeel.BackColor = Color.Transparent;
            lblMyfeel.Font = new Font("Segoe UI Semibold", 32F, FontStyle.Bold);
            lblMyfeel.ForeColor = Color.FromArgb(40, 40, 40);
            lblMyfeel.Location = new Point(450, 90);
            lblMyfeel.Name = "lblMyfeel";
            lblMyfeel.Size = new Size(431, 72);
            lblMyfeel.TabIndex = 1;
            lblMyfeel.Text = "Myfeel Logistika";
            // 
            // btnEditatu
            // 
            btnEditatu.Anchor = AnchorStyles.Bottom;
            btnEditatu.BackColor = Color.FromArgb(0, 120, 215);
            btnEditatu.Cursor = Cursors.Hand;
            btnEditatu.FlatAppearance.BorderSize = 0;
            btnEditatu.FlatStyle = FlatStyle.Flat;
            btnEditatu.Font = new Font("Segoe UI Semibold", 10F);
            btnEditatu.ForeColor = Color.White;
            btnEditatu.Location = new Point(443, 571);
            btnEditatu.Name = "btnEditatu";
            btnEditatu.Size = new Size(150, 50);
            btnEditatu.TabIndex = 2;
            btnEditatu.Text = "📄 Editatu dok.";
            btnEditatu.UseVisualStyleBackColor = false;
            btnEditatu.Click += btnEditatu_Click;
            // 
            // btnLogistika
            // 
            btnLogistika.Anchor = AnchorStyles.Bottom;
            btnLogistika.BackColor = Color.FromArgb(0, 120, 215);
            btnLogistika.Cursor = Cursors.Hand;
            btnLogistika.FlatAppearance.BorderSize = 0;
            btnLogistika.FlatStyle = FlatStyle.Flat;
            btnLogistika.Font = new Font("Segoe UI Semibold", 10F);
            btnLogistika.ForeColor = Color.White;
            btnLogistika.Location = new Point(381, 490);
            btnLogistika.Name = "btnLogistika";
            btnLogistika.Size = new Size(127, 50);
            btnLogistika.TabIndex = 1;
            btnLogistika.Text = "📄 Logistika";
            btnLogistika.UseVisualStyleBackColor = false;
            btnLogistika.Click += btnLogistika_Click;
            // 
            // btnIrten
            // 
            btnIrten.Anchor = AnchorStyles.Bottom;
            btnIrten.BackColor = Color.FromArgb(220, 53, 69);
            btnIrten.Cursor = Cursors.Hand;
            btnIrten.FlatAppearance.BorderSize = 0;
            btnIrten.FlatStyle = FlatStyle.Flat;
            btnIrten.Font = new Font("Segoe UI Semibold", 10F);
            btnIrten.ForeColor = Color.White;
            btnIrten.Location = new Point(621, 571);
            btnIrten.Name = "btnIrten";
            btnIrten.Size = new Size(127, 50);
            btnIrten.TabIndex = 3;
            btnIrten.Text = "<- Irten";
            btnIrten.UseVisualStyleBackColor = false;
            btnIrten.Click += btnIrten_Click;
            // 
            // cbxMota
            // 
            cbxMota.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cbxMota.BackColor = Color.White;
            cbxMota.FlatStyle = FlatStyle.Flat;
            cbxMota.Font = new Font("Segoe UI", 11F);
            cbxMota.FormattingEnabled = true;
            cbxMota.Items.AddRange(new object[] { "Klasikoa", "Berezia" });
            cbxMota.Location = new Point(381, 230);
            cbxMota.Name = "cbxMota";
            cbxMota.Size = new Size(458, 33);
            cbxMota.TabIndex = 0;
            cbxMota.Visible = false;
            // 
            // lblMota
            // 
            lblMota.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblMota.AutoSize = true;
            lblMota.Font = new Font("Segoe UI Semibold", 11F);
            lblMota.ForeColor = Color.DimGray;
            lblMota.Location = new Point(381, 175);
            lblMota.Name = "lblMota";
            lblMota.Size = new Size(114, 25);
            lblMota.TabIndex = 5;
            lblMota.Text = "Bidaia mota";
            lblMota.Visible = false;
            // 
            // panelak
            // 
            panelak.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelak.BackColor = Color.Transparent;
            panelak.Controls.Add(btnDokumentazioa);
            panelak.Controls.Add(btnPlangintza);
            panelak.Controls.Add(lblDoku);
            panelak.Controls.Add(cbxDoku);
            panelak.Controls.Add(btnIrten);
            panelak.Controls.Add(btnEditatu);
            panelak.Controls.Add(btnLogistika);
            panelak.Location = new Point(0, 0);
            panelak.Name = "panelak";
            panelak.Size = new Size(1309, 706);
            panelak.TabIndex = 6;
            // 
            // btnDokumentazioa
            // 
            btnDokumentazioa.Anchor = AnchorStyles.Bottom;
            btnDokumentazioa.BackColor = Color.FromArgb(0, 120, 215);
            btnDokumentazioa.Cursor = Cursors.Hand;
            btnDokumentazioa.FlatAppearance.BorderSize = 0;
            btnDokumentazioa.FlatStyle = FlatStyle.Flat;
            btnDokumentazioa.Font = new Font("Segoe UI Semibold", 10F);
            btnDokumentazioa.ForeColor = Color.White;
            btnDokumentazioa.Location = new Point(712, 490);
            btnDokumentazioa.Name = "btnDokumentazioa";
            btnDokumentazioa.Size = new Size(169, 50);
            btnDokumentazioa.TabIndex = 9;
            btnDokumentazioa.Text = "📄 Dokumentazioa";
            btnDokumentazioa.UseVisualStyleBackColor = false;
            // 
            // btnPlangintza
            // 
            btnPlangintza.Anchor = AnchorStyles.Bottom;
            btnPlangintza.BackColor = Color.FromArgb(0, 120, 215);
            btnPlangintza.Cursor = Cursors.Hand;
            btnPlangintza.FlatAppearance.BorderSize = 0;
            btnPlangintza.FlatStyle = FlatStyle.Flat;
            btnPlangintza.Font = new Font("Segoe UI Semibold", 10F);
            btnPlangintza.ForeColor = Color.White;
            btnPlangintza.Location = new Point(540, 490);
            btnPlangintza.Name = "btnPlangintza";
            btnPlangintza.Size = new Size(127, 50);
            btnPlangintza.TabIndex = 8;
            btnPlangintza.Text = "📄 Plangintza";
            btnPlangintza.UseVisualStyleBackColor = false;
            btnPlangintza.Click += btnPlangintza_Click;
            // 
            // lblDoku
            // 
            lblDoku.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblDoku.AutoSize = true;
            lblDoku.Font = new Font("Segoe UI Semibold", 11F);
            lblDoku.ForeColor = Color.DimGray;
            lblDoku.Location = new Point(381, 303);
            lblDoku.Name = "lblDoku";
            lblDoku.Size = new Size(286, 30);
            lblDoku.TabIndex = 7;
            lblDoku.Text = "Editatu nahi duzun dokumentua:";
            lblDoku.UseCompatibleTextRendering = true;
            lblDoku.Visible = false;
            // 
            // cbxDoku
            // 
            cbxDoku.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cbxDoku.BackColor = Color.White;
            cbxDoku.FlatStyle = FlatStyle.Flat;
            cbxDoku.Font = new Font("Segoe UI", 11F);
            cbxDoku.FormattingEnabled = true;
            cbxDoku.Items.AddRange(new object[] { "Klasikoak", "aaaa." });
            cbxDoku.Location = new Point(381, 345);
            cbxDoku.Name = "cbxDoku";
            cbxDoku.Size = new Size(458, 33);
            cbxDoku.TabIndex = 7;
            cbxDoku.Visible = false;
            cbxDoku.SelectedIndexChanged += cbxDoku_SelectedIndexChanged;
            //
            // btnJarraitu
            //
            btnJarraitu.Anchor = AnchorStyles.Top;
            btnJarraitu.BackColor = Color.FromArgb(0, 120, 215);
            btnJarraitu.Cursor = Cursors.Hand;
            btnJarraitu.FlatAppearance.BorderSize = 0;
            btnJarraitu.FlatStyle = FlatStyle.Flat;
            btnJarraitu.Font = new Font("Segoe UI Semibold", 10F);
            btnJarraitu.ForeColor = Color.White;
            btnJarraitu.Location = new Point(592, 300);
            btnJarraitu.Name = "btnJarraitu";
            btnJarraitu.Size = new Size(150, 45);
            btnJarraitu.TabIndex = 10;
            btnJarraitu.Text = "Jarraitu";
            btnJarraitu.UseVisualStyleBackColor = false;
            btnJarraitu.Visible = false;
            btnJarraitu.Click += btnJarraitu_Click;
            //
            // FLogina
            // 
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 246, 250);
            ClientSize = new Size(1309, 706);
            Controls.Add(cbxMota);
            Controls.Add(lblMota);
            Controls.Add(lblMyfeel);
            Controls.Add(panelak);
            Controls.Add(btnJarraitu);
            Font = new Font("Segoe UI", 10F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FLogina";
            Text = "Myfeel";
            WindowState = FormWindowState.Maximized;
            Load += FLogina_Load;
            panelak.ResumeLayout(false);
            panelak.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblMyfeel;
        private Button btnEditatu;
        private Button btnLogistika;
        private Button btnIrten;
        private ComboBox cbxMota;
        private Label lblMota;
        private Panel panelak;
        private Label lblDoku;
        private ComboBox cbxDoku;
        private Button btnDokumentazioa;
        private Button btnPlangintza;
        private Button btnJarraitu;
    }
}
