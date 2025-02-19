using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Atol.Drivers10.Fptr;
using AtolConstants = Atol.Drivers10.Fptr.Constants;
using Npgsql;

namespace Cash8
{
    public partial class Pay : Form
    {
        private int curpos = 0;
        private bool firs_input = true;
        private int curpos_non_cash = 0;
        //private int curpos_pay_bonus = 0;
        //private bool firs_input_pay_bonus = true;

        private bool firs_input_non_cash = true;
        public ListView listView_sertificates = null;
        public bool code_it_is_confirmed = false;//При списании бонусов, присланный код полтвержден клиентом  
        private bool complete = false;
        //private string reference_number = "";
        private string str_command_sale     = @"<?xml version=""1.0"" encoding=""UTF-8""?><request><field id = ""00"">sum</field><field id=""04"">643</field><field id = ""25"" >1</field><field id=""27"">id_terminal</field></request>";
        string str_return_sale      = @"<?xml version=""1.0"" encoding=""UTF-8""?><request><field id = ""00"">sum</field><field id=""04"">643</field><field id=""13"">sale_code_authorization_terminal</field><field id=""14"">number_reference</field><field id = ""25"" >29</field><field id=""27"">id_terminal</field></request>";
        string str_cancel_sale      = @"<?xml version=""1.0"" encoding=""UTF-8""?><request><field id = ""00"">sum</field><field id=""01"">sale_non_cash_money</field><field id=""04"">643</field><field id = ""25"">4</field><field id=""27"">id_terminal</field><field id=""13"">sale_code_authorization_terminal</field><field id=""14"">number_reference</field></request>";
        //string str_command_cancel_sale    = @"<?xml version=""1.0"" encoding=""UTF-8""?><request><field id = ""00"" >sum</field><field id=""04"">643</field><field id=""14"">number_reference</field><field id = ""25"" >4</field><field id=""27"">id_terminal</field></request>";
        string str_sale_sbp         = @"<?xml version=""1.0"" encoding=""UTF-8""?><request><field id = ""00"">sum</field><field id=""04"">643</field><field id=""14"">guid</field><field id = ""25"" >1</field><field id=""27"">id_terminal</field><field id=""53"">115</field></request>";
        string str_payment_status_sale_sbp  = @"<?xml version=""1.0"" encoding=""UTF-8""?><request><field id = ""00"">sum</field><field id=""04"">643</field><field id=""13"">sale_code_authorization_terminal</field><field id = ""25"" >1</field><field id=""27"">id_terminal</field><field id=""53"">117</field></request>";
        string str_return_sale_sbp  = @"<?xml version=""1.0"" encoding=""UTF-8""?><request><field id = ""00"">sum</field><field id=""04"">643</field><field id=""13"">sale_code_authorization_terminal</field><field id=""14"">guid</field><field id = ""25"" >29</field><field id=""27"">id_terminal</field><field id=""53"">118</field></request>";
        string str_payment_status_return_sale_sbp = @"<?xml version=""1.0"" encoding=""UTF-8""?><request><field id = ""00"">sum</field><field id=""04"">643</field><field id=""13"">sale_code_authorization_terminal</field><field id=""14"">guid</field><field id = ""25"" >29</field><field id=""27"">id_terminal</field><field id=""53"">119</field></request>";
        public Cash_check cc = null;
        private ToolTip toolTip = new ToolTip();
          

        
        public Pay()
        {
            InitializeComponent();
            this.txtB_cash_sum.SelectionStart = 0;
            this.non_cash_sum.SelectionStart = 0;
            this.pay_bonus.SelectionStart = 0;
                             
            this.KeyPreview = true;
            this.Load += new EventHandler(Pay_Load);
            this.txtB_cash_sum.KeyPress += new KeyPressEventHandler(txtB_cash_sum_KeyPress);            
            this.txtB_cash_sum.KeyUp += new KeyEventHandler(txtB_cash_sum_KeyUp);
            this.non_cash_sum.KeyPress += new KeyPressEventHandler(non_cash_KeyPress);
            this.non_cash_sum.KeyUp += new KeyEventHandler(non_cash_KeyUp);
            this.pay_bonus.KeyPress += new KeyPressEventHandler(pay_bonus_KeyPress);                      
            this.pay_bonus.KeyUp += new KeyEventHandler(pay_bonus_KeyUp);
            this.non_cash_sum_kop.KeyPress += new KeyPressEventHandler(non_cash_sum_kop_KeyPress);
            this.non_cash_sum_kop.KeyUp += new KeyEventHandler(non_cash_sum_kop_KeyUp);

            this.button_pay.Enabled = false;
            listView_sertificates = new ListView();
            if (MainStaticClass.NumberDecimalSeparator() == ".")
            {
                this.pay_bonus_many.Text = "0.00";
            }
            else
            {
                this.pay_bonus_many.Text = "0,00";
            }
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

            toolTip.ToolTipTitle = " Если оплата по терминалу для этого чека уже прошла ";
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.IsBalloon = true; // Для отображения подсказки в виде "баллона"

            toolTip.SetToolTip(this.checkBox_do_not_send_payment_to_the_terminal, "Не отправлять запрос об оплате на терминал");

            //toolTip.ToolTipTitle = "Если нажать на клавиатуре кнопку r ";
            //toolTip.ToolTipIcon = ToolTipIcon.Info;
            //toolTip.IsBalloon = true; // Для отображения подсказки в виде "баллона"

            //toolTip.SetToolTip(this.non_cash_sum, "То полностью заполнится поле карты оплаты суммой документа в том числе с копейками");

        }

        private void non_cash_sum_kop_KeyUp(object sender, KeyEventArgs e)
        {
            calculate();
        }

        void non_cash_sum_kop_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema ==3) || (MainStaticClass.GetWorkSchema == 4))
            //{
            //    e.Handled = true;
            //}
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Delete)
            {
                e.Handled = true;
            }

        

        }

        //public void set_kop_on_non_cash_sum_kop(string kop)
        //{
        //    if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema ==3) || (MainStaticClass.GetWorkSchema == 4))
        //    {
        //        non_cash_sum_kop.Text = kop;
        //    }
        //}

        private void non_cash_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.non_cash_sum.Text.Length == 0)
                {
                    if (numberDecimalSeparator() == ".")
                    {
                        if (MainStaticClass.get_currency() == "руб.")
                        {
                            this.non_cash_sum.Text = "0";
                        }
                        //else
                        //{
                        //    this.non_cash_sum.Text = "0.00";
                        //}
                    }
                    else
                    {
                        if (MainStaticClass.get_currency() == "руб.")
                        {
                            this.non_cash_sum.Text = "0";
                        }
                        //else
                        //{
                        //    this.non_cash_sum.Text = "0,00";
                        //}
                    }
                }
                calculate();
                //if (MainStaticClass.get_currency() == "руб.")
                //{

                //}
                //else
                //{
                //    if (curpos_non_cash < 0)
                //    {
                //        non_cash_sum.SelectionStart = 0;
                //        curpos_non_cash = 0;
                //    }
                //    else
                //    {
                //        non_cash_sum.SelectionStart = curpos_non_cash;
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("non_cash_KeyUp "+ex.Message);
            }
            int start = non_cash_sum.SelectionStart;
            string _sum_ = non_cash_sum.Text;
            non_cash_sum.Text = "";
            non_cash_sum.Text = _sum_;
            non_cash_sum.SelectionStart = start;
            non_cash_sum.Update();
            if (MainStaticClass.get_currency() == "руб.")
            {

            }
            else
            {
                non_cash_sum.SelectionStart = curpos_non_cash;
            }
        }

        private bool copFilledCorrectly()
        {
            bool result = true;
            if (non_cash_sum.Text.Trim().Length > 0)
            {
                if (Convert.ToInt32(non_cash_sum.Text) == 0)
                {
                    if (Convert.ToInt16(non_cash_sum_kop.Text) > 0)
                    {
                        DialogResult dialogResult = MessageBox.Show("У вас заполнены копейки для оплаты по карте,но не заполнена целая часть суммы оплты " +
                            " по карте, если вы выберете ДА тогда копейки будут оплачены по карте, если вы выберете НЕТ то тогда окпейки обнулятся и вам" +
                            " будет необходимо снова выбрать сумму и форму оплты", "Проверки при оплате картой", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.No)
                        {
                            non_cash_sum_kop.Text = "0";
                            result = false;
                        }
                    }

                }
                else
                {
                    //double total = get_sum_sum_at_a_discount();
                    ////if (Convert.ToInt16(non_cash_sum_kop.Text) != Math.Round(total - (int)total, 2) * 100)
                    ////{
                    ////    MessageBox.Show("У вас не заполнены копейки, нажмите клавишу к(r) и затем продолжите ввод целой части по карте оплаты.", "Проверки при оплате картой");
                    ////    result = false;
                    ////}
                    //if (Convert.ToInt32(non_cash_sum_kop.Text) != Math.Round(total - (int)total, 2, MidpointRounding.AwayFromZero) * 100)
                    //{
                    //    MessageBox.Show("У вас не заполнены копейки, нажмите клавишу к(r) и затем продолжите ввод целой части по карте оплаты.", "Проверки при оплате картой");
                    //    result = false;
                    //}
                }

            }
            else
            {
                MessageBox.Show("У вас пустое поле оплата по карте сделайте фото и создайте заявку в ит отдел.", "Проверки при оплате картой");
            }
            return result;
        }

        private void non_cash_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (Convert.ToInt16(non_cash_sum_kop.Text) == 0)
            {
                double total = get_sum_sum_at_a_discount();
                if (total != (int)total)
                {
                    e.Handled = true;
                    MessageBox.Show("У вас не заполнены копейки, нажмите клавишу к(r) и затем продолжите ввод целой части по карте оплаты.", "Проверки при оплате картой");
                    return;
                }
            }
            


            if ((Char.IsDigit(e.KeyChar)))
            {
                if (firs_input_non_cash)
                {
                    firs_input_non_cash = false;
                    non_cash_sum.Text = e.KeyChar + non_cash_sum.Text.Substring(1, non_cash_sum.Text.Length - 1);
                    e.Handled = true;
                    non_cash_sum.SelectionStart = 1;
                    curpos_non_cash = 1;
                }
            }

            if (MainStaticClass.get_currency() == "руб.")//зарпет на ввод запятой и точек можно только цифры или бекспейс
            {
                if (!(Char.IsDigit(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != 37))
                {
                    e.Handled = true;
                }
                //return;
            }

            if (!(Char.IsDigit(e.KeyChar)))
            {
                if ((e.KeyChar == '.') || (e.KeyChar == ','))
                {

                    non_cash_sum.SelectionStart = non_cash_sum.Text.IndexOf(MainStaticClass.NumberDecimalSeparator()) + 1;
                    curpos_non_cash = non_cash_sum.SelectionStart; e.Handled = true;
                }

                if (e.KeyChar != (char)Keys.Back)
                {

                    e.Handled = true;
                }
            }

            if (e.Handled == false)
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    curpos_non_cash = non_cash_sum.SelectionStart;
                    curpos_non_cash++;
                }
                else
                {
                    if (non_cash_sum.SelectionStart != 0)
                    {
                        if (non_cash_sum.Text.Substring(non_cash_sum.SelectionStart - 1, 1) == ".")
                        {
                            e.Handled = true;
                            non_cash_sum.SelectionStart -= 1;
                            curpos_non_cash = non_cash_sum.SelectionStart;
                        }
                        else if ((non_cash_sum.SelectionStart == 2) && (!e.Handled))
                        {
                            curpos_non_cash = 1;
                        }
                        else
                        {
                            curpos_non_cash = non_cash_sum.SelectionStart - 1;
                        }
                    }

                }
            }

            this.non_cash_sum.Update();            
        }

        /// <summary>
        /// При оплате бонусами необходимо контролировать 
        /// невозможность оплаты более 70 % накопленных бонусов
        /// и невозможности опалтить более 70 % чека
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pay_bonus_KeyUp(object sender, KeyEventArgs e)
        {
            //if (this.pay_bonus.Text.Trim().Length == 0)
            //{
            //    //if (numberDecimalSeparator() == ".")
            //    //{
            //    //    if (MainStaticClass.get_currency() == "руб.")//зарпет на ввод запятой и точек можно только цифры или бекспейс
            //    //    {
            //    //        this.pay_bonus.Text = "0.00";
            //    //    }
            //    //    else
            //    //    {
            //            this.pay_bonus.Text = "0";
            //        //}
            //    //}
            //    //else
            //    //{
            //    //    if (MainStaticClass.get_currency() == "руб.")//зарпет на ввод запятой и точек можно только цифры или бекспейс
            //    //    {
            //    //        this.pay_bonus.Text = "0,00";
            //    //    }
            //    //    else
            //    //    {
            //    //        this.pay_bonus.Text = "0";
            //    //    }
            //    //}
            //}


            //if (MainStaticClass.get_currency() != "руб.")
            //{
            //    if (pay_bonus.Text.Trim().Length > 0)
            //    {
            //        Int64 input_bonus = Convert.ToInt64(pay_bonus.Text);
            //        Int64 bonus_total = Convert.ToInt64(Convert.ToDecimal(bonus_total_in_centr.Text) * (decimal)0.7);
            //        if (input_bonus > bonus_total)
            //        {
            //            this.pay_bonus.Text = bonus_total.ToString();
            //        }
            //        else
            //        {
            //            pay_bonus.Text = Convert.ToInt32(pay_bonus.Text).ToString();
            //        }

            //        if (Convert.ToDecimal(pay_bonus.Text) > (Convert.ToDecimal(pay_sum.Text) / 100 * 70 * 100))
            //        {
            //            pay_bonus.Text = (Math.Round(Convert.ToDecimal(pay_sum.Text) / 100 * 70 * 100, 0, MidpointRounding.ToEven)).ToString();
            //        }

            //        pay_bonus_many.Text = String.Format("{0:0.00}", Convert.ToDecimal(pay_bonus.Text) / 100);
            //    }
            //    else
            //    {
            //        pay_bonus_many.Text = String.Format("{0:0.00}", 0);
            //    }
            //}
            //else
            //{
            //*****************************************************************************
            try
            {
                //if (this.pay_bonus.Text.Trim().Length == 0)
                //{
                //    //if (numberDecimalSeparator() == ".")
                //    //{
                //    //    if (MainStaticClass.get_currency() == "руб.")
                //    //    {
                //    this.pay_bonus.Text = "0";
                //    //    }                            
                //    //}
                //    //else
                //    //{
                //    //    if (MainStaticClass.get_currency() == "руб.")
                //    //    {
                //    //        this.cash_sum.Text = "0";
                //    //    }                            
                //    //}
                //}
                calculate();
                //if (MainStaticClass.get_currency() == "руб.")
                //{
                //    if (curpos_pay_bonus < 0)
                //    {
                //        pay_bonus.SelectionStart = 0;
                //        curpos_pay_bonus = 0;
                //    }
                //    else
                //    {
                //        pay_bonus.SelectionStart = curpos_pay_bonus;
                //    }
                //}                   
            }
            catch (Exception ex)
            {
                MessageBox.Show(" pay_bonus_KeyUp "+ex.Message);
            }
            int start = pay_bonus.SelectionStart;
            string _sum_ = pay_bonus.Text;
            pay_bonus.Text = "";
            pay_bonus.Text = _sum_;
            pay_bonus.SelectionStart = start;
            pay_bonus.Update();
            //if (MainStaticClass.get_currency() == "руб.")
            //{
            //pay_bonus.SelectionStart = curpos_pay_bonus;
            //}                
            //*****************************************************************************

            if (pay_bonus.Text.Trim().Length > 0)
            {
                //Decimal input_bonus = Convert.ToDecimal(pay_bonus.Text);
                Decimal input_bonus = 0;
                if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema ==3) || (MainStaticClass.GetWorkSchema == 4))
                {
                    input_bonus = Convert.ToDecimal(pay_bonus.Text);
                }
                else if (MainStaticClass.GetWorkSchema == 2)
                {
                    //input_bonus = Convert.ToDecimal(pay_bonus.Text)*100;
                    input_bonus = Convert.ToDecimal(pay_bonus.Text);
                }

                //Посчитать сумму документа - 10 рублей
                //Decimal bonus_total = Convert.ToDecimal(bonus_total_in_centr.Text);

                Int64 bonus_total = 0;
                if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema ==3) || (MainStaticClass.GetWorkSchema == 4))
                {
                    bonus_total = Convert.ToInt64(bonus_total_in_centr.Text);
                }
                else if (MainStaticClass.GetWorkSchema == 2)
                {
                    //bonus_total = Convert.ToInt64(bonus_total_in_centr.Text)*100;
                    bonus_total = Convert.ToInt64(bonus_total_in_centr.Text);
                }

                if (input_bonus > bonus_total)
                {
                    //this.pay_bonus.Text = String.Format("{0:0.00}",bonus_total.ToString());
                    this.pay_bonus.Text = bonus_total.ToString();
                }
                //else
                //{
                //    //pay_bonus.Text = String.Format("{0:0.00}", Math.Round(Convert.ToDecimal(pay_bonus.Text),2));
                //    pay_bonus.Text = Convert.ToInt64(pay_bonus.Text).ToString();
                //}

                //if (Convert.ToDecimal(pay_bonus.Text) > (Convert.ToDecimal(pay_sum.Text)-1))
                if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema ==3) || (MainStaticClass.GetWorkSchema == 4))
                {
                    int const_remaind = (int)(cc.calculation_quantity_on_document() / 100) + 1;
                    if (Convert.ToInt64(pay_bonus.Text) > (int)(Convert.ToDecimal(pay_sum.Text) - const_remaind))
                    {
                        pay_bonus.Text = ((int)(Convert.ToDecimal(pay_sum.Text) - const_remaind)).ToString();
                    }
                    pay_bonus_many.Text = pay_bonus.Text;
                }
                else if (MainStaticClass.GetWorkSchema == 2)
                {
                    int const_remaind = cc.calculation_quantity_on_document();
                    if (Convert.ToInt64(pay_bonus.Text)/100 > (Convert.ToDecimal(pay_sum.Text) - const_remaind))
                    {
                        pay_bonus.Text = ((int)(Convert.ToDecimal(pay_sum.Text) - const_remaind)*100).ToString();
                    }
                    pay_bonus_many.Text = (Convert.ToDecimal(pay_bonus.Text)/100).ToString();
                }

                


                //if ((Convert.ToInt32(pay_bonus.Text)) >= Convert.ToDecimal(pay_sum.Text)+ ((int)(cc.calculation_quantity_on_document() / 100)+1))
                //{
                //    int const_remaind = (int)(cc.calculation_quantity_on_document() / 100) + 1;
                //    pay_bonus_many.Text = (Convert.ToInt64(pay_bonus.Text) - const_remaind).ToString();
                //}
                //else
                //{
                //    pay_bonus_many.Text = pay_bonus.Text;
                //}
                //pay_bonus.Text = pay_bonus_many.Text;
            }
            else
            {
                //pay_bonus_many.Text = String.Format("{0:0.00}", 0);
                pay_bonus_many.Text = "0";// String.Format("{0:0.00}", 0);
            }

            //}
            calculate();
        }


        private void pay_bonus_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (MainStaticClass.get_currency() != "руб.")
            //{
            if ((!Char.IsDigit(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }
            //}
            /*else
            {
                if ((Char.IsDigit(e.KeyChar)))
                {
                    if (firs_input_pay_bonus)
                    {
                        firs_input_pay_bonus = false;
                        pay_bonus.Text = e.KeyChar + pay_bonus.Text.Substring(1, pay_bonus.Text.Length - 1);
                        e.Handled = true;
                        pay_bonus.SelectionStart = 1;
                        curpos_pay_bonus = 1;
                    }
                }               
                if (!(Char.IsDigit(e.KeyChar)))
                {
                    if ((e.KeyChar == '.') || (e.KeyChar == ','))
                    {

                        pay_bonus.SelectionStart = pay_bonus.Text.IndexOf(MainStaticClass.NumberDecimalSeparator()) + 1;
                        curpos_pay_bonus = pay_bonus.SelectionStart; 
                        e.Handled = true;
                    }

                    if (e.KeyChar != (char)Keys.Back)
                    {

                        e.Handled = true;
                    }
                }

                if (e.Handled == false)
                {
                    if (e.KeyChar != (char)Keys.Back)
                    {
                        curpos_pay_bonus = pay_bonus.SelectionStart;
                        curpos_pay_bonus++;
                    }
                    else
                    {
                        if (pay_bonus.SelectionStart != 0)
                        {
                            if (pay_bonus.Text.Substring(pay_bonus.SelectionStart - 1, 1) == ".")
                            {
                                e.Handled = true;
                                pay_bonus.SelectionStart -= 1;
                                curpos_pay_bonus = pay_bonus.SelectionStart;
                            }
                            else if ((pay_bonus.SelectionStart == 2) && (!e.Handled))
                            {
                                curpos_pay_bonus = 1;
                            }
                            else
                            {
                                curpos_pay_bonus = pay_bonus.SelectionStart - 1;
                            }
                        }
                    }
                }

                this.pay_bonus.Update();            
            }*/
        }
        
        //private Decimal get_bonus_on_client()
        //{
        //    decimal result = -1;

        //    if (!MainStaticClass.service_is_worker())
        //    {
        //        return result;
        //    }

        //    Cash8.DS.DS ds = MainStaticClass.get_ds();
        //    ds.Timeout = 2000;

        //    //Получить параметра для запроса на сервер 
        //    string nick_shop = MainStaticClass.Nick_Shop.Trim();
        //    if (nick_shop.Trim().Length == 0)
        //    {
        //        MessageBox.Show(" Не удалось получить название магазина ");
        //        return result;
        //    }

        //    string code_shop = MainStaticClass.Code_Shop.Trim();
        //    if (code_shop.Trim().Length == 0)
        //    {
        //        MessageBox.Show(" Не удалось получить код магазина ");
        //        return result;
        //    }

        //    string count_day = CryptorEngine.get_count_day();
        //    string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
        //    string data = nick_shop + "|" + cc.client.Tag.ToString().Trim() + "|" + code_shop.Trim();
        //    data = CryptorEngine.Encrypt(data, true, key);

        //    try
        //    {
        //        string result_query = ds.GetBonusOnClient_NEW(nick_shop, data);
        //        if (result_query != "-1")
        //        {
        //            string[] answer = CryptorEngine.Decrypt(result_query, true, key).Split('*');
        //            if ((answer[0].Trim() == nick_shop.Trim()) && (answer[2].Trim() == code_shop.Trim()))
        //            {
        //                result = Convert.ToDecimal(answer[1]);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        result = -1;
        //    }

        //    return result; 
        //}
        
        private string numberDecimalSeparator()
        {
            return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToString();
        }

        /// <summary>
        /// Если вариант 0 тогда считаем просто сумму в независимости от того есть ли целая часть 
        /// иначе считаем сумму только в том случае если есть целая часть иначе ноль
        /// </summary>
        /// <param name="variant"></param>
        /// <returns></returns>
        //private double get_non_cash_sum(int variant)
        //{
        //    double result = 0;

        //    if (variant == 0)
        //    {
        //        if (!MainStaticClass.fractional_exists(cc.listView1))
        //        {
        //            result += double.Parse(non_cash_sum.Text) + double.Parse(non_cash_sum_kop.Text) / 100;
        //        }
        //        else
        //        {
        //            if (double.Parse(non_cash_sum.Text) == 0)
        //            {
        //                result += 0;
        //            }
        //            else
        //            {
        //                result += double.Parse(non_cash_sum.Text) + double.Parse(non_cash_sum_kop.Text) / 100;
        //            }
        //        }                
        //    }
        //    else
        //    {
        //        if (double.Parse(non_cash_sum.Text) != 0)
        //        {
        //            result += double.Parse(non_cash_sum.Text) + double.Parse(non_cash_sum_kop.Text) / 100;
        //        }
        //    }

        //    return result;
        //}

        private double get_non_cash_sum()
        {
            double result = 0;

            result += double.Parse(non_cash_sum.Text) + double.Parse(non_cash_sum_kop.Text.Trim().Length==0 ? "0" : non_cash_sum_kop.Text) / 100;           

            return result;
        }

        //private void calculate()
        //{
        //    //MessageBox.Show(this.cash_sum.SelectionStart.ToString(), "Старт 1");
        //    //int selection_start = this.cash_sum.SelectionStart;

        //    try
        //    {
        //        //if (MainStaticClass.SelfServiceKiosk == 0)
        //        //{
        //        if (this.cash_sum.Text.Length == 0)
        //        {
        //            if (numberDecimalSeparator() == ".")
        //            {
        //                this.cash_sum.Text = "0";
        //                this.non_cash_sum.Text = "0";
        //                this.sertificates_sum.Text = "0";

        //            }
        //            else
        //            {
        //                //if (MainStaticClass.SelfServiceKiosk == 0)
        //                //{                            
        //                    this.non_cash_sum.Text = "0";                            
        //                //}
        //                this.sertificates_sum.Text = "0";
        //                this.cash_sum.Text = "0";
        //            }
        //        }
        //        //}

        //        this.cash_sum.Text = Convert.ToDouble(this.cash_sum.Text).ToString("F", System.Globalization.CultureInfo.CurrentCulture);
        //        //if (Convert.ToDecimal(pay_bonus_many.Text) == 0)//Если нет бонусов то проверить 
        //        //{
        //        //    decimal total = Convert.ToDecimal(pay_sum.Text);
        //        //    string kop = ((int)((total - (int)total) * 100)).ToString();
        //        //    kop = (kop.Length == 2 ? kop : "0" + kop);
        //        //    set_kop_on_non_cash_sum_kop(kop);
        //        //}

        //        if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema ==3) || (MainStaticClass.GetWorkSchema == 4))
        //        {

        //            this.remainder.Text = Math.Round(
        //                (double.Parse(cash_sum.Text) +
        //                double.Parse(pay_bonus_many.Text) +
        //                get_non_cash_sum() +
        //                double.Parse(sertificates_sum.Text) - double.Parse(pay_sum.Text)), 2).ToString("F", System.Globalization.CultureInfo.CurrentCulture);
        //        }
        //        //else if (MainStaticClass.GetWorkSchema == 2)
        //        //{
        //        //    this.remainder.Text = (double.Parse(cash_sum.Text) +
        //        //        double.Parse(pay_bonus_many.Text) +
        //        //        get_non_cash_sum(0) +
        //        //        double.Parse(sertificates_sum.Text) - double.Parse(pay_sum.Text)).ToString("F", System.Globalization.CultureInfo.CurrentCulture);
        //        //}

        //        if (Math.Round(double.Parse(cash_sum.Text.Replace(".", ",")) + double.Parse(non_cash_sum.Text) +
        //            double.Parse(sertificates_sum.Text) + double.Parse(pay_bonus_many.Text) + Convert.ToDouble(double.Parse(non_cash_sum_kop.Text) / 100),2,MidpointRounding.ToEven) - double.Parse(pay_sum.Text.Replace(".", ",")) < 0)
        //        {
        //            this.button_pay.Enabled = false;
        //        }
        //        else
        //        {
        //            //Здесь еще должна быть проверка на оплату бонусами если клиент обозначен по номеру телефона
        //            if (Convert.ToDecimal(pay_bonus_many.Text) > 0)
        //            {
        //                if (cc.client.Tag.ToString().Length == 11)
        //                {
        //                    if (code_it_is_confirmed)
        //                    {
        //                        this.button_pay.Enabled = true;
        //                    }
        //                }
        //                else
        //                {
        //                    this.button_pay.Enabled = true;
        //                }
        //            }
        //            else
        //            {
        //                this.button_pay.Enabled = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(" calculate "+ex.Message);
        //    }

        //    //int start = cash_sum.SelectionStart;
        //    //string _sum_ = cash_sum.Text;
        //    //cash_sum.Text = "";
        //    //cash_sum.Text = _sum_;
        //    //cash_sum.SelectionStart = start;
        //    cash_sum.Update();
        //    //MessageBox.Show(this.cash_sum.SelectionStart.ToString(), "Старт 2");
        //    //if (this.cash_sum.Text.Trim().Length-1 == selection_start)
        //    //{
        //    //    //this.cash_sum.SelectionStart += selection_start;
        //    //    MessageBox.Show(this.cash_sum.Text.Trim().Length.ToString() + " " + selection_start.ToString());
        //    //}
        //    //this.cash_sum.SelectedText = "";
        //}

        private void calculate()
        {
            try
            {
                if (this.txtB_cash_sum.Text.Length == 0)
                {
                    this.txtB_cash_sum.Text = "0";
                }

                this.txtB_cash_sum.Text = Convert.ToDouble(this.txtB_cash_sum.Text).ToString("F2", System.Globalization.CultureInfo.CurrentCulture);

                this.remainder.Text = Math.Round(
                (double.Parse(txtB_cash_sum.Text) +
                double.Parse(pay_bonus_many.Text) +
                get_non_cash_sum() +
                double.Parse(sertificates_sum.Text) - double.Parse(pay_sum.Text)), 2).ToString("F", System.Globalization.CultureInfo.CurrentCulture);

                if (Math.Round(double.Parse(txtB_cash_sum.Text.Replace(".", ",")) + double.Parse(non_cash_sum.Text) +
                double.Parse(sertificates_sum.Text) + double.Parse(pay_bonus_many.Text) + 
                Convert.ToDouble(double.Parse(non_cash_sum_kop.Text.Trim().Length==0 ? "0" : non_cash_sum_kop.Text) / 100), 2, MidpointRounding.ToEven) - double.Parse(pay_sum.Text.Replace(".", ",")) < 0)
                {
                    this.button_pay.Enabled = false;
                }
                else
                {
                    this.button_pay.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("calculate " + ex.Message);
            }

            txtB_cash_sum.Update();
        }

        //private void cash_sum_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (this.txtB_cash_sum.Text.Length == 0)
        //        {
        //            this.txtB_cash_sum.Text = "0" + MainStaticClass.NumberDecimalSeparator() + "00";
        //        }

        //        calculate();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("cash_sum_KeyUp " + ex.Message);
        //    }
        //}

        private void txtB_cash_sum_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.txtB_cash_sum.Text.Length == 0)
                {
                    //this.txtB_cash_sum.Text = "0" + MainStaticClass.NumberDecimalSeparator() + "00";
                    this.txtB_cash_sum.Text = "0";
                }

                calculate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("cash_sum_KeyUp " + ex.Message);
            }
        }


        //private void cash_sum_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (this.cash_sum.Text.Length == 0)
        //        {
        //            if (numberDecimalSeparator() == ".")
        //            {
        //                //if (MainStaticClass.get_currency() == "руб.")//зарпет на ввод запятой и точек можно только цифры или бекспейс
        //                //{
        //                //    this.cash_sum.Text = "0";
        //                //}
        //                //else
        //                //{
        //                    this.cash_sum.Text = "0.00";
        //                //}
        //            }
        //            else
        //            {
        //                //if (MainStaticClass.get_currency() == "руб.")//зарпет на ввод запятой и точек можно только цифры или бекспейс
        //                //{
        //                //    this.cash_sum.Text = "0";
        //                //}
        //                //else
        //                //{
        //                    this.cash_sum.Text = "0,00";
        //                //}                       
        //            }
        //        }

        //        //int selection_start = this.cash_sum.SelectionStart;
        //        calculate();
        //        //this.cash_sum.SelectionStart = selection_start;

        //        //if (MainStaticClass.get_currency() == "руб.")
        //        //{

        //        //}
        //        //else
        //        //{
        //        //    if (curpos < 0)
        //        //    {
        //        //        cash_sum.SelectionStart = 0;
        //        //        curpos = 0;
        //        //        //MessageBox.Show("0");
        //        //    }
        //        //    else
        //        //    {
        //        //        cash_sum.SelectionStart = curpos;
        //        //        //MessageBox.Show("1");
        //        //    }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(" cash_sum_KeyUp "+ex.Message);
        //    }
        //    //int start = cash_sum.SelectionStart;
        //    //string _sum_ = cash_sum.Text;
        //    //cash_sum.Text = "";
        //    //cash_sum.Text = _sum_;
        //    //cash_sum.SelectionStart = start;
        //    //cash_sum.Update();
        //    ////if (MainStaticClass.get_currency() == "руб.")
        //    ////{

        //    ////}
        //    ////else
        //    ////{
        //    //    cash_sum.SelectionStart = curpos;
        //    ////}
        //}

        //private void cash_sum_KeyPress(object sender, KeyPressEventArgs e)
        //{

        //    if ((Char.IsDigit(e.KeyChar)))
        //    {
        //        if (firs_input)
        //        {
        //            firs_input = false;
        //            cash_sum.Text = e.KeyChar + cash_sum.Text.Substring(1, cash_sum.Text.Length - 1);
        //            e.Handled = true;
        //            cash_sum.SelectionStart = 1;
        //            curpos = 1;
        //        }
        //    }

        //    if (!(Char.IsDigit(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
        //    {
        //        e.Handled = true;
        //    }                

        //    if (!(Char.IsDigit(e.KeyChar)))
        //    {
        //        if ((e.KeyChar == '.') || (e.KeyChar == ','))
        //        {

        //            cash_sum.SelectionStart = cash_sum.Text.IndexOf(MainStaticClass.NumberDecimalSeparator()) + 1;
        //            curpos = cash_sum.SelectionStart; e.Handled = true;
        //        }

        //        if (e.KeyChar != (char)Keys.Back)
        //        {

        //            e.Handled = true;
        //        }
        //    }           

        //    if (e.Handled == false)
        //    {
        //        if (e.KeyChar != (char)Keys.Back)
        //        {
        //            curpos = cash_sum.SelectionStart;
        //            curpos++;
        //        }
        //        else
        //        {
        //            if (cash_sum.SelectionStart != 0)
        //            {
        //                if (cash_sum.Text.Substring(cash_sum.SelectionStart - 1, 1) == ".")
        //                {
        //                    e.Handled = true;
        //                    cash_sum.SelectionStart -= 1;
        //                    curpos = cash_sum.SelectionStart;
        //                }
        //                else if ((cash_sum.SelectionStart == 2) && (!e.Handled))
        //                {
        //                    curpos = 1;
        //                }
        //                else
        //                {
        //                    curpos = cash_sum.SelectionStart - 1;
        //                }
        //            }

        //        }
        //    }

        //    this.cash_sum.Update();
        //}

        //private void cash_sum_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (Char.IsDigit(e.KeyChar))
        //    {
        //        int selectionStart = txtB_cash_sum.SelectionStart;
        //        if (firs_input)
        //        {
        //            firs_input = false;
        //            txtB_cash_sum.Text = e.KeyChar + txtB_cash_sum.Text.Substring(1);
        //            e.Handled = true;
        //            txtB_cash_sum.SelectionStart = 1;
        //        }
        //        else
        //        {
        //            txtB_cash_sum.Text = txtB_cash_sum.Text.Insert(selectionStart, e.KeyChar.ToString());
        //            txtB_cash_sum.SelectionStart = selectionStart + 1;
        //            e.Handled = true;
        //        }
        //    }
        //    else if (e.KeyChar == '.' || e.KeyChar == ',')
        //    {
        //        if (!txtB_cash_sum.Text.Contains(MainStaticClass.NumberDecimalSeparator()))
        //        {
        //            int selectionStart = txtB_cash_sum.SelectionStart;
        //            txtB_cash_sum.Text = txtB_cash_sum.Text.Insert(selectionStart, MainStaticClass.NumberDecimalSeparator());
        //            txtB_cash_sum.SelectionStart = selectionStart + 1;
        //            e.Handled = true;
        //        }
        //        else
        //        {
        //            txtB_cash_sum.SelectionStart = txtB_cash_sum.Text.IndexOf(MainStaticClass.NumberDecimalSeparator()) + 1;
        //            e.Handled = true;
        //        }
        //    }
        //    else if (e.KeyChar != (char)Keys.Back)
        //    {
        //        e.Handled = true;
        //    }

        //    // Ensure two digits after the decimal point
        //    if (txtB_cash_sum.Text.Contains(MainStaticClass.NumberDecimalSeparator()))
        //    {
        //        int decimalIndex = txtB_cash_sum.Text.IndexOf(MainStaticClass.NumberDecimalSeparator());
        //        if (txtB_cash_sum.Text.Length - decimalIndex - 1 < 2)
        //        {
        //            txtB_cash_sum.Text = txtB_cash_sum.Text.Substring(0, decimalIndex + 1) + txtB_cash_sum.Text.Substring(decimalIndex + 1).PadRight(2, '0');
        //            txtB_cash_sum.SelectionStart = decimalIndex + 1;
        //        }
        //    }

        //    // Correct cursor position if it moves to the start
        //    if (txtB_cash_sum.SelectionStart == 0)
        //    {
        //        txtB_cash_sum.SelectionStart = txtB_cash_sum.Text.Length;
        //    }

        //    this.txtB_cash_sum.Update();
        //}

        private void txtB_cash_sum_KeyPress(object sender, KeyPressEventArgs e)
        {

            // Разрешаем вводить цифры, Delete и Backspace
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }

            if (Char.IsDigit(e.KeyChar))
            {
                int selectionStart = txtB_cash_sum.SelectionStart;
                if (firs_input)
                {
                    firs_input = false;
                    txtB_cash_sum.Text = e.KeyChar + txtB_cash_sum.Text.Substring(1);
                    e.Handled = true;
                    txtB_cash_sum.SelectionStart = 1;
                }
                else
                {
                    txtB_cash_sum.Text = txtB_cash_sum.Text.Insert(selectionStart, e.KeyChar.ToString());
                    txtB_cash_sum.SelectionStart = selectionStart + 1;
                    e.Handled = true;
                }
            }
            else if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                if (!txtB_cash_sum.Text.Contains(MainStaticClass.NumberDecimalSeparator()))
                {
                    int selectionStart = txtB_cash_sum.SelectionStart;
                    txtB_cash_sum.Text = txtB_cash_sum.Text.Insert(selectionStart, MainStaticClass.NumberDecimalSeparator());
                    txtB_cash_sum.SelectionStart = selectionStart + 1;
                    e.Handled = true;
                }
                else
                {
                    txtB_cash_sum.SelectionStart = txtB_cash_sum.Text.IndexOf(MainStaticClass.NumberDecimalSeparator()) + 1;
                    e.Handled = true;
                }
            }
            else if (e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            //Ensure two digits after the decimal point
            if (txtB_cash_sum.Text.Contains(MainStaticClass.NumberDecimalSeparator()))
            {
                int decimalIndex = txtB_cash_sum.Text.IndexOf(MainStaticClass.NumberDecimalSeparator());
                if (txtB_cash_sum.Text.Length - decimalIndex - 1 < 2)
                {
                    txtB_cash_sum.Text = txtB_cash_sum.Text.Substring(0, decimalIndex + 1) + txtB_cash_sum.Text.Substring(decimalIndex + 1).PadRight(2, '0');
                    txtB_cash_sum.SelectionStart = decimalIndex + 1;
                }
            }

            // Correct cursor position if it moves to the start
            if (txtB_cash_sum.SelectionStart == 0)
            {
                txtB_cash_sum.SelectionStart = txtB_cash_sum.Text.Length;
            }

            this.txtB_cash_sum.Update();
        }

        private void Pay_Load(object sender, EventArgs e)
        {
            //if (MainStaticClass.SelfServiceKiosk == 1)//это киоск самообслуживания 
            //{
            //    this.cash_sum.Enabled = false;
            //    this.non_cash_sum.Enabled = false;                
            //}
            //else
            //{
            this.txtB_cash_sum.Text = ""; cc.calculation_of_the_sum_of_the_document().ToString();
            this.txtB_cash_sum.SelectionLength = 0;
            //}
            this.txtB_cash_sum.Focus();
            //txtB_cash_sum.SelectionStart = 0;
            non_cash_sum.SelectionStart = 0;
            pay_bonus.SelectionStart = 0;
            this.panel1.Size = new System.Drawing.Size(SystemInformation.PrimaryMonitorSize.Width - 100, SystemInformation.PrimaryMonitorSize.Height - 100);
            this.Top = 0;
            this.Left = 0;
            this.Size = new System.Drawing.Size(SystemInformation.PrimaryMonitorSize.Width, SystemInformation.PrimaryMonitorSize.Height);
            this.pay_bonus_many.Text = "0";
            this.pay_bonus.Text = "0";
            this.sertificates_sum.Text = "0";
            //this.non_cash_sum.Text = "0";
            //this.non_cash_sum_kop.Text = "0";
            //this.cash_sum.Text = "";

            //decimal summ_sertificates = 0;
            //foreach (ListViewItem lvi in listView_sertificates.Items)
            //{

            //    listView_sertificates.Items.Add((ListViewItem)lvi.Clone());
            //    summ_sertificates += decimal.Parse(lvi.SubItems[2].Text);
            //}

            //sertificates_sum.Text = summ_sertificates.ToString();

            //if (MainStaticClass.SelfServiceKiosk == 0)//это не киоск самообслуживания )
            //{
            this.non_cash_sum.Text = "0";
                this.non_cash_sum_kop.Text = "0";               
            //}

            calculate();


            //if (MainStaticClass.GetWorkSchema == 2)
            //{
            //    //if (cc.check_type.SelectedIndex == 0)
            //    //{
            //    //    continue_sales();//Для получения бонусов по документу и записи его в документ
            //    //}
            //    label4.Visible = true;
            //    bonus_on_document.Visible = true;
            //    label5.Visible = true;
            //    bonus_total_in_centr.Visible = true;
            //    label6.Visible = true;
            //    pay_bonus.Visible = true;
            //    pay_bonus_many.Visible = true;
            //    if (pay_bonus.Enabled)
            //    {
            //        if (bonus_total_in_centr.Text.Trim() != "")
            //        {
            //            if (Convert.ToInt64(bonus_total_in_centr.Text) == 0)
            //            {
            //                pay_bonus.Enabled = false;
            //            }
            //        }
            //        else
            //        {
            //            pay_bonus.Enabled = false;
            //        }
            //    }
            //}
            //if (MainStaticClass.Nick_Shop == "A01")
            //{
            //    
            //}
            if ((MainStaticClass.IpAddressAcquiringTerminal.Trim() != "") && (MainStaticClass.IdAcquirerTerminal.Trim() != ""))
            {
                if (MainStaticClass.GetAcquiringBank == 1)//РНКБ
                {
                    checkBox_payment_by_sbp.Visible = true;                    
                }
                checkBox_do_not_send_payment_to_the_terminal.Visible = true;
            }

            if (cc.payment_by_sbp_sales)
            {
                checkBox_payment_by_sbp.Checked = true;                
            }
            //WaitNonCashPay waitNonCashPay = new WaitNonCashPay();
            //waitNonCashPay.ShowDialog();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            cc.Visible = true;
        }


        private void set_non_Cash_pay()
        {
            txtB_cash_sum.Text = "";
            double sum_of_the_document = get_sum_sum_at_a_discount();
            string sumString = sum_of_the_document.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            string[] parts = sumString.Split('.');

            non_cash_sum.Text = parts[0];
            //non_cash_sum_kop.Text = parts.Length > 1 ? parts[1] : "00";
            non_cash_sum_kop.Text = parts[1];
        }

        private void set_Cash_pay()
        {
            txtB_cash_sum.Text = "";

            double sum_of_the_document = get_sum_sum_at_a_discount();
            var culture = new System.Globalization.CultureInfo("ru-RU"); // Например, для русской культуры            
            string sumString = sum_of_the_document.ToString("F2", culture);           

            non_cash_sum.Text = "0";
            non_cash_sum_kop.Text = "00";

            txtB_cash_sum.Text = sumString;         
            txtB_cash_sum.Focus();
        }



        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.R)
            {
                set_non_Cash_pay();
            }

            if (e.KeyCode == Keys.Y)
            {
                set_Cash_pay();
            }

            if (e.KeyCode == Keys.F4 && e.Alt)
            {
                e.Handled = true;
            }
            if (e.KeyCode == Keys.F5)
            {
                button1_Click(null, null);
            }
            if (e.KeyCode == Keys.F12)
            {
                if (button_pay.Enabled)
                {
                    button2_Click(null, null);
                }
            }           
            if (e.KeyValue == 37)
            {             
                if (txtB_cash_sum.Focused)
                {
                    if (curpos > 0)
                    {
                        curpos--;
                    }
                }
                if (non_cash_sum.Focused)
                {
                    if (curpos_non_cash > 0)
                    {
                        curpos_non_cash--;
                    }

                }             
            }
            if (e.KeyValue == 39)
            {               
                if (txtB_cash_sum.Focused)
                {
                    if (curpos < txtB_cash_sum.Text.Length - 1)
                    {
                        curpos++;
                    }
                }
                if (non_cash_sum.Focused)
                {
                    if (curpos_non_cash < non_cash_sum.Text.Length - 1)
                    {
                        curpos_non_cash++;
                    }
                }             
            }
            if (e.KeyCode == Keys.Delete)
            {
                if (txtB_cash_sum.Focused)
                {
                    if (txtB_cash_sum.Text.Trim().Length > 0)
                    {
                        if (txtB_cash_sum.Text.Substring(txtB_cash_sum.SelectionStart, 1) == ".")
                        {
                            e.Handled = true;
                        }
                        else
                        {
                            curpos--;
                        }
                    }
                }
            }
            if (e.KeyCode == Keys.F8)
            {
                InputSertificates i_s = new InputSertificates();
                i_s.pay = this;                
                i_s.ShowDialog(this);
                if (!i_s.closed_normally)
                {
                    MessageBox.Show("Введенные сертификаты будут удалены");
                    this.listView_sertificates.Clear();
                }
                i_s.Dispose();
                calculate();
                txtB_cash_sum.Focus();
            }
        //    if (e.KeyCode == Keys.F6)//попытка сделать доступным полее вода списания бонусов
        //    {
                
        //        if (cc.client.Tag == null)
        //        {
        //            MessageBox.Show("В чеке нет карты лояльности.Оплата бонусами невозможна.");
        //            return;
        //        }
        //        //if (cc.checkBox_viza_d.CheckState == CheckState.Checked)
        //        //{
        //        //    MessageBox.Show("В чеке выбрана скидка.Оплата бонусами невозможна!");
        //        //    return;
        //        //}
        //        if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema ==3) || (MainStaticClass.GetWorkSchema == 4))
        //        {
        //            if (!cc.check_bonus_is_on())
        //            {
        //                return;
        //            }
        //        }
        //        //if (Convert.ToInt32(bonus_total_in_centr.Text) == 0)
        //        //{
        //        //    MessageBox.Show("Нет доступных для списания бонусов");
        //        //    return;
        //        //}
        //        InputCodeNumberPhone inputCodeNumberPhone = new InputCodeNumberPhone();
        //        inputCodeNumberPhone.code_client = cc.client.Tag.ToString();
        //        inputCodeNumberPhone.phone_client = cc.phone_client;
        //        DialogResult dialogResult = inputCodeNumberPhone.ShowDialog();
        //        if (dialogResult == DialogResult.Yes)
        //        {
        //            code_it_is_confirmed = true;
        //            pay_bonus.Enabled = true;
        //        }
        //        calculate();
        //        if (button_pay.Enabled)
        //        {
        //            button2_Click(null, null);
        //        }
        //    }
        }

        /*Оплачено
         *Это процедура записи документа в базу данных 
         */
        private void it_is_paid()
        {
            //if (cc.it_is_paid(cash_sum.Text,cash_sum.Text, remainder.Text))
            if (cc.check_type.SelectedIndex == 0)
            {
                //здесь перед записью еще проверка процессингового центра 
                //if (!continue_sales())Пока убрал есть вызов выше 
                //{
                //    return;
                //}

                if ((Convert.ToDecimal(txtB_cash_sum.Text) - Convert.ToDecimal(remainder.Text)) < 0)
                {
                    MessageBox.Show("Ошибка при определении суммы наличных");
                    return;
                }
                //Получаем копейки которые необходимо распределить
                double total = Convert.ToDouble(pay_sum.Text);
                //double bonus = Convert.ToDouble(pay_bonus_many.Text);

                //if (!MainStaticClass.get_use_debug())
                //{                    

                //if (cc.client.Tag != null)
                //{
                //    //***********************************************   
                //    if ((MainStaticClass.GetWorkSchema == 1) || (MainStaticClass.GetWorkSchema == 3) || (MainStaticClass.GetWorkSchema == 4))
                //    {
                //        //if (Convert.ToDecimal(pay_bonus_many.Text) > 0)
                //        //{
                //        //    if (Convert.ToDecimal(non_cash_sum.Text) == 0)
                //        //    {
                //        //        cc.distribute(Convert.ToDouble(pay_bonus_many.Text) + (total - (int)total), total);//теперь бонусы 
                //        //    }
                //        //    else
                //        //    {
                //        //        cc.distribute(Convert.ToDouble(pay_bonus_many.Text), total);//теперь бонусы 
                //        //    }
                //        //}
                //        //else
                //        //{

                //        if (Convert.ToDecimal(non_cash_sum.Text) == 0)
                //        {
                //            cc.distribute(Math.Round(total - (int)total, 2, MidpointRounding.ToEven), total);
                //        }

                //        //}
                //    }
                //    //else if (MainStaticClass.GetWorkSchema == 2)
                //    //{
                //    //    if (Convert.ToDecimal(pay_bonus_many.Text) > 0)
                //    //    {
                //    //        cc.distribute(Convert.ToDouble(pay_bonus_many.Text), total);//теперь бонусы 
                //    //    }                       
                //    //}
                //}
                //else
                //{
                //    if ((MainStaticClass.GetWorkSchema == 1) || (MainStaticClass.GetWorkSchema == 3) || (MainStaticClass.GetWorkSchema == 4))
                //    {
                //        if (Convert.ToDecimal(non_cash_sum.Text) == 0)
                //        {
                //            cc.distribute(Math.Round(total - (int)total, 2, MidpointRounding.ToEven), total);//распределение без бонусов , здесь нет клиента нет бонусов
                //        }
                //    }
                //}

                if (Convert.ToDecimal(pay_sum.Text) - (Convert.ToDecimal(txtB_cash_sum.Text) - Convert.ToDecimal(remainder.Text) + Convert.ToDecimal(sertificates_sum.Text) + Convert.ToDecimal(pay_bonus_many.Text) + Convert.ToDecimal(non_cash_sum.Text)) > 1)
                {
                    MessageBox.Show(" Неверно внесенные суммы ");
                    return;
                }

                //Проверка суммы со скидкой
                bool less_than_zero = false;
                foreach (ListViewItem lvi in cc.listView1.Items)
                {
                    //MessageBox.Show(lvi.SubItems[0].Text + "   " + lvi.SubItems[7].Text);

                    if (Convert.ToDecimal(lvi.SubItems[7].Text) < 0)
                    {
                        less_than_zero = true;
                        break;
                    }
                }

                if (less_than_zero)
                {
                    MessageBox.Show(" При распределении расчетов получилась отрицательная сумма в строке, попробуйте ввести суммы оплаты еще раз");
                    return;
                }

                //параметры подключение терминала заполнены и сумма по карте к оплате заполнена
                double notCashSum = Convert.ToDouble(this.non_cash_sum.Text.Trim()) + Convert.ToDouble(non_cash_sum_kop.Text) / 100;

                if ((MainStaticClass.IpAddressAcquiringTerminal.Trim() != "") && (MainStaticClass.IdAcquirerTerminal.Trim() != "") && notCashSum > 0)
                {
                    if (checkBox_do_not_send_payment_to_the_terminal.CheckState == CheckState.Unchecked)                    {

                        string money = ((Convert.ToDouble(this.non_cash_sum.Text.Trim()) + Convert.ToDouble(non_cash_sum_kop.Text) / 100) * 100).ToString();

                        if (MainStaticClass.GetAcquiringBank == 1) //РНКБ
                        {
                            //if ((checkBox_payment_by_sbp.CheckState != CheckState.Checked) && (checkBox_do_not_send_payment_to_the_terminal.CheckState == CheckState.Unchecked))
                            if (checkBox_payment_by_sbp.CheckState != CheckState.Checked)
                            {
                                string url = "http://" + MainStaticClass.IpAddressAcquiringTerminal;
                                //string money = ((Convert.ToDouble(this.non_cash_sum.Text.Trim()) + Convert.ToDouble(non_cash_sum_kop.Text) / 100) * 100).ToString();
                                string _str_command_sale_ = str_command_sale.Replace("sum", money);
                                _str_command_sale_ = _str_command_sale_.Replace("id_terminal", MainStaticClass.IdAcquirerTerminal);
                                //MessageBox.Show(_str_command_sale_);

                                AnswerTerminal answerTerminal = new AnswerTerminal();

                                //if ((MainStaticClass.Nick_Shop == "A01")|| (MainStaticClass.Nick_Shop == "E50"))
                                //{
                                WaitNonCashPay waitNonCashPay = new WaitNonCashPay();
                                waitNonCashPay.Url = url;
                                waitNonCashPay.Data = _str_command_sale_;
                                waitNonCashPay.cc = this.cc;
                                waitNonCashPay.ShowDialog();
                                if (waitNonCashPay.commandResult != null)
                                {
                                    answerTerminal = waitNonCashPay.commandResult.AnswerTerminal;
                                    complete = waitNonCashPay.commandResult.Status;
                                }
                                else
                                {
                                    MessageBox.Show("Результат команды не получен.\r\nНеудачная попытка опалты", "Неудачная попытка опалты");
                                    calculate();
                                    return;
                                }
                                //}
                                //else
                                //{
                                //    send_command_acquiring_terminal(url, _str_command_sale_, ref complete, ref answerTerminal);
                                //}

                                if (!complete)//ответ от терминала не удовлетворительный
                                {
                                    calculate();
                                    cc.recharge_note = "";
                                    MessageBox.Show(" Неудачная попытка получения оплаты ", "Оплата по терминалу");
                                    return;
                                }
                                else
                                {
                                    cc.code_authorization_terminal = answerTerminal.code_authorization;     //13 поле
                                    cc.id_transaction_terminal = answerTerminal.number_reference;  //14 поле                                    
                                }
                            }
                            else
                            {
                                //if(checkBox_do_not_send_payment_to_the_terminal.CheckState == CheckState.Unchecked)
                                string url = "http://" + MainStaticClass.IpAddressAcquiringTerminal;
                                //string money = ((Convert.ToDouble(this.non_cash_sum.Text.Trim()) + Convert.ToDouble(non_cash_sum_kop.Text) / 100) * 100).ToString();
                                string _str_sale_sbp = str_sale_sbp.Replace("sum", money);
                                _str_sale_sbp = _str_sale_sbp.Replace("id_terminal", MainStaticClass.IdAcquirerTerminal);
                                _str_sale_sbp = _str_sale_sbp.Replace("guid", cc.guid);
                                ////MessageBox.Show(_str_command_sale_);
                                AnswerTerminal answerTerminal = new AnswerTerminal();
                                send_command_acquiring_terminal(url, _str_sale_sbp, ref complete, ref answerTerminal);
                                if (!complete)//ответ от терминала не удовлетворительный, значит операция в обработке необходим дополнительный запрос
                                {
                                    string _str_payment_status_sale_sbp = str_payment_status_sale_sbp.Replace("sum", money);
                                    _str_payment_status_sale_sbp = _str_payment_status_sale_sbp.Replace("id_terminal", MainStaticClass.IdAcquirerTerminal);
                                    _str_payment_status_sale_sbp = _str_payment_status_sale_sbp.Replace("sale_code_authorization_terminal", cc.guid);
                                    while (1 == 1)
                                    {
                                        answerTerminal = new AnswerTerminal();
                                        send_command_acquiring_terminal(url, _str_payment_status_sale_sbp, ref complete, ref answerTerminal);
                                        if (complete)//получен ответ об успешной оплате, прерываем цикл
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            if (answerTerminal.сode_response_in_15_field == "R00")//Операция в обработке 
                                            {
                                                if (answerTerminal.сode_response_in_39_field == "0")
                                                {
                                                    continue;
                                                }
                                            }

                                            if (answerTerminal.сode_response_in_15_field == "R10")
                                            {
                                                MessageBox.Show(" Операция отклонена ", "Оплата по терминалу");
                                                break;
                                            }
                                            else if (answerTerminal.сode_response_in_15_field == "R11")
                                            {
                                                MessageBox.Show(" Операции по QR коду не существует. ", "Оплата по терминалу");
                                                break;
                                            }
                                            else if (answerTerminal.сode_response_in_15_field == "R12")
                                            {
                                                if (answerTerminal.сode_response_in_39_field == "0")
                                                {
                                                    MessageBox.Show(" Не получен ответ на запрос статуса ", "Оплата по терминалу");
                                                    break;
                                                }
                                                else if (answerTerminal.сode_response_in_39_field == "16")
                                                {
                                                    MessageBox.Show(" Не получен ответ на запрос QR - кода ", "Оплата по терминалу");
                                                    break;
                                                }
                                            }
                                            else if (answerTerminal.сode_response_in_15_field == "R13")
                                            {
                                                MessageBox.Show(" Запрос статуса не отправлен ", "Оплата по терминалу");
                                                break;
                                            }
                                            else if (answerTerminal.сode_response_in_15_field == "R14")
                                            {
                                                MessageBox.Show(" Операция не добавлена в базу транзакций терминала ", "Оплата по терминалу");
                                                break;
                                            }
                                            if (answerTerminal.error)
                                            {
                                                if (answerTerminal.error_code != 404)
                                                {
                                                    if (MessageBox.Show(" Продолжать опрос об оплате клиента по СБП ", "Продолжать опрос об оплате клиента по СБП", MessageBoxButtons.YesNo) == DialogResult.No)
                                                    {
                                                        //пользователь отказался от дальнейшего ожидания информации об оплате
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (!complete)//если не удалось получить информацию об успешной оплате
                                    {
                                        calculate();
                                        cc.recharge_note = "";
                                        MessageBox.Show(" Неудачная попытка получения оплаты ", "СБП");
                                        return;
                                    }
                                    else
                                    {
                                        cc.code_authorization_terminal = answerTerminal.code_authorization;     //13 поле
                                        cc.id_transaction_terminal = answerTerminal.number_reference;           //14 поле
                                        cc.payment_by_sbp = (checkBox_payment_by_sbp.CheckState == CheckState.Checked ? true : false);
                                    }
                                }
                                else//был сразу получен успешный ответ по по оплате СБП
                                {
                                    cc.code_authorization_terminal = answerTerminal.code_authorization;     //13 поле
                                    cc.id_transaction_terminal = answerTerminal.number_reference;           //14 поле
                                    cc.payment_by_sbp = (checkBox_payment_by_sbp.CheckState == CheckState.Checked ? true : false);
                                }
                            }
                        }
                        else if (MainStaticClass.GetAcquiringBank == 2)//СБЕР
                        {
                            try
                            {
                                CommandWrapper.return_slip = "";
                                AuthAnswer13 authAnswer = CommandWrapper.Authorization(Convert.ToInt32(money));
                                cc.id_transaction_terminal = authAnswer.RRN;
                                if (CommandWrapper.return_slip.Trim().Length != 0)
                                {
                                    IFptr fptr = MainStaticClass.FPTR;
                                    if (!fptr.isOpened())
                                    {
                                        fptr.open();
                                    }

                                    fptr.beginNonfiscalDocument();
                                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, CommandWrapper.return_slip);
                                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_DEFER, AtolConstants.LIBFPTR_DEFER_POST);
                                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_CENTER);
                                    fptr.printText();
                                    fptr.endNonfiscalDocument();
                                    //if (MainStaticClass.GetVariantConnectFN == 1)
                                    //{
                                    //    fptr.close();
                                    //}
                                }
                                else
                                {
                                    MessageBox.Show(" Не удалось получить слип с терминала ", "Неудачная попытка оплаты по терминалу");
                                    calculate();
                                    return;
                                }
                                //authAnswer.
                                //Trace.WriteLine("Списание произвели. RRN:{authAnswer.RRN}. CardNumber:{authAnswer.CardID}");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Произошла ошибка при попытке оплаты по терминалу \r\n" + ex.Message);
                                calculate();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show(" У вас в константах не выбран банк эквайринга");
                            calculate();
                            return;
                        }
                    }
                }

                //Получить сумму наличных
                //если это возврат и если сумма безнала меньше 1 тогда копейки прибавить к наличным
                string sum_cash_pay = (Convert.ToDecimal(txtB_cash_sum.Text) - Convert.ToDecimal(remainder.Text)).ToString().Replace(",", ".");
                string non_sum_cash_pay = (get_non_cash_sum()).ToString().Replace(",", ".");
                cc.print_to_button = 0;
                //cc.payment_by_sbp = (checkBox_payment_by_sbp.CheckState == CheckState.Checked ? true : false);//Перенес выше в секцию РНКБ, здесь было до появления сбера
                if (cc.it_is_paid(txtB_cash_sum.Text, cc.calculation_of_the_sum_of_the_document().ToString().Replace(",", "."), remainder.Text.Replace(",", "."),
                (pay_bonus_many.Text.Trim() == "" ? "0" : pay_bonus_many.Text.Trim()),
                true,
            sum_cash_pay,
            non_sum_cash_pay,
            Convert.ToDecimal(sertificates_sum.Text).ToString().Replace(",", ".")))
                {
                    cc.closing = false;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            else//ЭТО ВОЗВРАТ
            {

                string sum_cash_pay = (Convert.ToDecimal(txtB_cash_sum.Text) - Convert.ToDecimal(remainder.Text)).ToString().Replace(",", ".");
                string non_sum_cash_pay = (get_non_cash_sum()).ToString().Replace(",", ".");
                if (cc.check_type.SelectedIndex == 1)
                {
                    if (get_non_cash_sum() < 1)
                    {
                        sum_cash_pay = (Convert.ToDecimal(txtB_cash_sum.Text) - Convert.ToDecimal(remainder.Text) + Convert.ToDecimal(get_non_cash_sum())).ToString().Replace(",", ".");
                        non_sum_cash_pay = "0";
                    }
                }

                //здесь надо понимать возврат сегодняшний или более ранний

                if ((MainStaticClass.IpAddressAcquiringTerminal.Trim() != "") && (MainStaticClass.IdAcquirerTerminal.Trim() != "") && (Convert.ToDouble(non_cash_sum.Text) > 0))
                {
                    if (checkBox_do_not_send_payment_to_the_terminal.CheckState == CheckState.Unchecked)
                    {
                        string money = ((Convert.ToDouble(this.non_cash_sum.Text.Trim()) + Convert.ToDouble(non_cash_sum_kop.Text) / 100) * 100).ToString();

                        if (MainStaticClass.GetAcquiringBank == 1)//РНКБ
                        {
                            string url = "http://" + MainStaticClass.IpAddressAcquiringTerminal;
                            //string money = ((Convert.ToDouble(this.non_cash_sum.Text.Trim()) + Convert.ToDouble(non_cash_sum_kop.Text) / 100) * 100).ToString();
                            //Поскольку нет автоматической конвертации отмены в возврат, то необходимо 2 варианта печати для возвратов                     
                            DateTime today = DateTime.Today;
                            AnswerTerminal answerTerminal = new AnswerTerminal();
                            if (checkBox_payment_by_sbp.CheckState != CheckState.Checked)
                            {
                                if (cc.sale_date.CompareTo(today) < 0)
                                {
                                    string _str_return_sale_ = str_return_sale.Replace("sum", money);
                                    _str_return_sale_ = _str_return_sale_.Replace("id_terminal", MainStaticClass.IdAcquirerTerminal);
                                    _str_return_sale_ = _str_return_sale_.Replace("sale_code_authorization_terminal", cc.sale_code_authorization_terminal);
                                    _str_return_sale_ = _str_return_sale_.Replace("number_reference", cc.sale_id_transaction_terminal);
                                    send_command_acquiring_terminal(url, _str_return_sale_, ref complete, ref answerTerminal);
                                }
                                else
                                {
                                    string _str_return_sale_ = str_cancel_sale.Replace("sum", money);
                                    _str_return_sale_ = _str_return_sale_.Replace("id_terminal", MainStaticClass.IdAcquirerTerminal);
                                    _str_return_sale_ = _str_return_sale_.Replace("sale_code_authorization_terminal", cc.sale_code_authorization_terminal);
                                    _str_return_sale_ = _str_return_sale_.Replace("number_reference", cc.sale_id_transaction_terminal);
                                    if (money.Trim() != (cc.sale_non_cash_money * 100).ToString().Trim())//Это частичная отмена.
                                    {
                                        _str_return_sale_ = _str_return_sale_.Replace("sale_non_cash_money", (cc.sale_non_cash_money * 100).ToString());
                                    }
                                    else
                                    {
                                        _str_return_sale_ = _str_return_sale_.Replace(@"<field id=""01"">sale_non_cash_money</field>", "");
                                    }

                                    send_command_acquiring_terminal(url, _str_return_sale_, ref complete, ref answerTerminal);
                                }
                            }
                            else
                            {
                                string _str_return_sale_sbp_ = str_return_sale_sbp.Replace("sum", money);
                                _str_return_sale_sbp_ = _str_return_sale_sbp_.Replace("id_terminal", MainStaticClass.IdAcquirerTerminal);
                                _str_return_sale_sbp_ = _str_return_sale_sbp_.Replace("sale_code_authorization_terminal", cc.sale_id_transaction_terminal);// cc.sale_code_authorization_terminal);
                                _str_return_sale_sbp_ = _str_return_sale_sbp_.Replace("guid", cc.guid_sales);
                                send_command_acquiring_terminal(url, _str_return_sale_sbp_, ref complete, ref answerTerminal);
                                if (!complete)//ответ от терминала не удовлетворительный
                                {
                                    string _str_payment_status_return_sale_sbp_ = str_payment_status_return_sale_sbp.Replace("sum", money);
                                    _str_payment_status_return_sale_sbp_ = _str_payment_status_return_sale_sbp_.Replace("id_terminal", MainStaticClass.IdAcquirerTerminal);
                                    _str_payment_status_return_sale_sbp_ = _str_payment_status_return_sale_sbp_.Replace("sale_code_authorization_terminal", cc.sale_id_transaction_terminal);// cc.sale_code_authorization_terminal);
                                    _str_payment_status_return_sale_sbp_ = _str_payment_status_return_sale_sbp_.Replace("guid", cc.guid_sales);
                                    //send_command_acquiring_terminal(url, _str_payment_status_return_sale_sbp_, ref complete, ref answerTerminal);
                                    while (1 == 1)
                                    {
                                        answerTerminal = new AnswerTerminal();
                                        send_command_acquiring_terminal(url, _str_payment_status_return_sale_sbp_, ref complete, ref answerTerminal);
                                        if (complete)//получен ответ об успешной оплате, прерываем цикл
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            if (answerTerminal.сode_response_in_15_field == "R10")
                                            {
                                                MessageBox.Show(" Операция отклонена ");
                                                break;
                                            }
                                            else if (answerTerminal.сode_response_in_15_field == "R11")
                                            {
                                                MessageBox.Show(" Операции по QR коду не существует. ");
                                                break;
                                            }
                                            else if (answerTerminal.сode_response_in_15_field == "R12")
                                            {
                                                if (answerTerminal.сode_response_in_39_field == "0")
                                                {
                                                    MessageBox.Show(" Не получен ответ на запрос статуса ");
                                                    break;
                                                }
                                                else if (answerTerminal.сode_response_in_39_field == "16")
                                                {
                                                    MessageBox.Show(" Не получен ответ на запрос QR - кода ");
                                                    break;
                                                }
                                            }
                                            else if (answerTerminal.сode_response_in_15_field == "R13")
                                            {
                                                MessageBox.Show(" Запрос статуса не отправлен ");
                                                break;
                                            }
                                            else if (answerTerminal.сode_response_in_15_field == "R14")
                                            {
                                                MessageBox.Show(" Операция не добавлена в базу транзакций терминала ");
                                                break;
                                            }
                                            if (answerTerminal.error)
                                            {
                                                if (MessageBox.Show(" Продолжать опрос по возврату оплаты по СБП ", "Опрос по возврату оплаты по СБП", MessageBoxButtons.YesNo) == DialogResult.No)
                                                {
                                                    //пользователь отказался от дальнейшего ожидания информации об оплате
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (!complete)//ответ от терминала не удовлетворительный
                            {
                                calculate();
                                MessageBox.Show(" Неудачная попытка возврата оплаты ", "СБП");
                                return;
                            }
                            else
                            {
                                cc.code_authorization_terminal = answerTerminal.code_authorization;//13 поле
                                cc.id_transaction_terminal = answerTerminal.number_reference;  //14 поле 
                                cc.payment_by_sbp = (checkBox_payment_by_sbp.CheckState == CheckState.Checked ? true : false);
                            }
                        }
                        else if (MainStaticClass.GetAcquiringBank == 2)//СБЕР
                        {
                            try
                            {
                                //AuthAnswer13 authAnswer = CommandWrapper.Authorization(Convert.ToInt32(money));
                                //cc.id_transaction_terminal = authAnswer.RRN;
                                //Trace.WriteLine("Списание произвели. RRN:{authAnswer.RRN}. CardNumber:{authAnswer.CardID}");
                                CommandWrapper.return_slip = "";
                                AuthAnswer13 authAnswer = CommandWrapper.ReturnAmountToCard(Convert.ToInt32(money), cc.sale_id_transaction_terminal);
                                cc.id_transaction_terminal = authAnswer.RRN;

                                if (CommandWrapper.return_slip.Trim().Length != 0)
                                {
                                    IFptr fptr = MainStaticClass.FPTR;
                                    if (!fptr.isOpened())
                                    {
                                        fptr.open();
                                    }

                                    fptr.beginNonfiscalDocument();
                                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, CommandWrapper.return_slip);
                                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_DEFER, AtolConstants.LIBFPTR_DEFER_POST);
                                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_CENTER);
                                    fptr.printText();
                                    fptr.endNonfiscalDocument();
                                    //if (MainStaticClass.GetVariantConnectFN == 1)
                                    //{
                                    //    fptr.close();
                                    //}
                                }
                                else
                                {
                                    MessageBox.Show(" Не удалось получить слип с терминала ", "Неудачная возврата средств по терминалу");
                                    calculate();
                                    return;
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Произошла ошибка при попытке возврата средств по терминалу \r\n" + ex.Message);
                                calculate();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show(" У вас в константах не выбран банк эквайринга");
                            calculate();
                            return;
                        }
                    }
                }

                cc.sale_cancellation_Click(sum_cash_pay, non_sum_cash_pay);
                cc.closing = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }    

        public class AnswerTerminal
        {
            public string code_authorization { get; set; }
            public string number_reference { get; set; }
            public string сode_response_in_15_field { get; set; }
            public string сode_response_in_39_field { get; set; }
            public bool error { get; set; }
            public int error_code { get; set; }

            public AnswerTerminal()
            {
                number_reference = "";
                code_authorization = "";
            }
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


        /// <summary>
        /// Отправляет команду в эквайринг
        /// терминал и возвращает результат
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Data"></param>
        /// <param name="status"></param>
        public void send_command_acquiring_terminal(string Url, string Data, ref bool status,ref AnswerTerminal answerTerminal)
        {
            //string Out = String.Empty;
            
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(Url);
                req.Method = "POST";
                req.Timeout = 80000;
                //req.Timeout = 0;
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
                                answerTerminal.сode_response_in_39_field = field.Text;
                                if (field.Text.Trim() == "1")
                                {
                                    status = true;
                                }
                                else
                                {
                                    status = false;
                                }
                            }
                            else if (field.Id == "13")
                            {
                                answerTerminal.code_authorization = field.Text.Trim();
                            }
                            else if (field.Id == "14")
                            {
                                answerTerminal.number_reference = field.Text.Trim();
                            }
                            else if (field.Id == "15")
                            {
                                answerTerminal.сode_response_in_15_field = field.Text.Trim();                                
                            }
                            else if (field.Id == "90")
                            {
                                cc.recharge_note = field.Text.Trim();
                                int num_pos = cc.recharge_note.IndexOf("(КАССИР)");
                                if (num_pos > 0)
                                {
                                    cc.recharge_note = cc.recharge_note.Substring(0, num_pos + 8);
                                    //if ((answerTerminal.code_authorization == "sbpnspk")&&(answerTerminal.number_reference==""))//Оплата по сбп и не вернулся номер транзакции
                                    //{
                                    //    int num_pos1 = cc.recharge_note.IndexOf("TRN:");
                                    //    int num_pos2 = cc.recharge_note.IndexOf("Статус:");
                                    //    answerTerminal.number_reference = cc.recharge_note.Substring(num_pos1 + 4, num_pos2 - (num_pos1 + 4)).Replace("\r\n", "").Trim();
                                    //}
                                }
                            }
                        }
                    }
                }
                else
                {
                    status = false;
                }

                req = null;
                sendStream = null;
                myHttpWebResponse.Close();// = null;
            }
            catch (WebException ex)
            {
                status = false;
                MessageBox.Show(" Ошибка при оплате по карте  " + ex.Message, "Оплата по терминалу");//Код ошибки  "+ ((System.Net.Sockets.SocketException)ex.InnerException).ErrorCode
                answerTerminal.error = true;
                if (ex.Message.IndexOf("404") != -1)
                {
                    answerTerminal.error_code = 404;
                }
            }
            catch (Exception ex)
            {
                status = false;
                MessageBox.Show(" Ошибка при оплате по карте  " + ex.Message, "Оплата по терминалу");
                answerTerminal.error = true;
            }
        }

        //private bool continue_sales()
        //{
        //    bool result = true;

            //if (MainStaticClass.PassPromo != "")//Пароль не пустой бонусная магазин включен в бнусную систему
            //{
            //    if (cc.check_type.SelectedIndex == 0) // Это продажа
            //    {
            //        if (cc.client.Tag != null)
            //        {
            //            if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema ==3) || (MainStaticClass.GetWorkSchema == 4))
            //            {
            //            //    if (cc.check_bonus_is_on())
            //            //    {
            //            //        SentDataOnBonus.BuynewResponse buynewResponse = null;
            //            //        buynewResponse = cc.get_bonus_on_document((Convert.ToDecimal(pay_bonus_many.Text)*100).ToString());

            //            //        if (buynewResponse != null)
            //            //        {
            //            //            if (buynewResponse.res == "1")
            //            //            {
            //            //                this.bonus_on_document.Text = ((int)(Convert.ToInt64(buynewResponse.bonusSum) / 100)).ToString();
            //            //                cc.id_transaction = buynewResponse.transactionId;
            //            //                cc.bonuses_it_is_counted = Convert.ToInt32(this.bonus_on_document.Text);
            //            //            }
            //            //            else
            //            //            {
            //            //                get_description_errors_on_code(buynewResponse.res);
            //            //                result = false;
            //            //            }
            //            //        }
            //            //        else // Если оплата бонусами и нет связи с процессинговым цетром, тогда отлуп 
            //            //        {
            //            //            if (Convert.ToInt32(pay_bonus_many.Text) > 0)
            //            //            {
            //            //                MessageBox.Show(" Нет связи с процессинговым центром ");
            //            //                result = false;
            //            //            }
            //            //        }
            //            //    }
            //            //}
            //            //else if (MainStaticClass.GetWorkSchema == 2)
            //            //{
            //            //    SentDataOnBonusEva.BuynewResponse buynewResponse = null;
            //            //    buynewResponse = cc.get_bonus_on_document_eva(pay_bonus_many.Text);

            //            //    if (buynewResponse != null)
            //            //    {
            //            //        if (buynewResponse.res == "1")
            //            //        {
            //            //            //this.bonus_on_document.Text = ((int)(Convert.ToInt64(buynewResponse.bonusSum) / 100)).ToString();
            //            //            this.bonus_on_document.Text = buynewResponse.bonusSum;
            //            //            cc.id_transaction = buynewResponse.transactionId;
            //            //            cc.bonuses_it_is_counted = Convert.ToInt32(this.bonus_on_document.Text);
            //            //            cc.message_processing = buynewResponse.message;
            //            //        }
            //            //        else
            //            //        {
            //            //            get_description_errors_on_code(buynewResponse.res);
            //            //            result = false;
            //            //        }
            //            //    }
            //            //    else // Если оплата бонусами и нет связи с процессинговым цетром, тогда отлуп 
            //            //    {
            //            //        if (Convert.ToInt32(pay_bonus_many.Text) > 0)
            //            //        {
            //            //            MessageBox.Show(" Нет связи с процессинговым центром ");
            //            //            result = false;
            //            //        }
            //            //    }
            //            }
            //        }
            //    }
            //    else
            //    {
            //    //    if (MainStaticClass.GetWorkSchema == 2)
            //    //    {
            //    //        if (cc.client.Tag != null)
            //    //        {
            //    //            SentDataOnBonusEva.TransactionResponse transactionResponse = cc.get_bonus_on_document_eva_by_return();
            //    //            if (transactionResponse != null)
            //    //            {
            //    //                if (transactionResponse.res == "1")
            //    //                {
            //    //                    cc.id_transaction = transactionResponse.returnTransactionId;
            //    //                    cc.message_processing = transactionResponse.message;
            //    //                    cc.bonuses_it_is_written_off = ((int)(Convert.ToInt64(transactionResponse.bonusSum) / 100));
            //    //                }
            //    //                else
            //    //                {
            //    //                    if (transactionResponse.error != null)
            //    //                    {
            //    //                        MessageBox.Show(transactionResponse.error);
            //    //                    }
            //    //                    get_description_errors_on_code(transactionResponse.res);//Сообщим об ошибках пользователю
            //    //                    result = false;
            //    //                }
            //    //            }
            //    //            else
            //    //            {
            //    //                MessageBox.Show(" Нет связи с процессинговым центром ");
            //    //                result = false;
            //    //            }
            //    //        }
            //    //    }
            //    }
            //}
        //    return result;
        //}


        ///// <summary>
        ///// получить текстовое описание по коду 
        ///// </summary>
        ///// <param name="code_error"></param>
        //private void get_description_errors_on_code(string code_answer)
        //{
        //    switch (code_answer)
        //    {
        //        case "2":
        //            MessageBox.Show("2. Неверный запрос ");
        //            break;
        //        case "3":
        //            MessageBox.Show("3. Клиент не найден ");
        //            break;
        //        case "4":
        //            MessageBox.Show("4. Недостаточно прав для операции ");
        //            break;
        //        case "5":
        //            MessageBox.Show("5. Карта не найдена ");
        //            break;
        //        case "6":
        //            MessageBox.Show("6. Операции с картой запрещены ");
        //            break;
        //        case "7":
        //            MessageBox.Show("7. Ошибочная операция с картой ");
        //            break;
        //        case "8":
        //            MessageBox.Show("8. Недостаточно средств ");
        //            break;
        //        case "9":
        //            MessageBox.Show("9. Транзакция не найдена(неверный transactionId) ");
        //            break;
        //        case "10":
        //            MessageBox.Show("10. Значение параметра выходит за границы ");
        //            break;
        //        case "11":
        //            MessageBox.Show("11. Id чека, сгенерированный кассой, не уникален ");
        //            break;
        //        case "12":
        //            MessageBox.Show("12. Артикул товара(SKU) не найден ");
        //            break;
        //        case "13":
        //            MessageBox.Show("13. Ошибка запроса ");
        //            break;
        //        case "14":
        //            MessageBox.Show("14. Ошибка базы данных ");
        //            break;
        //        case "15":
        //            MessageBox.Show("15. Транзакция отклонена ");
        //            break;
        //        case "16":
        //            MessageBox.Show("16. Транзакция уже существует ");
        //            break;

        //        default:
        //            Console.WriteLine(" Неизвестный ответ от процессингового центра ");
        //            break;
        //    }
        //}



        private void button2_Click(object sender, EventArgs e)
        {            
            this.button_pay.Enabled = false;

            if (!copFilledCorrectly())
            {
                calculate();
                return;
            }
            //Проверить заполнены копейки или нет 



            double cash_money = Math.Round(Convert.ToDouble(txtB_cash_sum.Text.Replace(".", ",")), 2);
            double non_cash_money = Math.Round(Convert.ToDouble(get_non_cash_sum()), 2);
            double sertificate_money = Math.Round(Convert.ToDouble(sertificates_sum.Text), 2);
            double bonus_money = Math.Round(Convert.ToDouble(pay_bonus_many.Text.Replace(".", ",")), 2);

            double sum_on_document = Math.Round(Convert.ToDouble(pay_sum.Text.Replace(".", ",")), 2);

            double all_cash_non_cash = cash_money + non_cash_money + sertificate_money + bonus_money;

            //if (Math.Round(Convert.ToDouble(cash_sum.Text.Replace(".", ",")),2, MidpointRounding.ToEven) + Math.Round(Convert.ToDouble(get_non_cash_sum(0)),2, MidpointRounding.ToEven) + Math.Round(Convert.ToDouble(sertificates_sum.Text),2, MidpointRounding.ToEven) + Math.Round(Convert.ToDouble(pay_bonus_many.Text.Replace(".", ",")),2, MidpointRounding.ToEven) - Math.Round(Convert.ToDouble(pay_sum.Text.Replace(".", ",")),2, MidpointRounding.ToEven) < 0)
            //if ((Math.Round(all_cash_non_cash, 2) - Math.Round(sum_on_document, 2)) < 0)
            //MessageBox.Show("Всего оплат " + all_cash_non_cash);
            //MessageBox.Show("all_cash_non_cash - sum_on_document=" + (Math.Round(all_cash_non_cash, 2) - Math.Round(sum_on_document, 2)));
            if (Math.Round(all_cash_non_cash, 2) - Math.Round(sum_on_document, 2) < 0)
            {
                //MessageBox.Show("Общая сумма оплат  " + (cash_money + non_cash_money + sertificate_money + bonus_money));
                //double minus = (cash_money + non_cash_money + sertificate_money + bonus_money) - sum_on_document;
                //MessageBox.Show(minus.ToString());
                MessageBox.Show("Проверьте сумму внесенной оплаты");
                MessageBox.Show("Наличные" + Math.Round(Convert.ToDouble(txtB_cash_sum.Text.Replace(".", ",")), 2).ToString());
                MessageBox.Show("Карта " + Math.Round(Convert.ToDouble(get_non_cash_sum()), 2).ToString());
                MessageBox.Show("Сертификаты " + Math.Round(Convert.ToDouble(sertificates_sum.Text), 2).ToString());
                MessageBox.Show("Бонусы " + Math.Round(Convert.ToDouble(pay_bonus_many.Text.Replace(".", ",")), 2).ToString());
                MessageBox.Show("Общая сумма  " + Math.Round(Convert.ToDouble(pay_sum.Text.Replace(".", ",")), 2));

                return;
            }
          
            if (Convert.ToDouble(remainder.Text.Trim()) > 0)
            {
                if (cc.check_type.SelectedIndex != 0)
                {
                    MessageBox.Show(" Сумма возврата должна быть равно сумме оплаты ");
                    return;
                }
            }

            if (Convert.ToDouble(pay_bonus_many.Text) != 0)//При оплате бонусами бонусы не начисляются
            {
                bonus_on_document.Text = "0";
            }

            if (Convert.ToDouble(pay_bonus_many.Text) > 0)                 
            {
                if (Convert.ToDouble(non_cash_sum.Text) + Convert.ToDouble(sertificates_sum.Text) + Convert.ToDouble(pay_bonus_many.Text) > Convert.ToDouble(pay_sum.Text))
                {
                    MessageBox.Show("Сумма сертификатов + сумма по карте оплаты + сумма по бонусам превышает сумму чека ");
                    return;
                }
            }
            else
            {
                if (Convert.ToDouble(non_cash_sum.Text) + Convert.ToDouble(sertificates_sum.Text) > Convert.ToDouble(pay_sum.Text))
                {
                    MessageBox.Show(" Сумма сертификатов + сумма по карте оплаты превышает сумму чека ");
                    return;
                } 
            }
                      


            cc.listView_sertificates.Items.Clear();
            foreach (ListViewItem lvi in listView_sertificates.Items)
            {
                cc.listView_sertificates.Items.Add((ListViewItem)lvi.Clone());
            }

            MainStaticClass.write_event_in_log("Окно оплаты перед записью и закрытием документа ", "Документ чек", cc.numdoc.ToString());

            //Необходимо проверка на сумму документа где сумма всех форм оплаты равно сумме документа
            //Получаем общу сумму по оплате 
            Double _cash_summ_ = Convert.ToDouble(txtB_cash_sum.Text) - Convert.ToDouble(remainder.Text);
            //MessageBox.Show("Наличные " + _cash_summ_.ToString());
            Double _non_cash_summ_ = Math.Round(Convert.ToDouble(get_non_cash_sum()),2);
            //MessageBox.Show("Безнал " + _non_cash_summ_.ToString());
            Double _sertificates_sum_ = Convert.ToDouble(sertificates_sum.Text);
            //MessageBox.Show("Сертификаты " + _sertificates_sum_.ToString());
            //decimal _pay_bonus_many_ = Convert.ToDecimal((int)(Convert.ToInt32(pay_bonus_many.Text)/100));
            Double _pay_bonus_many_ = Convert.ToDouble(pay_bonus_many.Text);
            //MessageBox.Show("Бонусы " + _pay_bonus_many_.ToString());
            Double sum_of_the_document = cc.calculation_of_the_sum_of_the_document();
            //decimal sum_of_the_document = Math.Round(Convert.ToDecimal(pay_sum.Text.Replace(".", ",")), 2);
            //MessageBox.Show("Сумма документа " + sum_of_the_document.ToString());

            //if ((_non_cash_summ_ == 0) && (!MainStaticClass.fractional_exists(cc.listView1)))
            //{
            //    //if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema ==3) || (MainStaticClass.GetWorkSchema == 4))
            //    //{                    
            //    //MessageBox.Show(" Сумма документа до преобразования " + sum_of_the_document.ToString());
            //    //sum_of_the_document = (int)sum_of_the_document;
            //    //MessageBox.Show(" Сумма документа после преобразования к целому " + sum_of_the_document.ToString());                    
            //    //sum_of_the_document = Math.Round(sum_of_the_document,0,MidpointRounding.AwayFromZero);
            //    //MessageBox.Show(" Сумма документа после преобразования к целому " + sum_of_the_document.ToString());

            //    //sum_of_the_document = (int)Math.Floor(sum_of_the_document);

            //    if (sum_of_the_document.ToString().IndexOf(".") != -1)
            //    {
            //        string[] parts = sum_of_the_document.ToString().Split('.');
            //        sum_of_the_document = Double.Parse(parts[0]);
            //    }
            //    else if (sum_of_the_document.ToString().IndexOf(",") != -1)
            //    {
            //        string[] parts = sum_of_the_document.ToString().Split(',');
            //        sum_of_the_document = Double.Parse(parts[0]);
            //    }


            //    double sum_sum_at_a_discount = get_sum_sum_at_a_discount();

            //    if (Math.Abs(sum_sum_at_a_discount - sum_of_the_document) > 0 && sum_sum_at_a_discount != 0)
            //    {
            //        sum_of_the_document = sum_sum_at_a_discount;
            //    }

            //    //sum_of_the_document = Math.Truncate(sum_of_the_document);
            //    //MessageBox.Show(" Сумма документа после преобразования к целому " + sum_of_the_document.ToString());
            //    //sum_of_the_document = Math.Round(sum_of_the_document,0);
            //    //}
            //}

            if ((MainStaticClass.GetWorkSchema == 1)||(MainStaticClass.GetWorkSchema == 3) || (MainStaticClass.GetWorkSchema == 4))
            {
                if (Math.Round(sum_of_the_document, 2) != Math.Round((_cash_summ_ + _non_cash_summ_ + _sertificates_sum_ + _pay_bonus_many_), 2))
                {

                    MessageBox.Show(" Повторно внесите суммы оплаты, обнаружено несхождение в окне оплаты ");
                    MessageBox.Show("Сумма документа = " + sum_of_the_document.ToString() + " а сумма оплат = " + (_cash_summ_ + _non_cash_summ_ + _sertificates_sum_ + _pay_bonus_many_).ToString());
                    MessageBox.Show("Сумма наличные = " + _cash_summ_.ToString());
                    MessageBox.Show("Сумма карта оплаты = " + _non_cash_summ_.ToString());
                    MessageBox.Show("Сумма сертификатов = " + _sertificates_sum_.ToString());
                    MessageBox.Show("Сумма бонусов = " + _pay_bonus_many_.ToString());

                    return;
                }
            }
            //else if (MainStaticClass.GetWorkSchema == 2)
            //{
            //    //if (sum_of_the_document != _cash_summ_ + _non_cash_summ_ + _sertificates_sum_ + _pay_bonus_many_)
            //    if (Math.Round(sum_of_the_document, 2) != Math.Round((_cash_summ_ + _non_cash_summ_ + _sertificates_sum_ + _pay_bonus_many_), 2))
            //    {

            //        MessageBox.Show(" Повторно внесите суммы оплаты, обнаружено несхождение в окне оплаты ");
            //        MessageBox.Show("Сумма документа = " + sum_of_the_document.ToString() + " а сумма оплат = " + (_cash_summ_ + _non_cash_summ_ + _sertificates_sum_ + _pay_bonus_many_).ToString());
            //        MessageBox.Show("Сумма наличные = " + _cash_summ_.ToString());
            //        MessageBox.Show("Сумма карта оплаты = " + _non_cash_summ_.ToString());
            //        MessageBox.Show("Сумма сертификатов = " + _sertificates_sum_.ToString());
            //        MessageBox.Show("Сумма бонусов = " + _pay_bonus_many_.ToString());

            //        return;
            //    }

            //}

            //здесь перед записью еще проверка процессингового центра 
            //if (cc.client.Tag != null)
            //{
            //    //if ((cc.check_bonus_is_on()||MainStaticClass.GetWorkSchema==2))
            //    //{
            //    //    if (!continue_sales())
            //    //    {
            //    //        return;
            //    //    }
            //    //}
            //}

            //Если это возврат то необходимо проверить сумму по каждой форме оплаты 
            if (cc.check_type.SelectedIndex == 1)
            {
                if (!MainStaticClass.validate_cash_sum_non_cash_sum_on_return(cc.id_sale, _cash_summ_, _non_cash_summ_))
                {
                    return;
                }
            }

            it_is_paid();
        }

        private double get_sum_sum_at_a_discount()
        {
            double result = 0;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            NpgsqlCommand command = null;
            try
            {
                //string query = "SELECT FLOOR(SUM(sum_at_a_discount)) FROM public.checks_table WHERE document_number = " + cc.numdoc.ToString();
                string query = "SELECT SUM(sum_at_a_discount) FROM public.checks_table WHERE document_number = " + cc.numdoc.ToString();
                conn.Open();
                command = new NpgsqlCommand(query, conn);
                result = Convert.ToDouble(command.ExecuteScalar());
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при получении целой части суммы документа  get_sum_sum_at_a_discount " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении целой части суммы документа  get_sum_sum_at_a_discount " + ex.Message);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                if (command != null)
                {
                    command.Dispose();
                }
            }

            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cc.check_type.SelectedIndex == 0)
            {
                MessageBox.Show("Список введённых подарков будет очищен." +
                    "При следующем переходе в окно оплаты необходимо повторить их ввод, если программа предложит это сделать");//.\r\n"+
                    //"ТАК ЖЕ ОЧЕНЬ ВАЖНО ПРИ ОПЛАТЕ ТЕРМИНАЛОМ ЗАПОЛНИТЬ СУММУ ПО КНОПКЕ, ЧТОБЫ ЗАПОЛНИЛИСЬ КОПЕЙКИ.");
            }
            cc.cancel_action();
            cc.listView_sertificates.Items.Clear();
            foreach (ListViewItem lvi in listView_sertificates.Items)
            {
                cc.listView_sertificates.Items.Add((ListViewItem)lvi.Clone());
            }
            //записать в лог что кассир вернулся в документ 
            MainStaticClass.write_event_in_log("Возврат в документ из окна оплата", "Документ чек",cc.numdoc.ToString());            
            this.Close();
        }

        private void InitializeComponent_CH()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.pay_sum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.remainder = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button_pay = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.non_cash_sum_kop = new System.Windows.Forms.TextBox();
            this.non_cash_sum = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.sertificates_sum = new System.Windows.Forms.TextBox();
            this.bonus_total_in_centr = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.bonus_on_document = new System.Windows.Forms.TextBox();
            this.txtB_cash_sum = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pay_bonus_many = new System.Windows.Forms.TextBox();
            this.pay_bonus = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(72, 185);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(354, 55);
            this.label1.TabIndex = 1;
            this.label1.Text = "СУММА ЧЕКА";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pay_sum
            // 
            this.pay_sum.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pay_sum.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pay_sum.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pay_sum.Enabled = false;
            this.pay_sum.Font = new System.Drawing.Font("Microsoft Sans Serif", 39.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pay_sum.ForeColor = System.Drawing.Color.Black;
            this.pay_sum.Location = new System.Drawing.Point(494, 180);
            this.pay_sum.MaxLength = 10;
            this.pay_sum.Multiline = true;
            this.pay_sum.Name = "pay_sum";
            this.pay_sum.ReadOnly = true;
            this.pay_sum.Size = new System.Drawing.Size(320, 55);
            this.pay_sum.TabIndex = 2;
            this.pay_sum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(72, 441);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(302, 55);
            this.label2.TabIndex = 3;
            this.label2.Text = "НАЛИЧНЫЕ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(72, 511);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(343, 55);
            this.label3.TabIndex = 5;
            this.label3.Text = "СДАЧА";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // remainder
            // 
            this.remainder.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.remainder.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.remainder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.remainder.Font = new System.Drawing.Font("Microsoft Sans Serif", 39.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.remainder.Location = new System.Drawing.Point(496, 511);
            this.remainder.MaxLength = 10;
            this.remainder.Multiline = true;
            this.remainder.Name = "remainder";
            this.remainder.ReadOnly = true;
            this.remainder.Size = new System.Drawing.Size(318, 55);
            this.remainder.TabIndex = 6;
            this.remainder.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.AutoSize = true;
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(76, 589);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(297, 77);
            this.button1.TabIndex = 7;
            this.button1.Text = "Вернуться в документ  (F5)";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_pay
            // 
            this.button_pay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_pay.AutoSize = true;
            this.button_pay.BackColor = System.Drawing.SystemColors.Control;
            this.button_pay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_pay.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_pay.ForeColor = System.Drawing.Color.Red;
            this.button_pay.Location = new System.Drawing.Point(529, 589);
            this.button_pay.Name = "button_pay";
            this.button_pay.Size = new System.Drawing.Size(285, 79);
            this.button_pay.TabIndex = 8;
            this.button_pay.Text = "Подтвердить оплату (F12)";
            this.button_pay.UseVisualStyleBackColor = false;
            this.button_pay.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.non_cash_sum_kop);
            this.panel1.Controls.Add(this.non_cash_sum);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.sertificates_sum);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.button_pay);
            this.panel1.Controls.Add(this.bonus_total_in_centr);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.bonus_on_document);
            this.panel1.Controls.Add(this.txtB_cash_sum);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.pay_sum);
            this.panel1.Controls.Add(this.pay_bonus_many);
            this.panel1.Controls.Add(this.remainder);
            this.panel1.Controls.Add(this.pay_bonus);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(912, 728);
            this.panel1.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(739, 372);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(18, 55);
            this.label9.TabIndex = 1;
            this.label9.Text = ",";
            this.label9.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // non_cash_sum_kop
            // 
            this.non_cash_sum_kop.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.non_cash_sum_kop.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.non_cash_sum_kop.Font = new System.Drawing.Font("Microsoft Sans Serif", 32.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.non_cash_sum_kop.Location = new System.Drawing.Point(760, 373);
            this.non_cash_sum_kop.MaxLength = 2;
            this.non_cash_sum_kop.Name = "non_cash_sum_kop";
            this.non_cash_sum_kop.Size = new System.Drawing.Size(54, 56);
            this.non_cash_sum_kop.TabIndex = 18;
            this.non_cash_sum_kop.Text = "0";
            this.non_cash_sum_kop.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // non_cash_sum
            // 
            this.non_cash_sum.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.non_cash_sum.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.non_cash_sum.Font = new System.Drawing.Font("Microsoft Sans Serif", 32.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.non_cash_sum.Location = new System.Drawing.Point(493, 373);
            this.non_cash_sum.MaxLength = 5;
            this.non_cash_sum.Name = "non_cash_sum";
            this.non_cash_sum.Size = new System.Drawing.Size(243, 56);
            this.non_cash_sum.TabIndex = 17;
            this.non_cash_sum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.SystemColors.Control;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(72, 373);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(403, 55);
            this.label8.TabIndex = 16;
            this.label8.Text = "КАРТА ОПЛАТЫ";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.SystemColors.Control;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(72, 310);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(398, 55);
            this.label7.TabIndex = 15;
            this.label7.Text = "СЕРТИФИКАТЫ";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sertificates_sum
            // 
            this.sertificates_sum.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.sertificates_sum.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.sertificates_sum.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sertificates_sum.Font = new System.Drawing.Font("Microsoft Sans Serif", 39.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.sertificates_sum.ForeColor = System.Drawing.Color.ForestGreen;
            this.sertificates_sum.Location = new System.Drawing.Point(494, 308);
            this.sertificates_sum.MaxLength = 10;
            this.sertificates_sum.Multiline = true;
            this.sertificates_sum.Name = "sertificates_sum";
            this.sertificates_sum.ReadOnly = true;
            this.sertificates_sum.Size = new System.Drawing.Size(320, 55);
            this.sertificates_sum.TabIndex = 14;
            this.sertificates_sum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // bonus_total_in_centr
            // 
            this.bonus_total_in_centr.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bonus_total_in_centr.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.bonus_total_in_centr.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.bonus_total_in_centr.Font = new System.Drawing.Font("Microsoft Sans Serif", 39.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bonus_total_in_centr.ForeColor = System.Drawing.Color.ForestGreen;
            this.bonus_total_in_centr.Location = new System.Drawing.Point(494, 112);
            this.bonus_total_in_centr.MaxLength = 10;
            this.bonus_total_in_centr.Multiline = true;
            this.bonus_total_in_centr.Name = "bonus_total_in_centr";
            this.bonus_total_in_centr.ReadOnly = true;
            this.bonus_total_in_centr.Size = new System.Drawing.Size(320, 55);
            this.bonus_total_in_centr.TabIndex = 8;
            this.bonus_total_in_centr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.bonus_total_in_centr.Visible = false;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(75, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(356, 55);
            this.label5.TabIndex = 7;
            this.label5.Text = "Бонусов всего";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label5.Visible = false;
            // 
            // bonus_on_document
            // 
            this.bonus_on_document.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bonus_on_document.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.bonus_on_document.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.bonus_on_document.Font = new System.Drawing.Font("Microsoft Sans Serif", 39.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bonus_on_document.ForeColor = System.Drawing.Color.ForestGreen;
            this.bonus_on_document.Location = new System.Drawing.Point(495, 45);
            this.bonus_on_document.MaxLength = 10;
            this.bonus_on_document.Multiline = true;
            this.bonus_on_document.Name = "bonus_on_document";
            this.bonus_on_document.ReadOnly = true;
            this.bonus_on_document.Size = new System.Drawing.Size(319, 55);
            this.bonus_on_document.TabIndex = 7;
            this.bonus_on_document.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.bonus_on_document.Visible = false;
            // 
            // cash_sum
            // 
            this.txtB_cash_sum.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtB_cash_sum.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.txtB_cash_sum.Font = new System.Drawing.Font("Microsoft Sans Serif", 32.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtB_cash_sum.Location = new System.Drawing.Point(495, 441);
            this.txtB_cash_sum.MaxLength = 10;
            this.txtB_cash_sum.Name = "cash_sum";
            this.txtB_cash_sum.Size = new System.Drawing.Size(319, 56);
            this.txtB_cash_sum.TabIndex = 16;
            this.txtB_cash_sum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(75, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(415, 42);
            this.label4.TabIndex = 6;
            this.label4.Text = "Бонусы по документу";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label4.Visible = false;
            // 
            // pay_bonus_many
            // 
            this.pay_bonus_many.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pay_bonus_many.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pay_bonus_many.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pay_bonus_many.Enabled = false;
            this.pay_bonus_many.Font = new System.Drawing.Font("Microsoft Sans Serif", 32.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pay_bonus_many.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.pay_bonus_many.Location = new System.Drawing.Point(679, 246);
            this.pay_bonus_many.MaxLength = 10;
            this.pay_bonus_many.Name = "pay_bonus_many";
            this.pay_bonus_many.Size = new System.Drawing.Size(135, 49);
            this.pay_bonus_many.TabIndex = 13;
            this.pay_bonus_many.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.pay_bonus_many.Visible = false;
            // 
            // pay_bonus
            // 
            this.pay_bonus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pay_bonus.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pay_bonus.Enabled = false;
            this.pay_bonus.Font = new System.Drawing.Font("Microsoft Sans Serif", 32.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.pay_bonus.Location = new System.Drawing.Point(493, 244);
            this.pay_bonus.MaxLength = 10;
            this.pay_bonus.Name = "pay_bonus";
            this.pay_bonus.Size = new System.Drawing.Size(180, 56);
            this.pay_bonus.TabIndex = 12;
            this.pay_bonus.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.pay_bonus.Visible = false;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.SystemColors.Control;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(72, 246);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(235, 55);
            this.label6.TabIndex = 11;
            this.label6.Text = "БОНУСЫ";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label6.Visible = false;
            // 
            // Pay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(912, 728);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Pay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Оплата";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

        private void checkBox_payment_by_sbp_CheckedChanged(object sender, EventArgs e)
        {
            if (cc.payment_by_sbp_sales)//Продажа была по СБП
            {
                if (checkBox_payment_by_sbp.CheckState != CheckState.Checked)
                {                    
                    non_cash_sum.Text = "0";
                    calculate();
                    MessageBox.Show("При продаже была оплата по сбп, при возврате должно быть так же!", "Контроль СБП");
                }
            }
        }
       
    }
}