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
            this.checkBox_print_m = new System.Windows.Forms.CheckBox();
            this.comboBox_system_taxation = new System.Windows.Forms.ComboBox();
            this.lbl_system_taxation = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtB_work_schema = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtB_version_fn = new System.Windows.Forms.TextBox();
            this.checkBox_enable_stock_processing_in_memory = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtB_id_acquiring_terminal = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtB_ip_address_acquiring_terminal = new System.Windows.Forms.TextBox();
            this.checkBox_self_service_kiosk = new System.Windows.Forms.CheckBox();
            this.groupBox_acquiring_terminal = new System.Windows.Forms.GroupBox();
            this.checkBox_one_monitors_connected = new System.Windows.Forms.CheckBox();
            this.checkBox_version2_marking = new System.Windows.Forms.CheckBox();
            this.checkBox_webservice_authorize = new System.Windows.Forms.CheckBox();
            this.checkBox_static_guid_in_print = new System.Windows.Forms.CheckBox();
            this.checkBox_printing_using_libraries = new System.Windows.Forms.CheckBox();
            this.groupBox_acquiring_terminal.SuspendLayout();
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
            this.write.Location = new System.Drawing.Point(239, 324);
            this.write.Name = "write";
            this.write.Size = new System.Drawing.Size(87, 23);
            this.write.TabIndex = 2;
            this.write.Text = "Записать";
            this.write.UseVisualStyleBackColor = true;
            this.write.Click += new System.EventHandler(this.write_Click);
            // 
            // _close_
            // 
            this._close_.Location = new System.Drawing.Point(360, 324);
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
            this.path_for_web_service.Location = new System.Drawing.Point(209, 247);
            this.path_for_web_service.MaxLength = 100;
            this.path_for_web_service.Name = "path_for_web_service";
            this.path_for_web_service.Size = new System.Drawing.Size(264, 20);
            this.path_for_web_service.TabIndex = 42;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(12, 250);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(152, 13);
            this.label15.TabIndex = 41;
            this.label15.Text = "Путь к вебсервису дисконта";
            // 
            // currency
            // 
            this.currency.Location = new System.Drawing.Point(253, 12);
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
            this.unloading_period.Location = new System.Drawing.Point(652, 103);
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
            // checkBox_print_m
            // 
            this.checkBox_print_m.AutoSize = true;
            this.checkBox_print_m.Location = new System.Drawing.Point(11, 103);
            this.checkBox_print_m.Name = "checkBox_print_m";
            this.checkBox_print_m.Size = new System.Drawing.Size(170, 17);
            this.checkBox_print_m.TabIndex = 61;
            this.checkBox_print_m.Text = "Печатать [M] перед товаром";
            this.checkBox_print_m.UseVisualStyleBackColor = true;
            // 
            // comboBox_system_taxation
            // 
            this.comboBox_system_taxation.FormattingEnabled = true;
            this.comboBox_system_taxation.Items.AddRange(new object[] {
            "Не выбрано",
            "ОСН",
            "УСН",
            " УСН + ПАТЕНТ"});
            this.comboBox_system_taxation.Location = new System.Drawing.Point(209, 206);
            this.comboBox_system_taxation.Name = "comboBox_system_taxation";
            this.comboBox_system_taxation.Size = new System.Drawing.Size(264, 21);
            this.comboBox_system_taxation.TabIndex = 63;
            this.comboBox_system_taxation.SelectedIndexChanged += new System.EventHandler(this.comboBox_system_taxation_SelectedIndexChanged);
            // 
            // lbl_system_taxation
            // 
            this.lbl_system_taxation.Location = new System.Drawing.Point(8, 196);
            this.lbl_system_taxation.Name = "lbl_system_taxation";
            this.lbl_system_taxation.Size = new System.Drawing.Size(100, 31);
            this.lbl_system_taxation.TabIndex = 64;
            this.lbl_system_taxation.Text = "Система налогообложения";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(319, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 65;
            this.label3.Text = "Схема";
            // 
            // txtB_work_schema
            // 
            this.txtB_work_schema.Enabled = false;
            this.txtB_work_schema.Location = new System.Drawing.Point(360, 12);
            this.txtB_work_schema.MaxLength = 1;
            this.txtB_work_schema.Name = "txtB_work_schema";
            this.txtB_work_schema.Size = new System.Drawing.Size(34, 20);
            this.txtB_work_schema.TabIndex = 66;
            this.txtB_work_schema.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 25);
            this.label4.TabIndex = 68;
            this.label4.Text = "Версия ФН";
            // 
            // txtB_version_fn
            // 
            this.txtB_version_fn.Location = new System.Drawing.Point(209, 175);
            this.txtB_version_fn.MaxLength = 1;
            this.txtB_version_fn.Name = "txtB_version_fn";
            this.txtB_version_fn.Size = new System.Drawing.Size(42, 20);
            this.txtB_version_fn.TabIndex = 69;
            // 
            // checkBox_enable_stock_processing_in_memory
            // 
            this.checkBox_enable_stock_processing_in_memory.AutoSize = true;
            this.checkBox_enable_stock_processing_in_memory.Enabled = false;
            this.checkBox_enable_stock_processing_in_memory.Location = new System.Drawing.Point(209, 78);
            this.checkBox_enable_stock_processing_in_memory.Name = "checkBox_enable_stock_processing_in_memory";
            this.checkBox_enable_stock_processing_in_memory.Size = new System.Drawing.Size(212, 17);
            this.checkBox_enable_stock_processing_in_memory.TabIndex = 70;
            this.checkBox_enable_stock_processing_in_memory.Text = "Включить обработку акций в памяти";
            this.checkBox_enable_stock_processing_in_memory.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(514, 190);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 31);
            this.label5.TabIndex = 72;
            this.label5.Text = "Ид эквайриного терминала";
            // 
            // txtB_id_acquiring_terminal
            // 
            this.txtB_id_acquiring_terminal.Location = new System.Drawing.Point(149, 59);
            this.txtB_id_acquiring_terminal.MaxLength = 8;
            this.txtB_id_acquiring_terminal.Name = "txtB_id_acquiring_terminal";
            this.txtB_id_acquiring_terminal.Size = new System.Drawing.Size(72, 20);
            this.txtB_id_acquiring_terminal.TabIndex = 73;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(514, 228);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 46);
            this.label6.TabIndex = 74;
            this.label6.Text = "IP адрес эквайриного терминала";
            // 
            // txtB_ip_address_acquiring_terminal
            // 
            this.txtB_ip_address_acquiring_terminal.Location = new System.Drawing.Point(595, 247);
            this.txtB_ip_address_acquiring_terminal.MaxLength = 21;
            this.txtB_ip_address_acquiring_terminal.Name = "txtB_ip_address_acquiring_terminal";
            this.txtB_ip_address_acquiring_terminal.Size = new System.Drawing.Size(129, 20);
            this.txtB_ip_address_acquiring_terminal.TabIndex = 75;
            // 
            // checkBox_self_service_kiosk
            // 
            this.checkBox_self_service_kiosk.AutoSize = true;
            this.checkBox_self_service_kiosk.Location = new System.Drawing.Point(514, 291);
            this.checkBox_self_service_kiosk.Name = "checkBox_self_service_kiosk";
            this.checkBox_self_service_kiosk.Size = new System.Drawing.Size(179, 17);
            this.checkBox_self_service_kiosk.TabIndex = 76;
            this.checkBox_self_service_kiosk.Text = "Это киоск самообслуживания";
            this.checkBox_self_service_kiosk.UseVisualStyleBackColor = true;
            // 
            // groupBox_acquiring_terminal
            // 
            this.groupBox_acquiring_terminal.Controls.Add(this.txtB_id_acquiring_terminal);
            this.groupBox_acquiring_terminal.Location = new System.Drawing.Point(503, 140);
            this.groupBox_acquiring_terminal.Name = "groupBox_acquiring_terminal";
            this.groupBox_acquiring_terminal.Size = new System.Drawing.Size(253, 178);
            this.groupBox_acquiring_terminal.TabIndex = 77;
            this.groupBox_acquiring_terminal.TabStop = false;
            this.groupBox_acquiring_terminal.Text = "Настройки эквайриногов терминала";
            // 
            // checkBox_one_monitors_connected
            // 
            this.checkBox_one_monitors_connected.AutoSize = true;
            this.checkBox_one_monitors_connected.Location = new System.Drawing.Point(209, 105);
            this.checkBox_one_monitors_connected.Name = "checkBox_one_monitors_connected";
            this.checkBox_one_monitors_connected.Size = new System.Drawing.Size(156, 17);
            this.checkBox_one_monitors_connected.TabIndex = 78;
            this.checkBox_one_monitors_connected.Text = "Подключен один монитор";
            this.checkBox_one_monitors_connected.UseVisualStyleBackColor = true;
            // 
            // checkBox_version2_marking
            // 
            this.checkBox_version2_marking.AutoSize = true;
            this.checkBox_version2_marking.Location = new System.Drawing.Point(209, 137);
            this.checkBox_version2_marking.Name = "checkBox_version2_marking";
            this.checkBox_version2_marking.Size = new System.Drawing.Size(137, 17);
            this.checkBox_version2_marking.TabIndex = 79;
            this.checkBox_version2_marking.Text = "Версия маркировки 2";
            this.checkBox_version2_marking.UseVisualStyleBackColor = true;
            // 
            // checkBox_webservice_authorize
            // 
            this.checkBox_webservice_authorize.AutoSize = true;
            this.checkBox_webservice_authorize.Location = new System.Drawing.Point(11, 137);
            this.checkBox_webservice_authorize.Name = "checkBox_webservice_authorize";
            this.checkBox_webservice_authorize.Size = new System.Drawing.Size(167, 17);
            this.checkBox_webservice_authorize.TabIndex = 80;
            this.checkBox_webservice_authorize.Text = "Веб сервис с авторизацией";
            this.checkBox_webservice_authorize.UseVisualStyleBackColor = true;
            // 
            // checkBox_static_guid_in_print
            // 
            this.checkBox_static_guid_in_print.AutoSize = true;
            this.checkBox_static_guid_in_print.Location = new System.Drawing.Point(209, 42);
            this.checkBox_static_guid_in_print.Name = "checkBox_static_guid_in_print";
            this.checkBox_static_guid_in_print.Size = new System.Drawing.Size(231, 17);
            this.checkBox_static_guid_in_print.TabIndex = 81;
            this.checkBox_static_guid_in_print.Text = "Статический uuid(документа) при печати";
            this.checkBox_static_guid_in_print.UseVisualStyleBackColor = true;
            // 
            // checkBox_printing_using_libraries
            // 
            this.checkBox_printing_using_libraries.AutoSize = true;
            this.checkBox_printing_using_libraries.Location = new System.Drawing.Point(257, 178);
            this.checkBox_printing_using_libraries.Name = "checkBox_printing_using_libraries";
            this.checkBox_printing_using_libraries.Size = new System.Drawing.Size(216, 17);
            this.checkBox_printing_using_libraries.TabIndex = 82;
            this.checkBox_printing_using_libraries.Text = "Печать с использованием библиотек";
            this.checkBox_printing_using_libraries.UseVisualStyleBackColor = true;
            // 
            // Constants
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 359);
            this.Controls.Add(this.checkBox_printing_using_libraries);
            this.Controls.Add(this.checkBox_static_guid_in_print);
            this.Controls.Add(this.checkBox_webservice_authorize);
            this.Controls.Add(this.checkBox_version2_marking);
            this.Controls.Add(this.checkBox_one_monitors_connected);
            this.Controls.Add(this.checkBox_self_service_kiosk);
            this.Controls.Add(this.txtB_ip_address_acquiring_terminal);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBox_enable_stock_processing_in_memory);
            this.Controls.Add(this.txtB_version_fn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtB_work_schema);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbl_system_taxation);
            this.Controls.Add(this.comboBox_system_taxation);
            this.Controls.Add(this.checkBox_print_m);
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
            this.Controls.Add(this.groupBox_acquiring_terminal);
            this.Name = "Constants";
            this.Text = "Константы";
            this.groupBox_acquiring_terminal.ResumeLayout(false);
            this.groupBox_acquiring_terminal.PerformLayout();
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
        private System.Windows.Forms.CheckBox checkBox_print_m;
        private System.Windows.Forms.ComboBox comboBox_system_taxation;
        private System.Windows.Forms.Label lbl_system_taxation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtB_work_schema;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtB_version_fn;
        private System.Windows.Forms.CheckBox checkBox_enable_stock_processing_in_memory;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtB_id_acquiring_terminal;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtB_ip_address_acquiring_terminal;
        private System.Windows.Forms.CheckBox checkBox_self_service_kiosk;
        private System.Windows.Forms.GroupBox groupBox_acquiring_terminal;
        private System.Windows.Forms.CheckBox checkBox_one_monitors_connected;
        private System.Windows.Forms.CheckBox checkBox_version2_marking;
        private System.Windows.Forms.CheckBox checkBox_webservice_authorize;
        private System.Windows.Forms.CheckBox checkBox_static_guid_in_print;
        private System.Windows.Forms.CheckBox checkBox_printing_using_libraries;
    }
}