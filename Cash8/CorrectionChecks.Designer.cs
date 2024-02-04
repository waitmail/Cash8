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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtB_tax_order
            // 
            this.txtB_tax_order.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_tax_order.Location = new System.Drawing.Point(313, 39);
            this.txtB_tax_order.MaxLength = 200;
            this.txtB_tax_order.Name = "txtB_tax_order";
            this.txtB_tax_order.Size = new System.Drawing.Size(457, 29);
            this.txtB_tax_order.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(295, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Текст налогового предписания";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(16, 156);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(754, 293);
            this.dataGridView1.TabIndex = 2;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(16, 113);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 3;
            // 
            // btn_fill_checks
            // 
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
            this.checkBox_print_on_paper.AutoSize = true;
            this.checkBox_print_on_paper.Location = new System.Drawing.Point(162, 471);
            this.checkBox_print_on_paper.Name = "checkBox_print_on_paper";
            this.checkBox_print_on_paper.Size = new System.Drawing.Size(127, 17);
            this.checkBox_print_on_paper.TabIndex = 6;
            this.checkBox_print_on_paper.Text = "Печатать на бумаге";
            this.checkBox_print_on_paper.UseVisualStyleBackColor = true;
            // 
            // CorrectionChecks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 501);
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
    }
}