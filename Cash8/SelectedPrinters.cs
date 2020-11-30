using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Cash8
{
    public partial class SelectedPrinters : Form
    {
        public int num_printer = 0;

        public SelectedPrinters()
        {
            InitializeComponent();
            this.Load += new EventHandler(SelectedPrinters_Load);
            this.listView1.KeyDown += new KeyEventHandler(listView1_KeyDown);
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    num_printer = listView1.SelectedItems[0].Index;
                    this.DialogResult = DialogResult.Yes;
                }
            }
        }
        
        private void SelectedPrinters_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;

            // Allow the user to rearrange columns.
            listView1.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            listView1.FullRowSelect = true;

            // Display grid lines.
            listView1.GridLines = true;
            listView1.Columns.Add("Наименование", 500, HorizontalAlignment.Left);

        }
    }
}
