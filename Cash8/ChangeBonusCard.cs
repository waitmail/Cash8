using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using Npgsql;
using System.Security.Cryptography;

namespace Cash8
{


    public partial class ChangeBonusCard : Form
    { 
        public string num_bonus_card = "";
        private string code_answer = "";
        public Cash_check cash_Check = null;
        private string uid = "";

        public ChangeBonusCard()
        {
            InitializeComponent();
        }

        public class Res
        {
            public int res { get; set; }
        }

        public class Register
        {
            public string cardNum { get; set; }
            public Buyer buyer { get; set; }
            public string cardPinHash { get; set; }
        }

        private int card_activate()
        {
            int result = 0;

            Register register = new Register();
            register.cardNum = txtB_num_card.Text;
            SHA256 sHA256 = SHA256.Create();
            register.cardPinHash = ComputeSHA256Hash(txtB_pin_code.Text);



            string json = JsonConvert.SerializeObject(register, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string url = MainStaticClass.GetStartUrl + "/v3/cardActivate/";

            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var authString = MainStaticClass.GetAuthStringProcessing;

            request.Headers.Add("Authorization", "Basic " + authString);

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
                    Res res = JsonConvert.DeserializeObject<Res>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    result = res.res;
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }


        private int card_block()
        {
            int result = 0;

            CardBlock cardBlock = new CardBlock();
            cardBlock.cardNum = num_bonus_card;
            cardBlock.comment = "Замена карты на " + txtB_num_card.Text;

            string json = JsonConvert.SerializeObject(cardBlock, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string url = MainStaticClass.GetStartUrl + "/";

            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            var authString = MainStaticClass.GetAuthStringProcessing;

            request.Headers.Add("Authorization", "Basic " + authString);
            request.Headers.Add("X-FXAPI-RQ-METHOD", "gcrm.operator.cardBlock");

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

                    Res res = JsonConvert.DeserializeObject<Res>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    result = res.res;

                    if (res.res == 1)
                    {
                        //Res res=
                        //code_answer = responceRequestSMSCode.code;
                        //label3.Text = label3.Text + "Проверочный код в программе успешно получен,введите ниже код полученный на телефон ";
                        //btn_create_bonus_card_or_add_phone.Enabled = true;
                        //txtB_check_code.Enabled = true;
                        //MessageBox.Show(code_answer);
                        
                    }
                    else if (res.res == 4) //Куда то записать информацию о трудностях
                    {
                        MessageBox.Show(" Карта не найдена ");
                    }
                    else if (res.res == 5) //Куда то записать информацию о трудностях
                    {
                        MessageBox.Show("Карта не активирована");
                    }
                    else if (res.res == 20) //Куда то записать информацию о трудностях
                    {
                        MessageBox.Show("недостаточно прав роли");
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

            return result;
        }

        //private int create_bonus_card(Register register)
        //{
        //    int result = 0;
        //    string url = MainStaticClass.GetStartUrl + "/";
        //    string json = JsonConvert.SerializeObject(register, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        //    byte[] body = Encoding.UTF8.GetBytes(json);
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    var authString = MainStaticClass.GetAuthStringProcessing;

        //    request.Headers.Add("Authorization", "Basic " + authString);
        //    request.Headers.Add("X-FXAPI-RQ-METHOD", "crm.cabinet.Register");

        //    request.Method = "POST";
        //    request.ContentType = "application/json; charset=utf-8";
        //    request.ContentLength = body.Length;

        //    try
        //    {
        //        using (Stream stream = request.GetRequestStream())
        //        {
        //            stream.Write(body, 0, body.Length);
        //            stream.Close();
        //        }


        //        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //        {
        //            byte[] buf = new byte[10000];
        //            int count = -1;
        //            String read = "";
        //            do
        //            {
        //                count = response.GetResponseStream().Read(buf, 0, buf.Length);
        //                read += Encoding.UTF8.GetString(buf, 0, count);
        //            } while (response.GetResponseStream().CanRead && count != 0);
        //            //string answer = JsonConvert.DeserializeObject(read.Replace("{}", @"""""")).ToString();//read.Replace("{}","\"\"")
        //            Res res = JsonConvert.DeserializeObject<Res>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //            result = res.res;
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //    return result;
        //}

        public class ResponceOperatorSearch
        {
            public string uid { get; set; }
            public string res { get; set; }
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

        private void btn_create_bonus_card_or_add_phone_Click(object sender, EventArgs e)
        {
            if (code_answer == txtB_check_code.Text.Trim())
            {
                //блокировка старой найденной карты 
                if (card_block() == 1)
                {
                    //присвение новой карты покупателю
                    BuyerInfoResponce buyerInfoResponce = get_buyerInfo_client_code_or_phone(0, txtB_num_card.Text);
                    if (buyerInfoResponce.cards.card[0].state == "1")
                    {
                        if (card_activate() != 1)
                        {
                            return;
                        }
                    }

                    if (assign_card() == 1)
                    {
                        MessageBox.Show("Введенный код принят, бонусная карта успешно создана и будет добавлена в чек");
                        cash_Check.client.Tag = txtB_num_card.Text;
                        cash_Check.client.Text = txtB_num_card.Text;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }

                }
            }
        }

        private void btn_request_sms_code_Click(object sender, EventArgs e)
        {
            label3.Text = "";
            BuyerInfoResponce buyerInfoResponce = get_buyerInfo_client_code_or_phone(1, txtBox_phone.Text);
            if (buyerInfoResponce.res != 1)
            {
                
            }
            else
            {
                num_bonus_card = buyerInfoResponce.cards.card[0].cardNum;
                uid = buyerInfoResponce.buyer.uid;
                label3.Text = "Карта клиента найдена \r\n";
                request_sms_code();
            }           
        }

        private string ComputeSHA256Hash(string text)
        {
            using (var sha256 = new SHA256Managed())
            {
                return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "");
            }
        }


        public class AssignCard
        {
            public string uid { get; set; }            
            public string cardNum { get; set; }
            public string cardPinHash { get; set; }            
        }


        private int assign_card()
        {
            int result = 0;

            AssignCard assignCard = new AssignCard();
            assignCard.uid = uid;
            assignCard.cardNum = txtB_num_card.Text;
            assignCard.cardPinHash = ComputeSHA256Hash(txtB_pin_code.Text);

            string json = JsonConvert.SerializeObject(assignCard, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string url = MainStaticClass.GetStartUrl + "/";

            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            var authString = MainStaticClass.GetAuthStringProcessing;

            request.Headers.Add("Authorization", "Basic " + authString);
            request.Headers.Add("X-FXAPI-RQ-METHOD", "crm.cabinet.assignCard");

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

                    Res res = JsonConvert.DeserializeObject<Res>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    result = res.res;

                    if (res.res == 1)
                    {                        

                    }
                    else if (res.res == 3) //Куда то записать информацию о трудностях
                    {
                        MessageBox.Show(" Карта не найдена ");
                    }
                    else if (res.res == 4) //Куда то записать информацию о трудностях
                    {
                        MessageBox.Show(" карта не найдена, или неправильный PIN");
                    }
                    else if (res.res == 5) //Куда то записать информацию о трудностях
                    {
                        MessageBox.Show("Карта не активирована");
                    }
                    else if (res.res == 11) //Куда то записать информацию о трудностях
                    {
                        MessageBox.Show("карта уже зарегистрирована");
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

            return result;
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

        public class CardBlock
        {
            public string cardNum { get; set; }
            public string comment { get; set; }
        }

        private int request_sms_code()
        {
            int result = 0;

            RequestSMSCode requestSMSCode = new RequestSMSCode();

            requestSMSCode.phone = "7"+txtBox_phone.Text;
            requestSMSCode.notRegistered = "0";//телефон в этом случае уже зарегистрирован

            string json = JsonConvert.SerializeObject(requestSMSCode, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string url = MainStaticClass.GetStartUrl + "/";

            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            var authString = MainStaticClass.GetAuthStringProcessing;

            request.Headers.Add("Authorization", "Basic " + authString);
            request.Headers.Add("X-FXAPI-RQ-METHOD", "crm.cabinet.requestSMSCode");

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
                    result = Convert.ToInt16(responceRequestSMSCode.res);

                    if (responceRequestSMSCode.res == "1")
                    {
                        code_answer = responceRequestSMSCode.code;
                        label3.Text = label3.Text+"Проверочный код в программе успешно получен,введите ниже код полученный на телефон ";
                        btn_create_bonus_card_or_add_phone.Enabled = true;
                        txtB_check_code.Enabled = true;
                        //MessageBox.Show(code_answer);
                    }
                    else if (responceRequestSMSCode.res == "13") //Куда то записать информацию о трудностях
                    {
                        MessageBox.Show(" Телефон не найден ");
                    }
                    else if (responceRequestSMSCode.res == "14") //Куда то записать информацию о трудностях
                    {
                        MessageBox.Show("Этот номер телефона уже зарегистрирован");
                    }
                    else if (responceRequestSMSCode.res == "25") //Куда то записать информацию о трудностях
                    {
                        MessageBox.Show("Превышено число попыток");
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

            return result;
        }

        private BuyerInfoResponce get_buyerInfo_client_code_or_phone(int variant, string client_code_or_phone)
        {
            
            BuyerInfoResponce buyerInfoResponce = null;

            BuyerInfoRequest buyerInfoRequest = new BuyerInfoRequest();
            if (variant == 0)
            {
                buyerInfoRequest.cardNum = client_code_or_phone;// client.Tag.ToString();
            }
            else
            {
                buyerInfoRequest.phone = "7"+client_code_or_phone;// client.Tag.ToString();
            }
            string json = JsonConvert.SerializeObject(buyerInfoRequest, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string url = MainStaticClass.GetStartUrl + "/v3/buyerInfo/";

            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            var authString = MainStaticClass.GetAuthStringProcessing;
            request.Headers.Add("Authorization", "Basic " + authString);
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
                    buyerInfoResponce = JsonConvert.DeserializeObject<BuyerInfoResponce>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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

            return buyerInfoResponce;
        }

        public class BuyerInfoRequest
        {
            public string cardTrack2 { get; set; }
            public string phone { get; set; }
            public string cardNum { get; set; }
        }



        public class CardStatus
        {

        }

        public class Card
        {
            public string cardNum { get; set; }
            public string state { get; set; }
            public string mode { get; set; }
            public string dateIssued { get; set; }
            public string dateExpired { get; set; }
            public string dateActivated { get; set; }
            public string dateRegistered { get; set; }
            public string discount { get; set; }
            public string value { get; set; }
            public CardStatus cardStatus { get; set; }
        }

        public class Cards
        {
            public List<Card> card { get; set; }
        }

        public class Buyer
        {
            public string uid { get; set; }
            public string firstName { get; set; }
            public string middleName { get; set; }
            public string lastName { get; set; }
            public string phone { get; set; }
            public string spendAllowed { get; set; }
            public string phoneConfirmed { get; set; }
        }       

        public class Balance
        {
            public string balance { get; set; }
            public string activeBalance { get; set; }
            public string inactiveBalance { get; set; }
            public string oddMoneyBalance { get; set; }
            public string oddMoneyFlags { get; set; }
        }

        public class BuyerInfoResponce
        {
            public Cards cards { get; set; }
            public Buyer buyer { get; set; }
            public Balance balance { get; set; }
            public string requestId { get; set; }
            public int res { get; set; }
        }

    }
}
