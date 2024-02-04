using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cash8
{
    public partial class ParametersReceiptCorrection : Form
    {
        public ParametersReceiptCorrection()
        {
            InitializeComponent();
            this.Load += ParametersReceiptCorrection_Load;
        }

        private void ParametersReceiptCorrection_Load(object sender, EventArgs e)
        {
            //if (MainStaticClass.Nick_Shop == "A90")
            //{
            //    txtB_tax_order.Text = "2.15-15/00373 от 12.01.2024";
            //}
            if (MainStaticClass.Nick_Shop == "A23")
            {
                txtB_tax_order.Text = "№14 / 001 от 24.01.2024";
            }
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btn_enable_Click(object sender, EventArgs e)
        {
            if (txtB_password.Text.Trim() == "123698745")
            {
                btn_Ok.Enabled = true;
                txtB_tax_order.Enabled = true;
            }
            else
            {
                MessageBox.Show("Вы ввели неправильный пароль ","Ошибка при вводе пароля");
                txtB_password.Text = "";
            }
        }
    }
}
