﻿namespace Cash8
{
    partial class Actions
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
            this.listView_doc = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // listView_doc
            // 
            this.listView_doc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView_doc.Location = new System.Drawing.Point(12, 98);
            this.listView_doc.Name = "listView_doc";
            this.listView_doc.Size = new System.Drawing.Size(768, 463);
            this.listView_doc.TabIndex = 0;
            this.listView_doc.UseCompatibleStateImageBehavior = false;
            // 
            // Actions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.listView_doc);
            this.Name = "Actions";
            this.Text = "Actions";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView_doc;
    }
}