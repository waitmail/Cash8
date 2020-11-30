namespace Cash8
{
    partial class InputeCodeClient
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
            this.txtB_inpute_code_client = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtB_inpute_code_client
            // 
            this.txtB_inpute_code_client.Location = new System.Drawing.Point(46, 44);
            this.txtB_inpute_code_client.MaxLength = 10;
            this.txtB_inpute_code_client.Name = "txtB_inpute_code_client";
            this.txtB_inpute_code_client.Size = new System.Drawing.Size(251, 20);
            this.txtB_inpute_code_client.TabIndex = 0;
            // 
            // InputeCodeClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 110);
            this.ControlBox = false;
            this.Controls.Add(this.txtB_inpute_code_client);
            this.KeyPreview = true;
            this.Name = "InputeCodeClient";
            this.Text = "Введите новый код карточки клиента сканером штрихкода";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtB_inpute_code_client;
    }
}