namespace Cash8
{
    partial class SendDataOnSalesPortions
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
            this.send_sales_data = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _close_
            // 
            this._close_.Location = new System.Drawing.Point(179, 25);
            this._close_.Name = "_close_";
            this._close_.Size = new System.Drawing.Size(139, 23);
            this._close_.TabIndex = 4;
            this._close_.Text = "Закрыть";
            this._close_.UseVisualStyleBackColor = true;
            this._close_.Click += new System.EventHandler(this._close__Click);
            // 
            // send_sales_data
            // 
            this.send_sales_data.Location = new System.Drawing.Point(41, 26);
            this.send_sales_data.Name = "send_sales_data";
            this.send_sales_data.Size = new System.Drawing.Size(122, 23);
            this.send_sales_data.TabIndex = 3;
            this.send_sales_data.Text = "Отправить данные ";
            this.send_sales_data.UseVisualStyleBackColor = true;
            this.send_sales_data.Click += new System.EventHandler(this.send_sales_data_Click);
            // 
            // SendDataOnSalesPortions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 67);
            this.Controls.Add(this._close_);
            this.Controls.Add(this.send_sales_data);
            this.Name = "SendDataOnSalesPortions";
            this.Text = "Отправить данные порционно";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _close_;
        private System.Windows.Forms.Button send_sales_data;
    }
}