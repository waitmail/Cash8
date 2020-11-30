using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Cash8
{
    public partial class MyMessageBox : Form
    {
        public MyMessageBox()
        {
            InitializeComponent();
            this.Load += new EventHandler(MyMessageBox_Load);
        }


        public MyMessageBox(string text_message, string text_header_form)
        {
            InitializeComponent();
            this.text_message.Text = text_message;
            this.Text = text_header_form;
            this.Load += new EventHandler(MyMessageBox_Load);
        }


        private void MyMessageBox_Load(object sender, EventArgs e)
        {
            this._close_.Focus();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void _close__Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
