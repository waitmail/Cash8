namespace Cash8
{
    partial class FPTK22
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtB_ofd_utility_status = new System.Windows.Forms.TextBox();
            this.btn_send_fiscal = new System.Windows.Forms.Button();
            this.btn_have_internet = new System.Windows.Forms.Button();
            this.txtB_ofd_exchange_status = new System.Windows.Forms.TextBox();
            this.btn_ofd_exchange_status = new System.Windows.Forms.Button();
            this.txtB_have_internet = new System.Windows.Forms.TextBox();
            this.print_last_check = new System.Windows.Forms.Button();
            this.annul_check = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.get_summ_in_cashe = new System.Windows.Forms.Button();
            this.sum_incass = new System.Windows.Forms.TextBox();
            this.incass = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.sum_avans = new System.Windows.Forms.TextBox();
            this.avans = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.z_report = new System.Windows.Forms.Button();
            this.x_report = new System.Windows.Forms.Button();
            this.txtB_fn_info = new System.Windows.Forms.TextBox();
            this.btn_reconciliation_of_totals = new System.Windows.Forms.Button();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtB_ofd_utility_status);
            this.groupBox4.Controls.Add(this.btn_send_fiscal);
            this.groupBox4.Controls.Add(this.btn_have_internet);
            this.groupBox4.Controls.Add(this.txtB_ofd_exchange_status);
            this.groupBox4.Controls.Add(this.btn_ofd_exchange_status);
            this.groupBox4.Controls.Add(this.txtB_have_internet);
            this.groupBox4.Location = new System.Drawing.Point(438, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(443, 337);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Проверки";
            // 
            // txtB_ofd_utility_status
            // 
            this.txtB_ofd_utility_status.Enabled = false;
            this.txtB_ofd_utility_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_ofd_utility_status.Location = new System.Drawing.Point(168, 148);
            this.txtB_ofd_utility_status.Name = "txtB_ofd_utility_status";
            this.txtB_ofd_utility_status.Size = new System.Drawing.Size(269, 35);
            this.txtB_ofd_utility_status.TabIndex = 23;
            this.txtB_ofd_utility_status.Visible = false;
            // 
            // btn_send_fiscal
            // 
            this.btn_send_fiscal.Location = new System.Drawing.Point(7, 154);
            this.btn_send_fiscal.Name = "btn_send_fiscal";
            this.btn_send_fiscal.Size = new System.Drawing.Size(155, 23);
            this.btn_send_fiscal.TabIndex = 22;
            this.btn_send_fiscal.Text = "Утилита отправки в ОФД";
            this.btn_send_fiscal.UseVisualStyleBackColor = true;
            this.btn_send_fiscal.Visible = false;
            this.btn_send_fiscal.Click += new System.EventHandler(this.btn_send_fiscal_Click);
            // 
            // btn_have_internet
            // 
            this.btn_have_internet.Location = new System.Drawing.Point(7, 20);
            this.btn_have_internet.Name = "btn_have_internet";
            this.btn_have_internet.Size = new System.Drawing.Size(155, 23);
            this.btn_have_internet.TabIndex = 21;
            this.btn_have_internet.Text = "Проверить интернет";
            this.btn_have_internet.UseVisualStyleBackColor = true;
            this.btn_have_internet.Click += new System.EventHandler(this.btn_have_internet_Click);
            // 
            // txtB_ofd_exchange_status
            // 
            this.txtB_ofd_exchange_status.Enabled = false;
            this.txtB_ofd_exchange_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_ofd_exchange_status.Location = new System.Drawing.Point(168, 55);
            this.txtB_ofd_exchange_status.MaxLength = 500;
            this.txtB_ofd_exchange_status.Multiline = true;
            this.txtB_ofd_exchange_status.Name = "txtB_ofd_exchange_status";
            this.txtB_ofd_exchange_status.Size = new System.Drawing.Size(269, 87);
            this.txtB_ofd_exchange_status.TabIndex = 2;
            // 
            // btn_ofd_exchange_status
            // 
            this.btn_ofd_exchange_status.Location = new System.Drawing.Point(7, 66);
            this.btn_ofd_exchange_status.Name = "btn_ofd_exchange_status";
            this.btn_ofd_exchange_status.Size = new System.Drawing.Size(155, 23);
            this.btn_ofd_exchange_status.TabIndex = 1;
            this.btn_ofd_exchange_status.Text = "Проверить отправку в офд";
            this.btn_ofd_exchange_status.UseVisualStyleBackColor = true;
            this.btn_ofd_exchange_status.Click += new System.EventHandler(this.btn_ofd_exchange_status_Click);
            // 
            // txtB_have_internet
            // 
            this.txtB_have_internet.Enabled = false;
            this.txtB_have_internet.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_have_internet.Location = new System.Drawing.Point(168, 15);
            this.txtB_have_internet.Name = "txtB_have_internet";
            this.txtB_have_internet.Size = new System.Drawing.Size(269, 35);
            this.txtB_have_internet.TabIndex = 0;
            // 
            // print_last_check
            // 
            this.print_last_check.Location = new System.Drawing.Point(267, 134);
            this.print_last_check.Name = "print_last_check";
            this.print_last_check.Size = new System.Drawing.Size(163, 23);
            this.print_last_check.TabIndex = 18;
            this.print_last_check.Text = "Копия последнего чека";
            this.print_last_check.UseVisualStyleBackColor = true;
            this.print_last_check.Click += new System.EventHandler(this.print_last_check_Click);
            // 
            // annul_check
            // 
            this.annul_check.Location = new System.Drawing.Point(267, 166);
            this.annul_check.Name = "annul_check";
            this.annul_check.Size = new System.Drawing.Size(163, 23);
            this.annul_check.TabIndex = 17;
            this.annul_check.Text = "Отмена последнего задания";
            this.annul_check.UseVisualStyleBackColor = true;
            this.annul_check.Click += new System.EventHandler(this.annul_check_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.get_summ_in_cashe);
            this.groupBox3.Controls.Add(this.sum_incass);
            this.groupBox3.Controls.Add(this.incass);
            this.groupBox3.Location = new System.Drawing.Point(12, 117);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(242, 85);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Инкассация";
            // 
            // get_summ_in_cashe
            // 
            this.get_summ_in_cashe.Location = new System.Drawing.Point(17, 20);
            this.get_summ_in_cashe.Name = "get_summ_in_cashe";
            this.get_summ_in_cashe.Size = new System.Drawing.Size(101, 21);
            this.get_summ_in_cashe.TabIndex = 21;
            this.get_summ_in_cashe.Text = "Сумма в ДЯ";
            this.get_summ_in_cashe.UseVisualStyleBackColor = true;
            this.get_summ_in_cashe.Click += new System.EventHandler(this.get_summ_in_cashe_Click);
            // 
            // sum_incass
            // 
            this.sum_incass.Location = new System.Drawing.Point(125, 19);
            this.sum_incass.Name = "sum_incass";
            this.sum_incass.Size = new System.Drawing.Size(100, 20);
            this.sum_incass.TabIndex = 6;
            this.sum_incass.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // incass
            // 
            this.incass.Location = new System.Drawing.Point(14, 49);
            this.incass.Name = "incass";
            this.incass.Size = new System.Drawing.Size(211, 23);
            this.incass.TabIndex = 0;
            this.incass.Text = "Служебная выдача";
            this.incass.UseVisualStyleBackColor = true;
            this.incass.Click += new System.EventHandler(this.incass_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Сумма аванса";
            // 
            // sum_avans
            // 
            this.sum_avans.Location = new System.Drawing.Point(137, 36);
            this.sum_avans.Name = "sum_avans";
            this.sum_avans.Size = new System.Drawing.Size(100, 20);
            this.sum_avans.TabIndex = 12;
            this.sum_avans.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // avans
            // 
            this.avans.Location = new System.Drawing.Point(24, 64);
            this.avans.Name = "avans";
            this.avans.Size = new System.Drawing.Size(213, 23);
            this.avans.TabIndex = 11;
            this.avans.Text = "Служебное внесение";
            this.avans.UseVisualStyleBackColor = true;
            this.avans.Click += new System.EventHandler(this.avans_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(242, 87);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Внесение аванса";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.z_report);
            this.groupBox2.Controls.Add(this.x_report);
            this.groupBox2.Location = new System.Drawing.Point(270, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(160, 87);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Отчеты";
            // 
            // z_report
            // 
            this.z_report.Location = new System.Drawing.Point(7, 50);
            this.z_report.Name = "z_report";
            this.z_report.Size = new System.Drawing.Size(144, 23);
            this.z_report.TabIndex = 4;
            this.z_report.Text = "Суточный z отчет";
            this.z_report.UseVisualStyleBackColor = true;
            this.z_report.Click += new System.EventHandler(this.z_report_Click);
            // 
            // x_report
            // 
            this.x_report.Location = new System.Drawing.Point(7, 17);
            this.x_report.Name = "x_report";
            this.x_report.Size = new System.Drawing.Size(144, 23);
            this.x_report.TabIndex = 3;
            this.x_report.Text = "Текущий х отчет";
            this.x_report.UseVisualStyleBackColor = true;
            this.x_report.Click += new System.EventHandler(this.x_report_Click);
            // 
            // txtB_fn_info
            // 
            this.txtB_fn_info.Enabled = false;
            this.txtB_fn_info.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_fn_info.Location = new System.Drawing.Point(445, 208);
            this.txtB_fn_info.Multiline = true;
            this.txtB_fn_info.Name = "txtB_fn_info";
            this.txtB_fn_info.Size = new System.Drawing.Size(430, 128);
            this.txtB_fn_info.TabIndex = 21;
            // 
            // btn_reconciliation_of_totals
            // 
            this.btn_reconciliation_of_totals.Enabled = false;
            this.btn_reconciliation_of_totals.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_reconciliation_of_totals.Location = new System.Drawing.Point(26, 208);
            this.btn_reconciliation_of_totals.Name = "btn_reconciliation_of_totals";
            this.btn_reconciliation_of_totals.Size = new System.Drawing.Size(211, 58);
            this.btn_reconciliation_of_totals.TabIndex = 22;
            this.btn_reconciliation_of_totals.Text = "Сверка итогов\r\n(Банк. терминал)";
            this.btn_reconciliation_of_totals.UseVisualStyleBackColor = true;
            this.btn_reconciliation_of_totals.Click += new System.EventHandler(this.btn_reconciliation_of_totals_Click);
            // 
            // FPTK22
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 366);
            this.Controls.Add(this.btn_reconciliation_of_totals);
            this.Controls.Add(this.txtB_fn_info);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.print_last_check);
            this.Controls.Add(this.annul_check);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sum_avans);
            this.Controls.Add(this.avans);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "FPTK22";
            this.Text = "Фискальный регистратор";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button print_last_check;
        private System.Windows.Forms.Button annul_check;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox sum_incass;
        private System.Windows.Forms.Button incass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sum_avans;
        private System.Windows.Forms.Button avans;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button z_report;
        private System.Windows.Forms.Button x_report;
        private System.Windows.Forms.Button get_summ_in_cashe;
        private System.Windows.Forms.Button btn_have_internet;
        private System.Windows.Forms.TextBox txtB_have_internet;
        private System.Windows.Forms.Button btn_ofd_exchange_status;
        private System.Windows.Forms.TextBox txtB_ofd_exchange_status;
        private System.Windows.Forms.TextBox txtB_ofd_utility_status;
        private System.Windows.Forms.Button btn_send_fiscal;
        private System.Windows.Forms.TextBox txtB_fn_info;
        private System.Windows.Forms.Button btn_reconciliation_of_totals;
    }
}