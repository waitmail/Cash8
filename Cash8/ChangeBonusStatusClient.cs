using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace Cash8
{
    public partial class ChangeBonusStatusClient : Form
    {
        public string client_code="";

        public ChangeBonusStatusClient()
        {
            InitializeComponent();
        }

        private void get_phone_client()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT clients.phone AS phone,temp_phone_clients.phone AS phone1 FROM clients " +
                    " left join temp_phone_clients ON clients.code = temp_phone_clients.barcode " +
                    " WHERE clients.code" + client_code;
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if ((reader["pnone"].ToString().Trim().Length >= 10))
                    {
                        txtB_phone.Text = reader["pnone"].ToString().Trim();
                    }
                    else if ((reader["pnone1"].ToString().Trim().Length >= 10))
                    {
                        txtB_phone.Text = reader["pnone1"].ToString().Trim();
                    }
                }
                reader.Close();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при проверка наличия телефона " + ex.Message);                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверка наличия телефона " + ex.Message);                
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        private void btn_execute_Click(object sender, EventArgs e)
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "INSERT INTO public.client_with_changed_status_to_send(client,date_change)VALUES(@client,@date_change);";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.Parameters.AddWithValue("@client",client_code);
                command.Parameters.AddWithValue("@date_change",(DateTime.Now.ToString("yyyy-MM-dd")));
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" Ошибка при изменении статуса  " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Ошибка при изменении статуса  " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
}
