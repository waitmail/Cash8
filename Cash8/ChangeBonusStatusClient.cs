﻿using System;
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
            this.Load += ChangeBonusStatusClient_Load;
        }

        private void ChangeBonusStatusClient_Load(object sender, EventArgs e)
        {
            get_phone_client();
        }

        private void get_phone_client()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {             
                conn.Open();
                string query = "SELECT clients.phone AS phone, coalesce(temp_phone_clients.phone,'') AS phone1 FROM clients " +
                    " left join temp_phone_clients ON clients.code = temp_phone_clients.barcode " +
                    " WHERE clients.code='" + client_code + "'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if ((reader["phone"].ToString().Trim().Length >= 10))
                    {
                        txtB_phone.Text = reader["phone"].ToString().Trim();
                    }
                    else if ((reader["phone1"].ToString().Trim().Length >= 10))
                    {
                        txtB_phone.Text = reader["phone1"].ToString().Trim();
                    }
                }
                reader.Close();
                conn.Close();

                if (txtB_phone.Text.Trim().Length > 10)
                {
                    txtB_phone.Text = txtB_phone.Text.Substring(0, 2) + " " + txtB_phone.Text.Substring(2, 3) + " " + txtB_phone.Text.Substring(5, 3) + " " + txtB_phone.Text.Substring(8, 2) + " " + txtB_phone.Text.Substring(10, 2);
                }
                else if (txtB_phone.Text.Trim().Length == 10)
                {
                    txtB_phone.Text = txtB_phone.Text.Substring(0, 3) + " " + txtB_phone.Text.Substring(3, 3) + " " + txtB_phone.Text.Substring(6, 2) + " " + txtB_phone.Text.Substring(8, 2);
                }
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


        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void btn_execute_Click(object sender, EventArgs e)
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "INSERT INTO public.client_with_changed_status_to_send(client,date_change,new_phone_number)VALUES(@client,@date_change,@new_phone_number);";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.Parameters.AddWithValue("@client",client_code);
                command.Parameters.AddWithValue("@date_change",(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")));
                string new_phone = maskedTxtB_new_phone_number.Text.Trim().Replace(" ", "").Replace("+7", "");
                if ((new_phone.Length != 10)&&(new_phone.Length != 0))
                {
                    MessageBox.Show("Неверная длина номера телефона");
                    return;
                }
                command.Parameters.AddWithValue("@new_phone_number", maskedTxtB_new_phone_number.Text.Trim().Replace(" ","").Replace("+7",""));

                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
                MessageBox.Show("Заявка на изменение статуса успешно создана");
                this.Close();
                this.DialogResult = DialogResult.OK;
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
