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
    public partial class ReasonsDeletionCheck : Form
    {
        public string reason = "";
        public ReasonsDeletionCheck()
        {
            InitializeComponent();
            this.Load += ReasonsDeletionCheck_Load;
        }

        private void ReasonsDeletionCheck_Load(object sender, EventArgs e)
        {
            this.comboBox_reasons.SelectedIndex = 0;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void comboBox_reasons_SelectedIndexChanged(object sender, EventArgs e)
        {
            reason = this.comboBox_reasons.SelectedItem.ToString();
        }
    }
}
