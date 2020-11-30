using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace Cash8
{
    public partial class InputePhoneClient : Form
    {
        public string barcode = "";
        public Cash_check cash_Check = null;

        public InputePhoneClient()
        {
            InitializeComponent();
            if (MainStaticClass.get_currency() == "руб.")
            {
                this.txtB_phone_number.MaxLength = 12;
            }
            this.txtB_phone_number.KeyPress += new KeyPressEventHandler(txtB_phone_number_KeyPress);
            this.Load += new EventHandler(InputePhoneClient_Load);
            this.txtB_phone_number.SelectionStart = 1;
            this.KeyPreview = true;
        }

        void InputePhoneClient_Load(object sender, EventArgs e)
        {
            //this.Close();
            if (barcode != "")
            {
                this.label_zagolovok.Text = " За картой № " + barcode + " не закреплен номер телефона." +
                    " Необходимо указать номер телефона.";
            }
            else
            {
                this.label_zagolovok.Text = " Для создания вимртуальной карты "+
                    " Необходимо указать номер телефона.";
            }
        }


        void txtB_phone_number_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar != 43)
            //{
                if (!(Char.IsDigit(e.KeyChar)))
                {
                    if (e.KeyChar != (char)Keys.Back)
                    {
                        e.Handled = true;
                    }
                }
            //}

            if (e.KeyChar == (char)Keys.Enter)
            {
                //int lenght = 0;
                //if (MainStaticClass.get_currency() == "руб.")
                //{
                //    lenght = 12;
                //}
                //else
                //{
                //    lenght = 13;
                //}

                if (this.txtB_phone_number.Text.Trim().Length != 10)
                {
                    MessageBox.Show(" Телефонный номер должен состоять из 10 цифр ");
                    return;
                }
                //{
                //    MessageBox.Show(" Телефонный номер введен некорректно ");
                //    return;
                //}
                //if (MainStaticClass.get_currency() == "руб.")
                //{
                    //if (this.txtB_phone_number.Text.Trim().Substring(0, 1) != "7")
                    //{
                    //    MessageBox.Show(" Телефонный номер введен некорректно, номер должен начинаться с +7 ");
                    //    return;
                    //}
                //}
                //else
                //{
                //    if (this.txtB_phone_number.Text.Trim().Substring(0, 3) != "+38")
                //    {
                //        MessageBox.Show(" Телефонный номер введен некорректно, номер должен начинаться с +38 ");
                //        return;
                //    }
                //}
            }
            else
            {
                return;
            }

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                //string query = "SELECT COUNT(*) FROM clients WHERE phone LIKE'%"+ txtB_phone_number.Text.Trim() + "%'";
                string query = "SELECT COUNT(*) FROM clients where right(phone,10)='" + txtB_phone_number.Text.Trim() + "'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);                
                if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                {
                    query = "DELETE FROM temp_phone_clients WHERE phone='" + this.txtB_phone_number.Text.Trim() + "'";
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                    if (barcode != "")
                    {
                        query = "INSERT INTO temp_phone_clients(barcode, phone)VALUES ('" + barcode + "','" + this.txtB_phone_number.Text.Trim() + "')";
                    }
                    else
                    {
                        query = "INSERT INTO temp_phone_clients(barcode, phone)VALUES ('" + this.txtB_phone_number.Text.Trim() + "','" + this.txtB_phone_number.Text.Trim() + "')";
                    }
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                    conn.Close();
                    if (barcode == "")
                    {
                        cash_Check.client.Tag = this.txtB_phone_number.Text.Trim();
                        cash_Check.client.Text = this.txtB_phone_number.Text.Trim();
                        cash_Check.Discount = Convert.ToDecimal(0.05);
                    }
                }
                else
                {
                    MessageBox.Show("У этого клиента уже есть дисконтная карта с привязанным номером телефона !!!");
                    MessageBox.Show("Введите номер телефона в соответсвующее поле !!!");

                    conn.Close();
                    this.Close();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" Ошибки при записи номера телефона " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Ошибки при записи номера телефона " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            this.Close();
        }


        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar  == (char)Keys.Escape)
            {
                this.Close();
            }
        }        
    }
}
