namespace Cash8
{
    partial class CorrectionChecks
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
            this.txtB_tax_order = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.btn_fill_checks = new System.Windows.Forms.Button();
            this.btn_print = new System.Windows.Forms.Button();
            this.checkBox_print_on_paper = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_enable = new System.Windows.Forms.Button();
            this.txtB_password = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtB_tax_order
            // 
            this.txtB_tax_order.Enabled = false;
            this.txtB_tax_order.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_tax_order.Location = new System.Drawing.Point(313, 74);
            this.txtB_tax_order.MaxLength = 200;
            this.txtB_tax_order.Name = "txtB_tax_order";
            this.txtB_tax_order.Size = new System.Drawing.Size(457, 29);
            this.txtB_tax_order.TabIndex = 0;
            this.txtB_tax_order.Text = "№14 / 001 от 24.01.2024";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(295, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Текст налогового предписания";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Enabled = false;
            this.dataGridView1.Location = new System.Drawing.Point(16, 156);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(754, 293);
            this.dataGridView1.TabIndex = 2;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Enabled = false;
            this.dateTimePicker1.Location = new System.Drawing.Point(16, 113);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 3;
            // 
            // btn_fill_checks
            // 
            this.btn_fill_checks.Enabled = false;
            this.btn_fill_checks.Location = new System.Drawing.Point(266, 114);
            this.btn_fill_checks.Name = "btn_fill_checks";
            this.btn_fill_checks.Size = new System.Drawing.Size(158, 23);
            this.btn_fill_checks.TabIndex = 4;
            this.btn_fill_checks.Text = "Заполнить по дате";
            this.btn_fill_checks.UseVisualStyleBackColor = true;
            this.btn_fill_checks.Click += new System.EventHandler(this.btn_fill_checks_Click);
            // 
            // btn_print
            // 
            this.btn_print.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_print.Enabled = false;
            this.btn_print.Location = new System.Drawing.Point(16, 466);
            this.btn_print.Name = "btn_print";
            this.btn_print.Size = new System.Drawing.Size(122, 23);
            this.btn_print.TabIndex = 5;
            this.btn_print.Text = "Напечатать";
            this.btn_print.UseVisualStyleBackColor = true;
            this.btn_print.Click += new System.EventHandler(this.btn_print_Click);
            // 
            // checkBox_print_on_paper
            // 
            this.checkBox_print_on_paper.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.checkBox_print_on_paper.AutoSize = true;
            this.checkBox_print_on_paper.Checked = true;
            this.checkBox_print_on_paper.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_print_on_paper.Enabled = false;
            this.checkBox_print_on_paper.Location = new System.Drawing.Point(162, 471);
            this.checkBox_print_on_paper.Name = "checkBox_print_on_paper";
            this.checkBox_print_on_paper.Size = new System.Drawing.Size(127, 17);
            this.checkBox_print_on_paper.TabIndex = 6;
            this.checkBox_print_on_paper.Text = "Печатать на бумаге";
            this.checkBox_print_on_paper.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(17, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 24);
            this.label3.TabIndex = 10;
            this.label3.Text = "Введите пароль";
            // 
            // btn_enable
            // 
            this.btn_enable.Location = new System.Drawing.Point(620, 22);
            this.btn_enable.Name = "btn_enable";
            this.btn_enable.Size = new System.Drawing.Size(150, 29);
            this.btn_enable.TabIndex = 9;
            this.btn_enable.Text = "Получить доступ";
            this.btn_enable.UseVisualStyleBackColor = true;
            this.btn_enable.Click += new System.EventHandler(this.btn_enable_Click);
            // 
            // txtB_password
            // 
            this.txtB_password.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_password.Location = new System.Drawing.Point(180, 20);
            this.txtB_password.MaxLength = 10;
            this.txtB_password.Name = "txtB_password";
            this.txtB_password.PasswordChar = '*';
            this.txtB_password.Size = new System.Drawing.Size(434, 29);
            this.txtB_password.TabIndex = 8;
            // 
            // CorrectionChecks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 501);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_enable);
            this.Controls.Add(this.txtB_password);
            this.Controls.Add(this.checkBox_print_on_paper);
            this.Controls.Add(this.btn_print);
            this.Controls.Add(this.btn_fill_checks);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtB_tax_order);
            this.Name = "CorrectionChecks";
            this.Text = "Чеки которые необходимо скорректировать";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtB_tax_order;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button btn_fill_checks;
        private System.Windows.Forms.Button btn_print;
        private System.Windows.Forms.CheckBox checkBox_print_on_paper;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_enable;
        private System.Windows.Forms.TextBox txtB_password;
    }
}