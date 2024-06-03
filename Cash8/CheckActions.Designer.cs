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
            this.dataGridView_tovar_execute = new System.Windows.Forms.DataGridView();
            this.txtB_client = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.txtB_client_code = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_tovar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_tovar_execute)).BeginInit();
            this.SuspendLayout();
            // 
            // txtB_input_code_or_barcode
            // 
            this.txtB_input_code_or_barcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_input_code_or_barcode.Location = new System.Drawing.Point(50, 23);
            this.txtB_input_code_or_barcode.MaxLength = 13;
            this.txtB_input_code_or_barcode.Name = "txtB_input_code_or_barcode";
            this.txtB_input_code_or_barcode.Size = new System.Drawing.Size(165, 29);
            this.txtB_input_code_or_barcode.TabIndex = 0;
            // 
            // dataGridView_tovar
            // 
            this.dataGridView_tovar.AllowUserToAddRows = false;
            this.dataGridView_tovar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_tovar.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_tovar.Location = new System.Drawing.Point(9, 65);
            this.dataGridView_tovar.Name = "dataGridView_tovar";
            this.dataGridView_tovar.ReadOnly = true;
            this.dataGridView_tovar.Size = new System.Drawing.Size(763, 222);
            this.dataGridView_tovar.TabIndex = 1;
            // 
            // dataGridView_tovar_execute
            // 
            this.dataGridView_tovar_execute.AllowUserToAddRows = false;
            this.dataGridView_tovar_execute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_tovar_execute.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_tovar_execute.Location = new System.Drawing.Point(9, 305);
            this.dataGridView_tovar_execute.Name = "dataGridView_tovar_execute";
            this.dataGridView_tovar_execute.ReadOnly = true;
            this.dataGridView_tovar_execute.Size = new System.Drawing.Size(763, 241);
            this.dataGridView_tovar_execute.TabIndex = 2;
            // 
            // txtB_client
            // 
            this.txtB_client.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_client.Location = new System.Drawing.Point(343, 23);
            this.txtB_client.Name = "txtB_client";
            this.txtB_client.ReadOnly = true;
            this.txtB_client.Size = new System.Drawing.Size(190, 29);
            this.txtB_client.TabIndex = 5;
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox2.Location = new System.Drawing.Point(13, 23);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(31, 29);
            this.textBox2.TabIndex = 6;
            this.textBox2.Text = "ШК";
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox3.Location = new System.Drawing.Point(222, 23);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(115, 29);
            this.textBox3.TabIndex = 7;
            this.textBox3.Text = "Покупатель";
            // 
            // txtB_client_code
            // 
            this.txtB_client_code.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_client_code.Location = new System.Drawing.Point(540, 23);
            this.txtB_client_code.MaxLength = 13;
            this.txtB_client_code.Name = "txtB_client_code";
            this.txtB_client_code.Size = new System.Drawing.Size(232, 29);
            this.txtB_client_code.TabIndex = 8;
            // 
            // CheckActions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.txtB_client_code);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.txtB_client);
            this.Controls.Add(this.dataGridView_tovar_execute);
            this.Controls.Add(this.dataGridView_tovar);
            this.Controls.Add(this.txtB_input_code_or_barcode);
            this.Name = "CheckActions";
            this.Text = "Проверка акций";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_tovar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_tovar_execute)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtB_input_code_or_barcode;
        private System.Windows.Forms.DataGridView dataGridView_tovar;
        private System.Windows.Forms.DataGridView dataGridView_tovar_execute;
        private System.Windows.Forms.TextBox txtB_client;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox txtB_client_code;
    }
}