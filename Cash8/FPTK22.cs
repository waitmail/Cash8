using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;


namespace Cash8
{
    public partial class FPTK22 : Form
    {
        public FPTK22()
        {
            InitializeComponent();
            sum_avans.KeyPress += new KeyPressEventHandler(sum_avans_KeyPress);
            incass.KeyPress += new KeyPressEventHandler(incass_KeyPress);
            this.Load += new EventHandler(FPTK22_Load);
        }

        private void FPTK22_Load(object sender, EventArgs e)
        {
            btn_ofd_exchange_status_Click(null, null);
            btn_have_internet_Click(null, null);
            get_summ_in_cashe_Click(null, null);
            btn_send_fiscal_Click(null, null);
            if (MainStaticClass.Code_right_of_user != 1)
            {
                get_summ_in_cashe.Enabled = false;
                x_report.Enabled = false;                
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
                MessageBox.Show("avans_Click"+ex.Message);
            }
            avans.Enabled = true;   
        }

        private void incass_Click(object sender, EventArgs e)
        {
            incass.Enabled = false;
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
                MessageBox.Show("incass_Click"+ex.Message);
            }
            incass.Enabled = true;
        }

        private void x_report_Click(object sender, EventArgs e)
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
                MessageBox.Show("x_report_Click"+ex.Message);
            }
        }

        private void z_report_Click(object sender, EventArgs e)
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
                MessageBox.Show("z_report_Click"+ex.Message);
            }
        }

        private void annul_check_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(FiscallPrintJason.delete_last_job());
            }
            catch(Exception ex)
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
                MessageBox.Show("print_last_check_Click"+ex.Message);
            }
        }
        
        private void get_summ_in_cashe_Click(object sender, EventArgs e)
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
                MessageBox.Show("get_summ_in_cashe_Click"+ex.Message);
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
    }
}
