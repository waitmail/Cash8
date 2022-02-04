namespace Cash8
{
    partial class Input_action_barcode
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
            this.SuspendLayout();
            // 
            // input_barcode
            // 
            this.input_barcode.Location = new System.Drawing.Point(16, 43);
            this.input_barcode.MaxLength = 200;
            this.input_barcode.Name = "input_barcode";
            this.input_barcode.Size = new System.Drawing.Size(472, 20);
            this.input_barcode.TabIndex = 0;
            this.input_barcode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.input_barcode_KeyPress);
            // 
            // authorization
            // 
            this.authorization.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.authorization.AutoSize = true;
            this.authorization.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.authorization.ForeColor = System.Drawing.Color.DarkGreen;
            this.authorization.Location = new System.Drawing.Point(70, 9);
            this.authorization.Name = "authorization";
            this.authorization.Size = new System.Drawing.Size(348, 20);
            this.authorization.TabIndex = 1;
            this.authorization.Text = "Введите штрихкод, включающий акцию";
            // 
            // Input_action_barcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 75);
            this.Controls.Add(this.authorization);
            this.Controls.Add(this.input_barcode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(332, 99);
            this.MaximumSize = new System.Drawing.Size(500, 99);
            this.MinimumSize = new System.Drawing.Size(500, 60);
            this.Name = "Input_action_barcode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Interface_switching";
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.TextBox input_barcode;
        private System.Windows.Forms.Label authorization;
    }
}