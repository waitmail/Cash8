using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

using System.Net;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;
using Atol.Drivers10.Fptr;
using AtolConstants = Atol.Drivers10.Fptr.Constants;
using Npgsql;



namespace Cash8
{
    public partial class FPTK22 : Form
    {
        private bool    complete      = false;
        private string  recharge_note = "";
        private DateTime dateTimeEnd;        

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
                z_report.Enabled = false;
            }

            if ((MainStaticClass.IpAddressAcquiringTerminal.Trim() != "") && (MainStaticClass.IdAcquirerTerminal.Trim() != ""))
            {
                btn_reconciliation_of_totals.Enabled = true;
                if (MainStaticClass.GetAcquiringBank == 1)
                {
                    btn_query_summary_report.Enabled = true;
                }
                btn_query_full_report.Enabled = true;
            }

            get_fiscall_info();

            btn_date_mark_Click(null, null);
            
            load_status_open_shop();
            load_status_close_shop();
        }


        private void get_fiscall_info()
        {
            //if (MainStaticClass.PrintingUsingLibraries == 0)
            //{
            //    try
            //    {
            //        Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.execute_operator_type("getFnInfo");
            //        if (result != null)
            //        {
            //            if (result.results[0].status == "ready")//Задание выполнено успешно 
            //            {
            //                //sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
            //                if (result.results[0].result.fnInfo.validityDate.Date > DateTime.Today.Date)
            //                {
            //                    (result.results[0].result.fnInfo.validityDate.Date - DateTime.Today.Date).TotalDays.ToString();
            //                }
            //                string fn_info = "Ресурс ФН " + (result.results[0].result.fnInfo.validityDate.Date - DateTime.Today.Date).TotalDays.ToString() + " дней ";
            //                if (result.results[0].result.fnInfo.warnings.needReplacement)
            //                {
            //                    fn_info += "\r\n" + "Требуется срочная замена ФН !!!";
            //                }
            //                if (result.results[0].result.fnInfo.warnings.ofdTimeout)
            //                {
            //                    if (txtB_ofd_exchange_status.Text.Contains("Не отправлено документов"))
            //                    {
            //                        fn_info += "\r\n" + "Превышено время ожидания ответа от ОФД !!!";
            //                    }
            //                }
            //                if (result.results[0].result.fnInfo.warnings.memoryOverflow)
            //                {
            //                    fn_info += "\r\n" + "Память ФН переполнена !!!";
            //                }
            //                if (result.results[0].result.fnInfo.warnings.resourceExhausted)
            //                {
            //                    fn_info += "\r\n" + "Исчерпан ресурс ФН !!!";
            //                }
            //                if (result.results[0].result.fnInfo.warnings.criticalError)
            //                {
            //                    fn_info += "\r\n" + "Критическая ошибка ФН !!!";
            //                }

            //                txtB_fn_info.Text = fn_info;
            //            }
            //            else
            //            {
            //                MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("Общая ошибка");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("get_fiscall_info" + ex.Message);
            //    }
            //}
            //else
            //{
                PrintingUsingLibraries printing = new PrintingUsingLibraries();
                txtB_fn_info.Text = printing.getFiscallInfo();               
            //}
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

            if (!MainStaticClass.continue_process(dateTimeEnd, 1))
            {                
                return;
            }


            MainStaticClass.validate_date_time_with_fn(13);

            avans.Enabled = false;
            //if (MainStaticClass.PrintingUsingLibraries == 0)
            //{
            //    if (sum_avans.Text.Trim().Length == 0)
            //    {
            //        MessageBox.Show(" Сумма внесения не заполнена ");
            //        return;
            //    }

            //    if (Convert.ToDouble(sum_avans.Text.Replace(".",",")) == 0)
            //    {
            //        MessageBox.Show(" Сумма внесения должна быть больше нуля ");
            //        return;
            //    }
            //    try
            //    {
            //        Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.cashe_in_out("cashIn", Convert.ToDouble(sum_avans.Text.Replace(".",",")));
            //        if (result != null)
            //        {
            //            if (result.results[0].status == "ready")//Задание выполнено успешно 
            //            {
            //                sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
            //            }
            //            else
            //            {
            //                MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("Общая ошибка");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("avans_Click" + ex.Message);
            //    }
            //}
            //else
            //{
                if (MainStaticClass.GetFiscalsForbidden)
                {
                    MessageBox.Show("Вам запрещена печать на фискаольном регистраторое", "Проверки при печати");
                    return;
                }
                else
                {
                    PrintingUsingLibraries printing = new PrintingUsingLibraries();
                    printing.cashIncome(Convert.ToDouble(sum_avans.Text.Replace(".", ",")));
                    get_summ_in_cashe_Click(null, null);
                }                
            //}
            //sum_avans.Text = "0";
            avans.Enabled = true;
            dateTimeEnd = DateTime.Now;
        }

        private void incass_Click(object sender, EventArgs e)
        {
            incass.Enabled = false;
            //if (MainStaticClass.PrintingUsingLibraries == 0)
            //{
            //    try
            //    {
            //        Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.cashe_in_out("cashOut", Convert.ToDouble(sum_incass.Text.Replace(".",",")));
            //        if (result != null)
            //        {
            //            if (result.results[0].status == "ready")//Задание выполнено успешно 
            //            {
            //                sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
            //            }
            //            else
            //            {
            //                MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("Общая ошибка");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("incass_Click" + ex.Message);
            //    }
            //}
            //else
            //{
            //    if (MainStaticClass.GetFiscalsForbidden)
            //    {
            //        MessageBox.Show("Вам запрещена печать на фискаольном регистраторое", "Проверки при печати");
            //        return;
            //    }
            //    else
            //    {
                    PrintingUsingLibraries printing = new PrintingUsingLibraries();
                    printing.cashOutcome(Convert.ToDouble(sum_incass.Text.Replace(".", ",")));
                    get_summ_in_cashe_Click(null, null);
                //}
            //}
            incass.Enabled = true;
        }

        private void x_report_Click(object sender, EventArgs e)
        {
            //if (MainStaticClass.PrintingUsingLibraries == 0)
            //{
            //    try
            //    {
            //        Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.execute_operator_type("reportX");
            //        if (result != null)
            //        {
            //            if (result.results[0].status == "ready")//Задание выполнено успешно 
            //            {
            //                //sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
            //            }
            //            else
            //            {
            //                MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("Общая Ошибка");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("x_report_Click" + ex.Message);
            //    }
            //}
            //else
            //{
                if (MainStaticClass.GetFiscalsForbidden)
                {
                    MessageBox.Show("Вам запрещена печать на фискаольном регистраторое", "Проверки при печати");
                    return;
                }
                else
                {
                    PrintingUsingLibraries printing = new PrintingUsingLibraries();
                    printing.reportX();
                }
            //}
        }

        public void z_report_Click(object sender, EventArgs e)
        {
            //if (MainStaticClass.PrintingUsingLibraries == 0)
            //{
            //    try
            //    {
            //        Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.execute_operator_type("closeShift");
            //        if (result != null)
            //        {
            //            if (result.results[0].status == "ready")//Задание выполнено успешно 
            //            {
            //                //sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
            //            }
            //            else
            //            {
            //                MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("Общая ошибка");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("z_report_Click" + ex.Message);
            //    }
            //}
            //else
            //{
                if (MainStaticClass.GetFiscalsForbidden)
                {
                    MessageBox.Show("Вам запрещена печать на фискаольном регистраторое", "Проверки при печати");
                    return;
                }
                else
                {
                    PrintingUsingLibraries printing = new PrintingUsingLibraries();
                    printing.reportZ();
                }
            //}
        }

        private void print_last_check_Click(object sender, EventArgs e)
        {
            //printLastReceiptCopy
            //if (MainStaticClass.PrintingUsingLibraries == 0)
            //{
            //    try
            //    {
            //        Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.execute_operator_type("printLastReceiptCopy");
            //        if (result != null)
            //        {
            //            if (result.results[0].status == "ready")//Задание выполнено успешно 
            //            {
            //                //sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
            //            }
            //            else
            //            {
            //                MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("Общая ошибка");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("print_last_check_Click" + ex.Message);
            //    }
            //}
            //else
            //{
                if (MainStaticClass.GetFiscalsForbidden)
                {
                    MessageBox.Show("Вам запрещена печать на фискаольном регистраторое", "Проверки при печати");
                    return;
                }
                else
                {
                    PrintingUsingLibraries printing = new PrintingUsingLibraries();
                    printing.print_last_document();
                }
            //}
        }


        private void get_summ_in_cashe_Click(object sender, EventArgs e)
        {
            //if (MainStaticClass.PrintingUsingLibraries == 0)
            //{
            //    try
            //    {
            //        Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.execute_operator_type("getCashDrawerStatus");
            //        if (result != null)
            //        {
            //            if (result.results[0].status == "ready")//Задание выполнено успешно 
            //            {
            //                sum_incass.Text = result.results[0].result.counters.cashSum.ToString();
            //            }
            //            else
            //            {
            //                MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("Общая ошибка");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("get_summ_in_cashe_Click" + ex.Message);
            //    }
            //}
            //else
            //{
            if (MainStaticClass.GetFiscalsForbidden)
            {
                MessageBox.Show("Вам запрещена печать на фискаольном регистраторое", "Проверки при печати");
                return;
            }
            else
            {

                PrintingUsingLibraries printing = new PrintingUsingLibraries();
                sum_incass.Text = printing.getCasheSumm();
                printing.get_register_data();
            }
            //}
        }


        private void zero_check_Click(object sender, EventArgs e)
        {

        }

        private void btn_have_internet_Click(object sender, EventArgs e)
        {
            txtB_have_internet.BackColor = Color.White;
            txtB_have_internet.Text = "Проверка ...";
            txtB_have_internet.Update();
            System.Threading.Thread.Sleep(1000);

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

            txtB_ofd_exchange_status.BackColor = Color.White;
            txtB_ofd_exchange_status.Text = "Проверка ...";
            txtB_ofd_exchange_status.Update();
            System.Threading.Thread.Sleep(1000);

            //if (MainStaticClass.PrintingUsingLibraries == 0)
            //{
            //    //txtB_ofd_exchange_status.BackColor = Color.Green;
            //    Cash8.FiscallPrintJason.RootObject result = MainStaticClass.get_ofd_exchange_status();
            //    if (result != null)
            //    {
            //        if (result.results[0].result != null)
            //        {

            //            if (result.results[0].result.status.notSentCount > 0)
            //            {
            //                if ((DateTime.Now - result.results[0].result.status.notSentFirstDocDateTime).Days > 3)
            //                {
            //                    txtB_ofd_exchange_status.Text = "В ОФД Не отправлено документов " + result.results[0].result.status.notSentCount.ToString().Trim() + "\r\n" +
            //                    " начиная с даты " + result.results[0].result.status.notSentFirstDocDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            //                    txtB_ofd_exchange_status.BackColor = Color.Gold;
            //                }
            //                else
            //                {
            //                    txtB_ofd_exchange_status.Text = "В ОФД Не отправлено документов " + result.results[0].result.status.notSentCount.ToString().Trim();
            //                }
            //            }
            //            else
            //            {
            //                txtB_ofd_exchange_status.Text = "Все отправлено";
            //            }
            //        }
            //        else
            //        {
            //            txtB_ofd_exchange_status.Text = "Нет связи";
            //            txtB_ofd_exchange_status.BackColor = Color.Gold;
            //        }
            //    }
            //    else
            //    {
            //        txtB_ofd_exchange_status.Text = "Нет связи";
            //        txtB_ofd_exchange_status.BackColor = Color.Gold;
            //    }
            //}
            //else
            //{
                PrintingUsingLibraries printing = new PrintingUsingLibraries();
                txtB_ofd_exchange_status.Text = printing.ofdExchangeStatus();
            //}
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
                req.Timeout = 120000;
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
            if (MainStaticClass.GetAcquiringBank == 1) //РНКБ
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
                        //if (MainStaticClass.PrintingUsingLibraries == 0)
                        //{
                        //    FiscallPrintJason2.NonFiscallDocument nonFiscallDocument = new FiscallPrintJason2.NonFiscallDocument();
                        //    nonFiscallDocument.type = "nonFiscal";
                        //    nonFiscallDocument.printFooter = false;

                        //    FiscallPrintJason2.ItemNonFiscal itemNonFiscal = new FiscallPrintJason2.ItemNonFiscal();
                        //    nonFiscallDocument.items = new List<FiscallPrintJason2.ItemNonFiscal>();

                        //    itemNonFiscal.type = "text";
                        //    itemNonFiscal.text = recharge_note.Replace("0xDF^^", "");
                        //    itemNonFiscal.alignment = "center";
                        //    nonFiscallDocument.items.Add(itemNonFiscal);
                        //    FiscallPrintJason2.print_not_fiscal_document(nonFiscallDocument);
                        //    recharge_note = "";
                        //}
                        //else
                        //{
                            IFptr fptr = MainStaticClass.FPTR;
                            if (!fptr.isOpened())
                            {
                                fptr.open();
                            }
                            fptr.beginNonfiscalDocument();
                            string s = recharge_note.Replace("0xDF^^", "");
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_CENTER);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                            fptr.printText();
                            fptr.endNonfiscalDocument();
                            recharge_note = "";

                        //}
                    }
                }
                else
                {

                }
            }
            else if (MainStaticClass.GetAcquiringBank == 2) //СБЕР
            {
                try
                {
                    string s = Regex.Replace(CommandWrapper.CloseDay(), @"~S\u0001", "").Trim();
                    IFptr fptr = MainStaticClass.FPTR;
                    if (!fptr.isOpened())
                    {
                        fptr.open();
                    }

                    fptr.beginNonfiscalDocument();

                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_DEFER, AtolConstants.LIBFPTR_DEFER_POST);
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_CENTER);
                    fptr.printText();
                    fptr.endNonfiscalDocument();
                    //if (MainStaticClass.GetVariantConnectFN == 1)
                    //{
                    //    fptr.close();
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибки при сверке итогов " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show(" У вас в константах не выбран банк эквайринга");
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
                    //if (MainStaticClass.PrintingUsingLibraries == 0)
                    //{
                    //    FiscallPrintJason2.NonFiscallDocument nonFiscallDocument = new FiscallPrintJason2.NonFiscallDocument();
                    //    nonFiscallDocument.type = "nonFiscal";
                    //    nonFiscallDocument.printFooter = false;

                    //    FiscallPrintJason2.ItemNonFiscal itemNonFiscal = new FiscallPrintJason2.ItemNonFiscal();
                    //    nonFiscallDocument.items = new List<FiscallPrintJason2.ItemNonFiscal>();

                    //    itemNonFiscal.type = "text";
                    //    itemNonFiscal.text = recharge_note.Replace("0xDF^^", "");
                    //    itemNonFiscal.alignment = "center";
                    //    nonFiscallDocument.items.Add(itemNonFiscal);
                    //    FiscallPrintJason2.print_not_fiscal_document(nonFiscallDocument);
                    //    recharge_note = "";
                    //}
                    //else
                    //{
                        IFptr fptr = MainStaticClass.FPTR;
                        if (!fptr.isOpened())
                        {
                            fptr.open();
                        }
                        fptr.beginNonfiscalDocument();
                        string s = recharge_note.Replace("0xDF^^", "");
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_CENTER);
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                        fptr.printText();
                        fptr.endNonfiscalDocument();
                        recharge_note = "";

                    //}
                }
            }
            else
            {

            }

        }

        private void btn_query_full_report_Click(object sender, EventArgs e)
        {
            if (MainStaticClass.GetAcquiringBank == 1) //РНКБ
            {
                string url = "http://" + MainStaticClass.IpAddressAcquiringTerminal;
                string _str_command_ = @"<?xml version=""1.0"" encoding=""utf-8""?><request><field id=""25"">63</field><field id=""27"">id_terminal</field><field id=""65"">21</field></request>";
                _str_command_ = _str_command_.Replace("id_terminal", MainStaticClass.IdAcquirerTerminal);
                recharge_note = "";
                AnswerTerminal answerTerminal = new AnswerTerminal();
                send_command_acquiring_terminal(url, _str_command_, ref complete, ref answerTerminal);
                if (complete)//ответ от терминала удовлетворительный
                {
                    if (recharge_note != "")
                    {
                        //if (MainStaticClass.PrintingUsingLibraries == 0)
                        //{
                        //    FiscallPrintJason2.NonFiscallDocument nonFiscallDocument = new FiscallPrintJason2.NonFiscallDocument();
                        //    nonFiscallDocument.type = "nonFiscal";
                        //    nonFiscallDocument.printFooter = false;

                        //    FiscallPrintJason2.ItemNonFiscal itemNonFiscal = new FiscallPrintJason2.ItemNonFiscal();
                        //    nonFiscallDocument.items = new List<FiscallPrintJason2.ItemNonFiscal>();

                        //    itemNonFiscal.type = "text";
                        //    itemNonFiscal.text = recharge_note.Replace("0xDF^^", "");
                        //    itemNonFiscal.alignment = "center";
                        //    nonFiscallDocument.items.Add(itemNonFiscal);
                        //    FiscallPrintJason2.print_not_fiscal_document(nonFiscallDocument);
                        //    recharge_note = "";
                        //}
                        //else
                        //{
                            IFptr fptr = MainStaticClass.FPTR;
                            if (!fptr.isOpened())
                            {
                                fptr.open();
                            }
                            fptr.beginNonfiscalDocument();
                            string s = recharge_note.Replace("0xDF^^", "");
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_CENTER);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                            fptr.printText();
                            fptr.endNonfiscalDocument();
                            recharge_note = "";
                        //}
                    }
                }
                else
                {

                }
            }
            else if (MainStaticClass.GetAcquiringBank == 2) //СБЕР
            {
                try
                {
                    string s = Regex.Replace(CommandWrapper.GetFullReport(), @"~S\u0001", "").Trim();
                    IFptr fptr = MainStaticClass.FPTR;
                    if (!fptr.isOpened())
                    {
                        fptr.open();
                    }
                    fptr.beginNonfiscalDocument();
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_DEFER, AtolConstants.LIBFPTR_DEFER_POST);
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_CENTER);
                    fptr.printText();
                    fptr.endNonfiscalDocument();
                    //if (MainStaticClass.GetVariantConnectFN == 1)
                    //{
                    //    fptr.close();
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибки при полном отчете  " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show(" У вас в константах не выбран банк эквайринга");
            }
        }

        private void btn_openDrawer_Click(object sender, EventArgs e)
        {
            if (MainStaticClass.PrintingUsingLibraries == 1)
            {
                try
                {
                    IFptr fptr = MainStaticClass.FPTR;
                    if (!fptr.isOpened())
                    {
                        fptr.open();
                    }
                    fptr.openDrawer();
                    //if (MainStaticClass.GetVariantConnectFN == 1)
                    //{
                    //    fptr.close();
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибки при попытке открыть денежный ящик  " + ex.Message);
                }
            }
        }

        private void btn_date_mark_Click(object sender, EventArgs e)
        {

            txtB_last_send_mark.BackColor = Color.White;
            txtB_last_send_mark.Text = "Проверка ...";
            txtB_last_send_mark.Update();
            System.Threading.Thread.Sleep(1000);

            if (MainStaticClass.PrintingUsingLibraries == 1)
            {
                try
                {
                    IFptr fptr = MainStaticClass.FPTR;
                    if (!fptr.isOpened())
                    {
                        fptr.open();
                    }

                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_DATA_TYPE, AtolConstants.LIBFPTR_DT_LAST_SENT_ISM_NOTICE_DATE_TIME);
                    fptr.queryData();

                    DateTime dateTime = fptr.getParamDateTime(AtolConstants.LIBFPTR_PARAM_DATE_TIME);
                    txtB_last_send_mark.Text = dateTime.ToString("dd-MM-yyyy HH:mm:ss");
                    //if (MainStaticClass.GetVariantConnectFN == 1)
                    //{
                    //    fptr.close();
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибки при получении даты о последней успешной отправке маркировки в ИСМ  " + ex.Message);
                }
            }
            else
            {
                btn_date_mark.Visible = false;
                txtB_last_send_mark.Visible = false;
            }
        }

        private void btn_open_shop_Click(object sender, EventArgs e)
        {
            using (var conn = MainStaticClass.NpgsqlConn())
            {
                try
                {
                    conn.Open();

                    // Проверяем, открыт ли магазин сегодня
                    string checkQuery = "SELECT COUNT(1) FROM open_close_shop WHERE date = @date";
                    using (var command = new NpgsqlCommand(checkQuery, conn))
                    {
                        command.Parameters.AddWithValue("@date", DateTime.Now.Date);

                        if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                        {
                            // Открываем магазин
                            string insertQuery = "INSERT INTO open_close_shop(open, date,its_sent) VALUES(@open, @date,@its_sent)";
                            using (var insertCommand = new NpgsqlCommand(insertQuery, conn))
                            {
                                insertCommand.Parameters.AddWithValue("@open", DateTime.Now);
                                insertCommand.Parameters.AddWithValue("@date", DateTime.Now.Date);
                                insertCommand.Parameters.AddWithValue("@its_sent", false);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Сегодня магазин уже был открыт ранее","Открытие магазина");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при открытии магазина");
                    MainStaticClass.WriteRecordErrorLog(ex, 0, MainStaticClass.CashDeskNumber, "Открытие магазина");
                }
            }
            load_status_open_shop();
        }

        private void load_status_open_shop()
        {
            using (var conn = MainStaticClass.NpgsqlConn())
            {
                try
                {
                    conn.Open();

                    // Проверяем, открыт ли магазин сегодня
                    string checkQuery = "SELECT open FROM open_close_shop WHERE date = @date";
                    using (var command = new NpgsqlCommand(checkQuery, conn))
                    {
                        command.Parameters.AddWithValue("@date", DateTime.Now.Date);
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            string status_open = "Не открыт";
                            while (reader.Read())
                            {
                                var openValue = reader["open"];
                                if (openValue != DBNull.Value)
                                {
                                    status_open = Convert.ToDateTime(openValue).ToString("dd:MM:yyyy HH:mm:ss");
                                }
                            }
                            label_open_shop.Text = status_open;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при чтении статуса открытия магазина");
                    MainStaticClass.WriteRecordErrorLog(ex, 0, MainStaticClass.CashDeskNumber, "Чтение статуса открытия магазина");
                }
            }
        }

        private void btn_close_shop_Click(object sender, EventArgs e)
        {
            using (var conn = MainStaticClass.NpgsqlConn())
            {
                try
                {
                    conn.Open();

                    // Проверяем, открыт ли магазин сегодня
                    string checkQuery = "SELECT COUNT(1) FROM open_close_shop WHERE date = @date";
                    using (var command = new NpgsqlCommand(checkQuery, conn))
                    {
                        command.Parameters.AddWithValue("@date", DateTime.Now.Date);

                        if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                        {
                            // Закрываем магазин
                            string insertQuery = "UPDATE open_close_shop  	SET close=@close,its_sent = @its_sent  WHERE date = @date";
                            using (var insertCommand = new NpgsqlCommand(insertQuery, conn))
                            {
                                insertCommand.Parameters.AddWithValue("@close", DateTime.Now);
                                insertCommand.Parameters.AddWithValue("@date", DateTime.Now.Date);
                                insertCommand.Parameters.AddWithValue("@its_sent", false);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Сегодня магазин еще не был открыт");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при закрытии магазина");
                    MainStaticClass.WriteRecordErrorLog(ex, 0, MainStaticClass.CashDeskNumber, "Закрытие магазина");
                }
            }
            load_status_close_shop();
        }

        private void load_status_close_shop()
        {
            using (var conn = MainStaticClass.NpgsqlConn())
            {
                try
                {
                    conn.Open();

                    // Проверяем, открыт ли магазин сегодня
                    string checkQuery = "SELECT close FROM open_close_shop WHERE date = @date";
                    using (var command = new NpgsqlCommand(checkQuery, conn))
                    {
                        command.Parameters.AddWithValue("@date", DateTime.Now.Date);
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            string status_close = "Не закрыт";
                            while (reader.Read())
                            {
                                var closeValue = reader["close"];
                                if (closeValue != DBNull.Value)
                                {
                                    status_close = Convert.ToDateTime(closeValue).ToString("dd:MM:yyyy HH:mm:ss");
                                }
                            }
                            label_close_shop.Text = status_close;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при чтении статуса закрытия магазина");
                    MainStaticClass.WriteRecordErrorLog(ex, 0, MainStaticClass.CashDeskNumber, "Чтение статуса закрытия магазина");
                }
            }
        }

    }
}
