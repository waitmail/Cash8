using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace Cash8
{

    class Work_Mini_FP_6
    {

        private System.Timers.Timer timer = new System.Timers.Timer();//Таймер для печати фискального принтера

        public Work_Mini_FP_6()
        {

        }

        /*Печать нулевого чека делается комбинацией двух функция
         * PrintSvcMsg  и CloseSvcMsg 
        */

        [DllImport("libminifp6", EntryPoint = "PrintSvcMsg", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool PrintSvcMsg(int i, IntPtr message);
        [DllImport("libminifp6", EntryPoint = "CloseSvcMsg", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CloseSvcMsg(int i);

        //Функция Открытия денежного ящика
        [DllImport("libminifp6", EntryPoint = "OpenBox", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool OpenBox(int i);

        //Печать строки коментария
        [DllImport("libminifp6", EntryPoint = "Comment", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Comment(int i, IntPtr s, int checkType);

        //Печать копии последнего чека
        [DllImport("libminifp6", EntryPoint = "PrintCopy", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool PrintCopy(int i);
        //Функция получение содержимого денежного ящика
        [DllImport("libminifp6", EntryPoint = "GetSumBox", CallingConvention = CallingConvention.Cdecl)]
        private static extern double GetSumBox(int i);
        //Функция внесения аванса в кассовый аппарат          
        [DllImport("libminifp6", EntryPoint = "AVANS", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Avans(int i, double sum);
        //Функция инкассасации
        [DllImport("libminifp6", EntryPoint = "INCASS", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Incass(int i, double sum);
        //Текущий х-отчет
        [DllImport("libminifp6", EntryPoint = "DayReport", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool DayReport(int i);
        //Суточный z-отчет
        [DllImport("libminifp6", EntryPoint = "DayClrReport", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool DayClrReport(int i);
        //Продажа товара
        [DllImport("libminifp6", EntryPoint = "Sale", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Sale(int i, IntPtr code, IntPtr name, double qty, double price, int taxGrp, bool show);
        //Возврат товара
        [DllImport("libminifp6", EntryPoint = "Disburse", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Disburse(int i, IntPtr code, IntPtr name, double qty, double price, int taxGrp, bool show);
        //Оплата товара
        [DllImport("libminifp6", EntryPoint = "Pay", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Pay(int i, double sum, int kind, bool show, out double remainder);
        //Анулирование чека
        [DllImport("libminifp6", EntryPoint = "AnnulCheck", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool AnnulCheck(int i);
        //Получить последнюю ошибку
        [DllImport("libminifp6", EntryPoint = "GetLastErr", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetLastErr();

        [DllImport("libminifp6", EntryPoint = "PeriodicReport", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool PeriodicReport(int i, IntPtr BeginDate, IntPtr EndDate);

        //        StrType:=1;				// «тип строки»  ~  последняя строка чека
        //Str:= ‘СПАСИБО ЗА ПОКУПКУ’;	// новое значение последней строки чека
        //Dh:=True;				// двойная высота шрифта
        //Dw:=True;				// двойная ширина шрифта

        [DllImport("libminifp6", EntryPoint = "SetStrCheck", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SetStrCheck(int i, int str_type, IntPtr str, bool h, bool w);


        public bool periodic_report(int i, string begin_date, string end_date)
        {
            byte[] arr = Encoding.UTF8.GetBytes(begin_date);
            GCHandle _begin_date_ = GCHandle.Alloc(arr, GCHandleType.Pinned);  // зафиксировать в памяти
            IntPtr intptr_begin_date = _begin_date_.AddrOfPinnedObject();   // и взять его адрес 

            arr = Encoding.UTF8.GetBytes(end_date);
            GCHandle _end_date_ = GCHandle.Alloc(arr, GCHandleType.Pinned);  // зафиксировать в памяти
            IntPtr intptr_end_date = _end_date_.AddrOfPinnedObject();   // и взять его адрес 

            bool result = PeriodicReport(i, intptr_begin_date, intptr_end_date);

            _begin_date_.Free();
            intptr_begin_date = IntPtr.Zero;
            _end_date_.Free();
            intptr_end_date = IntPtr.Zero;
            GC.Collect();

            if (!result)
            {
                MessageBox.Show("Не удалось распечатать z отчет за период с " + begin_date + " по " + end_date + " , код ошибки = " + Marshal.PtrToStringAuto(GetLastErr()));
                return result;
            }

            return result;
        }


        public bool print_copy(int i)
        {
            if (!PrintCopy(i))
            {
                MessageBox.Show("Не удалось распечатать копию последнего чека, код ошибки = " + Marshal.PtrToStringAuto(GetLastErr()));
                return false;
            }
            return true;
        }

        /*Печать нулевого чека осуществляется печатью парой функций
         * печать служебного сообщения и собсвенно закрытие служебного сообщения
         * 
         */
        public bool print_zero_check(int i, string message)
        {
            bool error = false;
            byte[] arr = Encoding.UTF8.GetBytes(message);
            GCHandle _message_ = GCHandle.Alloc(arr, GCHandleType.Pinned);  // зафиксировать в памяти
            IntPtr intptr_message = _message_.AddrOfPinnedObject();   // и взять его адрес 
            if (PrintSvcMsg(i, intptr_message))
            {
                if (!CloseSvcMsg(i))
                {
                    MessageBox.Show("Не удалось закрыть чек служебное сообщение, код ошибки = " + Marshal.PtrToStringAuto(GetLastErr()));
                    error = true;
                }
            }
            else
            {
                MessageBox.Show("Не удалось распечатать чек служебное сообщение, код ошибки = " + Marshal.PtrToStringAuto(GetLastErr()));
                error = true;
            }

            intptr_message = IntPtr.Zero;
            _message_.Free();
            GC.Collect();
            return error;
        }

        public bool open_box(int i)
        {
            return OpenBox(i);
        }

        public bool annul_check(int i)
        {
            if (!AnnulCheck(i))
            {
                MessageBox.Show("Не удалось анулировать чек, код ошибки = " + Marshal.PtrToStringAuto(GetLastErr()));
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool day_report(int i)
        {
            bool result = DayReport(i);
            if (!result)
            {
                MessageBox.Show("Не удалось распечатать х-отчет, код ошибки = " + Marshal.PtrToStringAuto(GetLastErr()));
            }
            return result;
        }

        public bool day_clr_report(int i)
        {
            bool result = DayClrReport(i);
            if (!result)
            {
                MessageBox.Show("Не удалось распечатать z-отчет, код ошибки = " + Marshal.PtrToStringAuto(GetLastErr()));
            }
            return result;
        }
        /*i    - Номер COM порта
         * sum - сумма аванса   
         */
        public bool avans(int i, double sum)
        {
            bool result = Avans(i, sum);
            if (!result)
            {
                MessageBox.Show("Не удалось внести аванс код ошибки = " + Marshal.PtrToStringAuto(GetLastErr()));
            }
            return result;
        }

        public bool incass(int i, double sum)
        {
            if (!to_check_up_sum_in_box(i, sum))
            {
                return false;
            }
            bool result = Incass(i, sum);
            if (!result)
            {
                MessageBox.Show("Не удалось произвести инкассацию = " + Marshal.PtrToStringAuto(GetLastErr()));
            }
            return result;
        }

        /*эта функция проверяет хватает ли денег в денежном ящике для произведения инкассации 
         * или возврата товара 
         */
        private bool to_check_up_sum_in_box(int i, double sum)
        {
            double result = GetSumBox(i);
            if (result == -1)
            {
                MessageBox.Show("Не удалось определить сумму в денежном ящике", "Проверка наличия средств в денежном ящике");
                return false;
            }
            if (sum > result)
            {
                MessageBox.Show("Сумма наличных в денежном ящике недостаточна для выполнения операции", "Проверка наличия средств в денежном ящике");
                return false;
            }
            return true;
        }

        private bool set_str_check(int i, string s)
        {
            byte[] arr = Encoding.UTF8.GetBytes(s);
            //byte[] arr2 = new byte[arr.Length + 1];
            //arr.CopyTo(arr2, 0);
            //arr2[arr.Length] = 0;//Добавить нолик завершение строки

            GCHandle shapka = GCHandle.Alloc(arr, GCHandleType.Pinned);  // зафиксировать в памяти
            IntPtr intptr_shapka = shapka.AddrOfPinnedObject();   // и взять его адрес 
            bool result = SetStrCheck(1, i, intptr_shapka, true, true);

            if (!result)
            {
                MessageBox.Show("Неудачная попытка установить текст шапки или заголовка , код ошибки " + Marshal.PtrToStringAuto(GetLastErr()));
            }

            intptr_shapka = IntPtr.Zero;
            shapka.Free();
            arr = null;
            GC.Collect();
            return result;
        }

        private bool comment(int i, string s, int check_type)
        {
            byte[] arr = Encoding.UTF8.GetBytes(s);
            GCHandle shapka = GCHandle.Alloc(arr, GCHandleType.Pinned);  // зафиксировать в памяти
            IntPtr intptr_shapka = shapka.AddrOfPinnedObject();   // и взять его адрес 
            bool result = Comment(i, intptr_shapka, check_type);

            if (!result)
            {
                MessageBox.Show("Неудачная попытка вывести сообщение оскидке, код ошибки " + Marshal.PtrToStringAuto(GetLastErr()));
            }

            intptr_shapka = IntPtr.Zero;
            shapka.Free();
            arr = null;
            GC.Collect();
            return result;
        }


        public void fiscall_print_sale(int i, ListViewItem[] listViewItemCollection, double sum, Int64 num_check, string discount)
        {

            //ListView print_listview=new ListView(listView1.Items.Count-1);
            //listView1.Items.CopyTo(print_listview, 0);
            bool error = false;
            //int co
            set_str_check(0, "Чистый дом");
            set_str_check(1, "Спасибо за покупку");

            open_box(i);
            //MessageBox.Show(discount.ToString());

            foreach (System.Windows.Forms.ListViewItem lvi in listViewItemCollection)
            {
                if (Convert.ToDecimal(lvi.SubItems[6].Text) == 0)
                {
                    continue;
                }
                //Получение ндс без ндс
                //Console.WriteLine("1 "+ lvi.SubItems[0].Text.Trim()+","+ lvi.SubItems[1].Text.Trim()+","+ Convert.ToDouble(lvi.SubItems[2].Text.Trim())+","+ Convert.ToDouble(lvi.SubItems[6].Text.Trim())+", 1, true");
                //if (!sale        (1,    lvi.SubItems[0].Text.Trim(),     lvi.SubItems[1].Text.Trim(),     Convert.ToDouble(lvi.SubItems[2].Text.Trim()),     Convert.ToDouble(lvi.SubItems[6].Text.Trim()),   1, true))
                string code = lvi.SubItems[0].Text.Trim();
                string name = lvi.SubItems[1].Text.Trim();
                if (name.Length > 50)
                {
                    name = name.Substring(0, 50);
                }
                double qty = Convert.ToDouble(lvi.SubItems[2].Text.Trim());
                double sum_str = Convert.ToDouble(lvi.SubItems[4].Text.Trim());

                if (!sale(i, code, name, qty, sum_str, 1, true))
                {
                    MessageBox.Show("Неудачная попытка напечатать чек , код ошибки " + Marshal.PtrToStringAuto(GetLastErr()));
                    error = true;
                    break;
                }
            }
            double remainder = 0;
            if (!error)
            {
                if (discount.Length > 0)
                {
                    if (Convert.ToDecimal(discount) > 0)
                    {
                        comment(i, "Скидка " + discount.Replace(",", ".") + " грн.", 0);
                    }
                }
                if (!Pay(1, Convert.ToDouble(sum), 3, true, out remainder))
                {
                    MessageBox.Show("Неудачная попытка оплатить чек , код ошибки " + Marshal.PtrToStringAuto(GetLastErr()));
                }
                else
                {
                    MainStaticClass.Result_Fiscal_Print = true;
                    set_str_check(1, "");
                }
            }
            GC.Collect();
        }

        public void fiscall_print_disburse(int i, ListViewItem[] listViewItemCollection, double sum)
        {
            // MessageBox.Show("Начало");
            //Начало

            if (!to_check_up_sum_in_box(i, sum))
            {
                return;
            }

            open_box(i);
            bool error = false;
            //int co
            //MessageBox.Show("Шапка");
            set_str_check(0, "Чистый дом");
            set_str_check(1, "Возврат товара");
            //MessageBox.Show("Строки");

            foreach (System.Windows.Forms.ListViewItem lvi in listViewItemCollection)
            {
                if (Convert.ToDecimal(lvi.SubItems[6].Text) == 0)
                {
                    continue;
                }
                //Получение ндс без ндс
                //Console.WriteLine("1 "+ lvi.SubItems[0].Text.Trim()+","+ lvi.SubItems[1].Text.Trim()+","+ Convert.ToDouble(lvi.SubItems[2].Text.Trim())+","+ Convert.ToDouble(lvi.SubItems[6].Text.Trim())+", 1, true");
                //if (!sale        (1,    lvi.SubItems[0].Text.Trim(),     lvi.SubItems[1].Text.Trim(),     Convert.ToDouble(lvi.SubItems[2].Text.Trim()),     Convert.ToDouble(lvi.SubItems[6].Text.Trim()),   1, true))
                string code = lvi.SubItems[0].Text.Trim();
                string name = lvi.SubItems[1].Text.Trim();
                if (name.Length > 50)
                {
                    name = name.Substring(0, 50);
                }
                double qty = Convert.ToDouble(lvi.SubItems[2].Text.Trim());
                double sum_str = Convert.ToDouble(lvi.SubItems[4].Text.Trim());

                if (!disburse(i, code, name, qty, sum_str, 1, true))
                {
                    MessageBox.Show("Неудачная попытка напечатать чек , код ошибки " + Marshal.PtrToStringAuto(GetLastErr()));
                    error = true;
                    break;
                }
            }
            double remainder = 0;
            if (!error)
            {
                if (!Pay(i, Convert.ToDouble(sum), 3, true, out remainder))
                {
                    MessageBox.Show("Неудачная попытка выдачи, код ошибки " + Marshal.PtrToStringAuto(GetLastErr()));
                }
                else
                {
                    MainStaticClass.Result_Fiscal_Print = true;
                    set_str_check(i, "");
                }

            }
            GC.Collect();
        }

        public bool disburse(int i, string code, string name, double qty, double price, int taxGrp, bool show)
        {
            bool result;
            byte[] arr = Encoding.UTF8.GetBytes(code);
            byte[] arr2 = new byte[arr.Length + 1];
            arr.CopyTo(arr2, 0);
            arr2[arr.Length] = 0;//Добавить нолик завершение строки

            GCHandle _code_ = GCHandle.Alloc(arr2, GCHandleType.Pinned);  // зафиксировать в памяти
            IntPtr intptr_code = _code_.AddrOfPinnedObject();   // и взять его адрес 

            arr = Encoding.UTF8.GetBytes(name);
            arr2 = new byte[arr.Length + 1];
            arr.CopyTo(arr2, 0);
            arr2[arr.Length] = 0;//Добавить нолик завершение строки
            GCHandle _name_ = GCHandle.Alloc(arr2, GCHandleType.Pinned);  // зафиксировать в памяти
            IntPtr intptr_name = _name_.AddrOfPinnedObject();  // и взять его адрес 

            result = Disburse(i, intptr_code, intptr_name, qty, price, taxGrp, show);

            intptr_code = IntPtr.Zero;
            intptr_name = IntPtr.Zero;
            arr = null;
            _name_.Free();
            _code_.Free();
            GC.Collect();
            return result;
        }

        public bool sale(int i, string code, string name, double qty, double price, int taxGrp, bool show)
        {
            bool result;
            byte[] arr = Encoding.UTF8.GetBytes(code);
            byte[] arr2 = new byte[arr.Length + 1];
            arr.CopyTo(arr2, 0);
            arr2[arr.Length] = 0;//Добавить нолик завершение строки

            GCHandle _code_ = GCHandle.Alloc(arr2, GCHandleType.Pinned);  // зафиксировать в памяти
            IntPtr intptr_code = _code_.AddrOfPinnedObject();   // и взять его адрес 

            arr = Encoding.UTF8.GetBytes(name);
            arr2 = new byte[arr.Length + 1];
            arr.CopyTo(arr2, 0);
            arr2[arr.Length] = 0;//Добавить нолик завершение строки
            GCHandle _name_ = GCHandle.Alloc(arr2, GCHandleType.Pinned);  // зафиксировать в памяти
            IntPtr intptr_name = _name_.AddrOfPinnedObject();  // и взять его адрес 

            result = Sale(i, intptr_code, intptr_name, qty, price, taxGrp, show);

            intptr_code = IntPtr.Zero;
            intptr_name = IntPtr.Zero;
            arr = null;
            _name_.Free();
            _code_.Free();
            GC.Collect();
            return result;
        }

    }
}
