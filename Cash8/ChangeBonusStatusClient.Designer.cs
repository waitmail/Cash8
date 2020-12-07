namespace Cash8
{
    partial class ChangeBonusStatusClient
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
            this.txtB_phone = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_execute = new System.Windows.Forms.Button();
            this.maskedTxtB_new_phone_number = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // txtB_phone
            // 
            this.txtB_phone.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtB_phone.Enabled = false;
            this.txtB_phone.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_phone.Location = new System.Drawing.Point(154, 32);
            this.txtB_phone.Name = "txtB_phone";
            this.txtB_phone.Size = new System.Drawing.Size(376, 47);
            this.txtB_phone.TabIndex = 0;
            this.txtB_phone.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(6, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Номер телефона";
            // 
            // btn_execute
            // 
            this.btn_execute.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_execute.Location = new System.Drawing.Point(154, 154);
            this.btn_execute.Name = "btn_execute";
            this.btn_execute.Size = new System.Drawing.Size(376, 63);
            this.btn_execute.TabIndex = 2;
            this.btn_execute.Text = "Перейти на бонусы";
            this.btn_execute.UseVisualStyleBackColor = true;
            this.btn_execute.Click += new System.EventHandler(this.btn_execute_Click);
            // 
            // maskedTxtB_new_phone_number
            // 
            this.maskedTxtB_new_phone_number.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.maskedTxtB_new_phone_number.Location = new System.Drawing.Point(252, 92);
            this.maskedTxtB_new_phone_number.Mask = "+7 ### ### ## ##";
            this.maskedTxtB_new_phone_number.Name = "maskedTxtB_new_phone_number";
            this.maskedTxtB_new_phone_number.Size = new System.Drawing.Size(278, 47);
            this.maskedTxtB_new_phone_number.TabIndex = 3;
            this.maskedTxtB_new_phone_number.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ChangeBonusStatusClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 263);
            this.ControlBox = false;
            this.Controls.Add(this.maskedTxtB_new_phone_number);
            this.Controls.Add(this.btn_execute);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtB_phone);
            this.KeyPreview = true;
            this.Name = "ChangeBonusStatusClient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Перевести на КЕШ БЭК(потом надо поменять)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtB_phone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_execute;
        private System.Windows.Forms.MaskedTextBox maskedTxtB_new_phone_number;
    }
}