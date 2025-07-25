﻿namespace Cash8
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
            this.path_for_web_service = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.unloading_period = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtB_last_date_download_bonus_clients = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.comboBox_system_taxation = new System.Windows.Forms.ComboBox();
            this.lbl_system_taxation = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtB_version_fn = new System.Windows.Forms.TextBox();
            this.checkBox_enable_cdn_markers = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtB_id_acquiring_terminal = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtB_ip_address_acquiring_terminal = new System.Windows.Forms.TextBox();
            this.groupBox_acquiring_terminal = new System.Windows.Forms.GroupBox();
            this.comboBox_acquiring_bank = new System.Windows.Forms.ComboBox();
            this.checkBox_webservice_authorize = new System.Windows.Forms.CheckBox();
            this.checkBox_get_weight_automatically = new System.Windows.Forms.CheckBox();
            this.comboBox_scale_port = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtB_constant_conversion_to_kilograms = new System.Windows.Forms.TextBox();
            this.btn_get_weight = new System.Windows.Forms.Button();
            this.checkBox_printing_using_libraries = new System.Windows.Forms.CheckBox();
            this.comboBox_fn_port = new System.Windows.Forms.ComboBox();
            this.comboBox_variant_connect_fn = new System.Windows.Forms.ComboBox();
            this.btn_trst_connection = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtB_fn_ipaddr = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox_nds_ip = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox_acquiring_terminal.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.write.Location = new System.Drawing.Point(245, 507);
            this.write.Name = "write";
            this.write.Size = new System.Drawing.Size(87, 23);
            this.write.TabIndex = 2;
            this.write.Text = "Записать";
            this.write.UseVisualStyleBackColor = true;
            this.write.Click += new System.EventHandler(this.write_Click);
            // 
            // _close_
            // 
            this._close_.Location = new System.Drawing.Point(366, 507);
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
            // path_for_web_service
            // 
            this.path_for_web_service.Enabled = false;
            this.path_for_web_service.Location = new System.Drawing.Point(208, 454);
            this.path_for_web_service.MaxLength = 100;
            this.path_for_web_service.Name = "path_for_web_service";
            this.path_for_web_service.Size = new System.Drawing.Size(264, 20);
            this.path_for_web_service.TabIndex = 42;
            this.path_for_web_service.Visible = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(11, 457);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(102, 13);
            this.label15.TabIndex = 41;
            this.label15.Text = "Путь к вебсервису";
            this.label15.Visible = false;
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
            // comboBox_system_taxation
            // 
            this.comboBox_system_taxation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_system_taxation.FormattingEnabled = true;
            this.comboBox_system_taxation.Items.AddRange(new object[] {
            "НЕ ВЫБРАНО",
            "ОСН",
            "УСН (ДОХОДЫ МИНУС РАСХОДЫ)",
            "УСН (ДОХОДЫ МИНУС РАСХОДЫ) + ПАТЕНТ",
            "УСН ДОХОДЫ",
            "УСН ДОХОДЫ + ПАТЕНТ"});
            this.comboBox_system_taxation.Location = new System.Drawing.Point(127, 77);
            this.comboBox_system_taxation.Name = "comboBox_system_taxation";
            this.comboBox_system_taxation.Size = new System.Drawing.Size(359, 21);
            this.comboBox_system_taxation.TabIndex = 63;
            this.comboBox_system_taxation.SelectedIndexChanged += new System.EventHandler(this.comboBox_system_taxation_SelectedIndexChanged);
            // 
            // lbl_system_taxation
            // 
            this.lbl_system_taxation.Location = new System.Drawing.Point(12, 77);
            this.lbl_system_taxation.Name = "lbl_system_taxation";
            this.lbl_system_taxation.Size = new System.Drawing.Size(100, 31);
            this.lbl_system_taxation.TabIndex = 64;
            this.lbl_system_taxation.Text = "Система налогообложения";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(247, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 25);
            this.label4.TabIndex = 68;
            this.label4.Text = "Версия ФН";
            // 
            // txtB_version_fn
            // 
            this.txtB_version_fn.Enabled = false;
            this.txtB_version_fn.Location = new System.Drawing.Point(326, 5);
            this.txtB_version_fn.MaxLength = 1;
            this.txtB_version_fn.Name = "txtB_version_fn";
            this.txtB_version_fn.Size = new System.Drawing.Size(34, 20);
            this.txtB_version_fn.TabIndex = 69;
            this.txtB_version_fn.Text = "2";
            this.txtB_version_fn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // checkBox_enable_cdn_markers
            // 
            this.checkBox_enable_cdn_markers.AutoSize = true;
            this.checkBox_enable_cdn_markers.Location = new System.Drawing.Point(11, 149);
            this.checkBox_enable_cdn_markers.Name = "checkBox_enable_cdn_markers";
            this.checkBox_enable_cdn_markers.Size = new System.Drawing.Size(165, 17);
            this.checkBox_enable_cdn_markers.TabIndex = 70;
            this.checkBox_enable_cdn_markers.Text = "Включить CDN маркировку";
            this.checkBox_enable_cdn_markers.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(514, 243);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 31);
            this.label5.TabIndex = 72;
            this.label5.Text = "Ид эквайриного терминала";
            // 
            // txtB_id_acquiring_terminal
            // 
            this.txtB_id_acquiring_terminal.Location = new System.Drawing.Point(149, 112);
            this.txtB_id_acquiring_terminal.MaxLength = 8;
            this.txtB_id_acquiring_terminal.Name = "txtB_id_acquiring_terminal";
            this.txtB_id_acquiring_terminal.Size = new System.Drawing.Size(72, 20);
            this.txtB_id_acquiring_terminal.TabIndex = 73;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(514, 281);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 46);
            this.label6.TabIndex = 74;
            this.label6.Text = "IP адрес эквайриного терминала";
            // 
            // txtB_ip_address_acquiring_terminal
            // 
            this.txtB_ip_address_acquiring_terminal.Location = new System.Drawing.Point(595, 300);
            this.txtB_ip_address_acquiring_terminal.MaxLength = 21;
            this.txtB_ip_address_acquiring_terminal.Name = "txtB_ip_address_acquiring_terminal";
            this.txtB_ip_address_acquiring_terminal.Size = new System.Drawing.Size(129, 20);
            this.txtB_ip_address_acquiring_terminal.TabIndex = 75;
            // 
            // groupBox_acquiring_terminal
            // 
            this.groupBox_acquiring_terminal.Controls.Add(this.comboBox_acquiring_bank);
            this.groupBox_acquiring_terminal.Controls.Add(this.txtB_id_acquiring_terminal);
            this.groupBox_acquiring_terminal.Location = new System.Drawing.Point(503, 137);
            this.groupBox_acquiring_terminal.Name = "groupBox_acquiring_terminal";
            this.groupBox_acquiring_terminal.Size = new System.Drawing.Size(253, 232);
            this.groupBox_acquiring_terminal.TabIndex = 77;
            this.groupBox_acquiring_terminal.TabStop = false;
            this.groupBox_acquiring_terminal.Text = "Настройки эквайриногов терминала";
            // 
            // comboBox_acquiring_bank
            // 
            this.comboBox_acquiring_bank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_acquiring_bank.FormattingEnabled = true;
            this.comboBox_acquiring_bank.Items.AddRange(new object[] {
            "НЕ ВЫБРАНО",
            "РНКБ",
            "СБЕР"});
            this.comboBox_acquiring_bank.Location = new System.Drawing.Point(14, 51);
            this.comboBox_acquiring_bank.Name = "comboBox_acquiring_bank";
            this.comboBox_acquiring_bank.Size = new System.Drawing.Size(207, 21);
            this.comboBox_acquiring_bank.TabIndex = 74;
            // 
            // checkBox_webservice_authorize
            // 
            this.checkBox_webservice_authorize.AutoSize = true;
            this.checkBox_webservice_authorize.Location = new System.Drawing.Point(543, 492);
            this.checkBox_webservice_authorize.Name = "checkBox_webservice_authorize";
            this.checkBox_webservice_authorize.Size = new System.Drawing.Size(167, 17);
            this.checkBox_webservice_authorize.TabIndex = 80;
            this.checkBox_webservice_authorize.Text = "Веб сервис с авторизацией";
            this.checkBox_webservice_authorize.UseVisualStyleBackColor = true;
            // 
            // checkBox_get_weight_automatically
            // 
            this.checkBox_get_weight_automatically.Location = new System.Drawing.Point(25, 24);
            this.checkBox_get_weight_automatically.Name = "checkBox_get_weight_automatically";
            this.checkBox_get_weight_automatically.Size = new System.Drawing.Size(160, 33);
            this.checkBox_get_weight_automatically.TabIndex = 87;
            this.checkBox_get_weight_automatically.Text = "Получать вес с весов автоматически";
            this.checkBox_get_weight_automatically.UseVisualStyleBackColor = true;
            // 
            // comboBox_scale_port
            // 
            this.comboBox_scale_port.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_scale_port.FormattingEnabled = true;
            this.comboBox_scale_port.Location = new System.Drawing.Point(187, 24);
            this.comboBox_scale_port.Name = "comboBox_scale_port";
            this.comboBox_scale_port.Size = new System.Drawing.Size(127, 21);
            this.comboBox_scale_port.TabIndex = 88;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.txtB_constant_conversion_to_kilograms);
            this.groupBox2.Controls.Add(this.btn_get_weight);
            this.groupBox2.Controls.Add(this.comboBox_scale_port);
            this.groupBox2.Controls.Add(this.checkBox_get_weight_automatically);
            this.groupBox2.Location = new System.Drawing.Point(17, 325);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(469, 112);
            this.groupBox2.TabIndex = 89;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Настройки весов";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(25, 60);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(137, 35);
            this.label10.TabIndex = 91;
            this.label10.Text = "Делитель для перевода веса в килограммы";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtB_constant_conversion_to_kilograms
            // 
            this.txtB_constant_conversion_to_kilograms.Location = new System.Drawing.Point(187, 68);
            this.txtB_constant_conversion_to_kilograms.MaxLength = 4;
            this.txtB_constant_conversion_to_kilograms.Name = "txtB_constant_conversion_to_kilograms";
            this.txtB_constant_conversion_to_kilograms.Size = new System.Drawing.Size(127, 20);
            this.txtB_constant_conversion_to_kilograms.TabIndex = 90;
            this.txtB_constant_conversion_to_kilograms.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btn_get_weight
            // 
            this.btn_get_weight.Location = new System.Drawing.Point(327, 24);
            this.btn_get_weight.Name = "btn_get_weight";
            this.btn_get_weight.Size = new System.Drawing.Size(129, 23);
            this.btn_get_weight.TabIndex = 89;
            this.btn_get_weight.Text = "Получить вес";
            this.btn_get_weight.UseVisualStyleBackColor = true;
            this.btn_get_weight.Click += new System.EventHandler(this.btn_get_weight_Click);
            // 
            // checkBox_printing_using_libraries
            // 
            this.checkBox_printing_using_libraries.AutoSize = true;
            this.checkBox_printing_using_libraries.Location = new System.Drawing.Point(14, 22);
            this.checkBox_printing_using_libraries.Name = "checkBox_printing_using_libraries";
            this.checkBox_printing_using_libraries.Size = new System.Drawing.Size(154, 17);
            this.checkBox_printing_using_libraries.TabIndex = 82;
            this.checkBox_printing_using_libraries.Text = "Включить прямую печать";
            this.checkBox_printing_using_libraries.UseVisualStyleBackColor = true;
            // 
            // comboBox_fn_port
            // 
            this.comboBox_fn_port.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_fn_port.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox_fn_port.FormattingEnabled = true;
            this.comboBox_fn_port.Items.AddRange(new object[] {
            "USB ==> COM",
            "ETHERNET"});
            this.comboBox_fn_port.Location = new System.Drawing.Point(177, 58);
            this.comboBox_fn_port.Name = "comboBox_fn_port";
            this.comboBox_fn_port.Size = new System.Drawing.Size(154, 24);
            this.comboBox_fn_port.TabIndex = 83;
            // 
            // comboBox_variant_connect_fn
            // 
            this.comboBox_variant_connect_fn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_variant_connect_fn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox_variant_connect_fn.FormattingEnabled = true;
            this.comboBox_variant_connect_fn.Items.AddRange(new object[] {
            "USB==>COM",
            "ETHERNET"});
            this.comboBox_variant_connect_fn.Location = new System.Drawing.Point(177, 20);
            this.comboBox_variant_connect_fn.Name = "comboBox_variant_connect_fn";
            this.comboBox_variant_connect_fn.Size = new System.Drawing.Size(154, 24);
            this.comboBox_variant_connect_fn.TabIndex = 83;
            this.comboBox_variant_connect_fn.SelectedIndexChanged += new System.EventHandler(this.comboBox_variant_connect_fn_SelectedIndexChanged);
            // 
            // btn_trst_connection
            // 
            this.btn_trst_connection.Location = new System.Drawing.Point(338, 18);
            this.btn_trst_connection.Name = "btn_trst_connection";
            this.btn_trst_connection.Size = new System.Drawing.Size(92, 98);
            this.btn_trst_connection.TabIndex = 84;
            this.btn_trst_connection.Text = "Проверка соединения";
            this.btn_trst_connection.UseVisualStyleBackColor = true;
            this.btn_trst_connection.Click += new System.EventHandler(this.btn_trst_connection_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtB_fn_ipaddr);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.btn_trst_connection);
            this.groupBox1.Controls.Add(this.comboBox_variant_connect_fn);
            this.groupBox1.Controls.Add(this.comboBox_fn_port);
            this.groupBox1.Controls.Add(this.checkBox_printing_using_libraries);
            this.groupBox1.Location = new System.Drawing.Point(11, 170);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(475, 133);
            this.groupBox1.TabIndex = 85;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настроки прямой печати";
            // 
            // txtB_fn_ipaddr
            // 
            this.txtB_fn_ipaddr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_fn_ipaddr.Location = new System.Drawing.Point(177, 95);
            this.txtB_fn_ipaddr.Name = "txtB_fn_ipaddr";
            this.txtB_fn_ipaddr.Size = new System.Drawing.Size(154, 22);
            this.txtB_fn_ipaddr.TabIndex = 88;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(15, 93);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 23);
            this.label9.TabIndex = 87;
            this.label9.Text = "ETHERNET";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(14, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(137, 22);
            this.label7.TabIndex = 86;
            this.label7.Text = "USB ==> COM";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(571, 393);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 77);
            this.button1.TabIndex = 90;
            this.button1.Text = "Печать картинки";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox_nds_ip
            // 
            this.comboBox_nds_ip.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_nds_ip.FormattingEnabled = true;
            this.comboBox_nds_ip.Items.AddRange(new object[] {
            "Без НДС",
            "5",
            "7"});
            this.comboBox_nds_ip.Location = new System.Drawing.Point(127, 116);
            this.comboBox_nds_ip.Name = "comboBox_nds_ip";
            this.comboBox_nds_ip.Size = new System.Drawing.Size(359, 21);
            this.comboBox_nds_ip.TabIndex = 91;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(14, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 35);
            this.label3.TabIndex = 92;
            this.label3.Text = "Ставка % НДС для ИП";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Constants
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox_nds_ip);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox_webservice_authorize);
            this.Controls.Add(this.txtB_ip_address_acquiring_terminal);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBox_enable_cdn_markers);
            this.Controls.Add(this.txtB_version_fn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbl_system_taxation);
            this.Controls.Add(this.comboBox_system_taxation);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtB_last_date_download_bonus_clients);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.unloading_period);
            this.Controls.Add(this.path_for_web_service);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.nick_shop);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._close_);
            this.Controls.Add(this.write);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cash_desk_number);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox_acquiring_terminal);
            this.Name = "Constants";
            this.Text = "Константы";
            this.groupBox_acquiring_terminal.ResumeLayout(false);
            this.groupBox_acquiring_terminal.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.TextBox path_for_web_service;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox unloading_period;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtB_last_date_download_bonus_clients;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox comboBox_system_taxation;
        private System.Windows.Forms.Label lbl_system_taxation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtB_version_fn;
        private System.Windows.Forms.CheckBox checkBox_enable_cdn_markers;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtB_id_acquiring_terminal;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtB_ip_address_acquiring_terminal;
        private System.Windows.Forms.GroupBox groupBox_acquiring_terminal;
        private System.Windows.Forms.CheckBox checkBox_webservice_authorize;
        private System.Windows.Forms.CheckBox checkBox_get_weight_automatically;
        private System.Windows.Forms.ComboBox comboBox_scale_port;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_get_weight;
        private System.Windows.Forms.CheckBox checkBox_printing_using_libraries;
        private System.Windows.Forms.ComboBox comboBox_fn_port;
        private System.Windows.Forms.ComboBox comboBox_variant_connect_fn;
        private System.Windows.Forms.Button btn_trst_connection;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtB_fn_ipaddr;
        private System.Windows.Forms.ComboBox comboBox_acquiring_bank;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtB_constant_conversion_to_kilograms;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox_nds_ip;
        private System.Windows.Forms.Label label3;
    }
}