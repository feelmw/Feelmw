namespace FeelmwLogistika
{
    internal static class AppEstiloa
    {
        public static readonly Color Fondoa = Color.FromArgb(243, 246, 250);
        public static readonly Color Txartela = Color.White;
        public static readonly Color TestuNagusia = Color.FromArgb(31, 41, 55);
        public static readonly Color TestuBiguna = Color.FromArgb(89, 104, 122);
        public static readonly Color Urdina = Color.FromArgb(37, 99, 235);
        public static readonly Color UrdinaHover = Color.FromArgb(29, 78, 216);
        public static readonly Color Berdea = Color.FromArgb(22, 163, 74);
        public static readonly Color BerdeaHover = Color.FromArgb(21, 128, 61);
        public static readonly Color Gorria = Color.FromArgb(220, 38, 38);
        public static readonly Color GorriaHover = Color.FromArgb(185, 28, 28);

        public static void Aplikatu(Form form)
        {
            form.BackColor = Fondoa;
            form.Font = new Font("Segoe UI", 10F);
            KontrolakAplikatu(form.Controls);
        }

        public static void KontrolakAplikatu(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                switch (control)
                {
                    case Label label:
                        LabelaAplikatu(label);
                        break;
                    case Button button:
                        BotoiaAplikatu(button);
                        break;
                    case Panel panel:
                        PanelaAplikatu(panel);
                        break;
                    case TextBox textBox:
                        TestuKutxaAplikatu(textBox);
                        break;
                    case ComboBox comboBox:
                        ComboKutxaAplikatu(comboBox);
                        break;
                    case NumericUpDown numeric:
                        ZenbakiKutxaAplikatu(numeric);
                        break;
                    case DateTimePicker dateTimePicker:
                        DataKutxaAplikatu(dateTimePicker);
                        break;
                    case CheckBox checkBox:
                        CheckKutxaAplikatu(checkBox);
                        break;
                    case DataGridView dataGridView:
                        TaulaAplikatu(dataGridView);
                        break;
                }

                if (control.HasChildren)
                {
                    KontrolakAplikatu(control.Controls);
                }
            }
        }

        private static void LabelaAplikatu(Label label)
        {
            label.BackColor = Color.Transparent;
            label.ForeColor = label.Font.Size >= 20 ? TestuNagusia : TestuBiguna;
            if (label.Font.Size >= 20)
            {
                label.Font = new Font("Segoe UI Semibold", label.Font.Size, FontStyle.Bold);
            }
        }

        private static void BotoiaAplikatu(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.Font = new Font("Segoe UI Semibold", Math.Max(10F, button.Font.Size), FontStyle.Bold);
            button.UseVisualStyleBackColor = false;

            bool arriskua = button.BackColor.R > 180 && button.BackColor.G < 90;
            bool ona = button.BackColor.G > 130 && button.BackColor.R < 90;
            bool txartelBotoia = button.BackColor == Color.White || button.BackColor == SystemColors.ControlLightLight;
            bool nagusia = !txartelBotoia && button.BackColor.B > 150;

            if (arriskua)
            {
                button.BackColor = Gorria;
                button.ForeColor = Color.White;
                button.FlatAppearance.MouseOverBackColor = GorriaHover;
            }
            else if (ona)
            {
                button.BackColor = Berdea;
                button.ForeColor = Color.White;
                button.FlatAppearance.MouseOverBackColor = BerdeaHover;
            }
            else if (nagusia)
            {
                button.BackColor = Urdina;
                button.ForeColor = Color.White;
                button.FlatAppearance.MouseOverBackColor = UrdinaHover;
            }
            else if (txartelBotoia)
            {
                button.BackColor = Color.White;
                button.ForeColor = TestuNagusia;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(248, 250, 252);
            }
        }

        private static void PanelaAplikatu(Panel panel)
        {
            if (panel.BackColor == Color.Transparent)
            {
                return;
            }

            panel.BackColor = panel.Dock == DockStyle.Fill && panel.Parent is Form ? Fondoa : Txartela;
            panel.BorderStyle = BorderStyle.None;
            if (panel.BackColor == Txartela)
            {
                panel.Paint += TxartelaPaint;
            }
        }

        private static void TxartelaPaint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel)
            {
                return;
            }

            using Pen pen = new Pen(Color.FromArgb(226, 232, 240));
            Rectangle rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
            e.Graphics.DrawRectangle(pen, rect);
        }

        private static void TestuKutxaAplikatu(TextBox textBox)
        {
            textBox.BackColor = Color.White;
            textBox.ForeColor = TestuNagusia;
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void ComboKutxaAplikatu(ComboBox comboBox)
        {
            comboBox.BackColor = Color.White;
            comboBox.ForeColor = TestuNagusia;
            comboBox.FlatStyle = FlatStyle.Flat;
        }

        private static void ZenbakiKutxaAplikatu(NumericUpDown numeric)
        {
            numeric.BackColor = Color.White;
            numeric.ForeColor = TestuNagusia;
            numeric.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void DataKutxaAplikatu(DateTimePicker dateTimePicker)
        {
            dateTimePicker.CalendarMonthBackground = Color.White;
            dateTimePicker.CalendarForeColor = TestuNagusia;
        }

        private static void CheckKutxaAplikatu(CheckBox checkBox)
        {
            checkBox.ForeColor = TestuNagusia;
            checkBox.BackColor = Color.Transparent;
            checkBox.Font = new Font("Segoe UI", 10F);
        }

        private static void TaulaAplikatu(DataGridView dataGridView)
        {
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.GridColor = Color.FromArgb(226, 232, 240);
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(239, 246, 255);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = TestuNagusia;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            dataGridView.DefaultCellStyle.BackColor = Color.White;
            dataGridView.DefaultCellStyle.ForeColor = TestuNagusia;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dataGridView.DefaultCellStyle.SelectionForeColor = TestuNagusia;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dataGridView.RowTemplate.Height = 34;
        }
    }
}
