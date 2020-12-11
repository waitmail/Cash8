using System;
using System.ComponentModel;
using System.Windows.Forms;

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


        public Cash_check cc = null;
               
        
        public Pay()
        {         
            InitializeComponent();
            this.cash_sum.SelectionStart = 0;
            this.non_cash_sum.SelectionStart = 0;
            this.pay_bonus.SelectionStart = 0;
                             
            this.KeyPreview = true;
            this.Load += new EventHandler(Pay_Load);
            this.cash_sum.KeyPress += new KeyPressEventHandler(cash_sum_KeyPress);            
            this.cash_sum.KeyUp += new KeyEventHandler(cash_sum_KeyUp);
            this.non_cash_sum.KeyPress += new KeyPressEventHandler(non_cash_KeyPress);
            this.non_cash_sum.KeyUp += new KeyEventHandler(non_cash_KeyUp);
            this.pay_bonus.KeyPress += new KeyPressEventHandler(pay_bonus_KeyPress);                      
            this.pay_bonus.KeyUp += new KeyEventHandler(pay_bonus_KeyUp);
            this.non_cash_sum_kop.KeyPress += new KeyPressEventHandler(non_cash_sum_kop_KeyPress);
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
        }

        void non_cash_sum_kop_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        public void set_kop_on_non_cash_sum_kop(string kop)
        {
            non_cash_sum_kop.Text = kop;
        }

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
                        else
                        {
                            this.non_cash_sum.Text = "0.00";
                        }
                    }
                    else
                    {
                        if (MainStaticClass.get_currency() == "руб.")
                        {
                            this.non_cash_sum.Text = "0";
                        }
                        else
                        {
                            this.non_cash_sum.Text = "0,00";
                        }
                    }
                }
                calculate();
                if (MainStaticClass.get_currency() == "руб.")
                {

                }
                else
                {
                    if (curpos_non_cash < 0)
                    {
                        non_cash_sum.SelectionStart = 0;
                        curpos_non_cash = 0;
                    }
                    else
                    {
                        non_cash_sum.SelectionStart = curpos_non_cash;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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


        private void non_cash_KeyPress(object sender, KeyPressEventArgs e)
        {
            

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
                MessageBox.Show(ex.Message);
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
                Int64 input_bonus = Convert.ToInt64(pay_bonus.Text);
                //Посчитать сумму документа - 10 рублей
                //Decimal bonus_total = Convert.ToDecimal(bonus_total_in_centr.Text);
                Int64 bonus_total = Convert.ToInt64(bonus_total_in_centr.Text);

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
                int const_remaind = (int)(cc.calculation_quantity_on_document() / 100) + 1;
                if (Convert.ToInt64(pay_bonus.Text) > (int)(Convert.ToDecimal(pay_sum.Text) - const_remaind))
                {
                    pay_bonus.Text = ((int)(Convert.ToDecimal(pay_sum.Text) - const_remaind)).ToString();
                }

                pay_bonus_many.Text = pay_bonus.Text;

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
        private double get_non_cash_sum(int variant)
        {
            double result = 0;

            if (variant == 0)
            {
                result += double.Parse(non_cash_sum.Text) + double.Parse(non_cash_sum_kop.Text) / 100;
                //result += double.Parse(non_cash_sum_kop.Text) / 100;
            }
            else
            {
                if (double.Parse(non_cash_sum.Text) != 0)
                {
                    result += double.Parse(non_cash_sum.Text) + double.Parse(non_cash_sum_kop.Text) / 100;
                }
            }
           
            return result;
        }

        private void calculate()
        {

            try
            {
                if (this.cash_sum.Text.Length == 0)
                {
                    if (numberDecimalSeparator() == ".")
                    {
                        this.cash_sum.Text = "0";
                        this.non_cash_sum.Text = "0";
                        this.sertificates_sum.Text = "0";

                    }
                    else
                    {
                        this.cash_sum.Text = "0";
                        this.non_cash_sum.Text = "0";
                        this.sertificates_sum.Text = "0";

                    }
                }

                this.cash_sum.Text = Convert.ToDouble(this.cash_sum.Text).ToString("F", System.Globalization.CultureInfo.CurrentCulture);
                if (Convert.ToDecimal(pay_bonus_many.Text) == 0)//Если нет бонусов то проверить 
                {
                    decimal total = Convert.ToDecimal(pay_sum.Text);
                    string kop = ((int)((total - (int)total) * 100)).ToString();
                    kop = (kop.Length == 2 ? kop : "0" + kop);
                    set_kop_on_non_cash_sum_kop(kop);
                }

                this.remainder.Text = Math.Round(
                    (double.Parse(cash_sum.Text) +
                    double.Parse(pay_bonus_many.Text) +
                    get_non_cash_sum(0) +
                    double.Parse(sertificates_sum.Text) - double.Parse(pay_sum.Text)), 2).ToString("F", System.Globalization.CultureInfo.CurrentCulture);

                if (double.Parse(cash_sum.Text.Replace(".", ",")) + double.Parse(non_cash_sum.Text) +
                    double.Parse(sertificates_sum.Text) + double.Parse(pay_bonus_many.Text) + double.Parse(non_cash_sum_kop.Text) / 100 - double.Parse(pay_sum.Text.Replace(".", ",")) < 0)
                {
                    this.button_pay.Enabled = false;
                }
                else
                {
                    //Здесь еще должна быть проверка на оплату бонусами если клиент обозначен по номеру телефона
                    if (Convert.ToInt32(pay_bonus_many.Text) > 0)
                    {
                        if (cc.client.Tag.ToString().Length == 11)
                        {
                            if (code_it_is_confirmed)
                            {
                                this.button_pay.Enabled = true;
                            }
                        }
                        else
                        {
                            this.button_pay.Enabled = true;
                        }
                    }
                    else
                    {
                        this.button_pay.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            int start = cash_sum.SelectionStart;
            string _sum_ = cash_sum.Text;
            cash_sum.Text = "";
            cash_sum.Text = _sum_;
            cash_sum.SelectionStart = start;
            cash_sum.Update();

        }
        
        private void cash_sum_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.cash_sum.Text.Length == 0)
                {
                    if (numberDecimalSeparator() == ".")
                    {
                        if (MainStaticClass.get_currency() == "руб.")//зарпет на ввод запятой и точек можно только цифры или бекспейс
                        {
                            this.cash_sum.Text = "0";
                        }
                        else
                        {
                            this.cash_sum.Text = "0.00";
                        }
                    }
                    else
                    {
                        if (MainStaticClass.get_currency() == "руб.")//зарпет на ввод запятой и точек можно только цифры или бекспейс
                        {
                            this.cash_sum.Text = "0";
                        }
                        else
                        {
                            this.cash_sum.Text = "0,00";
                        }                       
                    }
                }
                calculate();
                if (MainStaticClass.get_currency() == "руб.")
                {

                }
                else
                {
                    if (curpos < 0)
                    {
                        cash_sum.SelectionStart = 0;
                        curpos = 0;
                        //MessageBox.Show("0");
                    }
                    else
                    {
                        cash_sum.SelectionStart = curpos;
                        //MessageBox.Show("1");
                    }
                }
			}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            int start = cash_sum.SelectionStart;
            string _sum_ = cash_sum.Text;
            cash_sum.Text = "";
            cash_sum.Text = _sum_;
            cash_sum.SelectionStart = start;
            cash_sum.Update();
            if (MainStaticClass.get_currency() == "руб.")
            {

            }
            else
            {
                cash_sum.SelectionStart = curpos;
            }
        }      
        
        private void cash_sum_KeyPress(object sender, KeyPressEventArgs e)
        {
           
            if ((Char.IsDigit(e.KeyChar)))
            {
                if (firs_input)
                {
                    firs_input = false;
                    cash_sum.Text = e.KeyChar + cash_sum.Text.Substring(1, cash_sum.Text.Length - 1);
                    e.Handled = true;
                    cash_sum.SelectionStart = 1;
                    curpos = 1;
                }
            }
            if (MainStaticClass.get_currency() == "руб.")//зарпет на ввод запятой и точек можно только цифры или бекспейс
            {
                if (!(Char.IsDigit(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
                {
                    e.Handled = true;
                }                
            }
            if (!(Char.IsDigit(e.KeyChar)))
            {
                if ((e.KeyChar == '.') || (e.KeyChar == ','))
                {

                    cash_sum.SelectionStart = cash_sum.Text.IndexOf(MainStaticClass.NumberDecimalSeparator()) + 1;
                    curpos = cash_sum.SelectionStart; e.Handled = true;
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
                    curpos = cash_sum.SelectionStart;
                    curpos++;
                }
                else
                {
                    if (cash_sum.SelectionStart != 0)
                    {
                        if (cash_sum.Text.Substring(cash_sum.SelectionStart - 1, 1) == ".")
                        {
                            e.Handled = true;
                            cash_sum.SelectionStart -= 1;
                            curpos = cash_sum.SelectionStart;
                        }
                        else if ((cash_sum.SelectionStart == 2) && (!e.Handled))
                        {
                            curpos = 1;
                        }
                        else
                        {
                            curpos = cash_sum.SelectionStart - 1;
                        }
                    }

                }
            }

            this.cash_sum.Update();
        }

        private void Pay_Load(object sender, EventArgs e)
        {
            this.cash_sum.Focus();
            cash_sum.SelectionStart = 0;
            non_cash_sum.SelectionStart = 0;
            pay_bonus.SelectionStart = 0;
            this.panel1.Size = new System.Drawing.Size(SystemInformation.PrimaryMonitorSize.Width - 100, SystemInformation.PrimaryMonitorSize.Height - 100);
            this.Top = 0;
            this.Left = 0;
            this.Size = new System.Drawing.Size(SystemInformation.PrimaryMonitorSize.Width, SystemInformation.PrimaryMonitorSize.Height);           
            this.pay_bonus_many.Text = "0";
            this.pay_bonus.Text = "0";
            calculate();           
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            cc.Visible = true;
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
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
                if (cash_sum.Focused)
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
                if (cash_sum.Focused)
                {
                    if (curpos < cash_sum.Text.Length - 1)
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
                if (cash_sum.Focused)
                {                 
                    if (cash_sum.Text.Substring(cash_sum.SelectionStart, 1) == ".")
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        curpos--;
                    }
                }
            }
            if (e.KeyCode == Keys.F8)
            {
                if (cc.check_type.SelectedIndex == 0)
                {
                    InputSertificates i_s = new InputSertificates();
                    i_s.pay = this;
                    //i_s.Top = 1;
                    i_s.TopMost = true;
                    i_s.ShowDialog();
                    i_s.Dispose();
                    calculate();
                    cash_sum.Focus();
                }
            }
            if (e.KeyCode == Keys.F6)//попытка сделать доступным полее вода списания бонусов
            {
                if (!cc.check_bonus_is_on())
                {
                    return;
                }
                if (Convert.ToInt32(pay_bonus_many.Text) == 0)
                {
                    return;
                }
                InputCodeNumberPhone inputCodeNumberPhone = new InputCodeNumberPhone();
                inputCodeNumberPhone.code_client = cc.client.Tag.ToString();
                DialogResult dialogResult = inputCodeNumberPhone.ShowDialog();
                if (dialogResult == DialogResult.Yes)
                {
                    code_it_is_confirmed = true;                    
                }
                calculate();
                if (button_pay.Enabled)
                {
                    button2_Click(null, null);
                }
            }
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
                if (!continue_sales())
                {
                    return;
                }

                if ((Convert.ToDecimal(cash_sum.Text) - Convert.ToDecimal(remainder.Text)) < 0)
                {
                    MessageBox.Show("Ошибка при определении суммы наличных");
                    return;
                }
                //Получаем копейки которые необходимо распределить
                decimal total = Convert.ToDecimal(pay_sum.Text);
                decimal bonus = Convert.ToDecimal(pay_bonus_many.Text);

                //if (!MainStaticClass.get_use_debug())
                //{                    

                if (cc.client.Tag != null)
                {
                    //***********************************************                            
                    if (Convert.ToDecimal(pay_bonus_many.Text) > 0)
                    {
                        if (Convert.ToDecimal(non_cash_sum.Text) == 0)
                        {
                            cc.distribute(Convert.ToDecimal(pay_bonus_many.Text) + (total - (int)total), total);//теперь бонусы 
                        }
                        else
                        {
                            cc.distribute(Convert.ToDecimal(pay_bonus_many.Text), total);//теперь бонусы 
                        }
                    }
                    else
                    {

                        if (Convert.ToDecimal(non_cash_sum.Text) == 0)
                        {
                            cc.distribute(total - (int)total, total);//теперь бонусы 
                        }
                    }

                    //if (Convert.ToDecimal(pay_bonus_many.Text) > 0)
                    //{
                    //    if (Convert.ToDecimal(non_cash_sum.Text) == 0)
                    //    {                            
                    //        cc.distribute(total - (int)total, total);
                    //    }
                    //}
                    //else
                    //{

                    //    if (Convert.ToDecimal(non_cash_sum.Text) == 0)
                    //    {
                    //        cc.distribute(total - (int)total, total);
                    //    }
                    //}
                }
                else
                {                 

                    if (Convert.ToDecimal(non_cash_sum.Text) == 0)
                    {
                        cc.distribute(total - (int)total, total);//распределение без бонусов , здесь нет клиента нет бонусов
                    }

                }

                if (Convert.ToDecimal(pay_sum.Text) - (Convert.ToDecimal(cash_sum.Text) - Convert.ToDecimal(remainder.Text) + Convert.ToDecimal(sertificates_sum.Text) + Convert.ToDecimal(pay_bonus_many.Text) + Convert.ToDecimal(non_cash_sum.Text)) > 1)
                {
                    MessageBox.Show(" Неверно внесенные суммы ");
                    return;
                }


                //Получить сумму наличных
                //если это возврат и если сумма безнала меньше 1 тогда копейки прибаквить к наличным
                string sum_cash_pay = (Convert.ToDecimal(cash_sum.Text) - Convert.ToDecimal(remainder.Text)).ToString().Replace(",", ".");
                string non_sum_cash_pay = (get_non_cash_sum(1)).ToString().Replace(",", ".");


              


                if (cc.it_is_paid(cash_sum.Text, cc.calculation_of_the_sum_of_the_document().ToString().Replace(",", "."), remainder.Text.Replace(",", "."),
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
            else
            {

                string sum_cash_pay = (Convert.ToDecimal(cash_sum.Text) - Convert.ToDecimal(remainder.Text)).ToString().Replace(",", ".");
                string non_sum_cash_pay = (get_non_cash_sum(1)).ToString().Replace(",", ".");
                if (cc.check_type.SelectedIndex == 1)
                {
                    if (get_non_cash_sum(1) < 1)
                    {
                        sum_cash_pay = (Convert.ToDecimal(cash_sum.Text) - Convert.ToDecimal(remainder.Text) + Convert.ToDecimal(get_non_cash_sum(0))).ToString().Replace(",", ".");
                        non_sum_cash_pay = "0";
                    }
                }

                cc.sale_cancellation_Click(sum_cash_pay, non_sum_cash_pay);
                cc.closing = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool continue_sales()
        {
            bool result = true;

            if (MainStaticClass.PassPromo != "")//Пароль не пустой бонусная магазин включен в бнусную систему
            {
                if (cc.check_type.SelectedIndex == 0) // Это продажа
                {
                    if (cc.client.Tag != null)
                    {
                        //if ((cc.client.Tag.ToString().Trim().Length == 36) || (cc.client.Tag.ToString().Trim().Length == 11))//Это бонусная карта 
                        //if (cc.client.Tag.ToString().Trim().Length == 10) //Это бонусная карта 
                        //{
                        if (cc.check_bonus_is_on())
                        {
                            SentDataOnBonus.BuynewResponse buynewResponse = cc.get_bonus_on_document(pay_bonus_many.Text);
                            if (buynewResponse != null)
                            {
                                if (buynewResponse.res == "1")
                                {
                                    this.bonus_on_document.Text = ((int)(Convert.ToInt64(buynewResponse.bonusSum) / 100)).ToString();
                                    cc.id_transaction = buynewResponse.transactionId;
                                    cc.bonus_on_document = Convert.ToInt32(this.bonus_on_document.Text);
                                }
                                else
                                {
                                    get_description_errors_on_code(buynewResponse.res);
                                    result = false;
                                }
                            }
                            else // Если оплата бонусами и нет связи с процессинговым цетром, тогда отлуп 
                            {
                                if (Convert.ToInt32(pay_bonus_many.Text) > 0)
                                {
                                    MessageBox.Show(" Нет связи с процессинговым центром ");
                                    result = false;
                                }
                            }
                        }
                        //}
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// получить текстовое описание по коду 
        /// </summary>
        /// <param name="code_error"></param>
        private void get_description_errors_on_code(string code_answer)
        {
            switch (code_answer)
            {
                case "2":
                    MessageBox.Show("2. Неверный запрос ");
                    break;
                case "3":
                    MessageBox.Show("3. Клиент не найден ");
                    break;
                case "4":
                    MessageBox.Show("4. Недостаточно прав для операции ");
                    break;
                case "5":
                    MessageBox.Show("5. Карта не найдена ");
                    break;
                case "6":
                    MessageBox.Show("6. Операции с картой запрещены ");
                    break;
                case "7":
                    MessageBox.Show("7. Ошибочная операция с картой ");
                    break;
                case "8":
                    MessageBox.Show("8. Недостаточно средств ");
                    break;
                case "9":
                    MessageBox.Show("9. Транзакция не найдена(неверный transactionId) ");
                    break;
                case "10":
                    MessageBox.Show("10. Значение параметра выходит за границы ");
                    break;
                case "11":
                    MessageBox.Show("11. Id чека, сгенерированный кассой, не уникален ");
                    break;
                case "12":
                    MessageBox.Show("12. Артикул товара(SKU) не найден ");
                    break;
                case "13":
                    MessageBox.Show("13. Ошибка запроса ");
                    break;
                case "14":
                    MessageBox.Show("14. Ошибка базы данных ");
                    break;
                case "15":
                    MessageBox.Show("15. Транзакция отклонена ");
                    break;
                case "16":
                    MessageBox.Show("16. Транзакция уже существует ");
                    break;

                default:
                    Console.WriteLine(" Неизвестный ответ от процессингового центра ");
                    break;
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            
            this.button_pay.Enabled = false;

            if (Convert.ToDecimal(cash_sum.Text.Replace(".", ",")) + Convert.ToDecimal(get_non_cash_sum(0)) + Convert.ToDecimal(sertificates_sum.Text) + Convert.ToDecimal(pay_bonus_many.Text.Replace(".", ",")) - Convert.ToDecimal(pay_sum.Text.Replace(".", ",")) < 0)
            {
                MessageBox.Show("Проверьте сумму внесенной оплаты");
                return;
            }
          
            if (Convert.ToDecimal(remainder.Text.Trim()) > 0)
            {
                if (cc.check_type.SelectedIndex != 0)
                {
                    MessageBox.Show(" Сумма возврата должна быть равно сумме оплаты ");
                    return;
                }
            }

            if (Convert.ToInt32(pay_bonus_many.Text) != 0)//При оплате бонусами бонусы не начисляются
            {
                bonus_on_document.Text = "0";
            }

            if (Convert.ToDecimal(pay_bonus_many.Text) > 0)                 
            {
                if (Convert.ToDecimal(non_cash_sum.Text) + Convert.ToDecimal(sertificates_sum.Text) + Convert.ToDecimal(pay_bonus_many.Text) > Convert.ToDecimal(pay_sum.Text))
                {
                    MessageBox.Show("Сумма сертификатов + сумма по карте оплаты + сумма по бонусам превышает сумму чека ");
                    return;
                }
            }
            else
            {
                if (Convert.ToDecimal(non_cash_sum.Text) + Convert.ToDecimal(sertificates_sum.Text) > Convert.ToDecimal(pay_sum.Text))
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
            decimal _cash_summ_ = Convert.ToDecimal(cash_sum.Text) - Convert.ToDecimal(remainder.Text);
            decimal _non_cash_summ_ = Convert.ToDecimal(get_non_cash_sum(1));
            decimal _sertificates_sum_ = Convert.ToDecimal(sertificates_sum.Text);
            //decimal _pay_bonus_many_ = Convert.ToDecimal((int)(Convert.ToInt32(pay_bonus_many.Text)/100));
            decimal _pay_bonus_many_ = Convert.ToDecimal(pay_bonus_many.Text);

            decimal sum_of_the_document = cc.calculation_of_the_sum_of_the_document();

            if (_non_cash_summ_ == 0)
            {
                sum_of_the_document = (int)sum_of_the_document;
            }

            if (sum_of_the_document != (_cash_summ_ + _non_cash_summ_ + _sertificates_sum_+ _pay_bonus_many_))
            {
                MessageBox.Show(" Повторно внесите суммы оплаты, обнаружено несхождение в окне оплаты ");
                return;
            }

            //здесь перед записью еще проверка процессингового центра 
            if (cc.client.Tag != null)
            {
                if (cc.check_bonus_is_on())
                {
                    if (!continue_sales())
                    {
                        return;
                    }
                }
            }

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



        private void button1_Click(object sender, EventArgs e)
        {           
            cc.cancel_action();
            //записать в лог что кассир вернулся в документ 
            MainStaticClass.write_event_in_log("Возврат в документ из окна оплата", "Документ чек",cc.numdoc.ToString());            
            this.Close();
        }               
                
    }
}