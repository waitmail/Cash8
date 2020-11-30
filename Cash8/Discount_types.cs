using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Npgsql;

namespace Cash8
{
    public partial class Discount_types : Form
    {
        public Discount_types()
        {
            InitializeComponent();
        }

        void Discount_types_Load(object sender, System.EventArgs e)
        {
            // Set the view to show details.
            listView1.View = View.Details;

            // Allow the user to edit item text.
            listView1.LabelEdit = true;

            // Allow the user to rearrange columns.
            listView1.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            listView1.FullRowSelect = true;

            // Display grid lines.
            listView1.GridLines = true;

            // Sort the items in the list in ascending order.
            //listView1.Sorting = SortOrder.Ascending;

            listView1.Columns.Add("Код", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Процент скидки", 200, HorizontalAlignment.Left);
            listView1.Columns.Add("Сумма перехода", 200, HorizontalAlignment.Right);
            //listView1.Columns.Add("Последняя зак. цена", 100, HorizontalAlignment.Right);

            //ListViewItemSorter _lvwItemComparer = new ListViewItemComparer();
            //this.listView1.ListViewItemSorter = _lvwItemComparer;


            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            conn.Open();
            string myQuery = "SELECT * FROM discount_types order by code";
            NpgsqlCommand command = new NpgsqlCommand(myQuery, conn);
            NpgsqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            conn.Close();
            // Get the table from the data set
            //DataTable dtable = _DataSet.Tables["barcode"];

            // Clear the ListView control
            listView1.Items.Clear();

            // Display items in the ListView control
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataRow drow = dataTable.Rows[i];

                // Only row that have not been deleted
                if (drow.RowState != DataRowState.Deleted)
                {
                    // Define the list items
                    ListViewItem lvi = new ListViewItem(drow["code"].ToString());
                    //lvi.SubItems.Add(drow["name"].ToString());
                    lvi.SubItems.Add(drow["discount_percent"].ToString());
                    lvi.SubItems.Add(drow["transition_sum"].ToString());

                    // Add the list items to the ListView
                    listView1.Items.Add(lvi);
                }
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
