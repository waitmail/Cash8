using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Cash8
{
    public partial class Action : Form
    {
        public Action()
        {
            InitializeComponent();
            this.Load += new EventHandler(Action_Load);
        }

        private void Action_Load(object sender, EventArgs e)
        {
            //Главный список
            listView_tovar.View = View.Details;

            // Allow the user to rearrange columns.
            listView_tovar.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            listView_tovar.FullRowSelect = true;

            // Display grid lines.
            listView_tovar.GridLines = true;
            listView_tovar.Columns.Add("Код", 50, HorizontalAlignment.Left);
            listView_tovar.Columns.Add("Наименование", 700, HorizontalAlignment.Left);

        }
    }
}
