namespace Cash8
{
    partial class InputCodeNumberPhone
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
            this.txtB_last_6_number_code_phone = new System.Windows.Forms.TextBox();
            this.btn_send_sms = new System.Windows.Forms.Button();
            this.btn_check_code = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtB_last_6_number_code_phone
            // 
            this.txtB_last_6_number_code_phone.Location = new System.Drawing.Point(17, 42);
            this.txtB_last_6_number_code_phone.MaxLength = 6;
            this.txtB_last_6_number_code_phone.Name = "txtB_last_6_number_code_phone";
            this.txtB_last_6_number_code_phone.Size = new System.Drawing.Size(296, 20);
            this.txtB_last_6_number_code_phone.TabIndex = 0;
            // 
            // btn_send_sms
            // 
            //this.btn_send_sms.Location = new System.Drawing.Point(17, 72);
            //this.btn_send_sms.Name = "btn_send_sms";
            //this.btn_send_sms.Size = new System.Drawing.Size(97, 23);
            //this.btn_send_sms.TabIndex = 1;
            //this.btn_send_sms.Text = "Отправить смс";
            //this.btn_send_sms.UseVisualStyleBackColor = true;
            //this.btn_send_sms.Click += new System.EventHandler(this.btn_send_sms_Click);
            // 
            // btn_check_code
            // 
            this.btn_check_code.Enabled = false;
            this.btn_check_code.Location = new System.Drawing.Point(130, 71);
            this.btn_check_code.Name = "btn_check_code";
            this.btn_check_code.Size = new System.Drawing.Size(96, 23);
            this.btn_check_code.TabIndex = 2;
            this.btn_check_code.Text = "Проверить код";
            this.btn_check_code.UseVisualStyleBackColor = true;
            this.btn_check_code.Click += new System.EventHandler(this.btn_check_code_Click);
            // 
            // btn_close
            // 
            this.btn_close.Location = new System.Drawing.Point(242, 72);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(70, 23);
            this.btn_close.TabIndex = 3;
            this.btn_close.Text = "Закрыть";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 4;
            // 
            // InputCodeNumberPhone
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 107);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.btn_check_code);
            this.Controls.Add(this.btn_send_sms);
            this.Controls.Add(this.txtB_last_6_number_code_phone);
            this.Name = "InputCodeNumberPhone";
            this.Text = "Проверка корректности кода";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtB_last_6_number_code_phone;
        private System.Windows.Forms.Button btn_send_sms;
        private System.Windows.Forms.Button btn_check_code;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Label label1;
    }
}