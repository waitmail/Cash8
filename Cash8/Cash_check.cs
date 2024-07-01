using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Npgsql;
using System.Threading;
using System.Collections;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Cash8
{
    public partial class Cash_check : Form
    {

        //public int bonus_is_on_earlier = 0;
        //private int bonus_is_on_now = 0;
        public bool enable_delete = false;
        private string[] print_data;
        private bool selection_goods = false;
        public bool have_action = false;
        private StringBuilder print_string = new StringBuilder();
        private int count_pages = 0;
        public int to_print_certainly = 0;
        public int to_print_certainly_p = 0;
        //Флаг закрытия документа
        public bool closing = true;//в этом значении форма не закрывается, закрывается тогда когда становится false
        public bool inpun_action_barcode = false;//Доступ из формы ввода акционного штрихкода
        private Read_Data_From_Com_Port rd = null;
        // private System.IO.Ports.SerialPort mySerial;
        public ArrayList action_barcode_list = new ArrayList();//Доступ из формы ввода акционного штрихкода 
        public ArrayList action_barcode_bonus_list = new ArrayList();//Доступ из формы ввода акционного штрихкода                           
        ListView listview_original = new ListView();
        //private decimal discount = 1;
        private double discount = 0;
        public Int64 numdoc = 0;
        private bool inpun_client_barcode = false;
        //public Nomenklatura tovar;
        private int curpos = 0;
        public bool itsnew = true;
        //private bool input_edit_barcode = false;
        //private string num_cash = "";
        public string date_time_write = "";
        //Переменные для печати
        //string pay,string sum_doc, string remainder
        public string p_sum_pay = "";
        public string p_sum_doc = "";
        public string p_remainder = "";
        public string p_discount = "0";
        Thread workerThread = null;
        private DateTime start_action = DateTime.Now;
        private Int32 total_seconnds = 0;
        private DataTable table = new DataTable();
        private int action_num_doc = 0;
        public string cashier = "";
        public int added_bonus_when_replacing_card = 0;
        public decimal bonuses_it_is_written_off = 0;
        public int bonus_total_centr = 0;
        public int return_bonus = 0;
        public int client_barcode_scanned = 0;
        public bool it_is_possible_to_write_off_bonuses = false;//флаг возможности списания бонусов устанавливается в истина если клиент найден покарте, а не по номеру телефона
        public string id_transaction = "";
        private string id_transaction_sale = "";
        //public int bonus_on_document = 0;
        public int bonuses_it_is_counted = 0;
        private Pay pay_form = new Pay();
        //private string cardTrack2 = "";
        private string phone = "";
        public string qr_code = "";
        public Int32 id_sale = 0;
        public string phone_client = "";
        private int card_state = 0; // state – состояние карты, одно из значений: 1 – карта неактивна 2 – карта активирована(выдана на кассе) 3 – карта зарегистрирована(привязана к анкете клиента) 4 – карта заблокирована
        private string code_bonus_card = "";
        public string spendAllowed = "";
        public string message_processing = "";
        public bool change_bonus_card = false;
        private bool client_plastic_scaned = false;//клиент определен по платиковой карте или по номеру платиковой карты
        AnswerAddingKmArrayToTableTested answerAddingKmArrayToTableTested = null;
        public string id_transaction_terminal     = "";
        public string code_authorization_terminal = "";
        public string sale_id_transaction_terminal = "";
        public string sale_code_authorization_terminal = "";
        public DateTime sale_date;
        public int print_to_button = 0;
        public string recharge_note="";//здесь будет присвоена иформация об оплате по карте когда терминал без печтчающего устройства.
        public string guid = "";
        private string guid1 = "";
        public string guid_sales = "";
        public string tax_order = "";
        public bool external_fix = false;
        public double sale_non_cash_money = 0;
        public bool payment_by_sbp = false;
        public bool payment_by_sbp_sales = false;       

        //public Dictionary<string, Cash_check.CdnMarkerDateTime> cdn_markers_date_time = new Dictionary<string, Cash_check.CdnMarkerDateTime>();

        public Dictionary<string, uint>  cdn_markers_result_check = new Dictionary<string, uint>();

        System.Windows.Forms.Timer timer = null;

        HttpWebRequest request = null;
        //список допустимых длин qr кодов
        List<int> qr_code_lenght = new List<int>();
        ToolTip toolTip = null;
        //(HttpWebRequest)WebRequest.Create("url");
        //request.KeepAlive = true;
        //request.Method = "GET"; // Или другой метод, который вы используете

        //public class CdnMarkerDateTime
        //{
        //    public string reqId { get; set; }

        //    public long reqTimestamp { get; set; }
        //}

        //public class CdnMarkersResultCheck
        //{
        //    public string marker_code { get; set; }

        //    public Int16 result_check { get; set; }
        //}

        public class CustomerScreen
        {
            public List<CheckPosition> ListCheckPositions { get; set; }
            public int show_price { get; set; }
        }

        public class CheckPosition
        {
            public string NamePosition { get; set; }
            public string Quantity { get; set; }
            public string Price { get; set; }
        }
        /// <summary>
        /// Если mode == 0 то тогда товары не передаются чек закрыт
        /// mode == 1 Это отрисовка товаров 
        /// Если show_price == 0 тогда цены не отображаются
        /// отображаются номенклатура и количество
        /// Если show_price == 1 тогда цены отображаются, пока 
        /// этот режим будет доступен после перехода в окно оплаты
        /// </summary>
        /// <param name="mode"></param>
        public void SendDataToCustomerScreen(int mode, int show_price, int calculate_actionc)
        {
            //if (MainStaticClass.GetWorkSchema == 2)
            //{
            //    return;
            //}
            //if (MainStaticClass.OneMonitorsConnected == 1)
            //{
            //    return;
            //}
            try
            {
                //if ((MainStaticClass.UseOldProcessiingActions) || (!itsnew))
                if ((mode == 1 && show_price == 1) || (mode == 0 && show_price == 0))
                {
                    CustomerScreen customerScreen = new CustomerScreen();
                    customerScreen.show_price = show_price;
                    customerScreen.ListCheckPositions = new List<CheckPosition>();
                    if (mode == 1)
                    {
                        foreach (ListViewItem listViewItem in listView1.Items)
                        {
                            CheckPosition checkPosition = new CheckPosition();
                            checkPosition.NamePosition = listViewItem.SubItems[1].Text;
                            checkPosition.Quantity = listViewItem.SubItems[3].Text;
                            checkPosition.Price = listViewItem.SubItems[5].Text;
                            customerScreen.ListCheckPositions.Add(checkPosition);
                        }
                    }
                    string message = JsonConvert.SerializeObject(customerScreen, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    SendUDPMessage(message);
                }
                else
                {
                    CustomerScreen customerScreen = new CustomerScreen();
                    customerScreen.show_price = 1;
                    customerScreen.ListCheckPositions = new List<CheckPosition>();
                    DataTable dataTable = to_define_the_action_dt(false);
                    this.txtB_total_sum.Text = calculation_of_the_sum_of_the_document().ToString() +" / "+Convert.ToDouble(dataTable.Compute("Sum(sum_at_discount)", (string)null)).ToString("F2");
                    foreach (DataRow row in dataTable.Rows)
                    {
                        CheckPosition checkPosition = new CheckPosition();
                        checkPosition.NamePosition = row["tovar_name"].ToString();
                        checkPosition.Quantity = row["quantity"].ToString();
                        checkPosition.Price = row["price_at_discount"].ToString();
                        customerScreen.ListCheckPositions.Add(checkPosition);
                    }
                    string message = JsonConvert.SerializeObject(customerScreen, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    SendUDPMessage(message);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("SendDataToCustomerScreen "+ex.Message);
            }
        }

        private static void SendUDPMessage(string message)
        {
            UdpClient sender = new UdpClient(); // создаем UdpClient для отправки сообщений
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                sender.Send(data, data.Length, "127.0.0.1", 12345); // отправка
            }
            catch (Exception ex)
            {
                MessageBox.Show("SendDataToCustomerScreen " + ex.Message);
            }
            finally
            {
                sender.Close();
            }
        }


        //БОНУСЫ 
        //public Decimal bonus_on_document = 0;
        //

        public Cash_check()
        {
            InitializeComponent();

            //this.listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", MainStaticClass.Font_list_view(), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.KeyPreview = true;
            this.return_quantity.KeyPress += new KeyPressEventHandler(return_quantity_KeyPress);
            this.return_rouble.KeyPress += new KeyPressEventHandler(return_rouble_KeyPress);
            this.return_kop.KeyPress += new KeyPressEventHandler(return_kop_KeyPress);
            this.listView1.KeyPress += new KeyPressEventHandler(listView1_KeyPress);
            this.listView1.KeyDown += ListView1_KeyDown;            
            this.txtB_client_phone.KeyPress += new KeyPressEventHandler(txtB_client_phone_KeyPress);
            if (MainStaticClass.Code_right_of_user == 1)
            {
                this.checkBox_to_print_repeatedly.Visible = true;
            }
            this.checkBox_to_print_repeatedly.CheckStateChanged += new EventHandler(checkBox_to_print_repeatedly_CheckStateChanged);
            txtB_inn.KeyPress += new KeyPressEventHandler(TxtB_inn_KeyPress);
            comment.KeyPress += new KeyPressEventHandler(Comment_KeyPress);
            this.txtB_total_sum.MouseHover += TxtB_total_sum_MouseHover;
            //this.Paint += Cash_check_Paint;
            toolTip = new ToolTip();            
            toolTip.SetToolTip(this.txtB_total_sum, "Сумма без промо / Сумма с учетом промо");
            txtB_total_sum.KeyPress += TxtB_total_sum_KeyPress;
            numericUpDown_enter_quantity.KeyPress += NumericUpDown_enter_quantity_KeyPress;            
            numericUpDown_enter_quantity.ValueChanged += NumericUpDown_enter_quantity_ValueChanged;
            TextBox textBox = (TextBox)numericUpDown_enter_quantity.Controls[1];
            textBox.KeyPress += TextBox_KeyPress;
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            //MessageBox.Show(e.KeyChar.ToString(), "Нажатие");
            // Замена точки на запятую
            if (e.KeyChar == '.')
            {
                //textBox.Focus();
                //SendKeys.Send(",");
                //e.Handled = true;
                e.Handled = true; // предотвращаем ввод точки
                if (textBox.Text.IndexOf(",") == -1)
                {
                    //MessageBox.Show(" Это точка и сейчас преобразуется в запятую ");
                    textBox.Text += ','; // добавляем запятую в текстовое поле
                    textBox.SelectionStart = textBox.Text.Length; // перемещаем курсор в конец текста
                }
                return;
            }
            else
            {
                //MessageBox.Show(" Это не точка, код символа " + Convert.ToInt32(e.KeyChar).ToString());
            }

            // Проверка, что введенный символ - это цифра, управляющий символ или запятая
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            else
            {
                // Обработка ввода, если уже есть запятая в тексте
                if (textBox.Text.Contains(","))
                {
                    if ((e.KeyChar == '.') || (e.KeyChar == ','))
                    {
                        e.Handled = true;
                        return;
                    }
                    // Проверка количества знаков после запятой
                    string[] parts = textBox.Text.Split(',');
                    if (parts.Length == 2 && parts[1].Length >= numericUpDown_enter_quantity.DecimalPlaces)
                    {
                        // Если курсор находится после запятой и количество знаков после запятой уже равно или больше установленного
                        if (textBox.SelectionStart > textBox.Text.IndexOf(',') && parts[1].Length >= 3)
                        {
                            // Заменяем следующий символ, если не выделено символов для замены
                            if (textBox.SelectionLength == 0 && parts[1].Length == 3)
                            {
                                int selectionStart = textBox.SelectionStart;
                                if (selectionStart >= 0 && selectionStart < textBox.Text.Length)
                                {
                                    // Удаляем символ в позиции selectionStart, если это возможно
                                    textBox.Text = textBox.Text.Remove(selectionStart, 1);
                                    // Вставляем новый символ в ту же позицию
                                    textBox.Text = textBox.Text.Insert(selectionStart, e.KeyChar.ToString());
                                    textBox.SelectionStart = selectionStart + 1; // Сдвигаем курсор
                                    e.Handled = true;
                                }
                                textBox.SelectionStart = selectionStart + 1; // Сдвигаем курсор
                                e.Handled = true;
                            }
                        }
                    }
                }
            }
        }

        //private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    TextBox textBox = sender as TextBox;
        //    //// Проверяем, что введенный символ - это цифра или управляющий символ
        //    //if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
        //    //{
        //    //    e.Handled = true;
        //    //}
        //    //else
        //    //{
        //    //    // Проверяем количество знаков после запятой
        //    //    if (textBox.Text.Contains(","))
        //    //    {
        //    //        int commaPosition = textBox.Text.IndexOf(',');
        //    //        int decimalCount = textBox.Text.Length - commaPosition - 1;
        //    //        // Если курсор находится после запятой и количество знаков после запятой уже три или больше
        //    //        if (textBox.SelectionStart > commaPosition && decimalCount > 3)
        //    //        {
        //    //            e.Handled = true;
        //    //        }
        //    //    }
        //    //}

        //    //if ((Keys)e.KeyChar == Keys.Back)
        //    //{
        //    //    e.Handled = true;
        //    //}
        //    if (e.KeyChar == '.')
        //    {
        //        textBox.Focus();
        //        SendKeys.Send(",");
        //        e.Handled = true;
        //        return;
        //    }

        //    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
        //    {
        //        if (e.KeyChar.ToString() == ",")
        //        {
        //            if (textBox.Text.Contains(","))
        //            {
        //                e.Handled = true;
        //            }
        //        }
        //    }
        //    else
        //    {

        //        if (textBox.Text.Contains(","))
        //        {

        //            if (textBox.Text.Split(',').Length == 2)
        //            {
        //                if (textBox.Text.Split(',')[1].Length < numericUpDown_enter_quantity.DecimalPlaces)
        //                {
        //                    return;
        //                }
        //            }

        //            if (textBox.SelectionStart >= textBox.Text.Length)
        //            {
        //                e.Handled = true;
        //            }
        //            else
        //            {
        //                int commaPosition = textBox.Text.IndexOf(',');
        //                int decimalCount = textBox.Text.Length - commaPosition - 1;
        //                if (textBox.SelectionStart > commaPosition && decimalCount >= 3)
        //                {
        //                    // Если курсор находится после запятой и количество знаков после запятой уже три или больше
        //                    if (textBox.SelectionLength == 0 && decimalCount == 3)
        //                    {
        //                        // Заменяем следующий символ, если не выделено символов для замены
        //                        int selectionStart = textBox.SelectionStart;
        //                        textBox.Text = textBox.Text.Remove(selectionStart, 1).Insert(selectionStart, e.KeyChar.ToString());
        //                        textBox.SelectionStart = selectionStart + 1; // Сдвигаем курсор
        //                        e.Handled = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}


        private void NumericUpDown_enter_quantity_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown_enter_quantity.Value = Decimal.Round(numericUpDown_enter_quantity.Value, 3);
        }

        //private void enter_quantity_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        //{

        //    if (e.KeyChar == 27)
        //    {
        //        this.enter_quantity.Visible = false;
        //        this.panel1.Visible = false;
        //        return;
        //    }

        //    if (e.KeyChar == 13)
        //    {
        //        if ((this.enter_quantity.Text.Length == 0) || (Convert.ToDecimal(this.enter_quantity.Text) == 0))
        //        {
        //            MessageBox.Show("Количество не может быть пустым");
        //            return;
        //        }

        //        if (Convert.ToInt32(this.listView1.SelectedItems[0].SubItems[3].Text) > Convert.ToInt32(this.enter_quantity.Text))
        //        {
        //            //MessageBox.Show("Не администраторам запрещено менять количество на меньшее");
        //            ///////////////////////////////////////////////////////////////
        //            if (MainStaticClass.Code_right_of_user != 1)
        //            {
        //                enable_delete = false;
        //                Interface_switching isw = new Interface_switching();
        //                isw.caller_type = 3;
        //                isw.cc = this;
        //                isw.not_change_Cash_Operator = true;
        //                isw.ShowDialog();
        //                isw.Dispose();

        //                if (!enable_delete)
        //                {
        //                    MessageBox.Show("Вам запрещено менять количество на меньшее");
        //                    //this.enter_quantity.Text = "0";
        //                    return;
        //                }
        //            }

        //            insert_incident_record(listView1.SelectedItems[0].SubItems[0].Text, (Convert.ToInt32(listView1.SelectedItems[0].SubItems[3].Text) - Convert.ToInt32(this.enter_quantity.Text)).ToString(), "1");

        //        }
        //        else if (Convert.ToInt32(this.listView1.SelectedItems[0].SubItems[3].Text) < Convert.ToInt32(this.enter_quantity.Text))
        //        {

        //            //Проверка на сертификат 
        //            if (its_sertificate(this.listView1.SelectedItems[0].SubItems[0].Text.Trim()))
        //            {
        //                MessageBox.Show("Каждый сертификат продается отдельной строкой");
        //                return;
        //            }
        //        }

        //        this.listView1.SelectedItems[0].SubItems[3].Text = Convert.ToInt16(this.enter_quantity.Text).ToString();                
        //        recalculate_all();
        //        calculation_of_the_sum_of_the_document();                
        //        this.enter_quantity.Visible = false;
        //        this.panel1.Visible = false;
        //        this.listView1.Select();                
        //        this.listView1.Items[this.listView1.SelectedIndices[0]].Selected = true;
        //        write_new_document("0", "0", "0", "0", false, "0", "0", "0", "0");
        //    }

        //    if ((enter_quantity.Text.Length == 0))
        //    {
        //        if (e.KeyChar == 48)
        //        {
        //            e.Handled = true;
        //        }
        //    }

        //    if (!(Char.IsDigit(e.KeyChar)))
        //    {

        //        if (e.KeyChar != (char)Keys.Back)
        //        {
        //            e.Handled = true;
        //        }
        //    }

        //    //if (!(Char.IsDigit(e.KeyChar)) && !((e.KeyChar == '.') && (enter_quantity.Text.IndexOf(".") == -1) && (enter_quantity.Text.Length != 0)))
        //    //{
        //    //    if (e.KeyChar != (char)Keys.Back)
        //    //    {
        //    //        e.Handled = true;
        //    //    }
        //    //}

        //    //if (!(Char.IsDigit(e.KeyChar)) && !((e.KeyChar == '.') && (enter_quantity.Text.IndexOf(".") == -1)))
        //    //{
        //    //    if (e.KeyChar != (char)Keys.Back)
        //    //    {
        //    //        e.Handled = true;
        //    //    }
        //    //}

        //    SendDataToCustomerScreen(1, 0,1);
        //}

        private void NumericUpDown_enter_quantity_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == 27)
            {
                this.numericUpDown_enter_quantity.Visible = false;
                this.panel1.Visible = false;
                return;
            }

            if (e.KeyChar == 13)
            {
                if (this.numericUpDown_enter_quantity.Value == 0)
                {
                    MessageBox.Show("Количество не может быть пустым");
                    return;
                }

                if (Convert.ToDecimal(this.listView1.SelectedItems[0].SubItems[3].Text) > this.numericUpDown_enter_quantity.Value)
                {
                    //MessageBox.Show("Не администраторам запрещено менять количество на меньшее");
                    ///////////////////////////////////////////////////////////////
                    if (MainStaticClass.Code_right_of_user != 1)
                    {
                        enable_delete = false;
                        Interface_switching isw = new Interface_switching();
                        isw.caller_type = 3;
                        isw.cc = this;
                        isw.not_change_Cash_Operator = true;
                        isw.ShowDialog();
                        isw.Dispose();

                        if (!enable_delete)
                        {
                            MessageBox.Show("Вам запрещено менять количество на меньшее");
                            //this.enter_quantity.Text = "0";
                            return;
                        }
                    }
                    ReasonsDeletionCheck reasons = new ReasonsDeletionCheck();
                    DialogResult dialogResult = reasons.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        insert_incident_record(listView1.SelectedItems[0].SubItems[0].Text, Math.Round(Convert.ToDecimal(listView1.SelectedItems[0].SubItems[3].Text) - this.numericUpDown_enter_quantity.Value, 2, MidpointRounding.ToEven).ToString().Replace(",", "."), "1",reasons.reason);
                    }
                    else
                    {
                        return;
                    }

                }
                else if (Convert.ToDecimal(this.listView1.SelectedItems[0].SubItems[3].Text) < this.numericUpDown_enter_quantity.Value)
                {

                    //Проверка на сертификат 
                    if (its_sertificate(this.listView1.SelectedItems[0].SubItems[0].Text.Trim()))
                    {
                        MessageBox.Show("Каждый сертификат продается отдельной строкой");
                        return;
                    }
                }

                this.listView1.SelectedItems[0].SubItems[3].Text = Convert.ToDecimal(this.numericUpDown_enter_quantity.Value).ToString();
                recalculate_all();
                calculation_of_the_sum_of_the_document();
                this.numericUpDown_enter_quantity.Visible = false;
                this.panel1.Visible = false;
                this.listView1.Select();
                this.listView1.Items[this.listView1.SelectedIndices[0]].Selected = true;
                write_new_document("0", "0", "0", "0", false, "0", "0", "0", "0");
            }

            SendDataToCustomerScreen(1, 0, 1);
        }
              

        private void TxtB_total_sum_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        
        private void TxtB_total_sum_MouseHover(object sender, EventArgs e)
        {
            //ToolTip t = new ToolTip();            

            //// Установка задержек для ToolTip
            ////toolTip.AutoPopDelay = 50000;
            ////toolTip.InitialDelay = 1000;
            ////toolTip.ReshowDelay = 500;

            ////// Принудительное отображение текста ToolTip независимо от активности формы
            ////toolTip.ShowAlways = true;
            //toolTip.Show("Без промо / с учетом промо", this.txtB_total_sum);

        }

        //private void Cash_check_Paint(object sender, PaintEventArgs e)
        //{
        //    txtB_total_sum.Text = "Сумма: "+calculation_of_the_sum_of_the_document().ToString("F2");
        //}

        public void insert_incident_record(string tovar, string quantity, string type_of_operation,string reason)
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "INSERT INTO deleted_items(" +
                    "num_doc," +
                    "num_cash," +
                    "date_time_start," +
                    "date_time_action," +
                    "tovar," +
                    "quantity," +
                    "type_of_operation," +
                    "guid," +
                    "autor,"+
                    "reason)	VALUES(" +
                    numdoc.ToString() + "," +
                    num_cash.Tag.ToString() + ",'" +
                     date_time_start.Text.Replace("Чек", "").Trim() + "','" +
                    DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + "'," +
                    tovar.ToString() + "," +
                    quantity.ToString() + "," +
                    type_of_operation + ",'"+
                    guid+"','" +
                    MainStaticClass.CashOperatorInn+"','"+
                    reason+"');";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                command.Dispose();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" insert_incident_record" +ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(" insert_incident_record "+ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        /// <summary>
        /// Удаляет строку из чека
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView1_KeyDown(object sender, KeyEventArgs e)
        {

            if (!itsnew)
            {
                return;
            }

            if ((e.KeyData == Keys.Delete) && (check_type.SelectedIndex == 0))
            {
                if ((listView1.Items.Count > 1) && (listView1.SelectedItems.Count > 0))
                {
                    //MessageBox.Show("0 - " + Convert.ToInt16(listView1.SelectedItems[0].SubItems[0].Text.Trim()).ToString());
                    //MessageBox.Show("3 - " + Convert.ToInt16(listView1.SelectedItems[0].SubItems[3].Text.Trim()).ToString());
                    //MessageBox.Show("1-" + listView1.SelectedItems[0].SubItems[1].Text);
                    //MessageBox.Show("2-" + listView1.SelectedItems[0].SubItems[2].Text);
                    //MessageBox.Show("3-" + listView1.SelectedItems[0].SubItems[3].Text);

                    ///////////////////////////////////////////////////////////////
                    if (MainStaticClass.Code_right_of_user != 1)
                    {
                        //if (MainStaticClass.SelfServiceKiosk == 0)
                        //{
                            enable_delete = false;
                            Interface_switching isw = new Interface_switching();
                            isw.caller_type = 3;
                            isw.cc = this;
                            isw.not_change_Cash_Operator = true;
                            isw.ShowDialog();
                            isw.Dispose();
                        //}
                        //else
                        //{
                        //    enable_delete = true;
                        //}

                        if (!enable_delete)
                        {
                            MessageBox.Show("Вам запрещено удалять строки");
                            return;
                        }
                        else
                        {
                            ReasonsDeletionCheck reasons = new ReasonsDeletionCheck();
                            DialogResult dialogResult = reasons.ShowDialog();
                            if (dialogResult == DialogResult.OK)
                            {

                                insert_incident_record(listView1.SelectedItems[0].SubItems[0].Text, listView1.SelectedItems[0].SubItems[3].Text, "0",reasons.reason);
                                bool reloadKM = false;
                                if (listView1.SelectedItems[0].SubItems[14].Text.Trim().Length > 13)
                                {
                                    reloadKM = true;
                                }
                                listView1.Items.Remove(listView1.SelectedItems[0]);
                                if (reloadKM)
                                {
                                    reload_km_buffer();
                                }
                                calculation_of_the_sum_of_the_document();
                                write_new_document("0", calculation_of_the_sum_of_the_document().ToString().Replace(",", "."), "0", "0", false, "0", "0", "0", "0");
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        ReasonsDeletionCheck reasons = new ReasonsDeletionCheck();
                        DialogResult dialogResult = reasons.ShowDialog();
                        if (dialogResult == DialogResult.OK)
                        {
                            insert_incident_record(listView1.SelectedItems[0].SubItems[0].Text, listView1.SelectedItems[0].SubItems[3].Text, "0",reasons.reason);
                            bool reloadKM = false;
                            if (listView1.SelectedItems[0].SubItems[14].Text.Trim().Length > 13)
                            {
                                reloadKM = true;
                            }
                            listView1.Items.Remove(listView1.SelectedItems[0]);
                            if (reloadKM)
                            {
                                reload_km_buffer();
                            }
                            calculation_of_the_sum_of_the_document();
                            write_new_document("0", calculation_of_the_sum_of_the_document().ToString().Replace(",", "."), "0", "0", false, "0", "0", "0", "0");                         
                        }
                    }
                }
                else if (listView1.Items.Count == 1)
                {
                    MessageBox.Show("Единственную строку удалить нельзя, можно только удалить документ целиком");
                }
            }
        }

        private void Comment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 39)
            {
                e.Handled = true;
            }
        }

        private void TxtB_inn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        private void checkBox_to_print_repeatedly_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.checkBox_to_print_repeatedly.CheckState == CheckState.Checked)
            {
                to_print_certainly = 1;
            }
            else
            {
                to_print_certainly = 0;
            }

        }
        
        public class Buyer_UID
        {
            public string uid { get; set; }
        }
                          
        //private void create_virtual_card()
        //{
        //    CreateVirtualCard createVirtualCard = new CreateVirtualCard();
        //    createVirtualCard.txtBox_phone.Text = "7" + txtB_client_phone.Text.Trim();
        //    createVirtualCard.cash_Check = this;
        //    DialogResult dr = createVirtualCard.ShowDialog();
        //    if (dr == DialogResult.OK)
        //    {
        //        txtB_client_phone.Enabled = false;
        //        client_barcode.Enabled = false;
        //    }
        //}
               

        private void find_client_on_num_phone(string phone_number)
        {

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                //string query = "SELECT code FROM clients where phone LIKE'%" + txtB_client_phone.Text.Trim() + "%'";
                //string query = "SELECT MIN(to_number(code)),code,its_work,COALESCE(bonus_is_on,0) as bonus_is_on FROM clients where right(phone,10)='" + txtB_client_phone.Text.Trim() + "'";
                //string query = "SELECT MIN(CAST(code as numeric)),code,MAX(its_work) AS its_work,COALESCE(bonus_is_on, 0) as bonus_is_on FROM clients where right(phone,10)= '" + phone_number + "' " +
                //    " group by code,its_work,COALESCE(bonus_is_on, 0) order by MAX(its_work) DESC ,MIN(CAST(code as numeric)) limit 1";
                string query = "SELECT MIN(CAST(code as numeric)) AS number_card,code, MAX(its_work) AS its_work," +
                    "CASE WHEN left(code,10) = right(phone, 10) THEN 1 else 0 END AS virtual," +
                    " COALESCE(bonus_is_on, 0) as bonus_is_on FROM clients " +
                    " where right(phone,10)= '" + phone_number + "' group by code,virtual,bonus_is_on" +
                    //" where phone= '" + phone_number + "' group by code,virtual,bonus_is_on" +
                    " order by its_work DESC,virtual DESC,number_card limit 1";

                //string query = "SELECT code,MAX(its_work) AS its_work,COALESCE(bonus_is_on, 0) as bonus_is_on FROM clients where right(phone,10)= '" + phone_number + "' " +
                //    " group by code,COALESCE(bonus_is_on, 0) order by MAX(its_work) DESC  limit 1";

                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                string code_client = ""; int its_work = 0; //int bonus_is_on = 0;
                MainStaticClass.write_event_in_log(" Поиск клиента по номеру телефона старт ", " Документ ", numdoc.ToString());
                while (reader.Read())
                {
                    code_client = reader["code"].ToString();
                    its_work = Convert.ToInt16(reader["its_work"]);
                    //bonus_is_on = Convert.ToInt16(reader["bonus_is_on"]);
                }
                reader.Close();
                command.Dispose();
                MainStaticClass.write_event_in_log(" Поиск клиента по номеру телефона окончание ", " Документ ", numdoc.ToString());

                if (code_client == "")
                {
                    MainStaticClass.write_event_in_log(" По данному телефонному номеру клиент не найден "+ phone_number, " Документ ", numdoc.ToString());
                    MessageBox.Show(" По данному телефонному номеру клиент не найден ");
                }
                else
                {
                    if (its_work != 1)
                    {
                        MainStaticClass.write_event_in_log(" Эта карточка заблокирована " + phone_number, " Документ ", numdoc.ToString());
                        MessageBox.Show("Эта карточка заблокирована");
                        return;
                    }
                    else
                    {
                        //if (bonus_is_on == 1)
                        //{
                        //    if (MainStaticClass.PassPromo == "")//Бонусная программа выключена
                        //    {
                        //        process_client_discount(code_client);
                        //    }                            
                        //}
                        //else
                        //{
                            process_client_discount(code_client);
                        //}
                    }
                }

                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" find_client_on_num_phone" +ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("find_client_on_num_phone "+ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        /// <summary>
        /// При вводе номера телефона
        /// сначала происходит проверка 
        /// наличия клиента с таким номером телефона
        /// если клиент не найден он создается и работа 
        /// происходит с новой виртуальной картой 
        /// равной номеру телефона, если клиент 
        /// найден тогда действия с ним
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void check_and_verify_phone_number(string phone_number)
        {
            MainStaticClass.write_event_in_log(" Проверка наличия телефона старт ", " Документ ", numdoc.ToString());
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();                
                string query = "SELECT code,name FROM clients where phone='" + phone_number + "'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                bool client_exist = false;
                while (reader.Read())
                {
                    client_exist = true;
                    MainStaticClass.write_event_in_log(" Присвоение значения реквизиту на форме ", " Документ ", numdoc.ToString());
                    client.Tag = reader["code"].ToString();
                    client.Text = reader["name"].ToString();
                    Discount = Convert.ToDouble(0.05);
                    txtB_client_phone.Enabled = false;
                    client_barcode.Enabled = false;

                }
                if (!client_exist)
                {
                    MainStaticClass.write_event_in_log(" Проверка наличия телефона это новый телефон  ", " Документ ", numdoc.ToString());
                    query = "DELETE FROM temp_phone_clients WHERE phone='" + phone_number + "'";
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                    query = "INSERT INTO temp_phone_clients(barcode, phone)VALUES ('" + phone_number + "','" + phone_number + "')";
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                    conn.Close();
                    if (client.Tag == null)
                    {
                        MainStaticClass.write_event_in_log(" Присвоение значения реквизиту на форме ", " Документ ", numdoc.ToString());
                        client.Tag = phone_number;
                        client.Text = phone_number;
                        Discount = Convert.ToDouble(0.05);
                        txtB_client_phone.Enabled = false;
                        client_barcode.Enabled = false;
                    }
                }
                //Пересчет ТЧ в любом случае
                MainStaticClass.write_event_in_log(" Начало пересчета ТЧ " + phone_number, " Документ ", numdoc.ToString());
                foreach (ListViewItem lvi in listView1.Items)
                {
                    calculate_on_string(lvi);
                }
                MainStaticClass.write_event_in_log(" Окончание пересчета ТЧ " + phone_number, " Документ ", numdoc.ToString());
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
        }



        /// <summary>
        /// Поиск клиента по телефону
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtB_client_phone_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (MainStaticClass.PassPromo != "")//Бонусная программа выключена
            //{
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }
            //}

            if (e.KeyChar == (char)Keys.Enter)
            {
                if (txtB_client_phone.Text.ToString().Trim().Length > 0)
                {
                    if (txtB_client_phone.Text.ToString().Trim().Substring(0, 1) != "9")
                    {
                        MessageBox.Show("Номер телефона должен начинаться с цифры 9 ");
                        return;
                    }
                }

                if (txtB_client_phone.Text.ToString().Trim().Length != 10)
                {
                    MessageBox.Show("Номер телефона должен содержать 10 цифр");
                    return;
                }

                //if (DateTime.Now < new DateTime(2022, 08, 01))//Начиная с первого августа 2022 года алгоритм однаковый для чд и визы
                //{
                if ((MainStaticClass.GetWorkSchema == 1) || (MainStaticClass.GetWorkSchema == 3))//Это ЧД и новая схева Визы
                {
                    check_and_verify_phone_number(txtB_client_phone.Text.ToString().Trim());
                }
                //else if (MainStaticClass.GetWorkSchema == 2) //Это Ева
                //{
                //    get_client_in_processing();
                //}
                //}
                //else
                //{
                //    check_and_verify_phone_number(txtB_client_phone.Text.ToString().Trim());
                //}
            }
        }
    



        void return_kop_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        void return_rouble_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        private void return_quantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }

        }

        private int get_its_deleted_document()
        {
            int result = 0;

            NpgsqlConnection conn = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT checks_header.its_deleted FROM  checks_header where checks_header.date_time_write='"
                    + date_time_write + "'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToInt16(command.ExecuteScalar());
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибки при получении признака удаленности документа " + ex.Message);
                result = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибки при получении признака удаленности документа " + ex.Message);
                result = 1;
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

        //private string get_user_name()
        //{
        //    string result = "";
        //    NpgsqlConnection conn = null;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        string query = "SELECT name FROM public.users where code="+MainStaticClass.Cash_Operator;

        //    }
        //    catch (NpgsqlException ex)
        //    {

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {

        //    }


        //        return result;
        //}

        private void to_open_the_written_down_document()
        {
            //itsnew = false;
            NpgsqlConnection conn = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT checks_header.client, checks_header.cash_desk_number, checks_header.comment, checks_header.cash, " +
                               " checks_header.remainder,checks_header.date_time_start,checks_header.discount,clients.name AS clients_name ,users.name AS users_name  " +
                               ",tovar.name AS tovar_name ,checks_table.tovar_code, checks_table.quantity,checks_table.price, checks_table.price_at_a_discount,checks_table.sum, " +
                               " checks_table.sum_at_a_discount,checks_table.action_num_doc,checks_table.action_num_doc1,checks_table.action_num_doc2," +
                               " checks_header.check_type " +
                               " ,characteristic.name AS characteristic_name,checks_header.document_number,checks_header.autor,characteristic.guid,clients.code AS clients_code ," +
                               //" checks_header.sertificate_money,checks_header.non_cash_money,checks_header.cash_money,checks_header.sales_assistant,checks_header.bonuses_it_is_counted, " +
                               " checks_header.sertificate_money,checks_header.non_cash_money,checks_header.cash_money,checks_header.bonuses_it_is_counted, " +
                               " checks_header.bonuses_it_is_written_off, " +
                               " checks_table.bonus_standard,checks_table.bonus_promotion,checks_table.promotion_b_mover,checks_table.item_marker,checks_header.requisite," +
                               "checks_header.its_deleted,checks_header.system_taxation,checks_header.guid AS checks_header_guid,checks_header.guid1 AS checks_header_guid " +
                               " FROM checks_header left join checks_table ON checks_header.document_number=checks_table.document_number " +
                               " left join clients ON checks_header.client  = clients.code " +
                               " left join tovar ON checks_table.tovar_code = tovar.code " +
                               " left join users ON  checks_header.autor = users.code " +
                               " left join characteristic ON  checks_table.characteristic = characteristic.guid " +
                               " where checks_header.date_time_write='" + date_time_write + "' order by checks_table.numstr;";
                //string query = "SELECT checks_header.client, checks_header.cash_desk_number, checks_header.comment, checks_header.cash, " +
                //              " checks_header.remainder,checks_header.date_time_start,checks_header.discount,clients.name AS clients_name ,users.name AS users_name  " +
                //              ",tovar.name AS tovar_name ,checks_table.tovar_code, checks_table.quantity,checks_table.price, checks_table.price_at_a_discount,checks_table.sum, " +
                //              " checks_table.sum_at_a_discount,checks_table.action_num_doc,checks_table.action_num_doc1,checks_table.action_num_doc2," +
                //              " checks_header.check_type " +
                //              " ,characteristic.name AS characteristic_name,checks_header.document_number,checks_header.autor,characteristic.guid,clients.code AS clients_code ," +
                //              //" checks_header.sertificate_money,checks_header.non_cash_money,checks_header.cash_money,checks_header.sales_assistant,checks_header.bonuses_it_is_counted, " +
                //              " checks_header.sertificate_money,checks_header.non_cash_money,checks_header.cash_money,checks_header.bonuses_it_is_counted, " +
                //              " checks_header.bonuses_it_is_written_off, " +
                //              " checks_table.bonus_standard,checks_table.bonus_promotion,checks_table.promotion_b_mover,checks_table.item_marker,checks_header.requisite," +
                //              "checks_header.its_deleted,checks_header.system_taxation,checks_header.guid AS checks_header_guid,checks_header.guid1 AS checks_header_guid " +
                //              " FROM checks_header left join checks_table ON checks_header.document_number=checks_table.document_number " +
                //              " left join clients ON checks_header.client  = clients.code " +
                //              " left join tovar ON checks_table.tovar_code = tovar.code " +
                //              " left join users ON  checks_header.autor = users.code " +
                //              " left join characteristic ON  checks_table.characteristic = characteristic.guid " +
                //              " where checks_header.date_time_write='" + date_time_write + "' order by checks_table.numstr;";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                bool header_fill = false;
                while (reader.Read())
                {
                    if (!header_fill)//заполнить шапку
                    {
                        this.guid = reader["checks_header_guid"].ToString();
                        this.guid1 = reader["checks_header_guid"].ToString();
                        this.client.Tag = reader["client"].ToString();//Комментарий
                        this.comment.Text = reader["comment"].ToString();//Комментарий
                        this.p_sum_doc = Convert.ToDecimal(reader["cash"]).ToString();//Сумма документа                    
                        this.p_remainder = Convert.ToDecimal(reader["remainder"]).ToString();//Сдача                        
                        this.date_time_start.Text = Convert.ToDateTime(reader["date_time_start"]).ToString("yyy-MM-dd HH:mm:ss");//Время создания документа
                        this.p_sum_pay = reader["cash_money"].ToString();// (reader.GetDecimal(3) + reader.GetDecimal(4)).ToString();//Сумма наличных
                        this.p_discount = reader["discount"].ToString();// reader.GetDecimal(6).ToString();//Скидка по документу                                                                         
                        this.client.Text = reader["clients_name"].ToString();//.GetString(7);//Наименование клиента если была скидка
                        if (this.client.Text != "")
                        {
                            this.Discount = 0.05;
                        }                                                                             //}
                        this.user.Text = reader["users_name"].ToString();//.GetString(8);
                        //this.user.Text = MainStaticClass.Cash_Operator; //reader["users_name"].ToString();//.GetString(8);
                        this.pay.Text = "Напечатать F8";
                        if (Convert.ToInt32(reader["system_taxation"]) == 3)
                        {
                            checkBox_to_print_repeatedly_p.Visible = true;
                        }

                        header_fill = true;                        
                        this.check_type.SelectedIndex = Convert.ToInt16(reader["check_type"]);//.GetInt16(19);
                        //this.type_pay.SelectedIndex = reader.GetInt16(22);
                        this.numdoc = Convert.ToInt64(reader["document_number"]);
                        this.txtB_num_doc.Text = this.numdoc.ToString();
                        this.user.Tag = reader["autor"].ToString();
                        //this.user.Tag = MainStaticClass.Cash_Operator_Client_Code; reader["autor"].ToString();

                        this.client_barcode.Tag = reader["clients_code"].ToString();
                        this.txtB_sertificate_money.Text = reader["sertificate_money"].ToString();
                        this.txtB_non_cash_money.Text = reader["non_cash_money"].ToString();
                        this.txtB_cash_money.Text = reader["cash_money"].ToString();
                        //this.txtB_sales_assistant.Text = reader["sales_assistant"].ToString();
                        //this.bonus_on_document = Convert.ToDecimal(reader["bonuses_it_is_counted"]);                       
                        this.bonuses_it_is_written_off = Convert.ToDecimal(reader["bonuses_it_is_written_off"]);
                        this.txtB_bonus_money.Text = this.bonuses_it_is_written_off.ToString();

                        //if (reader["requisite"].ToString() == "1")
                        //{
                        //    this.checkBox_club.Checked = true;
                        //}

                        if (Convert.ToInt16(reader["its_deleted"]) == 1)
                        {
                            pay.Enabled = false;
                            checkBox_to_print_repeatedly.Enabled = false;
                        }                        
                    }

                    //ListViewItem lvi = new ListViewItem(reader.GetString(9).Trim());
                    if (Convert.ToDecimal(reader["price_at_a_discount"]) >= 0)
                    {
                        ListViewItem lvi = new ListViewItem(reader["tovar_code"].ToString().Trim());
                        lvi.Tag = reader["tovar_code"].ToString();// reader.GetInt32(10).ToString();
                        lvi.SubItems.Add(reader["tovar_name"].ToString().Trim());//.GetString(9));//Наименование
                        lvi.SubItems.Add(reader["characteristic_name"].ToString());//Характеристика
                        lvi.SubItems[2].Tag = reader["guid"].ToString();//guid Характеристики                    
                        lvi.SubItems.Add(reader["quantity"].ToString());//.GetInt32(11).ToString());//Количество
                        lvi.SubItems.Add(reader["price"].ToString());//.GetDecimal(12).ToString());//Цена без скидки
                        lvi.SubItems.Add(reader["price_at_a_discount"].ToString());//.GetDecimal(13).ToString());//Цена Со скидкой
                        lvi.SubItems.Add(reader["sum"].ToString());//.GetDecimal(14).ToString());//Сумма без скидки
                        lvi.SubItems.Add(reader["sum_at_a_discount"].ToString());//.GetDecimal(15).ToString());//Сумма со скидкой
                        lvi.SubItems.Add(reader["action_num_doc"].ToString());//.GetInt32(16).ToString());//Акционный документ
                        lvi.SubItems.Add(reader["action_num_doc1"].ToString());//.GetInt32(17).ToString());//Акционный документ
                        lvi.SubItems.Add(reader["action_num_doc2"].ToString());//.GetInt32(18).ToString());//Акционный документ                       
                        lvi.SubItems.Add(((int)(Convert.ToDecimal(reader["bonus_standard"]))).ToString());//bonus_standard                        
                        lvi.SubItems.Add(((int)(Convert.ToDecimal(reader["bonus_promotion"]))).ToString());//bonus_standard
                        lvi.SubItems.Add(reader["promotion_b_mover"].ToString());//promotion_b_mover
                        lvi.SubItems.Add(reader["item_marker"].ToString().Replace("vasya2021", "'"));//item_marker
                        listView1.Items.Add(lvi);
                    }
                    else
                    {
                        //ListViewItem lvi = new ListViewItem(reader.GetInt32(10).ToString());
                        //lvi.Tag = reader.GetInt32(10).ToString();
                        //lvi.SubItems.Add(reader.GetString(9));//Наименование
                        ListViewItem lvi = new ListViewItem(reader["tovar_code"].ToString());
                        lvi.Tag = reader["tovar_code"].ToString();// reader.GetInt32(10).ToString();
                        lvi.SubItems.Add(reader["tovar_name"].ToString());//.GetString(9));//Наименование
                        lvi.SubItems.Add((Convert.ToDecimal(reader["price_at_a_discount"]) * -1).ToString());//Сумма со скидкой
                        listView_sertificates.Items.Add(lvi);
                    }
                }
                //Если не администратор то печатать нельзя в любом случае
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("to_open_the_written_down_document "+ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    // conn.Dispose();
                }
            }
            //this.client_barcode.Enabled = false;
            //this.inventory.Enabled = false;
            //this.inputbarcode.Enabled = false;
            this.listView1.Focus();
            //this.choice_of_currencies.Enabled = false;
            //this.listView1.Enabled = false;
            this.Update();
            //if (choice_of_currencies.CheckState == CheckState.Checked)
            //{
            //    //Сумма в базе хранится в гривнах, если это рубли то надо умножить на курс
            //    this.p_sum_doc   = calculation_of_the_sum_of_the_document().ToString();
            //    decimal rate = MainStaticClass.get_rate();
            //    this.p_remainder = Math.Round(Convert.ToDecimal(this.p_remainder) * rate, 2).ToString();
            //    this.p_sum_pay = Math.Round(Convert.ToDecimal(this.p_sum_pay) * rate, 2).ToString();
            //}
        }

        public void ProcessPermanentChoice(string barcode)
        {
            this.find_barcode_or_code_in_tovar(barcode);
        }

        //protected override void OnFormClosing(FormClosingEventArgs e)
        //{
        //    if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
        //    {
        //        stop_com_barcode_scaner();
        //        this.timer.Stop();
        //        this.timer = null;
        //        workerThread = null;
        //        rd = null;
        //        GC.Collect();
        //    }
        //}


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (listView1.Items.Count > 0)
            {
                e.Cancel = closing;
            }
            if (!closing)
            {
                //if (itsnew)
                //{
                //   // MessageBox.Show("Закрывается");
                //    if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
                //    {
                //        if (workerThread != null)//При нажатии клавиши ESC уже могло все завершится
                //        {
                //            stop_com_barcode_scaner();
                //            this.timer.Stop();
                //            this.timer = null;
                //            workerThread = null;
                //            rd = null;
                //            GC.Collect();
                //        }
                //    }
                //}
                pay_form.Dispose();
                if (request != null)
                {
                    request.Abort();
                }
            }
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="password"></param>
        ///// <returns></returns>
        //private bool get_enable_delete_this_document(string password)
        //{
        //    bool result = false;
        //    return result;
        //}



        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.F4 && e.Alt)
            {
                e.Handled = true;
            }
            if (itsnew)
            {
                if (e.KeyCode == Keys.F7)
                {
                    if (inpun_client_barcode)
                    {
                        client_barcode.Focus();
                        inpun_client_barcode = false;
                    }
                    else
                    {
                        this.inputbarcode.Focus();
                    }
                }
                else if (e.KeyCode == Keys.F6)
                {
                    if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema==3) || (MainStaticClass.GetWorkSchema == 4))
                    {
                        inpun_client_barcode = true;
                    }
                    else
                    {

                    }
                }
                //if (e.KeyCode == Keys.F1)
                //{
                //    if (client.Tag != null)
                //    {
                //        check_availability_card_sale();
                //        if (code_bonus_card != "")
                //        {
                //            ChangeBonusCard changeBonusCard = new ChangeBonusCard();
                //            changeBonusCard.cash_Check = this;
                //            changeBonusCard.txtB_num_card.Text = code_bonus_card;
                //            DialogResult dialogResult = changeBonusCard.ShowDialog();
                //            if (dialogResult == DialogResult.OK)
                //            {
                //                change_bonus_card = true;
                //            }
                //        }
                //        else
                //        {
                //            MessageBox.Show("В строках чека нет бонусной карты на продажу");
                //        }
                //    }
                //}
                else if (e.KeyCode == Keys.F5)
                {
                    //Если сканер USB ---->> COM или COM окно не нужно
                    //Иначе показать окно ввода для акционного штрихкода
                    if (!MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
                    {
                        show_query_window_barcode(1, 0, 0);
                        //Input_action_barcode ib = new Input_action_barcode();
                        //ib.call_type = 1;
                        //ib.ShowDialog();
                    }
                    else
                    {
                        inpun_action_barcode = true;
                    }
                }

                else if (e.KeyCode == Keys.Escape)
                {
                    if (listView2.Focused)
                    {
                        return;
                    }
                    if (listView1.Items.Count == 0)
                    {
                        if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
                        {
                            stop_com_barcode_scaner();
                            this.timer.Stop();
                            this.timer = null;
                            workerThread = null;
                            rd = null;
                            GC.Collect();
                        }
                        MainStaticClass.write_event_in_log("Закрытие пустого документа", "Документ чек", numdoc.ToString());
                        this.Close();
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    //
                    if (check_type.SelectedIndex == 1)
                    {
                        if (listView1.Focused)//Удаление строки
                        {
                            //if (MainStaticClass.Code_right_of_user != 1)
                            //{
                            //    enable_delete = false;
                            //    Interface_switching isw = new Interface_switching();
                            //    isw.caller_type = 3;
                            //    isw.cc = this;
                            //    isw.not_change_Cash_Operator = true;
                            //    isw.ShowDialog();
                            //    isw.Dispose();

                            //    if (enable_delete)
                            //    {
                            //        listView1.Items.Remove(listView1.Items[listView1.SelectedIndices[0]]);
                            //        write_new_document("0", calculation_of_the_sum_of_the_document().ToString(), "0", "0", false, "0", "0", "0", "0");
                            //    }
                            //}
                            //else
                            //{
                           if (DialogResult.OK == MessageBox.Show("Вы действительно хотите удалить позицию ? ", "", MessageBoxButtons.OKCancel))
                            { 
                                //MainStaticClass.write_event_in_log("Удаление строки документа | " + listView1.Items[listView1.SelectedIndices[0]].SubItems[0].Text, "Документ чек");
                                listView1.Items.Remove(listView1.Items[listView1.SelectedIndices[0]]);
                                write_new_document("0", calculation_of_the_sum_of_the_document().ToString(), "0", "0", false, "0", "0", "0", "0");
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.F9)//Удаление чека
                {
                    if (!itsnew)
                    {
                        return;
                    }
                    if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
                    {
                        if (workerThread != null)//При нажатии клавиши ESC уже могло все завершится
                        {
                            stop_com_barcode_scaner();
                            this.timer.Stop();
                            this.timer = null;
                            workerThread = null;
                            rd = null;
                            GC.Collect();
                        }
                    }

                    if (MainStaticClass.Code_right_of_user != 1)
                    {

                        if (listView1.Items.Count == 0)
                        {
                            //if (MainStaticClass.SelfServiceKiosk == 1)
                            //{
                            //    this.Close();
                            //    return;
                            //}
                            //else
                            //{
                                MessageBox.Show("Нет строк");
                                return;
                            //}
                        }
                        enable_delete = true;
                        //if (MainStaticClass.SelfServiceKiosk == 0)
                        //{
                            enable_delete = false;
                            Interface_switching isw = new Interface_switching();
                            isw.caller_type = 3;
                            isw.cc = this;
                            isw.not_change_Cash_Operator = true;
                            isw.ShowDialog();
                            isw.Dispose();
                        //

                        if (enable_delete)
                        {
                            ReasonsDeletionCheck reasons = new ReasonsDeletionCheck();
                            DialogResult dialogResult = reasons.ShowDialog();
                            if (dialogResult == DialogResult.OK)
                            {
                                this.comment.Text += " >>"+reasons.reason+"<<";
                                calculation_of_the_sum_of_the_document();
                                write_new_document("0", calculation_of_the_sum_of_the_document().ToString().Replace(",", "."), "0", "0", false, "0", "0", "0", "1"); //Это удаляемый документ
                                closing = false;
                                this.Close();
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (listView1.Items.Count == 0)
                        {
                            //if (MainStaticClass.SelfServiceKiosk == 1)
                            //{
                            //    this.Close();
                            //    return;
                            //}
                            //else
                            //{
                                MessageBox.Show("Нет строк");
                                return;
                            //}
                        }
                        ReasonsDeletionCheck reasons = new ReasonsDeletionCheck();
                        DialogResult dialogResult = reasons.ShowDialog();                       
                        if (dialogResult == DialogResult.OK)
                        {
                            this.comment.Text += " >>"+reasons.reason+"<<";
                            write_new_document("0", calculation_of_the_sum_of_the_document().ToString().Replace(",", "."), "0", "0", false, "0", "0", "0", "1"); //Это удаляемый документ
                            closing = false;
                            this.Close();
                            return;
                        }
                    }
                }
                if (e.KeyCode == Keys.F10)//Переключение режима на бонусирование
                {
                    //if (client.Tag == null)
                    //{
                    //    return;
                    //}
                    //if (bonus_is_on_earlier == 1)
                    //{
                    //    return;
                    //}
                    //if (bonus_is_on_now == 0)
                    //{
                    //    bonus_is_on_now = 1;
                    //    client.BackColor = Color.LemonChiffon;
                    //}
                    //else //bonus_is_on==1
                    //{
                    //    bonus_is_on_now = 0;
                    //    client.BackColor = Color.White;
                    //}
                }
            }
            else
            {
                if (e.KeyCode == Keys.Escape)
                {
                    closing = false;
                    this.Close();
                    return;
                }

            }

            if (e.KeyCode == Keys.F8)
            {

                if (listView1.Items.Count == 0)
                {
                    MessageBox.Show(" Нет строк ", " Проверки переда записью документа ");
                    return;
                }
                if (itsnew)
                {

                    //Запишем в файл содержимое табьличной части на случай падения программы
                    //string s = "";
                    //StreamWriter sw = File.CreateText(Application.StartupPath + "/CashCheckTable.txt");
                    //foreach (ListViewItem lvi in listView1.Items)
                    //{
                    //    int count = lvi.SubItems.Count;
                    //    s = "";
                    //    for (int i = 0; i < count; i++)
                    //    {
                    //        s += lvi.SubItems[i].Text.Trim() + "|";
                    //    }
                    //    s += lvi.SubItems[2].Tag;
                    //    sw.WriteLine(s);
                    //}
                    //sw.Close();


                    if (this.listView1.Items.Count == 0)
                    {
                        MessageBox.Show("В документе нет строк, оплачивать нечего");
                        return;
                    }
                    else
                    {
                        if (calculation_of_the_sum_of_the_document() == 0)
                        {
                            MessageBox.Show("Сумма документа нулевая, оплачивать нечего");
                            return;
                        }
                    }
                    //if (check_type.SelectedIndex == 0)
                    //{
                    //show_pay_form();
                    pay_Click(null, null);
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Это возврат, нажмите F9", "Ошибка при возврате товара");
                    //}
                }
                else
                {
                    pay_Click(null, null);

                    //if (DateTime.Now.ToString("yyyy-MM-dd") == this.date_time_write.Substring(0, 10))
                    //{
                    //if (!MainStaticClass.Use_Fiscall_Print)
                    //{
                    //    update_comment();//Комментарий обновляется все время, даже если документ не новый
                    //    text_print(this.p_sum_pay, this.p_sum_doc, this.p_remainder,"0","0","0");
                    //}
                    //}
                    //if (MainStaticClass.Use_Fiscall_Print)
                    //{
                    //    if (!itc_printed())
                    //    {
                    //        if (this.check_type.SelectedIndex == 0)
                    //        {

                    //            fiscall_print_pay(this.p_sum_doc);
                    //        }
                    //        else
                    //        {
                    //            fiscall_print_disburse(txtB_cash_money.Text, txtB_non_cash_money.Text);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    if (MessageBox.Show("Повторная печать этого чека добавит сумму в фискальный принтер", "Повторная печать чека", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    //    {
                    //        if (MainStaticClass.Use_Fiscall_Print)
                    //        {
                    //            fiscall_print_pay(this.p_sum_doc);
                    //            closing = false;
                    //            this.Close();
                    //        }
                    //    }
                    //}
                    closing = false;
                    this.Close();
                }
            }
            //if (e.KeyCode == Keys.F9)//Перевод клиента на бонусную программу
            //{
            //    //if ((itsnew) && (check_type.SelectedIndex == 0))
            //    //{
            //    //    if (txtB_sales_assistant.Text.Trim().Length == 0)
            //    //    {
            //    //        show_query_window_barcode(4, 0, 0);
            //    //    }            
            //    //}
            //    //if (client.Tag != null)
            //    //{
            //    //    btn_change_status_client_Click(null, null);
            //    //}
            //}
        }

        ///// <summary>
        ///// Проверка возможности вернуться к старой схеме
        ///// скидок
        ///// </summary>
        ///// <returns></returns>
        //private bool check_return_discount_system()
        //{
        //    bool result = false;

        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //    try
        //    {
        //        conn.Open();
        //        NpgsqlCommand command = new NpgsqlCommand();
        //        command.Connection = conn;
        //        command.CommandText = "SELECT bonus_is_on  FROM clients where code= '" + client.Tag.ToString() + "'";
        //        object query_result = command.ExecuteScalar();
        //        if (query_result.ToString() != "")
        //        {
        //            result = (Convert.ToInt32(command.ExecuteScalar()) == 1 ? false : true);
        //        }
        //        else
        //        {
        //            result = true;
        //        }
        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show("Ошибка при определении вернутся к старой схеме " + ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Ошибка при определении вернутся к старой схеме " + ex.Message);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return result;
        //}


        private Int64 get_new_number_document()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = conn;
            //            command.CommandText = "SELECT COUNT(*) FROM checks_header";
            command.CommandText = "SELECT nextval('checks_header_document_number_seq'::regclass);";
            Int64 result = Convert.ToInt64(command.ExecuteScalar());
            conn.Close();
            MainStaticClass.write_event_in_log(" Получение номера для нового документа ", "Документ чек", result.ToString());
            return result;
        }

        public void recalculate_all()
        {
            foreach (ListViewItem lvi in listView1.Items)
            {
                calculate_on_string(lvi);
            }
        }

        private string its_certificate(string tovar_code)
        {
            string result = "-1";

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = " SELECT its_certificate FROM tovar WHERE code=" + tovar_code;
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                object result_query = command.ExecuteScalar().ToString();
                if (result_query != null)
                {
                    result = result_query.ToString();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("its_certificate "+ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("its_certificate "+ex.Message);
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


        public void calculate_on_string(ListViewItem lvi)
        {

            //4 цена ,  5 цена со скидкой, 6 сумма, 7 сумма со скидкой            
            if ((lvi.SubItems[8].Text.Trim() == "0") && (lvi.SubItems[9].Text.Trim() == "0"))//Признак того что не участвует в акции и не подарок
            {
                //Проверка на сертификат               
                if (its_certificate(lvi.Tag.ToString()) != "1")
                {
                    if (check_type.SelectedIndex == 0)
                    {
                        lvi.SubItems[5].Text = Math.Round(Convert.ToDouble(lvi.SubItems[4].Text) - Convert.ToDouble(lvi.SubItems[4].Text) * Discount, 2).ToString();//Цена со скидкой            
                    }
                    else if (check_type.SelectedIndex == 1)
                    {
                        lvi.SubItems[7].Text = Math.Round(Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text),2,MidpointRounding.ToEven).ToString();//Это возврат 
                    }
                }
                else
                {
                    lvi.SubItems[5].Text = (Math.Round(Convert.ToDecimal(lvi.SubItems[4].Text), 2)).ToString();//Цена со скидкой для сертификата будет равна его цене, т.е. номиналу            
                }

                lvi.SubItems[6].Text = ((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());
                lvi.SubItems[7].Text = Math.Round((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)),2,MidpointRounding.ToEven).ToString();
            }
            if (lvi.SubItems[8].Text.Trim() != "0")//Это подарок и необходимо проверить цена 0.01 или нет
            {
                if (Convert.ToDouble(lvi.SubItems[4].Text.Trim()) == 0.01)
                {
                    lvi.SubItems[6].Text = ((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());
                    lvi.SubItems[7].Text = Math.Round((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)),2,MidpointRounding.ToEven).ToString();
                }
            }
        }

        public void calculate_on_string_row(DataRow row)
        {

            //4 цена ,  5 цена со скидкой, 6 сумма, 7 сумма со скидкой            
            if ((row["action"].ToString().Trim() == "0") && (row["gift"].ToString().Trim() == "0"))//Признак того что не участвует в акции и не подарок
            {
                //Проверка на сертификат               
                if (its_certificate(row["code"].ToString()) != "1")
                {
                    row["price_at_discount"] = Math.Round(Convert.ToDouble(row["price"]) - Convert.ToDouble(row["price"]) * Discount, 2);//Цена со скидкой            
                }
                else
                {
                    row["price_at_discount"] = Math.Round(Convert.ToDecimal(row["price"]), 2);
                }
                row["sum_full"] = Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"]);
                row["sum_at_discount"] = Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"]);
            }
            if (row["action"].ToString().Trim() != "0")//Это подарок и необходимо проверить цена 0.01 или нет
            {
                if (Convert.ToDouble(row["price"]) == 0.01)
                {
                    row["sum_full"] = Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"]);
                    row["sum_at_discount"] = Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"]);
                }
            }
        }

        private void inputbarcode_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            this.inputbarcode.Text = this.inputbarcode.Text.Replace("\r\n", "");
            if (e.KeyChar == 13)
            {
                if (this.inputbarcode.Text.Length == 0)//||(this.inputbarcode.Text=="\r\n"))//тут еще проверка на минимальность символов
                {
                    //MessageBox.Show("Штрихкод не найден");
                    Tovar_Not_Found t_n_f = new Tovar_Not_Found();
                    t_n_f.ShowDialog();
                    t_n_f.Dispose();
                    return;
                }
                find_barcode_or_code_in_tovar(this.inputbarcode.Text);
                if (listView1.Items.Count > 0)
                {
                    btn_inpute_phone_client.Enabled = false;
                }
                inputbarcode.Text = "";
                //this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                //this.listView1.Items[this.listView1.Items.Count - 1].Focused = true;
                //this.listView1.EnsureVisible(this.listView1.Items.Count - 1);
                return;
            }

            if (!(Char.IsDigit(e.KeyChar)))
            {
                if (e.KeyChar != (char)Keys.Back)
                {

                    e.Handled = true;
                }
            }

        }

        //private decimal calculation_of_the_sum_of_the_document()        
        //{

        //    //Учесть скидку

        //    //decimal rate = 0;
        //    //bool rouble = false;
        //    //if (choice_of_currencies.CheckState == CheckState.Checked)
        //    //{
        //    //    rouble = true;
        //    //    rate = MainStaticClass.get_rate();
        //    //    if (rate == 0)
        //    //    {
        //    //        MessageBox.Show(" Курс не может быть нулевым ");
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    rate = 1;
        //    //}

        //    decimal total = 0;
        //    foreach (ListViewItem lvi in listView1.Items)
        //    {
        //        //if (rouble)
        //        //{
        //        //    total += Math.Round(Convert.ToDecimal(lvi.SubItems[6].Text)*rate,2);
        //        //}
        //        //else
        //        //{
        //            total += Convert.ToDecimal(lvi.SubItems[6].Text);
        //        //}
        //    }


        //    return total;
        //}

        public decimal calculation_of_the_sum_of_the_document()
        {
            decimal total = 0;
            foreach (ListViewItem lvi in listView1.Items)
            {
                total += Convert.ToDecimal(lvi.SubItems[7].Text);
                //MessageBox.Show("Подсчет суммы документа " + lvi.SubItems[7].Text + " общая сумма " + total.ToString()," Подсчет суммы ");
            }

            return total;
        }

        //public decimal calculation_of_the_sum_without_discount_of_the_document()
        //{
        //    decimal total = 0;
        //    foreach (ListViewItem lvi in listView1.Items)
        //    {
        //        total += Convert.ToDecimal(lvi.SubItems[6].Text);
        //        //MessageBox.Show("Подсчет суммы документа " + lvi.SubItems[7].Text + " общая сумма " + total.ToString()," Подсчет суммы ");
        //    }

        //    return total;
        //}

        ///// <summary>
        ///// Нет сдачи в копейках 
        ///// для этого отсекается копеечная часть
        ///// потом она равномерно распределяется
        ///// по товарам
        ///// </summary>
        ///// <param name="sum"></param>
        //public void distribute(decimal sum, decimal total)
        //{

        //    if (sum == 0)
        //    {
        //        return;
        //    }

        //    decimal part = Convert.ToDecimal(sum) / total;//как общая сумма относится к части
        //    decimal part_on_string = 0;

        //    foreach (ListViewItem lvi in listView1.Items)
        //    {
        //        MessageBox.Show("Надо распределить " + part.ToString());
        //        //if ((lvi.SubItems[7].Text.Trim() == "0") && (lvi.SubItems[8].Text.Trim() == "0") && (lvi.SubItems[9].Text == "0"))
        //        if (Convert.ToDecimal(lvi.SubItems[4].Text.Replace(".", ",")) > 1)
        //        //if (decimal.Parse(lvi.SubItems[3].Text.Replace(".",",")) > 1)
        //        {
        //            if (its_certificate(lvi.SubItems[0].Text) == "1")//Сертификаты с копейками не работают
        //            {
        //                continue;
        //            }

        //            MessageBox.Show(" Умножаем " + lvi.SubItems[7].Text + " на " + part.ToString());
        //            MessageBox.Show(" Надо распределить по строке без округления " + (Convert.ToDouble(lvi.SubItems[7].Text) * Convert.ToDouble(part)).ToString());
        //            part_on_string = Math.Round(Convert.ToDecimal(lvi.SubItems[7].Text) * part, 2);
        //            MessageBox.Show("Надо распределить по строке " + part_on_string.ToString());


        //            if ((part_on_string > sum) || (sum == part_on_string + (Convert.ToDecimal(1) / 100)))
        //            {
        //                part_on_string = sum;
        //            }

        //            MessageBox.Show("Распределяемое по строке " + part_on_string.ToString());

        //            if ((Convert.ToDecimal(lvi.SubItems[7].Text) > part_on_string) && (Convert.ToDecimal(lvi.SubItems[7].Text) > 1))
        //            {
        //                //получаем новую цену соскидкой
        //                lvi.SubItems[5].Text = (Math.Round((Convert.ToDecimal(lvi.SubItems[7].Text) - part_on_string) / Convert.ToDecimal(Convert.ToDecimal(lvi.SubItems[3].Text)), 2)).ToString();
        //                //Запоминаем старую сумму со скидкой
        //                decimal sum_on_string = Convert.ToDecimal(lvi.SubItems[7].Text);
        //                MessageBox.Show(" Старая сумма со скидкой " + sum_on_string.ToString());
        //                //получаем новую сумму со скидкой
        //                lvi.SubItems[7].Text = (Convert.ToDecimal(lvi.SubItems[5].Text) * Convert.ToDecimal(lvi.SubItems[3].Text)).ToString();
        //                MessageBox.Show(" Новая сумма со скидкой " + lvi.SubItems[7].Text.ToString());
        //                //Получаем разницу между старой суммойц со скидкой и новой
        //                decimal difference = sum_on_string - Convert.ToDecimal(lvi.SubItems[7].Text);
        //                MessageBox.Show(" Остаток " + difference.ToString());

        //                //lvi.SubItems[6].Text = (Convert.ToDecimal(lvi.SubItems[6].Text) - part_on_string).ToString();
        //                if (difference == part_on_string)
        //                {
        //                    sum -= part_on_string;
        //                }
        //                else
        //                {
        //                    sum -= difference;
        //                }
        //            }
        //            else
        //            {
        //                //sum = sum - part_on_string;//вычитаем из общей суммы распределения , то что распределилось по строке
        //                //if (Convert.ToDecimal(lvi.SubItems[6].Text) > part_on_string)//если в строке сумма больше распределения то вычитаем ее
        //                //{
        //                //    lvi.SubItems[6].Text = (Convert.ToDecimal(lvi.SubItems[6].Text) - part_on_string).ToString();
        //                //    part_on_string = 0;
        //                //}
        //                //else
        //                //{
        //                //    //lvi.SubItems[6].Text = (Convert.ToDecimal(1) / 100).ToString();
        //                //    part_on_string = part_on_string + (Convert.ToDecimal(1) / 100) - Convert.ToDecimal(lvi.SubItems[6].Text);
        //                //    sum += part_on_string;
        //                //}
        //            }
        //        }
        //        if (sum == 0)//уже все распределили
        //        {
        //            break;
        //        }
        //    }

        //    if (sum != 0)//Снова цикл
        //    {

        //        //if (sum < 0)
        //        //{
        //        //    sum = sum * -1;
        //        //}

        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            //                 if //(Convert.ToDecimal(lvi.SubItems[6].Text) > Convert.ToDecimal(1) / 100)
        //            //                   (Convert.ToDecimal(lvi.SubItems[3].Text)>1)
        //            //if (Convert.ToDecimal(lvi.SubItems[4].Text.Replace(".", ",")) > 1)
        //            if (Convert.ToDecimal(lvi.SubItems[7].Text.Replace(".", ",")) > 1)
        //            {
        //                if (Convert.ToDecimal(lvi.SubItems[3].Text) == 1)
        //                {
        //                    lvi.SubItems[5].Text = (Convert.ToDecimal(lvi.SubItems[7].Text) - sum).ToString();
        //                    lvi.SubItems[6].Text = (Convert.ToDecimal(lvi.SubItems[4].Text) * Convert.ToDecimal(lvi.SubItems[3].Text)).ToString();
        //                    lvi.SubItems[7].Text = (Convert.ToDecimal(lvi.SubItems[5].Text) * Convert.ToDecimal(lvi.SubItems[3].Text)).ToString();
        //                    sum = 0;
        //                }
        //                else
        //                {
        //                    lvi.SubItems[3].Text = (Convert.ToDecimal(lvi.SubItems[3].Text) - 1).ToString();
        //                    lvi.SubItems[6].Text = (Convert.ToDecimal(lvi.SubItems[4].Text) * Convert.ToDecimal(lvi.SubItems[3].Text)).ToString();
        //                    lvi.SubItems[7].Text = (Convert.ToDecimal(lvi.SubItems[5].Text) * Convert.ToDecimal(lvi.SubItems[3].Text)).ToString();
        //                    //уменьшаем кол-во на единицу , пересчитываем сумму 

        //                    //затем добавляем новый элемент
        //                    ListViewItem lvi2 = new ListViewItem(lvi.SubItems[0].Text);
        //                    lvi2.Tag = lvi.SubItems[0].Text;
        //                    lvi2.SubItems.Add(lvi.SubItems[1].Text);
        //                    lvi2.SubItems.Add(lvi.SubItems[2].Text);
        //                    //lvi2.SubItems.Add(lvi.SubItems[3].Text);
        //                    lvi2.SubItems[2].Tag = lvi.SubItems[2].Tag;
        //                    lvi2.SubItems.Add("1");
        //                    lvi2.SubItems.Add(lvi.SubItems[4].Text);
        //                    lvi2.SubItems.Add((Convert.ToDecimal(lvi.SubItems[5].Text) - sum).ToString());
        //                    lvi2.SubItems.Add((Convert.ToDecimal(lvi2.SubItems[4].Text) * Convert.ToDecimal(lvi2.SubItems[3].Text)).ToString());
        //                    lvi2.SubItems.Add((Convert.ToDecimal(lvi2.SubItems[5].Text) * Convert.ToDecimal(lvi2.SubItems[3].Text)).ToString());
        //                    lvi2.SubItems.Add(lvi.SubItems[8].Text);
        //                    lvi2.SubItems.Add(lvi.SubItems[9].Text);
        //                    lvi2.SubItems.Add(lvi.SubItems[10].Text);
        //                    //*****************************************************************************
        //                    lvi2.SubItems.Add("0");
        //                    lvi2.SubItems.Add("0");
        //                    lvi2.SubItems.Add("0");
        //                    lvi2.SubItems.Add("0");
        //                    //*****************************************************************************
        //                    listView1.Items.Add(lvi2);

        //                    sum = 0;
        //                }
        //            }
        //            //else
        //            //{

        //            //}
        //            if (sum == 0)//уже все распределили
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    if (sum != 0)//Снова цикл
        //    {
        //        MessageBox.Show("Не распределилось");
        //    }
        //}

        /// <summary>
        /// Нет сдачи в копейках 
        /// для этого отсекается копеечная часть
        /// потом она равномерно распределяется
        /// по товарам
        /// </summary>
        /// <param name="sum"></param>
        public void distribute(double sum, double total)
        {

            if (sum == 0)
            {
                return;
            }

            double part = sum / total;//как общая сумма относится к части
            double part_on_string = 0;

            foreach (ListViewItem lvi in listView1.Items)
            {
                //MessageBox.Show("Надо распределить " + part.ToString());
                //if ((lvi.SubItems[7].Text.Trim() == "0") && (lvi.SubItems[8].Text.Trim() == "0") && (lvi.SubItems[9].Text == "0"))
                if (Convert.ToDecimal(lvi.SubItems[4].Text.Replace(".", ",")) > 1)
                //if (decimal.Parse(lvi.SubItems[3].Text.Replace(".",",")) > 1)
                {
                    if (its_certificate(lvi.SubItems[0].Text) == "1")//Сертификаты с копейками не работают
                    {
                        continue;
                    }

                    //MessageBox.Show(" Умножаем " + lvi.SubItems[7].Text + " на " + part.ToString());
                    //MessageBox.Show(" Надо распределить по строке без округления " + (Convert.ToDouble(lvi.SubItems[7].Text) * Convert.ToDouble(part)).ToString());
                    part_on_string = Math.Round(Convert.ToDouble(lvi.SubItems[7].Text) * part, 2);
                    //MessageBox.Show("Надо распределить по строке " + part_on_string.ToString());
                    
                    if ((part_on_string > sum) || (sum == part_on_string + (Convert.ToDouble(1) / 100)))
                    {
                        part_on_string = sum;
                    }

                    //MessageBox.Show("Распределяемое по строке " + part_on_string.ToString());

                    if ((Convert.ToDouble(lvi.SubItems[7].Text) > part_on_string) && (Convert.ToDecimal(lvi.SubItems[7].Text) > 1))
                    {
                        //получаем новую цену соскидкой
                        lvi.SubItems[5].Text = (Math.Round((Convert.ToDouble(lvi.SubItems[7].Text) - part_on_string) / Convert.ToDouble(Convert.ToDouble(lvi.SubItems[3].Text)), 2)).ToString();
                        //Запоминаем старую сумму со скидкой
                        double sum_on_string = Convert.ToDouble(lvi.SubItems[7].Text);
                        //MessageBox.Show(" Старая сумма со скидкой " + sum_on_string.ToString());
                        //получаем новую сумму со скидкой
                        lvi.SubItems[7].Text = (Convert.ToDouble(lvi.SubItems[5].Text) * Convert.ToDouble(lvi.SubItems[3].Text)).ToString();
                        //MessageBox.Show(" Новая сумма со скидкой " + lvi.SubItems[7].Text.ToString());
                        //Получаем разницу между старой суммойц со скидкой и новой
                        double difference = Math.Round(sum_on_string - Convert.ToDouble(lvi.SubItems[7].Text),2,MidpointRounding.ToEven);
                        //MessageBox.Show(" Остаток " + difference.ToString());

                        //lvi.SubItems[6].Text = (Convert.ToDecimal(lvi.SubItems[6].Text) - part_on_string).ToString();
                        if (difference == part_on_string)
                        {
                            sum = Math.Round(sum - part_on_string,2,MidpointRounding.ToEven);
                        }
                        else
                        {
                            sum = Math.Round(sum - difference,2,MidpointRounding.ToEven);
                        }
                    }
                    else
                    {
                        //sum = sum - part_on_string;//вычитаем из общей суммы распределения , то что распределилось по строке
                        //if (Convert.ToDecimal(lvi.SubItems[6].Text) > part_on_string)//если в строке сумма больше распределения то вычитаем ее
                        //{
                        //    lvi.SubItems[6].Text = (Convert.ToDecimal(lvi.SubItems[6].Text) - part_on_string).ToString();
                        //    part_on_string = 0;
                        //}
                        //else
                        //{
                        //    //lvi.SubItems[6].Text = (Convert.ToDecimal(1) / 100).ToString();
                        //    part_on_string = part_on_string + (Convert.ToDecimal(1) / 100) - Convert.ToDecimal(lvi.SubItems[6].Text);
                        //    sum += part_on_string;
                        //}
                    }
                }
                if (sum == 0)//уже все распределили
                {
                    break;
                }
            }

            if (sum != 0)//Снова цикл
            {

                //if (sum < 0)
                //{
                //    sum = sum * -1;
                //}

                foreach (ListViewItem lvi in listView1.Items)
                {
                    //                 if //(Convert.ToDecimal(lvi.SubItems[6].Text) > Convert.ToDecimal(1) / 100)
                    //                   (Convert.ToDecimal(lvi.SubItems[3].Text)>1)
                    //if (Convert.ToDecimal(lvi.SubItems[4].Text.Replace(".", ",")) > 1)
                    if (its_certificate(lvi.SubItems[0].Text) == "1")//Сертификаты с копейками не работают
                    {
                        continue;
                    }
                    if (Convert.ToDecimal(lvi.SubItems[7].Text.Replace(".", ",")) > 1)
                    {
                        if (Convert.ToDecimal(lvi.SubItems[3].Text) == 1)
                        {
                            lvi.SubItems[5].Text = (Convert.ToDouble(lvi.SubItems[7].Text) - sum).ToString();
                            lvi.SubItems[6].Text = (Convert.ToDouble(lvi.SubItems[4].Text) * Convert.ToDouble(lvi.SubItems[3].Text)).ToString();
                            lvi.SubItems[7].Text = (Convert.ToDouble(lvi.SubItems[5].Text) * Convert.ToDouble(lvi.SubItems[3].Text)).ToString();
                            sum = 0;
                        }
                        else
                        {
                            lvi.SubItems[3].Text = (Convert.ToDouble(lvi.SubItems[3].Text) - 1).ToString();
                            lvi.SubItems[6].Text = (Convert.ToDouble(lvi.SubItems[4].Text) * Convert.ToDouble(lvi.SubItems[3].Text)).ToString();
                            lvi.SubItems[7].Text = (Convert.ToDouble(lvi.SubItems[5].Text) * Convert.ToDouble(lvi.SubItems[3].Text)).ToString();
                            //уменьшаем кол-во на единицу , пересчитываем сумму 

                            //затем добавляем новый элемент
                            ListViewItem lvi2 = new ListViewItem(lvi.SubItems[0].Text);
                            lvi2.Tag = lvi.SubItems[0].Text;
                            lvi2.SubItems.Add(lvi.SubItems[1].Text);
                            lvi2.SubItems.Add(lvi.SubItems[2].Text);
                            //lvi2.SubItems.Add(lvi.SubItems[3].Text);
                            lvi2.SubItems[2].Tag = lvi.SubItems[2].Tag;
                            lvi2.SubItems.Add("1");
                            lvi2.SubItems.Add(lvi.SubItems[4].Text);
                            lvi2.SubItems.Add((Convert.ToDouble(lvi.SubItems[5].Text) - sum).ToString());
                            lvi2.SubItems.Add((Convert.ToDouble(lvi2.SubItems[4].Text) * Convert.ToDouble(lvi2.SubItems[3].Text)).ToString());
                            lvi2.SubItems.Add((Convert.ToDouble(lvi2.SubItems[5].Text) * Convert.ToDouble(lvi2.SubItems[3].Text)).ToString());
                            lvi2.SubItems.Add(lvi.SubItems[8].Text);
                            lvi2.SubItems.Add(lvi.SubItems[9].Text);
                            lvi2.SubItems.Add(lvi.SubItems[10].Text);
                            //*****************************************************************************
                            lvi2.SubItems.Add("0");
                            lvi2.SubItems.Add("0");
                            lvi2.SubItems.Add("0");
                            lvi2.SubItems.Add("0");
                            //*****************************************************************************
                            listView1.Items.Add(lvi2);

                            sum = 0;
                        }
                    }
                    //else
                    //{

                    //}
                    if (sum == 0)//уже все распределили
                    {
                        break;
                    }
                }
            }
            if (sum != 0)//Снова цикл
            {
                MessageBox.Show("Не распределилось");
            }
        }


        public decimal calculation_of_the_discount_of_the_document()
        {

            //Учесть скидку
            decimal total = 0;
            foreach (ListViewItem lvi in listView1.Items)
            {
                total += Convert.ToDecimal(lvi.SubItems[6].Text) - Convert.ToDecimal(lvi.SubItems[7].Text);
            }

            //if (choice_of_currencies.CheckState == CheckState.Checked)
            //{
            //    decimal rate = MainStaticClass.get_rate();
            //    if (rate == 0)
            //    {
            //        MessageBox.Show(" Курс не может быть нулевым !!! ");
            //    }
            //    total = Math.Round(total * rate, 2);
            //}
            return total;
        }

        private ListViewItem exist_tovar_in_listView(ListView listView, Int64 tovar_code, object characteristic)
        {

            foreach (ListViewItem lvi in listView.Items)
            {
                if (Convert.ToInt64(lvi.Tag) == tovar_code)
                {
                    if ((characteristic == null) && ((lvi.SubItems[2].Tag == null) || (lvi.SubItems[2].Tag.ToString() == "")))
                    {
                        return lvi;
                    }
                    else if ((characteristic == null) && (lvi.SubItems[2].Tag != null))
                    {
                        continue;
                    }
                    else if ((characteristic != null) && (lvi.SubItems[2].Tag == null))
                    {
                        continue;
                    }
                    else if (characteristic.ToString().Trim() != lvi.SubItems[2].Tag.ToString().Trim())
                    {
                        continue;
                    }
                    else if (characteristic.ToString().Trim() == lvi.SubItems[2].Tag.ToString().Trim())
                    {
                        return lvi;
                    }
                }
            }

            return null;
        }


        //private ListViewItem get_tovar_in_listView(ListView listView, Int32 tovar_code, object characteristic)
        //{

        //    foreach (ListViewItem lvi in listView.Items)
        //    {
        //        if (Convert.ToInt32(lvi.Tag) == tovar_code)
        //        {
        //            if ((characteristic == null) && (lvi.SubItems[2].Tag == null))
        //            {
        //                return lvi;
        //            }
        //            else if ((characteristic == null) && (lvi.SubItems[2].Tag != null))
        //            {
        //                continue;
        //            }
        //            else if ((characteristic != null) && (lvi.SubItems[2].Tag == null))
        //            {
        //                continue;
        //            }
        //            else if (characteristic.ToString().Trim() != lvi.SubItems[2].Tag.ToString().Trim())
        //            {
        //                continue;
        //            }
        //            else if (characteristic.ToString().Trim() == lvi.SubItems[2].Tag.ToString().Trim())
        //            {
        //                return lvi;
        //            }
        //        }
        //    }

        //    return null;
        //}

        public double Discount
        {
            get
            {
                return discount;
            }
            set
            {
                discount = value;
            }
        }



        //Это старая схема 
        ///// <summary>
        ///// Возвращает цену подарка
        ///// </summary>
        ///// <param name="num_doc"></param>
        ///// <returns></returns>
        //private string get_price_action(int num_doc)
        //{
        //    string result = "";

        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

        //    try
        //    {

        //        conn.Open();
        //        string query = "SELECT action_header.tip, action_header.code_tovar, action_header.marker,tovar.retail_price  FROM action_header LEFT JOIN tovar ON action_header.code_tovar = tovar.code where num_doc=" + num_doc.ToString()+ " AND tovar.its_deleted=0 ";
        //        NpgsqlCommand command = new NpgsqlCommand(query, conn);                
        //        NpgsqlDataReader reader = command.ExecuteReader();                
        //        while (reader.Read())
        //        {
        //            if ((Convert.ToInt16(reader["tip"]) == 8)||(Convert.ToInt16(reader["tip"]) == 1) || (Convert.ToInt16(reader["tip"]) == 2) || (Convert.ToInt16(reader["tip"]) == 3))
        //            {
        //                if (Convert.ToInt16(reader["marker"]) == 1)//запрашивать подарок
        //                {                            
        //                    result = reader["retail_price"].ToString();//получить розничную цену подарка
        //                }
        //            }
        //        }              
        //        reader.Close();
        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }

        //    }

        //    return result;
        //}



        /// <summary>
        /// Возвращает цену подарка
        /// </summary>
        /// <param name="num_doc"></param>
        /// <returns></returns>
        private string get_price_action(int num_doc)
        {
            string result = "";

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {

                conn.Open();
                string query = "SELECT action_header.tip, action_header.gift_price, action_header.marker  FROM action_header where action_header.num_doc=" + num_doc.ToString();
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if ((Convert.ToInt16(reader["tip"]) == 1) || (Convert.ToInt16(reader["tip"]) == 2) || (Convert.ToInt16(reader["tip"]) == 3) || (Convert.ToInt16(reader["tip"]) == 4) || (Convert.ToInt16(reader["tip"]) == 5) || (Convert.ToInt16(reader["tip"]) == 8))
                    {
                        //if (Convert.ToInt16(reader["marker"]) == 1)//запрашивать подарок
                        //{
                        result = reader["gift_price"].ToString();//получить розничную цену подарка
                        //}
                    }
                }
                reader.Close();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("get_price_action "+ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("get_price_action "+ex.Message);
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
               
        /*Поиск товара по штрихкоду
         * и добвление его в табличную часть
         * это подарочный товар
         * добавляется по нулевой цене
         * barcode это код или штрихкод товара
         * count это количество позиций
         * sum_null если true тогда сумма и сумма со скидкой 0 иначе как обычный товар
         * это для акции 
         */
        public void find_barcode_or_code_in_tovar_action(string barcode, int count, bool sum_null, int num_doc)
        {
            //Повторная проверка если документ не новый или уже вызвано окно оплаты подбор товара не работает
            if (!itsnew)
            {
                return;
            }
            if (!selection_goods)
            {
                return;
            }

            NpgsqlConnection conn = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = conn;
                if (barcode.Length > 6)
                {
                    command.CommandText = "select tovar.code,tovar.name,tovar.retail_price from  barcode left join tovar ON barcode.tovar_code=tovar.code where barcode='" + barcode + "' AND tovar.its_deleted=0 ";
                }
                else
                {
                    command.CommandText = "select tovar.code,tovar.name,tovar.retail_price from  tovar where tovar.code='" + barcode + "' AND tovar.its_deleted=0 ";
                }

                NpgsqlDataReader reader = command.ExecuteReader();
                bool there_are_goods = false;//Флаг для понимания есть ли акционный товар

                while (reader.Read())
                {
                    ListViewItem lvi = new ListViewItem(reader.GetInt64(0).ToString());
                    lvi.Tag = reader.GetInt64(0);
                    lvi.SubItems.Add(reader.GetString(1));//Наименование
                    lvi.SubItems.Add("");//Характеристика
                    lvi.SubItems[2].Tag = "";
                    lvi.SubItems.Add(count.ToString());//Количество
                    string retail_price = get_price_action(num_doc);
                    if (retail_price != "")
                    {
                        //lvi.SubItems.Add(retail_price);//Цена
                        if (reader.GetDecimal(2) == 0)
                        {
                            MessageBox.Show(" У товара с кодом " + barcode + " не заполнена цена, ОБРАТИТЕСЬ К АНАЛИТИКАМ !!! ");
                            //MainStaticClass.write_event_in_log(" У товара с кодом " + barcode + " не заполнена цена,номер документа " + numdoc.ToString(), " Документ чек ", numdoc.ToString());
                            return;
                        }
                        lvi.SubItems.Add(reader.GetDecimal(2).ToString());//Цена
                        lvi.SubItems.Add(retail_price);//Цена соскидкой    
                    }
                    else
                    {
                        lvi.SubItems.Add(reader.GetDecimal(2).ToString());//Цена
                        lvi.SubItems.Add(reader.GetDecimal(2).ToString());//Цена соскидкой    
                    }
                    //if (sum_null)
                    //{
                    //    lvi.SubItems.Add("0");
                    //    lvi.SubItems.Add("0");
                    //}
                    //else
                    //{
                    lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());//Сумма
                    lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString()); //Сумма со скидкой                        
                    //}
                    lvi.SubItems.Add("0");
                    lvi.SubItems.Add(num_doc.ToString());
                    lvi.SubItems.Add(num_doc.ToString());
                    lvi.SubItems.Add("0");
                    lvi.SubItems.Add("0");
                    lvi.SubItems.Add("0");
                    lvi.SubItems.Add("0");
                    listView1.Items.Add(lvi);
                    SendDataToCustomerScreen(1, 0,1);
                    there_are_goods = true;
                    this.listView1.Select();
                    this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                }

                reader.Close();
                conn.Close();
                if (!there_are_goods)
                {
                    MessageBox.Show("ВНИМАНИЕ ПОДАРОК НЕ НАЙДЕН !!! СООБЩИТЕ АДМИНИСТРАТОРУ !!! ", "ОШИБКА ПРИ РАБОТЕ С АКЦИЯМИ");
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("find_barcode_or_code_in_tovar_action "+ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

            }
        }

        /*Обновить запись о последнем добавленном товаре иего цене
         */
        private void update_record_last_tovar(string tovar, string price)
        {
            this.last_tovar.Text = tovar;
            //this.last_cena.Text = price;
        }



        private string get_tovar_code(string barcode)
        {
            string result = "";
            if (barcode.Length <= 6)
            {
                result = barcode;
            }
            else
            {
                NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                try
                {
                    conn.Open();
                    string query = "SELECT tovar_code FROM barcode where barcode='" + barcode + "'";
                    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                    object result_query = command.ExecuteScalar();
                    if (result_query != null)
                    {
                        result = result_query.ToString();
                    }
                    conn.Close();
                }
                catch
                {

                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Проверка по продаваемому сертификату на 
        /// то что он только сегодня поступил в магазин 
        /// в качестве оплаты 
        /// </summary>
        /// <param name="tovar_code"></param>
        /// <returns></returns>
        private bool check_sertificate_for_sales(string barcode)
        {

            bool result = true;
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            NpgsqlParameter parameter = null;
            try
            {
                conn.Open();
                //string query = "SELECT checks_header.guid "+
                //               " FROM public.checks_header "+
                //               " LEFT JOIN public.checks_table ON checks_header.guid = checks_table.guid "+
                //               " WHERE checks_table.item_marker = @barcode AND sum_at_a_discount < 0 "+
                //               " AND date_time_start between @date_start AND @date_finish;";
                string query = " SELECT checks_header.guid,checks_table.item_marker "+
                               " FROM public.checks_header "+
                               " LEFT JOIN public.checks_table ON checks_header.guid = checks_table.guid "+
                               " WHERE checks_table.item_marker = @barcode AND sum_at_a_discount< 0  "+
                               " AND date_time_start between @date_start AND @date_finish;";

                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                parameter = new NpgsqlParameter("@date_start", DateTime.Now.Date.ToString("yyyy-MM-dd"));
                command.Parameters.Add(parameter);
                parameter = new NpgsqlParameter("@date_finish", DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd"));
                command.Parameters.Add(parameter);
                parameter = new NpgsqlParameter("@barcode", barcode);
                command.Parameters.Add(parameter);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    MessageBox.Show(" Вы пытаетесь продать сертификат который был сегодня получен в качестве оплаты на этой кассе. "," Проверка сертификатов ");
                    MainStaticClass.write_event_in_log(" Вы пытаетесь продать сертификат который был сегодня получен в качестве оплаты на этой кассе. ", "Документ", numdoc.ToString());
                    result = false;
                }
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при проверке сертификата" + ex.Message);
                result = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке сертификата" + ex.Message);
                result = false;
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

        /*Поиск товара по штрихкоду
         * и добвление его в табличную часть
         * стандартное добавление товара
         */
        public void find_barcode_or_code_in_tovar(string barcode)
        {
            //Повторная проверка если документ не новый или уже вызвано окно оплаты подбор товара не работает
            if (!itsnew)
            {
                return;
            }

            if (this.check_type.SelectedIndex > 0)
            {
                if (barcode.Trim().Length > 6)
                {
                    MessageBox.Show("Поиск товара прерван ! Длина кода превышает 6 символов ");
                    return;
                }
            }

            if (!selection_goods)
            {
                return;
            }
            //Если кассир не увидел предупреждение предупредим его

            if (this.listView2.Visible)
            {
                this.panel2.BackColor = Color.Red;
                MessageBox.Show("НЕ ЗАВЕРШЕН ПРЕДЫДУЩИЙ ВЫБОР ТОВАРА !!!", "ВНИМАНИЕ ОПЕРАТОР !!!");
                this.panel2.BackColor = Color.ForestGreen;
                listView2.Focus();
                listView2.Items[0].Selected = true;
                return;
            }

            MainStaticClass.write_event_in_log("Попытка добавить новый товар в чек " + barcode, "Документ чек", numdoc.ToString());

            NpgsqlConnection conn = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = conn;
                //select  COUNT(*),tovar.code,tovar.name from  barcode left join tovar ON barcode.tovar_code=tovar.code where barcode='3700074253582' group by tovar.code,tovar.name
                //command.CommandText = "select  tovar.code,tovar.name,tovar.retail_price from  barcode left join tovar ON barcode.tovar_code=tovar.code where barcode='" + inputbarcode.Text + "'";
                //if (inputbarcode.Text.Length > 6)
                if (barcode.Length > 6)
                {
                    //command.CommandText = "select tovar.code,tovar.name,tovar.retail_price from  barcode left join tovar ON barcode.tovar_code=tovar.code where barcode='" + inputbarcode.Text + "' ";
                    command.CommandText = "select tovar.code,tovar.name,tovar.retail_price,characteristic.name,characteristic.guid,characteristic.retail_price_characteristic,tovar.its_certificate,tovar.its_marked,tovar.cdn_check,tovar.fractional " +
                        " from  barcode left join tovar ON barcode.tovar_code=tovar.code " +
                    " left join characteristic ON tovar.code = characteristic.tovar_code " +
                    " where barcode='" + barcode + "' AND its_deleted=0  AND (retail_price<>0 OR characteristic.retail_price_characteristic<>0)";
                    //if (MainStaticClass.Use_Trassir > 0)
                    //{
                    //    string s = MainStaticClass.get_string_message_for_trassir("POSNG_POSITION_ADD_BY_SCANNER", numdoc.ToString(), MainStaticClass.Cash_Operator, DateTime.Now.Date.ToString("MM'/'dd'/'yyyy"), DateTime.Now.ToString("HH:mm:ss"), "", "1", "", barcode, "", MainStaticClass.CashDeskNumber.ToString(), "");
                    //    MainStaticClass.send_data_trassir(s);
                    //}
                    //

                }
                else
                {
                    //command.CommandText = "select tovar.code,tovar.name,tovar.retail_price from  tovar where tovar.code='" + inputbarcode.Text + "'";
                    command.CommandText = "select tovar.code,tovar.name,tovar.retail_price, characteristic.name,characteristic.guid,characteristic.retail_price_characteristic,tovar.its_certificate,tovar.its_marked,tovar.cdn_check,tovar.fractional " +
                        " FROM tovar left join characteristic  ON tovar.code = characteristic.tovar_code where tovar.its_deleted=0 AND tovar.its_certificate=0 AND  (retail_price<>0 OR characteristic.retail_price_characteristic<>0) " +
                        " AND tovar.code='" + barcode + "'";
                    //if (MainStaticClass.Use_Trassir > 0)
                    //{
                    //    string s = MainStaticClass.get_string_message_for_trassir("POSNG_POSITION_ADD_BY_ARTICLE", numdoc.ToString(), MainStaticClass.Cash_Operator, DateTime.Now.Date.ToString("MM'/'dd'/'yyyy"), DateTime.Now.ToString("HH:mm:ss"), "", "1", "", "", barcode, MainStaticClass.CashDeskNumber.ToString(), "");
                    //    MainStaticClass.send_data_trassir(s);
                    //}

                    //command.CommandText = " select tovar.code,tovar.name,tovar.retail_price, characteristic.name,characteristic.guid,characteristic.retail_price_characteristic FROM tovar left join characteristic " +
                    //                      " ON tovar.code = characteristic.tovar_code where tovar.code='" + barcode + "' AND its_deleted=0 AND retail_price<>0 OR characteristic.retail_price_characteristic<>0";

                }

                int its_certificate = 0;
                int its_marked = 0;
                NpgsqlDataReader reader = command.ExecuteReader();
                listView2.Items.Clear();
                bool find_sertificate = false;
                bool cdn_check = false;
                bool fractional = false;
                //string tovar_code = ""; 


                while (reader.Read())
                {
                    cdn_check = Convert.ToBoolean(reader["cdn_check"]);
                    fractional= Convert.ToBoolean(reader["fractional"]);
                    //Сначала добавляем в предварительный список listView2 для того чтобы дать выбор если таких товаров будет несколько
                    //ListViewItem lvi = new ListViewItem(reader[1].ToString());

                    //if (reader[3].ToString().Trim() == "3")
                    //{
                    //    its_bonus_card = true;
                    //}
                    ListViewItem lvi = new ListViewItem(reader[3].ToString().Trim());//Внутренний код товара
                    //lvi.Tag = reader.GetInt32(0);//Внутренний код товара
                    //lvi.SubItems.Add(reader[1].ToString().Trim());//Наименование
                    select_tovar.Text = reader[1].ToString().Trim();
                    select_tovar.Tag = reader.GetInt64(0).ToString();
                    //tovar_code = reader.GetInt64(0).ToString();

                    //lvi.SubItems.Add(reader[3].ToString().Trim());//Характеристика
                    lvi.Tag = reader[4].ToString().Trim();//GUID характеристики

                    lvi.SubItems.Add(reader.GetDecimal(2).ToString());//Цена
                    if (reader[5].ToString() != "")
                    {
                        lvi.SubItems[1].Text = reader.GetDecimal(5).ToString();
                    }
                    its_certificate = Convert.ToInt16(reader["its_certificate"]);
                    //its_marked = reader["its_marked"].ToString().Length > 13 ? 1 : 0;
                    its_marked = Convert.ToInt16(reader["its_marked"]);
                    listView2.Items.Add(lvi);

                    //Надо проверить может уже сертификат есть в чеке      
                    if (its_certificate == 1)
                    {
                        foreach (ListViewItem _lvi_ in listView1.Items)
                        {
                            //if (_lvi_.SubItems[0].Text == reader.GetInt64(0).ToString())
                            if (_lvi_.SubItems[14].Text == barcode)
                            {
                                find_sertificate = true;
                                break;
                            }
                        }
                    }
                    //КОНЕЦ Надо проверить может уже сертификат есть в чеке                    
                }

                if (find_sertificate)
                {
                    MessageBox.Show("Этот сертификат уже добавлен в чек");
                    return;
                }

                //if (its_bonus_card)
                //{
                //    MessageBox.Show("Этот бонусная карта и добавлена в чек может быть только по нажатию на F9 ");
                //    return;
                //}

                //Проверка по сертификату
                if (its_certificate == 1)
                {
                    if (!check_sertificate_for_sales(barcode))
                    {
                        return;
                    }
                    Cash8.DS.DS ds = MainStaticClass.get_ds();
                    ds.Timeout = 60000;
                    //Получить параметр для запроса на сервер 
                    string nick_shop = MainStaticClass.Nick_Shop.Trim();
                    if (nick_shop.Trim().Length == 0)
                    {
                        MessageBox.Show(" Не удалось получить название магазина ");
                        return;
                    }
                    string code_shop = MainStaticClass.Code_Shop.Trim();
                    if (code_shop.Trim().Length == 0)
                    {
                        MessageBox.Show(" Не удалось получить код магазина ");
                        return;
                    }
                    string count_day = CryptorEngine.get_count_day();
                    string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
                    //string sertificate_code = get_tovar_code(barcode);
                    string sertificate_code = barcode;
                    string encrypt_data = CryptorEngine.Encrypt(sertificate_code, true, key);
                    string status = "";
                    try
                    {
                        status = ds.GetStatusSertificat(MainStaticClass.Nick_Shop, encrypt_data, MainStaticClass.GetWorkSchema.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(" Произошли ошибки при работе с сертификатами " + ex.Message);
                        return;
                    }
                    if (status == "-1")
                    {
                        MessageBox.Show("Произошли ошибки при работе с сертификатами");
                        return;
                    }
                    else
                    {
                        string decrypt_data = CryptorEngine.Decrypt(status, true, key);
                        if (decrypt_data == "1")
                        {
                            MessageBox.Show("Сертификат уже активирован");
                            return;
                        }
                    }

                }
                else if (its_certificate == 2)//Это продажа бонусной карты проверяем, что в шапке нет другой карты
                {
                    if (check_availability_card_sale())
                    {
                        MessageBox.Show("В строках чека уже есть бонусная карта на продажу");
                        return;
                    }
                    //if (get_status_promo_card(barcode) != 1)
                    //{
                    //    MessageBox.Show("Данная бонусная карта уже активирована и повторно продана быть не может");
                    //    return;
                    //}
                    if (client.Tag != null)
                    {
                        if (client.Tag.ToString() != barcode)
                        {
                            MessageBox.Show("В чеке уже выбран клиент с другой бонусной2 картой, продажа бонусной карты в этом чеке невозможна");
                            return;
                        }
                        if (card_state != 1)//Проверяем статус карты он должен быть 1 т.е. не активирована
                        {
                            MessageBox.Show("Эта карта имеет неверный статус в процессиноговом центре и продана быть не может ");
                            return;
                        }
                    }
                }

                //Подсчет суммы по документу
                if (listView2.Items.Count == 1)//1 товар найден
                {
                    ListViewItem lvi = null;
                    if ((its_marked == 0) && (its_certificate == 0) && ((MainStaticClass.GetWorkSchema == 1) || (MainStaticClass.GetWorkSchema == 3)))
                    {
                        lvi = exist_tovar_in_listView(listView1, Convert.ToInt64(select_tovar.Tag), listView2.Items[0].Tag);
                    }
                    if (lvi == null)
                    {
                        //select_tovar.Tag.ToString()
                        lvi = new ListViewItem(select_tovar.Tag.ToString());
                        lvi.Tag = select_tovar.Tag.ToString();
                        lvi.SubItems.Add(select_tovar.Text);//Наименование
                        lvi.SubItems.Add(listView2.Items[0].Text);//Характеиристика                         

                        if (listView2.Items[0].Tag == null)
                        {
                            lvi.SubItems[2].Tag = "";  // listView2.Items[0].Tag;//GUID характеристики   
                        }
                        else
                        {
                            lvi.SubItems[2].Tag = listView2.Items[0].Tag;//GUID характеристики   
                        }
                        if (!fractional)
                        {
                            lvi.SubItems.Add("1");
                        }
                        else
                        {
                            lvi.SubItems.Add("0,001");
                            //lvi.SubItems.Add("999");
                        }
                        lvi.SubItems.Add(listView2.Items[0].SubItems[1].Text);//Цена 
                                                                              //Проверка на сертификат               
                        if (this.its_certificate(select_tovar.Tag.ToString()) != "1")
                        {
                            lvi.SubItems.Add(Math.Round(Convert.ToDouble(lvi.SubItems[4].Text) - Convert.ToDouble(lvi.SubItems[4].Text) * Discount, 2).ToString());//Цена со скидкой
                        }
                        else
                        {
                            lvi.SubItems.Add(Math.Round(Convert.ToDecimal(lvi.SubItems[4].Text), 2).ToString());//Цена со скидкой 
                        }
                        lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());//Сумма
                        lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString()); //Сумма со скидкой                        
                        lvi.SubItems.Add("0"); //Номер акционного документа скидка
                        lvi.SubItems.Add("0"); //Номер акционного документа подарок
                        lvi.SubItems.Add("0"); //Номер акционного документа дополнительное поле пометка что участвовало в акции, но скидка может быть
                        lvi.SubItems.Add("0");//Бонус
                        lvi.SubItems.Add("0");//Бонус1
                        lvi.SubItems.Add("0");//Бонус2
                        if (its_certificate == 0)
                        {
                            lvi.SubItems.Add("0");//Маркер
                        }
                        else if (its_certificate == 1)
                        {
                            lvi.SubItems.Add(barcode);//Это сертификат и при продаже мы добавляем его штрихкод 
                        }
                        //listView1.Items.Add(lvi);
                        //listView1.Select();
                        //listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                        //update_record_last_tovar(listView1.Items[this.listView1.Items.Count - 1].SubItems[1].Text, listView1.Items[this.listView1.Items.Count - 1].SubItems[3].Text);
                        bool error = false;
                        int code_marking_error = 0;
                        bool cdn_vrifyed = false;
                        string mark_str = "";
                        //bool its_marked = (check_sign_marker_code(select_tovar.Tag.ToString()) > 0);
                        if (its_marked > 0)
                        {
                            if (!Console.CapsLock)
                            {
                                if ((Control.ModifierKeys & Keys.Shift) != Keys.Shift)
                                {
                                    this.qr_code = "";
                                    Input_action_barcode input_Action_Barcode = new Input_action_barcode();
                                    input_Action_Barcode.call_type = 6;
                                    input_Action_Barcode.caller = this;
                                    if (DialogResult.Cancel == input_Action_Barcode.ShowDialog())
                                    {
                                        error = true;
                                    }
                                    if (this.qr_code != "")//Был введен qr код необходимо его внести в чек
                                    {

                                        if (this.qr_code.ToUpper().Substring(0, 4).IndexOf("HTTP") != -1)
                                        {
                                            error = true;
                                            MessageBox.Show("Считан не верный qr код");
                                            MainStaticClass.write_event_in_log("HTTP не верный qr код  " + barcode, "Документ чек", numdoc.ToString());
                                            this.qr_code = "";

                                        }
                                        else
                                        {
                                            //здесь проверка на известные ошибки \u001d                                        
                                            int num_pos = this.qr_code.IndexOf("\\");
                                            if (num_pos > 0)
                                            {
                                                if (this.qr_code.Substring(num_pos + 1, 5) == "u001d")//необходимо из строки вырезать этот символ
                                                {
                                                    this.qr_code = this.qr_code.Substring(0, num_pos) + this.qr_code.Substring(num_pos + 1 + 5, this.qr_code.Length - (num_pos + 1 + 5));
                                                    num_pos = this.qr_code.IndexOf("\\");
                                                    if (num_pos > 0)
                                                    {
                                                        this.qr_code = this.qr_code.Substring(0, num_pos) + this.qr_code.Substring(num_pos + 1 + 5, this.qr_code.Length - (num_pos + 1 + 5));
                                                    }
                                                }
                                            }

                                            if (this.qr_code != "")
                                            {
                                                if (this.qr_code.ToUpper().Substring(0, 4).IndexOf("HTTP") != -1)
                                                {
                                                    error = true;
                                                    MessageBox.Show("Считан не верный код маркировки");
                                                    MainStaticClass.write_event_in_log("HTTP не верный код маркировки  " + this.qr_code, "Документ чек", numdoc.ToString());
                                                }
                                            }

                                            //перед тем как добавить qr код в чек необходимо его проверить
                                            //string mark_str = "";
                                            string mark_str_cdn = "";
                                            if (this.qr_code.Trim().Length > 13)
                                            {
                                                if (!qr_code_lenght.Contains(this.qr_code.Trim().Length))
                                                {
                                                    MessageBox.Show(qr_code + "\r\n Ваш код маркировки имеет длину " + qr_code.Length.ToString() + " символов при этом он не входит в допустимый диапазок ", "Проверка qr-кода на допустимую длину");
                                                    //MessageBox.Show("Длина введенного qr-кода не входит в диапазон допустимых ! \r\n ТОВАР В ЧЕК ДОБАВЛЕНЕ НЕ БУДЕТ!!! ", "Проверки введенного qr кода");
                                                    MainStaticClass.write_event_in_log("Длина введенного кода маркировки не входит в диапазон допустимых, он имеет длину " + qr_code.Length.ToString(), "Документ чек", numdoc.ToString());
                                                    error = true;
                                                    return;
                                                }
                                            }
                                            if (MainStaticClass.Version2Marking == 1)
                                            {
                                                WortWithMarkingV3 markingV3 = new WortWithMarkingV3();
                                                mark_str = this.qr_code.Trim();
                                                mark_str = add_gs1(mark_str);
                                                bool result_check_cdn = false;
                                                bool timeout_check_cdn = false;//таймаут при проверке по cdn

                                                //*******************************************************************************************************************************
                                                foreach (ListViewItem listViewItem4 in this.listView1.Items)
                                                {
                                                    if (listViewItem4.SubItems[14].Text == mark_str)
                                                    {
                                                        MessageBox.Show("Номенклатура с введенным кодом маркировки который вы пытались добавить уже существует в чеке. \r\n Номенклатура не будет добавлена.");
                                                        error = true;
                                                        break;
                                                    }
                                                }
                                                if (cdn_check)
                                                {
                                                    if (MainStaticClass.CashDeskNumber != 9 && MainStaticClass.EnableCdnMarkers == 1)
                                                    {
                                                        if (MainStaticClass.CDN_Token == "")
                                                        {
                                                            MessageBox.Show("В этой кассе не заполнен CDN токен, \r\n ПРОДАЖА ДАННОГО ТОВАРА НЕВОЗМОЖНА ! ", "Проверка CDN");
                                                            return;
                                                        }
                                                        CDN cdn = new CDN();
                                                        List<string> codes = new List<string>();
                                                        mark_str_cdn = mark_str.Replace("\u001d", @"\u001d");                                                        
                                                        codes.Add(mark_str_cdn);                                                        
                                                        mark_str_cdn = mark_str_cdn.Replace("'", "\'");
                                                        Dictionary<string, string> d_tovar = new Dictionary<string, string>();
                                                        d_tovar[lvi.SubItems[1].Text] = lvi.SubItems[0].Text;                                                        
                                                        result_check_cdn = cdn.check_marker_code(codes, mark_str, this.numdoc, ref request, mark_str_cdn, d_tovar, ref timeout_check_cdn);
                                                        if ((!result_check_cdn) && (!timeout_check_cdn))//не прошел проверку и таймаута не было 
                                                        {
                                                            return;
                                                        }
                                                        else
                                                        {
                                                            cdn_vrifyed = true;
                                                        }
                                                    }
                                                }
                                                // && MainStaticClass.PrintingUsingLibraries==0
                                                //if (!cdn_vrifyed)
                                                //{
                                                byte[] textAsBytes = Encoding.Default.GetBytes(mark_str);
                                                string imc = Convert.ToBase64String(textAsBytes);

                                                if (MainStaticClass.PrintingUsingLibraries == 0)
                                                {
                                                    if (!cdn_vrifyed)
                                                    {
                                                        WortWithMarkingV3.Root root = markingV3.beginMarkingCodeValidation("auto", imc, "itemPieceSold", 1, "piece", 0, false);
                                                        if (root.results[0].errorCode != 0)
                                                        {
                                                            markingV3.cancelMarkingCodeValidation();//прерываем валидацию
                                                            code_marking_error = root.results[0].errorCode;
                                                            if ((code_marking_error != 421) && (code_marking_error != 402))
                                                            {
                                                                MainStaticClass.write_event_in_log("beginMarkingCodeValidation " + root.results[0].errorDescription + " " + code_marking_error, "Документ", numdoc.ToString());
                                                                error = true;//не прошли проверку товар не добавляем в чек
                                                            }
                                                        }
                                                        else
                                                        {
                                                            root = markingV3.getMarkingCodeValidationStatus();
                                                            if (root.results[0].result != null)
                                                            {
                                                                if (root.results[0].result.driverError != null)
                                                                {
                                                                    if (root.results[0].result.driverError.code != 0)
                                                                    {
                                                                        markingV3.cancelMarkingCodeValidation();//прерываем валидацию
                                                                        code_marking_error = root.results[0].result.driverError.code;
                                                                        //if (code_marking_error != 0)
                                                                        //{
                                                                        if ((code_marking_error != 421) && (code_marking_error != 402))
                                                                        {
                                                                            //markingV3.cancelMarkingCodeValidation();//прерываем валидацию
                                                                            MessageBox.Show("getMarkingCodeValidationStatus " + root.results[0].result.driverError.description, "Ошибка при начале проверки кода маркировки");
                                                                            error = true;//не прошли проверку товар не добавляем в чек
                                                                        }
                                                                    }
                                                                    else//проверяем статус и если все хорошо отправляем принять 
                                                                    {
                                                                        //if (root.results[0].result.ready)
                                                                        //{
                                                                        if (root.results[0].result.onlineValidation.itemInfoCheckResult.imcCheckFlag &&
                                                                            root.results[0].result.onlineValidation.itemInfoCheckResult.imcCheckResult &&
                                                                            root.results[0].result.onlineValidation.itemInfoCheckResult.imcStatusInfo &&
                                                                            root.results[0].result.onlineValidation.itemInfoCheckResult.imcEstimatedStatusCorrect)
                                                                        {
                                                                            //Все признаки успех, значит M+
                                                                            markingV3.acceptMarkingCode();
                                                                            MainStaticClass.write_event_in_log("acceptMarkingCode  " + lvi.SubItems[0].Text, "Документ чек", numdoc.ToString());
                                                                        }
                                                                        else// сообщим детально об ошибке 
                                                                        {
                                                                            if (!root.results[0].result.onlineValidation.itemInfoCheckResult.imcCheckFlag)
                                                                            {
                                                                                MessageBox.Show("Код маркировки не был проверен ФН и(или) ОИСМП");
                                                                            }
                                                                            if (!root.results[0].result.onlineValidation.itemInfoCheckResult.imcCheckResult)
                                                                            {
                                                                                MessageBox.Show("Результат проверки КП КМ отрицательный или код маркировки не был проверен");
                                                                            }
                                                                            if (!root.results[0].result.onlineValidation.itemInfoCheckResult.imcStatusInfo)
                                                                            {
                                                                                MessageBox.Show("Сведения о статусе товара от ОИСМП не получены");
                                                                            }
                                                                            if (!root.results[0].result.onlineValidation.itemInfoCheckResult.imcEstimatedStatusCorrect)
                                                                            {
                                                                                MessageBox.Show("От ОИСМП получены сведения, что планируемый статус товара некорректен или сведения о статусе товара от ОИСМП не получены");
                                                                            }

                                                                            error = true;
                                                                            markingV3.cancelMarkingCodeValidation();//прерываем валидацию
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    MainStaticClass.write_event_in_log("root.results[0].result.driverError == null  " + lvi.SubItems[0].Text, "Документ чек", numdoc.ToString());
                                                                    markingV3.cancelMarkingCodeValidation();//прерываем валидацию
                                                                    code_marking_error = 421;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                error = true;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    PrintingUsingLibraries printingUsingLibraries = new PrintingUsingLibraries();
                                                    if (!printingUsingLibraries.check_marking_code(imc, this.numdoc.ToString(), ref this.cdn_markers_result_check))
                                                    {
                                                        error = true;
                                                    }
                                                }
                                                //}
                                            }
                                            else
                                            {
                                                foreach (ListViewItem listViewItem4 in this.listView1.Items)
                                                {
                                                    if (listViewItem4.SubItems[14].Text == this.qr_code)
                                                    {
                                                        error = true;
                                                        MessageBox.Show("Номенклатура с введенным кодом маркировки который вы пытались добавить уже существует в чеке. \r\n Номенклатура не будет добавлена.");
                                                        break;
                                                    }
                                                }
                                            }
                                            if (error)
                                            {
                                                return;
                                            }
                                            if (mark_str != "")
                                            {
                                                lvi.SubItems[14].Text = mark_str;//добавим в чек qr код                                        
                                            }
                                            else
                                            {
                                                lvi.SubItems[14].Text = this.qr_code;//добавим в чек qr код                                        
                                            }
                                            //}
                                            //else
                                            //{
                                            //    MessageBox.Show("Введен неверный код маркировки, попробуйте еще раз. Номенклатура не будет добавлена.");
                                            //    error = true;
                                            //    //Не добавляем позицию в чек
                                            //}
                                            this.qr_code = "";//обнулим переменную
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("У вас нажата клавиша Shift, ввод кода маркировки невозможен.Номенклатура не будет добавлена.");
                                    //Не добавляем позицию в чек
                                    error = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("У вас нажата клавиша CapsLock, ввод кода маркировки невозможен.Номенклатура не будет добавлена.");
                                //Не добавляем позицию в чек
                                error = true;

                            }

                            if ((!error) || ((code_marking_error == 402) || (code_marking_error == 421) || cdn_vrifyed))//Если с qr кодом все хорошо тогда добавляем позицию иначе не добавляем 
                            {
                                listView1.Items.Add(lvi);                               

                                MainStaticClass.write_event_in_log("Товар добавлен " + barcode, "Документ чек", numdoc.ToString());                                
                                if (cdn_vrifyed && MainStaticClass.PrintingUsingLibraries == 0)
                                {                                    
                                    km_adding_to_buffer_index(listView1.Items.Count - 1);
                                    MainStaticClass.write_event_in_log("cdn_vrifyed = " + cdn_vrifyed + ", на фискальном регистраторе не проверяем ", "Документ чек", numdoc.ToString());
                                }
                            }
                            else
                            {
                                MainStaticClass.write_event_in_log("Отказ от ввода qr кода, товар не добавлен", "Документ чек", numdoc.ToString());
                                last_tovar.Text = barcode;
                                Tovar_Not_Found t_n_f = new Tovar_Not_Found();
                                t_n_f.textBox1.Text = "Код маркировки не прошел проверку";
                                t_n_f.textBox1.Font = new Font("Microsoft Sans Serif", 22);
                                t_n_f.label1.Text = " Код ошибки code_marking_error = " + code_marking_error.ToString();
                                t_n_f.ShowDialog();
                                t_n_f.Dispose();
                                return;
                            }
                        }
                        else
                        {
                            listView1.Items.Add(lvi);
                        }

                        if (fractional)
                        {
                            listView1.Focus();
                            listView1.Select();
                            listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                            listView1.Items[this.listView1.Items.Count - 1].Focused  = true;
                            //SendKeys.Send("Enter");                            
                            show_quantity_control(true);                           
                        }


                        //if ((!error) || ((code_marking_error == 402) || (code_marking_error == 421) || cdn_vrifyed))//Если с qr кодом все хорошо тогда добавляем позицию иначе не добавляем 
                        //{
                        //    MainStaticClass.write_event_in_log("Товар добавлен " + barcode, "Документ чек", numdoc.ToString());                            
                        //    //if ((code_marking_error == 402) || (code_marking_error == 421))//это говорит о том что включена новая схема маркировки и мы имеет проблемы с интернетом
                        //    //{
                        //        //km_adding_to_buffer(listView1.Items.Count - 1);//принудительно добавляем в буфер последнюю строку с маркировкой 
                        //        MainStaticClass.write_event_in_log("cdn_vrifyed = " + cdn_vrifyed + ", на фискальном регистраторе не проверяем ", "Документ чек", numdoc.ToString());
                        //        if (MainStaticClass.PrintingUsingLibraries == 0)
                        //        {
                        //            listView1.Items.Add(lvi);
                        //            km_adding_to_buffer_index(listView1.Items.Count - 1);                                    
                        //        }
                        //        else
                        //        {
                        //            //km_adding_to_buffer
                        //            PrintingUsingLibraries printingUsingLibraries = new PrintingUsingLibraries();
                        //            if (!printingUsingLibraries.km_adding_to_buffer(mark_str, this.numdoc.ToString()))
                        //            {
                        //                error = true;
                        //            }
                        //            else
                        //            {
                        //                listView1.Items.Add(lvi);
                        //            }
                        //        }                                
                        //    //}
                        //}
                        //else
                        //{
                        //    MainStaticClass.write_event_in_log("Отказ от ввода qr кода, товар не добавлен", "Документ чек", numdoc.ToString());
                        //    last_tovar.Text = barcode;
                        //    Tovar_Not_Found t_n_f = new Tovar_Not_Found();
                        //    t_n_f.textBox1.Text = "Код маркировки не прошел проверку";
                        //    t_n_f.textBox1.Font = new Font("Microsoft Sans Serif", 22);
                        //    t_n_f.label1.Text = " Код ошибки code_marking_error = " + code_marking_error.ToString();
                        //    t_n_f.ShowDialog();
                        //    t_n_f.Dispose();
                        //    return;
                        //}

                        SendDataToCustomerScreen(1, 0, 1);
                        if ((MainStaticClass.GetWorkSchema == 1) || (MainStaticClass.GetWorkSchema == 3))
                        {
                            if (!fractional)
                            {
                                listView1.Select();
                                listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                            }
                            //inputbarcode.Focus();//01.08.2022 Теперь здесь для Визы автоматом переност фокуса
                        }
                        else if (MainStaticClass.GetWorkSchema == 2)
                        {
                            //inputbarcode.Focus();
                        }

                        update_record_last_tovar(listView1.Items[this.listView1.Items.Count - 1].SubItems[1].Text, listView1.Items[this.listView1.Items.Count - 1].SubItems[3].Text);

                        //if (MainStaticClass.Use_Trassir > 0)
                        //{
                        //    string s = MainStaticClass.get_string_message_for_trassir("POSNG_POSITION_ADD", numdoc.ToString(), MainStaticClass.Cash_Operator, DateTime.Now.Date.ToString("MM'/'dd'/'yyyy"), DateTime.Now.ToString("HH:mm:ss"), (listView1.Items.Count - 1).ToString(), "1", listView2.Items[0].SubItems[1].Text, barcode, "", MainStaticClass.CashDeskNumber.ToString(), select_tovar.Text + " "+listView2.Items[0].Text);
                        //    MainStaticClass.send_data_trassir(s);
                        //}                        
                    }
                    else
                    {
                        if (!fractional)
                        {
                            lvi.SubItems[3].Text = (Convert.ToDecimal(lvi.SubItems[3].Text) + 1).ToString();
                            calculate_on_string(lvi);
                            lvi.Selected = true;
                            listView1.Select();
                        }
                        else
                        {
                            listView1.Focus();
                            listView1.Select();
                            listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                            listView1.Items[this.listView1.Items.Count - 1].Focused = true;                            
                            show_quantity_control(true);
                        }                        
                        update_record_last_tovar(lvi.SubItems[1].Text, lvi.SubItems[4].Text);
                    }
                    if (!fractional)
                    {
                        inputbarcode.Focus();
                    }
                    calculation_of_the_sum_of_the_document();
                }
                else if (listView2.Items.Count > 1)//Найденных товаров больше одного необходимо показать список выбра пользователю
                {
                    this.panel2.Visible = true;
                    this.panel2.BringToFront();
                    this.listView2.Visible = true;
                    listView2.Select();
                    listView2.Items[0].Selected = true;
                    listView2.Items[0].Focused = true;
                }
                else if (listView2.Items.Count == 0)
                {
                    //MessageBox.Show("Не найден");
                    //stop_com_barcode_scaner();
                    last_tovar.Text = barcode;
                    Tovar_Not_Found t_n_f = new Tovar_Not_Found();
                    t_n_f.ShowDialog();
                    t_n_f.Dispose();
                    //start_com_barcode_scaner();
                }
                reader.Close();
                conn.Close();                
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("find_barcode_or_code_in_tovar " + ex.Message, "NpgsqlException");
            }
            catch (Exception ex)
            {
                MessageBox.Show("find_barcode_or_code_in_tovar " + ex.Message, "Exception");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    // conn.Dispose();
                }
            }

            write_new_document("0", "0", "0", "0", false, "0", "0", "0", "0");
        }
        //5010182990247


            /// <summary>
            /// Добавить разделитель групп
            /// </summary>
            /// <param name="mark_str"></param>
            /// <returns></returns>
        private string add_gs1(string mark_str)
        {
            string GS1 = Char.ConvertFromUtf32(29);
            if ((mark_str.Length == 83) || (mark_str.Length == 127) || (mark_str.Length == 115))
            {
                mark_str = mark_str.Insert(31, GS1);
                mark_str = mark_str.Insert(38, GS1);
            }

            if (mark_str.Length == 37 && mark_str.Substring(16, 2) == "21")
            {
                //lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(31, GS1);
                mark_str = mark_str.Insert(31, GS1);
            }

            if (mark_str.Length == 30)
            {
                //lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(24, GS1);
                mark_str = mark_str.Insert(24, GS1);
            }

            if (mark_str.Length == 32)
            {
                //lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(26, GS1);                                                    
                mark_str = mark_str.Insert(26, GS1);
            }

            return mark_str;
        }


        ///// <summary>
        ///// Проверить на корректность код маркировки 
        ///// при помощи CDN серверов
        ///// 1. Успех если проверен успешно.
        ///// 2. Успех если нет связи(таймаут или что либо подобное)
        ///// Возвращает -1 если код проверен и он не корректне
        ///// Возвращает 1 если код успешно проверен
        ///// Возвращает 2 если по таймауту не удалось проверить код
        ///// </summary>
        ///// <param name="mark_str"></param>
        ///// <returns></returns>
        //private int check_marker_code_with_cdn(string mark_str)
        //{
        //    int result = 0;

        //    CDN cdn = new CDN();
        //    CDN.CDN_List cdn_list = MainStaticClass.CDN_List;

        //    if (cdn_list == null)//Нет доступных серверов для опроса
        //    {
        //        result = 2;
        //        MainStaticClass.write_event_in_log(" check_marker_code_with_cdn cdn_list = null","Документ",numdoc.ToString());
        //        return result;
        //    }
            
        //    if (cdn_list.hosts.Count > 0)
        //    {

        //        if (cdn.selection_and_sorting(cdn_list) == 0)
        //        {
        //            result = 2;
        //            MainStaticClass.write_event_in_log(" check_marker_code_with_cdn cdn_list != null, но доступных адресов CDN нет", "Документ", numdoc.ToString());
        //            return result;
        //        }
        //    }

        //    mark_str = add_gs1(mark_str);          
        //    List<string> codes = new List<string>();
        //    string mark_str_cdn = mark_str.Replace("\u001D", "\\u001d");
        //    codes.Add("mark_str_cdn");
        //    if (!cdn.check_marker_code(codes, mark_str, ref this.cdn_markers_date_time, this.numdoc, ref request))
        //    {
        //        if (cdn.error_timeout >= 3)
        //        {
        //            result = 2;
        //        }
        //        else
        //        {
        //            result = -1;
        //        }
        //    }
        //    else
        //    {
        //        result = 1;
        //    }

        //    return result;
        //}



        /// <summary>
        /// Очищаем буфер проверенных КМ
        /// а затем снова его заполняет
        /// при первичном заполнении происходит проверка 
        /// при перезаполнении мы повторно не проверяем на честном знаке
        /// </summary>
        private void reload_km_buffer()
        {
            clearMarkingCodeValidationResult();//сначала очизаем буфер проверенных
            foreach (ListViewItem lvi in listView1.Items)
            {
                if (lvi.SubItems[14].Text.Trim().Length > 13)
                {
                    km_adding_to_buffer(lvi.Index);//а затем снова добавляем в буфер проверенных без проверки , если они уже были добавлены в чек, значит проверены
                }
            }
        }

        private int check_sign_marker_code(string code_tovar)
        {
            int result = 0;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT its_marked FROM tovar where code=" + code_tovar;
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToInt32(command.ExecuteScalar());
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" ВНИМАНИЕ !!! Ошибка при получении признака маркировки " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(" ВНИМАНИЕ !!! Ошибка при получении признака маркировки " + ex.Message);
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
        private void listView2_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (!itsnew)
            {
                return;
            }
            if (e.KeyChar == 13)
            {
                object characteristic = listView2.Items[listView2.SelectedIndices[0]].Tag;

                ListViewItem lvi = exist_tovar_in_listView(listView1, Convert.ToInt32(select_tovar.Tag), characteristic);

                if (lvi == null)
                {
                    lvi = new ListViewItem(select_tovar.Tag.ToString());
                    lvi.Tag = select_tovar.Tag.ToString();
                    lvi.SubItems.Add(select_tovar.Text);//Наименование
                    lvi.SubItems.Add(listView2.Items[listView2.SelectedIndices[0]].Text);//Характеиристика
                    if (listView2.Items[listView2.SelectedIndices[0]].Tag == null) //GUID характеристики   
                    {
                        listView2.Items[listView2.SelectedIndices[0]].Tag = "";
                    }
                    else
                    {
                        lvi.SubItems[2].Tag = listView2.Items[listView2.SelectedIndices[0]].Tag;//GUID характеристики   
                    }

                    int index = listView2.SelectedIndices[0];
                    //lvi = new ListViewItem(listView2.Items[index].Text);
                    //lvi.Tag = listView2.Items[index].Tag;
                    //lvi.SubItems.Add(listView2.Items[index].SubItems[1].Text);//Наименование
                    //lvi.SubItems.Add(listView2.Items[index].SubItems[2].Text);//Характеристика
                    //lvi.SubItems[2].Tag = listView2.Items[0].SubItems[2].Tag;//GUID характеристики
                    lvi.SubItems.Add("1");//Количество
                    lvi.SubItems.Add(listView2.Items[index].SubItems[1].Text);//Цена                        
                    lvi.SubItems.Add(Math.Round(Convert.ToDouble(lvi.SubItems[4].Text) - Convert.ToDouble(lvi.SubItems[4].Text) * Convert.ToDouble(Discount), 2).ToString());//Цена со скидкой
                    lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());//Сумма
                    lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString()); //Сумма со скидкой                        
                    lvi.SubItems.Add("0");//Номер акционного документа скидка
                    lvi.SubItems.Add("0");//Номер акционного документа подарок
                    lvi.SubItems.Add("0"); //Номер акционного документа дополнительное поле пометка что участвовало в акции, но скидка может быть
                    listView1.Items.Add(lvi);
                    SendDataToCustomerScreen(1, 0,1);
                    this.listView1.Select();
                    this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                    update_record_last_tovar(listView1.Items[this.listView1.Items.Count - 1].SubItems[1].Text, listView1.Items[this.listView1.Items.Count - 1].SubItems[4].Text);



                    //lvi = new ListViewItem(listView2.Items[listView2.SelectedIndices[0]].Text);
                    //lvi.Tag = listView2.Items[listView2.SelectedIndices[0]].Tag;
                    //lvi.SubItems.Add("1");
                    //lvi.SubItems.Add(listView2.Items[listView2.SelectedIndices[0]].SubItems[1].Text);//Цена                        
                    //lvi.SubItems.Add(Math.Round(Convert.ToDecimal(lvi.SubItems[2].Text) - Convert.ToDecimal(lvi.SubItems[2].Text) * Discount, 2).ToString());//Цена со скидкой
                    //lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[1].Text) * Convert.ToDecimal(lvi.SubItems[2].Text)).ToString());//Сумма
                    //lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[1].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString()); //Сумма со скидкой                        
                    //listView1.Items.Add(lvi);
                }
                else
                {
                    lvi.SubItems[3].Text = (Convert.ToInt64(lvi.SubItems[3].Text) + 1).ToString();
                    //lvi.SubItems[4].Text = listView2.Items[listView2.SelectedIndices[0]].SubItems[3].Text;//Цена
                    calculate_on_string(lvi);
                    this.listView1.Select();
                    lvi.Selected = true;
                    update_record_last_tovar(lvi.SubItems[1].Text, lvi.SubItems[3].Text);
                }
                calculation_of_the_sum_of_the_document();
                this.panel2.Visible = false;
                this.listView2.Visible = false;

                this.last_tovar.Text = listView1.Items[this.listView1.Items.Count - 1].SubItems[1].Text;

            }
            else if (e.KeyChar == 27)
            {
                //this.panel2.Visible = false;
                //this.listView2.Visible = false;
            }
        }


        private string get_date_birthday()
        {
            string result = "";

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = conn;
                command.CommandText = "SELECT date_of_birth FROM clients WHERE clients.code='" + client.Tag.ToString() + "'";
                result = Convert.ToDateTime(command.ExecuteScalar()).ToString("dd-MM-yyyy");

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("get_date_birthday "+ex.Message);
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


        /// <summary>
        /// Проверка на ввод 3 чеков на кассе за один день
        /// если чеков меньше трех, то возврат истина иначе ложь
        /// </summary>
        /// <param name="code_client"></param>
        /// <returns></returns>
        private bool to_allow_input_of_the_check(string code_client)
        {
            bool result = true;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM checks_header WHERE client='" + code_client + "' AND date_time_start BETWEEN '" + DateTime.Now.ToString("yyyy-MM-dd 00:00:00") +
                    "' AND '" + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00") + "' AND its_deleted=0";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = (Convert.ToInt32(command.ExecuteScalar()) >= 3 ? false : true);
                conn.Close();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" Ошибки при контроле 3-х чеков " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Ошибки при контроле 3-х чеков " + ex.Message);
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

        /// <summary>
        /// Обработка ввода дисконтной карты клиента
        /// </summary>
        /// <param name="barcode"></param>
        private void process_client_discount(string barcode)
        {
            Discount = 0;
            //int bonus_is_on = 0;

            //if (listView1.Items.Count > 0)
            //{
            //    MyMessageBox mmb = new MyMessageBox(" Дисконтную карту можно сканировать только до ввода товаров ", " Проверка ввода ");
            //    mmb.ShowDialog();
            //    mmb.Dispose();
            //    return;
            //}

            //if ((barcode.Trim().Length == 10) || (barcode.Trim().Length == 11) || (barcode.Trim().Length == 13))
            if ((barcode.Trim().Length == 10) || (barcode.Trim().Length == 13))
            {
                MainStaticClass.write_event_in_log(" Код клиента имеет нормальную длину " + barcode, " Документ ", numdoc.ToString());
                //if (MainStaticClass.PassPromo == "")
                //{
                NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = conn;
                //command.CommandText = " SELECT discount_types.discount_percent,clients.code,clients.name,clients.phone AS clients_phone," +
                //    " temp_phone_clients.phone AS temp_phone_clients_phone,attribute,clients.its_work,COALESCE(clients.bonus_is_on,0) AS bonus_is_on  FROM clients " +
                //    " left join discount_types ON clients.discount_types_code= discount_types.code " +
                //    " left join temp_phone_clients ON clients.code = temp_phone_clients.barcode " +
                //    " WHERE clients.code='" + barcode + "' OR right(clients.phone,10)='" + barcode + "' AND clients.its_work = 1 ";

                if (barcode.Substring(0, 1) == "9")
                {
                    check_and_verify_phone_number(barcode);//возможно что это новый клиент необходимо провести проверку 

                    command.CommandText = " SELECT 5.00,clients.code,clients.name,clients.phone AS clients_phone," +
                     " temp_phone_clients.phone AS temp_phone_clients_phone,attribute,clients.its_work,COALESCE(clients.bonus_is_on,0) AS bonus_is_on  FROM clients " +                     
                     " left join temp_phone_clients ON clients.code = temp_phone_clients.barcode " +                     
                     " WHERE clients.phone='" + barcode + "' AND clients.its_work = 1 ";
                }
                else
                {
                    command.CommandText = " SELECT 5.00,clients.code,clients.name,clients.phone AS clients_phone," +
                        " temp_phone_clients.phone AS temp_phone_clients_phone,attribute,clients.its_work,COALESCE(clients.bonus_is_on,0) AS bonus_is_on  FROM clients " +                        
                        " left join temp_phone_clients ON clients.code = temp_phone_clients.barcode " +                        
                        " WHERE clients.code='" + barcode + "' AND clients.its_work = 1 ";
                }

                MainStaticClass.write_event_in_log("Старт поиска клиента", "Документ чек", numdoc.ToString());

                NpgsqlDataReader reader = command.ExecuteReader();
                //bool client_find = false;
                while (reader.Read())
                {
                    //bool client_find = true;

                    if (reader["its_work"].ToString() != "1")
                    {
                        MessageBox.Show(" Эта карточка клиента заблокирована !!!");
                        break;
                    }

                    //bonus_is_on = Convert.ToInt16(reader["bonus_is_on"]);

                    client_barcode_scanned = 1;

                    //if (bonus_is_on == 0)
                    //{
                        Discount = Convert.ToDouble(reader.GetDecimal(0))/100;
                    //}

                    client_barcode.Enabled = false;//дисконтная карта определена, сделаем недоступным окно ввода кода  
                    txtB_client_phone.Enabled = false;//дисконтная карта определена, сделаем недоступным окно ввода телефона  
                    btn_inpute_phone_client.Enabled = false;
                    this.btn_inpute_phone_client.Enabled = false;
                    //it_is_possible_to_write_off_bonuses = true;

                    if (reader["attribute"].ToString().Trim() == "1")
                    {
                        client.BackColor = System.Drawing.ColorTranslator.FromHtml("#22FF99");
                    }
                    MainStaticClass.write_event_in_log(" Клиент найден ", "Документ чек", numdoc.ToString());
                    MainStaticClass.write_event_in_log(" Присвоение значения реквизиту на форме ", " Документ ", numdoc.ToString());
                    //this.client.Tag = reader["code"].ToString();
                    this.client.Tag = reader["code"].ToString();
                    this.client.Text = reader["name"].ToString();
                    

                    if ((reader["clients_phone"].ToString().Trim().Length < 10) && (reader["temp_phone_clients_phone"].ToString().Trim().Length < 10))//будем считать, что номера телефона нет
                    {
                        InputePhoneClient ipc = new InputePhoneClient();
                        ipc.barcode = barcode;
                        DialogResult dialogResult = ipc.ShowDialog();
                        //if (bonus_is_on == 1)//Проверка для новой бонусной карты, при сканировании если нет привязанного номера телефона, то он обязательно должен быть введен 
                        //{
                        //    if (dialogResult != DialogResult.OK)
                        //    {
                        //        MessageBox.Show(" Для бонусной карты недопустимо отсутствие номера телефона ");
                        //        MessageBox.Show(" БОНУСНУЮ КАРТУ НЕ ВЫДАВАТЬ НИ В КОЕМ СЛУЧАЕ !!! ");
                        //        this.client.Tag = null;
                        //        this.client.Text = "";
                        //        this.client_barcode.Text = "";
                        //    }
                        //}
                        this.inputbarcode.Focus();
                        btn_inpute_phone_client.Enabled = true;
                    }
                    else//думалось заполнить телефонный номер в форму , но это наверное лишнее
                    {
                        btn_inpute_phone_client.Enabled = true;
                    }
                }
                reader.Close();
                conn.Close();
                

                if (this.client.Tag == null)//По каким то причинам клиент или не найден или не прошел проверки 
                {
                    MainStaticClass.write_event_in_log(" Клиент не найден ", "Документ чек", numdoc.ToString());
                    MessageBox.Show("Клиент не найден");
                    return;
                }              
                
               
                if (check_type.SelectedIndex == 1)//При возврате теперь можно использовать карту клиента 
                {
                    Discount = 0;
                }

                //Discount = Discount / 100;

                if (Discount != 0)//Пересчитать цены 
                {
                    MainStaticClass.write_event_in_log(" Начало пересчета ТЧ " + barcode, " Документ ", numdoc.ToString());
                    foreach (ListViewItem lvi in listView1.Items)
                    {
                        calculate_on_string(lvi);
                    }
                    MainStaticClass.write_event_in_log(" Окончание пересчета ТЧ " + barcode, " Документ ", numdoc.ToString());
                }

                //Проверить на день рождения и вывести предупреждение
                MainStaticClass.write_event_in_log(" Проверка на день рождения начало " + barcode, " Документ ", numdoc.ToString());
                if (actions_birthday())
                {
                    MessageBox.Show(" ДР " + get_date_birthday());
                    MainStaticClass.write_event_in_log(" Сегодня днюха у " + barcode, " Документ ", numdoc.ToString());
                }
                MainStaticClass.write_event_in_log(" Проверка на день рождения окончание " + barcode, " Документ ", numdoc.ToString());
                this.inputbarcode.Focus();
                this.client_barcode.Text = "";
                this.btn_inpute_phone_client.Enabled = false;                           
            }
            else
            {
                MessageBox.Show("Введено неверное количество символов");
            }
            MainStaticClass.write_event_in_log(" Выход из процедуры поиска клиента " + barcode, " Документ ", numdoc.ToString());
        }


        ///// <summary>
        ///// получить текстовое описание по коду 
        ///// </summary>
        ///// <param name="code_error"></param>
        //private void get_description_errors_on_code(int code_answer)
        //{
        //    switch (code_answer)
        //    {
        //        case 2:
        //            MessageBox.Show("2. Неверный запрос ");
        //            break;
        //        case 3:
        //            MessageBox.Show("3. Клиент не найден ");
        //            break;
        //        case 4:
        //            MessageBox.Show("4. Недостаточно прав для операции ");
        //            break;
        //        case 5:
        //            MessageBox.Show("5. Карта не найдена ");
        //            break;
        //        case 6:
        //            MessageBox.Show("6. Операции с картой запрещены ");
        //            break;
        //        case 7:
        //            MessageBox.Show("7. Ошибочная операция с картой ");
        //            break;
        //        case 8:
        //            MessageBox.Show("8. Недостаточно средств ");
        //            break;
        //        case 9:
        //            MessageBox.Show("9. Транзакция не найдена(неверный transactionId) ");
        //            break;
        //        case 10:
        //            MessageBox.Show("10. Значение параметра выходит за границы ");
        //            break;
        //        case 11:
        //            MessageBox.Show("11. Id чека, сгенерированный кассой, не уникален ");
        //            break;
        //        case 12:
        //            MessageBox.Show("12. Артикул товара(SKU) не найден ");
        //            break;
        //        case 13:
        //            MessageBox.Show("13. Ошибка запроса ");
        //            break;
        //        case 14:
        //            MessageBox.Show("14. Ошибка базы данных ");
        //            break;
        //        case 15:
        //            MessageBox.Show("15. Транзакция отклонена ");
        //            break;
        //        case 16:
        //            MessageBox.Show("16. Транзакция уже существует ");
        //            break;

        //        default:
        //            Console.WriteLine(" Неизвестный ответ от процессингового центра ");
        //            break;
        //    }
        //}

        private void client_barcode_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {                
                if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema==3)||(MainStaticClass.GetWorkSchema == 4))
                {
                    MainStaticClass.write_event_in_log(" Перед началом поиска клиента ", "Документ чек", numdoc.ToString());
                    process_client_discount(this.client_barcode.Text);
                }                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void get_old_document_Discount()
        {

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = conn;
                Discount = 0;
                command.CommandText = "SELECT discount_types.discount_percent,clients.code,clients.name  FROM clients left join discount_types ON clients.discount_types_code= discount_types.code WHERE clients.code='" + client_barcode.Tag.ToString() + "'";
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Discount = Convert.ToDouble(reader.GetDecimal(0));
                    Discount = Discount / 100;
                }
                reader.Close();
                conn.Close();
            }
            catch (NpgsqlException)
            {

            }
            catch (Exception)
            {

            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }



        private void enable_print()
        {
            if (MainStaticClass.SystemTaxation < 3)
            {
                checkBox_to_print_repeatedly.Enabled = true;
                checkBox_to_print_repeatedly_p.Enabled = false;
            }
            else
            {
                int _checkBox_to_print_repeatedly_ = 0;
                int _checkBox_to_print_repeatedly_p_ = 0;

                foreach (ListViewItem lvi in listView1.Items)
                {
                    if (lvi.SubItems[14].Text.Trim().Length > 13)
                    {
                        _checkBox_to_print_repeatedly_p_ = 1;//Здесь путаница, печатать не маркировку 
                    }
                    else
                    {
                        _checkBox_to_print_repeatedly_ = 1; //Здесь путаница, печатать маркировку 
                    }
                    if ((_checkBox_to_print_repeatedly_ == 1) && (_checkBox_to_print_repeatedly_p_ == 1))
                    {
                        break;
                    }
                }
                if (_checkBox_to_print_repeatedly_ == 1)
                {
                    checkBox_to_print_repeatedly.Enabled = true;
                }
                if (_checkBox_to_print_repeatedly_p_ == 1)
                {
                    checkBox_to_print_repeatedly_p.Enabled = true;
                }
            }
        }


        public void Cash_check_Load(object sender, System.EventArgs e)
        {
            //System.IO.File.Delete(Application.StartupPath.Replace("\\", "/") + "/" + "json.txt");

            //if (MainStaticClass.GetWorkSchema == 2)
            //{
            //    this.checkBox_club.Visible = true;
            //    btn_change_status_client.Visible = true;
            //    checkBox_viza_d.Visible = true;
            //    checkBox_viza_d.Enabled=false;
            //}
            //else
            //{
            //    this.checkBox_club.Visible = false;
            //}

            if (!MainStaticClass.Use_Fiscall_Print)
            {
                this.label12.Visible = false;
                this.txtB_email_telephone.Visible = false;
            }

            txtB_client_phone.Text = "";
            //btn_change_status_client.Visible = false;            

            this.check_type.Items.Add("Продажа");
            this.check_type.Items.Add("Возврат");
            if (MainStaticClass.Code_right_of_user == 1 && MainStaticClass.PrintingUsingLibraries == 1)
            {
                this.check_type.Items.Add("КоррекцияПродажи");                
            }

            this.WindowState = FormWindowState.Maximized;

            this.num_cash.Text = "КАССА № " + MainStaticClass.CashDeskNumber.ToString();
            this.num_cash.Tag = MainStaticClass.CashDeskNumber;

            // Set the view to show details.
            listView1.View = View.Details;

            listView1.AllowColumnReorder = false;

            // Select the item and subitems when selection is made.
            listView1.FullRowSelect = true;

            // Display grid lines.
            listView1.GridLines = true;

            // Sort the items in the list in ascending order.
            //listView1.Sorting = SortOrder.Ascending;
            listView1.Columns.Add("Код", 90, HorizontalAlignment.Left);
            listView1.Columns.Add("Товар", 450, HorizontalAlignment.Left);
            listView1.Columns.Add("Характеристика", 1, HorizontalAlignment.Left);
            listView1.Columns.Add("Количество", 80, HorizontalAlignment.Right);
            //if (MainStaticClass.SelfServiceKiosk==1)
            //{
            //    listView1.Columns.Add("Цена", 1, HorizontalAlignment.Right);
            //}
            //else
            //{
                listView1.Columns.Add("Цена", 10, HorizontalAlignment.Right);
            //}            
            listView1.Columns.Add("Цена со скидкой", 100, HorizontalAlignment.Right);
            //if (MainStaticClass.SelfServiceKiosk==1)
            //{
            //    listView1.Columns.Add("Сумма", 1, HorizontalAlignment.Right);
            //}
            //else
            //{
                listView1.Columns.Add("Сумма", 10, HorizontalAlignment.Right);
            //}
            listView1.Columns.Add("Сумма со скидкой", 150, HorizontalAlignment.Right);
            listView1.Columns.Add("Акция", 10);
            listView1.Columns.Add("Подарок", 10);
            listView1.Columns.Add("Акция2", 10);
            listView1.Columns.Add("БонусРег", 1);//bonus_standard бонусы, начисленные по стандартным значениям для производителей/марок/товаров
            listView1.Columns.Add("БонусАкц", 1);//bonus_promotion бонусы, начисленные по бонусным акциям
            listView1.Columns.Add("БонусАкцияБ", 1);//promotion_b_mover номер акции, указывается для товаров-инициаторов сработки            
            listView1.Columns.Add("Марка", 100);//promotion_b_recipient номер акции, указывается для товаров, по которым выполнено начисление акционных бонусов

            // Set the view to show details.
            listView_sertificates.View = View.Details;

            listView_sertificates.AllowColumnReorder = false;

            // Select the item and subitems when selection is made.
            listView_sertificates.FullRowSelect = true;

            // Display grid lines.
            listView_sertificates.GridLines = true;

            listView_sertificates.Columns.Add("Код", 100, HorizontalAlignment.Left);
            listView_sertificates.Columns.Add("Сертификат", 400, HorizontalAlignment.Left);
            listView_sertificates.Columns.Add("Номинал", 100, HorizontalAlignment.Left);
            listView_sertificates.Columns.Add("Штрихкод", 100, HorizontalAlignment.Left);

            //Создание таблицы для перераспределения акций
            DataColumn dc = new DataColumn("Code", System.Type.GetType("System.Int32"));
            table.Columns.Add(dc);
            dc = new DataColumn("Tovar", System.Type.GetType("System.String"));
            table.Columns.Add(dc);
            dc = new DataColumn("Characteristic", System.Type.GetType("System.String"));
            table.Columns.Add(dc);
            dc = new DataColumn("CharacteristicGuid", System.Type.GetType("System.String"));
            table.Columns.Add(dc);
            dc = new DataColumn("Quantity", System.Type.GetType("System.Int32"));
            table.Columns.Add(dc);
            dc = new DataColumn("Price", System.Type.GetType("System.Decimal"));
            table.Columns.Add(dc);
            dc = new DataColumn("PriceAtDiscount", System.Type.GetType("System.Decimal"));
            table.Columns.Add(dc);
            dc = new DataColumn("Sum", System.Type.GetType("System.Decimal"));
            table.Columns.Add(dc);
            dc = new DataColumn("SumAtDiscount", System.Type.GetType("System.Decimal"));
            table.Columns.Add(dc);
            dc = new DataColumn("Action", System.Type.GetType("System.Int32"));
            table.Columns.Add(dc);
            dc = new DataColumn("Gift", System.Type.GetType("System.Int32"));
            table.Columns.Add(dc);
            dc = new DataColumn("Action2", System.Type.GetType("System.Int32"));
            table.Columns.Add(dc);
                       
            this.inputbarcode.Focus();

            //Для дублей 

            // Set the view to show details.
            listView2.View = View.Details;

            // Allow the user to edit item text.
            //listView1.LabelEdit = true;           

            // Allow the user to rearrange columns.
            listView2.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            listView2.FullRowSelect = true;

            // Display grid lines.
            listView2.GridLines = true;

            // Sort the items in the list in ascending order.
            //listView1.Sorting = SortOrder.Ascending;
            //listView2.Columns.Add("Код", 100, HorizontalAlignment.Left);
            //listView2.Columns.Add("Товар", SystemInformation.PrimaryMonitorSize.Width-500, HorizontalAlignment.Left);
            listView2.Columns.Add("Характеристика", SystemInformation.PrimaryMonitorSize.Width - 500, HorizontalAlignment.Left);
            listView2.Columns.Add("Цена", 200, HorizontalAlignment.Right);
            //cash.SelectionStart = 0;
            //Здесь получаем признак документ новый или нет

            if (MainStaticClass.GetVersionFn == 1)
            {
                checkBox_print_check.Visible = false;
            }
            checkBox_print_check.Checked = true;

            if (itsnew)
            {
                guid  = Guid.NewGuid().ToString();
                guid1 = Guid.NewGuid().ToString();

                checkBox_to_print_repeatedly.Visible = false;
                label9.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
                label13.Visible = false;
                txtB_non_cash_money.Visible = false;
                txtB_sertificate_money.Visible = false;
                txtB_cash_money.Visible = false;
                txtB_bonus_money.Visible = false;

                inputbarcode.Focus();


                this.date_time_start.Text = "Чек   " + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss");
                this.Discount = 0;
                this.user.Text = MainStaticClass.Cash_Operator;
                this.user.Tag = MainStaticClass.Cash_Operator_Client_Code;//gaa поменять на инн
                numdoc = get_new_number_document();
                this.txtB_num_doc.Text = this.numdoc.ToString();
                MainStaticClass.write_event_in_log(" Ввод нового документа ", "Документ чек", numdoc.ToString());
                this.check_type.SelectedIndex = 0;
                this.check_type.Enabled = true;
                set_sale_disburse_button();
            }
            else
            {
                checkBox_print_check.Enabled = false;                
                //Документ не новый поэтому запретим в нем ввод и изменение                
                last_tovar.Enabled = false;
                txtB_email_telephone.Enabled = false;
                txtB_inn.Enabled = false;
                btn_get_name.Enabled = false;
                txtB_client_phone.Enabled = false;
                txtB_name.Enabled = false;
                comment.Enabled = false;
                //checkBox_club.Enabled = false;
                //Определяем это аварийный документ или нет
                int status = get_its_deleted_document();
                if ((status == 0) || (status == 1))
                {
                    //this.type_pay.Enabled = false;
                    this.label4.Enabled = false;
                    this.check_type.Enabled = false;
                    this.inputbarcode.Enabled = false;
                    this.client_barcode.Enabled = false;
                    //this.sale_cancellation.Enabled = false;
                    //this.inventory.Enabled = false;
                    //this.comment.Enabled = false;
                    to_open_the_written_down_document();
                    enable_print();
                    if (MainStaticClass.Code_right_of_user != 1)
                    {
                        this.pay.Enabled = false;
                    }
                }
                else if (status == 2)
                {
                    itsnew = true;
                    Discount = 0;
                    this.label4.Enabled = true;
                    this.check_type.Enabled = true;
                    this.inputbarcode.Enabled = true;
                    this.client_barcode.Enabled = false;
                    to_open_the_written_down_document();
                    get_old_document_Discount();
                    check_type.Enabled = false;
                    itsnew = true;
                }
            }

            this.Top = 0;
            this.Left = 0;
            this.Size = new System.Drawing.Size(SystemInformation.PrimaryMonitorSize.Width, SystemInformation.PrimaryMonitorSize.Height);
            this.panel2.Left = 0;
            this.listView2.Left = 20;

            this.panel2.Size = new System.Drawing.Size(SystemInformation.PrimaryMonitorSize.Width, SystemInformation.PrimaryMonitorSize.Height / 2);
            this.listView2.Size = new System.Drawing.Size(SystemInformation.PrimaryMonitorSize.Width - 50, SystemInformation.PrimaryMonitorSize.Height / 2 - 50);


            if (itsnew)
            {
                first_start_com_barcode_scaner();
                selection_goods = true;
                inputbarcode.Focus();
                //список допустимых длин qr кодов
                qr_code_lenght.Add(37);
                qr_code_lenght.Add(83);
                qr_code_lenght.Add(127);
                qr_code_lenght.Add(115);
                qr_code_lenght.Add(30);
                qr_code_lenght.Add(32);               
            }
            else
            {
                if (MainStaticClass.Use_Fiscall_Print)
                {
                    if (MainStaticClass.SystemTaxation != 3)
                    {
                        if (this.itc_printed())
                        {
                            this.pay.Enabled = false;
                            this.checkBox_to_print_repeatedly.Enabled = false;
                        }
                    }
                    else if (MainStaticClass.SystemTaxation == 3)
                    {
                        if (this.itc_printed())
                        {                            
                            this.checkBox_to_print_repeatedly.Enabled = false;                            
                        }
                        if (this.itc_printed_p())
                        {                            
                            this.checkBox_to_print_repeatedly_p.Enabled = false;
                        }
                        if (itc_printed() && this.itc_printed_p())
                        {
                            this.pay.Enabled = false;
                        }                        
                    }
                }
            }

            //if (MainStaticClass.SelfServiceKiosk == 0)
            //{
                //btn_del_position.Visible = false;
                //btn_cancel_check.Visible = false;
                //txtB_total_sum.Visible = false;
            //}
            //else
            //{
            //    process_client_discount("9999999999");
            //}
            //if (MainStaticClass.Nick_Shop == "A75")
            //{                
            //    client.Tag = "0000000001";
            //    client.Text = "0000000001";
            //    Discount = 0.05;
            //}
        }

        #region com_barcode_scaner

        private void first_start_com_barcode_scaner()
        {
            if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
            {
                MainStaticClass.continue_to_read_the_data_from_a_port = true;
                rd = new Read_Data_From_Com_Port();
                workerThread = new Thread(rd.to_read_the_data_from_a_port);
                workerThread.Start();
                timer = new System.Windows.Forms.Timer();
                timer.Interval = 400;
                timer.Start();
                timer.Tick += new EventHandler(timer_Tick);
            }
        }

        private void start_com_barcode_scaner()
        {
            if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
            {
                MainStaticClass.continue_to_read_the_data_from_a_port = true;
                rd = new Read_Data_From_Com_Port();
                workerThread = new Thread(rd.to_read_the_data_from_a_port);
                workerThread.Start();
                timer.Start();
            }
        }

        private void stop_com_barcode_scaner()
        {
            if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
            {
                timer.Stop();
                //timer = null;
                MainStaticClass.continue_to_read_the_data_from_a_port = false;//изменяем условие выхода из цикла                 

                /* try
                 {
                     workerThread.Abort();
                 }
                 catch
                 {

                 }*/
                if (rd.mySerial != null)
                    if (rd.mySerial.IsOpen)
                        rd.mySerial.Close();//закрываем COM порт если он открыт  
                workerThread.Join();//прекращаем действие потока  
                rd = null;
                GC.Collect();
            }
        }

        private void restart_com_barcode_scaner()
        {
            if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
            {
                MainStaticClass.continue_to_read_the_data_from_a_port = false;//изменяем условие выхода из цикла                 
                if (rd.mySerial != null)
                    if (rd.mySerial.IsOpen)
                        rd.mySerial.Close();//закрываем COM порт если он открыт                                        
                workerThread.Join();//прекращаем действие потока
                status_com_scaner.Text = "Сканер отвалился"; //Сигнализируем об этом пользователю
                timer.Stop();//Останавливаем таймер чтобы не мельтешил сейчас своими сработками
                //Thread.Sleep(500);//останавливаем текущий поток пауза пусть все завершится
                //Теперь стартуем прослушку по новой
                MainStaticClass.continue_to_read_the_data_from_a_port = true;
                rd = new Read_Data_From_Com_Port();
                workerThread = new Thread(rd.to_read_the_data_from_a_port);
                workerThread.Start();
                timer.Start();
            }
        }

        #endregion


        /*Проверяет есть ли акция с таким шриходом в настоящее время или нет ?
         * если есть возвращает true иначе false
         */
        public bool check_action(string barcode)
        {
            if (barcode.Trim().Length == 0)
            {
                return false;
            }
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            int count_action = 0;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "";
                if (barcode.Trim().Length > 4)
                {
                    query = "SELECT COUNT(*) FROM action_header WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end AND barcode='" + barcode + "'";
                }
                else
                {
                    query = "SELECT COUNT(*) FROM action_header WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end AND promo_code='" + barcode + "'";
                }
                command = new NpgsqlCommand(query, conn);
                count_action = Convert.ToInt32(command.ExecuteScalar());
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при работе с базой данных " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            if (count_action > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //Проверка работоспособности потока слушающего com port
            //if ((DateTime.Now - MainStaticClass.Last_Answer_Barcode_Scaner).TotalSeconds > 1)
            //{
            //    restart_com_barcode_scaner();               
            //}
            //else
            //{
            //    status_com_scaner.Text = "";                
            //}
            if (MainStaticClass.Barcode.Length > 0)
            {
                if (inpun_client_barcode)//Ожидается ввод дисконтной карты клиента
                {

                    client_barcode.Text = MainStaticClass.Barcode;
                    process_client_discount(MainStaticClass.Barcode);
                    inpun_client_barcode = false;

                }
                else if (inpun_action_barcode)//Ожидается ввод акционной дисконтной карты
                {
                    //Проверка есть ли акция с таким штрихкодом 
                    //process_client_discount(MainStaticClass.Barcode);
                    inpun_action_barcode = false;
                    if (check_action(MainStaticClass.Barcode))
                    {
                        if (action_barcode_list.IndexOf(MainStaticClass.Barcode) == -1)
                        {
                            action_barcode_list.Add(MainStaticClass.Barcode);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Акция с таким штрихкодом не найдена");
                    }
                }
                else//Ожидается ввод штрихкода товара
                {
                    find_barcode_or_code_in_tovar(MainStaticClass.Barcode);
                }
                lock (MainStaticClass.Barcode)
                {
                    MainStaticClass.Barcode = "";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //private void cash_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        //{
        //    //MessageBox.Show(cash.SelectionStart.ToString());
        //    if (cash.SelectionStart == 0)
        //    {
        //        e.Handled = true;
        //        return;
        //    }
        //    if (!(Char.IsDigit(e.KeyChar)) && !((e.KeyChar == '.') && (enter_quantity.Text.IndexOf(".") == -1)))
        //    {
        //        if (e.KeyChar != (char)Keys.Back)
        //        {
        //            e.Handled = true;
        //        }
        //        //else
        //        //{

        //        //}
        //    }


        //    //if (e.KeyChar == 13)
        //    //{
        //    //    //calculate_rests();
        //    //    //button2.Focus();
        //    //    //this.listView1.Select();
        //    //    //this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
        //    //}

        //    if (e.Handled == false)
        //    {
        //        if (e.KeyChar != (char)Keys.Back)
        //        {
        //            curpos = cash.SelectionStart;
        //            if ((curpos > 1) || ((curpos == 1) && (cash.Text.Substring(0, 1) != "0")))
        //            {
        //                curpos++;
        //            }
        //        }
        //        else
        //        {
        //            if (cash.SelectionStart != 0)
        //            {
        //                if (cash.Text.Substring(cash.SelectionStart - 1, 1) == ",")
        //                {
        //                    e.Handled = true;
        //                    cash.SelectionStart -= 1;
        //                }
        //                else if ((cash.SelectionStart == 2) && (!e.Handled))
        //                {
        //                    curpos = 1;
        //                }
        //                else
        //                {
        //                    curpos = cash.SelectionStart - 1;
        //                }
        //            }

        //        }
        //    }
        //}


       


        public bool its_sertificate(string code)
        {
            bool result = false;
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = "SELECT COUNT(*) AS qty FROM sertificates where code_tovar = " + code;
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                {
                    result = true;
                }
                command.Dispose();
                conn.Close();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при проверке на сертификат " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке на сертификат " + ex.Message);
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

        /// <summary>
        /// процедура для записи обычного документа
        /// </summary>
        /// <param name="pay"></param>
        /// <param name="sum_doc"></param>
        /// <param name="remainder"></param>
        /// <param name="pay_bonus_many"></param>
        /// <param name="last_rewrite"></param>
        /// <param name="cash_money"></param>
        /// <param name="non_cash_money"></param>
        /// <param name="sertificate_money"></param>
        /// <returns></returns>
        public bool write_new_document(string pay, string sum_doc, string remainder, string pay_bonus_many, bool last_rewrite, string cash_money, string non_cash_money, string sertificate_money, string its_deleted)
        {
            bonuses_it_is_written_off = Convert.ToDecimal(pay_bonus_many);
            bool result = false;

            if (listView1.Items.Count == 0)
            {
                return result;
            }

            double[] sum1=new double[3];
            sum1[0] = 0;
            sum1[1] = 0;
            sum1[2] = 0;

            if (MainStaticClass.SystemTaxation == 3)
            {
                if (last_rewrite)
                {
                    sum1 = get_cash_on_type_payment_3_new(Convert.ToDouble(cash_money.Replace(".", ",")), Convert.ToDouble(non_cash_money.Replace(".", ",")), Convert.ToDouble(sertificate_money.Replace(".", ",")));
                }
            }          

            NpgsqlConnection conn = null;
            NpgsqlTransaction tran = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                tran = conn.BeginTransaction();
                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM checks_header WHERE document_number=@num_doc;", conn);
                NpgsqlParameter npgsqlParameter = new NpgsqlParameter("num_doc", numdoc.ToString());
                command.Parameters.Add(npgsqlParameter);
                command.Transaction = tran;
                command.ExecuteNonQuery();
                command.Parameters.Clear();

                command = new NpgsqlCommand("DELETE FROM checks_table WHERE document_number=@num_doc;", conn);
                npgsqlParameter = new NpgsqlParameter("num_doc", numdoc.ToString());
                command.Parameters.Add(npgsqlParameter);
                command.Transaction = tran;
                command.ExecuteNonQuery();
                command.Parameters.Clear();

                date_time_write = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss");

                command = new NpgsqlCommand("INSERT INTO checks_header(" +
                                        "document_number," +
                                        "date_time_start," +
                                        "client," +
                                        "cash_desk_number," +
                                        "comment," +
                                        "cash," +
                                        "remainder," +
                                        "date_time_write," +
                                        "discount," +
                                        "autor," +
                                        "its_deleted," +
                                        "action_num_doc," +
                                        "check_type," +
                                        "have_action," +
                                        "bonuses_it_is_written_off," +
                                        "is_sent," +
                                        "cash_money," +
                                        "non_cash_money," +
                                        "sertificate_money," +
                                        "id_transaction," +
                                        //"bonus_is_on," +
                                        "its_print," +
                                        "id_transaction_sale," +
                                        "clientInfo_vatin," +
                                        "clientInfo_name," +
                                        "id_sale," +
                                        "sent_to_processing_center,"+
                                        "requisite,"+
                                        "bonuses_it_is_counted,"+
                                        //"viza_d,"+
                                        "id_transaction_terminal," +
                                        "system_taxation,"+
                                        "code_authorization_terminal,"+
                                        "cash_money1,"+
                                        "non_cash_money1,"+
                                        "sertificate_money1,"+
                                        "guid," +
                                        "guid1) VALUES(" +

                                        "@document_number," +
                                        "@date_time_start," +
                                        "@client," +
                                        "@cash_desk_number," +
                                        "@comment," +
                                        "@cash," +
                                        "@remainder," +
                                        "@date_time_write," +
                                        "@discount," +
                                        "@autor," +
                                        "@its_deleted," +
                                        "@action_num_doc," +
                                        "@check_type," +
                                        "@have_action," +
                                        "@bonuses_it_is_written_off," +
                                        "@is_sent," +
                                        "@cash_money," +
                                        "@non_cash_money," +
                                        "@sertificate_money," +
                                        "@id_transaction," +
                                        //"@bonus_is_on," +
                                        "@its_print," +
                                        "@id_transaction_sale," +
                                        "@clientInfo_vatin," +
                                        "@clientInfo_name," +
                                        "@id_sale," +
                                        "@sent_to_processing_center,"+
                                        "@requisite,"+
                                        "@bonuses_it_is_counted,"+
                                        //"@checkBox_viza_d,"+
                                        "@id_transaction_terminal," +
                                        "@system_taxation,"+
                                        "@code_authorization_terminal,"+
                                        "@cash_money1," +
                                        "@non_cash_money1," +
                                        "@sertificate_money1,"+
                                        "@guid," +
                                        "@guid1)", conn);

                command.Parameters.AddWithValue("document_number", numdoc.ToString());
                command.Parameters.AddWithValue("date_time_start", date_time_start.Text.Replace("Чек", ""));
                command.Parameters.AddWithValue("client", client.Tag);
                command.Parameters.AddWithValue("cash_desk_number", num_cash.Tag.ToString());
                command.Parameters.AddWithValue("comment", comment.Text.Trim().Replace("'", ""));
                command.Parameters.AddWithValue("cash", sum_doc.Replace(",", "."));
                command.Parameters.AddWithValue("remainder", remainder.Replace(",", "."));
                command.Parameters.AddWithValue("date_time_write", date_time_write);
                command.Parameters.AddWithValue("discount", calculation_of_the_discount_of_the_document().ToString().Replace(",", "."));
                command.Parameters.AddWithValue("autor", user.Tag.ToString());
                if (its_deleted == "0")
                {
                    command.Parameters.AddWithValue("its_deleted", (last_rewrite ? 0 : 2).ToString());
                }
                else
                {
                    command.Parameters.AddWithValue("its_deleted", its_deleted);
                }
                command.Parameters.AddWithValue("action_num_doc", action_num_doc.ToString());
                command.Parameters.AddWithValue("check_type", check_type.SelectedIndex.ToString());
                command.Parameters.AddWithValue("have_action", have_action.ToString());
                command.Parameters.AddWithValue("bonuses_it_is_written_off", (check_type.SelectedIndex == 1 ? return_bonus.ToString() : pay_bonus_many.ToString().Replace(",", ".")));
                command.Parameters.AddWithValue("is_sent", "0");
                command.Parameters.AddWithValue("cash_money", cash_money.Replace(",", "."));
                command.Parameters.AddWithValue("non_cash_money", non_cash_money.Replace(",", "."));
                command.Parameters.AddWithValue("sertificate_money", sertificate_money);
                command.Parameters.AddWithValue("id_transaction", id_transaction);
                //command.Parameters.AddWithValue("bonus_is_on", bonus_is_on_now.ToString());
                command.Parameters.AddWithValue("its_print", false.ToString());
                command.Parameters.AddWithValue("id_transaction_sale", id_transaction_sale);
                command.Parameters.AddWithValue("clientInfo_vatin", txtB_inn.Text.Trim());
                command.Parameters.AddWithValue("clientInfo_name", txtB_name.Text.Trim());
                command.Parameters.AddWithValue("id_sale", id_sale.ToString());
                //if (checkBox_club.CheckState == CheckState.Checked)
                //{
                //    command.Parameters.AddWithValue("requisite", 1);
                //}
                //else
                //{
                command.Parameters.AddWithValue("requisite", 0);
                //}               
                command.Parameters.AddWithValue("bonuses_it_is_counted", bonuses_it_is_counted.ToString());
                //command.Parameters.AddWithValue("checkBox_viza_d", checkBox_viza_d.Checked ? 1 : 0);
                command.Parameters.AddWithValue("id_transaction_terminal", id_transaction_terminal);
                command.Parameters.AddWithValue("system_taxation", MainStaticClass.SystemTaxation);
                command.Parameters.AddWithValue("code_authorization_terminal", code_authorization_terminal);
                command.Parameters.AddWithValue("cash_money1", sum1[0].ToString().Replace(",", "."));
                command.Parameters.AddWithValue("non_cash_money1", sum1[1].ToString().Replace(",", "."));
                command.Parameters.AddWithValue("sertificate_money1", sum1[2].ToString().Replace(",", "."));
                command.Parameters.AddWithValue("guid", guid);
                command.Parameters.AddWithValue("guid1", guid1);


                string sent_to_processing_center = "0";
                //if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema==3) || (MainStaticClass.GetWorkSchema == 4))
                //{
                //    //Необходимо отделить бонусные документы от дисконтных и те которые дисконтные записакть с sent_to_processing_center = 1
                //    //if (client.Tag != null)//Клиент обязательно должен быть выбран при этом 
                //    //{
                //    //    //if (MainStaticClass.PassPromo != "")
                //    //    //{
                //    //    //    if (check_bonus_is_on())
                //    //    //    {
                //    //    //        sent_to_processing_center = "0";
                //    //    //    }
                //    //    //}
                //    //}
                //    //else
                //    //{
                //    //    sent_to_processing_center = "1";
                //    //}
                //}
                //else if (MainStaticClass.GetWorkSchema == 2)//здесь пока ничего не делаем 
                //{

                //}

                command.Parameters.AddWithValue("sent_to_processing_center", sent_to_processing_center);

                command.Transaction = tran;
                command.ExecuteNonQuery();
                command.Parameters.Clear();

                foreach (ListViewItem lvi in listView1.Items)
                {
                    command = new NpgsqlCommand("INSERT INTO checks_table(" +
                        "document_number," +
                        "tovar_code," +
                        "characteristic," +
                        "quantity," +
                        "price," +
                        "price_at_a_discount," +
                        "sum," +
                        "sum_at_a_discount," +
                        "numstr," +
                        "action_num_doc," +
                        "action_num_doc1," +
                        "action_num_doc2," +
                        "bonus_standard," +
                        "bonus_promotion," +
                        "promotion_b_mover," +
                        "item_marker," +
                        "guid" +
                        ")VALUES(" +
                        "@document_number," +
                        "@tovar_code," +
                        "@characteristic," +
                        "@quantity," +
                        "@price," +
                        "@price_at_a_discount," +
                        "@sum," +
                        "@sum_at_a_discount," +
                        "@numstr," +
                        "@action_num_doc," +
                        "@action_num_doc1," +
                        "@action_num_doc2," +
                        "@bonus_standard," +
                        "@bonus_promotion," +
                        "@promotion_b_mover," +
                        "@item_marker,"+
                        "@guid)", conn);

                    command.Parameters.AddWithValue("document_number", numdoc.ToString());
                    command.Parameters.AddWithValue("tovar_code", lvi.Tag);
                    command.Parameters.AddWithValue("characteristic", (lvi.SubItems[2].Tag == null ? "" : lvi.SubItems[2].Tag.ToString()));
                    command.Parameters.AddWithValue("quantity", lvi.SubItems[3].Text.Replace(",","."));
                    command.Parameters.AddWithValue("price", lvi.SubItems[4].Text.Replace(",", "."));
                    command.Parameters.AddWithValue("price_at_a_discount", lvi.SubItems[5].Text.Replace(",", "."));
                    command.Parameters.AddWithValue("sum", lvi.SubItems[6].Text.Replace(",", "."));
                    command.Parameters.AddWithValue("sum_at_a_discount", lvi.SubItems[7].Text.Replace(",", "."));
                    command.Parameters.AddWithValue("numstr", lvi.Index.ToString());
                    command.Parameters.AddWithValue("action_num_doc", lvi.SubItems[8].Text);
                    command.Parameters.AddWithValue("action_num_doc1", lvi.SubItems[9].Text);
                    command.Parameters.AddWithValue("action_num_doc2", lvi.SubItems[10].Text);
                    command.Parameters.AddWithValue("bonus_standard", lvi.SubItems[11].Text);
                    command.Parameters.AddWithValue("bonus_promotion", lvi.SubItems[12].Text);
                    command.Parameters.AddWithValue("promotion_b_mover", lvi.SubItems[13].Text);
                    command.Parameters.AddWithValue("item_marker", lvi.SubItems[14].Text.Trim().Replace("'", "vasya2021"));
                    command.Parameters.AddWithValue("guid", guid);

                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }


                int num_str_t = listView1.Items.Count - 1;

                //Номер строки во второй табличной части нумеруется начиная с макисмального значения первой тч
                foreach (ListViewItem lvi in listView_sertificates.Items)
                {
                    num_str_t++;

                    command = new NpgsqlCommand("INSERT INTO checks_table(" +
                        " document_number,    " +
                        " tovar_code,         " +
                        " characteristic,     " +
                        " quantity,           " +
                        " price,              " +
                        " price_at_a_discount," +
                        " sum,                " +
                        " sum_at_a_discount,  " +
                        " numstr,             " +
                        " action_num_doc,     " +
                        " action_num_doc1,    " +
                        " action_num_doc2,    " +
                        "item_marker  ,"+
                        "guid)VALUES(" +
                        "@document_number,    " +
                        "@tovar_code,         " +
                        "@characteristic,     " +
                        "@quantity,           " +
                        "@price,              " +
                        "@price_at_a_discount," +
                        "@sum,                " +
                        "@sum_at_a_discount,  " +
                        "@numstr,             " +
                        "@action_num_doc,     " +
                        "@action_num_doc1,    " +
                        "@action_num_doc2,    "+
                        "@item_marker,"+
                        "@guid)", conn);

                    command.Parameters.AddWithValue("document_number", numdoc.ToString());
                    command.Parameters.AddWithValue("tovar_code", lvi.Tag);
                    command.Parameters.AddWithValue("characteristic", "");
                    command.Parameters.AddWithValue("quantity", "1");
                    command.Parameters.AddWithValue("price", "-" + lvi.SubItems[2].Text.Replace(",", "."));
                    command.Parameters.AddWithValue("price_at_a_discount", "-" + lvi.SubItems[2].Text.Replace(",", "."));
                    command.Parameters.AddWithValue("sum", "-" + lvi.SubItems[2].Text.Replace(",", "."));
                    command.Parameters.AddWithValue("sum_at_a_discount", "-" + lvi.SubItems[2].Text.Replace(",", "."));
                    command.Parameters.AddWithValue("numstr", num_str_t.ToString());
                    command.Parameters.AddWithValue("action_num_doc", "0");
                    command.Parameters.AddWithValue("action_num_doc1", "0");
                    command.Parameters.AddWithValue("action_num_doc2", "0");
                    command.Parameters.AddWithValue("item_marker", lvi.SubItems[3].Text);
                    command.Parameters.AddWithValue("guid", guid);

                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();

                    //Добавим строки обновим статус сертификата в локальной базе                    

                    command = new NpgsqlCommand("UPDATE sertificates   SET  is_active = 0 WHERE code_tovar = @tovar_code", conn);
                    command.Parameters.AddWithValue("tovar_code", lvi.Tag.ToString());
                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }

                tran.Commit();
                conn.Close();

                if (last_rewrite)
                {
                    itsnew = false;
                    if (this.check_type.SelectedIndex == 0)
                    {
                        SendDataToCustomerScreen(0, 0, 0);
                    }
                }
                else
                {
                    itsnew = true;
                    if (this.check_type.SelectedIndex == 0)
                    {
                        SendDataToCustomerScreen(1, 0, 1);
                    }
                }
                if (its_deleted == "1")
                {
                    if (this.check_type.SelectedIndex == 0)
                    {
                        SendDataToCustomerScreen(0, 0, 0);
                    }
                }
                result = true;
            }
            catch (NpgsqlException ex)
            {
                if (tran != null)
                {
                    tran.Rollback();
                }

                MessageBox.Show("Ошибка при записи документа " + ex.Message);
                result = false;
            }
            catch (Exception ex)
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
                MessageBox.Show("Ошибка при записи документа " + ex.Message);
                result = false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            if (result)
            {
                MainStaticClass.Last_Write_Check = DateTime.Now;
            }
            return result;
        }
               
        public bool it_is_paid(string pay, string sum_doc, string remainder, string pay_bonus_many, bool last_rewrite, string cash_money, string non_cash_money, string sertificate_money)
        {
            //Здесь необходимо добавить проверку на то что документ уже не новый
            bool result = true;

            if (itsnew)
            {
                MainStaticClass.write_event_in_log(" Финальная запись документа ", "Документ чек", numdoc.ToString());
                result = write_new_document(pay, sum_doc, remainder, pay_bonus_many, last_rewrite, cash_money, non_cash_money, sertificate_money, "0");
                if (result)
                {
                    if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
                    {
                        if (workerThread != null)//При нажатии клавиши ESC уже могло все завершится
                        {
                            stop_com_barcode_scaner();
                            this.timer.Stop();
                            this.timer = null;
                            workerThread = null;
                            rd = null;
                            GC.Collect();
                        }
                    }
                }
            }

            if (result)
            {
                //if (MainStaticClass.Use_Text_Print)
                //{
                //    try
                //    {
                //        text_print(pay, sum_doc, remainder, cash_money,  non_cash_money, sertificate_money);
                //    }
                //    catch { }

                //}
                if (MainStaticClass.Use_Fiscall_Print)
                {
                    MainStaticClass.write_event_in_log("Попытка распечатать чек ", "Документ чек", numdoc.ToString());
                    fiscall_print_pay(pay);
                }
            }

            return result;

            ////Проверить что оплачено правильно

            ////if ((Convert.ToDecimal(this.remainder.Text.Replace(".",",")) < 0)||(Convert.ToDecimal(this.cash.Text.Replace(".",","))==0))
            ////{
            ////    MessageBox.Show("Документ не оплачен");
            ////    return;
            ////}
            // NpgsqlConnection conn = null;

            //try
            //{
            //    conn = MainStaticClass.NpgsqlConn();
            //    conn.Open();
            //    NpgsqlCommand command = new NpgsqlCommand();
            //    //string numdoc = get_new_number_document().ToString();
            //    //command.Connection = conn;
            //    date_time_write = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss");
            //    command.CommandText = "INSERT INTO checks_header(document_number,   date_time_start           ,client,cash_desk_number,comment,cash,remainder,date_time_write,discount,autor,its_deleted,inventory,check_type)" +
            //                                              "VALUES(" + numdoc.ToString() + ",'" + date_time_start.Text.Replace("Чек", "") + "','" + client.Tag + "'," + num_cash.Tag + ",'" + comment.Text + "','" + sum_doc.Replace(",", ".") + "','" + remainder.Replace(",", ".") + "','" + date_time_write + "'," + calculation_of_the_discount_of_the_document().ToString().Replace(",", ".") + ",'" + user.Tag + "',0,'" + get_inventory() + "',"+this.check_type.SelectedIndex.ToString()+")";
            //    //                                          //"VALUES(" + numdoc + ",'" + date_time_start.Text + "','" + client.Tag + "'," + num_cash.Tag + ",'" + comment.Text + "'," + sum_doc.Text + "," + remainder.Text + ",'" + DateTime.Now.ToString() + "')";
            //    command.Connection = conn;
            //    command.ExecuteNonQuery();

            //    command.CommandText = "";

            //    ////теперь табличная часть
            //    foreach (ListViewItem lvi in listView1.Items)
            //    {
            //        command.CommandText += "INSERT INTO checks_table(document_number,tovar_code,quantity,price,price_at_a_discount, sum,sum_at_a_discount,numstr,action_num_doc,action_num_doc1,action_num_doc2)VALUES(" +
            //            numdoc.ToString() + "," + lvi.Tag + "," + lvi.SubItems[2].Text + ",'" + lvi.SubItems[3].Text.Replace(",", ".") + "','" + lvi.SubItems[4].Text.Replace(",", ".") +
            //            "','" + lvi.SubItems[5].Text.Replace(",", ".") + "','" + lvi.SubItems[6].Text.Replace(",", ".") + "'," + lvi.Index.ToString() + "," + lvi.SubItems[7].Text + "," + lvi.SubItems[8].Text + "," + lvi.SubItems[9].Text + ");";
            //    }
            //    command.ExecuteNonQuery();
            //    conn.Close();
            //    itsnew = false;
            //    //this.Close();
            //    if (MainStaticClass.Use_Text_Print)
            //    {
            //        text_print(pay, sum_doc, remainder);
            //    }
            //    else if (MainStaticClass.Use_Fiscall_Print)
            //    {
            //        fiscall_print_pay(pay);
            //    }

            //}
            //catch (NpgsqlException ex)
            //{
            //    MessageBox.Show("Ошибка при записи документа " + ex.Message + " | " + ex.Detail);
            //    return false;
            //}
            //finally
            //{
            //    if (conn.State == ConnectionState.Open)
            //    {
            //        conn.Close();
            //    }
            //}

            ////Печатаем чек
            ////PrintDocument pd = new PrintDocument();
            //////pd/printpag
            //////PrintDocument pd = new PrintDocument();
            ////pd.PrintController = new StandardPrintController();
            //////pd.PrinterSettings.PrinterName = "Posiflex PP6800 Partial Cut v3.01";
            ////pd.PrinterSettings.PrinterName = PrinterSettings.InstalledPrinters[0];// "DefaultPrint";
            //////pd.PrintPage += new PrintPageEventHandler
            //////   (this.printDocument1_PrintPage);
            ////pd.Print();

            //return true;
        }

        private int get_tovar_nds(string code)
        {
            int result = 0;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT nds  FROM tovar WHERE code=" + code;
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToInt32(command.ExecuteScalar());
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при получении значения ставки ндс " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении значения ставки ндс " + ex.Message);
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


        /// <summary>
        /// Получение сумм по типам оплаты
        /// 
        /// </summary>
        /// <returns></returns>
        private double[] get_cash_on_type_payment()
        {
            double[] result = new double[3];
            result[0] = 0;
            result[1] = 0;
            result[2] = 0;
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT cash_money, non_cash_money, sertificate_money  FROM checks_header WHERE document_number=" + numdoc;
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result[0] = Convert.ToDouble(reader.GetDecimal(0));
                    result[1] = Convert.ToDouble(reader.GetDecimal(1));
                    result[2] = Convert.ToDouble(reader.GetDecimal(2));
                }
                reader.Close();
                command.Dispose();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Произошли ошибки при получении сумм по типам оплаты" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошли ошибки при получении сумм по типам оплаты" + ex.Message);
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


        /// <summary>
        /// Получение сумм по типам оплаты
        /// для 3 типа налогообложения
        /// расчет сумм идет перед записью и храниться в базе 
        /// возвращаются суммы по формам оплаты только для 2 чека
        /// </summary>
        /// <returns></returns>
        private double[] get_cash_on_type_payment_3_new(double sum_cash,double sum_non_cashe,double sum_sertificate)
        {
            double[] result = new double[3];
            result[0] = sum_cash;
            result[1] = sum_non_cashe;
            result[2] = sum_sertificate;

            double[] result_variant_1 = new double[3];
            result_variant_1[0] = sum_cash;
            result_variant_1[1] = sum_non_cashe;
            result_variant_1[2] = sum_sertificate;
        
            try
            {                
                double sum_print = 0;
                foreach (ListViewItem lvi in listView1.Items)
                {
                    if (lvi.SubItems[14].Text.Trim().Length < 14)
                    {
                        sum_print += Convert.ToDouble(lvi.SubItems[7].Text.Trim().Replace(".",","));
                    }
                }
                
          
                if (result[0] > 0)
                {
                    if (result[0] >= sum_print)
                    {
                        result[0] = sum_print;
                        sum_print = 0;//Вся сумма распределеилась
                        result[1] = 0;
                        result[2] = 0;
                    }

                    if (result[0] < sum_print)
                    {
                        sum_print = Math.Round(sum_print - result[0], 2, MidpointRounding.ToEven);
                    }
                }

                if (result[1] > 0)
                {
                    if (result[1] >= sum_print)
                    {
                        result[1] = sum_print;
                        sum_print = 0;//Вся сумма распределеилась
                        result[2] = 0;
                    }

                    if (result[1] < sum_print)
                    {
                        sum_print = Math.Round(sum_print - result[1], 2, MidpointRounding.ToEven);
                    }
                }
                if (result[2] > 0)
                {
                    if (result[2] >= sum_print)
                    {
                        result[2] = sum_print;
                        sum_print = 0;//Вся сумма распределеилась
                    }

                    if (result[2] < sum_print)
                    {
                        sum_print = Math.Round(sum_print - result[2], 2, MidpointRounding.ToEven);
                    }
                }

                result_variant_1[0] = Math.Round(result_variant_1[0] - result[0], 2, MidpointRounding.ToEven);
                result_variant_1[1] = Math.Round(result_variant_1[1] - result[1], 2, MidpointRounding.ToEven);
                result_variant_1[2] = Math.Round(result_variant_1[2] - result[2], 2, MidpointRounding.ToEven);

                result[0] = result_variant_1[0];
                result[1] = result_variant_1[1];
                result[2] = result_variant_1[2];

                sum_print = 0;
                foreach (ListViewItem lvi in listView1.Items)
                {
                    if (lvi.SubItems[14].Text.Trim().Length > 13)
                    {
                        sum_print += Convert.ToDouble(lvi.SubItems[7].Text.Trim().Replace(".", ","));
                    }
                }

                if (result[0] > 0)
                {
                    if (result[0] >= sum_print)
                    {
                        result[0] = sum_print;
                        sum_print = 0;//Вся сумма распределеилась
                        result[1] = 0;
                        result[2] = 0;
                    }

                    if (result[0] < sum_print)
                    {
                        sum_print = Math.Round(sum_print - result[0], 2, MidpointRounding.ToEven);
                    }
                }

                if (result[1] > 0)
                {
                    if (result[1] >= sum_print)
                    {
                        result[1] = sum_print;
                        sum_print = 0;//Вся сумма распределеилась
                        result[2] = 0;
                    }

                    if (result[1] < sum_print)
                    {
                        sum_print = Math.Round(sum_print - result[1], 2, MidpointRounding.ToEven);
                    }
                }
                if (result[2] > 0)
                {
                    if (result[2] >= sum_print)
                    {
                        result[2] = sum_print;
                        sum_print = 0;//Вся сумма распределеилась
                    }

                    if (result[2] < sum_print)
                    {
                        sum_print = Math.Round(sum_print - result[2], 2, MidpointRounding.ToEven);
                    }
                }                
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Произошли ошибки при получении сумм по типам оплаты" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошли ошибки при получении сумм по типам оплаты" + ex.Message);
            }
            finally
            {
                //if (conn.State == ConnectionState.Open)
                //{
                //    conn.Close();
                //}
                //else
                //{
                //    conn.Dispose();
                //}
            }

            return result;
        }

        ///// <summary>
        ///// Получение сумм по типам оплаты
        ///// для 3 типа налогообложения
        ///// возвращает суммы только для первого чека
        ///// </summary>
        ///// <returns></returns>
        //private double[] get_cash_on_type_payment_3_1()
        //{
        //    double[] result = new double[3];
        //    result[0] = 0;
        //    result[1] = 0;
        //    result[2] = 0;

        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //    NpgsqlCommand command = null;
        //    try
        //    {
        //        conn.Open();
        //        string query = "";                               
        //            query = "SELECT SUM(cash_money - cash_money1),SUM(non_cash_money - non_cash_money1),SUM(sertificate_money - sertificate_money1) FROM checks_header WHERE document_number=" + numdoc;
        //            command = new NpgsqlCommand(query, conn);
        //        NpgsqlDataReader reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            result[0] = Convert.ToDouble(reader[0]);
        //            result[1] = Convert.ToDouble(reader[1]);
        //            result[2] = Convert.ToDouble(reader[2]);
        //        }
        //        reader.Close();                

        //        command.Dispose();
        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show("Произошли ошибки при получении сумм по типам оплаты" + ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Произошли ошибки при получении сумм по типам оплаты" + ex.Message);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //        else
        //        {
        //            conn.Dispose();
        //        }
        //    }

        //    return result;
        //}


        /// <summary>
        /// Получение сумм по типам оплаты
        /// для 3 типа налогообложения
        /// 
        /// </summary>
        /// <returns></returns>
        private double[] get_cash_on_type_payment_3(int variant,double[] result_variant_1)
        {
            double[] result = new double[3];
            result[0] = 0;
            result[1] = 0;
            result[2] = 0;
         
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            NpgsqlCommand command = null;
            try
            {
                conn.Open();
                string query = "";

                result[0] = result_variant_1[0];
                result[1] = result_variant_1[1];
                result[2] = result_variant_1[2];

                if (variant == 0)//печать без кодов маркировки 
                {
                    query = "SELECT SUM(sum_at_a_discount) FROM public.checks_table WHERE document_number=" + numdoc + " AND length(item_marker)<14 AND sum_at_a_discount>0";
                }
                else //1 вариант это только маркировка
                {
                    query = "SELECT SUM(sum_at_a_discount) FROM public.checks_table WHERE document_number=" + numdoc + " AND length(item_marker)>13 AND sum_at_a_discount>0 ";
                }

                command = new NpgsqlCommand(query, conn);
                double sum_print = Math.Round(Convert.ToDouble(command.ExecuteScalar()), 2, MidpointRounding.ToEven);
                if (result[0] > 0)
                {
                    if (result[0] >= sum_print)
                    {
                        result[0] = sum_print;
                        sum_print = 0;//Вся сумма распределеилась
                        result[1] = 0;
                        result[2] = 0;
                    }

                    if (result[0] < sum_print)
                    {
                        sum_print = Math.Round(sum_print - result[0], 2, MidpointRounding.ToEven);
                    }
                }

                if (result[1] > 0)
                {
                    if (result[1] >= sum_print)
                    {
                        result[1] = sum_print;
                        sum_print = 0;//Вся сумма распределеилась
                        result[2] = 0;
                    }

                    if (result[1] < sum_print)
                    {
                        sum_print = Math.Round(sum_print - result[1], 2, MidpointRounding.ToEven);
                    }
                }
                if (result[2] > 0)
                {
                    if (result[2] >= sum_print)
                    {
                        result[2] = sum_print;
                        sum_print = 0;//Вся сумма распределеилась
                    }

                    if (result[2] < sum_print)
                    {
                        sum_print = Math.Round(sum_print - result[2], 2, MidpointRounding.ToEven);
                    }
                }

                result_variant_1[0] = Math.Round(result_variant_1[0] - result[0], 2, MidpointRounding.ToEven);
                result_variant_1[1] = Math.Round(result_variant_1[1] - result[1], 2, MidpointRounding.ToEven);
                result_variant_1[2] = Math.Round(result_variant_1[2] - result[2], 2, MidpointRounding.ToEven);

                command.Dispose();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Произошли ошибки при получении сумм по типам оплаты" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошли ошибки при получении сумм по типам оплаты" + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                else
                {
                    conn.Dispose();
                }
            }

            return result;
        }

        private void print_fiscal_advertisement(FiscallPrintJason.Check check, FiscallPrintJason.PostItem pi)
        {


            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            //string s2 = ""; int length = 0;
            bool first_string = true;
            try
            {
                conn.Open();
                string query = "SELECT advertisement_text  FROM advertisement order by num_str";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (first_string)
                    {
                        first_string = false;
                        pi = new FiscallPrintJason.PostItem();
                        pi.type = "text";
                        pi.text = "*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*";
                        pi.alignment = "left";
                        check.postItems.Add(pi);
                    }
                    pi = new FiscallPrintJason.PostItem();
                    pi.type = "text";
                    pi.text = reader["advertisement_text"].ToString();
                    pi.alignment = "left";
                    check.postItems.Add(pi);
                }
                if (!first_string)
                {
                    pi = new FiscallPrintJason.PostItem();
                    pi.type = "text";
                    pi.text = "*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*";
                    pi.alignment = "left";
                    check.postItems.Add(pi);
                }
                reader.Close();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибки при выводе рекламного текста " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибки при выводе рекламного текста " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        private void print_fiscal_advertisement2(FiscallPrintJason2.Check check, FiscallPrintJason2.PostItem pi)
        {


            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            //string s2 = ""; int length = 0;
            bool first_string = true;
            try
            {
                conn.Open();
                string query = "SELECT advertisement_text  FROM advertisement order by num_str";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (first_string)
                    {
                        first_string = false;
                        pi = new FiscallPrintJason2.PostItem();
                        pi.type = "text";
                        pi.text = "*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*";
                        pi.alignment = "left";
                        check.postItems.Add(pi);
                    }
                    pi = new FiscallPrintJason2.PostItem();
                    pi.type = "text";
                    pi.text = reader["advertisement_text"].ToString();
                    pi.alignment = "left";
                    check.postItems.Add(pi);
                }
                if (!first_string)
                {
                    pi = new FiscallPrintJason2.PostItem();
                    pi.type = "text";
                    pi.text = "*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*";
                    pi.alignment = "left";
                    check.postItems.Add(pi);
                }
                reader.Close();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибки при выводе рекламного текста " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибки при выводе рекламного текста " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        //private void print_not_fiscal_advertisement(StringBuilder sb)
        //{
        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //    bool first_string = true;

        //    try
        //    {
        //        conn.Open();
        //        string query = "SELECT advertisement_text  FROM advertisement order by num_str";
        //        NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //        NpgsqlDataReader reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            if (first_string)
        //            {
        //                sb.Append("- - - - - - - - - - - - - - - - - -" + "\r\n");                                            
        //            }
        //            sb.Append(reader["advertisement_text"].ToString() + "\r\n");                    
        //        }
        //        if (!first_string)
        //        {
        //            sb.Append("- - - - - - - - - - - - - - - - - -" + "\r\n");
        //        }
        //        reader.Close();
        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show("Ошибки при выводе рекламного текста " + ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Ошибки при выводе рекламного текста " + ex.Message);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }

        //}

        //Регистрация кодов марикровки перед печатью чека
        /// <summary>
        /// Проверка кодов маркировки + добавление кодов в буфер
        /// </summary>
        private bool km_adding_to_buffer(int num_pos)
        {
            bool result = true;
            AddingKmArrayToTableTested addingKmArrayToTableTestedKm = new AddingKmArrayToTableTested();
            addingKmArrayToTableTestedKm.type = "addMarksToBuffer";            
            addingKmArrayToTableTestedKm.@params = new List<AddingKmArrayToTableTested.Param>();            
            
            //ListViewItem lvi = listView1.Items[num_pos];
            
            MainStaticClass.write_event_in_log(" Принудительно добавляем лог маркировки " + num_pos.ToString(), "Документ чек", numdoc.ToString());            

            
            int num = 0;
            foreach (ListViewItem lvi in listView1.Items)
            {
                if (lvi.SubItems[14].Text.Trim().Length > 13)
                {
                    if (num_pos != num)
                    {
                        num++;
                        continue;
                    }
                    num++;

                    AddingKmArrayToTableTested.Param param = new AddingKmArrayToTableTested.Param();
                    param.imcType = "auto";

                    //string GS1 = Char.ConvertFromUtf32(29);
                    //if ((lvi.SubItems[14].Text.Trim().Length == 83) || (lvi.SubItems[14].Text.Trim().Length == 127) || (lvi.SubItems[14].Text.Trim().Length == 115))
                    //{
                    //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(31, GS1);
                    //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(38, GS1);
                    //}

                    //if (lvi.SubItems[14].Text.Trim().Length == 37 && lvi.SubItems[14].Text.Substring(16, 2) == "21")
                    //{
                    //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(31, GS1);
                    //}

                    //if (lvi.SubItems[14].Text.Trim().Length == 30)
                    //{
                    //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(24, GS1);
                    //}

                    //if (lvi.SubItems[14].Text.Trim().Length == 32)
                    //{
                    //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(26, GS1);
                    //}

                    lvi.SubItems[14].Text = add_gs1(lvi.SubItems[14].Text);

                    byte[] textAsBytes = Encoding.Default.GetBytes(lvi.SubItems[14].Text.Trim());
                    //string mark_str = Convert.ToBase64String(textAsBytes);
                    //param.imc = mark_str;
                    param.imc = Convert.ToBase64String(textAsBytes);
                    param.itemEstimatedStatus = "itemPieceSold";
                    param.imcModeProcessing = 0;
                    addingKmArrayToTableTestedKm.@params.Add(param);
                }
            }
            
            try
            {
                AnswerAddingKmArrayToTableTested answerAddingKmArrayToTableTested = Tested_Km(addingKmArrayToTableTestedKm);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = false;
                MainStaticClass.write_event_in_log(ex.Message + "km_adding_to_buffer", "Документ чек", numdoc.ToString());
            }
            return result;
        }

        //Регистрация кодов марикровки перед печатью чека
        /// <summary>
        /// Проверка кодов маркировки + добавление кодов в буфер
        /// </summary>
        private bool km_adding_to_buffer_index(int num_pos)
        {
            bool result = true;
            AddingKmArrayToTableTested addingKmArrayToTableTestedKm = new AddingKmArrayToTableTested();
            addingKmArrayToTableTestedKm.type = "addMarksToBuffer";
            addingKmArrayToTableTestedKm.@params = new List<AddingKmArrayToTableTested.Param>();

            ListViewItem lvi = listView1.Items[num_pos];

            MainStaticClass.write_event_in_log(" Принудительно добавляем маркированный товар " + num_pos.ToString(), "Документ чек", numdoc.ToString());
            AddingKmArrayToTableTested.Param param = new AddingKmArrayToTableTested.Param();
            param.imcType = "auto";

            //string GS1 = Char.ConvertFromUtf32(29);
            //if ((lvi.SubItems[14].Text.Trim().Length == 83) || (lvi.SubItems[14].Text.Trim().Length == 127) || (lvi.SubItems[14].Text.Trim().Length == 115))
            //{
            //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(31, GS1);
            //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(38, GS1);
            //}

            //if (lvi.SubItems[14].Text.Trim().Length == 37 && lvi.SubItems[14].Text.Substring(16, 2) == "21")
            //{
            //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(31, GS1);
            //}

            //if (lvi.SubItems[14].Text.Trim().Length == 30)
            //{
            //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(24, GS1);
            //}

            //if (lvi.SubItems[14].Text.Trim().Length == 32)
            //{
            //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(26, GS1);
            //}

            lvi.SubItems[14].Text = add_gs1(lvi.SubItems[14].Text);
            byte[] textAsBytes = Encoding.Default.GetBytes(lvi.SubItems[14].Text.Trim());
            //string mark_str = Convert.ToBase64String(textAsBytes);
            //param.imc = mark_str;
            param.imc = Convert.ToBase64String(textAsBytes);
            param.itemEstimatedStatus = "itemPieceSold";
            param.imcModeProcessing = 0;
            addingKmArrayToTableTestedKm.@params.Add(param);

            try
            {
                AnswerAddingKmArrayToTableTested answerAddingKmArrayToTableTested = Tested_Km(addingKmArrayToTableTestedKm);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = false;
                MainStaticClass.write_event_in_log(ex.Message + "km_adding_to_buffer", "Документ чек", numdoc.ToString());
            }
            return result;
        }


        //Регистрация кодов марикровки перед печатью чека
        /// <summary>
        /// Проверка кодов маркировки + добавление кодов в буфер
        /// </summary>
        private int checking_km_before_adding_to_buffer()
        {
            int result = 0;
            AddingKmArrayToTableTested addingKmArrayToTableTestedKm = new AddingKmArrayToTableTested();
            addingKmArrayToTableTestedKm.type = "validateMarks";
            addingKmArrayToTableTestedKm.timeout = 6000;
            addingKmArrayToTableTestedKm.@params = new List<AddingKmArrayToTableTested.Param>();
            var num_strt_km = new Dictionary<int, int>();
            //int num = 0;
            foreach (ListViewItem lvi in listView1.Items)
            {
                if (lvi.SubItems[14].Text.Trim().Length > 13)
                {
                    num_strt_km.Add(num_strt_km.Count, lvi.Index + 1);
                    AddingKmArrayToTableTested.Param param = new AddingKmArrayToTableTested.Param();
                    param.imcType = "auto";

                    //string GS1 = Char.ConvertFromUtf32(29);
                    //if ((lvi.SubItems[14].Text.Trim().Length == 83)  ||
                    //    (lvi.SubItems[14].Text.Trim().Length == 127) || 
                    //    (lvi.SubItems[14].Text.Trim().Length == 115)) 

                    //{
                    //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(31, GS1);
                    //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(38, GS1);
                    //}

                    //if (lvi.SubItems[14].Text.Trim().Length == 37 && lvi.SubItems[14].Text.Substring(16, 2) == "21")
                    //{
                    //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(31, GS1);
                    //}

                    //if (lvi.SubItems[14].Text.Trim().Length == 30)
                    //{
                    //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(24, GS1);
                    //}

                    //if (lvi.SubItems[14].Text.Trim().Length == 32)
                    //{
                    //    lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(26, GS1);
                    //}
                    lvi.SubItems[14].Text = add_gs1(lvi.SubItems[14].Text);

                    byte[] textAsBytes = Encoding.Default.GetBytes(lvi.SubItems[14].Text.Trim().Replace("vasya2021", "'"));
                    //string mark_str = Convert.ToBase64String(textAsBytes);
                    //param.imc = mark_str;
                    param.imc = Convert.ToBase64String(textAsBytes);
                    param.itemEstimatedStatus = "itemPieceSold";
                    param.imcModeProcessing = 0;
                    addingKmArrayToTableTestedKm.@params.Add(param);
                }
            }
                        
            try
            {
                AnswerAddingKmArrayToTableTested answerAddingKmArrayToTableTested = Tested_Km(addingKmArrayToTableTestedKm);
                if (answerAddingKmArrayToTableTested.results[0].status != "ready")
                {
                    MessageBox.Show("Не удалось получить корректный ответ от ФР в течении 48 секунд, можете попробовать еще раз.");
                    MainStaticClass.write_event_in_log("Не удалось получить корректный ответ от ФР в течении 48 секунд, можете попробовать еще раз.", "Документ чек", numdoc.ToString());
                }             

                int x = 0;
                while (x < answerAddingKmArrayToTableTested.results[0].result.Count)
                {                   
                    if (answerAddingKmArrayToTableTested.results[0].result[x].onlineValidation != null)
                    {                        
                        if (answerAddingKmArrayToTableTested.results[0].result[x].driverError.code == 0)
                        {
                            if (!answerAddingKmArrayToTableTested.results[0].result[x].onlineValidation.itemInfoCheckResult.imcCheckResult)
                                {
                                    if (answerAddingKmArrayToTableTested.results[0].result[x].onlineValidation.markOperatorResponseResult != "correct")
                                    {
                                        if (answerAddingKmArrayToTableTested.results[0].result[x].onlineValidation.markOperatorResponseResult == "incorrect")
                                        {
                                            MessageBox.Show("Запрос имеет некорректный формат(incorrect) в строке = " + num_strt_km[x].ToString());
                                            MainStaticClass.write_event_in_log("Запрос имеет некорректный формат(incorrect) в строке = " + num_strt_km[x].ToString(), "Документ чек", numdoc.ToString());
                                            result = -1;
                                        }
                                        if (answerAddingKmArrayToTableTested.results[0].result[x].onlineValidation.markOperatorResponseResult == "unrecognized")
                                        {
                                            MessageBox.Show("КМ имеет некорретный формат(unrecognized) в строке = " + num_strt_km[x].ToString());
                                            MainStaticClass.write_event_in_log("КМ имеет некорретный формат(unrecognized) в строке = " + num_strt_km[x].ToString(), "Документ чек", numdoc.ToString());
                                            result = -1;//попробовать вызвать еще раз с км с измененным регистром
                                        }
                                    }
                                }                              
                        }
                        else
                        {
                            result = answerAddingKmArrayToTableTested.results[0].result[x].driverError.code;
                            string description = "";
                            if (answerAddingKmArrayToTableTested.results[0].result[x].driverError.error != null)
                            {
                                description += answerAddingKmArrayToTableTested.results[0].result[x].driverError.error;
                            }
                            if (answerAddingKmArrayToTableTested.results[0].result[x].driverError.description != null)
                            {
                                description += answerAddingKmArrayToTableTested.results[0].result[x].driverError.description;
                            }                            

                            if ((result == 421) || (result == 402))
                            {
                                //MessageBox.Show("Ошибка при проверке кодов маркировки код ошибки " + answerAddingKmArrayToTableTested.results[0].result[x].driverError.code.ToString() + " " + description);
                                km_adding_to_buffer(x);
                            }
                            else
                            {
                                MessageBox.Show("Ошибка при проверке кодов маркировки код ошибки " + answerAddingKmArrayToTableTested.results[0].result[x].driverError.code.ToString() + " " + description);
                                MainStaticClass.write_event_in_log("Ошибка при проверке кодов маркировки код ошибки " + answerAddingKmArrayToTableTested.results[0].result[x].driverError.code.ToString() + " " + description, "Документ чек", numdoc.ToString());
                            }
                        }
                    }
                    else
                    {
                        
                        result = answerAddingKmArrayToTableTested.results[0].result[x].driverError.code;
                        if ((result == 421) || (result == 402))
                        {
                            km_adding_to_buffer(x);
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при проверке кодов маркировки код ошибки " + answerAddingKmArrayToTableTested.results[0].result[x].driverError.code + " " + answerAddingKmArrayToTableTested.results[0].result[x].driverError.description);
                            MainStaticClass.write_event_in_log("Ошибка при проверке кодов маркировки код ошибки " + answerAddingKmArrayToTableTested.results[0].result[x].driverError.code + " " + answerAddingKmArrayToTableTested.results[0].result[x].driverError.description, "Документ чек", numdoc.ToString());
                        }                        
                    }                 
                    x++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = -1;
            }
            return result;
        }






        //Регистрация кодов марикровки перед печатью чека
        /// <summary>
        /// Проверка кодов маркировки + добавление кодов в буфер
        /// </summary>
        private bool checking_km_before_adding_to_buffer(ref AnswerAddingKmArrayToTableTested answerAddingKmArrayToTableTested)
        {
            bool result = true;
            AddingKmArrayToTableTested addingKmArrayToTableTestedKm = new AddingKmArrayToTableTested();
            addingKmArrayToTableTestedKm.type = "validateMarks";
            addingKmArrayToTableTestedKm.timeout = 6000;
            addingKmArrayToTableTestedKm.@params = new List<AddingKmArrayToTableTested.Param>();
            var num_strt_km = new Dictionary<int, int>();
            //int num = 0;
            foreach (ListViewItem lvi in listView1.Items)
            {
                if (lvi.SubItems[14].Text.Trim().Length > 13)
                {
                    num_strt_km.Add(num_strt_km.Count, lvi.Index + 1);
                    AddingKmArrayToTableTested.Param param = new AddingKmArrayToTableTested.Param();
                    param.imcType = "auto";

                    //string GS1 = Char.ConvertFromUtf32(29);                    
                    //if ((lvi.SubItems[14].Text.Trim().Length == 83) || (lvi.SubItems[14].Text.Trim().Length == 127) || (lvi.SubItems[14].Text.Trim().Length == 115))
                    // { 
                    //     lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(31, GS1);
                    //     lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(38, GS1);
                    // }

                    // if (lvi.SubItems[14].Text.Trim().Length == 37 && lvi.SubItems[14].Text.Substring(16, 2) == "21")
                    // {
                    //     lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(31, GS1);
                    // }

                    // if (lvi.SubItems[14].Text.Trim().Length == 30)
                    // {
                    //     lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(24, GS1);
                    // }

                    // if (lvi.SubItems[14].Text.Trim().Length == 32)
                    // {
                    //     lvi.SubItems[14].Text = lvi.SubItems[14].Text.Insert(26, GS1);
                    // }
                    lvi.SubItems[14].Text = add_gs1(lvi.SubItems[14].Text);

                    byte[] textAsBytes = Encoding.Default.GetBytes(lvi.SubItems[14].Text.Trim());
                    //string mark_str = Convert.ToBase64String(textAsBytes);
                    //param.imc = mark_str;
                    param.imc = Convert.ToBase64String(textAsBytes);
                    param.itemEstimatedStatus = "itemPieceSold";
                    param.imcModeProcessing = 0;
                    addingKmArrayToTableTestedKm.@params.Add(param);
                }
            }

            //MessageBox.Show("1");
            try
            {
                answerAddingKmArrayToTableTested = Tested_Km(addingKmArrayToTableTestedKm);
                if (answerAddingKmArrayToTableTested.results[0].status != "ready")
                {
                    MessageBox.Show("Не удалось получить корректный ответ от ФР в течении 48 секунд, можете попробовать еще раз.");
                }
                if (answerAddingKmArrayToTableTested.results[0].errorCode == 0)
                {
                    //MessageBox.Show("2");
                    if (answerAddingKmArrayToTableTested.results[0].errorCode == 0)
                    {
                        //MessageBox.Show("3");
                        if (answerAddingKmArrayToTableTested.results[0].result[0].onlineValidation != null)
                        {
                            //MessageBox.Show("4");
                            if (answerAddingKmArrayToTableTested.results[0].result[0].driverError.code == 0)
                            {
                                //MessageBox.Show("5");
                                int i = 0;
                                while (i < answerAddingKmArrayToTableTested.results[0].result.Count)
                                {   //Успешная проверка это когда в секции "itemInfoCheckResult" реквизит "imcCheckResult" имеет значение true
                                    if (!answerAddingKmArrayToTableTested.results[0].result[i].onlineValidation.itemInfoCheckResult.imcCheckResult)
                                    {
                                        if (answerAddingKmArrayToTableTested.results[0].result[i].onlineValidation.markOperatorResponseResult != "correct")
                                        {
                                            if (answerAddingKmArrayToTableTested.results[0].result[i].onlineValidation.markOperatorResponseResult == "incorrect")
                                            {
                                                //MessageBox.Show("Запрос имеет некорректный формат markOperatorResponseResult = " + answerAddingKmArrayToTableTested.results[0].result[0].onlineValidation.markOperatorResponseResult.ToString());
                                                MessageBox.Show("Запрос имеет некорректный формат в строке = " + num_strt_km[i].ToString());
                                                result = false;
                                            }
                                            if (answerAddingKmArrayToTableTested.results[0].result[i].onlineValidation.markOperatorResponseResult == "unrecognized")
                                            {
                                                //MessageBox.Show("КМ имеет некорретный формат markOperatorResponseResult = " + answerAddingKmArrayToTableTested.results[0].result[0].onlineValidation.markOperatorResponseResult.ToString());
                                                MessageBox.Show("КМ имеет некорретный формат в строке = " + num_strt_km[i].ToString());
                                                //listView1.Items[num_strt_km[i]-1].SubItems[14].Text = change_case_in_line(listView1.Items[num_strt_km[i]-1].SubItems[14].Text);
                                                result = false;//попробовать вызвать еще раз с км с измененным регистром
                                            }
                                        }
                                    }
                                    i++;
                                }
                            }
                            else
                            {
                                string description = "";
                                if (answerAddingKmArrayToTableTested.results[0].result[0].driverError.error != null)
                                {
                                    description += answerAddingKmArrayToTableTested.results[0].result[0].driverError.error;
                                }
                                if (answerAddingKmArrayToTableTested.results[0].result[0].driverError.description != null)
                                {
                                    description += answerAddingKmArrayToTableTested.results[0].result[0].driverError.description;
                                }

                                MessageBox.Show("Ошибка при проверке кодов маркировки код ошибки " + answerAddingKmArrayToTableTested.results[0].result[0].driverError.code.ToString() + " " + description);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при проверке кодов маркировки код ошибки " + answerAddingKmArrayToTableTested.results[0].result[0].driverError.code + " " + answerAddingKmArrayToTableTested.results[0].result[0].driverError.description);
                            result = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show(" Ошибка при проверке кодов маркировки код ошибки " + answerAddingKmArrayToTableTested.results[0].errorCode.ToString() + " " + answerAddingKmArrayToTableTested.results[0].errorDescription.ToString());
                        result = false;
                    }
                }
                else
                {
                    MessageBox.Show(" Ошибка при проверке кодов маркировки код ошибки " + answerAddingKmArrayToTableTested.results[0].errorCode.ToString() + " " + answerAddingKmArrayToTableTested.results[0].errorDescription.ToString());
                    result = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = false;
            }
            return result;
        }


        private string change_case_in_line(string s)
        {
            string result = "";
            Regex reg = new Regex("[a-zA-Z]");
            foreach (char c in s)
            {
                //Проверяем что это буква                
                if (reg.IsMatch(c.ToString()))
                {
                    if (char.IsLower(c))
                    {
                        result += c.ToString().ToUpper();
                    }
                    else
                    {
                        result += c.ToString().ToLower();
                    }
                }
                else
                {
                    result += c.ToString();
                }
            }            

            return result;
        }

        /// <summary>
        /// Получение результата
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        private static AnswerAddingKmArrayToTableTested GET_AnswerAddingKmArrayToTableTested(string Url, string Data)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "/" + Data);
            req.Timeout = 480000;
            MainStaticClass.set_basic_auth(req);

            System.Net.WebResponse resp = req.GetResponse();
            //HttpWebResponse myHttpWebResponse = (HttpWebResponse)req.GetResponse();

            System.IO.Stream stream = resp.GetResponseStream();

            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();

            //Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(Out);
            //var obj = JsonConvert.DeserializeObject(Out) as System.Collections.Generic.ICollection<Results>; 
            //System.IO.File.AppendAllText(Application.StartupPath.Replace("\\", "/") + "/" + "json.txt", DateTime.Now.ToString() + " answer_chech_KM \r\n");
            //File.AppendAllText(Application.StartupPath.Replace("\\","/") + "/"+ "json.txt", Out+"\r\n");
            
            var results = JsonConvert.DeserializeObject<AnswerAddingKmArrayToTableTested>(Out);

            //AnswerAddingKmArrayToTableTested results = JsonConvert.DeserializeObject<AnswerAddingKmArrayToTableTested>(Out);
            //Thread.Sleep(1000);
            req = null;
            resp.Close();
            stream.Close();
            sr = null;

            return results;
            //return Out;
        }


       



        /// <summary>
        /// Отправка сообщения и чтение результата задания по проверке массива КМ
        /// Кодов Маркировки
        /// </summary>
        /// <param name="addingKmArrayToTableTestedKm"></param>
        /// <returns></returns>
        private static AnswerAddingKmArrayToTableTested Tested_Km(AddingKmArrayToTableTested addingKmArrayToTableTestedKm)
        {
            string status = "";
            AnswerAddingKmArrayToTableTested result = null;
            string km_array_to_table_tested = JsonConvert.SerializeObject(addingKmArrayToTableTestedKm, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string json = MainStaticClass.shablon.Replace("body", km_array_to_table_tested);
            string guid = Guid.NewGuid().ToString();
            string replace = "\"uuid\": \"" + guid + "\"";
            json = json.Replace("uuid", replace);
            //System.IO.File.AppendAllText(Application.StartupPath.Replace("\\", "/") + "/" + "json.txt", DateTime.Now.ToString() + " chech_KM\r\n");            
            //File.AppendAllText(Application.StartupPath.Replace("\\", "/") + "/" + "json.txt", json+"\r\n");
            if (FiscallPrintJason.POST(MainStaticClass.url, json) == "Created")
            {
                int count = 0;
                while (1 == 1)
                {
                    count++;
                    Thread.Sleep(1000);
                    result = GET_AnswerAddingKmArrayToTableTested(MainStaticClass.url, guid);
                    status = result.results[0].status;
                    if ((status != "ready") && (status != "error"))
                    {
                        if (count > 199)
                        {
                            break;
                        }
                    }
                    else if ((status == "ready") || (status == "error"))
                    {
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Класс для запроса состояния буфера 
        /// КМ
        /// </summary>
        public class СheckImcWorkState
        {
            public string type { get; set; }
            public СheckImcWorkState()
            {
                type = "checkImcWorkState";
            }
        }


        private static AnswerCheckImcWorkState GET_AnswerCheckImcWorkState(string Url, string Data)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "/" + Data);
            req.Timeout = 10000;
            MainStaticClass.set_basic_auth(req);
            System.Net.WebResponse resp = req.GetResponse();
            //HttpWebResponse myHttpWebResponse = (HttpWebResponse)req.GetResponse();            
            System.IO.Stream stream = resp.GetResponseStream();

            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();

            //Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(Out);
            //var obj = JsonConvert.DeserializeObject(Out) as System.Collections.Generic.ICollection<Results>; 

            var results = JsonConvert.DeserializeObject<AnswerCheckImcWorkState>(Out);

            //AnswerAddingKmArrayToTableTested results = JsonConvert.DeserializeObject<AnswerAddingKmArrayToTableTested>(Out);
            //Thread.Sleep(1000);
            req = null;
            resp.Close();
            stream.Close();
            sr = null;

            return results;
            //return Out;
        }

        /// <summary>
        /// Проверка состояния буфера
        /// кодов маркировки
        /// </summary>
        private bool check_imc_work_state(int count_km)
        {
            СheckImcWorkState сheckImcWorkState = new СheckImcWorkState();
            string status = "";
            bool result = false;
            AnswerCheckImcWorkState answerCheckImcWorkState = null;
            string сheckImc = JsonConvert.SerializeObject(сheckImcWorkState, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string json = MainStaticClass.shablon.Replace("body", сheckImc);
            string guid = Guid.NewGuid().ToString();
            string replace = "\"uuid\": \"" + guid + "\"";
            json = json.Replace("uuid", replace);
            if (FiscallPrintJason.POST(MainStaticClass.url, json) == "Created")
            {
                int count = 0;
                while (1 == 1)
                {
                    count++;
                    Thread.Sleep(1000);
                    answerCheckImcWorkState = GET_AnswerCheckImcWorkState(MainStaticClass.url, guid);
                    status = answerCheckImcWorkState.results[0].status;
                    if ((status != "ready") && (status != "error"))
                    {
                        if (count > 14)
                        {
                            break;
                        }
                    }
                    else if ((status == "ready") || (status == "error"))
                    {
                        break;
                    }
                }
            }
            try
            {
                if (answerCheckImcWorkState != null)
                {
                    if (answerCheckImcWorkState.results[0].errorCode == 0)
                    {
                        if (count_km == answerCheckImcWorkState.results[0].result.fm.checkingCount)//количество кодов маркировки в чеке и в буфере ФР одинаковое
                        {
                            result = true;
                        }
                        else
                        {
                            MessageBox.Show("В чеке " + count_km.ToString() + " код(а) маркировки,а в буфере ФР " + answerCheckImcWorkState.results[0].result.fm.checkingCount.ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неопознанная ошибка, код ошибки " + answerCheckImcWorkState.results[0].errorCode.ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;

        }
               
        private void fiscall_print_pay_1(string pay)
        {
            {

                //if (MainStaticClass.SystemTaxation == 0)
                //{
                //    MessageBox.Show("В константах не опрелена система налогобложения, печать чеков невозможна");
                //    return;
                //}

                bool error = false;


                //if (to_print_certainly == 1)
                //{
                //    MainStaticClass.delete_document_wil_be_printed(numdoc.ToString());
                //}

                //if (MainStaticClass.get_document_wil_be_printed(numdoc.ToString()) != 0)
                //{
                //    MessageBox.Show("Этот чек уже был успешно отправлен на печать");
                //    return;
                //}

                closing = false;

                print_terminal_check();

                FiscallPrintJason.Check check = new FiscallPrintJason.Check();
                check.type = "sell";
                check.electronically = false;
                if (client.Tag != null)
                {
                    if (client.Tag.ToString() == "0000000123")
                    {
                        txtB_email_telephone.Text = "zajtsev@gmail.com";
                        check.electronically = true;
                        MessageBox.Show(" Чек отправлен по электронной почте ");
                    }
                }

                //if (DateTime.Now > new DateTime(2021, 1, 1))
                //{
                //    check.taxationType = (MainStaticClass.UsnIncomeOutcome ? "usnIncomeOutcome" : "osn");
                //}
                //else
                //{
                //    check.taxationType = (MainStaticClass.Use_Envd ? "envd" : "osn");
                //}

                check.taxationType = (MainStaticClass.SystemTaxation == 1 ? "osn" : "usnIncomeOutcome");

                check.ignoreNonFiscalPrintErrors = false;
                check.@operator = new FiscallPrintJason.Operator();
                check.@operator.name = MainStaticClass.Cash_Operator;
                check.@operator.vatin = MainStaticClass.CashOperatorInn;
                check.items = new List<Cash8.FiscallPrintJason.Item>();

                try
                {

                    int nomer_naloga = 0;
                    string tax_type = "";

                    foreach (ListViewItem lvi in listView1.Items)
                    {
                        if (Convert.ToDouble(lvi.SubItems[7].Text) != 0)
                        {
                            if (MainStaticClass.SystemTaxation == 1)
                            {
                                int stavka_nds = get_tovar_nds(lvi.SubItems[0].Text.Trim());
                                //nomer_naloga = 0;
                                //MainStaticClass.use
                                if (stavka_nds == 0)
                                {
                                    nomer_naloga = 1;
                                    tax_type = "vat0";
                                }
                                else if (stavka_nds == 10)
                                {
                                    nomer_naloga = 2;
                                    tax_type = "vat10";
                                }
                                else if (stavka_nds == 18)
                                {
                                    nomer_naloga = 3;
                                    tax_type = "vat20"; //tax_type = "vat18";
                                }
                                else if (stavka_nds == 20)
                                {
                                    nomer_naloga = 3;
                                    tax_type = "vat20";
                                }
                                else
                                {
                                    MessageBox.Show("Неизвестная ставка ндс");
                                }

                                if (its_certificate(lvi.SubItems[0].Text.Trim()) == "1")
                                {
                                    tax_type = "vat120";
                                    //item.paymentMethod = "advance";
                                }

                            }
                            else
                            {
                                nomer_naloga = 4;
                                tax_type = "none";
                            }

                            FiscallPrintJason.Item item = new FiscallPrintJason.Item();
                            item.name = lvi.SubItems[0].Text.Trim() + " " + lvi.SubItems[1].Text.Trim();// "Первая Позиция";
                            item.price = Convert.ToDouble(lvi.SubItems[5].Text);// 1;
                            item.quantity = Convert.ToDouble(lvi.SubItems[3].Text);
                            item.type = "position";
                            item.amount = Convert.ToDouble(lvi.SubItems[7].Text);
                            item.tax = new FiscallPrintJason.Tax();

                            if (lvi.SubItems[14].Text.Trim().Length<=13)//Если код маркировки не заполнен тогда передаем тнвэд
                            {
                                //item.nomenclatureCode = get_tnved(lvi.SubItems[0].Text);
                            }
                            else//иначе передаем код маркера маркировки 
                            {
                                //Пока оставлю новый но не рабочий кусок
                                //FiscallPrintJason.MarkingCode markingCode = new FiscallPrintJason.MarkingCode();
                                //byte[] textAsBytes = Encoding.UTF8.GetBytes(lvi.SubItems[14].Text.Trim());
                                //string mark_str = Convert.ToBase64String(textAsBytes);
                                //markingCode.mark = mark_str.Substring(0, mark_str.Length - 1);
                                //item.markingCode = markingCode;

                                FiscallPrintJason.NomenclatureCode nomenclatureCode = new FiscallPrintJason.NomenclatureCode();
                                nomenclatureCode.gtin = lvi.SubItems[14].Text.Trim().Substring(2, 14);
                                nomenclatureCode.serial = lvi.SubItems[14].Text.Trim().Substring(18, 13);
                                nomenclatureCode.type = "shoes";
                                item.nomenclatureCode = nomenclatureCode;
                                if (MainStaticClass.get_print_m())
                                {
                                    item.name = "[M] " + item.name;
                                }
                            }

                            //item.department = nomer_naloga;
                            //MessageBox.Show(its_certificate(lvi.SubItems[0].Text.Trim()));
                            //if (its_certificate(lvi.SubItems[0].Text.Trim()) == "1")
                            //{
                            //    tax_type = "vat120";
                            //    item.paymentMethod = "advance";
                            //}

                            item.tax.type = tax_type;//ндс 
                            if (tax_type == "vat120")//Это специально для сертификатов если магазин не envd
                            {
                                item.paymentMethod = "advance";
                            }

                            check.items.Add(item);
                        }
                    }

                    check.clientInfo = new FiscallPrintJason.ClientInfo();
                    if (txtB_email_telephone.Text.Trim().Length > 0)
                    {
                        check.clientInfo.emailOrPhone = txtB_email_telephone.Text;
                    }

                    if ((txtB_inn.Text.Trim().Length > 0) && (txtB_name.Text.Trim().Length > 0))
                    {
                        check.clientInfo.vatin = txtB_inn.Text;
                        check.clientInfo.name = txtB_name.Text;
                    }



                    //cash или 0 - наличными 
                    //electronically или 1 - электронными 
                    //prepaid или 2 - предварительная оплата (аванс) 
                    //credit или 3 - последующая оплата (кредит) 
                    //other или 4 - иная форма оп

                    //fiscal.Payment(false, Convert.ToDouble(pay), this.type_pay.SelectedIndex, out remainder, out change);
                    check.payments = new List<FiscallPrintJason.Payment>();

                    double[] get_result_paymen = get_cash_on_type_payment();
                    if (get_result_paymen[0] != 0)//Наличные
                    {
                        FiscallPrintJason.Payment payment = new FiscallPrintJason.Payment();
                        payment.type = "cash";
                        //payment.type = "other";
                        payment.sum = Convert.ToDouble(get_result_paymen[0]);
                        check.payments.Add(payment);
                        //fiscal.Payment(false, Convert.ToDouble(result[0]), 0, out remainder, out change);
                    }
                    if (get_result_paymen[1] != 0)
                    {
                        FiscallPrintJason.Payment payment = new FiscallPrintJason.Payment();
                        payment.type = "electronically";
                        payment.sum = Convert.ToDouble(get_result_paymen[1]);
                        check.payments.Add(payment);
                        //fiscal.Payment(false, Convert.ToDouble(result[1]), 3, out remainder, out change);
                    }
                    if (get_result_paymen[2] != 0)
                    {
                        FiscallPrintJason.Payment payment = new FiscallPrintJason.Payment();
                        payment.type = "prepaid";
                        payment.sum = Convert.ToDouble(get_result_paymen[2]);
                        check.payments.Add(payment);
                        //fiscal.Payment(false, Convert.ToDouble(result[2]), 4, out remainder, out change);
                    }

                    //вывести информацию об ндс

                    FiscallPrintJason.PostItem pi = new FiscallPrintJason.PostItem();
                    check.postItems = new List<FiscallPrintJason.PostItem>();
                    int length = 0;
                    if (Discount != 0)
                    {
                        string s = "Вами получена скидка " + calculation_of_the_discount_of_the_document().ToString().Replace(",", ".") + " " + MainStaticClass.get_currency();
                        length = s.Length;
                        pi = new FiscallPrintJason.PostItem();
                        pi.type = "text";
                        pi.text = s;
                        pi.alignment = "center";
                        check.postItems.Add(pi);
                        //fiscal.PrintString(out length, 1, s, 0);

                        if (client.Tag != null)
                        {
                            if (client.Tag == user.Tag)
                            {
                                s = "ДК: стандартная скидка";
                                pi = new FiscallPrintJason.PostItem();
                                pi.type = "text";
                                pi.text = s;
                                pi.alignment = "left";
                                check.postItems.Add(pi);
                            }
                            else
                            {
                                s = "ДК: " + client.Tag.ToString();
                                pi = new FiscallPrintJason.PostItem();
                                pi.type = "text";
                                pi.text = s;
                                pi.alignment = "left";
                                check.postItems.Add(pi);
                            }

                            //length = s.Length;
                            //fiscal.PrintString(out length, 1, s, 0);
                        }
                    }


                    //length = MainStaticClass.Nick_Shop.Length;
                    //length = (MainStaticClass.Nick_Shop + " кассир " + this.cashier).Length;
                    //Бонусы если такие есть 
                    //if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema==3) || (MainStaticClass.GetWorkSchema == 4))
                    //{
                        //if (MainStaticClass.PassPromo != "")
                        //{
                        //    //bonus_total_centr
                        //    if (bonus_total_centr != 0)
                        //    {
                        //        if (bonus_total_centr > 0)
                        //        {
                        //            pi = new FiscallPrintJason.PostItem();
                        //            pi.type = "text";
                        //            pi.text = "На вашем счету бонусов : " + bonus_total_centr.ToString();
                        //            pi.alignment = "left";
                        //            check.postItems.Add(pi);
                        //        }
                        //        else if (bonus_total_centr < 0)
                        //        {
                        //            pi = new FiscallPrintJason.PostItem();
                        //            pi.type = "text";
                        //            pi.text = "Нет доступа к текущему балансу. Общее кол-во бонусов будет обновлено в течении 7 дней";
                        //            pi.alignment = "left";
                        //            check.postItems.Add(pi);
                        //        }
                        //    }

                        //    if (bonuses_it_is_written_off == 0)
                        //    {
                        //        if (bonuses_it_is_counted != 0)
                        //        {
                        //            pi = new FiscallPrintJason.PostItem();
                        //            pi.type = "text";
                        //            pi.text = "Начислено бонусов : " + bonuses_it_is_counted.ToString();
                        //            pi.alignment = "left";
                        //            check.postItems.Add(pi);
                        //        }
                        //    }

                        //    if (bonuses_it_is_written_off != 0)
                        //    {
                        //        pi = new FiscallPrintJason.PostItem();
                        //        pi.type = "text";
                        //        pi.text = "Списано бонусов : " + (((int)bonuses_it_is_written_off) * 100).ToString();
                        //        pi.alignment = "left";
                        //        check.postItems.Add(pi);
                        //    }
                        //}
                    //}
                    //else if (MainStaticClass.GetWorkSchema == 2)//Это Ева
                    //{
                    //    if (client.Tag != null)//В чеке есть карта клиента
                    //    {
                    //        pi = new FiscallPrintJason.PostItem();
                    //        pi.type = "text";
                    //        pi.text = "СОСТОЯНИЕ СЧЕТА БОНУСОВ";
                    //        pi.alignment = "left";
                    //        check.postItems.Add(pi);

                    //        pi = new FiscallPrintJason.PostItem();
                    //        pi.type = "text";
                    //        pi.text = "ORANGE CARD";
                    //        pi.alignment = "left";
                    //        check.postItems.Add(pi);

                    //        pi = new FiscallPrintJason.PostItem();
                    //        pi.type = "text";
                    //        pi.text = "КАРТА: # " + client.Tag.ToString();//Пока думаем что всегда номер карты это типа код клиента
                    //        pi.alignment = "left";
                    //        check.postItems.Add(pi);

                    //        pi = new FiscallPrintJason.PostItem();
                    //        pi.type = "text";
                    //        pi.text = "PRN " + id_transaction;//id транзакции прир продаже
                    //        pi.alignment = "left";
                    //        check.postItems.Add(pi);

                    //        pi = new FiscallPrintJason.PostItem();
                    //        pi.type = "text";
                    //        pi.text = "БАЛАНС: " + bonus_total_centr.ToString();
                    //        pi.alignment = "left";
                    //        check.postItems.Add(pi);

                    //        if (bonuses_it_is_written_off == 0)
                    //        {
                    //            pi = new FiscallPrintJason.PostItem();
                    //            pi.type = "text";
                    //            pi.text = "НАЧИСЛЕНО: " + bonuses_it_is_counted.ToString();
                    //            pi.alignment = "left";
                    //            check.postItems.Add(pi);
                    //        }
                    //        else
                    //        {
                    //            pi = new FiscallPrintJason.PostItem();
                    //            pi.type = "text";
                    //            pi.text = "	СПИСАНО: " + (bonuses_it_is_written_off * 100).ToString();
                    //            pi.alignment = "left";
                    //            check.postItems.Add(pi);
                    //        }

                    //        pi = new FiscallPrintJason.PostItem();
                    //        pi.type = "text";
                    //        if (spendAllowed == "1")
                    //        {
                    //            if (bonuses_it_is_written_off == 0)
                    //            {
                    //                pi.text = "ДОСТУПНО: " + (bonus_total_centr + bonuses_it_is_counted).ToString();
                    //            }
                    //            else
                    //            {
                    //                pi.text = "ДОСТУПНО: " + (bonus_total_centr - bonuses_it_is_written_off * 100).ToString() + "\r\n" +
                    //                    "накапливайте с\r\n|+" +
                    //                    "ORANGE CARD, тратьте\r\n" +
                    //                    "с удовольствием.\r\n" +
                    //                    "ПОДРОБНОСТИ НА\r\n" +
                    //                    "www.evacosmetics.ru\r\n";
                    //            }
                    //        }
                    //        else
                    //        {
                    //            pi.text = "ДОСТУПНО: " + (bonus_total_centr + bonuses_it_is_counted).ToString() + "\r\n" +
                    //                "ВНИМАНИЕ!\r\n" +
                    //                "Использование\r\n" +
                    //                "бонусов невозможно.\r\n" +
                    //                "Анкета не полная!\r\n" +
                    //                "Информация:88002500880\r\n" +
                    //                "или в Личном Кабинете\r\n" +
                    //                "www.evacosmetics.ru";
                    //        }

                    //        pi.alignment = "left";
                    //        check.postItems.Add(pi);
                    //    }
                    //}

                    pi = new FiscallPrintJason.PostItem();
                    pi.type = "text";
                    pi.text = MainStaticClass.Nick_Shop + "-" + MainStaticClass.CashDeskNumber.ToString() + "-" + numdoc.ToString();// +" кассир " + this.cashier;
                    pi.alignment = "left";
                    check.postItems.Add(pi);




                    //fiscal.PrintString(out length, 1, MainStaticClass.Nick_Shop + " кассир " + this.cashier, 0);

                    /////////////////////////////////////////////////////////////////////
                    //string s2 = "- - - - - - - - - - - - - - - - - -";
                    //length = s2.Length;
                    //fiscal.PrintString(out length, 1, s2, 0);

                    //s2 = "   ДЕКЛАРИРОВАНИЕ ДОХОДОВ -";
                    //length = s2.Length;
                    //fiscal.PrintString(out length, 1, s2, 0);

                    //s2 = "ЗАЛОГ ВАШЕГО УСПЕШНОГО БУДУЩЕГО";
                    //length = s2.Length;
                    //fiscal.PrintString(out length, 1, s2, 0);



                    //s2 = "- - - - - - - - - - - - - - - - - -";
                    //length = s2.Length;
                    //fiscal.PrintString(out length, 1, s2, 0);
                    /////////////////////////////////////////////////////////////////////

                    print_fiscal_advertisement(check, pi);

                    //fiscal.CloseCheck(false, this.type_pay.SelectedIndex);
                    check.total = get_result_paymen[0] + get_result_paymen[1] + get_result_paymen[2];
                    try
                    {


                        //*********************************************
                        //Ситуация проверка на повторную печать успешности предыдущей печати                    
                        //1. Этот чек уже был успешно распечататн
                        //2. Этот чек был послан на печать, но печать завершилась неудачно.
                        //Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.check_last_check_print();
                        //if (result != null)
                        //{
                        //    if (result.results[0].status != "ready")//Предыдущее задание успешно завершено 
                        //    {
                        //        //MainStaticClass.write_event_in_log(" Чек успешно распечатан ", "Документ чек", numdoc.ToString());
                        //    }
                        //    else
                        //    {
                        //        if (MessageBox.Show(" Предыдущее задание завершилось с ошибкой !!! " + result.results[0].status + " | " + result.results[0].errorDescription, " Вы все равно хотите распечатать этот чек ? ", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        //        {
                        //            MessageBox.Show("Печать чека отменена");
                        //            return;
                        //        }
                        //    }
                        //}                    
                        //*********************************************


                        Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.check_print("sell", check, numdoc.ToString(),0);

                        if (result.results[0].status == "ready")//Задание выполнено успешно 
                        {
                            its_print();
                            MainStaticClass.write_event_in_log(" Чек успешно распечатан ", "Документ чек", numdoc.ToString());
                        }
                        else
                        {
                            MessageBox.Show(" ФРЕГ  Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                            MainStaticClass.write_event_in_log(" ФРЕГ  Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription, " Печать чека продажи ", numdoc.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                //catch (fptrsharp.FptrException ex)
                //{
                //    MessageBox.Show("Ошибка принтера " + ex.Message + " | " + ex.ResultCode + " | " + ex.ResutlDescription + " | " + ex.BadParamDescription);
                //    error = true;
                //}
                catch (Exception ex)
                {
                    MessageBox.Show("Общая ошибка " + ex.Message);
                    MainStaticClass.write_event_in_log("Общая ошибка " + ex.Message, "Печать яека продажи ", numdoc.ToString());
                    error = true;
                }
                finally
                {
                    //if (fiscal != null)
                    //{
                    //    fiscal.Dispose();
                    //}
                }

                //check = null;


                if (!error)
                {
                    //its_print();
                }

                //MainStaticClass.delete_events_in_log(numdoc.ToString());
                this.Close();


                //finally
                //{

                //}


                //fiscal.Report(1, 0, 0, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, 0, 0, false);

                //Work_Mini_FP_6 tpr = new Work_Mini_FP_6();
                //ListViewItem[] list = null;
                //list = new ListViewItem[listView1.Items.Count];
                //listView1.Items.CopyTo(list, 0);            

                //Thread t = new System.Threading.Thread(delegate() { tpr.fiscall_print_sale(1,list, Convert.ToDouble(pay),this.numdoc,calculation_of_the_discount_of_the_document().ToString()); });
                //MainStaticClass.Result_Fiscal_Print = false;
                //t.Start();
                //t.Join();

            }
        }
                      

        public class BeginMarkingCodeValidation
        {
            public string type { get; set; }
            public Params @params { get; set; }
        }

        public class Params
        {
            public string imcType { get; set; }
            public string imc { get; set; }
            public string itemEstimatedStatus { get; set; }
            public double itemQuantity { get; set; }
            public string itemUnits { get; set; }
            public int imcModeProcessing { get; set; }
            public bool notSendToServer { get; set; }
        }


        public class OfflineValidation
        {
            public bool fmCheck { get; set; }
            public bool fmCheckResult { get; set; }
            public string fmCheckErrorReason { get; set; }
        }



        /// <summary>
        /// Посылаем запрос beginMarkingCodeValidation и получает ответ 
        /// </summary>
        /// <param name="imcType"></param>
        /// <param name="imc"></param>
        /// <param name="itemEstimatedStatus"></param>
        /// <param name="itemQuantity"></param>
        /// <param name="itemUnits"></param>
        /// <param name="imcModeProcessing"></param>
        /// <param name="notSendToServer"></param>
        private void beginMarkingCodeValidation(string imcType,string imc,string itemEstimatedStatus,double itemQuantity,string itemUnits,int imcModeProcessing,bool notSendToServer)
        {
            BeginMarkingCodeValidation beginMarkingCodeValidation = new BeginMarkingCodeValidation();
            beginMarkingCodeValidation.type = "beginMarkingCodeValidation";
            beginMarkingCodeValidation.@params = new Params();
            beginMarkingCodeValidation.@params.imcType = imcType;
            beginMarkingCodeValidation.@params.imc = imc;
            beginMarkingCodeValidation.@params.itemEstimatedStatus = itemEstimatedStatus;
            beginMarkingCodeValidation.@params.imcModeProcessing = imcModeProcessing;                        
            beginMarkingCodeValidation.@params.itemQuantity = itemQuantity;
            beginMarkingCodeValidation.@params.itemUnits = itemUnits;
            beginMarkingCodeValidation.@params.notSendToServer = notSendToServer;
                                 
            string status = "";
            bool result = false;
            
            OfflineValidation offlineValidation = null;
            string сheck_validation = JsonConvert.SerializeObject(beginMarkingCodeValidation, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string json = MainStaticClass.shablon.Replace("body", сheck_validation);
            string guid = Guid.NewGuid().ToString();
            string replace = "\"uuid\": \"" + guid + "\"";
            json = json.Replace("uuid", replace);
            if (FiscallPrintJason.POST(MainStaticClass.url, json) == "Created")
            {
                int count = 0;
                while (1 == 1)
                {
                    count++;
                    Thread.Sleep(1000);
                    offlineValidation = GetOfflineValidation(MainStaticClass.url, guid);
                    //status = answerCheckImcWorkState.results[0].status;
                    status = "9";
                    if ((status != "ready") && (status != "error"))
                    {
                        if (count > 14)
                        {
                            break;
                        }
                    }
                    else if ((status == "ready") || (status == "error"))
                    {
                        break;
                    }
                }
            }

        }

        private static OfflineValidation GetOfflineValidation(string Url, string Data)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "/" + Data);
            req.Timeout = 10000;
            System.Net.WebResponse resp = req.GetResponse();
            //HttpWebResponse myHttpWebResponse = (HttpWebResponse)req.GetResponse();

            System.IO.Stream stream = resp.GetResponseStream();

            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();

            //Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(Out);
            //var obj = JsonConvert.DeserializeObject(Out) as System.Collections.Generic.ICollection<Results>; 

            var results = JsonConvert.DeserializeObject<OfflineValidation>(Out);

            //AnswerAddingKmArrayToTableTested results = JsonConvert.DeserializeObject<AnswerAddingKmArrayToTableTested>(Out);
            //Thread.Sleep(1000);
            req = null;
            resp.Close();
            stream.Close();
            sr = null;

            return results;
            //return Out;
        }

        #region MarkingCodeValidationStatus

        public class MarkingCodeValidationStatus
        {
            public bool ready { get; set; }
            public bool sentImcRequest { get; set; }
            public DriverError driverError { get; set; }
            public OnlineValidation onlineValidation { get; set; }
        }

        public class DriverError
        {
            public int code { get; set; }
        }

        public class ItemInfoCheckResult
        {
            public bool imcCheckFlag { get; set; }
            public bool imcCheckResult { get; set; }
            public bool imcStatusInfo { get; set; }
            public bool ecrStandAloneFlag { get; set; }
        }

        public class MarkOperatorResponse
        {
            public bool responseStatus { get; set; }
            public bool itemStatusCheck { get; set; }
        }

        public class OnlineValidation
        {
            public ItemInfoCheckResult itemInfoCheckResult { get; set; }
            public string markOperatorItemStatus { get; set; }
            public MarkOperatorResponse markOperatorResponse { get; set; }
            public string markOperatorResponseResult { get; set; }
            public string imcType { get; set; }
            public string imcBarcode { get; set; }
            public int imcModeProcessing { get; set; }
        }

      

        #endregion


        /// <summary>
        /// проверка на подакцизный товар
        /// </summary>
        /// <param name="code_tovar"></param>
        /// <returns></returns>
        private int its_excise(string code_tovar)
        {
            int result = 0;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {                
                conn.Open();
                string query = "SELECT its_excise FROM tovar WHERE code="+code_tovar;
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToInt16(command.ExecuteScalar());
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при чтении свойства подакцизный товар "+ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при чтении свойства подакцизный товар "+ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Dispose();
            }

            return result;
        }

        private void print_terminal_check()
        {
            if (recharge_note != "")
            {
                FiscallPrintJason2.NonFiscallDocument nonFiscallDocument = new FiscallPrintJason2.NonFiscallDocument();
                nonFiscallDocument.type = "nonFiscal";
                nonFiscallDocument.printFooter = false;

                FiscallPrintJason2.ItemNonFiscal itemNonFiscal = new FiscallPrintJason2.ItemNonFiscal();
                nonFiscallDocument.items = new List<FiscallPrintJason2.ItemNonFiscal>();

                itemNonFiscal.type = "text";
                itemNonFiscal.text = recharge_note.Replace("0xDF^^","");
                itemNonFiscal.alignment = "center";
                nonFiscallDocument.items.Add(itemNonFiscal);
                FiscallPrintJason2.print_not_fiscal_document(nonFiscallDocument);
                recharge_note = "";
            }
        }




        /// <summary>
        /// Печать чеа по ффд 1.2
        /// </summary>
        /// <param name="pay"></param>
        private void fiscall_print_pay_2(string pay)
        {

            //if (MainStaticClass.SystemTaxation == 0)
            //{
            //    MessageBox.Show("В константах не опрелена система налогобложения, печать чеков невозможна");
            //    return;
            //}

            bool error = false;


            if (to_print_certainly == 1)
            {
                MainStaticClass.delete_document_wil_be_printed(numdoc.ToString());
            }

            if (MainStaticClass.get_document_wil_be_printed(numdoc.ToString()) != 0)
            {
                MessageBox.Show("Этот чек уже был успешно отправлен на печать");
                return;
            }

            closing = false;

            print_terminal_check();

            //if (recharge_note != "")
            //{
            //    FiscallPrintJason2.NonFiscallDocument nonFiscallDocument = new FiscallPrintJason2.NonFiscallDocument();
            //    nonFiscallDocument.type = "nonFiscal";
            //    nonFiscallDocument.printFooter = false;

            //    FiscallPrintJason2.ItemNonFiscal itemNonFiscal   = new FiscallPrintJason2.ItemNonFiscal();
            //    nonFiscallDocument.items = new List<FiscallPrintJason2.ItemNonFiscal>();

            //    itemNonFiscal.type = "text";
            //    itemNonFiscal.text = recharge_note;
            //    itemNonFiscal.alignment = "center";
            //    nonFiscallDocument.items.Add(itemNonFiscal);
            //    FiscallPrintJason2.print_not_fiscal_document(nonFiscallDocument);
            //}

            FiscallPrintJason2.Check check = new FiscallPrintJason2.Check();
            FiscallPrintJason2.PostItem pi = new FiscallPrintJason2.PostItem();
            check.postItems = new List<FiscallPrintJason2.PostItem>();
            
            //check.guid = guid;                                   
            check.type = "sell";

            if (!checkBox_print_check.Checked)
            {
                check.electronically = true;
            }

            check.taxationType = (MainStaticClass.SystemTaxation == 1 ? "osn" : "usnIncomeOutcome");

            check.ignoreNonFiscalPrintErrors = false;
            //check.ignoreNonFiscalPrintErrors = true;
            check.validateMarkingCodes = false;
            check.@operator = new FiscallPrintJason.Operator();
            check.@operator.name = MainStaticClass.Cash_Operator;
            check.@operator.vatin = MainStaticClass.CashOperatorInn;
            check.items = new List<Cash8.FiscallPrintJason2.Item>();

            try
            {
                int nomer_naloga = 0;
                string tax_type = "";
                int index_marker_position = 0;

                foreach (ListViewItem lvi in listView1.Items)
                {
                    if (Convert.ToDouble(lvi.SubItems[7].Text) != 0)
                    {
                        if (MainStaticClass.SystemTaxation == 1)
                        {
                            int stavka_nds = get_tovar_nds(lvi.SubItems[0].Text.Trim());
                            //nomer_naloga = 0;
                            //MainStaticClass.use
                            if (stavka_nds == 0)
                            {
                                nomer_naloga = 1;
                                tax_type = "vat0";
                            }
                            else if (stavka_nds == 10)
                            {
                                nomer_naloga = 2;
                                tax_type = "vat10";
                            }
                            else if (stavka_nds == 18)
                            {
                                nomer_naloga = 3;
                                tax_type = "vat20"; //tax_type = "vat18";
                            }
                            else if (stavka_nds == 20)
                            {
                                nomer_naloga = 3;
                                tax_type = "vat20";
                            }
                            else
                            {
                                MessageBox.Show("Неизвестная ставка ндс");
                            }

                            if (its_certificate(lvi.SubItems[0].Text.Trim()) == "1")
                            {
                                tax_type = "vat120";
                                //item.paymentMethod = "advance";
                            }

                        }
                        else
                        {
                            nomer_naloga = 4;
                            tax_type = "none";
                        }

                        FiscallPrintJason2.Item item = new FiscallPrintJason2.Item();
                        item.name = lvi.SubItems[0].Text.Trim() + " " + lvi.SubItems[1].Text.Trim();// "Первая Позиция";
                        item.price = Convert.ToDouble(lvi.SubItems[5].Text);// 1;
                        item.quantity = Convert.ToDouble(lvi.SubItems[3].Text);
                        item.type = "position";
                        item.amount = Convert.ToDouble(lvi.SubItems[7].Text);
                        item.tax = new FiscallPrintJason2.Tax();
                        item.measurementUnit = MainStaticClass.check_fractional_tovar(lvi.SubItems[0].Text.Trim()); //piece
                        if (its_excise(lvi.SubItems[0].Text.Trim()) == 0)
                        {
                            if (lvi.SubItems[14].Text.Trim().Length <= 13)//код маркировки не заполнен
                            {
                                item.paymentObject = "commodityWithoutMarking";
                            }
                            else//иначе передаем код маркировки 
                            {
                                item.paymentObject = "commodityWithMarking";
                                FiscallPrintJason2.ImcParams imcParams = new FiscallPrintJason2.ImcParams();
                                imcParams.imcType = "auto";
                                byte[] textAsBytes = Encoding.Default.GetBytes(lvi.SubItems[14].Text.Trim().Replace("vasya2021", "'"));
                                //string mark_str = Convert.ToBase64String(textAsBytes);
                                //imcParams.imc = mark_str;
                                imcParams.imc = Convert.ToBase64String(textAsBytes);
                                imcParams.itemEstimatedStatus = "itemPieceSold";
                                imcParams.imcModeProcessing = 0;

                                //imcParams.itemInfoCheckResult                           = new FiscallPrintJason2.ItemInfoCheckResult();
                                //imcParams.itemInfoCheckResult.ecrStandAloneFlag         = true;//this.answerAddingKmArrayToTableTested.results[0].result[index_marker_position].onlineValidation.itemInfoCheckResult.ecrStandAloneFlag;
                                //imcParams.itemInfoCheckResult.imcCheckFlag              = true;//this.answerAddingKmArrayToTableTested.results[0].result[index_marker_position].onlineValidation.itemInfoCheckResult.imcCheckFlag;
                                //imcParams.itemInfoCheckResult.imcCheckResult            = true;//this.answerAddingKmArrayToTableTested.results[0].result[index_marker_position].onlineValidation.itemInfoCheckResult.imcCheckResult;
                                //imcParams.itemInfoCheckResult.imcEstimatedStatusCorrect = true; //his.answerAddingKmArrayToTableTested.results[0].result[index_marker_position].onlineValidation.itemInfoCheckResult.imcEstimatedStatusCorrect;
                                //imcParams.itemInfoCheckResult.imcStatusInfo             = true;// this.answerAddingKmArrayToTableTested.results[0].result[index_marker_position].onlineValidation.itemInfoCheckResult.imcStatusInfo;

                                item.imcParams = imcParams;
                                index_marker_position++;
                            }
                        }
                        else
                        {
                            if (lvi.SubItems[14].Text.Trim().Length <= 13)//код маркировки не заполнен
                            {
                                item.paymentObject = "exciseWithoutMarking";
                            }
                            else//иначе передаем код маркировки 
                            {
                                item.paymentObject = "exciseWithMarking";
                                FiscallPrintJason2.ImcParams imcParams = new FiscallPrintJason2.ImcParams();
                                imcParams.imcType = "auto";
                                byte[] textAsBytes = Encoding.Default.GetBytes(lvi.SubItems[14].Text.Trim());
                                //string mark_str = Convert.ToBase64String(textAsBytes);
                                //imcParams.imc = mark_str;
                                imcParams.imc = Convert.ToBase64String(textAsBytes);
                                imcParams.itemEstimatedStatus = "itemPieceSold";
                                imcParams.imcModeProcessing = 0;
                                item.imcParams = imcParams;
                                index_marker_position++;
                            }
                        }

                        item.tax.type = tax_type;//ндс 
                        if (tax_type == "vat120")//Это специально для сертификатов если магазин не envd
                        {
                            item.paymentMethod = "advance";
                        }

                        check.items.Add(item);
                    }
                }

                check.clientInfo = new FiscallPrintJason2.ClientInfo();
                if (txtB_email_telephone.Text.Trim().Length > 0)
                {
                    check.clientInfo.emailOrPhone = txtB_email_telephone.Text;
                }

                if ((txtB_inn.Text.Trim().Length > 0) && (txtB_name.Text.Trim().Length > 0))
                {
                    check.clientInfo.vatin = txtB_inn.Text;
                    check.clientInfo.name = txtB_name.Text;
                }

                //cash или 0 - наличными 
                //electronically или 1 - электронными 
                //prepaid или 2 - предварительная оплата (аванс) 
                //credit или 3 - последующая оплата (кредит) 
                //other или 4 - иная форма оп

                //fiscal.Payment(false, Convert.ToDouble(pay), this.type_pay.SelectedIndex, out remainder, out change);
                check.payments = new List<FiscallPrintJason2.Payment>();

                double[] get_result_payment = get_cash_on_type_payment();
                if (get_result_payment[0] != 0)//Наличные
                {
                    FiscallPrintJason2.Payment payment = new FiscallPrintJason2.Payment();
                    payment.type = "cash";
                    //payment.type = "other";
                    payment.sum = Convert.ToDouble(get_result_payment[0]);
                    check.payments.Add(payment);
                    //fiscal.Payment(false, Convert.ToDouble(result[0]), 0, out remainder, out change);
                }
                if (get_result_payment[1] != 0)
                {
                    FiscallPrintJason2.Payment payment = new FiscallPrintJason2.Payment();
                    payment.type = "electronically";
                    payment.sum = Convert.ToDouble(get_result_payment[1]);
                    check.payments.Add(payment);
                    //fiscal.Payment(false, Convert.ToDouble(result[1]), 3, out remainder, out change);
                }
                if (get_result_payment[2] != 0)
                {
                    FiscallPrintJason2.Payment payment = new FiscallPrintJason2.Payment();
                    payment.type = "prepaid";
                    payment.sum = Convert.ToDouble(get_result_payment[2]);
                    check.payments.Add(payment);
                    //fiscal.Payment(false, Convert.ToDouble(result[2]), 4, out remainder, out change);
                }

                //вывести информацию об ндс

                //FiscallPrintJason2.PostItem pi = new FiscallPrintJason2.PostItem();
                //check.postItems = new List<FiscallPrintJason2.PostItem>();
                int length = 0;
                if (Discount != 0)
                {
                    string s = "Вами получена скидка " + calculation_of_the_discount_of_the_document().ToString().Replace(",", ".") + " " + MainStaticClass.get_currency();
                    length = s.Length;
                    pi = new FiscallPrintJason2.PostItem();
                    pi.type = "text";
                    pi.text = s;
                    pi.alignment = "center";
                    check.postItems.Add(pi);

                    if (client.Tag != null)
                    {
                        if (client.Tag == user.Tag)
                        {
                            s = "ДК: стандартная скидка";
                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = s;
                            pi.alignment = "left";
                            check.postItems.Add(pi);
                        }
                        else
                        {
                            s = "ДК: " + client.Tag.ToString();
                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = s;
                            pi.alignment = "left";
                            check.postItems.Add(pi);
                        }
                    }
                }

                //Бонусы если такие есть 
                //if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema==3) || (MainStaticClass.GetWorkSchema == 4))
                //{
                    //if (MainStaticClass.PassPromo != "")
                    //{
                    //    //bonus_total_centr
                    //    if (bonus_total_centr != 0)
                    //    {
                    //        if (bonus_total_centr > 0)
                    //        {
                    //            pi = new FiscallPrintJason2.PostItem();
                    //            pi.type = "text";
                    //            pi.text = "На вашем счету бонусов : " + bonus_total_centr.ToString();
                    //            pi.alignment = "left";
                    //            check.postItems.Add(pi);
                    //        }
                    //        else if (bonus_total_centr < 0)
                    //        {
                    //            pi = new FiscallPrintJason2.PostItem();
                    //            pi.type = "text";
                    //            pi.text = "Нет доступа к текущему балансу. Общее кол-во      бонусов будет обновлено в течении 7 дней";
                    //            pi.alignment = "left";
                    //            check.postItems.Add(pi);
                    //        }
                    //    }

                    //    if (bonuses_it_is_written_off == 0)
                    //    {
                    //        if (bonuses_it_is_counted != 0)
                    //        {
                    //            pi = new FiscallPrintJason2.PostItem();
                    //            pi.type = "text";
                    //            pi.text = "Начислено бонусов : " + bonuses_it_is_counted.ToString();
                    //            pi.alignment = "left";
                    //            check.postItems.Add(pi);
                    //        }
                    //    }

                    //    if (bonuses_it_is_written_off != 0)
                    //    {
                    //        pi = new FiscallPrintJason2.PostItem();
                    //        pi.type = "text";
                    //        pi.text = "Списано бонусов : " + (((int)bonuses_it_is_written_off) * 100).ToString();
                    //        pi.alignment = "left";
                    //        check.postItems.Add(pi);
                    //    }
                    //}
                //}
                //else if (MainStaticClass.GetWorkSchema == 2)//Это Ева
                //{
                //    if (client.Tag != null)//В чеке есть карта клиента
                //    {
                //        pi = new FiscallPrintJason2.PostItem();
                //        pi.type = "text";
                //        pi.text = "СОСТОЯНИЕ СЧЕТА БОНУСОВ";
                //        pi.alignment = "left";
                //        check.postItems.Add(pi);

                //        pi = new FiscallPrintJason2.PostItem();
                //        pi.type = "text";
                //        pi.text = "ORANGE CARD";
                //        pi.alignment = "left";
                //        check.postItems.Add(pi);

                //        pi = new FiscallPrintJason2.PostItem();
                //        pi.type = "text";
                //        pi.text = "КАРТА: # " + client.Tag.ToString();//Пока думаем что всегда номер карты это типа код клиента
                //        pi.alignment = "left";
                //        check.postItems.Add(pi);

                //        pi = new FiscallPrintJason2.PostItem();
                //        pi.type = "text";
                //        pi.text = "PRN " + id_transaction;//id транзакции прир продаже
                //        pi.alignment = "left";
                //        check.postItems.Add(pi);

                //        pi = new FiscallPrintJason2.PostItem();
                //        pi.type = "text";
                //        pi.text = "БАЛАНС: " + bonus_total_centr.ToString();
                //        pi.alignment = "left";
                //        check.postItems.Add(pi);

                //        if (bonuses_it_is_written_off == 0)
                //        {
                //            pi = new FiscallPrintJason2.PostItem();
                //            pi.type = "text";
                //            pi.text = "НАЧИСЛЕНО: " + bonuses_it_is_counted.ToString();
                //            pi.alignment = "left";
                //            check.postItems.Add(pi);
                //        }
                //        else
                //        {
                //            pi = new FiscallPrintJason2.PostItem();
                //            pi.type = "text";
                //            pi.text = "СПИСАНО: " + (bonuses_it_is_written_off * 100).ToString();
                //            pi.alignment = "left";
                //            check.postItems.Add(pi);
                //        }

                //        pi = new FiscallPrintJason2.PostItem();
                //        pi.type = "text";
                //        if (spendAllowed == "1")
                //        {
                //            if (bonuses_it_is_written_off == 0)
                //            {
                //                pi.text = "ДОСТУПНО: " + (bonus_total_centr + bonuses_it_is_counted).ToString();
                //            }
                //            else
                //            {
                //                pi.text = "ДОСТУПНО: " + (bonus_total_centr - bonuses_it_is_written_off * 100).ToString() + "\r\n" +
                //                    "накапливайте с\r\n|+" +
                //                    "ORANGE CARD, тратьте\r\n" +
                //                    "с удовольствием.\r\n" +
                //                    "ПОДРОБНОСТИ НА\r\n" +
                //                    "www.evacosmetics.ru\r\n";
                //            }
                //        }
                //        else
                //        {
                //            pi.text = "ДОСТУПНО: " + (bonus_total_centr + bonuses_it_is_counted).ToString() + "\r\n" +
                //                "ВНИМАНИЕ!\r\n" +
                //                "Использование\r\n" +
                //                "бонусов невозможно.\r\n" +
                //                "Анкета не полная!\r\n" +
                //                "Информация:88002500880\r\n" +
                //                "или в Личном Кабинете\r\n" +
                //                "www.evacosmetics.ru";
                //        }

                //        pi.alignment = "left";
                //        check.postItems.Add(pi);
                //    }
                //}

                pi = new FiscallPrintJason2.PostItem();
                pi.type = "text";
                pi.text = MainStaticClass.Nick_Shop + "-" + MainStaticClass.CashDeskNumber.ToString() + "-" + numdoc.ToString();// +" кассир " + this.cashier;
                pi.alignment = "left";
                check.postItems.Add(pi);
                print_fiscal_advertisement2(check, pi);
                check.total = get_result_payment[0] + get_result_payment[1] + get_result_payment[2];
                try
                {

                    Cash8.FiscallPrintJason2.RootObject result = FiscallPrintJason2.check_print("sell", check, numdoc.ToString(),0,guid);

                    if (result.results[0].status == "ready")//Задание выполнено успешно 
                    {
                        its_print();
                        MainStaticClass.write_event_in_log(" Чек успешно распечатан ", "Документ чек", numdoc.ToString());
                    }
                    else
                    {
                        MessageBox.Show(" ФРЕГ  Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                        MainStaticClass.write_event_in_log(" ФРЕГ  Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription, " Печать чека продажи ", numdoc.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Общая ошибка " + ex.Message);                
                error = true;
            }
            finally
            {
                //if (fiscal != null)
                //{
                //    fiscal.Dispose();
                //}
            }

            //check = null;


            if (!error)
            {
                //its_print();
            }

            MainStaticClass.delete_events_in_log(numdoc.ToString());
            this.Close();
        }


        /// <summary>
        /// При 3 схеме налогообложения необходимо 
        /// зафиксировать для 1с какие суммы 
        /// и в каких разрезах
        /// были распределены на 2 чек
        /// тот что маркировка 
        /// </summary>
        /// <param name="variant"></param>
        /// <param name="sum"></param>
        //private void update_summ1_systemtaxation3(int variant,double sum)
        //{
        //    NpgsqlConnection conn = null;            
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        string query = "";
        //        if (variant == 0)
        //        {
        //            query = "UPDATE public.checks_header	SET cash_money1=" + sum.ToString().Replace(",", ".") + "	WHERE document_number=" + numdoc.ToString();
        //        }
        //        else if (variant == 1)
        //        {
        //            query = "UPDATE public.checks_header	SET non_cash_money1=" + sum.ToString().Replace(",", ".") + "	WHERE document_number=" + numdoc.ToString();
        //        }
        //        else if (variant == 2)
        //        {
        //            query = "UPDATE public.checks_header	SET sertificate_money1=" + sum.ToString().Replace(",", ".") + "	WHERE document_number=" + numdoc.ToString();
        //        }

        //        NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //        command.ExecuteNonQuery();
        //        command.Dispose();                
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(" Ошибки при установке 2 суммы " + ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(" Ошибки при установке 2 суммы " + ex.Message);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            conn.Dispose();
        //        }
        //    }
        //}
        
        /// <summary>
        /// variant это форма оплаты
        /// 0 наличные        
        /// 1 карта
        /// 2 сертификат
        /// </summary>
        /// <param name="variant"></param>
        /// <returns></returns>
        public double[] get_summ1_systemtaxation3(int variant )
        {
            double[] result = new double[3];

            NpgsqlConnection conn = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "";
                if (variant == 0)
                {
                    query = "SELECT SUM(cash_money - cash_money1),SUM(non_cash_money - non_cash_money1),SUM(sertificate_money - sertificate_money1) FROM checks_header WHERE document_number=" + numdoc;
                }
                else
                {
                    query = "SELECT cash_money1,non_cash_money1,sertificate_money1 FROM checks_header WHERE document_number=" + numdoc.ToString();
                }

                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result[0] = Convert.ToDouble(reader[0]);
                    result[1] = Convert.ToDouble(reader[1]);
                    result[2] = Convert.ToDouble(reader[2]);
                }
                reader.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" Ошибки при чтении 2 суммы " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Ошибки при чтении 2 суммы " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }            

            return result;
        }



        //private bool calculate_summ1_systemtaxation3()
        //{
        //    bool result = true;

        //    double[] result_variant_1 = new double[3];
        //    result_variant_1[0] = 0;
        //    result_variant_1[1] = 0;
        //    result_variant_1[2] = 0;
        //    result_variant_1 = get_cash_on_type_payment();

        //    double[] get_result_payment = get_cash_on_type_payment_3(0, result_variant_1);// get_cash_on_type_payment();
        //    get_result_payment = get_cash_on_type_payment_3(1, result_variant_1);// get_cash_on_type_payment();

        //    return result;
        //}


        /// <summary>
        /// Печать чеа по ффд 1.2
        /// если схема налогообложения = 3
        /// то тогда отдельно печатается
        /// номенклатура без 
        /// маркировки и с маркировкой
        /// вариант 0 это печать обычного товара
        /// вариант 1 это маркированный товар
        /// </summary>
        /// <param name="pay"></param>
        /// /// <param name="variant"></param>
        private void fiscall_print_pay_2_3(string pay,int variant,string print_guid)
        {
            bool error = false;

            print_terminal_check();

            if (variant == 0)
            {
                if (to_print_certainly == 1)
                {
                    MainStaticClass.delete_document_wil_be_printed(numdoc.ToString(),variant);
                }

                if (MainStaticClass.get_document_wil_be_printed(numdoc.ToString()) != 0)
                {
                    MessageBox.Show("Этот чек уже был успешно отправлен на печать");
                    return;
                }
            }
            else if (variant == 1)
            {
                if (to_print_certainly_p == 1)
                {
                    MainStaticClass.delete_document_wil_be_printed(numdoc.ToString(), variant);
                }

                if (MainStaticClass.get_document_wil_be_printed(numdoc.ToString(),variant) != 0)
                {
                    MessageBox.Show("Этот чек уже был успешно отправлен на печать");
                    return;
                }
                //if (MainStaticClass.SystemTaxation != 3)
                //{
                //System.IO.File.AppendAllText(Application.StartupPath.Replace("\\", "/") + "/" + "json.txt", DateTime.Now.ToString() + " clearMarkingBuffer \r\n");
                if ((MainStaticClass.Version2Marking == 0) || (this.check_type.SelectedIndex == 1) || !itsnew)//старый механизм работы с макрировкой, для возвратов так же пока старая схема
                {                    
                    clearMarkingCodeValidationResult();

                    int count_km = 0;
                    foreach (ListViewItem lvi in listView1.Items)
                    {
                        if (lvi.SubItems[14].Text.Trim().Length > 13)
                        {
                            count_km++;
                        }
                    }
                    //bool continue_print = true;
                    int code_error = 0;// continue_print = true;
                    if (count_km != 0)
                    {
                        code_error = checking_km_before_adding_to_buffer();
                        //this.answerAddingKmArrayToTableTested = null;
                        //continue_print = checking_km_before_adding_to_buffer(ref this.answerAddingKmArrayToTableTested);
                        check_imc_work_state(count_km);
                        if (code_error == 0)
                        {
                            //continue_print = check_imc_work_state(count_km);//Проверка соответсвия состояния буфера в ФН и 
                        }
                        //else
                        //{
                        //    //return;
                        //}
                    }
                }
            }

            closing = false;

            FiscallPrintJason2.Check check = new FiscallPrintJason2.Check();
            check.type = "sell";
            


            if (variant == 0)
            {
                check.taxationType = "patent";                
            }
            else if (variant == 1)
            {
                check.taxationType = "usnIncomeOutcome";
            }
            else
            {
                MessageBox.Show("В печать не передан вариант печати, печать невозможна");
                return;
            }

            check.ignoreNonFiscalPrintErrors = false;
            //check.ignoreNonFiscalPrintErrors = true;
            check.validateMarkingCodes = false;
            check.@operator = new FiscallPrintJason.Operator();
            check.@operator.name = MainStaticClass.Cash_Operator;
            check.@operator.vatin = MainStaticClass.CashOperatorInn;
            check.items = new List<Cash8.FiscallPrintJason2.Item>();

            try
            {
                //int nomer_naloga = 0;
                string tax_type = "";
                int index_marker_position = 0;

                foreach (ListViewItem lvi in listView1.Items)
                {
                    if (Convert.ToDouble(lvi.SubItems[7].Text) != 0)
                    {
                        if (variant == 0)
                        {
                            if (lvi.SubItems[14].Text.Trim().Length > 13)
                            {
                                continue;
                            }
                        }
                        else if (variant == 1)
                        {
                            if (lvi.SubItems[14].Text.Trim().Length < 14)
                            {
                                continue;
                            }
                        }
                        tax_type = "none";                        

                        FiscallPrintJason2.Item item = new FiscallPrintJason2.Item();
                        item.name = lvi.SubItems[0].Text.Trim() + " " + lvi.SubItems[1].Text.Trim();// "Первая Позиция";
                        item.price = Convert.ToDouble(lvi.SubItems[5].Text);// 1;
                        item.quantity = Convert.ToDouble(lvi.SubItems[3].Text);
                        item.type = "position";
                        item.amount = Convert.ToDouble(lvi.SubItems[7].Text);
                        item.tax = new FiscallPrintJason2.Tax();
                        item.measurementUnit = MainStaticClass.check_fractional_tovar(lvi.SubItems[0].Text.Trim());// "piece";
                        if (lvi.SubItems[14].Text.Trim().Length <= 13)//код маркировки не заполнен оригинальное условие                          
                        {
                            item.paymentObject = "commodityWithoutMarking";
                        }
                        else//иначе передаем код маркировки 
                        {
                            item.paymentObject = "commodityWithMarking";
                            FiscallPrintJason2.ImcParams imcParams = new FiscallPrintJason2.ImcParams();
                            imcParams.imcType = "auto";
                            byte[] textAsBytes = Encoding.Default.GetBytes(lvi.SubItems[14].Text.Trim());
                            //string mark_str = Convert.ToBase64String(textAsBytes);
                            //imcParams.imc = mark_str;
                            imcParams.imc = Convert.ToBase64String(textAsBytes);
                            imcParams.itemEstimatedStatus = "itemPieceSold";
                            imcParams.imcModeProcessing = 0;

                            //imcParams.itemInfoCheckResult                           = new FiscallPrintJason2.ItemInfoCheckResult();
                            //imcParams.itemInfoCheckResult.ecrStandAloneFlag         = true;//this.answerAddingKmArrayToTableTested.results[0].result[index_marker_position].onlineValidation.itemInfoCheckResult.ecrStandAloneFlag;
                            //imcParams.itemInfoCheckResult.imcCheckFlag              = true;//this.answerAddingKmArrayToTableTested.results[0].result[index_marker_position].onlineValidation.itemInfoCheckResult.imcCheckFlag;
                            //imcParams.itemInfoCheckResult.imcCheckResult            = true;//this.answerAddingKmArrayToTableTested.results[0].result[index_marker_position].onlineValidation.itemInfoCheckResult.imcCheckResult;
                            //imcParams.itemInfoCheckResult.imcEstimatedStatusCorrect = true; //his.answerAddingKmArrayToTableTested.results[0].result[index_marker_position].onlineValidation.itemInfoCheckResult.imcEstimatedStatusCorrect;
                            //imcParams.itemInfoCheckResult.imcStatusInfo             = true;// this.answerAddingKmArrayToTableTested.results[0].result[index_marker_position].onlineValidation.itemInfoCheckResult.imcStatusInfo;

                            item.imcParams = imcParams;

                            index_marker_position++;
                        }

                        item.tax.type = tax_type;//ндс 
                        if (tax_type == "vat120")//Это специально для сертификатов если магазин не envd
                        {
                            item.paymentMethod = "advance";
                        }

                        check.items.Add(item);
                    }
                }

                //здесь проверяем есть ли строки по даннному виду налогообложения
                if (check.items.Count == 0)
                {
                    //здесь необходимо поставить флаг распечатан
                    its_print_p(variant);
                    return;
                }


                check.clientInfo = new FiscallPrintJason2.ClientInfo();
                if (txtB_email_telephone.Text.Trim().Length > 0)
                {
                    check.clientInfo.emailOrPhone = txtB_email_telephone.Text;
                }

                if ((txtB_inn.Text.Trim().Length > 0) && (txtB_name.Text.Trim().Length > 0))
                {
                    check.clientInfo.vatin = txtB_inn.Text;
                    check.clientInfo.name = txtB_name.Text;
                }

                //cash или 0 - наличными 
                //electronically или 1 - электронными 
                //prepaid или 2 - предварительная оплата (аванс) 
                //credit или 3 - последующая оплата (кредит) 
                //other или 4 - иная форма оп

                //fiscal.Payment(false, Convert.ToDouble(pay), this.type_pay.SelectedIndex, out remainder, out change);

                check.payments = new List<FiscallPrintJason2.Payment>();
                
                double[] get_result_payment = get_summ1_systemtaxation3(variant);
                
             
                if (get_result_payment[0] != 0)//Наличные
                {
                    FiscallPrintJason2.Payment payment = new FiscallPrintJason2.Payment();
                    payment.type = "cash";
                    payment.sum = Convert.ToDouble(get_result_payment[0]);
                    check.payments.Add(payment);
                }
                if (get_result_payment[1] != 0)//карточка
                {
                    FiscallPrintJason2.Payment payment = new FiscallPrintJason2.Payment();
                    payment.type = "electronically";
                    payment.sum = Convert.ToDouble(get_result_payment[1]);
                    check.payments.Add(payment);
                }
                if (get_result_payment[2] != 0)//сертификат
                {
                    FiscallPrintJason2.Payment payment = new FiscallPrintJason2.Payment();
                    payment.type = "prepaid";
                    payment.sum = Convert.ToDouble(get_result_payment[2]);
                    check.payments.Add(payment);
                }

                //вывести информацию об ндс

                FiscallPrintJason2.PostItem pi = new FiscallPrintJason2.PostItem();
                check.postItems = new List<FiscallPrintJason2.PostItem>();
                int length = 0;
                if (Discount != 0)
                {
                    string s = "Вами получена скидка " + calculation_of_the_discount_of_the_document().ToString().Replace(",", ".") + " " + MainStaticClass.get_currency();
                    length = s.Length;
                    pi = new FiscallPrintJason2.PostItem();
                    pi.type = "text";
                    pi.text = s;
                    pi.alignment = "center";
                    check.postItems.Add(pi);

                    if (client.Tag != null)
                    {
                        if (client.Tag == user.Tag)
                        {
                            s = "ДК: стандартная скидка";
                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = s;
                            pi.alignment = "left";
                            check.postItems.Add(pi);
                        }
                        else
                        {
                            s = "ДК: " + client.Tag.ToString();
                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = s;
                            pi.alignment = "left";
                            check.postItems.Add(pi);
                        }
                    }
                }

                //Бонусы если такие есть 
                //if ((MainStaticClass.GetWorkSchema == 1) || (MainStaticClass.GetWorkSchema == 3))
                //{
                //    if (MainStaticClass.PassPromo != "")
                //    {
                //        //bonus_total_centr
                //        if (bonus_total_centr != 0)
                //        {
                //            if (bonus_total_centr > 0)
                //            {
                //                pi = new FiscallPrintJason2.PostItem();
                //                pi.type = "text";
                //                pi.text = "На вашем счету бонусов : " + bonus_total_centr.ToString();
                //                pi.alignment = "left";
                //                check.postItems.Add(pi);
                //            }
                //            else if (bonus_total_centr < 0)
                //            {
                //                pi = new FiscallPrintJason2.PostItem();
                //                pi.type = "text";
                //                pi.text = "Нет доступа к текущему балансу. Общее кол-во      бонусов будет обновлено в течении 7 дней";
                //                pi.alignment = "left";
                //                check.postItems.Add(pi);
                //            }
                //        }

                //        if (bonuses_it_is_written_off == 0)
                //        {
                //            if (bonuses_it_is_counted != 0)
                //            {
                //                pi = new FiscallPrintJason2.PostItem();
                //                pi.type = "text";
                //                pi.text = "Начислено бонусов : " + bonuses_it_is_counted.ToString();
                //                pi.alignment = "left";
                //                check.postItems.Add(pi);
                //            }
                //        }

                //        if (bonuses_it_is_written_off != 0)
                //        {
                //            pi = new FiscallPrintJason2.PostItem();
                //            pi.type = "text";
                //            pi.text = "Списано бонусов : " + (((int)bonuses_it_is_written_off) * 100).ToString();
                //            pi.alignment = "left";
                //            check.postItems.Add(pi);
                //        }
                //    }
                //}
                //else if (MainStaticClass.GetWorkSchema == 2)//Это Ева
                //{
                //    if (client.Tag != null)//В чеке есть карта клиента
                //    {
                //        pi = new FiscallPrintJason2.PostItem();
                //        pi.type = "text";
                //        pi.text = "СОСТОЯНИЕ СЧЕТА БОНУСОВ";
                //        pi.alignment = "left";
                //        check.postItems.Add(pi);

                //        pi = new FiscallPrintJason2.PostItem();
                //        pi.type = "text";
                //        pi.text = "ORANGE CARD";
                //        pi.alignment = "left";
                //        check.postItems.Add(pi);

                //        pi = new FiscallPrintJason2.PostItem();
                //        pi.type = "text";
                //        pi.text = "КАРТА: # " + client.Tag.ToString();//Пока думаем что всегда номер карты это типа код клиента
                //        pi.alignment = "left";
                //        check.postItems.Add(pi);

                //        pi = new FiscallPrintJason2.PostItem();
                //        pi.type = "text";
                //        pi.text = "PRN " + id_transaction;//id транзакции прир продаже
                //        pi.alignment = "left";
                //        check.postItems.Add(pi);

                //        pi = new FiscallPrintJason2.PostItem();
                //        pi.type = "text";
                //        pi.text = "БАЛАНС: " + bonus_total_centr.ToString();
                //        pi.alignment = "left";
                //        check.postItems.Add(pi);

                //        if (bonuses_it_is_written_off == 0)
                //        {
                //            pi = new FiscallPrintJason2.PostItem();
                //            pi.type = "text";
                //            pi.text = "НАЧИСЛЕНО: " + bonuses_it_is_counted.ToString();
                //            pi.alignment = "left";
                //            check.postItems.Add(pi);
                //        }
                //        else
                //        {
                //            pi = new FiscallPrintJason2.PostItem();
                //            pi.type = "text";
                //            pi.text = "СПИСАНО: " + (bonuses_it_is_written_off * 100).ToString();
                //            pi.alignment = "left";
                //            check.postItems.Add(pi);
                //        }

                //        pi = new FiscallPrintJason2.PostItem();
                //        pi.type = "text";
                //        if (spendAllowed == "1")
                //        {
                //            if (bonuses_it_is_written_off == 0)
                //            {
                //                pi.text = "ДОСТУПНО: " + (bonus_total_centr + bonuses_it_is_counted).ToString();
                //            }
                //            else
                //            {
                //                pi.text = "ДОСТУПНО: " + (bonus_total_centr - bonuses_it_is_written_off * 100).ToString() + "\r\n" +
                //                    "накапливайте с\r\n|+" +
                //                    "ORANGE CARD, тратьте\r\n" +
                //                    "с удовольствием.\r\n" +
                //                    "ПОДРОБНОСТИ НА\r\n" +
                //                    "www.evacosmetics.ru\r\n";
                //            }
                //        }
                //        else
                //        {
                //            pi.text = "ДОСТУПНО: " + (bonus_total_centr + bonuses_it_is_counted).ToString() + "\r\n" +
                //                "ВНИМАНИЕ!\r\n" +
                //                "Использование\r\n" +
                //                "бонусов невозможно.\r\n" +
                //                "Анкета не полная!\r\n" +
                //                "Информация:88002500880\r\n" +
                //                "или в Личном Кабинете\r\n" +
                //                "www.evacosmetics.ru";
                //        }

                //        pi.alignment = "left";
                //        check.postItems.Add(pi);
                //    }
                //}

                pi = new FiscallPrintJason2.PostItem();
                pi.type = "text";
                pi.text = MainStaticClass.Nick_Shop + "-" + MainStaticClass.CashDeskNumber.ToString() + "-" + numdoc.ToString();// +" кассир " + this.cashier;
                pi.alignment = "left";
                check.postItems.Add(pi);
                print_fiscal_advertisement2(check, pi);
                check.total = get_result_payment[0] + get_result_payment[1] + get_result_payment[2];
                try
                {

                    Cash8.FiscallPrintJason2.RootObject result = FiscallPrintJason2.check_print("sell", check, numdoc.ToString(),variant, print_guid);

                    if (result.results[0].status == "ready")//Задание выполнено успешно 
                    {
                        its_print_p(variant);
                        MainStaticClass.write_event_in_log(" Чек успешно распечатан ", "Документ чек", numdoc.ToString());
                    }
                    else
                    {
                        MessageBox.Show(" ФРЕГ  Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                        MainStaticClass.write_event_in_log(" ФРЕГ  Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription, " Печать чека продажи ", numdoc.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Общая ошибка " + ex.Message);
                error = true;
            }
            finally
            {
                //if (fiscal != null)
                //{
                //    fiscal.Dispose();
                //}
            }

            //check = null;


            if (!error)
            {
                //its_print();
            }

            MainStaticClass.delete_events_in_log(numdoc.ToString());
            //this.Close();

        }

        /// <summary>
        /// Фискальная Печать
        /// регистрация продажного чека
        /// </summary>
        /// <param name="pay"></param>

        private void fiscall_print_pay(string pay)
        {
            if (MainStaticClass.SystemTaxation == 0)
            {
                MessageBox.Show("В константах не опрелена система налогобложения, печать чеков невозможна");
                return;
            }


            if (MainStaticClass.GetVersionFn == 1)
            {
                fiscall_print_pay_1(pay);
            }
            else if (MainStaticClass.GetVersionFn == 2)
            {
                if (MainStaticClass.SystemTaxation != 3)
                {
                    if (MainStaticClass.PrintingUsingLibraries == 0)
                    {
                        this.fiscall_print_pay_2(pay);

                    }
                    else
                    {
                        PrintingUsingLibraries printingUsingLibraries = new PrintingUsingLibraries();
                        {
                            printingUsingLibraries.print_sell_2_or_return_sell(this);

                        }
                    }                    
                }
                else
                {
                    //сначала печатаем без марикровки, а потом маркировку
                    //Если печать по кнопке то печататем только там где выбран флаг
                    //Здесь будет храниться информация для распределения по видам опдлаты между чеками
                    //double[] result_variant_1 = new double[3];
                    //result_variant_1[0] = 0;
                    //result_variant_1[1] = 0;
                    //result_variant_1[2] = 0;
                    //result_variant_1 = get_cash_on_type_payment();
                    //Перед печатью необходимо получить информацию какая сумма в каком виде оплаты содержится

                    if (print_to_button == 0)
                    {

                        if (MainStaticClass.PrintingUsingLibraries == 0)
                        {
                            this.fiscall_print_pay_2_3(pay, 0, this.guid);
                            this.fiscall_print_pay_2_3(pay, 1, this.guid1);
                        }
                        else
                        {
                            PrintingUsingLibraries printingUsingLibraries = new PrintingUsingLibraries();
                            printingUsingLibraries.print_sell_2_3_or_return_sell(this, 0);
                            printingUsingLibraries.print_sell_2_3_or_return_sell(this, 1);
                        }
                    }
                    else if (print_to_button==1)
                    {
                        if (this.checkBox_to_print_repeatedly.CheckState == CheckState.Checked)
                        {
                            if (MainStaticClass.PrintingUsingLibraries == 0)
                                this.fiscall_print_pay_2_3(pay, 0, this.guid);
                            else
                                new PrintingUsingLibraries().print_sell_2_3_or_return_sell(this, 0);
                        }
                        if (this.checkBox_to_print_repeatedly_p.CheckState == CheckState.Checked)
                        {
                            if (MainStaticClass.PrintingUsingLibraries == 0)
                                this.fiscall_print_pay_2_3(pay, 1, this.guid1);
                            else
                                new PrintingUsingLibraries().print_sell_2_3_or_return_sell(this, 1);
                        }
                    }
                    closing = false;
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Не удалось определить версию ФН, печать чеков невозможна");
            }          
        }       


        /// <summary>
        /// Бонусы начисленные и бонусы списанные 
        /// </summary>
        /// <returns></returns>
        private int[] get_bonus_rus()
        {
            int[] result = new int[2];

            foreach (ListViewItem lvi in listView1.Items)
            {
                result[0] += (int)Math.Round(Convert.ToDecimal(lvi.SubItems[11].Text) + (int)Convert.ToDecimal(lvi.SubItems[12].Text), 2, MidpointRounding.AwayFromZero);
                result[1] += (int)Math.Round(Convert.ToDecimal(lvi.SubItems[12].Text), 2, MidpointRounding.AwayFromZero);
            }

            return result;
        }

        private void fiscall_print_disburse_1(string cash_money, string non_cash_money)
        {
            print_terminal_check();

            if (MainStaticClass.SystemTaxation == 0)
            {
                MessageBox.Show("В константах не опрелена система налогобложения, печать чеков невозможна");
                return;
            }

            string output = calculation_of_the_sum_of_the_document().ToString();
            if (itsnew)
            {
                if (!write_new_document(output, output, "0", "0", true, cash_money, non_cash_money, "0", "0"))
                {
                    return;
                }
            }

            bool error = false;

            if (to_print_certainly == 1)
            {
                MainStaticClass.delete_document_wil_be_printed(numdoc.ToString());
            }

            if (MainStaticClass.get_document_wil_be_printed(numdoc.ToString()) != 0)
            {
                MessageBox.Show("Этот чек уже был успешно отправлен на печать");
                return;
            }

            if (MainStaticClass.Use_Fiscall_Print)
            {
                //fptrsharp.Fptr fiscal = null;
                closing = false;

                FiscallPrintJason.Check check = new FiscallPrintJason.Check();
                check.type = "sellReturn";
                check.taxationType = (MainStaticClass.SystemTaxation == 1 ? "osn" : "usnIncomeOutcome");

                check.ignoreNonFiscalPrintErrors = false;
                check.@operator = new FiscallPrintJason.Operator();
                check.@operator.name = MainStaticClass.Cash_Operator;
                check.@operator.vatin = MainStaticClass.CashOperatorInn;
                check.items = new List<Cash8.FiscallPrintJason.Item>();

                FiscallPrintJason.PostItem pi = new FiscallPrintJason.PostItem();
                check.postItems = new List<FiscallPrintJason.PostItem>();

                try
                {
                    //Cash8.Work_FPTK22 fptk22 = new Work_FPTK22(1);
                    //fiscal = fptk22.get_FPTK22;
                    int nomer_naloga = 0;
                    string tax_type = "";
                    foreach (ListViewItem lvi in listView1.Items)
                    {
                        if (Convert.ToDouble(lvi.SubItems[7].Text) != 0)
                        {
                            //fiscal.Return(false, 2, 1, lvi.SubItems[0].Text.Trim() + " " + lvi.SubItems[1].Text.Trim(), Convert.ToDouble(lvi.SubItems[3].Text), Convert.ToDouble(lvi.SubItems[5].Text), 0, true);
                            //int stavka_nds = get_tovar_nds(lvi.SubItems[0].Text.Trim());
                            //int nomer_naloga = 0;
                            //if (!MainStaticClass.Use_Envd)
                            //{
                            int stavka_nds = get_tovar_nds(lvi.SubItems[0].Text.Trim());
                            //int nomer_naloga = 0;
                            if (MainStaticClass.SystemTaxation == 1)
                            {
                                if (stavka_nds == 0)
                                {
                                    nomer_naloga = 1;
                                    tax_type = "vat0";
                                }
                                else if (stavka_nds == 10)
                                {
                                    nomer_naloga = 2;
                                    tax_type = "vat10";
                                }
                                else if (stavka_nds == 18)
                                {
                                    nomer_naloga = 3;
                                    tax_type = "vat20";
                                }
                                else if (stavka_nds == 20)
                                {
                                    nomer_naloga = 3;
                                    tax_type = "vat20";
                                }
                                else
                                {
                                    MessageBox.Show("Неизвестная ставка ндс");
                                }
                            }
                            else
                            {
                                nomer_naloga = 4;
                                tax_type = "none";
                            }
                            //}
                            //else
                            //{
                            //    nomer_naloga = 4;
                            //    tax_type = "none";
                            //}

                            FiscallPrintJason.Item item = new FiscallPrintJason.Item();
                            item.name = lvi.SubItems[0].Text.Trim() + " " + lvi.SubItems[1].Text.Trim();// "Первая Позиция";
                            item.price = Convert.ToDouble(lvi.SubItems[5].Text);// 1;
                            item.quantity = Convert.ToDouble(lvi.SubItems[3].Text);
                            item.type = "position";
                            item.amount = Convert.ToDouble(lvi.SubItems[7].Text);
                            item.tax = new FiscallPrintJason.Tax();
                            item.tax.type = tax_type;//ндс
                            if (lvi.SubItems[14].Text.Trim().Length > 13)
                            {
                                //FiscallPrintJason.MarkingCode markingCode = new FiscallPrintJason.MarkingCode();
                                //byte[] textAsBytes = Encoding.UTF8.GetBytes(lvi.SubItems[14].Text.Trim());
                                //string mark_str = Convert.ToBase64String(textAsBytes);
                                //markingCode.mark = mark_str.Substring(0, mark_str.Length - 1);
                                //item.markingCode = markingCode;
                                FiscallPrintJason.NomenclatureCode nomenclatureCode = new FiscallPrintJason.NomenclatureCode();
                                nomenclatureCode.gtin = lvi.SubItems[14].Text.Trim().Substring(3, 14);
                                nomenclatureCode.serial = lvi.SubItems[14].Text.Trim().Substring(18, 13);
                                nomenclatureCode.type = "shoes";
                                item.nomenclatureCode = nomenclatureCode;
                                if (MainStaticClass.get_print_m())
                                {
                                    item.name = "[M] " + item.name;
                                }
                            }

                            check.items.Add(item);
                        }
                    }


                    check.clientInfo = new FiscallPrintJason.ClientInfo();
                    if (txtB_email_telephone.Text.Trim().Length > 0)
                    {
                        check.clientInfo.emailOrPhone = txtB_email_telephone.Text;
                    }

                    if ((txtB_inn.Text.Trim().Length > 0) && (txtB_name.Text.Trim().Length > 0))
                    {
                        check.clientInfo.vatin = txtB_inn.Text;
                        check.clientInfo.name = txtB_name.Text;
                    }



                    double[] get_result_payment = get_cash_on_type_payment();
                    check.payments = new List<FiscallPrintJason.Payment>();
                    if (get_result_payment[0] != 0)//Наличные
                    {
                        FiscallPrintJason.Payment payment = new FiscallPrintJason.Payment();
                        payment.type = "cash";
                        payment.sum = Convert.ToDouble(get_result_payment[0]);
                        check.payments.Add(payment);
                        //fiscal.Payment(Convert.ToDouble(result[0]), 0, out remainder, out change);
                    }
                    if (get_result_payment[1] != 0)
                    {
                        FiscallPrintJason.Payment payment = new FiscallPrintJason.Payment();
                        payment.type = "electronically";
                        payment.sum = Convert.ToDouble(get_result_payment[1]);
                        check.payments.Add(payment);
                        //fiscal.Payment(Convert.ToDouble(result[1]), 3, out remainder, out change);
                    }


                    if (MainStaticClass.GetWorkSchema == 2)
                    {
                        if (client.Tag != null)
                        {
                            //здесь блок по бонусам
                            pi = new FiscallPrintJason.PostItem();
                            pi.type = "text";
                            pi.text = "Перерасчет БОНУСОВ";
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason.PostItem();
                            pi.type = "text";
                            pi.text = "ПОСЛЕ ВОЗВРАТА  ТОВАРА";
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason.PostItem();
                            pi.type = "text";
                            pi.text = "СПИСАНО " + bonuses_it_is_written_off.ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason.PostItem();
                            pi.type = "text";
                            pi.text = "СОСТОЯНИЕ СЧЕТА БОНУСОВ";
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason.PostItem();
                            pi.type = "text";
                            pi.text = "ORANGE CARD";
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason.PostItem();
                            pi.type = "text";
                            pi.text = "КАРТА #: " + client.Tag.ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason.PostItem();
                            pi.type = "text";
                            pi.text = "PRN: " + id_transaction;
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason.PostItem();
                            pi.type = "text";
                            pi.text = "БАЛАНС: " + bonus_total_centr.ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason.PostItem();
                            pi.type = "text";
                            pi.text = "СПИСАНО -" + bonuses_it_is_written_off.ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason.PostItem();
                            pi.type = "text";
                            pi.text = "ДОСТУПНО " + (bonus_total_centr - bonuses_it_is_written_off).ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);
                        }

                    }

                    //if (result[2] != 0)
                    //{
                    //    fiscal.Payment(Convert.ToDouble(result[2]), 4, out remainder, out change);
                    //}
                    pi = new FiscallPrintJason.PostItem();
                    pi.type = "text";
                    //pi.text = MainStaticClass.Nick_Shop + "-" + comment.Text.Trim();
                    pi.text = MainStaticClass.Nick_Shop + "-" + MainStaticClass.CashDeskNumber.ToString() + "-" + numdoc.ToString() + " (" + comment.Text.Trim() + ")";// +" кассир " + this.cashier;
                    pi.alignment = "left";
                    check.postItems.Add(pi);


                    //Оплата клиенту
                    //fiscal.CloseCheck(false, type_pay.SelectedIndex);
                    check.total = get_result_payment[0] + get_result_payment[1];

                    try
                    {
                        Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.check_print("sellReturn", check, numdoc.ToString(),0);

                        if (result.results[0].status == "ready")//Задание выполнено успешно 
                        {
                            its_print();
                        }
                        else
                        {
                            MessageBox.Show(" ФРЕГ Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                            MainStaticClass.write_event_in_log(" ФРЕГ  Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription, " Печать чека возврата ", numdoc.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }

                //catch (fptrsharp.FptrException ex)
                //{
                //    MessageBox.Show(ex.Message);
                //    error = true;
                //}
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    error = true;
                }
                finally
                {
                    //if (fiscal != null)
                    //{
                    //    fiscal.Dispose();
                    //}
                }
            }
            //else
            //{
            //    //string s = this.calculation_of_the_sum_of_the_document().ToString();
            //    text_print(output, output, "0", "0", "0", "0");
            //}


            this.Close();

        }

        private void fiscall_print_disburse_2(string cash_money, string non_cash_money)
        {
            print_terminal_check();

            if (MainStaticClass.SystemTaxation == 0)
            {
                MessageBox.Show("В константах не опрелена система налогобложения, печать чеков невозможна");
                return;
            }
                   

            string output = calculation_of_the_sum_of_the_document().ToString();
            if (itsnew)
            {
                if (!write_new_document(output, output, "0", "0", true, cash_money, non_cash_money, "0", "0"))
                {
                    return;
                }
            }

            //bool error = false;

            if (to_print_certainly == 1)
            {
                MainStaticClass.delete_document_wil_be_printed(numdoc.ToString());
            }

            if (MainStaticClass.get_document_wil_be_printed(numdoc.ToString()) != 0)
            {
                MessageBox.Show("Этот чек уже был успешно отправлен на печать");
                return;
            }

            if (MainStaticClass.Use_Fiscall_Print)
            {
                //fptrsharp.Fptr fiscal = null;
                closing = false;

                FiscallPrintJason2.Check check = new FiscallPrintJason2.Check();
                check.type = "sellReturn";
                check.taxationType = (MainStaticClass.SystemTaxation == 1 ? "osn" : "usnIncomeOutcome");
                check.ignoreNonFiscalPrintErrors = false;
                check.validateMarkingCodes = false;
                check.@operator = new FiscallPrintJason.Operator();
                check.@operator.name = MainStaticClass.Cash_Operator;
                check.@operator.vatin = MainStaticClass.CashOperatorInn;
                check.items = new List<Cash8.FiscallPrintJason2.Item>();

                FiscallPrintJason2.PostItem pi = new FiscallPrintJason2.PostItem();
                check.postItems = new List<FiscallPrintJason2.PostItem>();

                try
                {
                    //Cash8.Work_FPTK22 fptk22 = new Work_FPTK22(1);
                    //fiscal = fptk22.get_FPTK22;
                    int nomer_naloga = 0;
                    string tax_type = "";
                    foreach (ListViewItem lvi in listView1.Items)
                    {
                        if (Convert.ToDouble(lvi.SubItems[7].Text) != 0)
                        {
                            //fiscal.Return(false, 2, 1, lvi.SubItems[0].Text.Trim() + " " + lvi.SubItems[1].Text.Trim(), Convert.ToDouble(lvi.SubItems[3].Text), Convert.ToDouble(lvi.SubItems[5].Text), 0, true);
                            //int stavka_nds = get_tovar_nds(lvi.SubItems[0].Text.Trim());
                            //int nomer_naloga = 0;
                            //if (!MainStaticClass.Use_Envd)
                            //{
                            int stavka_nds = get_tovar_nds(lvi.SubItems[0].Text.Trim());
                            //int nomer_naloga = 0;
                            if (MainStaticClass.SystemTaxation == 1)
                            {
                                if (stavka_nds == 0)
                                {
                                    nomer_naloga = 1;
                                    tax_type = "vat0";
                                }
                                else if (stavka_nds == 10)
                                {
                                    nomer_naloga = 2;
                                    tax_type = "vat10";
                                }
                                else if (stavka_nds == 18)
                                {
                                    nomer_naloga = 3;
                                    tax_type = "vat20";
                                }
                                else if (stavka_nds == 20)
                                {
                                    nomer_naloga = 3;
                                    tax_type = "vat20";
                                }
                                else
                                {
                                    MessageBox.Show("Неизвестная ставка ндс");
                                }
                            }
                            else
                            {
                                nomer_naloga = 4;
                                tax_type = "none";
                            }
                            //}
                            //else
                            //{
                            //    nomer_naloga = 4;
                            //    tax_type = "none";
                            //}

                            FiscallPrintJason2.Item item = new FiscallPrintJason2.Item();
                            item.name = lvi.SubItems[0].Text.Trim() + " " + lvi.SubItems[1].Text.Trim();// "Первая Позиция";
                            item.price = Convert.ToDouble(lvi.SubItems[5].Text);// 1;
                            item.quantity = Convert.ToDouble(lvi.SubItems[3].Text);
                            item.type = "position";
                            item.amount = Convert.ToDouble(lvi.SubItems[7].Text);
                            item.tax = new FiscallPrintJason2.Tax();
                            item.tax.type = tax_type;//ндс
                            item.measurementUnit = MainStaticClass.check_fractional_tovar(lvi.SubItems[0].Text.Trim());// "piece";
                            if (lvi.SubItems[14].Text.Trim().Length <= 13)//Код маркировки не заполнен
                            {
                                item.paymentObject = "commodityWithoutMarking";
                            }
                            else//иначе передаем код маркировки 
                            {
                                item.paymentObject = "commodityWithMarking";
                                FiscallPrintJason2.ImcParams imcParams = new FiscallPrintJason2.ImcParams();
                                imcParams.imcType = "auto";
                                byte[] textAsBytes = Encoding.Default.GetBytes(lvi.SubItems[14].Text.Trim());
                                //string mark_str = Convert.ToBase64String(textAsBytes);
                                //imcParams.imc = mark_str;
                                imcParams.imc = Convert.ToBase64String(textAsBytes);
                                imcParams.itemEstimatedStatus = "itemPieceSold";
                                imcParams.imcModeProcessing = 0;
                                item.imcParams = imcParams;
                            }

                            check.items.Add(item);
                        }
                    }


                    check.clientInfo = new FiscallPrintJason2.ClientInfo();
                    if (txtB_email_telephone.Text.Trim().Length > 0)
                    {
                        check.clientInfo.emailOrPhone = txtB_email_telephone.Text;
                    }

                    if ((txtB_inn.Text.Trim().Length > 0) && (txtB_name.Text.Trim().Length > 0))
                    {
                        check.clientInfo.vatin = txtB_inn.Text;
                        check.clientInfo.name = txtB_name.Text;
                    }



                    double[] get_result_payment = get_cash_on_type_payment();
                    check.payments = new List<FiscallPrintJason2.Payment>();
                    if (get_result_payment[0] != 0)//Наличные
                    {
                        FiscallPrintJason2.Payment payment = new FiscallPrintJason2.Payment();
                        payment.type = "cash";
                        payment.sum = Convert.ToDouble(get_result_payment[0]);
                        check.payments.Add(payment);
                        //fiscal.Payment(Convert.ToDouble(result[0]), 0, out remainder, out change);
                    }
                    if (get_result_payment[1] != 0)
                    {
                        FiscallPrintJason2.Payment payment = new FiscallPrintJason2.Payment();
                        payment.type = "electronically";
                        payment.sum = Convert.ToDouble(get_result_payment[1]);
                        check.payments.Add(payment);
                        //fiscal.Payment(Convert.ToDouble(result[1]), 3, out remainder, out change);
                    }
                    if (get_result_payment[2] != 0)
                    {
                        FiscallPrintJason2.Payment payment = new FiscallPrintJason2.Payment();
                        payment.type = "prepaid";
                        payment.sum = Convert.ToDouble(get_result_payment[2]);
                        check.payments.Add(payment);
                        //fiscal.Payment(false, Convert.ToDouble(result[2]), 4, out remainder, out change);
                    }


                    if (MainStaticClass.GetWorkSchema == 2)
                    {
                        if (client.Tag != null)
                        {
                            //здесь блок по бонусам
                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "Перерасчет БОНУСОВ";
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "ПОСЛЕ ВОЗВРАТА  ТОВАРА";
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "СПИСАНО " + bonuses_it_is_written_off.ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "СОСТОЯНИЕ СЧЕТА БОНУСОВ";
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "ORANGE CARD";
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "КАРТА #: " + client.Tag.ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "PRN: " + id_transaction;
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "БАЛАНС: " + bonus_total_centr.ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "СПИСАНО -" + bonuses_it_is_written_off.ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "ДОСТУПНО " + (bonus_total_centr - bonuses_it_is_written_off).ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);
                        }

                    }

                    //if (result[2] != 0)
                    //{
                    //    fiscal.Payment(Convert.ToDouble(result[2]), 4, out remainder, out change);
                    //}
                    pi = new FiscallPrintJason2.PostItem();
                    pi.type = "text";
                    //pi.text = MainStaticClass.Nick_Shop + "-" + comment.Text.Trim();
                    pi.text = MainStaticClass.Nick_Shop + "-" + MainStaticClass.CashDeskNumber.ToString() + "-" + numdoc.ToString() + " (" + comment.Text.Trim() + ")";// +" кассир " + this.cashier;
                    pi.alignment = "left";
                    check.postItems.Add(pi);


                    //Оплата клиенту
                    //fiscal.CloseCheck(false, type_pay.SelectedIndex);
                    check.total = get_result_payment[0] + get_result_payment[1]+ get_result_payment[2];

                    try
                    {
                        Cash8.FiscallPrintJason2.RootObject result = FiscallPrintJason2.check_print("sellReturn", check, numdoc.ToString(),0,guid);

                        if (result.results[0].status == "ready")//Задание выполнено успешно 
                        {
                            its_print();
                        }
                        else
                        {
                            MessageBox.Show(" ФРЕГ Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                            MainStaticClass.write_event_in_log(" ФРЕГ  Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription, " Печать чека возврата ", numdoc.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }

                //catch (fptrsharp.FptrException ex)
                //{
                //    MessageBox.Show(ex.Message);
                //    error = true;
                //}
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //error = true;
                }
                finally
                {
                    //if (fiscal != null)
                    //{
                    //    fiscal.Dispose();
                    //}
                }
            }
            //else
            //{
            //    //string s = this.calculation_of_the_sum_of_the_document().ToString();
            //    text_print(output, output, "0", "0", "0", "0");
            //}


            this.Close();

        }

        private void fiscall_print_disburse_2_3(string cash_money, string non_cash_money, int variant,string print_guid)
        {

            if (MainStaticClass.SystemTaxation == 0)
            {
                MessageBox.Show("В константах не опрелена система налогобложения, печать чеков невозможна");
                return;
            }

            string output = calculation_of_the_sum_of_the_document().ToString();
            if (itsnew)
            {
                if (!write_new_document(output, output, "0", "0", true, cash_money, non_cash_money, "0", "0"))
                {
                    return;
                }
            }

            bool error = false;

            if (to_print_certainly == 1)
            {
                MainStaticClass.delete_document_wil_be_printed(numdoc.ToString());
            }

            if (MainStaticClass.get_document_wil_be_printed(numdoc.ToString()) != 0)
            {
                MessageBox.Show("Этот чек уже был успешно отправлен на печать");
                return;
            }

            if (MainStaticClass.Use_Fiscall_Print)
            {
                //fptrsharp.Fptr fiscal = null;
                closing = false;

                FiscallPrintJason2.Check check = new FiscallPrintJason2.Check();
                check.type = "sellReturn";
                //check.taxationType = (MainStaticClass.SystemTaxation == 1 ? "osn" : "usnIncomeOutcome");
                if (variant == 0)
                {
                    check.taxationType = "patent";
                }
                else if (variant == 1)
                {
                    check.taxationType = "usnIncomeOutcome";
                }
                else
                {
                    MessageBox.Show("В печать не передан вариант печати, печать невозможна");
                    return;
                }

                check.ignoreNonFiscalPrintErrors = false;
                check.validateMarkingCodes = false;
                check.@operator = new FiscallPrintJason.Operator();
                check.@operator.name = MainStaticClass.Cash_Operator;
                check.@operator.vatin = MainStaticClass.CashOperatorInn;
                check.items = new List<Cash8.FiscallPrintJason2.Item>();

                FiscallPrintJason2.PostItem pi = new FiscallPrintJason2.PostItem();
                check.postItems = new List<FiscallPrintJason2.PostItem>();

                try
                {
                    //Cash8.Work_FPTK22 fptk22 = new Work_FPTK22(1);
                    //fiscal = fptk22.get_FPTK22;
                    int nomer_naloga = 0;
                    string tax_type = "";
                    foreach (ListViewItem lvi in listView1.Items)
                    {
                        if (Convert.ToDouble(lvi.SubItems[7].Text) != 0)
                        {
                            if (variant == 0)
                            {
                                if (lvi.SubItems[14].Text.Trim().Length > 13)
                                {
                                    continue;
                                }
                            }
                            else if (variant == 1)
                            {
                                if (lvi.SubItems[14].Text.Trim().Length < 14)
                                {
                                    continue;
                                }
                            }
                            //fiscal.Return(false, 2, 1, lvi.SubItems[0].Text.Trim() + " " + lvi.SubItems[1].Text.Trim(), Convert.ToDouble(lvi.SubItems[3].Text), Convert.ToDouble(lvi.SubItems[5].Text), 0, true);
                            //int stavka_nds = get_tovar_nds(lvi.SubItems[0].Text.Trim());
                            //int nomer_naloga = 0;
                            //if (!MainStaticClass.Use_Envd)
                            //{
                            int stavka_nds = get_tovar_nds(lvi.SubItems[0].Text.Trim());
                            //int nomer_naloga = 0;
                            if (MainStaticClass.SystemTaxation == 1)
                            {
                                if (stavka_nds == 0)
                                {
                                    nomer_naloga = 1;
                                    tax_type = "vat0";
                                }
                                else if (stavka_nds == 10)
                                {
                                    nomer_naloga = 2;
                                    tax_type = "vat10";
                                }
                                else if (stavka_nds == 18)
                                {
                                    nomer_naloga = 3;
                                    tax_type = "vat20";
                                }
                                else if (stavka_nds == 20)
                                {
                                    nomer_naloga = 3;
                                    tax_type = "vat20";
                                }
                                else
                                {
                                    MessageBox.Show("Неизвестная ставка ндс");
                                }
                            }
                            else
                            {
                                nomer_naloga = 4;
                                tax_type = "none";
                            }
                            //}
                            //else
                            //{
                            //    nomer_naloga = 4;
                            //    tax_type = "none";
                            //}

                            FiscallPrintJason2.Item item = new FiscallPrintJason2.Item();
                            item.name = lvi.SubItems[0].Text.Trim() + " " + lvi.SubItems[1].Text.Trim();// "Первая Позиция";
                            item.price = Convert.ToDouble(lvi.SubItems[5].Text);// 1;
                            item.quantity = Convert.ToDouble(lvi.SubItems[3].Text);
                            item.type = "position";
                            item.amount = Convert.ToDouble(lvi.SubItems[7].Text);
                            item.tax = new FiscallPrintJason2.Tax();
                            item.tax.type = tax_type;//ндс
                            item.measurementUnit = MainStaticClass.check_fractional_tovar(lvi.SubItems[0].Text.Trim());// "piece";
                            if (lvi.SubItems[14].Text.Trim().Length <= 13)//Код маркировки не заполнен
                            {
                                item.paymentObject = "commodityWithoutMarking";
                            }
                            else//иначе передаем код маркировки 
                            {
                                item.paymentObject = "commodityWithMarking";
                                FiscallPrintJason2.ImcParams imcParams = new FiscallPrintJason2.ImcParams();
                                imcParams.imcType = "auto";
                                byte[] textAsBytes = Encoding.Default.GetBytes(lvi.SubItems[14].Text.Trim());
                                //string mark_str = Convert.ToBase64String(textAsBytes);
                                //imcParams.imc = mark_str;
                                imcParams.imc = Convert.ToBase64String(textAsBytes);
                                imcParams.itemEstimatedStatus = "itemPieceSold";
                                imcParams.imcModeProcessing = 0;
                                item.imcParams = imcParams;
                            }

                            check.items.Add(item);
                        }
                    }

                    if (check.items.Count == 0)
                    {
                        //здесь необходимо поставить флаг распечатан
                        its_print_p(variant);
                        return;
                    }

                    check.clientInfo = new FiscallPrintJason2.ClientInfo();
                    if (txtB_email_telephone.Text.Trim().Length > 0)
                    {
                        check.clientInfo.emailOrPhone = txtB_email_telephone.Text;
                    }

                    if ((txtB_inn.Text.Trim().Length > 0) && (txtB_name.Text.Trim().Length > 0))
                    {
                        check.clientInfo.vatin = txtB_inn.Text;
                        check.clientInfo.name = txtB_name.Text;
                    }
                                        
                    double[] get_result_payment = get_summ1_systemtaxation3(variant);
                    check.payments = new List<FiscallPrintJason2.Payment>();
                    if (get_result_payment[0] != 0)//Наличные
                    {
                        FiscallPrintJason2.Payment payment = new FiscallPrintJason2.Payment();
                        payment.type = "cash";
                        payment.sum = Convert.ToDouble(get_result_payment[0]);
                        check.payments.Add(payment);
                        //fiscal.Payment(Convert.ToDouble(result[0]), 0, out remainder, out change);
                    }
                    if (get_result_payment[1] != 0)
                    {
                        FiscallPrintJason2.Payment payment = new FiscallPrintJason2.Payment();
                        payment.type = "electronically";
                        payment.sum = Convert.ToDouble(get_result_payment[1]);
                        check.payments.Add(payment);
                        //fiscal.Payment(Convert.ToDouble(result[1]), 3, out remainder, out change);
                    }
                    if (get_result_payment[2] != 0)
                    {
                        FiscallPrintJason2.Payment payment = new FiscallPrintJason2.Payment();
                        payment.type = "prepaid";
                        payment.sum = Convert.ToDouble(get_result_payment[2]);
                        check.payments.Add(payment);
                        //fiscal.Payment(false, Convert.ToDouble(result[2]), 4, out remainder, out change);
                    }


                    if (MainStaticClass.GetWorkSchema == 2)
                    {
                        if (client.Tag != null)
                        {
                            //здесь блок по бонусам
                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "Перерасчет БОНУСОВ";
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "ПОСЛЕ ВОЗВРАТА  ТОВАРА";
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "СПИСАНО " + bonuses_it_is_written_off.ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "СОСТОЯНИЕ СЧЕТА БОНУСОВ";
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "ORANGE CARD";
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "КАРТА #: " + client.Tag.ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "PRN: " + id_transaction;
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "БАЛАНС: " + bonus_total_centr.ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "СПИСАНО -" + bonuses_it_is_written_off.ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);

                            pi = new FiscallPrintJason2.PostItem();
                            pi.type = "text";
                            pi.text = "ДОСТУПНО " + (bonus_total_centr - bonuses_it_is_written_off).ToString();
                            pi.alignment = "left";
                            check.postItems.Add(pi);
                        }

                    }

                    //if (result[2] != 0)
                    //{
                    //    fiscal.Payment(Convert.ToDouble(result[2]), 4, out remainder, out change);
                    //}
                    pi = new FiscallPrintJason2.PostItem();
                    pi.type = "text";
                    //pi.text = MainStaticClass.Nick_Shop + "-" + comment.Text.Trim();
                    pi.text = MainStaticClass.Nick_Shop + "-" + MainStaticClass.CashDeskNumber.ToString() + "-" + numdoc.ToString() + " (" + comment.Text.Trim() + ")";// +" кассир " + this.cashier;
                    pi.alignment = "left";
                    check.postItems.Add(pi);


                    //Оплата клиенту
                    //fiscal.CloseCheck(false, type_pay.SelectedIndex);
                    check.total = get_result_payment[0] + get_result_payment[1] + get_result_payment[2];

                    try
                    {
                        if (variant == 1)
                        {
                            clearMarkingCodeValidationResult();

                            int count_km = 0;
                            foreach (ListViewItem lvi in listView1.Items)
                            {
                                if (lvi.SubItems[14].Text.Trim().Length > 13)
                                {
                                    count_km++;
                                }
                            }
                            //bool continue_print = true;
                            int code_error = 0;// continue_print = true;
                            if (count_km != 0)
                            {
                                code_error = checking_km_before_adding_to_buffer();
                                //this.answerAddingKmArrayToTableTested = null;
                                //continue_print = checking_km_before_adding_to_buffer(ref this.answerAddingKmArrayToTableTested);
                                check_imc_work_state(count_km);
                                if (code_error == 0)
                                {
                                    //continue_print = check_imc_work_state(count_km);//Проверка соответсвия состояния буфера в ФН и 
                                }
                                //else
                                //{
                                //    //return;
                                //}
                            }
                        }
                        Cash8.FiscallPrintJason2.RootObject result = FiscallPrintJason2.check_print("sellReturn", check, numdoc.ToString(), 0, print_guid);

                        if (result.results[0].status == "ready")//Задание выполнено успешно 
                        {                            
                            its_print_p(variant);
                            MainStaticClass.write_event_in_log(" Чек успешно распечатан ", "Документ чек", numdoc.ToString());
                        }
                        else
                        {
                            MessageBox.Show(" ФРЕГ Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                            MainStaticClass.write_event_in_log(" ФРЕГ  Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription, " Печать чека возврата ", numdoc.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }                
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    error = true;
                }
                finally
                {
                    //if (fiscal != null)
                    //{
                    //    fiscal.Dispose();
                    //}
                }
            }
            //else
            //{
            //    //string s = this.calculation_of_the_sum_of_the_document().ToString();
            //    text_print(output, output, "0", "0", "0", "0");
            //}
            this.Close();
        }

        private void fiscall_print_disburse(string cash_money, string non_cash_money)
        {
            if (MainStaticClass.GetVersionFn == 1)
            {
                fiscall_print_disburse_1(cash_money, non_cash_money);
            }
            else if (MainStaticClass.GetVersionFn == 2)
            {
                if (MainStaticClass.PrintingUsingLibraries == 0)
                {
                    int count_km = 0;
                    foreach (ListViewItem lvi in listView1.Items)
                    {
                        if (lvi.SubItems[14].Text.Trim().Length > 13)
                        {
                            count_km++;
                        }
                    }
                    bool continue_print = true;
                    if (count_km != 0)
                    {
                        continue_print = check_imc_work_state(count_km);//Проверка соответсвия состояния буфера в ФН и 
                    }
                    if (!continue_print)
                    {
                        return;
                    }
                }
                if (MainStaticClass.SystemTaxation == 3)
                {
                    if (MainStaticClass.PrintingUsingLibraries == 0)
                    {
                        this.fiscall_print_disburse_2_3(cash_money, non_cash_money, 0, this.guid);
                        this.fiscall_print_disburse_2_3(cash_money, non_cash_money, 1, this.guid1);
                    }
                    else
                    {
                        string sum_pay = this.calculation_of_the_sum_of_the_document().ToString();
                        if (itsnew)
                        {
                            write_new_document(sum_pay, sum_pay, "0", "0", true, cash_money, non_cash_money, "0", "0");
                        }
                        PrintingUsingLibraries printingUsingLibraries = new PrintingUsingLibraries();
                        printingUsingLibraries.print_sell_2_3_or_return_sell(this, 0);
                        printingUsingLibraries.print_sell_2_3_or_return_sell(this, 1);
                        this.Close();
                    }
                }
                else
                {
                    if (MainStaticClass.PrintingUsingLibraries == 0)
                    {
                        this.fiscall_print_disburse_2(cash_money, non_cash_money);
                    }
                    else
                    {
                        string sum_pay = this.calculation_of_the_sum_of_the_document().ToString();
                        if (itsnew)
                        {
                            write_new_document(sum_pay, sum_pay, "0", "0", true, cash_money, non_cash_money, "0", "0");
                        }
                        //if (this.itsnew)
                        //{
                            PrintingUsingLibraries printingUsingLibraries = new PrintingUsingLibraries();
                            printingUsingLibraries.print_sell_2_or_return_sell(this);
                        //}
                        this.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Не удалось определить версию ФН, печать чеков невозможна");
            }

        }
        //{

        //    if (MainStaticClass.SystemTaxation == 0)
        //    {
        //        MessageBox.Show("В константах не опрелена система налогобложения, печать чеков невозможна");
        //        return;
        //    }

        //    string output = calculation_of_the_sum_of_the_document().ToString();
        //    if (itsnew)
        //    {
        //        if (!write_new_document(output, output, "0", "0", true, cash_money, non_cash_money, "0","0"))
        //        {
        //            return;
        //        }
        //    }

        //    bool error = false;

        //    if (to_print_certainly == 1)
        //    {
        //        MainStaticClass.delete_document_wil_be_printed(numdoc.ToString());
        //    }

        //    if (MainStaticClass.get_document_wil_be_printed(numdoc.ToString()) != 0)
        //    {
        //        MessageBox.Show("Этот чек уже был успешно отправлен на печать");
        //        return;
        //    }

        //    if (MainStaticClass.Use_Fiscall_Print)
        //    {
        //        //fptrsharp.Fptr fiscal = null;
        //        closing = false;

        //        FiscallPrintJason.Check check = new FiscallPrintJason.Check();
        //        check.type = "sellReturn";                
        //        check.taxationType = (MainStaticClass.SystemTaxation==1 ? "osn" : "usnIncomeOutcome");

        //        check.ignoreNonFiscalPrintErrors = false;
        //        check.@operator = new FiscallPrintJason.Operator();
        //        check.@operator.name = MainStaticClass.Cash_Operator;
        //        check.@operator.vatin = MainStaticClass.cash_operator_inn;
        //        check.items = new List<Cash8.FiscallPrintJason.Item>();

        //        FiscallPrintJason.PostItem pi = new FiscallPrintJason.PostItem();
        //        check.postItems = new List<FiscallPrintJason.PostItem>();

        //        try
        //        {
        //            //Cash8.Work_FPTK22 fptk22 = new Work_FPTK22(1);
        //            //fiscal = fptk22.get_FPTK22;
        //            int nomer_naloga = 0;
        //            string tax_type = "";
        //            foreach (ListViewItem lvi in listView1.Items)
        //            {
        //                if (Convert.ToDouble(lvi.SubItems[7].Text) != 0)
        //                {
        //                    //fiscal.Return(false, 2, 1, lvi.SubItems[0].Text.Trim() + " " + lvi.SubItems[1].Text.Trim(), Convert.ToDouble(lvi.SubItems[3].Text), Convert.ToDouble(lvi.SubItems[5].Text), 0, true);
        //                    //int stavka_nds = get_tovar_nds(lvi.SubItems[0].Text.Trim());
        //                    //int nomer_naloga = 0;
        //                    //if (!MainStaticClass.Use_Envd)
        //                    //{
        //                        int stavka_nds = get_tovar_nds(lvi.SubItems[0].Text.Trim());
        //                    //int nomer_naloga = 0;
        //                    if (MainStaticClass.SystemTaxation==1)
        //                    {
        //                        if (stavka_nds == 0)
        //                        {
        //                            nomer_naloga = 1;
        //                            tax_type = "vat0";
        //                        }
        //                        else if (stavka_nds == 10)
        //                        {
        //                            nomer_naloga = 2;
        //                            tax_type = "vat10";
        //                        }
        //                        else if (stavka_nds == 18)
        //                        {
        //                            nomer_naloga = 3;
        //                            tax_type = "vat20";
        //                        }
        //                        else if (stavka_nds == 20)
        //                        {
        //                            nomer_naloga = 3;
        //                            tax_type = "vat20";
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show("Неизвестная ставка ндс");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        nomer_naloga = 4;
        //                        tax_type = "none";
        //                    }                                
        //                    //}
        //                    //else
        //                    //{
        //                    //    nomer_naloga = 4;
        //                    //    tax_type = "none";
        //                    //}

        //                    FiscallPrintJason.Item item = new FiscallPrintJason.Item();
        //                    item.name = lvi.SubItems[0].Text.Trim() + " " + lvi.SubItems[1].Text.Trim();// "Первая Позиция";
        //                    item.price = Convert.ToDouble(lvi.SubItems[5].Text);// 1;
        //                    item.quantity = Convert.ToDouble(lvi.SubItems[3].Text);
        //                    item.type = "position";
        //                    item.amount = Convert.ToDouble(lvi.SubItems[7].Text);
        //                    item.tax = new FiscallPrintJason.Tax();
        //                    item.tax.type = tax_type;//ндс
        //                    if (lvi.SubItems[14].Text.Trim().Length > 1)
        //                    {
        //                        //FiscallPrintJason.MarkingCode markingCode = new FiscallPrintJason.MarkingCode();
        //                        //byte[] textAsBytes = Encoding.UTF8.GetBytes(lvi.SubItems[14].Text.Trim());
        //                        //string mark_str = Convert.ToBase64String(textAsBytes);
        //                        //markingCode.mark = mark_str.Substring(0, mark_str.Length - 1);
        //                        //item.markingCode = markingCode;
        //                        FiscallPrintJason.NomenclatureCode nomenclatureCode = new FiscallPrintJason.NomenclatureCode();
        //                        nomenclatureCode.gtin = lvi.SubItems[14].Text.Trim().Substring(3, 14);
        //                        nomenclatureCode.serial = lvi.SubItems[14].Text.Trim().Substring(18, 13);
        //                        nomenclatureCode.type = "shoes";
        //                        item.nomenclatureCode = nomenclatureCode;
        //                        if (MainStaticClass.get_print_m())
        //                        {
        //                            item.name = "[M] "+item.name;
        //                        }
        //                    }

        //                    check.items.Add(item);
        //                }
        //            }


        //            check.clientInfo = new FiscallPrintJason.ClientInfo();
        //            if (txtB_email_telephone.Text.Trim().Length > 0)
        //            {
        //                check.clientInfo.emailOrPhone = txtB_email_telephone.Text;
        //            }

        //            if ((txtB_inn.Text.Trim().Length > 0) && (txtB_name.Text.Trim().Length > 0))
        //            {
        //                check.clientInfo.vatin = txtB_inn.Text;
        //                check.clientInfo.name = txtB_name.Text;
        //            }



        //            double[] get_result_payment = get_cash_on_type_payment();
        //            check.payments = new List<FiscallPrintJason.Payment>();
        //            if (get_result_payment[0] != 0)//Наличные
        //            {
        //                FiscallPrintJason.Payment payment = new FiscallPrintJason.Payment();
        //                payment.type = "cash";
        //                payment.sum = Convert.ToDouble(get_result_payment[0]);
        //                check.payments.Add(payment);
        //                //fiscal.Payment(Convert.ToDouble(result[0]), 0, out remainder, out change);
        //            }
        //            if (get_result_payment[1] != 0)
        //            {
        //                FiscallPrintJason.Payment payment = new FiscallPrintJason.Payment();
        //                payment.type = "electronically";
        //                payment.sum = Convert.ToDouble(get_result_payment[1]);
        //                check.payments.Add(payment);
        //                //fiscal.Payment(Convert.ToDouble(result[1]), 3, out remainder, out change);
        //            }


        //            if (MainStaticClass.GetWorkSchema == 2)
        //            {
        //                //здесь блок по бонусам
        //                pi = new FiscallPrintJason.PostItem();
        //                pi.type = "text";
        //                pi.text = "Перерасчет БОНУСОВ";
        //                pi.alignment = "left";
        //                check.postItems.Add(pi);

        //                pi = new FiscallPrintJason.PostItem();
        //                pi.type = "text";
        //                pi.text = "ПОСЛЕ ВОЗВРАТА  ТОВАРА";
        //                pi.alignment = "left";
        //                check.postItems.Add(pi);

        //                pi = new FiscallPrintJason.PostItem();
        //                pi.type = "text";
        //                pi.text = "СПИСАНО "+bonuses_it_is_written_off.ToString();
        //                pi.alignment = "left";
        //                check.postItems.Add(pi);

        //                pi = new FiscallPrintJason.PostItem();
        //                pi.type = "text";
        //                pi.text = "СОСТОЯНИЕ СЧЕТА БОНУСОВ";
        //                pi.alignment = "left";
        //                check.postItems.Add(pi);

        //                pi = new FiscallPrintJason.PostItem();
        //                pi.type = "text";
        //                pi.text = "ORANGE CARD";
        //                pi.alignment = "left";
        //                check.postItems.Add(pi);

        //                pi = new FiscallPrintJason.PostItem();
        //                pi.type = "text";
        //                pi.text = "КАРТА #: "+client.Tag.ToString();
        //                pi.alignment = "left";
        //                check.postItems.Add(pi);

        //                pi = new FiscallPrintJason.PostItem();
        //                pi.type = "text";
        //                pi.text = "PRN: " + id_transaction;
        //                pi.alignment = "left";
        //                check.postItems.Add(pi);

        //                pi = new FiscallPrintJason.PostItem();
        //                pi.type = "text";
        //                pi.text = "БАЛАНС: "+ bonus_total_centr.ToString(); 
        //                pi.alignment = "left";
        //                check.postItems.Add(pi);

        //                pi = new FiscallPrintJason.PostItem();
        //                pi.type = "text";
        //                pi.text = "СПИСАНО -" + bonuses_it_is_written_off.ToString();
        //                pi.alignment = "left";
        //                check.postItems.Add(pi);

        //                pi = new FiscallPrintJason.PostItem();
        //                pi.type = "text";
        //                pi.text = "ДОСТУПНО " + (bonus_total_centr - bonuses_it_is_written_off).ToString();
        //                pi.alignment = "left";
        //                check.postItems.Add(pi);

        //            }

        //            //if (result[2] != 0)
        //            //{
        //            //    fiscal.Payment(Convert.ToDouble(result[2]), 4, out remainder, out change);
        //            //}
        //            pi = new FiscallPrintJason.PostItem();
        //            pi.type = "text";
        //            //pi.text = MainStaticClass.Nick_Shop + "-" + comment.Text.Trim();
        //            pi.text = MainStaticClass.Nick_Shop + "-" + MainStaticClass.CashDeskNumber.ToString() + "-" + numdoc.ToString()+ " (" + comment.Text.Trim()+")";// +" кассир " + this.cashier;
        //            pi.alignment = "left";
        //            check.postItems.Add(pi);


        //            //Оплата клиенту
        //            //fiscal.CloseCheck(false, type_pay.SelectedIndex);
        //            check.total = get_result_payment[0] + get_result_payment[1];

        //            try
        //            {
        //                Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.check_print("sellReturn", check,numdoc.ToString());

        //                if (result.results[0].status == "ready")//Задание выполнено успешно 
        //                {
        //                    its_print();
        //                }
        //                else
        //                {
        //                    MessageBox.Show(" ФРЕГ Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
        //                    MainStaticClass.write_event_in_log(" ФРЕГ  Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription, " Печать чека возврата ", numdoc.ToString());
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message);
        //            }

        //        }

        //        //catch (fptrsharp.FptrException ex)
        //        //{
        //        //    MessageBox.Show(ex.Message);
        //        //    error = true;
        //        //}
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //            error = true;
        //        }
        //        finally
        //        {
        //            //if (fiscal != null)
        //            //{
        //            //    fiscal.Dispose();
        //            //}
        //        }
        //    }
        //    //else
        //    //{
        //    //    //string s = this.calculation_of_the_sum_of_the_document().ToString();
        //    //    text_print(output, output, "0", "0", "0", "0");
        //    //}


        //    this.Close();

        //}




        /*Возвращает количество пустых строк 
         * которое необходимо добавить в конец чека
         * нужно для тех принтеров которые 
         * не протягивают бумагу до конца
         */
        private int quantity_of_empty_lines()
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            int result = 0;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT quantity_of_empty_lines FROM constants";
                command = new NpgsqlCommand(query, conn);
                result = Convert.ToInt32(command.ExecuteScalar());
                conn.Close();
            }
            catch (NpgsqlException ex)
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

        private string probel(int count)
        {
            string result = "";
            for (int i = 0; i < count * 2; i++)
            {
                result = result + " ";
            }
            return result;
        }


        //private void pd_PrintPage(object sender, PrintPageEventArgs e)
        //{
        //    if (count_pages < print_data.Length - 1)
        //    {
        //        count_pages++;
        //        e.HasMorePages = true;
        //    }
        //    else
        //    {
        //        e.HasMorePages = false;
        //    }
        //    e.Graphics.DrawString(print_data[count_pages].ToString(), Font, new SolidBrush(Color.Black), new RectangleF(5, 5, 400, 120000));

        //}

        private void listView1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            if (!itsnew)
            {
                return;
            }
            if (e.KeyChar == 13)//Нажата клавиша enter
            {

                if (MainStaticClass.GetWorkSchema == 2)
                {
                    return;
                }
                ListView view = (ListView)sender;
                if (check_tovar_fractional(view.SelectedItems[0].SubItems[0].Text))
                {
                    numericUpDown_enter_quantity.DecimalPlaces = 3;
                }
                else
                {
                    numericUpDown_enter_quantity.DecimalPlaces = 0;
                }

                //Если это фискальни тогда
                //if (MainStaticClass.Use_Fiscall_Print)
                //{
                if (this.check_type.SelectedIndex == 1)
                {
                    this.panel_return.Location = new System.Drawing.Point(this.listView1.Bounds.Location.X + this.listView1.Columns[0].Width + this.listView1.Columns[1].Width, this.listView1.Bounds.Location.Y + this.listView1.SelectedIndices[0] * this.listView1.SelectedItems[0].Bounds.Height);
                    //this.listView1.SelectedItems[0].SubItems[3].Text;
                    this.return_quantity.Text = this.listView1.SelectedItems[0].SubItems[3].Text;
                    string price = this.listView1.SelectedItems[0].SubItems[5].Text.Trim();

                    if (price.Length > 0)
                    {

                        int num_pos = price.IndexOf(",");
                        this.return_rouble.Text = price.Substring(0, num_pos);
                        this.return_kop.Text = price.Substring(num_pos + 1, 2);

                        //this.listView1.SelectedItems[0].SubItems[4].Text = this.return_rouble.Text.Trim() + "," + this.return_kop.Text.Trim();

                    }

                    this.panel_return.Visible = true;
                    this.panel_return.BringToFront();
                    //this.panel_return.
                    this.return_quantity.Focus();
                    if (MainStaticClass.Code_right_of_user != 1)
                    {
                        return_rouble.Enabled = false;
                        return_kop.Enabled = false;
                    }
                    write_new_document("0", calculation_of_the_sum_of_the_document().ToString(), "0", "0", false, "0", "0", "0", "0");
                    return;
                }
                //}

                //Проверить что этот товар не маркированный иначе вовзрат ибо нельзя изменять количество в строке маркированного товара 
                if (this.listView1.SelectedItems[0].SubItems[14].Text.Trim() != "0")
                {
                    MessageBox.Show("Изменять количество для маркированного товара нельзя !!!");
                    return;
                }

                show_quantity_control(false);

                //this.numericUpDown_enter_quantity.Visible = true;
                //this.panel1.Visible = true;

                ////this.panel1.Location = new System.Drawing.Point(this.listView1.Bounds.Location.X + this.listView1.Columns[0].Width + this.listView1.Columns[1].Width, this.listView1.Bounds.Location.Y + this.listView1.SelectedIndices[0] * this.listView1.SelectedItems[0].Bounds.Height);
                //this.panel1.Location = new System.Drawing.Point(this.tabControl1.Location.X +
                //    this.listView1.Bounds.Location.X +
                //    this.listView1.Columns[0].Width +
                //    this.listView1.Columns[1].Width,
                //    //20+                   
                //    this.tabControl1.Location.Y+
                //    this.listView1.Bounds.Location.Y + (this.listView1.SelectedIndices[0]+1) * this.listView1.SelectedItems[0].Bounds.Height);

                //this.numericUpDown_enter_quantity.Value = Convert.ToDecimal(this.listView1.SelectedItems[0].SubItems[3].Text);
                //this.panel1.BringToFront();
                ////this.enter_quantity.BringToFront();
                //this.numericUpDown_enter_quantity.Focus();

                //TextBox tb = (TextBox)numericUpDown_enter_quantity.Controls[1];                                
                //tb.SelectionStart = 0;
                //tb.SelectionLength = tb.Text.Length;//gaa

                //write_new_document("0", "0", "0", "0", false);
            }
            else if (e.KeyChar == 110)
            {

            }
        }

        private void show_quantity_control(bool НоваяСтрока)
        {
            //this.numericUpDown_enter_quantity.Visible = true;
            //this.panel1.Visible = true;

            ////this.panel1.Location = new System.Drawing.Point(this.listView1.Bounds.Location.X + this.listView1.Columns[0].Width + this.listView1.Columns[1].Width, this.listView1.Bounds.Location.Y + this.listView1.SelectedIndices[0] * this.listView1.SelectedItems[0].Bounds.Height);
            //this.panel1.Location = new System.Drawing.Point(this.tabControl1.Location.X +
            //    this.listView1.Bounds.Location.X +
            //    this.listView1.Columns[0].Width +
            //    this.listView1.Columns[1].Width,
            //    //20+                   
            //    this.tabControl1.Location.Y +
            //    this.listView1.Bounds.Location.Y + (this.listView1.SelectedIndices[0] + 1) * this.listView1.SelectedItems[0].Bounds.Height);

            //this.numericUpDown_enter_quantity.Value = Convert.ToDecimal(this.listView1.SelectedItems[0].SubItems[3].Text);
            //this.panel1.BringToFront();
            ////this.enter_quantity.BringToFront();
            //this.numericUpDown_enter_quantity.Focus();

            //TextBox tb = (TextBox)numericUpDown_enter_quantity.Controls[1];
            //tb.SelectionStart = 0;
            //tb.SelectionLength = tb.Text.Length;
            //if (MainStaticClass.GetWeightAutomatically == 1)
            //{
            //    Dictionary<double, int> frequencyMap = new Dictionary<double, int>();
            //    if (MessageBox.Show("Ввод веса будет из весов ? ", "Истоник веса", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    {
            //        double weigt = 0;
            //        weigt = MainStaticClass.GetWeight();
            //        if (weigt > 0)
            //        {
            //            this.numericUpDown_enter_quantity.Value = Convert.ToDecimal(weigt);
            //        }
            //    }
            //}


            EnterQuantity enterQuantity = new EnterQuantity();
            if (check_tovar_fractional(this.listView1.SelectedItems[0].SubItems[0].Text))
            {
                enterQuantity.numericUpDown_enter_quantity.DecimalPlaces = 3;
            }
            else
            {
                enterQuantity.numericUpDown_enter_quantity.DecimalPlaces = 0;
            }
            
            DialogResult result = enterQuantity.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                if (НоваяСтрока)
                {
                    ReasonsDeletionCheck reasons = new ReasonsDeletionCheck();
                    DialogResult dialogResult = reasons.ShowDialog();
                    while (dialogResult != DialogResult.OK)
                    {
                        dialogResult = reasons.ShowDialog();
                    }
                    //if (dialogResult == DialogResult.OK)
                    //{
                    insert_incident_record(listView1.SelectedItems[0].SubItems[0].Text, listView1.SelectedItems[0].SubItems[3].Text, "0", reasons.reason);
                    listView1.Items.Remove(listView1.SelectedItems[0]);
                    //}                      
                }
            }
            else if (result == DialogResult.OK)
            {
                listView1.SelectedItems[0].SubItems[3].Text = enterQuantity.numericUpDown_enter_quantity.Value.ToString();
                recalculate_all();
                calculation_of_the_sum_of_the_document();
                if (listView1.Items.Count > 0)
                {
                    SendDataToCustomerScreen(1, 0, 1);
                }
            }
        }

        private bool check_tovar_fractional(string tovar_code)
        {
            bool result = false;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT fractional FROM tovar WHERE code ="+ tovar_code;
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToBoolean(command.ExecuteScalar());
                command.Dispose();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" Ошибка при получении признака весовой "+ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Ошибка при получении признака весовой " + ex.Message);
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

        private void clear_client_Click(object sender, EventArgs e)
        {
            client_barcode.Text = "";
            this.client.Text = "";
            this.client.Tag = "";
            this.Discount = 0;
            this.recalculate_all();
            //            show_discount_persent();
            calculation_of_the_sum_of_the_document();
        }

        private void update_comment()
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "UPDATE checks_header   SET comment='" + this.comment.Text.Trim() + "' WHERE date_time_start='" + date_time_start.Text + "'";
                command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                //conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    // conn.Dispose();
                }
            }

        }

        /// <summary>
        /// Функция возвращает значение флага напечатан для чека,
        /// при ошибке получения вернется истина
        /// </summary>
        /// <returns></returns>
        private bool itc_printed()
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            bool result = true;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT its_print  FROM checks_header WHERE date_time_write = '" + this.date_time_write + "'";
                command = new NpgsqlCommand(query, conn);
                object result_query = command.ExecuteScalar();

                if (result_query != DBNull.Value)
                {
                    result = Convert.ToBoolean(result_query);
                }
                else
                {
                    result = false;
                }

                conn.Close();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при получении флага распечатан " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении флага распечатан " + ex.Message);
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

        /// <summary>
        /// Функция возвращает значение флага напечатан для чека,
        /// при ошибке получения вернется истина
        /// </summary>
        /// <returns></returns>
        private bool itc_printed_p()
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            bool result = true;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT its_print_p  FROM checks_header WHERE date_time_write = '" + this.date_time_write + "'";
                command = new NpgsqlCommand(query, conn);
                object result_query = command.ExecuteScalar();

                if (result_query != DBNull.Value)
                {
                    result = Convert.ToBoolean(result_query);
                }
                else
                {
                    result = false;
                }

                conn.Close();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при получении флага распечатан по патенту " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении флага распечатан по патенту " + ex.Message);
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

        private void its_print()
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            NpgsqlTransaction trans = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                trans = conn.BeginTransaction();
                string query = " UPDATE checks_header   SET its_print=true WHERE document_number=" + numdoc.ToString() +";" + "UPDATE checks_header SET is_sent = 0 WHERE document_number = " + numdoc.ToString();//date_time_start='" + date_time_start.Text.Replace("Чек", "") + "'"; ;
                command = new NpgsqlCommand(query, conn);
                command.Transaction = trans;
                command.ExecuteNonQuery();

                query = " UPDATE checks_header   SET its_print_p=true WHERE document_number=" + numdoc.ToString() + ";" + "UPDATE checks_header SET is_sent = 0 WHERE document_number = " + numdoc.ToString(); //date_time_start='" + date_time_start.Text.Replace("Чек", "") + "'"; ;
                command = new NpgsqlCommand(query, conn);
                command.Transaction = trans;
                command.ExecuteNonQuery();

                query = " DELETE FROM document_wil_be_printed WHERE document_number=" + numdoc.ToString() + " AND tax_type=" + MainStaticClass.SystemTaxation.ToString();
                command = new NpgsqlCommand(query, conn);
                command.Transaction = trans;
                command.ExecuteNonQuery();
                trans.Commit();
                conn.Close();
                command.Dispose();
                trans.Dispose();
            }
            catch (NpgsqlException ex)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }
                MessageBox.Show("Ошибка при установке флага распечатан " + ex.Message);
            }
            catch (Exception ex)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }
                MessageBox.Show("Ошибка при установке флага распечатан " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Dispose();
            }
        }


        /// <summary>
        /// Устанвалоивает флаг 
        /// распечатан для налогообложения
        /// по схеме патент
        /// </summary>
        public void its_print_p(int variant)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            NpgsqlTransaction trans = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                trans = conn.BeginTransaction();

                string query = "";
                if (variant == 0)
                {
                    query = " UPDATE checks_header   SET its_print=true WHERE document_number=" + numdoc.ToString()+";" + "UPDATE checks_header SET is_sent = 0 WHERE document_number = " + numdoc.ToString();
                }
                else
                {
                    query = " UPDATE checks_header   SET its_print_p=true WHERE document_number=" + numdoc.ToString() + ";" + "UPDATE checks_header SET is_sent = 0 WHERE document_number = " + numdoc.ToString();
                }
                    
                command = new NpgsqlCommand(query, conn);
                command.Transaction = trans;
                command.ExecuteNonQuery();

                query = " DELETE FROM document_wil_be_printed WHERE document_number=" + numdoc.ToString()+ " AND tax_type ="+(MainStaticClass.SystemTaxation+variant).ToString();
                command = new NpgsqlCommand(query, conn);
                command.Transaction = trans;
                command.ExecuteNonQuery();
                trans.Commit();
                command.Dispose();
                conn.Close();

            }
            catch (NpgsqlException ex)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }
                MessageBox.Show("Ошибка при установке флага распечатан " + ex.Message);
            }
            catch (Exception ex)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }
                MessageBox.Show("Ошибка при установке флага распечатан " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();                    
                }
                conn.Dispose();
            }
        }
               
        private void pay_Click(object sender, EventArgs e)
        {
            recharge_note = "";
            print_to_button = 1;
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show(" Нет строк ", " Проверки переда записью документа ");
                return;
            }

            listView1.Focus();

            if (listView1.Items.Count == 0)
            {
                MessageBox.Show(" Нет строк ", " Проверки переда записью документа ");
                return;
            }

            if (get_its_deleted_document() == 1)
            {
                MessageBox.Show("Удаленный чек не может быть распечатан");
                return;
            }
            //Проверка ИНН и Наименования
            if ((txtB_inn.Text.Trim().Length != 0) || (txtB_name.Text.Trim().Length != 0))
            {
                if ((txtB_inn.Text.Trim().Length == 0) || (txtB_name.Text.Trim().Length == 0))
                {
                    MessageBox.Show("Если заполнен ИНН, то должно быть заполнено и наименование и наоборот");
                    return;
                }
            }

            if (MainStaticClass.SystemTaxation < 3)
            {
                if (to_print_certainly == 1)
                {
                    MainStaticClass.delete_document_wil_be_printed(numdoc.ToString());
                }

                if (MainStaticClass.get_document_wil_be_printed(numdoc.ToString()) != 0)
                {
                    MessageBox.Show("Этот чек уже был успешно отправлен на печать");
                    return;
                }
            }
            else if (MainStaticClass.SystemTaxation == 3)
            {
                if (check_type.SelectedIndex==1)//Комбинированный тип налогообложения необходимо проверить чтобы не было вместе маркированного и не маркированного товара
                {
                    //int marker = 0;
                    //int not_marker = 0;
                    //bool error = false;
                    //foreach (ListViewItem lvi in listView1.Items)
                    //{
                    //    if (Convert.ToDouble(lvi.SubItems[7].Text) != 0)
                    //    {
                    //        if (lvi.SubItems[14].Text.Trim().Length <= 13)//Код маркировки не заполнен
                    //        {
                    //            not_marker++;
                    //        }
                    //        else
                    //        {
                    //            marker++;
                    //        }
                    //    }
                    //    if ((marker != 0) && (not_marker != 0))
                    //    {
                    //        error = true;
                    //        break;
                    //    }
                    //}
                    //if (error)
                    //{
                    //    MessageBox.Show("При данном типе налогообложения необходимо возвращать товар раздельно т.е. маркированный товар одним чеком, а не маркированный товар другим чеком.");
                    //    return;
                    //}
                }
            }



            //ПРОВЕРКА МАССИВА КОДОВ МАРКИРОВКИ
            if (MainStaticClass.GetVersionFn == 2)
            {
                if ((MainStaticClass.SystemTaxation != 3) || (check_type.SelectedIndex == 1))
                {
                    //if ((MainStaticClass.Version2Marking == 0)||(this.check_type.SelectedIndex==1)||!itsnew)//старый механизм работы с макрировкой, для возвратов так же пока старая схема
                    if (MainStaticClass.GetVersionFn == 2 && (MainStaticClass.SystemTaxation != 3 || this.check_type.SelectedIndex == 1)
                        && (MainStaticClass.Version2Marking == 0 || this.check_type.SelectedIndex == 1 || !this.itsnew) && MainStaticClass.PrintingUsingLibraries == 0)
                    {
                        //System.IO.File.AppendAllText(Application.StartupPath.Replace("\\", "/") + "/" + "json.txt", DateTime.Now.ToString() + " clearMarkingBuffer \r\n");
                        clearMarkingCodeValidationResult();

                        int count_km = 0;
                        foreach (ListViewItem lvi in listView1.Items)
                        {
                            if (lvi.SubItems[14].Text.Trim().Length > 13)
                            {
                                count_km++;
                            }
                        }
                        //bool continue_print = true;
                        int code_error = 0;// continue_print = true;
                        if (count_km != 0)
                        {
                            code_error = checking_km_before_adding_to_buffer();
                            //this.answerAddingKmArrayToTableTested = null;
                            //continue_print = checking_km_before_adding_to_buffer(ref this.answerAddingKmArrayToTableTested);
                            check_imc_work_state(count_km);
                            if (code_error == 0)
                            {
                                //continue_print = check_imc_work_state(count_km);//Проверка соответсвия состояния буфера в ФН и 
                            }
                            //else
                            //{
                            //    //return;
                            //}
                        }
                    }
                }
                //if (!continue_print)
                //{
                //    //return;
                //}
            }
            //КОНЕЦ ПРОВЕРКИ МАССИВА КОДОВ МАРКИРОВКИ



            if (itsnew)
            {
                show_pay_form();
            }
            else
            {
                if (MainStaticClass.Use_Fiscall_Print)
                {
                    if (MainStaticClass.SystemTaxation != 3)
                    {
                        if (!itc_printed())
                        {
                            if (this.check_type.SelectedIndex == 0)
                            {
                                fiscall_print_pay(this.p_sum_doc);
                            }
                            else
                            {
                                fiscall_print_disburse(txtB_cash_money.Text, txtB_non_cash_money.Text);
                            }
                        }
                    }
                    else if (MainStaticClass.SystemTaxation == 3)
                    {
                        if (!itc_printed()|| !itc_printed_p())
                        {
                            if (this.check_type.SelectedIndex == 0)
                            {
                                fiscall_print_pay(this.p_sum_doc);
                            }
                            else
                            {
                                fiscall_print_disburse(txtB_cash_money.Text, txtB_non_cash_money.Text);
                            }
                        }

                    }
                }

                this.Close();
            }
        }


        #region actions

        /*
        * Определить есть ли акции в данный период 
         * по введенному шрихкоду
         * 
        */
        //private void to_define_the_action(string barcode)
        //{
        //    total_seconnds = 0;
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    short tip_action;// = 0;
        //    bool action_by_discount = false;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        string query = "SELECT tip,num_doc,persent,comment,code_tovar,sum,barcode,marker,action_by_discount FROM action_header WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end AND barcode='" + barcode + "' AND kind=1";
        //        command = new NpgsqlCommand(query, conn);
        //        NpgsqlDataReader reader = command.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            tip_action = reader.GetInt16(0);

        //            //Проверка акции по дисконту
        //            action_by_discount = reader.GetBoolean(8);
        //            if (action_by_discount)
        //            {
        //                if (client.Tag == null)
        //                {
        //                    continue;
        //                }
        //            }

        //            /* Обработать акцию по типу 1
        //            * первый тип это скидка на конкретный товар
        //            * если есть процент скидки то дается скидка 
        //            * иначе выдается сообщение о подарке*/
        //            if (tip_action == 1)
        //            {
        //                if (reader.GetDecimal(2) != 0)
        //                {
        //                    action_1(reader.GetInt32(1), reader.GetDecimal(2));//Дать скидку на эту позицию                                                 
        //                }
        //                else
        //                {
        //                    action_1(reader.GetInt32(1), reader.GetString(3), reader.GetInt16(7), reader.GetInt32(4)); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
        //                }
        //            }
        //            else if (tip_action == 2)
        //            {
        //                if (reader.GetDecimal(2) != 0)
        //                {
        //                    action_2(reader.GetInt32(1), reader.GetDecimal(2));//Дать скидку на эту позицию                                                 
        //                }
        //                else
        //                {
        //                    action_2(reader.GetInt32(1), reader.GetString(3), reader.GetInt32(7), reader.GetInt16(4)); //Сообщить о подарке                           
        //                }

        //            }
        //            else if (tip_action == 3)
        //            {
        //                //action_2(reader.GetInt32(1));
        //                if (reader.GetDecimal(2) != 0)
        //                {
        //                    action_3(reader.GetInt32(1), reader.GetDecimal(2), reader.GetDecimal(5));//Дать скидку на все позиции из списка позицию                                                 
        //                }
        //                else
        //                {
        //                    action_3(reader.GetInt32(1), reader.GetString(3), reader.GetDecimal(5), reader.GetInt16(7)); //Сообщить о подарке                           
        //                }
        //            }
        //            else if (tip_action == 4)
        //            {
        //                if (reader.GetDecimal(2) != 0)
        //                {
        //                    action_4(reader.GetInt32(1), reader.GetDecimal(2), reader.GetDecimal(5));
        //                }
        //                else
        //                {
        //                    action_4(reader.GetInt32(1), reader.GetString(3), reader.GetDecimal(5), reader.GetInt64(4));
        //                }
        //            }
        //            else if (tip_action == 5)//Этот тип акции работает только по предъявлению купона, то есть по штрихкоду
        //            {
        //                action_5(reader.GetInt32(1), reader.GetDecimal(5));
        //            }
        //            else if (tip_action == 7)
        //            {
        //                action_7(reader.GetInt32(1), reader.GetInt32(4));
        //            }
        //        }
        //        reader.Close();
        //        //conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке акций по шрихкоду");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //    //MessageBox.Show(total_seconnds.ToString());
        //}
               
        private bool actions_birthday()
        {

            bool result = false;


            if (client.Tag == null)
            {
                return result;
            }

            NpgsqlConnection conn = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                //string query = "SELECT COUNT(*)FROM clients WHERE code='" + client.Tag.ToString().Trim()+ "' AND date_of_birth between '" +
                //    DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd")+"' AND '"+DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd")+"'";


                string query = "SELECT COUNT(*)FROM clients WHERE code='" + client.Tag.ToString().Trim() + "' AND date_part('month',date_of_birth)=" + DateTime.Now.Date.Month +
                    " AND  date_part('day',date_of_birth) BETWEEN " + DateTime.Now.Date.AddDays(-1).Day.ToString() + " AND " + DateTime.Now.Date.AddDays(1).Day.ToString() +
                    " AND date_of_birth<>'01.01.1900'";

                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (Convert.ToInt16(command.ExecuteScalar()) != 0)
                {
                    result = true;
                }
                //conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message, "Акция день рождения");
                result = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Акция день рождения");
                result = false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    // conn.Dispose();
                }
            }

            return result;
        }


        /*перерсоздание временной таблицы для товаров
         *          
         */
        private bool check_and_create_checks_table_temp()
        {
            bool result = true;
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {

                conn.Open();
                string query = "SELECT COUNT(*) from information_schema.tables 		where table_schema='public' 	and table_name='checks_table_temp'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);

                if (Convert.ToInt16(command.ExecuteScalar()) == 0)
                {
                    query = "CREATE TABLE checks_table_temp( tovar bigint)WITH (  OIDS=FALSE);ALTER TABLE checks_table_temp  OWNER TO postgres;";

                }
                else
                {
                    query = "DROP TABLE checks_table_temp;CREATE TABLE checks_table_temp( tovar bigint)WITH (  OIDS=FALSE);ALTER TABLE checks_table_temp  OWNER TO postgres;";
                }

                command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();

                StringBuilder sb = new StringBuilder();
                foreach (ListViewItem lvi in listView1.Items)
                {
                    sb.Append("INSERT INTO checks_table_temp(tovar)VALUES (" + lvi.SubItems[0].Text + ");");
                }

                query = sb.ToString();
                command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();

                conn.Close();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
                result = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = false;

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

        private bool check_tovar_in_action(string num_doc_l)
        {

            bool result = true;


            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = " SELECT COUNT(*) FROM action_table WHERE code_tovar in (SELECT tovar  FROM checks_table_temp) AND num_doc=" + num_doc_l;
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                {
                    result = false;
                }
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
                result = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = false;
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

        private bool check_tovar_in_action()
        {

            bool result = true;


            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = " SELECT COUNT(*) FROM action_table WHERE code_tovar in (SELECT tovar  FROM checks_table_temp)";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                {
                    result = false;
                }
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
                result = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = false;
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

        #region action_dt        
        /// <summary>
        /// Здесь происходит обработка по всем 
        /// регулярным акциям течто со штрихкодом
        /// и те что без штрихкода        
        /// </summary>
        /// <param name="show_messages"></param>
        /// <returns></returns>
        private DataTable to_define_the_action_dt(bool show_messages)
        {
            DataTable dataTable = null;
            if (!itsnew)
            {
                return dataTable;
            }
            if (this.check_type.SelectedIndex > 0)
            {
                return dataTable;
            }
            ProcessingOfActions processingOfActions = new ProcessingOfActions();            
            processingOfActions.dt = processingOfActions.create_dt(listView1);
            processingOfActions.show_messages = show_messages;
            MainStaticClass.write_event_in_log(" Попытка обработать акции по штрихкодам ", "Документ чек", numdoc.ToString());
            foreach (string barcode in action_barcode_list)
            {
                processingOfActions.to_define_the_action_dt(barcode);
            }
            
            if (client.Tag != null)
            {
                processingOfActions.to_define_the_action_personal_dt(this.client.Tag.ToString());
            }

            processingOfActions.to_define_the_action_dt();

            dataTable = processingOfActions.dt;

            if (show_messages)//если с показом сообщений, то это уже боевой режим 
            {
                have_action = processingOfActions.have_action;
            }

            return dataTable;
        }       

        /*Эта акция срабатывает когда сумма без скидки в документе >= сумме акции
        * тогда дается скидка на те позиции которые перечисляются в условии акции
        */

        #endregion

        /*
         * Определить есть ли акции в данный период
         */
        //private void to_define_the_action()
        //{
        //    if (!check_and_create_checks_table_temp())
        //    {
        //        return;
        //    }

        //    //if (!check_tovar_in_action())
        //    //{
        //    //    return;
        //    //}

        //    total_seconnds = 0;
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    short tip_action;// = 0;
        //    Int64 count_minutes = Convert.ToInt64((DateTime.Now - DateTime.Now.Date).TotalMinutes);
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        string query = "SELECT tip,num_doc,persent,comment,code_tovar,sum,barcode,marker,execution_order FROM action_header " +
        //            " WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end " +
        //            " AND " + count_minutes.ToString() + " between time_start AND time_end AND bonus_promotion=0 " +
        //            " AND barcode='' AND kind=0 AND tip<>10 AND num_doc in(" +//AND tip<>10 
        //            " SELECT DISTINCT action_table.num_doc FROM checks_table_temp " +
        //            " LEFT JOIN action_table ON checks_table_temp.tovar = action_table.code_tovar) order by execution_order asc, tip asc";//date_started asc,, tip desc

        //        command = new NpgsqlCommand(query, conn);
        //        NpgsqlDataReader reader = command.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            //listView1.Focus();
        //            if (reader.GetString(6).Trim().Length != 0)
        //            {
        //                continue;
        //            }

        //            //if (!check_tovar_in_action(reader[1].ToString()))
        //            //{
        //            //    continue;
        //            //}

        //            tip_action = reader.GetInt16(0);
        //            /* Обработать акцию по типу 1
        //            * первый тип это скидка на конкретный товар
        //            * если есть процент скидки то дается скидка 
        //            * иначе выдается сообщение о подарке*/
        //            if (tip_action == 1)
        //            {
        //                //start_action = DateTime.Now;
        //                if (reader.GetDecimal(2) != 0)
        //                {
        //                    action_1(reader.GetInt32(1), reader.GetDecimal(2));//Дать скидку на эту позицию                                                 
        //                }
        //                else
        //                {
        //                    action_1(reader.GetInt32(1), reader.GetString(3), reader.GetInt16(7), reader.GetInt64(4)); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
        //                }
        //                //write_time_execution(reader[1].ToString(), tip_action.ToString());
        //            }
        //            else if (tip_action == 2)
        //            {
        //                //start_action = DateTime.Now;

        //                if (reader.GetDecimal(2) != 0)
        //                {
        //                    action_2(reader.GetInt32(1), reader.GetDecimal(2));//Дать скидку на эту позицию                                                 
        //                }
        //                else
        //                {
        //                    action_2(reader.GetInt32(1), reader.GetString(3), reader.GetInt16(7), reader.GetInt64(4)); //Сообщить о подарке                           
        //                }
        //                //write_time_execution(reader[1].ToString(), tip_action.ToString());

        //            }
        //            else if (tip_action == 3)
        //            {
        //                //start_action = DateTime.Now;

        //                //action_2(reader.GetInt32(1));
        //                if (reader.GetDecimal(2) != 0)
        //                {
        //                    action_3(reader.GetInt32(1), reader.GetDecimal(2), reader.GetDecimal(5));//Дать скидку на все позиции из списка позицию                                                 
        //                }
        //                else
        //                {
        //                    action_3(reader.GetInt32(1), reader.GetString(3), reader.GetDecimal(5), reader.GetInt16(7)); //Сообщить о подарке                           
        //                }
        //                //write_time_execution(reader[1].ToString(), tip_action.ToString());

        //            }
        //            else if (tip_action == 4)
        //            {
        //                //start_action = DateTime.Now;

        //                if (reader.GetDecimal(2) != 0)
        //                {
        //                    action_4(reader.GetInt32(1), reader.GetDecimal(2), reader.GetDecimal(5));//Дать скидку на все позиции из списка позицию                                                 
        //                }
        //                else
        //                {
        //                    action_4(reader.GetInt32(1), reader.GetString(3), reader.GetDecimal(5), reader.GetInt64(4));
        //                }
        //                //write_time_execution(reader[1].ToString(), tip_action.ToString());
        //            }
        //            else if (tip_action == 6)
        //            {           //Номер документа  //Сообщение о подарке //Сумма в данном случае шаг акции
        //                //start_action = DateTime.Now;
        //                action_6(reader.GetInt32(1), reader.GetString(3), reader.GetDecimal(5), reader.GetInt16(7));
        //                //write_time_execution(reader[1].ToString(), tip_action.ToString());
        //            }
        //            else if (tip_action == 8)
        //            {
        //                //start_action = DateTime.Now;
        //                if (reader.GetDecimal(2) != 0)
        //                {
        //                    action_8(reader.GetInt32(1), reader.GetDecimal(2), reader.GetDecimal(5));//Дать скидку на все позиции из списка позицию                                                 
        //                }
        //                else
        //                {
        //                    action_8(reader.GetInt32(1), reader.GetString(3), reader.GetDecimal(5), reader.GetInt64(4), reader.GetInt16(7));
        //                }
        //                //write_time_execution(reader[1].ToString(), tip_action.ToString());
        //            }
        //            else if (tip_action == 9)//Акция работает в день рождения владельца дисконтной карты
        //            {
        //                //start_action = DateTime.Now;
        //                if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema==3))
        //                {
        //                    if (!actions_birthday())
        //                    {
        //                        //write_time_execution("проверка на день рождения", tip_action.ToString());
        //                        continue;
        //                    }
        //                }
        //                else if (MainStaticClass.GetWorkSchema == 2)//Для кулуба супермама будет дана скидка
        //                {
        //                    if (checkBox_club.CheckState != CheckState.Checked)
        //                    {
        //                        continue;
        //                    }
        //                }

        //                if (reader.GetDecimal(2) != 0)
        //                {
        //                    action_1(reader.GetInt32(1), reader.GetDecimal(2));//Дать скидку на эту позицию                                                 
        //                }
        //                else
        //                {
        //                    action_1(reader.GetInt32(1), reader.GetString(3), reader.GetInt16(7), reader.GetInt64(4)); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
        //                }
        //                //write_time_execution(reader[1].ToString(), tip_action.ToString());
        //            }
        //            //else if (tip_action == 10)
        //            //{

        //            //    //    //gaa
        //            //    //    //Если список заполнен то сумма должна накапливаться по вхождению позиции номенклатуры 

        //            //    //    //если список пустой то сумма насчитывается на весь документ
        //            //    //if (reader.GetDecimal(5) <= calculation_of_the_sum_of_the_document())
        //            //    if (reader.GetDecimal(5) <= action_10(reader.GetInt32(1)))
        //            //    {
        //            //        MessageBox.Show(reader[3].ToString());
        //            //        action_num_doc = Convert.ToInt32(reader[1].ToString());
        //            //    }
        //            //}
        //            else
        //            {
        //                MessageBox.Show("Неопознанный тип акции в документе  № " + reader[1].ToString(), " Обработка акций ");
        //            }
        //        }
        //        reader.Close();
        //        command.Dispose();

        //        //10 тип для товаров вынесен отдельно, а затем проверка на акцию без товаров

        //        query = "SELECT tip,num_doc,persent,comment,code_tovar,sum,barcode,marker,execution_order FROM action_header " +
        //          " WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end " +
        //          " AND " + count_minutes.ToString() + " between time_start AND time_end AND bonus_promotion=0 " +
        //          " AND barcode='' AND tip=10 AND num_doc in(" +//AND tip<>10 
        //          " SELECT DISTINCT action_table.num_doc FROM checks_table_temp " +
        //          " LEFT JOIN action_table ON checks_table_temp.tovar = action_table.code_tovar) order by execution_order asc, tip asc";//date_started asc,, tip desc

        //        command = new NpgsqlCommand(query, conn);
        //        reader = command.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            if (reader.GetDecimal(5) <= action_10(reader.GetInt32(1)))
        //            {
        //                int multiplicity = (int)(calculation_of_the_sum_of_the_document() / action_10(reader.GetInt32(1)));
        //                MessageBox.Show("Крастность " + multiplicity.ToString() + " " + reader[3].ToString());
        //                action_num_doc = Convert.ToInt32(reader[1].ToString());
        //            }
        //        }

        //        reader.Close();
        //        conn.Close();
        //        command.Dispose();

        //        checked_action_10();//Отдельная проверка поскольку может не быть товарной части, а все акции выше проверяются именно на вхождение товаров документа в таб части акционных документов
        //        //После сработки всех акций проверка и подсказка по сработке дальнейших акций
        //        load_list1_action2();

        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //    //MessageBox.Show(total_seconnds.ToString());
        //}


        /// <summary>
        /// Возвращает true если в табличной части условия акций есть строки и false
        /// если строк нет
        /// </summary>
        /// <param name="num_action_doc"></param>
        /// <returns></returns>
        private bool checked_action_table_rows(int num_action_doc)
        {
            bool result = false;

            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            Int64 count_minutes = Convert.ToInt64((DateTime.Now - DateTime.Now.Date).TotalMinutes);
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();

                string query = "SELECT COUNT(*) FROM action_table WHERE num_doc=" + num_action_doc.ToString();
                command = new NpgsqlCommand(query, conn);
                int result_query = Convert.ToInt32(command.ExecuteScalar());
                if (result_query > 0)
                {
                    result = true;
                }
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
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



        //private void checked_action_10()
        //{

        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    Int64 count_minutes = Convert.ToInt64((DateTime.Now - DateTime.Now.Date).TotalMinutes);
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();

        //        string query = "SELECT tip,num_doc,persent,comment,code_tovar,sum,barcode,marker FROM action_header " +
        //            " WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end " +
        //            " AND " + count_minutes.ToString() + " between time_start AND time_end and tip = 10";
        //        command = new NpgsqlCommand(query, conn);
        //        NpgsqlDataReader reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            if (checked_action_table_rows(Convert.ToInt32(reader[1].ToString())))
        //            {
        //                continue;
        //            }

        //            if (reader.GetDecimal(5) <= calculation_of_the_sum_of_the_document())// action_10(Convert.ToInt32(reader["num_doc"]))
        //            {
        //                int multiplicity = (int)(calculation_of_the_sum_of_the_document() / reader.GetDecimal(5));
        //                MessageBox.Show("Кратность " + multiplicity.ToString() + " " + reader[3].ToString());
        //                action_num_doc = Convert.ToInt32(reader[1].ToString());
        //            }
        //        }
        //        reader.Close();
        //        conn.Close();
        //        command.Dispose();

        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}


        /// <summary>
        /// Это 10 акция для тех акционных документов у которых есть строки и 
        /// здесь происходит проверка по сумме на вхождение строк документа чек
        /// в строки акционного документа
        /// 
        /// </summary>
        /// <param name="num_doc"></param>
        /// <returns></returns>
        //private Decimal action_10(int num_doc)
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    Decimal result = 0;
        //    //bool is_found = false;

        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        string query = "";
        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            if (Convert.ToInt16(command.ExecuteScalar()) == 1)//вхождение найдено 
        //            {
        //                result += Convert.ToDecimal(lvi.SubItems[7].Text);
        //                //is_found = true;
        //            }
        //        }
        //        conn.Close();
        //        command.Dispose();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }

        //    //if (is_found)
        //    //{
        //    //    result = calculation_of_the_sum_of_the_document(); 
        //    //}

        //    return result;
        //}



        /*
         * Обработать акцию по типу 1
         * первый тип это скидка на конкретный товар
         * если есть процент скидки то дается скидка 
         * иначе выдается сообщение о подарке
         * 
         * здесь сообщение о подарке
         */
        //private void action_1(int num_doc, string comment, int marker, long code_tovar)
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    Int16 result = 0;
        //    bool the_action_has_worked = false;//Признак того что акция сработала
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 
        //        string query = "";
        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                continue;
        //            }
        //            query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            result = Convert.ToInt16(command.ExecuteScalar());
        //            if (result > 0)
        //            {
        //                have_action = true;//Признак того что в документе есть сработка по акции

        //                the_action_has_worked = true;
        //                lvi.SubItems[9].Text = num_doc.ToString();//Тип акции 
        //                //lvi.SubItems[5].Text = "0";
        //                //lvi.SubItems[6].Text = "0";
        //                MessageBox.Show("Сработала акция, НЕОБХОДИМО выдать подарок " + comment);
        //                DialogResult dr = DialogResult.Cancel;
        //                if (marker == 1)
        //                {
        //                    dr = show_query_window_barcode(2, 1, num_doc);
        //                }
        //                //Добавить товар по акции если там есть код товара
        //                //query = "SELECT code_tovar FROM action_header WHERE num_doc=" + num_doc.ToString();
        //                //command = new NpgsqlCommand(query, conn);
        //                //object code_tovar = command.ExecuteScalar();
        //                //if (!System.DBNull.Value.Equals(code_tovar))
        //                //{

        //                //10.09.2020 Непонятно зачем здесь такая проверка  
        //                if (dr != DialogResult.Cancel)
        //                {
        //                    if (code_tovar != 0)
        //                    {
        //                        find_barcode_or_code_in_tovar(code_tovar.ToString());
        //                        //здесь необходимо изменить цену

        //                        listView1.Items[listView1.Items.Count - 1].SubItems[8].Text = num_doc.ToString(); //Номер акционного документа 
        //                        listView1.Items[listView1.Items.Count - 1].SubItems[9].Text = num_doc.ToString(); //Номер акционного документа 
        //                        listView1.Items[listView1.Items.Count - 1].SubItems[10].Text = num_doc.ToString(); //Номер акционного документа 
        //                    }
        //                }

        //                //break;
        //            }
        //        }

        //        /*акция сработала
        //         * надо отметить все товарные позиции 
        //         * чтобы они не участвовали в других акциях 
        //         */

        //        if (the_action_has_worked)
        //        {
        //            marked_action_tovar(num_doc);
        //        }
        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 1 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            //// conn.Dispose();
        //        }
        //    }
        //}


        ///*
        // * Обработать акцию по типу 1
        // * первый тип это скидка на конкретный товар
        // * если есть процент скидки то дается скидка 
        // * иначе выдается сообщение о подарке
        // * 
        // * Здесь скидка
        // */
        //private void action_1(int num_doc, decimal persent)
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 
        //        string query = "";
        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                continue;
        //            }

        //            query = "SELECT price FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            object result_query = command.ExecuteScalar();
        //            command.Dispose();
        //            if (result_query != null)
        //            {
        //                if (Convert.ToDecimal(result_query) == 0)
        //                {
        //                    have_action = true;//Признак того что в документе есть сработка по акции
        //                    lvi.SubItems[5].Text = (Math.Round(Convert.ToDecimal(lvi.SubItems[4].Text) - Convert.ToDecimal(lvi.SubItems[4].Text) * persent / 100, 2)).ToString();//Цена со скидкой            
        //                    lvi.SubItems[6].Text = ((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());
        //                    lvi.SubItems[7].Text = ((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString());
        //                    lvi.SubItems[8].Text = num_doc.ToString(); //Номер акционного документа
        //                    lvi.SubItems[10].Text = num_doc.ToString();//Тип акции                  
        //                }
        //                else
        //                {
        //                    have_action = true;
        //                    lvi.SubItems[5].Text = (result_query.ToString()).ToString();//Цена из табличной части документа акции
        //                    lvi.SubItems[6].Text = ((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());
        //                    lvi.SubItems[7].Text = ((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString());
        //                    lvi.SubItems[8].Text = num_doc.ToString(); //Номер акционного документа
        //                    lvi.SubItems[10].Text = num_doc.ToString();//Тип акции                        
        //                }
        //            }
        //        }
        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 1 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }


        //}

        /*
         * Обработать акцию по 2 типу
         * это значит в документе должен быть товар 
         * по вхождению в акционный список 
         * 
         * Здесь дается скидка на кратное количество позиций из 1-го списка
         */

        //private void action_2(int num_doc, decimal persent)
        //{
        //    /*В этой переменной запомнится позиция которая первой входит в первый список акции
        //    * на него будет дана скидка, необходимо скопировать эту позицию в конец списка 
        //    * и дать не на него скидку
        //    */
        //    int first_string_actions = 1000000;
        //    //int num_list=0;

        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    //object result = null;
        //    int quantity_of_pieces = 0;//Количество штук товара в строке
        //    int min_quantity = 1000000000;
        //    int num_list = 0;
        //    int num_pos = 0;
        //    ArrayList ar = new ArrayList();

        //    //Int16 result = 0;

        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();

        //        //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 

        //        string query = "SELECT COUNT(*) from(SELECT DISTINCT num_list FROM action_table where num_doc=" + num_doc + ") as foo";
        //        command = new NpgsqlCommand(query, conn);
        //        int[] action_list = new int[Convert.ToInt16(command.ExecuteScalar())];
        //        int x = 0;
        //        while (x < action_list.Length)
        //        {
        //            action_list[x] = 0;
        //            x++;
        //        }

        //        //ListViewItem lvi = null;
        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                continue;
        //            }
        //            quantity_of_pieces = Convert.ToInt16(lvi.SubItems[3].Text);
        //            while (quantity_of_pieces > 0)
        //            {
        //                query = "SELECT num_list FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //                command = new NpgsqlCommand(query, conn);
        //                NpgsqlDataReader reader = command.ExecuteReader();
        //                min_quantity = 1000000000;
        //                num_list = 0;
        //                int marker_min_amount = 1000000;
        //                while (reader.Read())
        //                {
        //                    min_quantity = Math.Min(min_quantity, action_list[reader.GetInt32(0) - 1]);
        //                    if (min_quantity < marker_min_amount)
        //                    {
        //                        num_list = reader.GetInt32(0);
        //                        marker_min_amount = min_quantity;
        //                    }
        //                    if ((first_string_actions == 1000000) && (reader.GetInt32(0) == 1))
        //                    {
        //                        first_string_actions = lvi.Index; //Запомним номер строки товара на который будем давать скидку
        //                    }
        //                }
        //                if (num_list != 0)
        //                {
        //                    action_list[num_list - 1] += 1;
        //                    num_pos = lvi.Index;
        //                }
        //                if (num_list == 1)
        //                {
        //                    ar.Add(lvi.Tag.ToString());
        //                }
        //                quantity_of_pieces--;
        //            }
        //        }
        //        //                conn.Close();
        //        bool the_action_has_worked = false;
        //        x = 0;
        //        min_quantity = 1000000000;

        //        while (x < action_list.Length)
        //        {
        //            if (action_list[x] == 0)
        //            {
        //                the_action_has_worked = false;
        //                break;
        //            }
        //            else
        //            {
        //                //Здесь надо получить кратность подарка
        //                min_quantity = Math.Min(min_quantity, action_list[x]);
        //                the_action_has_worked = true;
        //            }
        //            x++;
        //        }
        //        if (the_action_has_worked)
        //        {
        //            //if (MainStaticClass.GetWorkSchema == 2)
        //            //{
        //            //    load_list1_action2(num_doc.ToString());
        //            //    //if (listView2.Items.Count > 0)
        //            //    //{
        //            //    //    panel2.BringToFront();
        //            //    //    panel2.Visible = true;
        //            //    //}
        //            //    //MessageBox.Show("1");
        //            //    //Для Евы необходимо вытащить содержимое первого списка 
        //            //    //MessageBox.Show("Сработала акция, НЕОБХОДИМО выдать подарок количестве " + min_quantity.ToString() + " шт. " + comment);
        //            //}
        //            foreach (ListViewItem lvi in listView1.Items)
        //            {

        //                if (min_quantity > 0)
        //                {
        //                    have_action = true;//Признак того что в документе есть сработка по акции

        //                    //Сначала получим количество,если больше кратного количества наборов то копируем строку,
        //                    //а в исходной уменьшаем количество на количества наборов и пересчитываем суммы
        //                    //
        //                    if (ar.IndexOf(lvi.Tag.ToString(), 0) == -1)
        //                    {
        //                        continue;
        //                    }
        //                    //ListViewItem lvi = listView1.Items[first_string_actions];
        //                    quantity_of_pieces = Convert.ToInt16(lvi.SubItems[3].Text);
        //                    //int quantity_action_pieces = Math.Min(quantity_of_pieces, min_quantity);
        //                    if (quantity_of_pieces <= min_quantity)
        //                    {
        //                        lvi.SubItems[5].Text = (Math.Round(Convert.ToDecimal(lvi.SubItems[4].Text) - Convert.ToDecimal(lvi.SubItems[4].Text) * persent / 100, 2)).ToString();//Цена со скидкой            
        //                        lvi.SubItems[6].Text = ((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());
        //                        lvi.SubItems[7].Text = ((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString());
        //                        lvi.SubItems[8].Text = num_doc.ToString(); //Номер акционного документа 
        //                        min_quantity = min_quantity - quantity_of_pieces;
        //                    }
        //                    if ((quantity_of_pieces > min_quantity) && (min_quantity > 0))
        //                    {
        //                        lvi.SubItems[3].Text = (Convert.ToInt32(lvi.SubItems[3].Text) - min_quantity).ToString();
        //                        calculate_on_string(lvi);
        //                        //Добавляем строку с акционным товаром 
        //                        ListViewItem lvi_new = new ListViewItem(lvi.Tag.ToString());
        //                        lvi_new.Tag = lvi.Tag;
        //                        x = 0;
        //                        while (x < lvi.SubItems.Count - 1)
        //                        {
        //                            lvi_new.SubItems.Add(lvi.SubItems[x + 1].Text);
        //                            x++;
        //                        }

        //                        lvi_new.SubItems[2].Text = lvi.SubItems[2].Text;
        //                        lvi_new.SubItems[2].Tag = lvi.SubItems[2].Tag;
        //                        lvi_new.SubItems[3].Text = min_quantity.ToString();
        //                        lvi_new.SubItems[5].Text = (Math.Round(Convert.ToDecimal(lvi.SubItems[4].Text) - Convert.ToDecimal(lvi.SubItems[4].Text) * persent / 100, 2)).ToString();//Цена со скидкой            
        //                        lvi_new.SubItems[6].Text = ((Convert.ToDecimal(lvi_new.SubItems[3].Text) * Convert.ToDecimal(lvi_new.SubItems[4].Text)).ToString());
        //                        lvi_new.SubItems[7].Text = ((Convert.ToDecimal(lvi_new.SubItems[3].Text) * Convert.ToDecimal(lvi_new.SubItems[5].Text)).ToString());
        //                        lvi_new.SubItems[8].Text = num_doc.ToString(); //Номер акционного документа
        //                        lvi_new.SubItems[10].Text = num_doc.ToString(); //Номер акционного документа
        //                        //*****************************************************************************
        //                        lvi_new.SubItems[11].Text = "0";
        //                        lvi_new.SubItems[12].Text = "0";
        //                        lvi_new.SubItems[13].Text = "0";
        //                        lvi_new.SubItems[14].Text = "0";
        //                        //*****************************************************************************
        //                        listView1.Items.Add(lvi_new);
        //                        SendDataToCustomerScreen(1, 0,1);
        //                        min_quantity = 0;
        //                    }
        //                }
        //            }

        //            /*акция сработала
        //         * надо отметить все товарные позиции 
        //         * чтобы они не участвовали в других акциях 
        //         */
        //            if (the_action_has_worked)
        //            {
        //                marked_action_tovar(num_doc);
        //            }
        //        }
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 2 типа акций");
        //    }

        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}
            
        /// <summary>
        /// вывести сообщение по 1 списку в акции 2 типа 
        /// </summary>
        /// <param name="num_doc"></param>
        private void show_list1(string num_doc)
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT tovar.name FROM public.action_table LEFT JOIN tovar ON action_table.code_tovar = tovar.code where num_doc="+num_doc+" and num_list= 1 limit 10";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                string result = "";
                while (reader.Read())
                {
                    result += reader[0].ToString().Trim() + "\r\n"; 
                }
                if (result != "")
                {
                    //MessageBox.Show(result,"Для сработки акции необходимо добавить любую позицию ИЗ ... ");
                    MyMessageBox myMessageBox = new MyMessageBox();
                    myMessageBox.text_message.Text = result;
                    myMessageBox.Text = " Для сработки акции необходимо добавить любую позицию ИЗ ... ";
                    myMessageBox.text_message.TextAlign = HorizontalAlignment.Left;
                    myMessageBox.ShowDialog();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при подсказке по 2 акции" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подсказке по 2 акции" + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
               
        /// <summary>
        /// Для Евы напоминание о подарке
        /// После сработки всех акций 
        /// 1.Сначала в табличной части проверяем товары которые не участвовали в акциях
        /// 2.Формируем список акций 2 типа(скидочные) которые в периоде действия и в них максимальный номер списка=2
        /// 3.Последовательно проверяем свободные товары на вхождение во 2-й список этих акций подокументно и выводим первые 10 поизций 1 списка в сообщение 
        /// </summary>
        /// <param name="num_doc"></param>
        private void load_list1_action2()
        {
            string list_code_tovar = "";
            foreach (ListViewItem lvi in listView1.Items)
            {
                if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) == 0)//Этот товар не участовал в акции
                {
                    list_code_tovar += lvi.SubItems[0].Text + ",";
                }
            }
            if (list_code_tovar == "")
            {
                return;
            }
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT action_header.num_doc FROM action_header " +
                    " LEFT JOIN action_table ON action_header.num_doc = action_table.num_doc " +
                    " WHERE tip = 2 AND  '" + DateTime.Now.ToString("dd-MM-yyyy") + "' between date_started AND date_end " +
                    " AND action_table.code_tovar in (" + list_code_tovar.Substring(0, list_code_tovar.Length - 1) +
                    " ) AND action_table.num_list = 2 GROUP BY action_header.num_doc HAVING MAX(action_table.num_list)=2 ";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    show_list1(reader[0].ToString());
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при подсказке по 2 акции" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подсказке по 2 акции" + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        /*
       * Обработать акцию по 2 типу
       * это значит в документе должен быть товар 
       * по вхождению в акционный список 
         * 
       * Здесь выдается сообщение о подарке
         * 
       * Списки товаров могут быть абсолютно одинаковыми, а могут и отличатся
       * 
       */
        //private void action_2(int num_doc, string comment, int marker, long code_tovar)
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    //object result = null;
        //    int quantity_of_pieces = 0;//Количество штук товара в строке
        //    int min_quantity = 1000000000;
        //    int num_list = 0;
        //    int num_pos = 0;
        //    //Int16 result = 0;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 

        //        string query = "SELECT COUNT(*) from(SELECT DISTINCT num_list FROM action_table where num_doc=" + num_doc + ") as foo";
        //        command = new NpgsqlCommand(query, conn);
        //        int[] action_list = new int[Convert.ToInt16(command.ExecuteScalar())];
        //        int x = 0;
        //        while (x < action_list.Length)
        //        {
        //            action_list[x] = 0;
        //            x++;
        //        }

        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                continue;
        //            }
        //            quantity_of_pieces = Convert.ToInt16(lvi.SubItems[3].Text);
        //            while (quantity_of_pieces > 0)
        //            {
        //                query = "SELECT num_list FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //                command = new NpgsqlCommand(query, conn);
        //                NpgsqlDataReader reader = command.ExecuteReader();
        //                min_quantity = 1000000000;
        //                num_list = 0;
        //                int marker_min_amount = 1000000;
        //                while (reader.Read())
        //                {
        //                    min_quantity = Math.Min(min_quantity, action_list[reader.GetInt32(0) - 1]);
        //                    if (min_quantity < marker_min_amount)
        //                    {
        //                        num_list = reader.GetInt32(0);
        //                        marker_min_amount = min_quantity;
        //                    }
        //                }
        //                if (num_list != 0)
        //                {
        //                    action_list[num_list - 1] += 1;
        //                    num_pos = lvi.Index;
        //                }
        //                quantity_of_pieces--;
        //            }
        //        }
        //        //                conn.Close();
        //        bool the_action_has_worked = false;
        //        x = 0;
        //        min_quantity = 1000000000;

        //        while (x < action_list.Length)
        //        {
        //            if (action_list[x] == 0)
        //            {
        //                the_action_has_worked = false;
        //                break;
        //            }
        //            else
        //            {
        //                //Здесь надо получить кратность подарка
        //                min_quantity = Math.Min(min_quantity, action_list[x]);
        //                the_action_has_worked = true;
        //            }
        //            x++;
        //        }
        //        if (the_action_has_worked)
        //        {
        //            have_action = true;//Признак того что в документе есть сработка по акции

        //            listView1.Items[num_pos].SubItems[9].Text = num_doc.ToString();
        //            MessageBox.Show("Сработала акция, НЕОБХОДИМО выдать подарок количестве " + min_quantity.ToString() + " шт. " + comment);
        //            DialogResult dr = DialogResult.Cancel;
        //            if (marker == 1)
        //            {
        //                dr = show_query_window_barcode(2, min_quantity, num_doc);
        //            }

        //            if (dr != DialogResult.Cancel)
        //            {
        //                find_barcode_or_code_in_tovar(code_tovar.ToString());
        //            }

        //            /*акция сработала
        //            * надо отметить все товарные позиции 
        //            * чтобы они не участвовали в других акциях 
        //            */

        //            if (the_action_has_worked)
        //            {
        //                marked_action_tovar(num_doc);
        //            }

        //        }
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 2 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}

        /*Эта акция срабатывает когда сумма без скидки в документе >= сумме акции
        * тогда дается скидка на те позиции которые перечисляются в условии акции
        */
        //private void action_3(int num_doc, decimal persent, decimal sum)
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    decimal sum_on_doc = 0;//сумма документа без скидок 
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();

        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                continue;
        //            }
        //            string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            if (Convert.ToInt16(command.ExecuteScalar()) != 0)
        //            {
        //                sum_on_doc += Convert.ToDecimal(lvi.SubItems[6].Text);
        //            }
        //        }
        //        //Сумма документа без скидки больше или равна той что в условии ации
        //        //значит акция сработала
        //        if (sum_on_doc >= sum)
        //        {
        //            have_action = true;//Признак того что в документе есть сработка по акции

        //            foreach (ListViewItem lvi in listView1.Items)
        //            {
        //                string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //                command = new NpgsqlCommand(query, conn);
        //                if (Convert.ToInt16(command.ExecuteScalar()) != 0)
        //                {
        //                    lvi.SubItems[5].Text = (Math.Round(Convert.ToDecimal(lvi.SubItems[4].Text) - Convert.ToDecimal(lvi.SubItems[4].Text) * persent / 100, 2)).ToString();//Цена со скидкой            
        //                    lvi.SubItems[6].Text = ((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());
        //                    lvi.SubItems[7].Text = ((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString());
        //                    lvi.SubItems[8].Text = num_doc.ToString();
        //                    lvi.SubItems[10].Text = num_doc.ToString();
        //                }
        //            }

        //        }
        //        //                conn.Close();           
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 3 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }

        //}

        /*Эта акция срабатывает когда сумма без скидки в документе >= сумме акции
         * тогда выдается сообщение о подарке
         */
        //private void action_3(int num_doc, string comment, decimal sum, int marker)
        //{

        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    decimal sum_on_doc = 0;//сумма документа без скидок 
        //    int index = 0;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();

        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                continue;
        //            }
        //            string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            if (Convert.ToInt16(command.ExecuteScalar()) != 0)
        //            {
        //                sum_on_doc += Convert.ToDecimal(lvi.SubItems[6].Text);
        //                index = lvi.Index;
        //            }
        //        }
        //        //Сумма документа без скидки больше или равна той что в условии ации
        //        //значит акция сработала
        //        if (sum_on_doc >= sum)
        //        {
        //            have_action = true;//Признак того что в документе есть сработка по акции

        //            //action_num_doc = num_doc;
        //            listView1.Items[index].SubItems[9].Text = num_doc.ToString();//Тип акции                    
        //            MessageBox.Show(comment, " АКЦИЯ !!!");
        //            DialogResult dr = DialogResult.Cancel;
        //            if (marker == 1)
        //            {
        //                dr = show_query_window_barcode(2, 1, num_doc);
        //            }

        //            /*акция сработала
        //            * надо отметить все товарные позиции 
        //            * чтобы они не участвовали в других акциях 
        //            */
        //            marked_action_tovar(num_doc);


        //        }
        //        //                conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 3 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}

        private bool create_temp_tovar_table()
        {
            bool result = true;

            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();

                string query = "select COUNT(*) from information_schema.tables 		where table_schema='public' 	and table_name='tovar_action'	";
                command = new NpgsqlCommand(query, conn);

                if (Convert.ToInt16(command.ExecuteScalar()) == 0)
                {
                    query = "CREATE TABLE tovar_action(  code bigint NOT NULL,  retail_price numeric(10,2) NOT NULL,  quantity integer,characteristic_name character varying(100),characteristic_guid character varying(36))WITH (  OIDS=FALSE);ALTER TABLE tovar_action  OWNER TO postgres;";
                }
                else
                {
                    query = "DROP TABLE tovar_action;CREATE TABLE tovar_action(  code bigint NOT NULL,  retail_price numeric(10,2) NOT NULL,  quantity integer ,characteristic_name character varying(100),characteristic_guid character varying(36))WITH (  OIDS=FALSE);ALTER TABLE tovar_action  OWNER TO postgres;";
                }
                command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                //                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message, " Ошибка при создании временной таблицы ");
                result = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " Ошибка при создании временной таблицы ");
                result = false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    // conn.Dispose();
                }
            }

            return result;
        }

        //private bool create_temp_tovar_table_4()
        //{
        //    bool result = true;

        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;

        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();

        //        string query = "select COUNT(*) from information_schema.tables 		where table_schema='public' 	and table_name='tovar_action'	";
        //        command = new NpgsqlCommand(query, conn);

        //        if (Convert.ToInt16(command.ExecuteScalar()) == 0)
        //        {
        //            query = "CREATE TABLE tovar_action(  code bigint NOT NULL,  retail_price numeric(10,2) NOT NULL,  quantity integer, retail_price_discount numeric(10,2) ,characteristic_name character varying(100),characteristic_guid character varying(36) )WITH (  OIDS=FALSE);ALTER TABLE tovar_action  OWNER TO postgres;";
        //        }
        //        else
        //        {
        //            query = "DROP TABLE tovar_action;CREATE TABLE tovar_action(  code bigint NOT NULL,  retail_price numeric(10,2) NOT NULL,  quantity integer, retail_price_discount numeric(10,2) ,characteristic_name character varying(100),characteristic_guid character varying(36) )WITH (  OIDS=FALSE);ALTER TABLE tovar_action  OWNER TO postgres;";
        //        }
        //        command = new NpgsqlCommand(query, conn);
        //        command.ExecuteNonQuery();
        //        //                conn.Close();
        //        command.Dispose();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message, " Ошибка при создании временной таблицы ");
        //        result = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, " Ошибка при создании временной таблицы ");
        //        result = false;
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }

        //    return result;
        //}


        private void copy_list_view(ListView listview_source, ListView listview_destination)
        {
            for (int x = 0; x < listview_source.Items.Count; x++)
            {
                ListViewItem lvi = new ListViewItem(listview_original.Items[x].Text);
                lvi.Tag = listview_original.Items[x].Text;
                lvi.SubItems.Add(listview_original.Items[x].SubItems[1].Text);//Наименование
                lvi.SubItems.Add(listview_original.Items[x].SubItems[2].Text);//Характеристика
                lvi.SubItems[2].Tag = listview_original.Items[x].SubItems[2].Tag;//GUID характеристики
                lvi.SubItems.Add(listview_original.Items[x].SubItems[3].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[4].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[5].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[6].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[7].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[8].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[9].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[10].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[11].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[12].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[13].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[14].Text);
                listview_destination.Items.Add(lvi);
            }
        }

        /// <summary>
        /// Попытка обойти проблему (ListViewItem)lvi.clone()
        /// которая иногда возникает при добавленном в цикле элементе ListViewItem в listView1
        /// </summary>
        /// <param name="listViewItem_source"></param>
        /// <param name="listview_destination"></param>
        private void copy_list_view_item(ListViewItem listViewItem_source, ListView listview_destination)
        {
            ListViewItem lvi = new ListViewItem(listViewItem_source.SubItems[0].Text);
            lvi.Tag = listViewItem_source.SubItems[0].Text;
            lvi.SubItems.Add(listViewItem_source.SubItems[1].Text);//Наименование
            lvi.SubItems.Add(listViewItem_source.SubItems[2].Text);//Характеристика
            lvi.SubItems[2].Tag = listViewItem_source.SubItems[2].Tag;//GUID характеристики
            lvi.SubItems.Add(listViewItem_source.SubItems[3].Text);
            lvi.SubItems.Add(listViewItem_source.SubItems[4].Text);
            lvi.SubItems.Add(listViewItem_source.SubItems[5].Text);
            lvi.SubItems.Add(listViewItem_source.SubItems[6].Text);
            lvi.SubItems.Add(listViewItem_source.SubItems[7].Text);
            lvi.SubItems.Add(listViewItem_source.SubItems[8].Text);
            lvi.SubItems.Add(listViewItem_source.SubItems[9].Text);
            lvi.SubItems.Add(listViewItem_source.SubItems[10].Text);
            lvi.SubItems.Add(listViewItem_source.SubItems[11].Text);
            lvi.SubItems.Add(listViewItem_source.SubItems[12].Text);
            lvi.SubItems.Add(listViewItem_source.SubItems[13].Text);
            lvi.SubItems.Add(listViewItem_source.SubItems[14].Text);
            listview_destination.Items.Add(lvi);
        }


        /*Эта акция срабатывает когда количество товаров 
         * в документе >= сумме(количество) товаров в акции
         * тогда дается скидка на кратное количество товара
         * на самый дешевый товар из участвующих в акции 
         * 
         */
        //private void action_4(int num_doc, decimal persent, decimal sum)
        //{

        //    if (!create_temp_tovar_table_4())
        //    {
        //        return;
        //    }

        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    decimal quantity_on_doc = 0;//количество позиций в документе            
        //    ListView clon = new ListView();
        //    StringBuilder query = new StringBuilder();
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        string query_string = "";

        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                //clon.Items.Add((ListViewItem)lvi.Clone());
        //                copy_list_view_item(lvi, clon);
        //                continue;
        //            }
        //            query_string = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query_string, conn);

        //            if (Convert.ToInt16(command.ExecuteScalar()) != 0)
        //            {

        //                for (int i = 0; i < Convert.ToInt32(lvi.SubItems[3].Text); i++)
        //                {
        //                    query.Append("INSERT INTO tovar_action(code, retail_price, quantity,characteristic_name,characteristic_guid)VALUES(" +
        //                       lvi.SubItems[0].Text + "," +
        //                       lvi.SubItems[4].Text.Replace(",", ".") + "," +
        //                       "1,'" +
        //                       lvi.SubItems[2].Text + "','" +
        //                       (lvi.SubItems[2].Tag == null ? "" : lvi.SubItems[2].Tag.ToString()) + "');");
        //                }
        //                quantity_on_doc += Convert.ToDecimal(lvi.SubItems[3].Text);
        //                //listView1.Items.Remove(lvi);
        //            }
        //            else
        //            {
        //                //clon.Items.Add((ListViewItem)lvi.Clone());
        //                copy_list_view_item(lvi, clon);
        //            }
        //        }


        //        if (quantity_on_doc >= sum)//Есть вхождение в акцию
        //        {
        //            have_action = true;//Признак того что в документе есть сработка по акции

        //            listView1.Items.Clear();
        //            foreach (ListViewItem lvi in clon.Items)
        //            {
        //                listView1.Items.Add((ListViewItem)lvi.Clone());
        //            }

        //            command = new NpgsqlCommand(query.ToString(), conn);//устанавливаем акционные позиции во временную таблицу
        //            command.ExecuteNonQuery();
        //            query.Append("DELETE FROM tovar_action;");//Очищаем таблицу акционных товаров 
        //            //иначе результат задваивается ранее эта строка была закомментирована и при 2 товарах по 1 шт. учавствующих в акции
        //            //работала неверно

        //            int multiplication_factor = (int)(quantity_on_doc / sum);
        //            query_string = " SELECT code, retail_price, quantity,characteristic_name,characteristic_guid FROM tovar_action ORDER BY retail_price desc";//запросим товары отсортированные по цене, теперь еще и с характеристиками
        //            command = new NpgsqlCommand(query_string, conn);
        //            NpgsqlDataReader reader = command.ExecuteReader();

        //            Decimal sum_on_string = sum;

        //            int num_records = 1;

        //            while (reader.Read())
        //            {
        //                if (multiplication_factor > 0)
        //                {
        //                    if ((decimal)num_records / sum == Math.Round(num_records / sum, 0, MidpointRounding.AwayFromZero))//(sum_on_string == sum) && 
        //                    {
        //                        query.Append("INSERT INTO tovar_action(code, retail_price, quantity,characteristic_name,characteristic_guid,retail_price_discount)VALUES(" +
        //                          reader[0].ToString() + "," +
        //                          reader[1].ToString().Replace(",", ".") + "," + //Цена
        //                          "1,'" +
        //                          reader[3].ToString() + "','" +
        //                          reader[4].ToString() + "'," +
        //                          (Math.Round(Convert.ToDecimal(reader[1].ToString()) - Convert.ToDecimal(reader[1].ToString()) * persent / 100, 2)).ToString().Replace(",", ".") + //Цена со скидкой            
        //                          ");");
        //                        multiplication_factor--;
        //                    }
        //                    else
        //                    {
        //                        query.Append("INSERT INTO tovar_action(code, retail_price, quantity,characteristic_name,characteristic_guid,retail_price_discount)VALUES(" +
        //                                                 reader[0].ToString() + "," +
        //                                                 reader[1].ToString().Replace(",", ".") + "," +
        //                                                 "1,'" +
        //                                                reader[3].ToString() + "','" +
        //                                                reader[4].ToString() + "'," +
        //                                                reader[1].ToString().Replace(",", ".") + ");");
        //                    }

        //                }
        //                else
        //                {
        //                    query.Append("INSERT INTO tovar_action(code, retail_price, quantity,characteristic_name,characteristic_guid,retail_price_discount)VALUES(" +
        //                                                reader[0].ToString() + "," +
        //                                                reader[1].ToString().Replace(",", ".") + "," +
        //                                                "1,'" +
        //                                               reader[3].ToString() + "','" +
        //                                               reader[4].ToString() + "'," +
        //                                               reader[1].ToString().Replace(",", ".") + ");");
        //                }
        //                num_records++;
        //            }

        //            command = new NpgsqlCommand(query.ToString(), conn);
        //            command.ExecuteNonQuery();

        //            query_string = " SELECT tovar_action.code,tovar.name, tovar_action.retail_price,tovar_action.retail_price_discount ," +
        //            " SUM(quantity),characteristic_name,characteristic_guid FROM tovar_action " +
        //            " LEFT JOIN tovar ON tovar_action.code=tovar.code " +
        //            " GROUP BY tovar_action.code,tovar.name, tovar.retail_price,tovar_action.retail_price,characteristic_name,characteristic_guid," +
        //            " retail_price_discount";

        //            command = new NpgsqlCommand(query_string, conn);
        //            reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                ListViewItem lvi = new ListViewItem(reader[0].ToString());
        //                lvi.Tag = reader.GetInt64(0).ToString();          //Внутренний код товара
        //                lvi.SubItems.Add(reader[1].ToString().Trim());    //Наименование
        //                lvi.SubItems.Add(reader[5].ToString());           //Характеристика
        //                lvi.SubItems[2].Tag = reader[6].ToString();       //guid Характеристики
        //                lvi.SubItems.Add(reader[4].ToString().Trim());    //Количество
        //                lvi.SubItems.Add(reader.GetDecimal(2).ToString());//Цена
        //                lvi.SubItems.Add(reader.GetDecimal(3).ToString());//Цена со скидкой
        //                lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());//Сумма без скидки
        //                lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString());//Сумма со скидкой
        //                lvi.SubItems.Add("0"); //Номер акционного документа скидка
        //                lvi.SubItems.Add("0"); //Номер акционного документа подарок
        //                lvi.SubItems.Add("0"); //Номер акционного документа дополнительное поле пометка что участвовало в акции, но скидка может быть
        //                if (Convert.ToDecimal(lvi.SubItems[4].Text) != Convert.ToDecimal(lvi.SubItems[5].Text))
        //                {
        //                    lvi.SubItems[8].Text = num_doc.ToString();
        //                }
        //                lvi.SubItems[10].Text = num_doc.ToString();
        //                //*****************************************************************************
        //                lvi.SubItems.Add("0");// lvi.SubItems[11].Text = "0";
        //                lvi.SubItems.Add("0");// lvi.SubItems[12].Text = "0";
        //                lvi.SubItems.Add("0");// lvi.SubItems[13].Text = "0";
        //                lvi.SubItems.Add("0");//lvi.SubItems[14].Text = "0";
        //                //*****************************************************************************
        //                listView1.Items.Add(lvi);
        //                SendDataToCustomerScreen(1, 0,1);
        //            }

        //            /*акция сработала
        //     * надо отметить все товарные позиции 
        //     * чтобы они не участвовали в других акциях 
        //     */

        //            marked_action_tovar(num_doc);

        //        }

        //        //                conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, " Ошибка при обработке 4 типа акций ");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}


        /*Эта акция срабатывает когда количество товаров в документе >= сумме(количество) товаров в акции
        * тогда выдается сообщение о подарке
        * самый дешевый товар из документа дается в подарок кратное число единиц 
        * и еще добавляется некий товар из акционного документа 
        * 
        */
        //private void action_4(int num_doc, string comment, decimal sum, long code_tovar)
        //{
        //    if (!create_temp_tovar_table()) //Создать временную таблицу для акционного товара
        //    {
        //        return;
        //    }

        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    decimal quantity_on_doc = 0;//количество позиций в документе            
        //    ListView clon = new ListView();
        //    StringBuilder query = new StringBuilder();
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        string query_string = "";


        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                //clon.Items.Add((ListViewItem)lvi.Clone());
        //                copy_list_view_item(lvi, clon);
        //                continue;
        //            }
        //            query_string = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query_string, conn);

        //            if (Convert.ToInt16(command.ExecuteScalar()) != 0)//Это говорит о том что товар учавствует в этой акции
        //            {
        //                for (int i = 0; i < Convert.ToInt32(lvi.SubItems[3].Text); i++)
        //                {
        //                    query.Append("INSERT INTO tovar_action(code, retail_price, quantity,characteristic_name,characteristic_guid)VALUES(" +
        //                       lvi.SubItems[0].Text + "," +
        //                       lvi.SubItems[4].Text.Replace(",", ".") + "," +
        //                       "1,'" +
        //                       lvi.SubItems[2].Text + "','" +
        //                       (lvi.SubItems[2].Tag == null ? "" : lvi.SubItems[2].Tag.ToString()) + "');");
        //                }
        //                quantity_on_doc += Convert.ToDecimal(lvi.SubItems[3].Text);
        //            }
        //            else
        //            {
        //                //clon.Items.Add((ListViewItem)lvi.Clone());
        //                copy_list_view_item(lvi, clon);
        //            }
        //        }

        //        if (quantity_on_doc >= sum)//Есть вхождение в акцию
        //        {
        //            //MessageBox.Show(comment, " АКЦИЯ !!!");
        //            have_action = true;//Признак того что в документе есть сработка по акции

        //            listView1.Items.Clear();//Очищаем многострочную часть документа
        //            foreach (ListViewItem lvi in clon.Items)//Заполняем теми позициями которые точно не участвую в акции, либо ои участвовали в других акциях
        //            {
        //                listView1.Items.Add((ListViewItem)lvi.Clone());
        //            }

        //            command = new NpgsqlCommand(query.ToString(), conn);//устанавливаем акционные позиции во временную таблицу
        //            command.ExecuteNonQuery();

        //            int multiplication_factor = (int)(quantity_on_doc / sum);
        //            query_string = " SELECT tovar_action.code,tovar.name, tovar_action.retail_price,tovar_action.retail_price,tovar_action.quantity,tovar_action.characteristic_name,tovar_action.characteristic_guid " +
        //                " FROM tovar_action LEFT JOIN tovar ON tovar_action.code=tovar.code " +
        //                //" LEFT JOIN characteristic ON tovar_action.characteristic_guid = characteristic.guid " +
        //                " order by tovar_action.retail_price desc ";

        //            //query_string = " SELECT tovar_action.code,tovar.name, tovar_action.retail_price,tovar_action.retail_price,SUM(tovar_action.quantity) AS quantity,tovar_action.characteristic_name,tovar_action.characteristic_guid " +
        //            // " FROM tovar_action LEFT JOIN tovar ON tovar_action.code=tovar.code " +
        //            // //" LEFT JOIN characteristic ON tovar_action.characteristic_guid = characteristic.guid " +
        //            // " GROUP BY tovar_action.code,tovar.name, tovar_action.retail_price,tovar_action.retail_price,tovar_action.characteristic_name,tovar_action.characteristic_guid " +
        //            // "order by tovar_action.retail_price desc ";
        //            command = new NpgsqlCommand(query_string, conn);
        //            NpgsqlDataReader reader = command.ExecuteReader();

        //            //DateTime new_date = new DateTime(2019, 01, 01);//Дата с которой работа с подарками идет по новому 

        //            int num_records = 1;
        //            while (reader.Read())
        //            {
        //                if (multiplication_factor > 0)//Это количество позиций которые будут выданы в качестве подарка
        //                {
        //                    if ((decimal)num_records / sum == Math.Round(num_records / sum, 0, MidpointRounding.AwayFromZero))
        //                    {
        //                        ListViewItem lvi = new ListViewItem(reader[0].ToString());
        //                        lvi.Tag = reader.GetInt64(0).ToString();          //Внутренний код товара
        //                        lvi.SubItems.Add(reader[1].ToString().Trim());    //Наименование
        //                        lvi.SubItems.Add(reader[5].ToString());    //Характеристика
        //                        lvi.SubItems[2].Tag = reader[6].ToString();
        //                        lvi.SubItems.Add("1");
        //                        lvi.SubItems.Add(reader.GetDecimal(2).ToString());//Цена без скидки                                                                                  
        //                        string price = get_price_action(num_doc);
        //                        lvi.SubItems.Add(price.ToString());//Цена со скидкой должна быть получена из подарка
        //                        lvi.SubItems.Add(reader.GetDecimal(2).ToString());//Сумма без скидки
        //                        lvi.SubItems.Add(price.ToString());//Сумма со скидкой должна быть получена из подарка
        //                        lvi.SubItems.Add("0"); //Номер акционного документа скидка
        //                        lvi.SubItems.Add(num_doc.ToString()); //Номер акционного документа подарок
        //                        lvi.SubItems.Add("0"); //Номер акционного документа дополнительное поле пометка что участвовало в акции, но скидка может быть                                
        //                        lvi.SubItems[10].Text = num_doc.ToString();
        //                        //}
        //                        //*****************************************************************************
        //                        lvi.SubItems.Add("0");
        //                        lvi.SubItems.Add("0");
        //                        lvi.SubItems.Add("0");
        //                        lvi.SubItems.Add("0");
        //                        //*****************************************************************************
        //                        listView1.Items.Add(lvi);
        //                        SendDataToCustomerScreen(1, 0, 1);
        //                        multiplication_factor--;
        //                    }
        //                    else
        //                    {
        //                        ListViewItem lvi = new ListViewItem(reader[0].ToString());
        //                        lvi.Tag = reader.GetInt64(0).ToString();          //Внутренний код товара
        //                        lvi.SubItems.Add(reader[1].ToString().Trim());    //Наименование
        //                        lvi.SubItems.Add(reader[5].ToString());           //Характеристика
        //                        lvi.SubItems[2].Tag = reader[6].ToString();       //guid Характеристики
        //                        //lvi.SubItems.Add("1");    //Количество
        //                        lvi.SubItems.Add(reader[4].ToString());
        //                        lvi.SubItems.Add(reader.GetDecimal(2).ToString());//Цена
        //                        lvi.SubItems.Add(reader.GetDecimal(3).ToString());//Цена со скидкой
        //                        lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());//Сумма без скидки
        //                        lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString());//Сумма со скидкой
        //                        lvi.SubItems.Add("0"); //Номер акционного документа скидка
        //                        lvi.SubItems.Add("0"); //Номер акционного документа подарок
        //                        lvi.SubItems.Add("0"); //Номер акционного документа дополнительное поле пометка что участвовало в акции, но скидка может быть                                
        //                        lvi.SubItems[10].Text = num_doc.ToString();
        //                        //*****************************************************************************
        //                        lvi.SubItems.Add("0");
        //                        lvi.SubItems.Add("0");
        //                        lvi.SubItems.Add("0");
        //                        lvi.SubItems.Add("0");
        //                        //*****************************************************************************
        //                        listView1.Items.Add(lvi);
        //                        SendDataToCustomerScreen(1, 0, 1);
        //                    }
        //                }
        //                else
        //                {
        //                    ListViewItem lvi = new ListViewItem(reader[0].ToString());
        //                    lvi.Tag = reader.GetInt64(0).ToString();          //Внутренний код товара
        //                    lvi.SubItems.Add(reader[1].ToString().Trim());    //Наименование
        //                    lvi.SubItems.Add(reader[5].ToString());           //Характеристика
        //                    lvi.SubItems[2].Tag = reader[6].ToString();       //guid Характеристики                            
        //                    //lvi.SubItems.Add("1");    //Количество
        //                    lvi.SubItems.Add(reader[4].ToString());
        //                    lvi.SubItems.Add(reader.GetDecimal(2).ToString());//Цена
        //                    lvi.SubItems.Add(reader.GetDecimal(3).ToString());//Цена со скидкой
        //                    lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());//Сумма без скидки
        //                    lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString());//Сумма со скидкой
        //                    lvi.SubItems.Add("0"); //Номер акционного документа скидка
        //                    lvi.SubItems.Add("0"); //Номер акционного документа подарок
        //                    lvi.SubItems.Add("0"); //Номер акционного документа дополнительное поле пометка что участвовало в акции, но скидка может быть                            
        //                    lvi.SubItems[10].Text = num_doc.ToString();
        //                    //*****************************************************************************
        //                    lvi.SubItems.Add("0");
        //                    lvi.SubItems.Add("0");
        //                    lvi.SubItems.Add("0");
        //                    lvi.SubItems.Add("0");
        //                    //*****************************************************************************
        //                    listView1.Items.Add(lvi);
        //                    SendDataToCustomerScreen(1, 0, 1);
        //                }
        //                num_records++;
        //            }
        //            /*акция сработала
        //     * надо отметить все товарные позиции 
        //     * чтобы они не участвовали в других акциях 
        //     */
        //            marked_action_tovar(num_doc);
        //        }
        //        //                conn.Close();
        //        //Теперь надо свернуть табличную часть если много позиций, тогда чек не может распечататься 
        //        //ListView clon2 = new ListView();
        //        //foreach (ListViewItem lvi in listView1.Items)
        //        //{
        //        //    copy_list_view_item(lvi, clon2);
        //        //}
        //        //listView1.Items.Clear();
        //        //foreach (ListViewItem lvi2 in clon2.Items)
        //        //{
        //        //    ListViewItem lvi = new ListViewItem(lvi2.SubItems[0].Text);
        //        //    lvi.Tag = lvi2.SubItems[0].Text;          //Внутренний код товара
        //        //    lvi.SubItems.Add(lvi2.SubItems[1].Text);    //Наименование
        //        //    lvi.SubItems.Add(lvi2.SubItems[2].Text);    //Характеристика
        //        //    lvi.SubItems[2].Tag = lvi2.SubItems[2].Tag.ToString();
        //        //    lvi.SubItems.Add("1");
        //        //    lvi.SubItems.Add(lvi2.SubItems[4].Text);//Цена без скидки                                                                                  
        //        //    lvi.SubItems.Add(lvi2.SubItems[5].Text);//Цена со скидкой
        //        //    lvi.SubItems.Add(lvi2.SubItems[6].Text);//Сумма без скидки
        //        //    lvi.SubItems.Add(lvi2.SubItems[7].Text);//Сумма со скидкой
        //        //    lvi.SubItems.Add(lvi2.SubItems[8].Text);//Цена со скидкой
        //        //    lvi.SubItems.Add(lvi2.SubItems[9].Text);//Сумма без скидки
        //        //    lvi.SubItems.Add(lvi2.SubItems[10].Text);//Сумма со скидкой

        //        //    //string price = get_price_action(num_doc);
        //        //    //lvi.SubItems.Add(price.ToString());//Цена со скидкой должна быть получена из подарка
        //        //    lvi.SubItems.Add(reader.GetDecimal(2).ToString());//Сумма без скидки
        //        //    lvi.SubItems.Add(price.ToString());//Сумма со скидкой должна быть получена из подарка
        //        //    lvi.SubItems.Add("0"); //Номер акционного документа скидка
        //        //    lvi.SubItems.Add(num_doc.ToString()); //Номер акционного документа подарок
        //        //    lvi.SubItems.Add("0"); //Номер акционного документа дополнительное поле пометка что участвовало в акции, но скидка может быть                                
        //        //    lvi.SubItems[10].Text = num_doc.ToString();
        //        //    //}
        //        //    //*****************************************************************************
        //        //    lvi.SubItems.Add("0");
        //        //    lvi.SubItems.Add("0");
        //        //    lvi.SubItems.Add("0");
        //        //    lvi.SubItems.Add("0");
        //        //    //*****************************************************************************
        //        //    listView1.Items.Add(lvi);
        //        //    SendDataToCustomerScreen(1, 0, 1);
        //        //}                
        //        roll_up_listview();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 4 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}


        private void roll_up_listview()
        {
            //return;
            string query = "";
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            NpgsqlTransaction trans = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                trans = conn.BeginTransaction();
                query = "DELETE FROM roll_up_temp ";
                command = new NpgsqlCommand(query, conn);
                command.Transaction = trans;
                command.ExecuteNonQuery();

                foreach (ListViewItem lvi in listView1.Items)
                {
                    query = "INSERT INTO roll_up_temp(code_tovar," +
                                                            " name_tovar, " +
                                                            "characteristic_guid, " +
                                                            "characteristic_name, " +
                                                            "quantity, " +
                                                            "price, " +
                                                            "price_at_a_discount, " +
                                                            "sum, " +
                                                            "sum_at_a_discount, " +
                                                            "action_num_doc, " +
                                                            "action_num_doc1, " +
                                                            "action_num_doc2, " +
                                                            "item_marker)VALUES(" +
                                                            lvi.SubItems[0].Text + ",'" +
                                                            lvi.SubItems[1].Text + "','" +
                                                            lvi.SubItems[2].Tag.ToString() + "','" +
                                                            lvi.SubItems[2].Text + "'," +
                                                            lvi.SubItems[3].Text + "," +
                                                            lvi.SubItems[4].Text.Replace(",", ".") + "," +
                                                            lvi.SubItems[5].Text.Replace(",", ".") + "," +
                                                            lvi.SubItems[6].Text.Replace(",", ".") + "," +
                                                            lvi.SubItems[7].Text.Replace(",", ".") + "," +
                                                            lvi.SubItems[8].Text + "," +
                                                            lvi.SubItems[9].Text + "," +
                                                            lvi.SubItems[10].Text + ",'" +
                                                            lvi.SubItems[14].Text.Trim().Replace("'", "vasya2021")+ "')";
                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = trans;
                    command.ExecuteNonQuery();
                }
                query = "SELECT code_tovar, name_tovar, characteristic_guid, characteristic_name, SUM(quantity), price," +
                    " price_at_a_discount, SUM(sum), SUM(sum_at_a_discount), action_num_doc, action_num_doc1, action_num_doc2, item_marker" +
                    " FROM public.roll_up_temp    GROUP BY code_tovar, name_tovar, characteristic_guid, characteristic_name, price," +
                    " price_at_a_discount, action_num_doc, action_num_doc1, action_num_doc2, item_marker;";
                command = new NpgsqlCommand(query, conn);
                command.Transaction = trans;
                NpgsqlDataReader reader = command.ExecuteReader();
                listView1.Items.Clear();
                while (reader.Read())
                {
                    ListViewItem lvi = new ListViewItem(reader[0].ToString());
                    lvi.Tag = reader.GetInt64(0).ToString();          //Внутренний код товара
                    lvi.SubItems.Add(reader[1].ToString().Trim());    //Наименование
                    lvi.SubItems.Add(reader[2].ToString());    //Характеристика
                    lvi.SubItems[2].Tag = reader[3].ToString();
                    lvi.SubItems.Add(reader[4].ToString());
                    lvi.SubItems.Add(reader.GetDecimal(5).ToString());//Цена без скидки                                                                                                      
                    lvi.SubItems.Add(reader.GetDecimal(6).ToString());//Цена со скидкой
                    lvi.SubItems.Add(reader.GetDecimal(7).ToString());//Сумма без скидки                                                                                                      
                    lvi.SubItems.Add(reader.GetDecimal(8).ToString());//Сумма со скидкой                    
                    lvi.SubItems.Add(reader[9].ToString()); //Номер акционного документа скидка
                    lvi.SubItems.Add(reader[10].ToString()); //Номер акционного документа скидка
                    lvi.SubItems.Add(reader[11].ToString()); //Номер акционного документа скидка                    
                    //*****************************************************************************
                    lvi.SubItems.Add("0");
                    lvi.SubItems.Add("0");
                    lvi.SubItems.Add("0");
                    //*****************************************************************************
                    lvi.SubItems.Add(reader[12].ToString().Replace("vasya2021", "'"));//qr-code                                        
                    listView1.Items.Add(lvi);
                    SendDataToCustomerScreen(1, 0, 1);
                }
                reader.Close();
                conn.Close();
            }
            catch (NpgsqlException ex)
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

        private decimal get_reatil_price(string code_tovar)
        {
            decimal result = 0;
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT retail_price  FROM tovar where code=" + code_tovar;
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToDecimal(command.ExecuteScalar());
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибки при получении цены подарка " + ex.Message);
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

        ///*Эта акция срабатывает когда количество товаров в документе >= сумме(количество) товаров в акции
        // * тогда выдается сообщение о подарке
        // * самый дешевый товар из документа дается в подарок кратное число единиц 
        // * и еще добавляется некий товар из акционного документа 
        // * 
        // */
        //private void action_4_1(int num_doc, string comment, decimal sum, long code_tovar)
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    decimal quantity_on_doc = 0;//количество позиций в документе
        //    decimal min_cena = 0;
        //    //int x = 0;
        //    int index_position = 1000000;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();

        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[9].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                continue;
        //            }
        //            string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            if (Convert.ToInt16(command.ExecuteScalar()) != 0)
        //            {
        //                if (min_cena == 0)
        //                {
        //                    min_cena = Convert.ToDecimal(lvi.SubItems[3].Text);
        //                    index_position = lvi.Index;
        //                }
        //                else
        //                {
        //                    if (min_cena > Convert.ToDecimal(lvi.SubItems[3].Text))
        //                    {
        //                        index_position = lvi.Index;
        //                    }
        //                }
        //                quantity_on_doc += Convert.ToDecimal(lvi.SubItems[2].Text);
        //                //if (quantity_on_doc >= sum)
        //                //{
        //                //    if (min_cena == 0)
        //                //    {
        //                //        min_cena = Convert.ToDecimal(lvi.SubItems[3].Text);
        //                //        index_position = lvi.Index;
        //                //    } 
        //                //}                   
        //            }
        //        }
        //        //                if(index_position!=1000000)//Есть вхождение в акцию
        //        if (quantity_on_doc >= sum)//Есть вхождение в акцию
        //        {
        //            have_action = true;//Признак того что в документе есть сработка по акции

        //            ListViewItem lvi = listView1.Items[index_position];
        //            if (lvi.SubItems[2].Text == "1")
        //            {
        //                // lvi.SubItems[3].Text = "0";Цена должна остаться
        //                // lvi.SubItems[4].Text = "0";
        //                lvi.SubItems[5].Text = "0";
        //                lvi.SubItems[6].Text = "0";
        //                lvi.SubItems[7].Text = num_doc.ToString(); //Номер акционного документа 
        //                lvi.SubItems[9].Text = num_doc.ToString(); //Номер акционного документа
        //                //*****************************************************************************
        //                lvi.SubItems[11].Text = "0";
        //                lvi.SubItems[12].Text = "0";
        //                lvi.SubItems[13].Text = "0";
        //                lvi.SubItems[14].Text = "0";
        //                //*****************************************************************************
        //            }
        //            else
        //            {
        //                //уменьшаем количество на 1
        //                lvi.SubItems[2].Text = (Convert.ToInt32(lvi.SubItems[2].Text) - 1).ToString();
        //                calculate_on_string(lvi);//пересчитываем сумму по этой строке
        //                ListViewItem lvi_new = (ListViewItem)lvi.Clone();
        //                lvi_new.SubItems[2].Text = "1";
        //                //lvi_new.SubItems[3].Text = "0";Цена должна остаться
        //                //lvi_new.SubItems[4].Text = "0";
        //                lvi_new.SubItems[5].Text = "0";
        //                lvi_new.SubItems[6].Text = "0";
        //                lvi_new.SubItems[7].Text = num_doc.ToString(); //Номер акционного документа 
        //                lvi_new.SubItems[9].Text = num_doc.ToString(); //Номер акционного документа 
        //                //*****************************************************************************
        //                lvi.SubItems[11].Text = "0";
        //                lvi.SubItems[12].Text = "0";
        //                lvi.SubItems[13].Text = "0";
        //                lvi.SubItems[14].Text = "0";
        //                //*****************************************************************************
        //                listView1.Items.Add(lvi_new);
        //                SendDataToCustomerScreen(1, 0);
        //            }

        //            //Добавляем подарок

        //            find_barcode_or_code_in_tovar(code_tovar.ToString());
        //            listView1.Items[listView1.Items.Count - 1].SubItems[7].Text = num_doc.ToString(); //Номер акционного документа 
        //            listView1.Items[listView1.Items.Count - 1].SubItems[9].Text = num_doc.ToString(); //Номер акционного документа 
        //            /*акция сработала
        //     * надо отметить все товарные позиции 
        //     * чтобы они не участвовали в других акциях 
        //     */

        //            marked_action_tovar(num_doc);

        //        }
        //        //                conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 4 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}

        /*Эта акция работает только по предъявлению 
         * акционного купона то есть по штрихкоду
         * если в чеке есть товары из списка. в случае, если сумма этих товаров в чеке, меньше суммы, 
         * на которую даётся скидка, разница покупателю не возвращается, а цены товаров ставятся равными 0.01грн.; 
         * скидка выдаётся при предъявлении специального купона с ШК. в случае, если в чеке вообще нет товаров, 
         * на которые должна выдаваться скидка, то выдаётся предупреждение и купон не используется. 
         * в одном чеке может использоваться только один акционный купон!!!
         * имеется ввиду что сколько раз не предъявляй купон скидка не накапливается
         */
        //private void action_5(int num_doc, decimal sum)
        //{
        //    // 1 сНачала надо проверить сработку акции
        //    //bool the_action_has_worked = false;
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    Int16 result = 0;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 
        //        string query = "";
        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                continue;
        //            }
        //            query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            result = Convert.ToInt16(command.ExecuteScalar());

        //            if (result == 1)//Сработала акция
        //            {
        //                have_action = true;//Признак того что в документе есть сработка по акции

        //                decimal the_sum_without_a_discount = Convert.ToDecimal(lvi.SubItems[6].Text);
        //                if (sum >= the_sum_without_a_discount)
        //                {
        //                    sum = sum - the_sum_without_a_discount;
        //                    lvi.SubItems[5].Text = (Convert.ToDecimal(1 / 100)).ToString();
        //                    lvi.SubItems[6].Text = ((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString());
        //                    lvi.SubItems[7].Text = lvi.SubItems[6].Text;// Convert.ToDecimal(the_sum_without_a_discount - sum).ToString();//Сумма со скидкой
        //                    lvi.SubItems[8].Text = num_doc.ToString(); //Номер акционного документа
        //                    lvi.SubItems[10].Text = num_doc.ToString(); //Номер акционного документа
        //                }
        //                else
        //                {
        //                    //Здесь сначала получаем сумму путем вычитания оставшейся суммы скидки 
        //                    //от суммы без скидки, а только затем получаем цену со скидкой
        //                    lvi.SubItems[7].Text = Convert.ToDecimal(the_sum_without_a_discount - sum).ToString();//Сумма со скидкой
        //                    lvi.SubItems[5].Text = Math.Round(Convert.ToDecimal(lvi.SubItems[7].Text) / Convert.ToDecimal(lvi.SubItems[3].Text), 2).ToString();
        //                    lvi.SubItems[8].Text = num_doc.ToString(); //Номер акционного документа
        //                    lvi.SubItems[10].Text = num_doc.ToString(); //Номер акционного документа
        //                    sum = 0; break; //Поскольку сумма скидки закончилась прерываем цикл
        //                }
        //            }
        //        }
        //        //                conn.Close();
        //        /*Помечаем позиции которые 
        //         * остались не помеченными 
        //         * при сработке акции
        //         * 
        //         */
        //        marked_action_tovar(num_doc);

        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 5 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}

        /*новый тип акции (6): за каждые ***грн. внутри чека (товаров из определённого списка) выдаётся сообщение типа 
         * "акция такая-то сработала *** раз, выдайте *** подарков" 
         * (в реальной акции, которая должна быть, будут выдаваться стикера, которые наклеиваются на купон, когда человек 
         * собирает 10 стикеров на этом купоне, то может обменять этот купон с наклеенными стикерами на подарочный комплект,
         * состав может быть разный, контролировать будет кассир)
         * 
         */
        //private void action_6(int num_doc, string comment, decimal sum, Int32 marker)
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    Int16 result = 0;
        //    decimal action_sum_of_this_document = 0;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 
        //        string query = "";
        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                continue;
        //            }
        //            query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            result = Convert.ToInt16(command.ExecuteScalar());
        //            if (result == 1)//Акция сработала
        //            {
        //                have_action = true;//Признак того что в документе есть сработка по акции

        //                action_sum_of_this_document += Convert.ToDecimal(lvi.SubItems[6].Text);
        //            }
        //        }
        //        //                conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message, "Ошибка при работе с базой данных");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 6 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }

        //    int quantity_of_gifts = (int)(action_sum_of_this_document / sum);
        //    //decimal quantity_of_gifts = Math.Round(action_sum_of_this_document / sum,0);

        //    if (quantity_of_gifts > 0)//значит акция сработала
        //    {
        //        MessageBox.Show(comment + " количество подарков = " + quantity_of_gifts.ToString() + " шт. ", " АКЦИЯ !!!");
        //        /*акция сработала
        //     * надо отметить все товарные позиции 
        //     * чтобы они не участвовали в других акциях 
        //     */
        //        if (marker == 1)
        //        {
        //            for (int i = 0; i < quantity_of_gifts; i++)
        //            {
        //                show_query_window_barcode(2, 1, num_doc);
        //            }
        //        }
        //        marked_action_tovar(num_doc);
        //    }
        //}

        /*новый тип акции (7). для фиксирования выданных подарков по акции (6)
         * человек приносит купон и выбирает определённые товары с полки (согласно условиям акции),
         * кассир пробивает эти товары (подарки) в отдельный чек и проводит ШК купона по сканеру.
         * в чеке обнуляются цены на эти товары и добавляется строка типа "ПОДАРОК 0.01грн" - 4шт. 
         * (количество равно количеству товаров-подарков). кассир сам будет контролировать товарный состав чека, точнее, чтобы количество 
         * подарков было такое, которое позволяет купон со стикерами, если вдруг в чеке будут обычные товары, то их цена не должна меняться 
         * или можно выдать предупреждение, что для этого типа акции наличие в чеке не акционных товаров недопустимо.
         */
        //private void action_7(int num_doc, long code_tovar)
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    int gift = 0;
        //    Int16 result = 0;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 
        //        string query = "";
        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            //if (lvi.SubItems[9].Text.Trim().Length > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            //{
        //            //    continue;
        //            //}
        //            query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            result = Convert.ToInt16(command.ExecuteScalar());
        //            if (result > 0)
        //            {
        //                have_action = true;//Признак того что в документе есть сработка по акции

        //                //lvi.SubItems[4].Text = ((decimal)1 / 100).ToString();
        //                lvi.SubItems[6].Text = "0";
        //                lvi.SubItems[7].Text = "0"; //((Convert.ToDecimal(lvi.SubItems[2].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());
        //                lvi.SubItems[8].Text = num_doc.ToString(); //Номер акционного документа
        //                lvi.SubItems[10].Text = num_doc.ToString(); //Номер акционного документа
        //                gift += Convert.ToInt32(lvi.SubItems[3].Text);
        //            }
        //            else
        //            {
        //                MessageBox.Show("Обнаружен неакционный товар " + lvi.SubItems[0].Text);
        //            }
        //        }
        //        find_barcode_or_code_in_tovar_action(code_tovar.ToString(), gift, false, num_doc);
        //        //                conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message, "Ошибка при работе с базой данных");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 7 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}


        /// <summary>
        /// При покупке указанного количества 
        /// товаров покупатель может получить 
        /// указанную скидку на эти товары 
        /// либо указанный подарок,
        /// здесь дается скидка на все позиции 
        /// из списка
        ///  
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="persent"></param>
        /// <param name="sum"></param>
        //private void action_8(int num_doc, decimal persent, decimal sum)
        //{


        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //    NpgsqlCommand command = null;
        //    string query_string = "";
        //    StringBuilder query = new StringBuilder();
        //    decimal quantity_on_doc = 0;

        //    if (!create_temp_tovar_table())
        //    {
        //        return;
        //    }



        //    try
        //    {

        //        conn.Open();
        //        ListView clon = new ListView();
        //        //int total_quantity = 0;
        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                //clon.Items.Add((ListViewItem)lvi.Clone());
        //                copy_list_view_item(lvi, clon);
        //                continue;
        //            }
        //            query_string = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query_string, conn);

        //            if (Convert.ToInt16(command.ExecuteScalar()) != 0)
        //            {

        //                for (int i = 0; i < Convert.ToInt32(lvi.SubItems[3].Text); i++)
        //                {
        //                    query.Append("INSERT INTO tovar_action(code, retail_price, quantity)VALUES(" +
        //                       lvi.SubItems[0].Text + "," +
        //                       lvi.SubItems[4].Text.Replace(",", ".") + "," +
        //                       "1);");
        //                }
        //                quantity_on_doc += Convert.ToDecimal(lvi.SubItems[3].Text);
        //            }
        //            else
        //            {
        //                //clon.Items.Add((ListViewItem)lvi.Clone());
        //                copy_list_view_item(lvi, clon);
        //            }
        //        }


        //        if (quantity_on_doc >= sum)//Есть вхождение в акцию
        //        {

        //            have_action = true;//Признак того что в документе есть сработка по акции

        //            listView1.Items.Clear();
        //            foreach (ListViewItem lvi in clon.Items)
        //            {
        //                listView1.Items.Add((ListViewItem)lvi.Clone());
        //            }
        //            command = new NpgsqlCommand(query.ToString(), conn);//устанавливаем акционные позиции во временную таблицу
        //            command.ExecuteNonQuery();
        //            //query = new StringBuilder();
        //            query.Append("DELETE FROM tovar_action;");//Очищаем таблицу акционных товаров 
        //            //иначе результат задваивается ранее эта строка была закомментирована и при 2 товарах по 1 шт. учавстсующих в акции
        //            //работала неверно

        //            int multiplication_factor = (int)(quantity_on_doc / sum);
        //            //query_string = " SELECT code, retail_price, quantity FROM tovar_action ORDER BY retail_price ";//запросим товары отсортированные по цене
        //            query_string = " SELECT tovar_action.code,tovar.name, tovar.retail_price,tovar_action.retail_price, quantity FROM tovar_action LEFT JOIN tovar ON tovar_action.code=tovar.code ";//запросим товары отсортированные по цене
        //            command = new NpgsqlCommand(query_string, conn);
        //            NpgsqlDataReader reader = command.ExecuteReader();
        //            decimal _sum_ = sum;
        //            while (reader.Read())
        //            {
        //                if ((multiplication_factor > 0) || (_sum_ > 0))
        //                {
        //                    if ((_sum_ == 0) && (multiplication_factor > 0))
        //                    {
        //                        _sum_ = sum;
        //                        multiplication_factor--;

        //                    }
        //                    _sum_ -= 1;

        //                    ListViewItem lvi = new ListViewItem(reader[0].ToString());
        //                    lvi.Tag = reader.GetInt64(0).ToString();          //Внутренний код товара
        //                    lvi.SubItems.Add(reader[1].ToString().Trim());    //Наименование
        //                    lvi.SubItems.Add("");                             //Характеристика
        //                    lvi.SubItems[2].Tag = "";
        //                    lvi.SubItems.Add(reader[4].ToString().Trim());    //Количество
        //                    lvi.SubItems.Add(reader.GetDecimal(2).ToString());//Цена
        //                    lvi.SubItems.Add(Math.Round(reader.GetDecimal(2) - reader.GetDecimal(2) * persent / 100, 2).ToString());//Цена со скидкой            
        //                    //lvi.SubItems.Add(reader.GetDecimal(3).ToString());//Цена со скидкой
        //                    lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());//Сумма без скидки
        //                    lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString());//Сумма со скидкой
        //                    lvi.SubItems.Add(num_doc.ToString()); //Номер акционного документа скидка
        //                    lvi.SubItems.Add("0"); //Номер акционного документа подарок
        //                    lvi.SubItems.Add("0"); //Номер акционного документа дополнительное поле пометка что участвовало в акции, но скидка может быть

        //                    lvi.SubItems[10].Text = num_doc.ToString();
        //                    //*****************************************************************************
        //                    //lvi.SubItems[11].Text = "0";
        //                    //lvi.SubItems[12].Text = "0";
        //                    //lvi.SubItems[13].Text = "0";
        //                    //lvi.SubItems[14].Text = "0";
        //                    lvi.SubItems.Add("0");
        //                    lvi.SubItems.Add("0");
        //                    lvi.SubItems.Add("0");
        //                    lvi.SubItems.Add("0");
        //                    //*****************************************************************************
        //                    listView1.Items.Add(lvi);
        //                    SendDataToCustomerScreen(1, 0,1);

        //                }
        //                else
        //                {
        //                    ListViewItem lvi = new ListViewItem(reader[0].ToString());
        //                    lvi.Tag = reader.GetInt64(0).ToString();          //Внутренний код товара
        //                    lvi.SubItems.Add(reader[1].ToString().Trim());    //Наименование
        //                    lvi.SubItems.Add("");    //Характеристика
        //                    lvi.SubItems[2].Tag = "";
        //                    lvi.SubItems.Add(reader[4].ToString().Trim());    //Количество
        //                    lvi.SubItems.Add(reader.GetDecimal(2).ToString());//Цена                            
        //                    lvi.SubItems.Add(reader.GetDecimal(3).ToString());//Цена со скидкой
        //                    lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());//Сумма без скидки
        //                    lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString());//Сумма со скидкой
        //                    lvi.SubItems.Add("0"); //Номер акционного документа скидка
        //                    lvi.SubItems.Add("0"); //Номер акционного документа подарок
        //                    lvi.SubItems.Add("0"); //Номер акционного документа дополнительное поле пометка что участвовало в акции, но скидка может быть

        //                    lvi.SubItems[10].Text = num_doc.ToString();
        //                    //*****************************************************************************
        //                    //lvi.SubItems[11].Text = "0";
        //                    //lvi.SubItems[12].Text = "0";
        //                    //lvi.SubItems[13].Text = "0";
        //                    //lvi.SubItems[14].Text = "0";
        //                    lvi.SubItems.Add("0");
        //                    lvi.SubItems.Add("0");
        //                    lvi.SubItems.Add("0");
        //                    lvi.SubItems.Add("0");
        //                    //*****************************************************************************
        //                    listView1.Items.Add(lvi);
        //                    SendDataToCustomerScreen(1, 0,1);
        //                }

        //            }

        //            /*акция сработала
        //     * надо отметить все товарные позиции 
        //     * чтобы они не участвовали в других акциях 
        //     */

        //            marked_action_tovar(num_doc);
        //        }

        //        roll_up_listview();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message, " 8 акция ");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, " 8 акция ");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}


        /// <summary>
        /// При покупке указанного количества 
        /// товаров покупатель может получить 
        /// указанную скидку на эти товары 
        /// либо указанный подарок,
        /// здесь выдается подарок
        /// из списка
        /// если маркер =1 и заполнен код товара то будет выдан запрос
        /// на ввод кода или штрихкода и ему будет проставлена цена 
        /// кода товара
        /// Проверено 23.05.2019
        ///  
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="persent"></param>
        /// <param name="sum"></param>
        //private void action_8(int num_doc, string comment, decimal sum, long code_tovar, Int32 marker)
        //{


        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //    NpgsqlCommand command = null;
        //    string query_string = "";
        //    //StringBuilder query = new StringBuilder();
        //    decimal quantity_on_doc = 0;

        //    if (!create_temp_tovar_table())
        //    {
        //        return;
        //    }

        //    try
        //    {

        //        conn.Open();
        //        //ListView clon = new ListView();
        //        //int total_quantity = 0;
        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                continue;
        //            }
        //            query_string = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query_string, conn);

        //            if (Convert.ToInt16(command.ExecuteScalar()) != 0)
        //            {
        //                quantity_on_doc += Convert.ToDecimal(lvi.SubItems[3].Text);
        //            }
        //        }


        //        if (quantity_on_doc >= sum)//Есть вхождение в акцию
        //        {

        //            have_action = true;//Признак того что в документе есть сработка по акции

        //            int multiplication_factor = (int)(quantity_on_doc / sum);


        //            MessageBox.Show(comment.Trim() + " количество подарков = " + multiplication_factor.ToString() + " шт. ", " АКЦИЯ !!!");

        //            if (marker == 1)
        //            {
        //                for (int i = 0; i < multiplication_factor; i++)
        //                {
        //                    show_query_window_barcode(2, 1, num_doc);
        //                }
        //            }

        //            marked_action_tovar(num_doc);

        //        }
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message, " 8 акция ");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, " 8 акция ");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}


        /*Пометка товарных позиций которые участвовали в акции
         * для того чтобы они не участвоствовали в следующих акциях
         */
        //private void marked_action_tovar(int num_doc)
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                continue;
        //            }
        //            string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            Int16 result = Convert.ToInt16(command.ExecuteScalar());
        //            if (result == 1)
        //            {
        //                lvi.SubItems[10].Text = num_doc.ToString();
        //            }
        //        }
        //        //                conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "пометка товарных позиций участвующих в акции");
        //    }

        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}


        /*Пометка товарных позиций которые участвовали в акции
         * для того чтобы они не участвоствовали в следующих акциях
         */
        private void marked_bonus_action_tovar(int num_doc)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                foreach (ListViewItem lvi in listView1.Items)
                {
                    if (Convert.ToInt32(lvi.SubItems[13].Text.Trim()) > 0)//Этот товар уже участвовал в бонусной акции значит его пропускаем
                    {
                        continue;
                    }
                    string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query, conn);
                    Int16 result = Convert.ToInt16(command.ExecuteScalar());
                    if (result == 1)
                    {
                        lvi.SubItems[13].Text = num_doc.ToString();
                    }
                }
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "пометка товарных позиций участвующих в акции");
            }

            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    // conn.Dispose();
                }
            }
        }


        #endregion


        /*Оплата отменена
         *загружаем старое состояние табличной части которое было до расчета акций
         *
         */
        public void cancel_action()
        {

            if (check_type.SelectedIndex != 0)
            {
                return;
            }

            selection_goods = true;
            listView1.Items.Clear();


            //for (int x = 0; x < listview_original.Items.Count; x++)
            //{
            //    ListViewItem lvi = (ListViewItem)listview_original.Items[x].Clone();

            //    listView1.Items.Add(lvi);

            //}


            for (int x = 0; x < listview_original.Items.Count; x++)
            {

                ListViewItem lvi = new ListViewItem(listview_original.Items[x].Text);
                lvi.Tag = listview_original.Items[x].Text;
                lvi.SubItems.Add(listview_original.Items[x].SubItems[1].Text);//Наименование
                lvi.SubItems.Add(listview_original.Items[x].SubItems[2].Text);//Характеристика
                if (listview_original.Items[x].SubItems[2].Tag == null)
                {
                    lvi.SubItems[2].Tag = "";// listview_original.Items[x].SubItems[2].Tag;//GUID характеристики
                }
                else
                {
                    lvi.SubItems[2].Tag = listview_original.Items[x].SubItems[2].Tag;//GUID характеристики
                }
                lvi.SubItems.Add(listview_original.Items[x].SubItems[3].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[4].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[5].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[6].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[7].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[8].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[9].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[10].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[11].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[12].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[13].Text);
                lvi.SubItems.Add(listview_original.Items[x].SubItems[14].Text);

                listView1.Items.Add(lvi);
            }
            write_new_document("0", "0", "0", "0", false, "0", "0", "0", "0");
            SendDataToCustomerScreen(1, 0,1);
            inputbarcode.Focus();
        }

        /*
         * Когда по акции должен быть выдан подарок
         * иногда вводится штрихкод, то есть подарок
         * зараннее может быть не определен
         * 1.Вызов для ввода акционного штрихкода который определяет собой акцию
         * 2.Вызов для ввода штрихкода по которму будет найден подарок по акции с нулевой ценой
         * 
         */
        private DialogResult show_query_window_barcode(int call_type, int count, int num_doc)
        {
            Input_action_barcode ib = new Input_action_barcode();
            ib.count = count;
            ib.caller = this;
            ib.call_type = call_type;
            ib.num_doc = num_doc;
            DialogResult dr = ib.ShowDialog();
            ib.Dispose();
            return dr;
        }


        //private int get_percent_bonus(string code)
        //{
        //    int result = 0;

        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

        //    try
        //    {
        //        conn.Open();
        //        string query = "SELECT percent_bonus FROM tovar where code="+code;
        //        NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //        object result_query = command.ExecuteScalar();
        //        result = Convert.ToInt16(result_query);
        //        conn.Close();
        //        command.Dispose();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show("Ошибка при получении процента бонуса "+ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Ошибка при получении процента бонуса "+ex.Message);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return result;
        //}


        //#region buyerInfoRequest
        //public class BuyerInfoRequest
        //{
        //    public string cardTrack2 { get; set; }
        //    public string phone { get; set; }

        //    public string cardNum { get; set; }
        //}



        //#endregion
              

        private void show_pay_form()
        {

            //string machine_name = Environment.MachineName;//имя компьютера
            //int path_execute_actions = 0;

            //if (machine_name == "GAA-IT")
            //{
            //    path_execute_actions = 1;
            //}

            MainStaticClass.write_event_in_log("Попытка перейти в окно оплаты", "Документ чек", numdoc.ToString());

            //if (listView_sertificates.Items.Count > 0)
            //{
            //    pay_form.listView_sertificates.Items.Clear();
            //    foreach (ListViewItem lvi in listView_sertificates.Items)
            //    {
            //        pay_form.listView_sertificates.Items.Add((ListViewItem)lvi.Clone());
            //    }
            //}
            //Pay pay_form = new Pay();
            //pay_form = new Pay();
            pay_form.pay_bonus.Text = "0";
            pay_form.pay_bonus.Visible = false;
            pay_form.pay_bonus_many.Text = "0";
            pay_form.pay_bonus.Enabled = false;

            listView_sertificates.Items.Clear();
            pay_form.listView_sertificates.Items.Clear();
            pay_form.cc = this;
            DialogResult dr ;

            if (this.check_type.SelectedIndex == 0)
            {

                /*Копируем табличную часть один ListView в другой
                 *  чтобы если оплата отменится и с чеком будут дальше работать
                 *  отменить все расчитанные акции одним махом
                 */
                MainStaticClass.write_event_in_log(" Копируем табличную часть один ListView в другой ", "Документ чек", numdoc.ToString());
                listview_original.Items.Clear();
                for (int x = 0; x < listView1.Items.Count; x++)
                {
                    ListViewItem lvi = (ListViewItem)listView1.Items[x].Clone();
                    lvi.SubItems[2].Tag = listView1.Items[x].SubItems[2].Tag;
                    listview_original.Items.Add(lvi);

                }

                MainStaticClass.write_event_in_log(" Попытка обработать акции по штрихкодам ", "Документ чек", numdoc.ToString());
                //if (MainStaticClass.EnableStockProcessingInMemory <= 0)
                //{
                //    //Акции по штрихкодам
                //    foreach (string barcode in action_barcode_list)
                //    {
                //        to_define_the_action(barcode);
                //    }
                //    //Теперь все остальные акции
                //    MainStaticClass.write_event_in_log(" Попытка обработать все остальные акции ", "Документ чек", numdoc.ToString());                    
                //    to_define_the_action();//Обработка на дисконтные акции 
                //}
                //else
                //{                    
                    //Акции по штрихкодам                    
                    DataTable dataTable = to_define_the_action_dt(true);//Обработка на дисконтные акции с использованием datatable 
                    
                    //рассчитанные данные в памяти по акциям теперь помещаем в листвью 
                    listView1.Items.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        ListViewItem lvi = new ListViewItem(row["tovar_code"].ToString());
                        lvi.Tag = row["tovar_code"].ToString();
                        lvi.SubItems.Add(row["tovar_name"].ToString());//Наименование                        
                        lvi.SubItems.Add(row["characteristic_name"].ToString());//Характеристика
                        lvi.SubItems[2].Tag = row["characteristic_code"].ToString();
                        lvi.SubItems.Add(row["quantity"].ToString());//Количество
                        lvi.SubItems.Add(row["price"].ToString());//Цена без скидки
                        lvi.SubItems.Add(row["price_at_discount"].ToString());//Цена Со скидкой
                        lvi.SubItems.Add(row["sum_full"].ToString());//Сумма без скидки
                        lvi.SubItems.Add(row["sum_at_discount"].ToString());//Сумма со скидкой
                        lvi.SubItems.Add(row["action"].ToString());//Акционный документ
                        lvi.SubItems.Add(row["gift"].ToString());//Акционный документ
                        lvi.SubItems.Add(row["action2"].ToString());//Акционный документ
                        lvi.SubItems.Add(row["bonus_reg"].ToString());//Бонус
                        lvi.SubItems.Add(row["bonus_action"].ToString());//Бонус
                        lvi.SubItems.Add(row["bonus_action_b"].ToString());//Бонус
                        lvi.SubItems.Add(row["marking"].ToString());//Маркировка

                        listView1.Items.Add(lvi);
                    }
                //}

                //Теперь необходимо пересчитать все суммы по документу

                selection_goods = false;

                //ПРОВЕРОЧНЫЙ ПЕРЕСЧЕТ ПО АКЦИЯМ 
                MainStaticClass.write_event_in_log(" Попытка пересчитать чек ", "Документ чек", numdoc.ToString());
                recalculate_all();
                // КОНЕЦ ПРОВЕРОЧНЫЙ ПЕРЕСЧЕТ ПО АКЦИЯМ               
               
                //Если это киоск самообслуживания 
                //if (MainStaticClass.SelfServiceKiosk == 1)//это киоск самообслуживания 
                //{
                //    pay_form.cash_sum.Enabled         = false;
                //    pay_form.non_cash_sum.Enabled     = false;
                //    pay_form.non_cash_sum_kop.Enabled = false;
                //    pay_form.pay_sum.Text             = calculation_of_the_sum_of_the_document().ToString();

                //    double total = Convert.ToDouble(calculation_of_the_sum_of_the_document());
                //    string kop = ((int)((total - (int)total) * 100)).ToString();
                //    kop = (kop.Length == 2 ? kop : "0" + kop);
                //    pay_form.set_kop_on_non_cash_sum_kop(kop);
                //    pay_form.non_cash_sum.Text = ((int)total).ToString();
                //    //этот блок надо переделать
                //}
                //else
                //{
                    pay_form.pay_sum.Text = calculation_of_the_sum_of_the_document().ToString();
                    decimal total = calculation_of_the_sum_of_the_document();
                    string kop = ((int)((total - (int)total) * 100)).ToString();
                    kop = (kop.Length == 2 ? kop : "0" + kop);
                    pay_form.set_kop_on_non_cash_sum_kop(kop);
                //}

            }
            else
            {                
                pay_form.pay_sum.Text = calculation_of_the_sum_of_the_document().ToString();
                //if (MainStaticClass.SelfServiceKiosk == 1)
                //{
                //    pay_form.cash_sum.Enabled = false;
                //    pay_form.non_cash_sum.Enabled = false;
                //    pay_form.non_cash_sum_kop.Enabled = false;
                //    pay_form.pay_sum.Text = calculation_of_the_sum_of_the_document().ToString();

                //    double total = Convert.ToDouble(calculation_of_the_sum_of_the_document());
                //    string kop = ((int)((total - (int)total) * 100)).ToString();
                //    kop = (kop.Length == 2 ? kop : "0" + kop);
                //    pay_form.set_kop_on_non_cash_sum_kop(kop);
                //    pay_form.non_cash_sum.Text = ((int)total).ToString();
                //}
            }

            pay_form.cash_sum.Focus();
            //pay_form.TopMost = true;
            //pay_form.Show(); //для тестов пока так
            //Для бонусной программы проверяем заполненность пароля для передачи данных провайдеру


            //ДЕЙСТВИЯ ПО НОВОЙ БОНУСНОЙ ПРОГРАММЕ 
            //if (MainStaticClass.PassPromo != "")//Пароль не пустой бонусная магазин включен в бнусную систему
            //{
            //    if (check_type.SelectedIndex == 0) // Это продажа
            //    {
            //        if (MainStaticClass.GetWorkSchema == 2)
            //        {
            //            if (client_plastic_scaned)
            //            {
            //                pay_form.pay_bonus.Enabled = true;
            //            }
            //            //if (!change_bonus_card)//Если это не замена карты то проверить 
            //            //{
            //            //Проверяем есть ли в чеке бонусная карта на продажу.
            //            bool first = (card_state == 1);//В чек считана бонусная карта клиента со статусом 1 т.е. не активирована                       
            //            bool second = check_availability_card_sale();
            //            if (first != second)
            //            {

            //                if (client.Tag == null)
            //                {
            //                    MessageBox.Show("В шапке чека отсутсвует бонусная карта клиента со статусом 1 т.е. не активирована");
            //                    return;
            //                }
            //                if (client.Tag.ToString() != code_bonus_card)
            //                {
            //                    if (change_bonus_card)//Это замена карты дальше не проверяем
            //                    {
            //                        return;
            //                    }

            //                    if (first)
            //                    {
            //                        MessageBox.Show("В шапке чека существует бонусная карта клиента со статусом 1 т.е. не активирована, а в строках нет данной бонусной карты,необходимо ее добавить в строки");
            //                        cancel_action();
            //                        return;
            //                    }
            //                    else
            //                    {
            //                        MessageBox.Show("В шапке чека отсуствует бонусная карта клиента со статусом 1 т.е. не активирована, а в строках она есть,необходимо ее добавить в шапку чека");
            //                        cancel_action();
            //                        return;
            //                    }
            //                }
            //            }
            //            //}
            //        }
            //    }
            //}
            //При переходе в окно оплаты цены должны быть отрисованы
            SendDataToCustomerScreen(1,1,1);
            //pay_form.Top = this.Top;
            //pay_form.Left = this.Left;
            //pay_form.Right = this.Right;

            //pay_form.Top = this.Parent.Top
            dr = pay_form.ShowDialog();
            //pay_form.Dispose();
          
            if (dr == DialogResult.OK)
            {
                this.Close();
            }
            inputbarcode.Focus();
            pay_form = new Pay();


        }

        /// <summary>
        /// Проверить наличие в товарной части чека 
        /// наличие бонусной карты и записать ее код в переменную
        /// </summary>
        /// <returns></returns>
        private bool check_availability_card_sale()
        {
            bool result = false;
            code_bonus_card = "";

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "";
                int query_result = 0;
                foreach (ListViewItem lvi in listView1.Items)
                {
                    query = "SELECT its_certificate FROM tovar WHERE code="+lvi.SubItems[0].Text;
                    code_bonus_card = lvi.SubItems[0].Text;
                    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                    query_result = Convert.ToInt16(command.ExecuteScalar());
                    if (query_result == 2)
                    {
                        result = true;
                        break;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при проверке товарной части на наличие бонусной карты " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке товарной части на наличие бонусной карты " + ex.Message);
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

        /// <summary>
        /// Подсчет общего количества в документе 
        /// для бонусной программы
        /// </summary>
        /// <returns></returns>
        public int calculation_quantity_on_document()
        {
            int result = 0;

            foreach (ListViewItem lvi in listView1.Items)
            {
                result += Convert.ToInt32(lvi.SubItems[3].Text);
            }
            return result;
        }
        

        /// <summary>
        /// печть возвратной накладной
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void sale_cancellation_Click(string cash_money, string non_cash_money)
        {
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show(" Нет строк ", " Проверки переда записью документа ");
                return;
            }

            if (MainStaticClass.Use_Fiscall_Print)
            {
                fiscall_print_disburse(cash_money, non_cash_money);
            }
            //else
            //{
            //    fiscall_print_disburse_not_fiscall(cash_money,  non_cash_money);
            //}
        }


        private void set_sale_disburse_button()
        {
            //if ((!itsnew) && (itc_printed()))
            if (!itsnew)//Если документ не новый он для чтения и там ничего менять нельзя
            {
                return;
            }
            //if (MainStaticClass.SelfServiceKiosk == 1)
            //{
            //    this.client.Text = "";
            //    this.client.Tag = "";
            //}
            //else
            //{ 
            if (this.check_type.SelectedIndex == 1)
            {
                if (client.Tag != null)
                {
                    if (client.Tag.ToString().Trim() != "")//Выбрана дисконтная карта, тип документа изенен быть не может
                    {
                        MessageBox.Show(" Выбрана дисконтная карта, тип документа изменен быть не может ");
                        this.check_type.SelectedIndex = 0;
                        return;
                    }
                }
            }
            //}
        }

        private void check_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (itsnew)
            {
                this.check_type.Enabled = false;
                if (check_type.SelectedIndex >0)
                {
                    if (listView1.Items.Count > 0)
                    {
                        MessageBox.Show("Тип чека необходимо выбирать перед добавлением строк");
                        return;
                    }
                    btn_fill_on_sales.Visible = true;
                    txtB_num_sales.Visible = true;
                    //if (MainStaticClass.Code_right_of_user != 1)
                    //{
                    inputbarcode.Enabled = false;
                    client_barcode.Enabled = false;
                    txtB_client_phone.Enabled = false;
                    //}
                }
            }
            set_sale_disburse_button();
        }
                

        private void return_enter_Click(object sender, EventArgs e)
        {
            //здесь надо поставить проверки ввода

            if (return_quantity.Text.Trim().Length == 0)
            {
                MessageBox.Show("Не заполнено количество");
                return;
            }
            if (Convert.ToInt32(return_quantity.Text.Trim()) == 0)
            {
                MessageBox.Show("Количество не может быть нулевым !!!");
                return;
            }

            if (return_rouble.Text.Trim().Length == 0)
            {
                MessageBox.Show("Не заполнена цена");
                return;
            }
            if (Convert.ToInt32(return_rouble.Text.Trim()) == 0)
            {
                MessageBox.Show("Цена не может быть нулевой !!!");
                return;
            }

            this.panel_return.Visible = false;
            this.listView1.SelectedItems[0].SubItems[3].Text = this.return_quantity.Text;
            this.listView1.SelectedItems[0].SubItems[5].Text = this.return_rouble.Text.Trim() + "," + this.return_kop.Text.Trim();
            this.calculate_on_string(this.listView1.SelectedItems[0]);
            this.return_rouble.Text = "";
            this.return_quantity.Text = "";
            this.return_kop.Text = "";
            write_new_document("0", calculation_of_the_sum_of_the_document().ToString(), "0", "0", false, "0", "0", "0", "0");

        }
        
        private void load_document_Click(object sender, EventArgs e)
        {
            using (StreamReader reader = new StreamReader(Application.StartupPath + "/CashCheckTable.txt"))
            {
                string[] line_items;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line_items = line.Split('|');
                    ListViewItem lvi = new ListViewItem(line_items[0]);
                    lvi.Tag = line_items[0];
                    lvi.SubItems.Add(line_items[1]);//Наименование
                    lvi.SubItems.Add(line_items[2]);//Характеристика
                    lvi.SubItems[2].Tag = line_items[11];
                    lvi.SubItems.Add(line_items[3]);//Количество
                    lvi.SubItems.Add(line_items[4]);//Цена без скидки
                    lvi.SubItems.Add(line_items[5]);//Цена Со скидкой
                    lvi.SubItems.Add(line_items[6]);//Сумма без скидки
                    lvi.SubItems.Add(line_items[7]);//Сумма со скидкой
                    lvi.SubItems.Add(line_items[8]);//Акционный документ
                    lvi.SubItems.Add(line_items[9]);//Акционный документ
                    lvi.SubItems.Add(line_items[10]);//Акционный документ
                    lvi.SubItems.Add(line_items[11]);//Бонус
                    lvi.SubItems.Add(line_items[12]);//Бонус
                    lvi.SubItems.Add(line_items[13]);//Бонус
                    lvi.SubItems.Add(line_items[14]);//Маркер

                    listView1.Items.Add(lvi);

                }
            }
        }

        //private void check_action(ListView lv)
        //{
        //    string code_tovar_not_actions = "";

        //    foreach (ListViewItem lvi in listView1.Items)
        //    {
        //        if (lvi.SubItems[8].Text == "0" && lvi.SubItems[9].Text == "0" && lvi.SubItems[10].Text == "0")
        //        {
        //            code_tovar_not_actions += lvi.SubItems[0].Text + ",";
        //        }
        //    }

        //    if (code_tovar_not_actions.Length == 0)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        code_tovar_not_actions = code_tovar_not_actions.Substring(0, code_tovar_not_actions.Length - 1); 
        //    }

        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //    try
        //    {
        //        conn.Open();
        //        string query = " SELECT action_table.code_tovar,tovar.name,comment,action_table.num_doc " +
        //                       " FROM action_header LEFT JOIN action_table ON action_header.num_doc = action_table.num_doc " +
        //                       " LEFT JOIN tovar ON action_table.code_tovar = tovar.code " +
        //                       " WHERE '" + DateTime.Now.ToString("dd-MM-yyyy") + "' between date_started AND date_end " +
        //                       " AND action_header.tip in (2,3,4,6,8) AND action_table.code_tovar in (" + code_tovar_not_actions + ")" +
        //                       " GROUP BY action_table.code_tovar,tovar.name,action_table.num_doc,comment";
        //        NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //        NpgsqlDataReader reader = command.ExecuteReader();
        //        lv.Items.Clear();
        //        while (reader.Read())
        //        {
        //            ListViewItem lvi = new ListViewItem(reader[0].ToString());
        //            lvi.SubItems.Add(reader[1].ToString());
        //            lvi.SubItems.Add(reader[2].ToString());
        //            lv.Items.Add(lvi); 
        //        }
        //        reader.Close();
        //        conn.Close();
        //    }            
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(" Ошибка при анализе товаров вне акции " + ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(" Ошибка при анализе товаров вне акции " + ex.Message);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }
 
        //}

       
        private void btn_inpute_phone_client_Click(object sender, EventArgs e)
        {
            InputCodeNumberPhone inputCodeNumberPhone = new InputCodeNumberPhone();
            inputCodeNumberPhone.code_client = "";
            inputCodeNumberPhone.phone_client = "7"+txtB_client_phone.Text.Trim();
            DialogResult dialogResult = inputCodeNumberPhone.ShowDialog();
            if (dialogResult == DialogResult.Yes)
            {
                this.client.Tag = "7" + txtB_client_phone.Text.Trim();
                this.client.Text = "7" + txtB_client_phone.Text.Trim();
            }

            //InputePhoneClient ipc = new InputePhoneClient();
            //ipc.barcode = txtB_client_phone.Text;            
            //ipc.cash_Check = this;
            //DialogResult dr = ipc.ShowDialog();
            //btn_inpute_phone_client.Enabled = false;
            //inputbarcode.Focus();
            //if (dr == DialogResult.Yes)
            //{
            //    process_client_discount(ipc.txtB_phone_number.Text);
            //}
        }

        /// <summary>
        /// Возвращает true если клиент бонусный
        /// иначе false
        /// </summary>
        /// <returns></returns>
        //public bool check_bonus_is_on()
        //{
        //    bool result = false;

        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //    try
        //    {
        //        conn.Open();
        //        string query = "SELECT COALESCE(bonus_is_on,0)  FROM clients where code = '" + client.Tag.ToString() + "' and bonus_is_on=1";
        //        NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //        NpgsqlDataReader reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            result = true;
        //        }
        //        reader.Close();
        //        command.Dispose();
        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show("Ошибка при получении статуса бонусный"+ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Ошибка при получении статуса бонусный" + ex.Message);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return result;
        //}



        /// <summary>
        /// Проверка наличия номера телефона в карточке клиента
        /// </summary>
        /// <returns></returns>
        private bool check_client_have_telephone()
        {
            bool result = true;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT clients.phone AS phone, coalesce(temp_phone_clients.phone,'') AS phone1 FROM clients " +                    
                    " left join temp_phone_clients ON clients.code = temp_phone_clients.barcode " +
                    " WHERE clients.code='" + client.Tag.ToString()+"'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if ((reader["phone"].ToString().Trim().Length < 10)&&(reader["phone1"].ToString().Trim().Length < 10))
                    {
                        result = false;
                    }
                }
                reader.Close();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при проверка наличия телефона " + ex.Message);
                result = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверка наличия телефона " + ex.Message);
                result = false;
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

        /// <summary>
        /// Возвращает true если клиент уже есть в таблице
        /// иначе false
        /// при ошибках возвращает true
        /// т.е. блокирует дальнейшее выполнение 
        /// </summary>
        /// <returns></returns>
        private bool check_in_change_status_client()
        {
            bool result = true;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM client_with_changed_status_to_send where client='"+client.Tag.ToString()+"'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (Convert.ToInt16(command.ExecuteScalar()) == 0)
                {
                    result = false;
                }
                command.Dispose();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при проверке в промежуточной таблице статусов клиентов "+ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке в промежуточной таблице статусов клиентов " + ex.Message);
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

        //private void btn_change_status_client_Click(object sender, EventArgs e)
        //{
        //    if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema==3) || (MainStaticClass.GetWorkSchema == 4))
        //    {
        //        if (listView1.Items.Count > 0)
        //        {
        //            MessageBox.Show(" В чеке уже есть товар, операция по переходу невозможна  ");
        //            return;
        //        }

        //        if (MainStaticClass.PassPromo == "")
        //        {
        //            MessageBox.Show(" Эта касса не участвует в бонусной программе  ");
        //            return;
        //        }

        //        //Прежде чем вызвать окно изменения статуча надо проверить номер телефона
        //        if (!check_client_have_telephone())
        //        {
        //            MessageBox.Show(" У покупателя не заполнен номер телефона ");
        //            return;
        //        }

        //        //Проверить нет ли клиента в уже измененных
        //        if (check_in_change_status_client())
        //        {
        //            MessageBox.Show(" У этого покупателя уже возможно изменен статус ");
        //            return;
        //        }

        //        //if (check_bonus_is_on())
        //        //{
        //        //    MessageBox.Show(" У этого покупателя уже установлен статус бонусный ");
        //        //    return;
        //        //}

        //        ChangeBonusStatusClient changeBonusStatusClient = new ChangeBonusStatusClient();
        //        changeBonusStatusClient.client_code = this.client.Tag.ToString();
        //        DialogResult dr = changeBonusStatusClient.ShowDialog();
        //        if (dr == DialogResult.OK)
        //        {
        //            btn_change_status_client.Visible = false;
        //        }
        //    }
        //    else if (MainStaticClass.GetWorkSchema == 2)
        //    {
        //        if (client.Tag == null)
        //        {
        //            MessageBox.Show("Необходимо считать карту клиента в поле код клиента");
        //            return;
        //        }
        //        if (client.Tag.ToString().Substring(0, 2) != "29")
        //        {
        //            MessageBox.Show("Введенная карта не соответсвтует критериям, продолжение невозможно");
        //            return;
        //        }
                
        //        bool find = false;
        //        foreach (ListViewItem lvi in listView1.Items)
        //        {
        //            if (lvi.SubItems[0].Text == client.Tag.ToString())
        //            {
        //                find = true;
        //                break;
        //            }
        //        }

        //        if (find)
        //        {
        //            CreateBonusCardOrAddPhone createBonusCardOrAddPhone = new CreateBonusCardOrAddPhone();
        //            createBonusCardOrAddPhone.txtB_num_card.Text = client.Tag.ToString();
        //            createBonusCardOrAddPhone.its_new = true;
        //            createBonusCardOrAddPhone.cash_Check = this;                    
        //            createBonusCardOrAddPhone.ShowDialog();
        //        }
        //        else //Это замена карты
        //        {

        //        }
        //    }
        //}

        public class Suggestion
        {
            public string value { get; set; }
            public string unrestricted_value { get; set; }
        }

        public class Answer
        {
            public List<Suggestion> suggestions { get; set; }
        }

        private void btn_get_name_Click(object sender, EventArgs e)
        {
            if (txtB_inn.Text.Trim().Length == 0)
            {
                MessageBox.Show("Для получения наименования покупателя необходимо заполнить его ИНН");
                return;
            }
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create("https://suggestions.dadata.ru/suggestions/api/4_1/rs/findById/party");
                req.Method = "POST";
                req.ContentType = "application/json";
                req.Headers.Add("Authorization", "Token 9d101a5cde3d28f5bade72ea5613f8f536a4d219");
                req.Timeout = 20000;
                //{ "query": "7707083893" }
                string inn = "{\"query\": \"" + txtB_inn.Text + "\" }";                
                byte[] sentData = Encoding.UTF8.GetBytes(inn);
                req.ContentLength = sentData.Length;
                System.IO.Stream sendStream = req.GetRequestStream();
                sendStream.Write(sentData, 0, sentData.Length);
                sendStream.Close();
                System.Net.WebResponse res = req.GetResponse();
                System.IO.Stream ReceiveStream = res.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(ReceiveStream, Encoding.UTF8);
                //Кодировка указывается в зависимости от кодировки ответа сервера
                //sr.ReadToEnd();
                Char[] read = new Char[512];
                int count = sr.Read(read, 0, 512);
                string Out = String.Empty;
                while (count > 0)
                {
                    String str = new String(read, 0, count);
                    Out += str;
                    count = sr.Read(read, 0, 512);
                }
                //MessageBox.Show(Out);
                Answer suggestion = JsonConvert.DeserializeObject<Answer>(Out);
                if (suggestion.suggestions.Count > 0)
                {
                    txtB_name.Text = suggestion.suggestions[0].value;
                }
                else
                {
                    MessageBox.Show("По данному ИНН ничего не найдено");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("При поиске по ИНН произошли ошибки "+ex.Message);
            }
        }

        public class ClearMarkingCodeValidationResult
        {
            public string type { get; set; }
            public ClearMarkingCodeValidationResult()
            {
                type = "clearMarkingCodeValidationResult";
            }
        }

        //private static AnswerAddingKmArrayToTableTested GET_AnswerAddingKmArrayToTableTested(string Url, string Data)
        //{
        //    System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "/" + Data);
        //    req.Timeout = 10000;
        //    System.Net.WebResponse resp = req.GetResponse();
        //    //HttpWebResponse myHttpWebResponse = (HttpWebResponse)req.GetResponse();

        //    System.IO.Stream stream = resp.GetResponseStream();

        //    System.IO.StreamReader sr = new System.IO.StreamReader(stream);
        //    string Out = sr.ReadToEnd();
        //    sr.Close();

        //    //Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(Out);
        //    //var obj = JsonConvert.DeserializeObject(Out) as System.Collections.Generic.ICollection<Results>; 

        //    var results = JsonConvert.DeserializeObject<AnswerAddingKmArrayToTableTested>(Out);

        //    //AnswerAddingKmArrayToTableTested results = JsonConvert.DeserializeObject<AnswerAddingKmArrayToTableTested>(Out);
        //    //Thread.Sleep(1000);
        //    req = null;
        //    resp.Close();
        //    stream.Close();
        //    sr = null;

        //    return results;
        //    //return Out;
        //}

        private void clearMarkingCodeValidationResult()
        {
            ClearMarkingCodeValidationResult clearMarkingCodeValidationResult = new ClearMarkingCodeValidationResult();
            ////////////////////////////////////////
            string status = "";
            AnswerAddingKmArrayToTableTested result = null;
            string clearMarking = JsonConvert.SerializeObject(clearMarkingCodeValidationResult, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string json = MainStaticClass.shablon.Replace("body", clearMarking);
            string guid = Guid.NewGuid().ToString();
            string replace = "\"uuid\": \"" + guid + "\"";
            json = json.Replace("uuid", replace);
            if (FiscallPrintJason.POST(MainStaticClass.url, json) == "Created")
            {
                int count = 0;
                while (1 == 1)
                {
                    count++;
                    Thread.Sleep(1000);
                    result = GET_AnswerAddingKmArrayToTableTested(MainStaticClass.url, guid);
                    status = result.results[0].status;
                    if ((status != "ready")&&(status != "error"))
                    {
                        if (count > 14)
                        {
                            break;
                        }
                    }
                    else if ((status == "ready") || (status == "error"))
                    {
                        break;
                    }
                }
            }

            //return result;
            ////////////////////////////////////////
        }

        //private void fill_client_on_return(string code_client)
        //{
        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

        //    try
        //    {
        //        conn.Open();
        //        string query = "SELECT code, pin FROM bonus_cards WHERE code='"+ code_client+"'";
        //        NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //        NpgsqlDataReader reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            client.Tag = reader["code"].ToString();
        //            client.Text = reader["code"].ToString();
        //        }               
        //        reader.Close();
        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}
              

        public void fill_on_sales()
        {

            if (txtB_num_sales.Text.Trim().Length == 0)
            {
                return;
            }

            id_sale = Convert.ToInt32(txtB_num_sales.Text);
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema==3)|| (MainStaticClass.GetWorkSchema == 4))
            {
                try
                {
                    string query = "";

                    query = " SELECT dt.tovar_code, dt.name,SUM(dt.quantity)AS quantity, dt.price, dt.price_at_a_discount, SUM(dt.sum) AS sum," +
                                   "  SUM(dt.sum_at_a_discount) AS sum_at_a_discount, dt.id_transaction,dt.client,dt.item_marker" +
                                   " FROM " +
                                   " (SELECT tovar_code, tovar.name, quantity AS quantity, price, price_at_a_discount, sum, sum_at_a_discount," +
                                   " checks_header.id_transaction, checks_header.client, item_marker" +
                                   " FROM " +
                                   " checks_table LEFT JOIN tovar ON checks_table.tovar_code = tovar.code " +
                                   " LEFT JOIN checks_header ON checks_table.document_number = checks_header.document_number " +
                                   " WHERE checks_table.document_number = '" + txtB_num_sales.Text.Trim() + "' AND checks_header.check_type = 0 AND checks_header.its_deleted = 0 " +
                                   " AND checks_header.date_time_write BETWEEN '" + DateTime.Now.AddDays(-14).Date.ToString("dd-MM-yyyy") + "' AND  '" + DateTime.Now.AddDays(1).ToString("dd-MM-yyyy") + "'" +
                                   " UNION ALL " +
                                   " SELECT tovar_code, tovar.name, -quantity, price, price_at_a_discount, -sum, -sum_at_a_discount, checks_header.id_transaction," +
                                   " checks_header.client, item_marker FROM checks_table LEFT JOIN tovar ON checks_table.tovar_code = tovar.code " +
                                   " LEFT JOIN checks_header ON checks_table.document_number = checks_header.document_number " +
                                   " WHERE checks_header.id_sale = '" + txtB_num_sales.Text.Trim() + "'  AND checks_header.check_type = 1 AND checks_header.its_deleted = 0 " +
                                   " AND checks_header.date_time_write BETWEEN '" + DateTime.Now.AddDays(-14).Date.ToString("dd-MM-yyyy") + "' AND  '" + DateTime.Now.AddDays(1).ToString("dd-MM-yyyy") + "' )AS dt " +
                                   " GROUP BY " +
                                   "dt.tovar_code, dt.name, dt.price, dt.price_at_a_discount, dt.id_transaction,dt.client,dt.item_marker " +
                                   " HAVING SUM(dt.quantity) > 0 ";

                    conn.Open();
                    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        //if (its_sertificate(reader[0].ToString()))
                        //{
                        //    continue;
                        //}
                        //**************************
                        id_transaction_sale = reader["id_transaction"].ToString();
                        if (reader["client"].ToString().Trim().Length != 0)
                        {
                            client.Tag = reader["client"].ToString().Trim();
                            client.Text = reader["client"].ToString().Trim();
                            //fill_client_on_return(reader["client"].ToString().Trim());
                            client_barcode.Enabled = false;
                        }
                        //**************************
                        ListViewItem lvi = new ListViewItem(reader[0].ToString());
                        lvi.Tag = reader[0].ToString();
                        lvi.SubItems.Add(reader[1].ToString());//Наименование
                        lvi.SubItems.Add("");//Характеристика
                        lvi.SubItems[2].Tag = "";
                        lvi.SubItems.Add(reader[2].ToString());//Количество
                        lvi.SubItems.Add(reader[3].ToString());//Цена без скидки
                        lvi.SubItems.Add(reader[4].ToString());//Цена Со скидкой
                        lvi.SubItems.Add(reader[5].ToString());//Сумма без скидки
                        lvi.SubItems.Add(reader[6].ToString());//Сумма со скидкой
                        lvi.SubItems.Add("0");//Акционный документ
                        lvi.SubItems.Add("0");//Акционный документ
                        lvi.SubItems.Add("0");//Акционный документ
                        lvi.SubItems.Add("0");//Бонус
                        lvi.SubItems.Add("0");//Бонус
                        lvi.SubItems.Add("0");//Бонус
                        lvi.SubItems.Add(reader["item_marker"].ToString().Replace("vasya2021", "'"));//Маркер                    
                        listView1.Items.Add(lvi);
                    }

                    query = "SELECT id_transaction_terminal,code_authorization_terminal,date_time_write,non_cash_money FROM  checks_header WHERE document_number=" + txtB_num_sales.Text;
                    command = new NpgsqlCommand(query, conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        sale_id_transaction_terminal = reader["id_transaction_terminal"].ToString();
                        sale_code_authorization_terminal = reader["code_authorization_terminal"].ToString();
                        sale_date = Convert.ToDateTime(reader["date_time_write"]);
                        sale_non_cash_money = Convert.ToDouble(reader["non_cash_money"]);

                    }

                    reader.Close();
                    conn.Close();
                    command.Dispose();                    
                    write_new_document("0", calculation_of_the_sum_of_the_document().ToString(), "0", "0", false, "0", "0", "0", "0");
                }
                catch (NpgsqlException ex)
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
            //else if (MainStaticClass.GetWorkSchema == 2)
            //{
            //    try
            //    {
            //        string query_sales = "SELECT tovar_code, tovar.name, quantity AS quantity, price, price_at_a_discount, sum, sum_at_a_discount," +
            //                            " checks_header.id_transaction, checks_header.client, item_marker " +
            //                            " FROM " +
            //                            " checks_table LEFT JOIN tovar ON checks_table.tovar_code = tovar.code " +
            //                            " LEFT JOIN checks_header ON checks_table.document_number = checks_header.document_number " +
            //                            " WHERE checks_table.document_number = '" + txtB_num_sales.Text.Trim() + "' AND checks_header.check_type = 0 AND checks_header.its_deleted = 0 " +
            //                            " AND checks_header.date_time_write BETWEEN '" + DateTime.Now.AddDays(-14).Date.ToString("dd-MM-yyyy") +
            //                            "' AND  '" + DateTime.Now.AddDays(1).ToString("dd-MM-yyyy") + "'";

            //        string query_return = " SELECT tovar_code, tovar.name, quantity, price, price_at_a_discount, sum, sum_at_a_discount, checks_header.id_transaction," +
            //                                " checks_header.client, item_marker FROM checks_table LEFT JOIN tovar ON checks_table.tovar_code = tovar.code " +
            //                                " LEFT JOIN checks_header ON checks_table.document_number = checks_header.document_number " +
            //                                " WHERE checks_header.id_sale = '" + txtB_num_sales.Text.Trim() + "'  AND checks_header.check_type = 1 AND checks_header.its_deleted = 0 " +
            //                                " AND checks_header.date_time_write BETWEEN '" + DateTime.Now.AddDays(-14).Date.ToString("dd-MM-yyyy") + "' AND  '" +
            //                                DateTime.Now.AddDays(1).ToString("dd-MM-yyyy") + "'";

            //        conn.Open();
            //        DataTable t_sales = new DataTable();
            //        using (NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(query_sales, conn))
            //        {
            //            npgsqlDataAdapter.Fill(t_sales);
            //        }

            //        DataTable t_return = new DataTable();
            //        using (NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter(query_return, conn))
            //        {
            //            npgsqlDataAdapter.Fill(t_return);
            //        }

            //        foreach (DataRow row in t_return.Rows)
            //        {
            //            DataRow[] result = t_sales.Select("tovar_code=" + row["tovar_code"] +
            //                                                " AND price_at_a_discount='" + row["price_at_a_discount"] + "'"+
            //                                                " AND item_marker='" + row["item_marker"] + "'");
            //            if (result.Length > 0)
            //            {
            //                t_sales.Rows.Remove(result[0]);
            //            }
            //        }

            //        if (t_sales.Rows.Count > 0)
            //        {
            //            foreach (DataRow row in t_sales.Rows)
            //            {
            //                if (its_sertificate(row["tovar_code"].ToString()))
            //                {
            //                    continue;
            //                }
            //                id_transaction_sale = row["id_transaction"].ToString();
            //                if (row["client"].ToString().Trim().Length != 0)
            //                {
            //                    client.Tag = row["client"].ToString().Trim();// Надо заполнить еще и строку                                 
            //                    client.Text = row["client"].ToString().Trim();
            //                    client_barcode.Enabled = false;
            //                }
            //                ListViewItem lvi = new ListViewItem(row["tovar_code"].ToString());
            //                lvi.Tag = row["tovar_code"].ToString();
            //                lvi.SubItems.Add(row["name"].ToString());//Наименование
            //                lvi.SubItems.Add("");//Характеристика
            //                lvi.SubItems[2].Tag = "";
            //                lvi.SubItems.Add(row["quantity"].ToString());//Количество
            //                lvi.SubItems.Add(row["price"].ToString());//Цена без скидки
            //                lvi.SubItems.Add(row["price_at_a_discount"].ToString());//Цена Со скидкой
            //                lvi.SubItems.Add(row["sum"].ToString());//Сумма без скидки
            //                lvi.SubItems.Add(row["sum_at_a_discount"].ToString());//Сумма со скидкой
            //                lvi.SubItems.Add("0");//Акционный документ
            //                lvi.SubItems.Add("0");//Акционный документ
            //                lvi.SubItems.Add("0");//Акционный документ
            //                lvi.SubItems.Add("0");//Бонус
            //                lvi.SubItems.Add("0");//Бонус
            //                lvi.SubItems.Add("0");//Бонус
            //                lvi.SubItems.Add(row["item_marker"].ToString().Replace("vasya2021", "'"));//Маркер                    
            //                listView1.Items.Add(lvi);
            //            }
            //        }                    
            //        conn.Close();                    
            //    }
            //    catch (NpgsqlException ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //    finally
            //    {
            //        if (conn.State == ConnectionState.Open)
            //        {
            //            conn.Close();
            //        }
            //    }
            //    if (listView1.Items.Count == 0)
            //    {
            //        MessageBox.Show(" По введенному номеру  " + txtB_num_sales.Text + " за период 14 дней чек не найден ");
            //    }
            //    else
            //    {
            //        comment.Text = txtB_num_sales.Text;
            //        btn_fill_on_sales.Enabled = false;
            //    }
            //    write_new_document("0", calculation_of_the_sum_of_the_document().ToString(), "0", "0", false, "0", "0", "0", "0");
            //}
        }

    /// <summary>
    /// Заполнить табличную часть возврата
    /// по номеру чека продажи
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btn_fill_on_sales_Click(object sender, EventArgs e)
        {

            if (listView1.Items.Count > 0)
            {
                DialogResult dr = MessageBox.Show(" Перезаполнить товары в чеке ?", "", MessageBoxButtons.YesNo);
                if (dr == DialogResult.No)
                {
                    return;
                }
                else
                {
                    listView1.Items.Clear();
                }              
            }

            fill_on_sales();            
        }

        //private void checkBox_viza_d_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (client.Tag == null)
        //    {
        //        return;
        //    }
        //    if (checkBox_viza_d.Checked)
        //    {
        //        Discount = Convert.ToDouble(0.05);
        //    }
        //    else
        //    {
        //        Discount = 0;
        //    }

        //    recalculate_all();
        //}

        private void checkBox_to_print_repeatedly_p_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_to_print_repeatedly_p.CheckState == CheckState.Checked)
            {
                to_print_certainly_p = 1;
            }
            else
            {
                to_print_certainly_p = 0;
            }
        }

        private void btn_del_position_Click(object sender, EventArgs e)
        {
            KeyEventArgs args = new KeyEventArgs(Keys.Delete);            
            ListView1_KeyDown(null, args);
        }

        private void btn_cancel_check_Click(object sender, EventArgs e)
        {
            KeyEventArgs args = new KeyEventArgs(Keys.F12);
            OnKeyDown(args);
        }       
    }
}