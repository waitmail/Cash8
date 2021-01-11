namespace Cash8
{
    partial class Constants
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
            this.cash_desk_number = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.write = new System.Windows.Forms.Button();
            this._close_ = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nick_shop = new System.Windows.Forms.TextBox();
            this.use_debug = new System.Windows.Forms.CheckBox();
            this.path_for_web_service = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.currency = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.unloading_period = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtB_last_date_download_bonus_clients = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.checkBox_envd = new System.Windows.Forms.CheckBox();
            this.checkBox_print_m = new System.Windows.Forms.CheckBox();
            this.checkBox_osn_usnIncomeOutcome = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cash_desk_number
            // 
            this.cash_desk_number.Location = new System.Drawing.Point(127, 40);
            this.cash_desk_number.MaxLength = 1;
            this.cash_desk_number.Name = "cash_desk_number";
            this.cash_desk_number.Size = new System.Drawing.Size(75, 20);
            this.cash_desk_number.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "№ Кассы";
            // 
            // write
            // 
            this.write.Location = new System.Drawing.Point(237, 215);
            this.write.Name = "write";
            this.write.Size = new System.Drawing.Size(87, 23);
            this.write.TabIndex = 2;
            this.write.Text = "Записать";
            this.write.UseVisualStyleBackColor = true;
            this.write.Click += new System.EventHandler(this.write_Click);
            // 
            // _close_
            // 
            this._close_.Location = new System.Drawing.Point(360, 215);
            this._close_.Name = "_close_";
            this._close_.Size = new System.Drawing.Size(75, 23);
            this._close_.TabIndex = 3;
            this._close_.Text = "Закрыть";
            this._close_.UseVisualStyleBackColor = true;
            this._close_.Click += new System.EventHandler(this._close__Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Название магазина";
            // 
            // nick_shop
            // 
            this.nick_shop.Location = new System.Drawing.Point(127, 5);
            this.nick_shop.MaxLength = 5;
            this.nick_shop.Name = "nick_shop";
            this.nick_shop.Size = new System.Drawing.Size(75, 20);
            this.nick_shop.TabIndex = 5;
            // 
            // use_debug
            // 
            this.use_debug.AutoSize = true;
            this.use_debug.Enabled = false;
            this.use_debug.Location = new System.Drawing.Point(11, 78);
            this.use_debug.Name = "use_debug";
            this.use_debug.Size = new System.Drawing.Size(191, 17);
            this.use_debug.TabIndex = 22;
            this.use_debug.Text = "Отключить бонусную программу";
            this.use_debug.UseVisualStyleBackColor = true;
            // 
            // path_for_web_service
            // 
            this.path_for_web_service.Enabled = false;
            this.path_for_web_service.Location = new System.Drawing.Point(174, 179);
            this.path_for_web_service.MaxLength = 100;
            this.path_for_web_service.Name = "path_for_web_service";
            this.path_for_web_service.Size = new System.Drawing.Size(303, 20);
            this.path_for_web_service.TabIndex = 42;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(16, 182);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(152, 13);
            this.label15.TabIndex = 41;
            this.label15.Text = "Путь к вебсервису дисконта";
            // 
            // currency
            // 
            this.currency.Location = new System.Drawing.Point(269, 12);
            this.currency.MaxLength = 4;
            this.currency.Name = "currency";
            this.currency.Size = new System.Drawing.Size(43, 20);
            this.currency.TabIndex = 43;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(222, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 44;
            this.label8.Text = "Вал.";
            // 
            // unloading_period
            // 
            this.unloading_period.Location = new System.Drawing.Point(633, 103);
            this.unloading_period.MaxLength = 3;
            this.unloading_period.Name = "unloading_period";
            this.unloading_period.Size = new System.Drawing.Size(72, 20);
            this.unloading_period.TabIndex = 46;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(514, 90);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(100, 44);
            this.label11.TabIndex = 47;
            this.label11.Text = "Период выгрузки  в Центр (в минутах)";
            // 
            // txtB_last_date_download_bonus_clients
            // 
            this.txtB_last_date_download_bonus_clients.Location = new System.Drawing.Point(610, 28);
            this.txtB_last_date_download_bonus_clients.MaxLength = 15;
            this.txtB_last_date_download_bonus_clients.Name = "txtB_last_date_download_bonus_clients";
            this.txtB_last_date_download_bonus_clients.ReadOnly = true;
            this.txtB_last_date_download_bonus_clients.Size = new System.Drawing.Size(100, 20);
            this.txtB_last_date_download_bonus_clients.TabIndex = 53;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(514, 12);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(90, 53);
            this.label14.TabIndex = 54;
            this.label14.Text = "Дата последней загрузки карточек покупателей";
            // 
            // checkBox_envd
            // 
            this.checkBox_envd.AutoSize = true;
            this.checkBox_envd.Location = new System.Drawing.Point(11, 100);
            this.checkBox_envd.Name = "checkBox_envd";
            this.checkBox_envd.Size = new System.Drawing.Size(168, 17);
            this.checkBox_envd.TabIndex = 60;
            this.checkBox_envd.Text = "Магазин работает по ЕНВД";
            this.checkBox_envd.UseVisualStyleBackColor = true;
            // 
            // checkBox_print_m
            // 
            this.checkBox_print_m.AutoSize = true;
            this.checkBox_print_m.Location = new System.Drawing.Point(11, 123);
            this.checkBox_print_m.Name = "checkBox_print_m";
            this.checkBox_print_m.Size = new System.Drawing.Size(170, 17);
            this.checkBox_print_m.TabIndex = 61;
            this.checkBox_print_m.Text = "Печатать [M] перед товаром";
            this.checkBox_print_m.UseVisualStyleBackColor = true;
            // 
            // checkBox_osn_usnIncomeOutcome
            // 
            this.checkBox_osn_usnIncomeOutcome.AutoSize = true;
            this.checkBox_osn_usnIncomeOutcome.Location = new System.Drawing.Point(11, 147);
            this.checkBox_osn_usnIncomeOutcome.Name = "checkBox_osn_usnIncomeOutcome";
            this.checkBox_osn_usnIncomeOutcome.Size = new System.Drawing.Size(160, 17);
            this.checkBox_osn_usnIncomeOutcome.TabIndex = 62;
            this.checkBox_osn_usnIncomeOutcome.Text = "Магазин работает по УСН";
            this.checkBox_osn_usnIncomeOutcome.UseVisualStyleBackColor = true;
            // 
            // Constants
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 286);
            this.Controls.Add(this.checkBox_osn_usnIncomeOutcome);
            this.Controls.Add(this.checkBox_print_m);
            this.Controls.Add(this.checkBox_envd);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtB_last_date_download_bonus_clients);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.unloading_period);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.currency);
            this.Controls.Add(this.path_for_web_service);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.use_debug);
            this.Controls.Add(this.nick_shop);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._close_);
            this.Controls.Add(this.write);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cash_desk_number);
            this.Name = "Constants";
            this.Text = "Константы";
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private System.Windows.Forms.TextBox cash_desk_number;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button write;
        private System.Windows.Forms.Button _close_;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nick_shop;
        private System.Windows.Forms.CheckBox use_debug;
        private System.Windows.Forms.TextBox path_for_web_service;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox currency;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox unloading_period;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtB_last_date_download_bonus_clients;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox checkBox_envd;
        private System.Windows.Forms.CheckBox checkBox_print_m;
        private System.Windows.Forms.CheckBox checkBox_osn_usnIncomeOutcome;
    }
}