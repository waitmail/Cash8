namespace Cash8
{
    partial class Interface_switching
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
            this.input_barcode = new System.Windows.Forms.TextBox();
            this.authorization = new System.Windows.Forms.Label();
            this.fail_autorize = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // input_barcode
            // 
            this.input_barcode.Location = new System.Drawing.Point(136, 43);
            this.input_barcode.MaxLength = 10;
            this.input_barcode.Name = "input_barcode";
            this.input_barcode.PasswordChar = '$';
            this.input_barcode.Size = new System.Drawing.Size(229, 20);
            this.input_barcode.TabIndex = 0;            
            this.input_barcode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.input_barcode_KeyPress);
            // 
            // authorization
            // 
            this.authorization.AutoSize = true;
            this.authorization.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.authorization.ForeColor = System.Drawing.Color.DarkGreen;
            this.authorization.Location = new System.Drawing.Point(97, 9);
            this.authorization.Name = "authorization";
            this.authorization.Size = new System.Drawing.Size(291, 20);
            this.authorization.TabIndex = 1;
            this.authorization.Text = "АВТОРИЗАЦИЯ(введите пароль)";
            // 
            // fail_autorize
            // 
            this.fail_autorize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fail_autorize.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.fail_autorize.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fail_autorize.Enabled = false;
            this.fail_autorize.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fail_autorize.Location = new System.Drawing.Point(12, 69);
            this.fail_autorize.Name = "fail_autorize";
            this.fail_autorize.Size = new System.Drawing.Size(476, 19);
            this.fail_autorize.TabIndex = 2;
            this.fail_autorize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Interface_switching
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 99);
            this.Controls.Add(this.fail_autorize);
            this.Controls.Add(this.authorization);
            this.Controls.Add(this.input_barcode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(332, 99);
            this.MaximumSize = new System.Drawing.Size(500, 99);
            this.MinimumSize = new System.Drawing.Size(500, 60);
            this.Name = "Interface_switching";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Interface_switching";
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.TextBox input_barcode;
        private System.Windows.Forms.Label authorization;
        private System.Windows.Forms.TextBox fail_autorize;
    }
}