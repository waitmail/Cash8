namespace Cash8
{
    partial class ParametersReceiptCorrection
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
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Ok = new System.Windows.Forms.Button();
            this.txtB_tax_order = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtB_password = new System.Windows.Forms.TextBox();
            this.btn_enable = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Salmon;
            this.label1.Location = new System.Drawing.Point(17, 152);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(512, 159);
            this.label1.TabIndex = 0;
            this.label1.Text = "Если вы случайно попали в это окно тогда нажмите отмена!";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(12, 319);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(156, 47);
            this.btn_Cancel.TabIndex = 1;
            this.btn_Cancel.Text = "ОТМЕНА";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Ok
            // 
            this.btn_Ok.Enabled = false;
            this.btn_Ok.Location = new System.Drawing.Point(355, 319);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(174, 47);
            this.btn_Ok.TabIndex = 2;
            this.btn_Ok.Text = "ОК";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // txtB_tax_order
            // 
            this.txtB_tax_order.Enabled = false;
            this.txtB_tax_order.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_tax_order.Location = new System.Drawing.Point(241, 100);
            this.txtB_tax_order.MaxLength = 100;
            this.txtB_tax_order.Name = "txtB_tax_order";
            this.txtB_tax_order.Size = new System.Drawing.Size(288, 40);
            this.txtB_tax_order.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(2, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(233, 49);
            this.label2.TabIndex = 4;
            this.label2.Text = "Номер налогового поручения если он есть";
            // 
            // txtB_password
            // 
            this.txtB_password.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_password.Location = new System.Drawing.Point(178, 27);
            this.txtB_password.MaxLength = 10;
            this.txtB_password.Name = "txtB_password";
            this.txtB_password.PasswordChar = '*';
            this.txtB_password.Size = new System.Drawing.Size(147, 29);
            this.txtB_password.TabIndex = 5;
            // 
            // btn_enable
            // 
            this.btn_enable.Location = new System.Drawing.Point(344, 27);
            this.btn_enable.Name = "btn_enable";
            this.btn_enable.Size = new System.Drawing.Size(150, 29);
            this.btn_enable.TabIndex = 6;
            this.btn_enable.Text = "Получить доступ";
            this.btn_enable.UseVisualStyleBackColor = true;
            this.btn_enable.Click += new System.EventHandler(this.btn_enable_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(13, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 24);
            this.label3.TabIndex = 7;
            this.label3.Text = "Введите пароль";
            // 
            // ParametersReceiptCorrection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 378);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_enable);
            this.Controls.Add(this.txtB_password);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtB_tax_order);
            this.Controls.Add(this.btn_Ok);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.label1);
            this.Name = "ParametersReceiptCorrection";
            this.Text = "Для бухгалтера";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtB_tax_order;
        private System.Windows.Forms.TextBox txtB_password;
        private System.Windows.Forms.Button btn_enable;
        private System.Windows.Forms.Label label3;
    }
}