namespace Cash8
{
    partial class InputSertificates
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
            this.input_sertificate = new System.Windows.Forms.TextBox();
            this.listView_sertificates = new System.Windows.Forms.ListView();
            this.button_commit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // input_sertificate
            // 
            this.input_sertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.input_sertificate.Location = new System.Drawing.Point(524, 23);
            this.input_sertificate.MaxLength = 13;
            this.input_sertificate.Name = "input_sertificate";
            this.input_sertificate.Size = new System.Drawing.Size(228, 20);
            this.input_sertificate.TabIndex = 0;
            // 
            // listView_sertificates
            // 
            this.listView_sertificates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView_sertificates.Location = new System.Drawing.Point(13, 71);
            this.listView_sertificates.Name = "listView_sertificates";
            this.listView_sertificates.Size = new System.Drawing.Size(739, 305);
            this.listView_sertificates.TabIndex = 1;
            this.listView_sertificates.UseCompatibleStateImageBehavior = false;
            // 
            // button_commit
            // 
            this.button_commit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button_commit.AutoSize = true;
            this.button_commit.BackColor = System.Drawing.SystemColors.Control;
            this.button_commit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_commit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_commit.ForeColor = System.Drawing.Color.Red;
            this.button_commit.Location = new System.Drawing.Point(13, 439);
            this.button_commit.Name = "button_commit";
            this.button_commit.Size = new System.Drawing.Size(739, 102);
            this.button_commit.TabIndex = 10;
            this.button_commit.Text = "Подтвердить ввод (F12)";
            this.button_commit.UseVisualStyleBackColor = false;
            this.button_commit.Click += new System.EventHandler(this.button_commit_Click);
            // 
            // InputSertificates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 562);
            this.ControlBox = false;
            this.Controls.Add(this.button_commit);
            this.Controls.Add(this.listView_sertificates);
            this.Controls.Add(this.input_sertificate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputSertificates";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InputSertificates";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox input_sertificate;
        private System.Windows.Forms.Button button_commit;
        public System.Windows.Forms.ListView listView_sertificates;
    }
}