using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Npgsql;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace Cash8
{
    public partial class InputCodeNumberPhone : Form
    {
        public string code_client = "";
        public string phone_client = "";
        private string code_answer = "";       

        public InputCodeNumberPhone()
        {
            InitializeComponent();
            //this.txtB_last_6_number_code_phone.KeyPress += new KeyPressEventHandler(txtB_last_4_number_phone_KeyPress);
            this.txtB_last_6_number_code_phone.KeyPress += TxtB_last_6_number_code_phone_KeyPress;
            this.DialogResult = DialogResult.No;
        }

        private void TxtB_last_6_number_code_phone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {
                e.Handled = true;
            }
            else
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (txtB_last_6_number_code_phone.Text.Trim().Length < 6)
                    {
                        MessageBox.Show(" Для проверки присланного кода должно быть введено не менее 6 символов ");
                    }
                    else
                    {
                        //verification_of_the_phone_number();
                    }
                }
            }
        }

        //void txtB_last_4_number_phone_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
        //    {
        //        e.Handled = true;
        //    }
        //    else
        //    {
        //        if (e.KeyChar == (char)Keys.Enter)
        //        {
        //            if (txtB_last_6_number_code_phone.Text.Trim().Length < 4)
        //            {
        //                MessageBox.Show("Должно быть введено для проверки номера телефона не менее 4 последних цыфр номера телефона");
        //            }
        //            else
        //            {
        //                verification_of_the_phone_number();
        //            }
        //        }
        //    }
        //}


        ///// <summary>
        ///// Проверка правильности введенных 4 последних
        ///// цыфр телефонного номера
        ///// </summary>
        //private void verification_of_the_phone_number()
        //{
        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //    int result = 0;
        //    try
        //    {
        //        conn.Open();
        //        string query = "SELECT COUNT(*) FROM clients where right(phone,4)='"+txtB_last_6_number_phone.Text+"' AND code='"+code_client+"'";
        //        NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //        result = Convert.ToInt16(command.ExecuteScalar());
        //        conn.Close();
        //        command.Dispose();                
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show("Произошла ошибка при проверке " + ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Произошла ошибка при проверке " + ex.Message);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }
        //    if (result == 1)
        //    {
        //        this.Close();
        //        this.DialogResult = DialogResult.OK;
        //    }
        //}

        protected override void OnKeyDown(KeyEventArgs e)
        {
              if (e.KeyCode == Keys.Escape)
              {
                  this.Close();
                  this.DialogResult = DialogResult.Cancel;
              }
        }

               
        public class RequestSMSCode
        {
            public string phone { get; set; }
            public string notRegistered { get; set; }              
        }

        public class ResponceRequestSMSCode
        {
            public string code { get; set; }
            public string res { get; set; }
        }



        private void btn_send_sms_Click(object sender, EventArgs e)
        {
            RequestSMSCode requestSMSCode = new RequestSMSCode();
            if (MainStaticClass.GetWorkSchema == 1)
            {
                requestSMSCode.phone = code_client;
            }
            else if (MainStaticClass.GetWorkSchema == 2)
            {
                requestSMSCode.phone = phone_client;
                //requestSMSCode.notRegistered = "1";
            }
            if (MainStaticClass.GetWorkSchema == 1)
            {
                requestSMSCode.notRegistered = "0";
            }

            string json = JsonConvert.SerializeObject(requestSMSCode, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            //txtB_jason.Text = json;
            //string url = "http://92.242.41.218/processing/v3/requestSMSCode/";
            string url = MainStaticClass.GetStartUrl + "/v3/requestSMSCode/";

            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //string shop_request = "";
            //if (MainStaticClass.Nick_Shop.Substring(0, 1).ToUpper() == "A")
            //{
            //    shop_request = MainStaticClass.Nick_Shop + MainStaticClass.CashDeskNumber;
            //}
            //else
            //{
            //    shop_request = "1" + Convert.ToInt16(MainStaticClass.Nick_Shop.Substring(1, 2)).ToString() + MainStaticClass.CashDeskNumber;
            //}

            ////var authString = Convert.ToBase64String(Encoding.Default.GetBytes("A011" + ":" + "JpDkHs~AE%zS8Y7HDpVM"));
            //var authString = Convert.ToBase64String(Encoding.Default.GetBytes(shop_request + ":" + MainStaticClass.PassPromo));
            var authString = MainStaticClass.GetAuthStringProcessing;

            request.Headers.Add("Authorization", "Basic " + authString);

            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = body.Length;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                    stream.Close();
                }




                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    byte[] buf = new byte[10000];
                    int count = -1;
                    String read = "";
                    do
                    {
                        count = response.GetResponseStream().Read(buf, 0, buf.Length);
                        read += Encoding.UTF8.GetString(buf, 0, count);
                    } while (response.GetResponseStream().CanRead && count != 0);
                    //txtB_json_response.Text = JsonConvert.DeserializeObject(read).ToString();
                    //string answer = JsonConvert.DeserializeObject(read.Replace("{}", @"""""")).ToString();//read.Replace("{}","\"\"")
                    ResponceRequestSMSCode responceRequestSMSCode = JsonConvert.DeserializeObject<ResponceRequestSMSCode>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (responceRequestSMSCode.res == "1")
                    {
                        code_answer = responceRequestSMSCode.code;
                        label1.Text = "Проверочный код успешно получен";
                        btn_check_code.Enabled = true;
                        //MessageBox.Show(code_answer);
                    }
                    else if (responceRequestSMSCode.res == "20") //Куда то записать информацию о трудностях
                    {
                        MessageBox.Show(" Номер телефона не найден ");
                    }
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            //return result;
        }

        private void btn_check_code_Click(object sender, EventArgs e)
        {
            if (txtB_last_6_number_code_phone.Text.Trim() != code_answer)
            {
                MessageBox.Show("Вы ввели " + txtB_last_6_number_code_phone.Text + " это неверный код ");
            }
            else
            {
                MessageBox.Show("Вы ввели правильный проверочный код");
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }           
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
