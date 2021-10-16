namespace Cash8
{
    partial class CreateVirtualCard
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
            this.txtBox_phone = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_create_virtual_card = new System.Windows.Forms.Button();
            this.btn_check_number_phone = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtB_check_code = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtBox_phone
            // 
            this.txtBox_phone.Enabled = false;
            this.txtBox_phone.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtBox_phone.Location = new System.Drawing.Point(184, 12);
            this.txtBox_phone.MaxLength = 10;
            this.txtBox_phone.Name = "txtBox_phone";
            this.txtBox_phone.Size = new System.Drawing.Size(197, 40);
            this.txtBox_phone.TabIndex = 0;
            this.txtBox_phone.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(6, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 40);
            this.label1.TabIndex = 1;
            this.label1.Text = "Номер телефона";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_create_virtual_card
            // 
            this.btn_create_virtual_card.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_create_virtual_card.Location = new System.Drawing.Point(6, 56);
            this.btn_create_virtual_card.Name = "btn_create_virtual_card";
            this.btn_create_virtual_card.Size = new System.Drawing.Size(375, 53);
            this.btn_create_virtual_card.TabIndex = 2;
            this.btn_create_virtual_card.Text = "Создать виртуальную карту и прислать код подтверждения";
            this.btn_create_virtual_card.UseVisualStyleBackColor = true;
            this.btn_create_virtual_card.Click += new System.EventHandler(this.btn_create_virtual_card_Click);
            // 
            // btn_check_number_phone
            // 
            this.btn_check_number_phone.Enabled = false;
            this.btn_check_number_phone.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_check_number_phone.Location = new System.Drawing.Point(6, 205);
            this.btn_check_number_phone.Name = "btn_check_number_phone";
            this.btn_check_number_phone.Size = new System.Drawing.Size(375, 53);
            this.btn_check_number_phone.TabIndex = 5;
            this.btn_check_number_phone.Text = "Проверить введенный код";
            this.btn_check_number_phone.UseVisualStyleBackColor = true;
            this.btn_check_number_phone.Click += new System.EventHandler(this.btn_check_number_phone_Click);
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(6, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(236, 40);
            this.label2.TabIndex = 4;
            this.label2.Text = "Код подтверждения";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtB_check_code
            // 
            this.txtB_check_code.Enabled = false;
            this.txtB_check_code.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_check_code.Location = new System.Drawing.Point(248, 161);
            this.txtB_check_code.MaxLength = 10;
            this.txtB_check_code.Name = "txtB_check_code";
            this.txtB_check_code.Size = new System.Drawing.Size(133, 40);
            this.txtB_check_code.TabIndex = 3;
            this.txtB_check_code.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(11, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(368, 39);
            this.label3.TabIndex = 6;
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CreateVirtualCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 266);
            this.ControlBox = false;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_check_number_phone);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtB_check_code);
            this.Controls.Add(this.btn_create_virtual_card);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBox_phone);
            this.KeyPreview = true;
            this.Name = "CreateVirtualCard";
            this.Text = "Создание винртуальной карты";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtBox_phone;
        private System.Windows.Forms.Button btn_create_virtual_card;
        private System.Windows.Forms.Button btn_check_number_phone;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtB_check_code;
        private System.Windows.Forms.Label label3;
    }
}