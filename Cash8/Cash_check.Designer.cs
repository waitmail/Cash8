﻿namespace Cash8
{
    partial class Cash_check
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
            this.comment = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.client = new System.Windows.Forms.TextBox();
            this.client_barcode = new System.Windows.Forms.TextBox();
            this.inputbarcode = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.enter_quantity = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.listView2 = new System.Windows.Forms.ListView();
            this.select_tovar = new System.Windows.Forms.TextBox();
            this.panel_return = new System.Windows.Forms.Panel();
            this.return_enter = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.return_kop = new System.Windows.Forms.TextBox();
            this.return_rouble = new System.Windows.Forms.TextBox();
            this.return_quantity = new System.Windows.Forms.TextBox();
            this.pay = new System.Windows.Forms.Button();
            this.last_tovar = new System.Windows.Forms.TextBox();
            this.date_time_start = new System.Windows.Forms.TextBox();
            this.num_cash = new System.Windows.Forms.TextBox();
            this.user = new System.Windows.Forms.TextBox();
            this.status_com_scaner = new System.Windows.Forms.Label();
            this.check_type = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listView_sertificates = new System.Windows.Forms.ListView();
            this.txtB_sertificate_money = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtB_non_cash_money = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtB_cash_money = new System.Windows.Forms.TextBox();
            this.txtB_email_telephone = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtB_client_phone = new System.Windows.Forms.TextBox();
            this.label_client_barcode = new System.Windows.Forms.Label();
            this.label_client_phone = new System.Windows.Forms.Label();
            this.btn_inpute_phone_client = new System.Windows.Forms.Button();
            this.btn_change_status_client = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.txtB_bonus_money = new System.Windows.Forms.TextBox();
            this.checkBox_to_print_repeatedly = new System.Windows.Forms.CheckBox();
            this.lbl_inn = new System.Windows.Forms.Label();
            this.txtB_inn = new System.Windows.Forms.TextBox();
            this.btn_get_name = new System.Windows.Forms.Button();
            this.txtB_name = new System.Windows.Forms.TextBox();
            this.txtB_num_sales = new System.Windows.Forms.TextBox();
            this.btn_fill_on_sales = new System.Windows.Forms.Button();
            this.cash = new System.Windows.Forms.TextBox();
            this.checkBox_to_print_repeatedly_p = new System.Windows.Forms.CheckBox();
            this.txtB_total_sum = new System.Windows.Forms.TextBox();
            this.txtB_num_doc = new System.Windows.Forms.TextBox();
            this.checkBox_print_check = new System.Windows.Forms.CheckBox();
            this.checkBox_payment_by_sbp = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel_return.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comment
            // 
            this.comment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comment.Location = new System.Drawing.Point(81, 567);
            this.comment.MaxLength = 50;
            this.comment.Name = "comment";
            this.comment.Size = new System.Drawing.Size(192, 20);
            this.comment.TabIndex = 3;
            this.comment.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 572);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Коментарий";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Клиент";
            // 
            // client
            // 
            this.client.Enabled = false;
            this.client.Location = new System.Drawing.Point(61, 42);
            this.client.MaxLength = 200;
            this.client.Name = "client";
            this.client.Size = new System.Drawing.Size(185, 20);
            this.client.TabIndex = 17;
            // 
            // client_barcode
            // 
            this.client_barcode.BackColor = System.Drawing.SystemColors.Window;
            this.client_barcode.Location = new System.Drawing.Point(366, 45);
            this.client_barcode.MaxLength = 13;
            this.client_barcode.Name = "client_barcode";
            this.client_barcode.Size = new System.Drawing.Size(99, 20);
            this.client_barcode.TabIndex = 0;
            this.client_barcode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.client_barcode_KeyPress);
            // 
            // inputbarcode
            // 
            this.inputbarcode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.inputbarcode.BackColor = System.Drawing.SystemColors.Window;
            this.inputbarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.inputbarcode.ForeColor = System.Drawing.Color.Green;
            this.inputbarcode.Location = new System.Drawing.Point(611, 123);
            this.inputbarcode.MaxLength = 13;
            this.inputbarcode.Name = "inputbarcode";
            this.inputbarcode.Size = new System.Drawing.Size(177, 29);
            this.inputbarcode.TabIndex = 2;
            this.inputbarcode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.inputbarcode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.inputbarcode_KeyPress);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.enter_quantity);
            this.panel1.Location = new System.Drawing.Point(21, 351);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(166, 44);
            this.panel1.TabIndex = 21;
            this.panel1.Visible = false;
            // 
            // enter_quantity
            // 
            this.enter_quantity.BackColor = System.Drawing.Color.White;
            this.enter_quantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.enter_quantity.Location = new System.Drawing.Point(3, 8);
            this.enter_quantity.MaxLength = 10;
            this.enter_quantity.Name = "enter_quantity";
            this.enter_quantity.Size = new System.Drawing.Size(160, 29);
            this.enter_quantity.TabIndex = 0;
            this.enter_quantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.enter_quantity.Visible = false;
            this.enter_quantity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.enter_quantity_KeyPress);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.ForestGreen;
            this.panel2.Controls.Add(this.listView2);
            this.panel2.Controls.Add(this.select_tovar);
            this.panel2.Location = new System.Drawing.Point(16, 34);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(745, 243);
            this.panel2.TabIndex = 23;
            this.panel2.Visible = false;
            // 
            // listView2
            // 
            this.listView2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listView2.HideSelection = false;
            this.listView2.Location = new System.Drawing.Point(22, 53);
            this.listView2.MultiSelect = false;
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(698, 147);
            this.listView2.TabIndex = 0;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.Visible = false;
            this.listView2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listView2_KeyPress);
            // 
            // select_tovar
            // 
            this.select_tovar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.select_tovar.Enabled = false;
            this.select_tovar.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.select_tovar.Location = new System.Drawing.Point(22, 18);
            this.select_tovar.Name = "select_tovar";
            this.select_tovar.Size = new System.Drawing.Size(698, 29);
            this.select_tovar.TabIndex = 1;
            // 
            // panel_return
            // 
            this.panel_return.Controls.Add(this.return_enter);
            this.panel_return.Controls.Add(this.label7);
            this.panel_return.Controls.Add(this.label6);
            this.panel_return.Controls.Add(this.label5);
            this.panel_return.Controls.Add(this.return_kop);
            this.panel_return.Controls.Add(this.return_rouble);
            this.panel_return.Controls.Add(this.return_quantity);
            this.panel_return.Location = new System.Drawing.Point(24, 298);
            this.panel_return.Name = "panel_return";
            this.panel_return.Size = new System.Drawing.Size(432, 88);
            this.panel_return.TabIndex = 42;
            this.panel_return.Visible = false;
            // 
            // return_enter
            // 
            this.return_enter.Location = new System.Drawing.Point(170, 59);
            this.return_enter.Name = "return_enter";
            this.return_enter.Size = new System.Drawing.Size(89, 23);
            this.return_enter.TabIndex = 6;
            this.return_enter.Text = "Ввод";
            this.return_enter.UseVisualStyleBackColor = true;
            this.return_enter.Click += new System.EventHandler(this.return_enter_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Количество";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(198, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Цена";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(335, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 24);
            this.label5.TabIndex = 3;
            this.label5.Text = ".";
            // 
            // return_kop
            // 
            this.return_kop.Enabled = false;
            this.return_kop.Location = new System.Drawing.Point(352, 27);
            this.return_kop.MaxLength = 2;
            this.return_kop.Name = "return_kop";
            this.return_kop.Size = new System.Drawing.Size(57, 20);
            this.return_kop.TabIndex = 2;
            this.return_kop.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // return_rouble
            // 
            this.return_rouble.Enabled = false;
            this.return_rouble.Location = new System.Drawing.Point(237, 27);
            this.return_rouble.MaxLength = 12;
            this.return_rouble.Name = "return_rouble";
            this.return_rouble.Size = new System.Drawing.Size(100, 20);
            this.return_rouble.TabIndex = 1;
            // 
            // return_quantity
            // 
            this.return_quantity.Location = new System.Drawing.Point(93, 29);
            this.return_quantity.MaxLength = 10;
            this.return_quantity.Name = "return_quantity";
            this.return_quantity.Size = new System.Drawing.Size(100, 20);
            this.return_quantity.TabIndex = 0;
            // 
            // pay
            // 
            this.pay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pay.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pay.Location = new System.Drawing.Point(639, 544);
            this.pay.Name = "pay";
            this.pay.Size = new System.Drawing.Size(157, 48);
            this.pay.TabIndex = 25;
            this.pay.Text = "Оплатить/Вернуть(F8)";
            this.pay.UseVisualStyleBackColor = true;
            this.pay.Click += new System.EventHandler(this.pay_Click);
            // 
            // last_tovar
            // 
            this.last_tovar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.last_tovar.BackColor = System.Drawing.SystemColors.Window;
            this.last_tovar.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.last_tovar.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.last_tovar.Location = new System.Drawing.Point(13, 68);
            this.last_tovar.MaxLength = 200;
            this.last_tovar.Multiline = true;
            this.last_tovar.Name = "last_tovar";
            this.last_tovar.ReadOnly = true;
            this.last_tovar.Size = new System.Drawing.Size(775, 47);
            this.last_tovar.TabIndex = 29;
            // 
            // date_time_start
            // 
            this.date_time_start.Location = new System.Drawing.Point(520, 8);
            this.date_time_start.Name = "date_time_start";
            this.date_time_start.ReadOnly = true;
            this.date_time_start.Size = new System.Drawing.Size(141, 20);
            this.date_time_start.TabIndex = 32;
            // 
            // num_cash
            // 
            this.num_cash.Location = new System.Drawing.Point(414, 7);
            this.num_cash.Name = "num_cash";
            this.num_cash.ReadOnly = true;
            this.num_cash.Size = new System.Drawing.Size(100, 20);
            this.num_cash.TabIndex = 31;
            this.num_cash.Text = "КАССА №";
            // 
            // user
            // 
            this.user.BackColor = System.Drawing.SystemColors.Control;
            this.user.Location = new System.Drawing.Point(667, 8);
            this.user.Name = "user";
            this.user.Size = new System.Drawing.Size(119, 20);
            this.user.TabIndex = 35;
            // 
            // status_com_scaner
            // 
            this.status_com_scaner.AutoSize = true;
            this.status_com_scaner.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.status_com_scaner.Location = new System.Drawing.Point(875, 8);
            this.status_com_scaner.Name = "status_com_scaner";
            this.status_com_scaner.Size = new System.Drawing.Size(0, 20);
            this.status_com_scaner.TabIndex = 36;
            // 
            // check_type
            // 
            this.check_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.check_type.FormattingEnabled = true;
            this.check_type.Location = new System.Drawing.Point(67, 5);
            this.check_type.Name = "check_type";
            this.check_type.Size = new System.Drawing.Size(103, 21);
            this.check_type.TabIndex = 39;
            this.check_type.SelectedIndexChanged += new System.EventHandler(this.check_type_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 40;
            this.label4.Text = "Тип чека";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(8, 175);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(791, 363);
            this.tabControl1.TabIndex = 46;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel_return);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.listView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(783, 337);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Товары";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(780, 334);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listView_sertificates);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(783, 337);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Сертификаты";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listView_sertificates
            // 
            this.listView_sertificates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView_sertificates.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listView_sertificates.HideSelection = false;
            this.listView_sertificates.Location = new System.Drawing.Point(3, 0);
            this.listView_sertificates.Name = "listView_sertificates";
            this.listView_sertificates.Size = new System.Drawing.Size(922, 362);
            this.listView_sertificates.TabIndex = 0;
            this.listView_sertificates.UseCompatibleStateImageBehavior = false;
            // 
            // txtB_sertificate_money
            // 
            this.txtB_sertificate_money.Enabled = false;
            this.txtB_sertificate_money.Location = new System.Drawing.Point(88, 128);
            this.txtB_sertificate_money.Name = "txtB_sertificate_money";
            this.txtB_sertificate_money.Size = new System.Drawing.Size(53, 20);
            this.txtB_sertificate_money.TabIndex = 47;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 133);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 13);
            this.label9.TabIndex = 48;
            this.label9.Text = "Сертификаты";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(144, 131);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 13);
            this.label10.TabIndex = 49;
            this.label10.Text = "Карта оплаты";
            // 
            // txtB_non_cash_money
            // 
            this.txtB_non_cash_money.Enabled = false;
            this.txtB_non_cash_money.Location = new System.Drawing.Point(222, 127);
            this.txtB_non_cash_money.Name = "txtB_non_cash_money";
            this.txtB_non_cash_money.Size = new System.Drawing.Size(51, 20);
            this.txtB_non_cash_money.TabIndex = 50;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(330, 133);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 13);
            this.label11.TabIndex = 51;
            this.label11.Text = "Наличные";
            // 
            // txtB_cash_money
            // 
            this.txtB_cash_money.Enabled = false;
            this.txtB_cash_money.Location = new System.Drawing.Point(388, 129);
            this.txtB_cash_money.Name = "txtB_cash_money";
            this.txtB_cash_money.Size = new System.Drawing.Size(53, 20);
            this.txtB_cash_money.TabIndex = 52;
            // 
            // txtB_email_telephone
            // 
            this.txtB_email_telephone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtB_email_telephone.Location = new System.Drawing.Point(570, 157);
            this.txtB_email_telephone.MaxLength = 50;
            this.txtB_email_telephone.Name = "txtB_email_telephone";
            this.txtB_email_telephone.Size = new System.Drawing.Size(216, 20);
            this.txtB_email_telephone.TabIndex = 53;
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(484, 160);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(86, 13);
            this.label12.TabIndex = 54;
            this.label12.Text = "Email / телефон";
            // 
            // txtB_client_phone
            // 
            this.txtB_client_phone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtB_client_phone.Location = new System.Drawing.Point(545, 45);
            this.txtB_client_phone.MaxLength = 10;
            this.txtB_client_phone.Name = "txtB_client_phone";
            this.txtB_client_phone.Size = new System.Drawing.Size(72, 20);
            this.txtB_client_phone.TabIndex = 1;
            // 
            // label_client_barcode
            // 
            this.label_client_barcode.AutoSize = true;
            this.label_client_barcode.Location = new System.Drawing.Point(290, 49);
            this.label_client_barcode.Name = "label_client_barcode";
            this.label_client_barcode.Size = new System.Drawing.Size(70, 13);
            this.label_client_barcode.TabIndex = 58;
            this.label_client_barcode.Text = "Код клиента";
            // 
            // label_client_phone
            // 
            this.label_client_phone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_client_phone.AutoSize = true;
            this.label_client_phone.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label_client_phone.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_client_phone.Location = new System.Drawing.Point(470, 47);
            this.label_client_phone.Name = "label_client_phone";
            this.label_client_phone.Size = new System.Drawing.Size(69, 15);
            this.label_client_phone.TabIndex = 59;
            this.label_client_phone.Text = "Телефон +7";
            this.label_client_phone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_inpute_phone_client
            // 
            this.btn_inpute_phone_client.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_inpute_phone_client.Location = new System.Drawing.Point(-230, 100);
            this.btn_inpute_phone_client.Name = "btn_inpute_phone_client";
            this.btn_inpute_phone_client.Size = new System.Drawing.Size(82, 22);
            this.btn_inpute_phone_client.TabIndex = 60;
            this.btn_inpute_phone_client.Text = "Создать";
            this.btn_inpute_phone_client.UseVisualStyleBackColor = true;
            this.btn_inpute_phone_client.Visible = false;
            this.btn_inpute_phone_client.Click += new System.EventHandler(this.btn_inpute_phone_client_Click);
            // 
            // btn_change_status_client
            // 
            //this.btn_change_status_client.Location = new System.Drawing.Point(252, 43);
            //this.btn_change_status_client.Name = "btn_change_status_client";
            //this.btn_change_status_client.Size = new System.Drawing.Size(32, 23);
            //this.btn_change_status_client.TabIndex = 61;
            //this.btn_change_status_client.Text = "...";
            //this.btn_change_status_client.UseVisualStyleBackColor = true;
            //this.btn_change_status_client.Visible = false;
            //this.btn_change_status_client.Click += new System.EventHandler(this.btn_change_status_client_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(444, 134);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(45, 13);
            this.label13.TabIndex = 62;
            this.label13.Text = "Бонусы";
            // 
            // txtB_bonus_money
            // 
            this.txtB_bonus_money.Enabled = false;
            this.txtB_bonus_money.Location = new System.Drawing.Point(490, 129);
            this.txtB_bonus_money.Name = "txtB_bonus_money";
            this.txtB_bonus_money.Size = new System.Drawing.Size(50, 20);
            this.txtB_bonus_money.TabIndex = 63;
            // 
            // checkBox_to_print_repeatedly
            // 
            this.checkBox_to_print_repeatedly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_to_print_repeatedly.AutoSize = true;
            this.checkBox_to_print_repeatedly.Enabled = false;
            this.checkBox_to_print_repeatedly.Location = new System.Drawing.Point(278, 544);
            this.checkBox_to_print_repeatedly.Name = "checkBox_to_print_repeatedly";
            this.checkBox_to_print_repeatedly.Size = new System.Drawing.Size(135, 17);
            this.checkBox_to_print_repeatedly.TabIndex = 64;
            this.checkBox_to_print_repeatedly.Text = "Напечатать повторно";
            this.checkBox_to_print_repeatedly.UseVisualStyleBackColor = true;
            this.checkBox_to_print_repeatedly.Visible = false;
            // 
            // lbl_inn
            // 
            this.lbl_inn.AutoSize = true;
            this.lbl_inn.Location = new System.Drawing.Point(8, 156);
            this.lbl_inn.Name = "lbl_inn";
            this.lbl_inn.Size = new System.Drawing.Size(31, 13);
            this.lbl_inn.TabIndex = 65;
            this.lbl_inn.Text = "ИНН";
            // 
            // txtB_inn
            // 
            this.txtB_inn.Location = new System.Drawing.Point(45, 153);
            this.txtB_inn.MaxLength = 12;
            this.txtB_inn.Name = "txtB_inn";
            this.txtB_inn.Size = new System.Drawing.Size(83, 20);
            this.txtB_inn.TabIndex = 66;
            // 
            // btn_get_name
            // 
            this.btn_get_name.Location = new System.Drawing.Point(135, 154);
            this.btn_get_name.Name = "btn_get_name";
            this.btn_get_name.Size = new System.Drawing.Size(64, 23);
            this.btn_get_name.TabIndex = 67;
            this.btn_get_name.Text = "==>";
            this.btn_get_name.UseVisualStyleBackColor = true;
            this.btn_get_name.Click += new System.EventHandler(this.btn_get_name_Click);
            // 
            // txtB_name
            // 
            this.txtB_name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtB_name.Location = new System.Drawing.Point(205, 156);
            this.txtB_name.MaxLength = 250;
            this.txtB_name.Name = "txtB_name";
            this.txtB_name.Size = new System.Drawing.Size(273, 20);
            this.txtB_name.TabIndex = 68;
            // 
            // txtB_num_sales
            // 
            this.txtB_num_sales.Location = new System.Drawing.Point(175, 6);
            this.txtB_num_sales.MaxLength = 10;
            this.txtB_num_sales.Name = "txtB_num_sales";
            this.txtB_num_sales.Size = new System.Drawing.Size(98, 20);
            this.txtB_num_sales.TabIndex = 69;
            this.txtB_num_sales.Visible = false;
            // 
            // btn_fill_on_sales
            // 
            this.btn_fill_on_sales.Location = new System.Drawing.Point(279, 4);
            this.btn_fill_on_sales.Name = "btn_fill_on_sales";
            this.btn_fill_on_sales.Size = new System.Drawing.Size(131, 23);
            this.btn_fill_on_sales.TabIndex = 70;
            this.btn_fill_on_sales.Text = "Заполнить по продаже";
            this.btn_fill_on_sales.UseVisualStyleBackColor = true;
            this.btn_fill_on_sales.Visible = false;
            this.btn_fill_on_sales.Click += new System.EventHandler(this.btn_fill_on_sales_Click);
            // 
            // cash
            // 
            this.cash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cash.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cash.ForeColor = System.Drawing.Color.Black;
            this.cash.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cash.Location = new System.Drawing.Point(45, 25);
            this.cash.MaxLength = 10;
            this.cash.Name = "cash";
            this.cash.Size = new System.Drawing.Size(122, 23);
            this.cash.TabIndex = 8;
            this.cash.Text = "0.00";
            this.cash.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.cash.Visible = false;
            this.cash.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cash_KeyPress);
            // 
            // checkBox_to_print_repeatedly_p
            // 
            this.checkBox_to_print_repeatedly_p.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_to_print_repeatedly_p.AutoSize = true;
            this.checkBox_to_print_repeatedly_p.Enabled = false;
            this.checkBox_to_print_repeatedly_p.Location = new System.Drawing.Point(278, 566);
            this.checkBox_to_print_repeatedly_p.Name = "checkBox_to_print_repeatedly_p";
            this.checkBox_to_print_repeatedly_p.Size = new System.Drawing.Size(149, 17);
            this.checkBox_to_print_repeatedly_p.TabIndex = 73;
            this.checkBox_to_print_repeatedly_p.Text = "Напечатать маркировку";
            this.checkBox_to_print_repeatedly_p.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_to_print_repeatedly_p.UseVisualStyleBackColor = true;
            this.checkBox_to_print_repeatedly_p.Visible = false;
            this.checkBox_to_print_repeatedly_p.CheckedChanged += new System.EventHandler(this.checkBox_to_print_repeatedly_p_CheckedChanged);
            // 
            // txtB_total_sum
            // 
            this.txtB_total_sum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtB_total_sum.BackColor = System.Drawing.SystemColors.Window;
            this.txtB_total_sum.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_total_sum.ForeColor = System.Drawing.Color.Blue;
            this.txtB_total_sum.Location = new System.Drawing.Point(422, 542);
            this.txtB_total_sum.Name = "txtB_total_sum";
            this.txtB_total_sum.ReadOnly = true;
            this.txtB_total_sum.Size = new System.Drawing.Size(214, 31);
            this.txtB_total_sum.TabIndex = 76;
            this.txtB_total_sum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtB_num_doc
            // 
            this.txtB_num_doc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtB_num_doc.Enabled = false;
            this.txtB_num_doc.Location = new System.Drawing.Point(545, 130);
            this.txtB_num_doc.Name = "txtB_num_doc";
            this.txtB_num_doc.Size = new System.Drawing.Size(52, 20);
            this.txtB_num_doc.TabIndex = 77;
            this.txtB_num_doc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // checkBox_print_check
            // 
            this.checkBox_print_check.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_print_check.AutoSize = true;
            this.checkBox_print_check.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBox_print_check.Location = new System.Drawing.Point(471, 564);
            this.checkBox_print_check.Name = "checkBox_print_check";
            this.checkBox_print_check.Size = new System.Drawing.Size(149, 28);
            this.checkBox_print_check.TabIndex = 78;
            this.checkBox_print_check.Text = "Печатать чек";
            this.checkBox_print_check.UseVisualStyleBackColor = true;
            this.checkBox_print_check.Visible = false;
            // 
            // checkBox_payment_by_sbp
            // 
            this.checkBox_payment_by_sbp.AutoSize = true;
            this.checkBox_payment_by_sbp.Enabled = false;
            this.checkBox_payment_by_sbp.Location = new System.Drawing.Point(278, 132);
            this.checkBox_payment_by_sbp.Name = "checkBox_payment_by_sbp";
            this.checkBox_payment_by_sbp.Size = new System.Drawing.Size(48, 17);
            this.checkBox_payment_by_sbp.TabIndex = 79;
            this.checkBox_payment_by_sbp.Text = "СБП";
            this.checkBox_payment_by_sbp.UseVisualStyleBackColor = true;
            this.checkBox_payment_by_sbp.Visible = false;
            // 
            // Cash_check
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.checkBox_payment_by_sbp);
            this.Controls.Add(this.checkBox_print_check);
            this.Controls.Add(this.txtB_num_doc);
            this.Controls.Add(this.checkBox_to_print_repeatedly_p);
            this.Controls.Add(this.btn_fill_on_sales);
            this.Controls.Add(this.txtB_num_sales);
            this.Controls.Add(this.txtB_name);
            this.Controls.Add(this.btn_get_name);
            this.Controls.Add(this.txtB_inn);
            this.Controls.Add(this.lbl_inn);
            this.Controls.Add(this.checkBox_to_print_repeatedly);
            this.Controls.Add(this.txtB_bonus_money);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.btn_change_status_client);
            this.Controls.Add(this.btn_inpute_phone_client);
            this.Controls.Add(this.label_client_phone);
            this.Controls.Add(this.label_client_barcode);
            this.Controls.Add(this.txtB_client_phone);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtB_email_telephone);
            this.Controls.Add(this.txtB_cash_money);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtB_non_cash_money);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtB_sertificate_money);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.check_type);
            this.Controls.Add(this.status_com_scaner);
            this.Controls.Add(this.user);
            this.Controls.Add(this.num_cash);
            this.Controls.Add(this.date_time_start);
            this.Controls.Add(this.pay);
            this.Controls.Add(this.cash);
            this.Controls.Add(this.inputbarcode);
            this.Controls.Add(this.client_barcode);
            this.Controls.Add(this.client);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comment);
            this.Controls.Add(this.last_tovar);
            this.Controls.Add(this.txtB_total_sum);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Cash_check";
            this.Text = "Cash_check";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Cash_check_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel_return.ResumeLayout(false);
            this.panel_return.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.TextBox comment;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox client_barcode;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox enter_quantity;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.Button pay;
        private System.Windows.Forms.TextBox last_tovar;
        private System.Windows.Forms.TextBox date_time_start;
        private System.Windows.Forms.Label status_com_scaner;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox inputbarcode;
        public System.Windows.Forms.TextBox client;
        public System.Windows.Forms.TextBox num_cash;
        private System.Windows.Forms.TextBox select_tovar;
        private System.Windows.Forms.Panel panel_return;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox return_kop;
        private System.Windows.Forms.TextBox return_rouble;
        private System.Windows.Forms.TextBox return_quantity;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button return_enter;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        public System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TabPage tabPage2;
        public System.Windows.Forms.ListView listView_sertificates;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.ComboBox check_type;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtB_client_phone;
        private System.Windows.Forms.Label label_client_barcode;
        private System.Windows.Forms.Label label_client_phone;
        private System.Windows.Forms.Button btn_inpute_phone_client;
        private System.Windows.Forms.Button btn_change_status_client;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtB_bonus_money;
        private System.Windows.Forms.CheckBox checkBox_to_print_repeatedly;
        private System.Windows.Forms.Label lbl_inn;
        private System.Windows.Forms.Button btn_get_name;
        private System.Windows.Forms.Button btn_fill_on_sales;
        private System.Windows.Forms.TextBox cash;
        private System.Windows.Forms.CheckBox checkBox_to_print_repeatedly_p;
        private System.Windows.Forms.TextBox txtB_total_sum;
        private System.Windows.Forms.TextBox txtB_num_doc;
        private System.Windows.Forms.CheckBox checkBox_print_check;
        public System.Windows.Forms.TextBox user;
        public System.Windows.Forms.TextBox txtB_email_telephone;
        public System.Windows.Forms.TextBox txtB_inn;
        public System.Windows.Forms.TextBox txtB_name;
        public System.Windows.Forms.TextBox txtB_num_sales;
        public System.Windows.Forms.TextBox txtB_sertificate_money;
        public System.Windows.Forms.TextBox txtB_non_cash_money;
        public System.Windows.Forms.TextBox txtB_cash_money;
        public System.Windows.Forms.CheckBox checkBox_payment_by_sbp;
    }
}