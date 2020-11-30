namespace Cash8
{
    partial class SentDataOnBonus
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
            this.sent = new System.Windows.Forms.Button();
            this._close_ = new System.Windows.Forms.Button();
            this.txtB_jason = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // sent
            // 
            this.sent.Location = new System.Drawing.Point(34, 439);
            this.sent.Name = "sent";
            this.sent.Size = new System.Drawing.Size(122, 23);
            this.sent.TabIndex = 0;
            this.sent.Text = "Отправить";
            this.sent.UseVisualStyleBackColor = true;
            this.sent.Click += new System.EventHandler(this.sent_Click);
            // 
            // _close_
            // 
            this._close_.Location = new System.Drawing.Point(247, 439);
            this._close_.Name = "_close_";
            this._close_.Size = new System.Drawing.Size(140, 23);
            this._close_.TabIndex = 1;
            this._close_.Text = "Закрыть";
            this._close_.UseVisualStyleBackColor = true;
            this._close_.Click += new System.EventHandler(this._close__Click);
            // 
            // txtB_jason
            // 
            this.txtB_jason.Location = new System.Drawing.Point(24, 41);
            this.txtB_jason.Multiline = true;
            this.txtB_jason.Name = "txtB_jason";
            this.txtB_jason.Size = new System.Drawing.Size(390, 371);
            this.txtB_jason.TabIndex = 2;
            // 
            // SentDataOnBonus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 487);
            this.Controls.Add(this.txtB_jason);
            this.Controls.Add(this._close_);
            this.Controls.Add(this.sent);
            this.Name = "SentDataOnBonus";
            this.Text = "Отправка данных по бонусной программе";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sent;
        private System.Windows.Forms.Button _close_;
        private System.Windows.Forms.TextBox txtB_jason;
    }
}