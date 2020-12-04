namespace Cash8
{
    partial class InputePhoneClient
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
            this.txtB_phone_number = new System.Windows.Forms.TextBox();
            this.label_zagolovok = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtB_phone_number
            // 
            this.txtB_phone_number.Location = new System.Drawing.Point(65, 78);
            this.txtB_phone_number.MaxLength = 10;
            this.txtB_phone_number.Name = "txtB_phone_number";
            this.txtB_phone_number.Size = new System.Drawing.Size(215, 20);
            this.txtB_phone_number.TabIndex = 0;
            // 
            // label_zagolovok
            // 
            this.label_zagolovok.Location = new System.Drawing.Point(-2, 10);
            this.label_zagolovok.Name = "label_zagolovok";
            this.label_zagolovok.Size = new System.Drawing.Size(347, 59);
            this.label_zagolovok.TabIndex = 1;
            this.label_zagolovok.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "+7";
            // 
            // InputePhoneClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 110);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_zagolovok);
            this.Controls.Add(this.txtB_phone_number);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputePhoneClient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Введите номер телефона";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label label_zagolovok;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtB_phone_number;
    }
}