namespace Cash8
{
    partial class ReasonsDeletionCheck
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
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.comboBox_reasons = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btn_ok
            // 
            this.btn_ok.Location = new System.Drawing.Point(24, 262);
            this.btn_ok.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(258, 94);
            this.btn_ok.TabIndex = 0;
            this.btn_ok.Text = "Выбрать";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(656, 262);
            this.btn_cancel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(264, 94);
            this.btn_cancel.TabIndex = 1;
            this.btn_cancel.Text = "Отмена";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // comboBox_reasons
            // 
            this.comboBox_reasons.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_reasons.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox_reasons.FormattingEnabled = true;
            this.comboBox_reasons.Items.AddRange(new object[] {
            "Проверка акции.",
            "Несрабатывание акции, переоценки, уценки.",
            "Ошибка магазина.",
            "Техническая ошибка.",
            "Клиента не устроила цена.",
            "Клиенту не хватило денег.",
            "IT-отдел",
            "Иные причины."});
            this.comboBox_reasons.Location = new System.Drawing.Point(24, 54);
            this.comboBox_reasons.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.comboBox_reasons.Name = "comboBox_reasons";
            this.comboBox_reasons.Size = new System.Drawing.Size(892, 52);
            this.comboBox_reasons.TabIndex = 2;
            this.comboBox_reasons.SelectedIndexChanged += new System.EventHandler(this.comboBox_reasons_SelectedIndexChanged);
            // 
            // ReasonsDeletionCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 421);
            this.ControlBox = false;
            this.Controls.Add(this.comboBox_reasons);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "ReasonsDeletionCheck";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Причины удаления чека";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.ComboBox comboBox_reasons;
    }
}