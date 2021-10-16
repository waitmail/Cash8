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
    public partial class CreateVirtualCard : Form
    {
        string code_answer = "";
        public Cash_check cash_Check = null;
        public CreateVirtualCard()
        {
            InitializeComponent();
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


        public class Buyer
        {
            public string phone { get; set; }
        }

        public class Register
        {
            public Buyer buyer { get; set; }
        }

        public class Res
        {
            public int res { get; set; }
        }


        private int create_virtual_card()
        {
            int result = 0;
            string url = MainStaticClass.GetStartUrl;//"https://evaviza1.cardnonstop.com/test/";
            Register register = new Register();
            Buyer buyer = new Buyer();
            buyer.phone = txtBox_phone.Text;
            register.buyer = buyer;
            //register.buyer = new Buyer();
            string json = JsonConvert.SerializeObject(register, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            var authString = MainStaticClass.GetAuthStringProcessing;

            request.Headers.Add("Authorization", "Basic " + authString);
            request.Headers.Add("X-FXAPI-RQ-METHOD", "crm.cabinet.Register");

            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = body.Length;

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
                    //string answer = JsonConvert.DeserializeObject(read.Replace("{}", @"""""")).ToString();//read.Replace("{}","\"\"")
                    Res res = JsonConvert.DeserializeObject<Res>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    result = res.res;
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

            return result;
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void btn_create_virtual_card_Click(object sender, EventArgs e)
        {

            int result = create_virtual_card();
            if (result != 1)
            {
                if (result == 4)
                {
                    MessageBox.Show("e - mail уже существует");
                }
                else if (result == 5)
                {
                    MessageBox.Show("карта не найдена");
                }
                else if (result == 6)
                {
                    MessageBox.Show("карта не активирована");
                }
                else if (result == 7)
                {
                    MessageBox.Show("карта заблокирована");
                }
                else if (result == 8)
                {
                    MessageBox.Show("карта уже зарегистрирована");
                }
                else if (result == 14)
                {
                    MessageBox.Show("телефон уже существует");
                }
                else if (result == 21)
                {
                    MessageBox.Show("неправильный код авторизации");
                }

                MessageBox.Show("Неудачная попытка создания виртуальной карты");
                return;
            }

            RequestSMSCode requestSMSCode = new RequestSMSCode();
            requestSMSCode.phone = "79788231858";//txtBox_phone.Text; // ЭТО НЕОБХОДИО РАСКОММЕНТИРОВАТЬ !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            string json = JsonConvert.SerializeObject(requestSMSCode, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string url = MainStaticClass.GetStartUrl + "/v3/requestSMSCode/";

            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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

                    ResponceRequestSMSCode responceRequestSMSCode = JsonConvert.DeserializeObject<ResponceRequestSMSCode>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (responceRequestSMSCode.res == "1")
                    {
                        code_answer = responceRequestSMSCode.code;
                        label3.Text = "Проверочный код в программе успешно получен,введите ниже код полученный на телефон ";
                        btn_check_number_phone.Enabled = true;
                        txtB_check_code.Enabled = true;
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
        }

        private void btn_check_number_phone_Click(object sender, EventArgs e)
        {
            if (txtB_check_code.Text.Trim() == code_answer)
            {
                MessageBox.Show("Введенный код принят, карта будет добавлена в чек");
                cash_Check.client.Tag  = txtBox_phone.Text;
                cash_Check.client.Text = txtBox_phone.Text;
            }
            else
            {
                MessageBox.Show("Вы ввели не верный код");
            }
        }
    }
}
