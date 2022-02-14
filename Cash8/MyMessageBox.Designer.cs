namespace Cash8
{
    partial class MyMessageBox
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
            this._close_ = new System.Windows.Forms.Button();
            this.text_message = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _close_
            // 
            this._close_.Location = new System.Drawing.Point(331, 207);
            this._close_.Name = "_close_";
            this._close_.Size = new System.Drawing.Size(128, 23);
            this._close_.TabIndex = 3;
            this._close_.Text = "Закрыть";
            this._close_.UseVisualStyleBackColor = true;
            this._close_.Click += new System.EventHandler(this._close__Click);
            // 
            // text_message
            // 
            this.text_message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.text_message.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.text_message.Enabled = false;
            this.text_message.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.text_message.Location = new System.Drawing.Point(12, 12);
            this.text_message.Multiline = true;
            this.text_message.Name = "text_message";
            this.text_message.Size = new System.Drawing.Size(708, 189);
            this.text_message.TabIndex = 2;
            this.text_message.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // MyMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 236);
            this.Controls.Add(this._close_);
            this.Controls.Add(this.text_message);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MyMessageBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MyMessageBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _close_;
        public System.Windows.Forms.TextBox text_message;
    }
}