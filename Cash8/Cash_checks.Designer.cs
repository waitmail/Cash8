namespace Cash8
{
    partial class Cash_checks
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.button1 = new System.Windows.Forms.Button();
            this.num_cash = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fill = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.txtB_not_unloaded_docs = new System.Windows.Forms.TextBox();
            this.txtB_cashier = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_update_status_send = new System.Windows.Forms.Button();
            this.checkBox_show_3_last_checks = new System.Windows.Forms.CheckBox();
            this.btn_check_actions = new System.Windows.Forms.Button();
            this.pictureBox_get_update_program = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_get_update_program)).BeginInit();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(12, 101);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(776, 467);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(1028, 688);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Закрыть";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // num_cash
            // 
            this.num_cash.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.num_cash.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.num_cash.ForeColor = System.Drawing.Color.Maroon;
            this.num_cash.Location = new System.Drawing.Point(623, 68);
            this.num_cash.Name = "num_cash";
            this.num_cash.Size = new System.Drawing.Size(121, 24);
            this.num_cash.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "На дату";
            // 
            // fill
            // 
            this.fill.BackColor = System.Drawing.SystemColors.Control;
            this.fill.Location = new System.Drawing.Point(273, 66);
            this.fill.Name = "fill";
            this.fill.Size = new System.Drawing.Size(75, 22);
            this.fill.TabIndex = 14;
            this.fill.Text = "Заполнить";
            this.fill.UseVisualStyleBackColor = false;
            this.fill.Click += new System.EventHandler(this.fill_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(66, 66);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 13;
            // 
            // txtB_not_unloaded_docs
            // 
            this.txtB_not_unloaded_docs.BackColor = System.Drawing.Color.White;
            this.txtB_not_unloaded_docs.Enabled = false;
            this.txtB_not_unloaded_docs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_not_unloaded_docs.ForeColor = System.Drawing.SystemColors.MenuText;
            this.txtB_not_unloaded_docs.Location = new System.Drawing.Point(20, 4);
            this.txtB_not_unloaded_docs.MaxLength = 100;
            this.txtB_not_unloaded_docs.Multiline = true;
            this.txtB_not_unloaded_docs.Name = "txtB_not_unloaded_docs";
            this.txtB_not_unloaded_docs.Size = new System.Drawing.Size(481, 25);
            this.txtB_not_unloaded_docs.TabIndex = 17;
            // 
            // txtB_cashier
            // 
            this.txtB_cashier.CausesValidation = false;
            this.txtB_cashier.Enabled = false;
            this.txtB_cashier.Font = new System.Drawing.Font("FreeSans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_cashier.Location = new System.Drawing.Point(400, 68);
            this.txtB_cashier.MaxLength = 20;
            this.txtB_cashier.Name = "txtB_cashier";
            this.txtB_cashier.Size = new System.Drawing.Size(195, 21);
            this.txtB_cashier.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(353, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Кассир";
            // 
            // btn_update_status_send
            // 
            this.btn_update_status_send.Location = new System.Drawing.Point(507, 3);
            this.btn_update_status_send.Name = "btn_update_status_send";
            this.btn_update_status_send.Size = new System.Drawing.Size(67, 26);
            this.btn_update_status_send.TabIndex = 20;
            this.btn_update_status_send.Text = "Обновить";
            this.btn_update_status_send.UseVisualStyleBackColor = true;
            this.btn_update_status_send.Click += new System.EventHandler(this.btn_update_status_send_Click);
            // 
            // checkBox_show_3_last_checks
            // 
            this.checkBox_show_3_last_checks.AutoSize = true;
            this.checkBox_show_3_last_checks.Checked = true;
            this.checkBox_show_3_last_checks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_show_3_last_checks.Location = new System.Drawing.Point(753, 74);
            this.checkBox_show_3_last_checks.Name = "checkBox_show_3_last_checks";
            this.checkBox_show_3_last_checks.Size = new System.Drawing.Size(32, 17);
            this.checkBox_show_3_last_checks.TabIndex = 21;
            this.checkBox_show_3_last_checks.Text = "3";
            this.checkBox_show_3_last_checks.UseVisualStyleBackColor = true;
            this.checkBox_show_3_last_checks.CheckedChanged += new System.EventHandler(this.checkBox_show_3_last_checks_CheckedChanged);
            // 
            // btn_check_actions
            // 
            this.btn_check_actions.Location = new System.Drawing.Point(580, 4);
            this.btn_check_actions.Name = "btn_check_actions";
            this.btn_check_actions.Size = new System.Drawing.Size(125, 25);
            this.btn_check_actions.TabIndex = 22;
            this.btn_check_actions.Text = "ПРОВЕРКА АКЦИЙ";
            this.btn_check_actions.UseVisualStyleBackColor = true;
            this.btn_check_actions.Click += new System.EventHandler(this.btn_check_actions_Click);
            // 
            // pictureBox_get_update_program
            // 
            this.pictureBox_get_update_program.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox_get_update_program.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox_get_update_program.ErrorImage = null;
            this.pictureBox_get_update_program.Image = global::Cash8.Properties.Resources.ExistUpdateProgramm;
            this.pictureBox_get_update_program.InitialImage = null;
            this.pictureBox_get_update_program.Location = new System.Drawing.Point(720, 3);
            this.pictureBox_get_update_program.Name = "pictureBox_get_update_program";
            this.pictureBox_get_update_program.Size = new System.Drawing.Size(65, 50);
            this.pictureBox_get_update_program.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_get_update_program.TabIndex = 23;
            this.pictureBox_get_update_program.TabStop = false;
            this.pictureBox_get_update_program.Visible = false;
            this.pictureBox_get_update_program.Click += new System.EventHandler(this.pictureBox_get_update_program_Click);
            // 
            // Cash_checks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 580);
            this.Controls.Add(this.pictureBox_get_update_program);
            this.Controls.Add(this.btn_check_actions);
            this.Controls.Add(this.checkBox_show_3_last_checks);
            this.Controls.Add(this.btn_update_status_send);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtB_cashier);
            this.Controls.Add(this.txtB_not_unloaded_docs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fill);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.num_cash);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Cash_checks";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Кассовые чеки";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Cash_checks_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_get_update_program)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }






        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label num_cash;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button fill;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.TextBox txtB_not_unloaded_docs;
        private System.Windows.Forms.TextBox txtB_cashier;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_update_status_send;
        private System.Windows.Forms.CheckBox checkBox_show_3_last_checks;
        private System.Windows.Forms.Button btn_check_actions;
        private System.Windows.Forms.PictureBox pictureBox_get_update_program;
    }
}