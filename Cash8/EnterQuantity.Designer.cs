namespace Cash8
{
    partial class EnterQuantity
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
            this.panelenter_quantity = new System.Windows.Forms.Panel();
            this.numericUpDown_enter_quantity = new System.Windows.Forms.NumericUpDown();
            this.panelenter_quantity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_enter_quantity)).BeginInit();
            this.SuspendLayout();
            // 
            // panelenter_quantity
            // 
            this.panelenter_quantity.BackColor = System.Drawing.Color.LightSalmon;
            this.panelenter_quantity.Controls.Add(this.numericUpDown_enter_quantity);
            this.panelenter_quantity.Location = new System.Drawing.Point(12, 12);
            this.panelenter_quantity.Name = "panelenter_quantity";
            this.panelenter_quantity.Size = new System.Drawing.Size(333, 154);
            this.panelenter_quantity.TabIndex = 22;
            // 
            // numericUpDown_enter_quantity
            // 
            this.numericUpDown_enter_quantity.DecimalPlaces = 3;
            this.numericUpDown_enter_quantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.numericUpDown_enter_quantity.Location = new System.Drawing.Point(19, 48);
            this.numericUpDown_enter_quantity.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDown_enter_quantity.Name = "numericUpDown_enter_quantity";
            this.numericUpDown_enter_quantity.Size = new System.Drawing.Size(296, 62);
            this.numericUpDown_enter_quantity.TabIndex = 0;
            this.numericUpDown_enter_quantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // EnterQuantity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 181);
            this.ControlBox = false;
            this.Controls.Add(this.panelenter_quantity);
            this.KeyPreview = true;
            this.Name = "EnterQuantity";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Введите количество";
            this.panelenter_quantity.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_enter_quantity)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelenter_quantity;
        public System.Windows.Forms.NumericUpDown numericUpDown_enter_quantity;
    }
}