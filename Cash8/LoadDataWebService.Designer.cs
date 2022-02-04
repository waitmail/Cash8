namespace Cash8
{
    partial class LoadDataWebService
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
            this.download_bonus_clients = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_new_load = new System.Windows.Forms.Button();
            this.btn_update_only = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // download_bonus_clients
            // 
            this.download_bonus_clients.Enabled = false;
            this.download_bonus_clients.Location = new System.Drawing.Point(6, 58);
            this.download_bonus_clients.Name = "download_bonus_clients";
            this.download_bonus_clients.Size = new System.Drawing.Size(248, 23);
            this.download_bonus_clients.TabIndex = 1;
            this.download_bonus_clients.Text = "Обновить клиентов";
            this.download_bonus_clients.UseVisualStyleBackColor = true;
            this.download_bonus_clients.Click += new System.EventHandler(this.download_bonus_clients_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(18, 160);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(248, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.download_bonus_clients);
            this.groupBox1.Location = new System.Drawing.Point(12, 134);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 90);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Клиенты";
            // 
            // btn_new_load
            // 
            this.btn_new_load.Location = new System.Drawing.Point(18, 76);
            this.btn_new_load.Name = "btn_new_load";
            this.btn_new_load.Size = new System.Drawing.Size(248, 52);
            this.btn_new_load.TabIndex = 7;
            this.btn_new_load.Text = "Загрузить данные (ПОЛНАЯ ЗАГРУЗКА)";
            this.btn_new_load.UseVisualStyleBackColor = true;
            this.btn_new_load.Click += new System.EventHandler(this.btn_new_load_Click);
            // 
            // btn_update_only
            // 
            this.btn_update_only.Enabled = false;
            this.btn_update_only.Location = new System.Drawing.Point(18, 12);
            this.btn_update_only.Name = "btn_update_only";
            this.btn_update_only.Size = new System.Drawing.Size(248, 58);
            this.btn_update_only.TabIndex = 8;
            this.btn_update_only.Text = "Загрузить данные (ИЗМЕНЕНИЯ)  ";
            this.btn_update_only.UseVisualStyleBackColor = true;
            this.btn_update_only.Click += new System.EventHandler(this.btn_update_only_Click);
            // 
            // LoadDataWebService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 229);
            this.Controls.Add(this.btn_update_only);
            this.Controls.Add(this.btn_new_load);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Name = "LoadDataWebService";
            this.Text = "Загрузить данные из интернета";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button download_bonus_clients;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_new_load;
        private System.Windows.Forms.Button btn_update_only;
    }
}