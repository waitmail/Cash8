using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using System.Net;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;



namespace Cash8
{
    public partial class FPTK22 : Form
    {
        private bool    complete      = false;
        private string  recharge_note = "";

        public FPTK22()
        {
            InitializeComponent();
            sum_avans.KeyPress += new KeyPressEventHandler(sum_avans_KeyPress);
            incass.KeyPress += new KeyPressEventHandler(incass_KeyPress);
            this.Load += new EventHandler(FPTK22_Load);
        }

        [XmlRoot(ElementName = "field")]
        public class Field
        {

            [XmlAttribute(AttributeName = "id")]
            public string Id { get; set; }

            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "response")]
        public class Response
        {

            [XmlElement(ElementName = "field")]
            public List<Field> Field { get; set; }
        }

        private void FPTK22_Load(object sender, EventArgs e)
        {
            get_fiscall_info();

            btn_ofd_exchange_status_Click(null, null);
            btn_have_internet_Click(null, null);

            btn_send_fiscal_Click(null, null);
            if (MainStaticClass.Code_right_of_user == 1)
            {
                get_summ_in_cashe_Click(null, null);
            }
            else
            {
                get_summ_in_cashe.Enabled = false;
                x_report.Enabled = false;
            }
            if ((MainStaticClass.IpAddressAcquiringTerminal.Trim() != "") && (MainStaticClass.IdAcquirerTerminal.Trim() != ""))
            {
                btn_reconciliation_of_totals.Enabled = true;
                btn_query_summary_report.Enabled = true;
                btn_query_full_report.Enabled = true;

            }            

        }


        private void get_fiscall_info()
        {
            if (MainStaticClass.PrintingUsingLibraries == 0)
            {
                try
                {
                    Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.execute_operator_type("getFnInfo");
                    if (result != null)
                    {
                        if (result.results[0].status == "ready")//Задание выполнено успешно 
                        {
                            //sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
                            if (result.results[0].result.fnInfo.validityDate.Date > DateTime.Today.Date)
                            {
                                (result.results[0].result.fnInfo.validityDate.Date - DateTime.Today.Date).TotalDays.ToString();
                            }
                            string fn_info = "Ресурс ФН " + (result.results[0].result.fnInfo.validityDate.Date - DateTime.Today.Date).TotalDays.ToString() + " дней ";
                            if (result.results[0].result.fnInfo.warnings.needReplacement)
                            {
                                fn_info += "\r\n" + "Требуется срочная замена ФН !!!";
                            }
                            if (result.results[0].result.fnInfo.warnings.ofdTimeout)
                            {
                                fn_info += "\r\n" + "Превышено время ожидания ответа от ОФД !!!";
                            }
                            if (result.results[0].result.fnInfo.warnings.memoryOverflow)
                            {
                                fn_info += "\r\n" + "Память ФН переполнена !!!";
                            }
                            if (result.results[0].result.fnInfo.warnings.resourceExhausted)
                            {
                                fn_info += "\r\n" + "Исчерпан ресурс ФН !!!";
                            }
                            if (result.results[0].result.fnInfo.warnings.criticalError)
                            {
                                fn_info += "\r\n" + "Критическая ошибка ФН !!!";
                            }

                            txtB_fn_info.Text = fn_info;
                        }
                        else
                        {
                            MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Общая ошибка");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("get_fiscall_info" + ex.Message);
                }
            }
            else
            {
                PrintingUsingLibraries printing = new PrintingUsingLibraries();
                txtB_fn_info.Text = printing.getFiscallInfo();               
            }
        }

        void incass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)) && !((e.KeyChar == '.')))
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }
        }

        void sum_avans_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)) && !((e.KeyChar == '.')))
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }
        }

        private void avans_Click(object sender, EventArgs e)
        {
            avans.Enabled = false;
            if (MainStaticClass.PrintingUsingLibraries == 0)
            {
                if (sum_avans.Text.Trim().Length == 0)
                {
                    MessageBox.Show(" Сумма внесения не заполнена ");
                    return;
                }

                if (Convert.ToDouble(sum_avans.Text) == 0)
                {
                    MessageBox.Show(" Сумма внесения должна быть больше нуля ");
                    return;
                }
                try
                {
                    Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.cashe_in_out("cashIn", Convert.ToDouble(sum_avans.Text));
                    if (result != null)
                    {
                        if (result.results[0].status == "ready")//Задание выполнено успешно 
                        {
                            sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
                        }
                        else
                        {
                            MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Общая ошибка");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("avans_Click" + ex.Message);
                }
            }
            else
            {
                PrintingUsingLibraries printing = new PrintingUsingLibraries();
                printing.cashIncome(Convert.ToDouble(sum_avans.Text));
            }
            avans.Enabled = true;
        }

        private void incass_Click(object sender, EventArgs e)
        {
            incass.Enabled = false;
            if (MainStaticClass.PrintingUsingLibraries == 0)
            {
                try
                {
                    Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.cashe_in_out("cashOut", Convert.ToDouble(sum_incass.Text));
                    if (result != null)
                    {
                        if (result.results[0].status == "ready")//Задание выполнено успешно 
                        {
                            sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
                        }
                        else
                        {
                            MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Общая ошибка");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("incass_Click" + ex.Message);
                }
            }
            else
            {
                PrintingUsingLibraries printing = new PrintingUsingLibraries();
                printing.cashOutcome(Convert.ToDouble(sum_incass.Text));
            }
            incass.Enabled = true;
        }

        private void x_report_Click(object sender, EventArgs e)
        {
            if (MainStaticClass.PrintingUsingLibraries == 0)
            {
                try
                {
                    Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.execute_operator_type("reportX");
                    if (result != null)
                    {
                        if (result.results[0].status == "ready")//Задание выполнено успешно 
                        {
                            //sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
                        }
                        else
                        {
                            MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Общая Ошибка");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("x_report_Click" + ex.Message);
                }
            }
            else
            {
                PrintingUsingLibraries printing = new PrintingUsingLibraries();
                printing.reportX();
            }
        }

        private void z_report_Click(object sender, EventArgs e)
        {
            if (MainStaticClass.PrintingUsingLibraries == 0)
            {
                try
                {
                    Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.execute_operator_type("closeShift");
                    if (result != null)
                    {
                        if (result.results[0].status == "ready")//Задание выполнено успешно 
                        {
                            //sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
                        }
                        else
                        {
                            MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Общая ошибка");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("z_report_Click" + ex.Message);
                }
            }
            else
            {
                PrintingUsingLibraries printing = new PrintingUsingLibraries();
                printing.reportZ();
            }
        }

        private void annul_check_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(FiscallPrintJason.delete_last_job());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void print_last_check_Click(object sender, EventArgs e)
        {
            //printLastReceiptCopy
            try
            {
                Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.execute_operator_type("printLastReceiptCopy");
                if (result != null)
                {
                    if (result.results[0].status == "ready")//Задание выполнено успешно 
                    {
                        //sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
                    }
                    else
                    {
                        MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                    }
                }
                else
                {
                    MessageBox.Show("Общая ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("print_last_check_Click" + ex.Message);
            }
        }

        private void get_summ_in_cashe_Click(object sender, EventArgs e)
        {
            if (MainStaticClass.PrintingUsingLibraries == 0)
            {
                try
                {
                    Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.execute_operator_type("getCashDrawerStatus");
                    if (result != null)
                    {
                        if (result.results[0].status == "ready")//Задание выполнено успешно 
                        {
                            sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
                        }
                        else
                        {
                            MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Общая ошибка");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("get_summ_in_cashe_Click" + ex.Message);
                }
            }
            else
            {
                PrintingUsingLibraries printing = new PrintingUsingLibraries();
                sum_incass.Text = printing.getCasheSumm();
            }
        }


        private void zero_check_Click(object sender, EventArgs e)
        {

        }

        private void btn_have_internet_Click(object sender, EventArgs e)
        {
            if (MainStaticClass.get_exists_internet())
            {
                txtB_have_internet.Text = "Работает";
                txtB_have_internet.BackColor = Color.Green;
            }
            else
            {
                txtB_have_internet.Text = "Не работает";
                txtB_have_internet.BackColor = Color.Gold;
            }
        }

        private void btn_ofd_exchange_status_Click(object sender, EventArgs e)
        {
            if (MainStaticClass.PrintingUsingLibraries == 0)
            {
                txtB_ofd_exchange_status.BackColor = Color.Green;
                Cash8.FiscallPrintJason.RootObject result = MainStaticClass.get_ofd_exchange_status();
                if (result != null)
                {
                    if (result.results[0].result != null)
                    {

                        if (result.results[0].result.status.notSentCount > 0)
                        {
                            if ((DateTime.Now - result.results[0].result.status.notSentFirstDocDateTime).Days > 3)
                            {
                                txtB_ofd_exchange_status.Text = "Не отправлено документов " + result.results[0].result.status.notSentCount.ToString().Trim() + "\r\n" +
                                " начиная с даты " + result.results[0].result.status.notSentFirstDocDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                                txtB_ofd_exchange_status.BackColor = Color.Gold;
                            }
                            else
                            {
                                txtB_ofd_exchange_status.Text = "Не отправлено документов " + result.results[0].result.status.notSentCount.ToString().Trim();
                            }
                        }
                        else
                        {
                            txtB_ofd_exchange_status.Text = "Все отправлено";
                        }
                    }
                    else
                    {
                        txtB_ofd_exchange_status.Text = "Нет связи";
                        txtB_ofd_exchange_status.BackColor = Color.Gold;
                    }
                }
                else
                {
                    txtB_ofd_exchange_status.Text = "Нет связи";
                    txtB_ofd_exchange_status.BackColor = Color.Gold;
                }
            }
            else
            {
                PrintingUsingLibraries printing = new PrintingUsingLibraries();
                txtB_ofd_exchange_status.Text = printing.ofdExchangeStatus();
            }
        }

        private void btn_send_fiscal_Click(object sender, EventArgs e)
        {
            Process[] ethOverUsbProcesses = Process.GetProcessesByName("EthOverUsb");
            if (ethOverUsbProcesses.Length > 0)
            {
                txtB_ofd_utility_status.Text = "Запущена";
                txtB_ofd_utility_status.BackColor = Color.Green;
            }
            else
            {
                txtB_ofd_utility_status.Text = "Не запущена";
                txtB_ofd_utility_status.BackColor = Color.Gold;
            }
        }

        public class AnswerTerminal
        {
            public string code_authorization { get; set; }
            public string number_reference { get; set; }

            public AnswerTerminal()
            {
                number_reference = "";
                code_authorization = "";
            }
        }

        /// <summary>
        /// Отправляет команду в эквайринг
        /// терминал и возвращает результат
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Data"></param>
        /// <param name="status"></param>
        public void send_command_acquiring_terminal(string Url, string Data, ref bool status, ref AnswerTerminal answerTerminal)
        {
            //string Out = String.Empty;
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(Url);
                req.Method = "POST";
                req.Timeout = 60000;
                req.ContentType = "text/xml;charset = windows-1251";
                //req.ContentType = "text/xml;charset = UTF-8";                
                byte[] sentData = Encoding.GetEncoding("Windows-1251").GetBytes(Data);
                //byte[] sentData = Encoding.UTF8.GetBytes(Data);
                req.ContentLength = sentData.Length;
                System.IO.Stream sendStream = req.GetRequestStream();
                sendStream.Write(sentData, 0, sentData.Length);
                sendStream.Close();
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)req.GetResponse();
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    //status = true;
                    var streamReader = new StreamReader(myHttpWebResponse.GetResponseStream(), Encoding.GetEncoding("Windows-1251"));
                    var responseContent = streamReader.ReadToEnd();

                    XmlSerializer serializer = new XmlSerializer(typeof(Response));
                    using (StringReader reader = new StringReader(responseContent))
                    {
                        var test = (Response)serializer.Deserialize(reader);
                        foreach (Field field in test.Field)
                        {
                            if (field.Id == "39")
                            {
                                if (field.Text.Trim() == "1")
                                {
                                    status = true;
                                }
                                else
                                {
                                    status = false;
                                }
                            }                           
                            else if (field.Id == "90")
                            {
                                recharge_note = field.Text.Trim();
                            }
                        }
                    }
                }
                else
                {
                    status = false;
                    MessageBox.Show(" Ответ от терминала "+myHttpWebResponse.StatusCode.ToString());
                }

                req = null;
                sendStream = null;
                myHttpWebResponse.Close();// = null;
            }
            catch (WebException ex)
            {
                status = false;
                MessageBox.Show(" Ошибка при сверке итогов  " + ex.Message);
            }
            catch (Exception ex)
            {
                status = false;
                MessageBox.Show(" Ошибка при сверке итогов  " + ex.Message);
            }
        }


        private void btn_reconciliation_of_totals_Click(object sender, EventArgs e)
        {
            string url = "http://" + MainStaticClass.IpAddressAcquiringTerminal;
            string _str_command_ = @"<?xml version=""1.0"" encoding=""utf-8""?><request><field id=""25"">59</field><field id=""27"">id_terminal</field></request>";
            _str_command_ = _str_command_.Replace("id_terminal", MainStaticClass.IdAcquirerTerminal);
            recharge_note = "";
            AnswerTerminal answerTerminal = new AnswerTerminal();
            send_command_acquiring_terminal(url, _str_command_, ref complete, ref answerTerminal);
            if (complete)//ответ от терминала не удовлетворительный
            {
                if (recharge_note != "")
                {
                    FiscallPrintJason2.NonFiscallDocument nonFiscallDocument = new FiscallPrintJason2.NonFiscallDocument();
                    nonFiscallDocument.type = "nonFiscal";
                    nonFiscallDocument.printFooter = false;

                    FiscallPrintJason2.ItemNonFiscal itemNonFiscal = new FiscallPrintJason2.ItemNonFiscal();
                    nonFiscallDocument.items = new List<FiscallPrintJason2.ItemNonFiscal>();

                    itemNonFiscal.type = "text";
                    itemNonFiscal.text = recharge_note.Replace("0xDF^^", "");
                    itemNonFiscal.alignment = "center";
                    nonFiscallDocument.items.Add(itemNonFiscal);
                    FiscallPrintJason2.print_not_fiscal_document(nonFiscallDocument);
                    recharge_note = "";
                }
            }
            else
            {

            }
        }

        private void btn_query_summary_report_Click(object sender, EventArgs e)
        {
            string url = "http://" + MainStaticClass.IpAddressAcquiringTerminal;
            string _str_command_ = @"<?xml version=""1.0"" encoding=""utf-8""?><request><field id=""25"">63</field><field id=""27"">id_terminal</field><field id=""65"">20</field></request>";
            _str_command_ = _str_command_.Replace("id_terminal", MainStaticClass.IdAcquirerTerminal);
            recharge_note = "";
            AnswerTerminal answerTerminal = new AnswerTerminal();
            send_command_acquiring_terminal(url, _str_command_, ref complete, ref answerTerminal);
            if (complete)//ответ от терминала не удовлетворительный
            {
                if (recharge_note != "")
                {
                    FiscallPrintJason2.NonFiscallDocument nonFiscallDocument = new FiscallPrintJason2.NonFiscallDocument();
                    nonFiscallDocument.type = "nonFiscal";
                    nonFiscallDocument.printFooter = false;

                    FiscallPrintJason2.ItemNonFiscal itemNonFiscal = new FiscallPrintJason2.ItemNonFiscal();
                    nonFiscallDocument.items = new List<FiscallPrintJason2.ItemNonFiscal>();

                    itemNonFiscal.type = "text";
                    itemNonFiscal.text = recharge_note.Replace("0xDF^^", "");
                    itemNonFiscal.alignment = "center";
                    nonFiscallDocument.items.Add(itemNonFiscal);
                    FiscallPrintJason2.print_not_fiscal_document(nonFiscallDocument);
                    recharge_note = "";
                }
            }
            else
            {

            }

        }

        private void btn_query_full_report_Click(object sender, EventArgs e)
        {
            string url = "http://" + MainStaticClass.IpAddressAcquiringTerminal;
            string _str_command_ = @"<?xml version=""1.0"" encoding=""utf-8""?><request><field id=""25"">63</field><field id=""27"">id_terminal</field><field id=""65"">21</field></request>";
            _str_command_ = _str_command_.Replace("id_terminal", MainStaticClass.IdAcquirerTerminal);
            recharge_note = "";
            AnswerTerminal answerTerminal = new AnswerTerminal();
            send_command_acquiring_terminal(url, _str_command_, ref complete, ref answerTerminal);
            if (complete)//ответ от терминала не удовлетворительный
            {
                if (recharge_note != "")
                {
                    FiscallPrintJason2.NonFiscallDocument nonFiscallDocument = new FiscallPrintJason2.NonFiscallDocument();
                    nonFiscallDocument.type = "nonFiscal";
                    nonFiscallDocument.printFooter = false;

                    FiscallPrintJason2.ItemNonFiscal itemNonFiscal = new FiscallPrintJason2.ItemNonFiscal();
                    nonFiscallDocument.items = new List<FiscallPrintJason2.ItemNonFiscal>();

                    itemNonFiscal.type = "text";
                    itemNonFiscal.text = recharge_note.Replace("0xDF^^", "");
                    itemNonFiscal.alignment = "center";
                    nonFiscallDocument.items.Add(itemNonFiscal);
                    FiscallPrintJason2.print_not_fiscal_document(nonFiscallDocument);
                    recharge_note = "";
                }
            }
            else
            {

            }
        }
    }
}
