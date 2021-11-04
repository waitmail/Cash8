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
    public partial class CreateBonusCardOrAddPhone : Form
    {
        public bool its_new = false;
        string code_answer = "";
        public Cash_check cash_Check = null;


        public CreateBonusCardOrAddPhone()
        {
            InitializeComponent();
            this.txtBox_phone.KeyPress += TxtBox_phone_KeyPress;
        }

        private void TxtBox_phone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                if (txtBox_phone.Text.ToString().Trim().Length != 10)
                {
                    MessageBox.Show("Номер телефона должен содержать 10 цифр");
                    return;
                }
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

        //public class ResponceOperatorSearch
        //{
        //    public string uid { get; set; }
        //    public string res { get; set; }
        //}

        public class Buyer
        {
            public string phone { get; set; }
        }

        public class Register
        {
            public string cardNum { get; set; }
            public Buyer buyer { get; set; }
            public string cardPinHash { get; set; }            
        }

        public class Res
        {
            public int res { get; set; }
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
        
        private int create_bonus_card(Register register)
        {
            int result = 0;
            string url = MainStaticClass.GetStartUrl + "/";
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


        private int request_sms_code()
        {
            int result = 0;

            RequestSMSCode requestSMSCode = new RequestSMSCode();

            requestSMSCode.phone = txtBox_phone.Text;
            requestSMSCode.notRegistered = "1";

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
                        label3.Text = "Проверочный код в программе успешно получен,введите ниже код полученный на телефон ";
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

        private string ComputeSHA256Hash(string text)
        {
            using (var sha256 = new SHA256Managed())
            {
                return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "");
            }
        }

        private void btn_create_bonus_card_or_add_phone_Click(object sender, EventArgs e)
        {
            if (code_answer == txtB_check_code.Text.Trim())
            {
                if (its_new)
                {                    
                    Register register = new Register();
                    register.cardNum= txtB_num_card.Text;
                    SHA256 sHA256 = SHA256.Create();
                    register.cardPinHash = ComputeSHA256Hash(txtB_pin_code.Text);
                    Buyer buyer = new Buyer();
                    buyer.phone = txtBox_phone.Text;
                    //buyer.cardNum = txtB_num_card.Text;
                    register.buyer = buyer;

                    int result = create_bonus_card(register);

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

                        MessageBox.Show("Неудачная попытка при регистрации карты");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Введенный код принят, бонусная карта успешно создана и будет добавлена в чек");
                        cash_Check.client.Tag = register.cardNum;
                        cash_Check.client.Text = txtBox_phone.Text;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    //    ResponceOperatorSearch responceOperatorSearch = operator_search();
                    //    if (responceOperatorSearch.res == "1")
                    //    {
                    //        Res res = update_buyer_info(responceOperatorSearch.uid);
                    //        if (res.res == 1)
                    //        {
                    //            this.Close();
                    //        }
                    //    }
                    //    else if (responceOperatorSearch.res == "26")
                    //    {
                    MessageBox.Show("Произошло необрабатываемое исключение");
                    this.Close();

                    //    }
                }
            }
            else
            {
                MessageBox.Show("Вы ввели неправильный проверочный код");
            }
        }


        public class UpdateBuyerInfo
        {
            public Buyer buyer { get; set; }
            public string uid { get; set; }
        }

        /// <summary>
        /// Привязывает номер телефона к карточке покупателя
        /// </summary>
        private Res update_buyer_info(string uid)
        {
            Res res = null;
            string url = MainStaticClass.GetStartUrl + "/";
            UpdateBuyerInfo updateBuyerInfo = new UpdateBuyerInfo();
            updateBuyerInfo.uid = uid;
            Buyer buyer = new Buyer();
            buyer.phone = txtBox_phone.Text;
            updateBuyerInfo.buyer = buyer;
            string json = JsonConvert.SerializeObject(updateBuyerInfo, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var authString = MainStaticClass.GetAuthStringProcessing;
            request.Headers.Add("Authorization", "Basic " + authString);
            request.Headers.Add("X-FXAPI-RQ-METHOD", "crm.cabinet.updateBuyerInfo");
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
                    res = JsonConvert.DeserializeObject<Res>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (res.res == 1)
                    {
                        MessageBox.Show("Номер телефона успешно добавлен в карточку клиента");
                    }
                    else if (res.res == 3)
                    {
                        MessageBox.Show("Ошибка при добавлении номера телефона в карточку клиента, uid не найден !!!");
                    }
                    else if (res.res == 4)
                    {
                        MessageBox.Show("Ошибка при добавлении номера телефона в карточку клиента, email существует !!!");
                    }
                    else if (res.res == 14)
                    {
                        MessageBox.Show("Ошибка при добавлении номера телефона в карточку клиента, номер телефона уже существует !!!");
                    }
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

            return res;
        }


        ///// <summary>
        ///// Ищет на процессинге покупателя
        ///// по номеру карты
        ///// и возвращает его uid
        ///// </summary>
        //private ResponceOperatorSearch operator_search()
        //{
        //    ResponceOperatorSearch responceOperatorSearch = null;
        //    string url = MainStaticClass.GetStartUrl + "/";                        
        //    Buyer buyer = new Buyer();            
        //    buyer.cardNum = txtB_num_card.Text;            
        //    string json = JsonConvert.SerializeObject(buyer, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    byte[] body = Encoding.UTF8.GetBytes(json);
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    var authString = MainStaticClass.GetAuthStringProcessing;
        //    request.Headers.Add("Authorization", "Basic " + authString);
        //    request.Headers.Add("X-FXAPI-RQ-METHOD", "crm.operator.Search");
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
        //            responceOperatorSearch = JsonConvert.DeserializeObject<ResponceOperatorSearch>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });                    
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

        //    return responceOperatorSearch;
        //}

        private void btn_request_sms_code_Click(object sender, EventArgs e)
        {
            int status_card = get_status_promo_card(txtB_num_card.Text);
            if (status_card == 1)
            {
                if (card_activate() == 1)
                {
                    request_sms_code();
                }
            }
            else if (status_card == 3)
            {
                request_sms_code();
            }
        }

        private BuyerInfoResponce get_buyerInfo_client_code_or_phone(int variant, string client_code_or_phone)
        {
            //bool result = true;
            BuyerInfoResponce buyerInfoResponce = null;

            BuyerInfoRequest buyerInfoRequest = new BuyerInfoRequest();
            if (variant == 0)
            {
                buyerInfoRequest.cardNum = client_code_or_phone;// client.Tag.ToString();
            }
            else
            {
                buyerInfoRequest.phone = client_code_or_phone;// client.Tag.ToString();
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

        //public class Buyer
        //{
        //    public string uid { get; set; }
        //    public string firstName { get; set; }
        //    public string middleName { get; set; }
        //    public string lastName { get; set; }
        //    public string phone { get; set; }
        //    public string spendAllowed { get; set; }
        //    public string phoneConfirmed { get; set; }
        //}

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

        /// <summary>
        /// По штрихкоду карты возвращает ее статус
        /// </summary>
        /// <param name="card_barcode"></param>
        /// <returns></returns>
        private int get_status_promo_card(string card_barcode)
        {
            int result = 0;

            BuyerInfoResponce buyerInfoResponce = null;
            buyerInfoResponce = get_buyerInfo_client_code_or_phone(0, card_barcode);

            if (buyerInfoResponce != null)
            {
                if (buyerInfoResponce.res == 1)
                {
                    if (buyerInfoResponce.cards.card.Count > 0)
                    {
                        result = Convert.ToInt16(buyerInfoResponce.cards.card[0].state);
                    }
                }
            }

            return result;
        }

    }
}
