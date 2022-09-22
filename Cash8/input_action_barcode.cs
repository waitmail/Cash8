using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Npgsql;
using System.Text.RegularExpressions;

namespace Cash8
{
    public partial class Input_action_barcode : Form
    {
        /*Тип вызова этой формы
         * 1.Вызов для ввода акционного штрихкода (штрихкод акции)
         * 2.Вызов для ввода акционного штрихкода (штрихкод товара когда подарок)
         * 3.Вызов для ввода 
         * 4.Вызов для ввода штрихкода продавца консультанта
         * 5.вызов для ввод 4 последних цифр телефона 
         * 6.вызов для ввода QR - кода маркера товара
         */
        public Cash_check caller = null;
        //public ProcessingOfActions caller = null;

        public int call_type = 0;
        public int count = 0;
        public int num_doc = 0; //номер акционного документа по которму выдается подарок
        System.Windows.Forms.Timer input_barcode_timer = null;

        public Input_action_barcode()
        {
            this.TopMost = true;
            InitializeComponent();
            this.Load += new EventHandler(Input_action_barcode_Load);
        }

        private void input_barcode_MouseUp(object sender, MouseEventArgs e)
        {
            this.input_barcode.Text = "";
        }

        private void input_barcode_timer_Tick(object sender, EventArgs e)
        {
            input_barcode_timer.Stop();
            input_barcode.Text = "";
            //execute_enter(input_barcode.Text);
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            //if (call_type != 6)
            //{
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            //}
        }

        //private void input_barcode_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        //{
        //    if (call_type == 4)
        //    {
        //        if (!(Char.IsDigit(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
        //        {
        //            e.Handled = true;
        //        }

        //        if (!e.Handled)
        //        {
        //            if (input_barcode.Text.Trim().Length == 1)
        //            {
        //                input_barcode_timer.Start();
        //            }
        //        }
        //    }
        //}


        void Input_action_barcode_Load(object sender, EventArgs e)
        {
            if (call_type == 1)
            {
                authorization.Text = "Введите штрихкод(промокод), включающий акцию";
                this.input_barcode.MaxLength = 13;
            }
            else if (call_type == 2)
            {
                authorization.Text = "Введите штрихкод подарка";
                this.input_barcode.MaxLength = 13;
            }
            else if (call_type == 3)
            {
                authorization.Text = "Введите штрихкод администратора";
                this.input_barcode.MaxLength = 13;
            }
            else if (call_type == 4)
            {
                authorization.Text = "Введите штрихкод продавца";
                input_barcode_timer = new System.Windows.Forms.Timer();
                input_barcode_timer.Interval = 700;
                this.input_barcode_timer.Tick += new EventHandler(input_barcode_timer_Tick);
                this.input_barcode.MouseUp += new MouseEventHandler(input_barcode_MouseUp);
                this.input_barcode.MaxLength = 13;
            }
            else if (call_type == 5)
            {
                authorization.Text = "Введите последние 4 цифры номера телефона";
                this.input_barcode.MaxLength = 4;               
            }
            else if (call_type == 6)
            {
                authorization.Text = " Просканируйте код маркировки. Отказ - Esc ";
                //this.input_barcode.MaxLength = 100;
            }

            input_barcode.Focus();            
        }

        private void input_barcode_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (call_type == 1)
            {
                //Cash_check parent = ((Cash_check)this.Parent);

                if (e.KeyChar == 13)
                {
                    if (!(caller.chect_action(input_barcode.Text)))
                    {
                        MessageBox.Show("Акция с таким штрихкодом не найдена");
                    }
                    else
                    {
                        if (input_barcode.Text.Trim().Length > 4)
                        {
                            if (caller.action_barcode_list.IndexOf(input_barcode.Text) == -1)
                            {
                                caller.action_barcode_list.Add(input_barcode.Text);//Для обычных акций
                            }
                        }
                        else
                        {
                            if (caller.action_barcode_bonus_list.IndexOf(input_barcode.Text) == -1)
                            {
                                caller.action_barcode_bonus_list.Add(input_barcode.Text);//Для бонусных акций
                            }
                        }
                    }

                    caller.inpun_action_barcode = false;

                    //if (caller.chect_action(MainStaticClass.Barcode))
                    //{
                    //    if (caller.action_barcode_list.IndexOf(MainStaticClass.Barcode) == -1)
                    //    {
                    //        caller.action_barcode_list.Add(MainStaticClass.Barcode);
                    //    }
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Акция с таким штрихкодом не найдена");
                    //}

                    this.Close();
                }
            }
            else if (call_type == 2)//После сообщения о подарке ввод штрихкода товара
            {
                //Cash_check parent = ((Cash_check)this.Parent);
                if (e.KeyChar == 13)
                {
                    //int count = caller.listView1.Items.Count; это ошибка, количество позиций присваивается предварительно 
                    int count_string = caller.listView1.Items.Count;//это количество строк

                    if (this.input_barcode.Text.Trim() != "")
                    {
                        caller.find_barcode_or_code_in_tovar_action(this.input_barcode.Text, count, true, num_doc);
                    }

                    //Этот подарок всегда добавляется отдельной строкой

                    //if (count_string == caller.listView1.Items.Count)
                    //{
                    //    this.DialogResult = DialogResult.Cancel;
                    //}
                    //else
                    //{
                    //    this.DialogResult = DialogResult.OK;
                    //}
                    this.Close();

                }
            }
            else if (call_type == 3)//Проверка на удаление документа
            {
                if (e.KeyChar == 13)
                {
                    caller.inpun_action_barcode = false;
                    this.Close();
                }
            }
            //else if (call_type == 4)//Штрихкод продавца в данный момент не используется
            //{
            //    if (!(Char.IsDigit(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            //    {
            //        e.Handled = true;
            //    }

            //    if (!e.Handled)
            //    {
            //        if (input_barcode.Text.Trim().Length == 1)
            //        {
            //            input_barcode_timer.Start();
            //        }
            //    }

            //    if (e.KeyChar == 13)
            //    {
            //        if (input_barcode.Text.Trim().Length == 11)
            //        {
            //            caller.txtB_sales_assistant.Text = input_barcode.Text;
            //        }
            //        else
            //        {
            //            if (input_barcode.Text.Trim().Length == 0)
            //            {
            //                MessageBox.Show(" Введен некорректный штрихкод ");
            //            }
            //            else
            //            {
            //                MessageBox.Show(" Штрихкод не введен ");
            //            }
            //        }
            //        this.Close();
            //    }
            //}
            else if (call_type == 5)//Проверка на 4 последние цифры телефона 
            {
                if (e.KeyChar != 13)
                {
                    //MessageBox.Show("необходимо ввести 4 цифры ");
                    return;
                }
                if (input_barcode.Text.Trim().Length < 4)
                {
                    MessageBox.Show("Необходимо ввести 4 цифры ");
                    return;
                }
                int result = 0;
                string client = caller.client.Tag.ToString();
                NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM clients where right(phone, 4)='" + input_barcode.Text + "' AND code='" + caller.client.Tag.ToString() + "'";
                    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                    result = Convert.ToInt16(command.ExecuteScalar());
                    conn.Close();
                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show("Ошибка при проверке номера телефона " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при проверке номера телефона " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
                if (result != 1)
                {
                    MessageBox.Show("Введенные цифры не верны");
                    input_barcode.Text = "";
                    if (MainStaticClass.ckeck_failed_input_phone_on_client(caller.client.Tag.ToString()) > 2)
                    {
                        MessageBox.Show("Вы превысили число попыток(3) ввести последние 4 цифры номера телефона");
                        this.Close();
                    }
                    insert_record_failed_input_phone();
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }

            }
            else if (call_type == 6)//Проверка на длину кода маркировки
            {
                if (e.KeyChar != 13)
                {
                    return;
                }
                //длина строки маркера не должна быть меньше 31 символов
                //if (input_barcode.Text.Trim().Length < 42)
                //{
                //    MessageBox.Show("Длина строки кода маркера меньше 42 символа, это ошибка !!! ");
                //    input_barcode.Text = "";
                //    return;
                //}
                /////////////////////////////////////////////////ЭТО НАДО ВСЕ ПРОВЕРИТЬ !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //Здесь проверяем, на отсутствие символов кирилицы
                //Regex reg = new Regex(@"^([^а-яА-Я]+)$");
                //System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(e.KeyChar.ToString(), "[а-яА-Я]");
                Regex reg = new Regex("[а-яА-ЯёЁ]");
                if (reg.IsMatch(input_barcode.Text.Trim()))
                {
                    MessageBox.Show("Обнаружены кирилличиские символы,ПЕРЕКЛЮЧИТЕ ЯЗЫК ВВОДА НА АНГЛИЙСКИЙ И ПОВТОРИТЕ ВВОД КОДА МАРКИРОВКИ ЕЩЕ РАЗ");
                    input_barcode.Text = "";
                    return;
                }

                caller.qr_code = this.input_barcode.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            else if (e.KeyChar == 27)
            {
                if (call_type == 1)
                {
                    caller.inpun_action_barcode = false;
                }
                this.Close();
            }
        }

        private void insert_record_failed_input_phone()
        {
            bool errors=false;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "INSERT INTO failed_input_phone(client_code, datetime_input)  VALUES ('" + caller.client.Tag.ToString() + "','" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "');";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при определении количества попыток ввода неправильного номера телефона "+ex.Message);
                errors = true;               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при определении количества попыток ввода неправильного номера телефона "+ex.Message);
                errors = true;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            if (errors)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}
