namespace Cash8
{
    partial class CheckActions
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
            this.txtB_input_code_or_barcode = new System.Windows.Forms.TextBox();
            this.dataGridView_tovar = new System.Windows.Forms.DataGridView();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btn_check_actions = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_tovar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtB_input_code_or_barcode
            // 
            this.txtB_input_code_or_barcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_input_code_or_barcode.Location = new System.Drawing.Point(9, 22);
            this.txtB_input_code_or_barcode.MaxLength = 13;
            this.txtB_input_code_or_barcode.Name = "txtB_input_code_or_barcode";
            this.txtB_input_code_or_barcode.Size = new System.Drawing.Size(165, 29);
            this.txtB_input_code_or_barcode.TabIndex = 0;
            // 
            // dataGridView_tovar
            // 
            this.dataGridView_tovar.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_tovar.Location = new System.Drawing.Point(9, 65);
            this.dataGridView_tovar.Name = "dataGridView_tovar";
            this.dataGridView_tovar.Size = new System.Drawing.Size(763, 222);
            this.dataGridView_tovar.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(9, 324);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(763, 222);
            this.dataGridView1.TabIndex = 2;
            // 
            // btn_check_actions
            // 
            this.btn_check_actions.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_check_actions.Location = new System.Drawing.Point(494, 5);
            this.btn_check_actions.Name = "btn_check_actions";
            this.btn_check_actions.Size = new System.Drawing.Size(278, 46);
            this.btn_check_actions.TabIndex = 4;
            this.btn_check_actions.Text = "Выполнить проверку акций";
            this.btn_check_actions.UseVisualStyleBackColor = true;
            // 
            // CheckActions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.btn_check_actions);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.dataGridView_tovar);
            this.Controls.Add(this.txtB_input_code_or_barcode);
            this.Name = "CheckActions";
            this.Text = "Проверка цен и акций";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_tovar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtB_input_code_or_barcode;
        private System.Windows.Forms.DataGridView dataGridView_tovar;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btn_check_actions;
    }
}