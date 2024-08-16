//using System;
//using System.Text;
//using System.Windows.Forms;
//using System.Net;
//using System.Threading.Tasks;
//using System.Xml.Serialization;
//using System.IO;
//using System.Collections.Generic;
//using System.Threading;



//namespace Cash8
//{
//    public partial class WaitNonCashPay : Form
//    {
//        private System.Windows.Forms.Timer timer;
//        private int timeElapsed;
//        public event Action<bool, AnswerTerminal> CommandCompleted;
//        public Cash_check cc;
//        public string Url;
//        public string Data;
//        private CancellationTokenSource cts;
//        private int timeout = 80;



//        public WaitNonCashPay()
//        {
//            InitializeComponent();
//            this.Load += WaitNonCashPay_Load;            
//        }

//        private void WaitNonCashPay_Load(object sender, EventArgs e)
//        {
//            timer = new System.Windows.Forms.Timer();
//            timer.Interval = 1000; // 1 second
//            timer.Tick += Timer_Tick;
//            timer.Start();
//            CommandCompleted += WaitNonCashPay_CommandCompleted;
//            cts = new CancellationTokenSource();

//            // Запуск асинхронной процедуры
//            //Task.Factory.StartNew(() => send_command_acquiring_terminal("your_url", "your_data", cts.Token)).ContinueWith(task => CommandCompleted?.DynamicInvoke(task.Result)); 

//            //Task.Factory.StartNew(() => send_command_acquiring_terminal(this.Url  , this.Data  , cts.Token)).ContinueWith(task => CommandCompleted?.DynamicInvoke(task.Result));
//            var task = Task.Factory.StartNew(() => send_command_acquiring_terminal(this.Url, this.Data, cts.Token));
//            //Установа таймаута 
//            if (!task.Wait(TimeSpan.FromSeconds(timeout)))
//            {
//                cts.Cancel();
//            }
//        }

//        private void WaitNonCashPay_CommandCompleted(bool arg1, AnswerTerminal arg2)
//        {
//            //timer.Stop();
//            //this.Close();
//        }

//        private void Timer_Tick(object sender, EventArgs e)
//        {
//            if (timeElapsed < timeout)
//            {
//                timeElapsed++;
//                progressBarNonCashPay.Invoke((MethodInvoker)delegate
//                {
//                    progressBarNonCashPay.Value = timeElapsed;
//                });
//            }
//            else
//            {
//                timer.Stop();
//                cts.Cancel();
//                this.Close();
//            }
//        }

//        public class AnswerTerminal
//        {
//            public string code_authorization { get; set; }
//            public string number_reference { get; set; }
//            public string сode_response_in_15_field { get; set; }
//            public string сode_response_in_39_field { get; set; }
//            public bool error { get; set; }
//            public int error_code { get; set; }


//            public AnswerTerminal()
//            {
//                number_reference = "";
//                code_authorization = "";
//            }
//        }


//        [XmlRoot(ElementName = "field")]
//        public class Field
//        {

//            [XmlAttribute(AttributeName = "id")]
//            public string Id { get; set; }

//            [XmlText]
//            public string Text { get; set; }
//        }

//        [XmlRoot(ElementName = "response")]
//        public class Response
//        {

//            [XmlElement(ElementName = "field")]
//            public List<Field> Field { get; set; }
//        }

//        public class CommandResult
//        {
//            public bool Status { get; set; }
//            public AnswerTerminal AnswerTerminal { get; set; } = new AnswerTerminal();
//        }

//        /// <summary>
//        /// Отправляет команду в эквайринг
//        /// терминал и возвращает результат
//        /// </summary>
//        /// <param name="Url"></param>
//        /// <param name="Data"></param>        
//        public Task<CommandResult> send_command_acquiring_terminal(string Url, string Data, CancellationToken cancellationToken)
//        {
//            var tcs = new TaskCompletionSource<CommandResult>();
//            var result = new CommandResult();

//            Task.Factory.StartNew(() =>
//            {
//                try
//                {
//                    var request = (HttpWebRequest)WebRequest.Create(Url);
//                    request.Method = "POST";
//                    request.ContentType = "text/xml; charset=windows-1251";
//                    byte[] byteArray = Encoding.GetEncoding("Windows-1251").GetBytes(Data);
//                    request.ContentLength = byteArray.Length;

//                    using (Stream dataStream = request.GetRequestStream())
//                    {
//                        dataStream.Write(byteArray, 0, byteArray.Length);
//                    }

//                    using (WebResponse response = request.GetResponse())
//                    {
//                        using (Stream responseStream = response.GetResponseStream())
//                        {
//                            using (StreamReader reader = new StreamReader(responseStream))
//                            {
//                                string responseContent = reader.ReadToEnd();

//                                XmlSerializer serializer = new XmlSerializer(typeof(Response));
//                                using (StringReader stringReader = new StringReader(responseContent))
//                                {
//                                    var test = (Response)serializer.Deserialize(stringReader);
//                                    foreach (Field field in test.Field)
//                                    {
//                                        if (field.Id == "39")
//                                        {
//                                            result.AnswerTerminal.сode_response_in_39_field = field.Text;
//                                            result.Status = field.Text.Trim() == "1";
//                                        }
//                                        else if (field.Id == "13")
//                                        {
//                                            result.AnswerTerminal.code_authorization = field.Text.Trim();
//                                        }
//                                        else if (field.Id == "14")
//                                        {
//                                            result.AnswerTerminal.number_reference = field.Text.Trim();
//                                        }
//                                        else if (field.Id == "15")
//                                        {
//                                            result.AnswerTerminal.сode_response_in_15_field = field.Text.Trim();
//                                        }
//                                        else if (field.Id == "90")
//                                        {
//                                            cc.recharge_note = field.Text.Trim();
//                                            int num_pos = cc.recharge_note.IndexOf("(КАССИР)");
//                                            if (num_pos > 0)
//                                            {
//                                                cc.recharge_note = cc.recharge_note.Substring(0, num_pos + 8);
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                    this.DialogResult = DialogResult.OK;
//                }
//                catch (WebException ex)
//                {
//                    result.Status = false;
//                    MessageBox.Show("Ошибка при оплате по карте: " + ex.Message, "Оплата по терминалу");
//                    result.AnswerTerminal.error = true;
//                    //timer.Stop();
//                    //this.Close();
//                    this.DialogResult = DialogResult.Cancel;
//                }
//                catch (Exception ex)
//                {
//                    result.Status = false;
//                    MessageBox.Show("Ошибка при оплате по карте: " + ex.Message, "Оплата по терминалу");
//                    result.AnswerTerminal.error = true;
//                    //timer.Stop();
//                    //this.Close();
//                    this.DialogResult = DialogResult.Cancel;
//                }

//                tcs.SetResult(result);
//            }, cancellationToken);

//            return tcs.Task;
//        }


//        private void OnCommandCompleted(CommandResult result)
//        {
//            // Обработка завершения команды
//            MessageBox.Show("Команда завершена. Статус: " + result.Status);
//        }
//    }
//}

//using System;
//using System.Text;
//using System.Windows.Forms;
//using System.Net;
//using System.Threading.Tasks;
//using System.Xml.Serialization;
//using System.IO;
//using System.Collections.Generic;
//using System.Threading;



//namespace Cash8
//{
//    public partial class WaitNonCashPay : Form
//    {
//        private System.Windows.Forms.Timer timer;
//        private int timeElapsed;
//        public event Action<bool, Cash8.Pay.AnswerTerminal> CommandCompleted;
//        public Cash_check cc;
//        public string Url;
//        public string Data;
//        private CancellationTokenSource cts;
//        private int timeout = 80;
//        public CommandResult commandResult = null;
//        private bool isOperationCompleted = false;




//        public WaitNonCashPay()
//        {
//            InitializeComponent();
//            this.Load += WaitNonCashPay_Load;
//            this.FormClosing += WaitNonCashPay_FormClosing;
//        }

//        private void WaitNonCashPay_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            if (!isOperationCompleted)
//            {
//                e.Cancel = true;
//            }
//        }

//        private void WaitNonCashPay_Load(object sender, EventArgs e)
//        {
//            timer = new System.Windows.Forms.Timer();
//            timer.Interval = 1000; // 1 second
//            timer.Tick += Timer_Tick;
//            timer.Start();
//            CommandCompleted += WaitNonCashPay_CommandCompleted;
//            cts = new CancellationTokenSource();

//            // Запуск асинхронной процедуры
//            RunSendCommandAsync(this.Url, this.Data, cts.Token).ContinueWith(t =>
//            {
//                if (t.IsFaulted)
//                {
//                    HandleTaskCompletion(false, t.Exception?.GetBaseException().Message, null);
//                }
//                else if (t.IsCanceled)
//                {
//                    HandleTaskCompletion(false, "Операция была отменена.", null);
//                }
//                else
//                {
//                    var result = t.Result;
//                    this.commandResult = result;
//                    HandleTaskCompletion(result.Status, null, result.AnswerTerminal);
//                }
//            }, TaskScheduler.FromCurrentSynchronizationContext());

//            // Установка таймаута
//            Task.Factory.StartNew(() =>
//            {
//                Thread.Sleep(TimeSpan.FromSeconds(timeout - 5));
//                cts.Cancel();
//            });
//        }


//        private Task<CommandResult> RunSendCommandAsync(string url, string data, CancellationToken cancellationToken)
//        {
//            var tcs = new TaskCompletionSource<CommandResult>();

//            Task.Factory.StartNew(() =>
//            {
//                var result = send_command_acquiring_terminal(url, data, cancellationToken).Result;
//                tcs.SetResult(result);
//            }, cancellationToken);

//            return tcs.Task;
//        }

//        private void HandleTaskCompletion(bool status, string errorMessage, Cash8.Pay.AnswerTerminal answerTerminal)
//        {
//            if (!string.IsNullOrEmpty(errorMessage))
//            {
//                MessageBox.Show(errorMessage);
//            }
//            else
//            {
//                CommandCompleted?.Invoke(status, answerTerminal);
//            }

//            // Дополнительная обработка завершения задачи
//            // Например, остановка таймера и закрытие формы
//            isOperationCompleted = true;
//            timer.Stop();
//            this.Close();
//        }



//        private void WaitNonCashPay_CommandCompleted(bool arg1, Cash8.Pay.AnswerTerminal arg2)
//        {
//            this.commandResult = new CommandResult();
//            this.commandResult.Status = arg1;
//            this.commandResult.AnswerTerminal = arg2;
//            //timer.Stop();
//            //this.Close();           
//            //MessageBox.Show("WaitNonCashPay_CommandCompleted");
//        }

//        //private void OnCommandCompleted(CommandResult result)
//        //{
//        //    // Обработка завершения команды
//        //    MessageBox.Show("Команда завершена. Статус: " + result.Status);
//        //}        

//        private void Timer_Tick(object sender, EventArgs e)
//        {
//            if (timeElapsed < timeout)
//            {
//                timeElapsed++;
//                if (progressBarNonCashPay.IsHandleCreated)
//                {
//                    progressBarNonCashPay.Invoke((MethodInvoker)delegate
//                    {
//                        progressBarNonCashPay.Value = timeElapsed;
//                    });
//                }
//            }
//            else
//            {
//                timer.Stop();
//                cts.Cancel();
//                this.Close();
//            }
//        }

//        [XmlRoot(ElementName = "field")]
//        public class Field
//        {

//            [XmlAttribute(AttributeName = "id")]
//            public string Id { get; set; }

//            [XmlText]
//            public string Text { get; set; }
//        }

//        [XmlRoot(ElementName = "response")]
//        public class Response
//        {

//            [XmlElement(ElementName = "field")]
//            public List<Field> Field { get; set; }
//        }

//        public class CommandResult
//        {
//            public bool Status { get; set; }
//            public Cash8.Pay.AnswerTerminal AnswerTerminal { get; set; } = new Pay.AnswerTerminal();
//        }

//        /// <summary>
//        /// Отправляет команду в эквайринг
//        /// терминал и возвращает результат
//        /// </summary>
//        /// <param name="Url"></param>
//        /// <param name="Data"></param>        
//        public Task<CommandResult> send_command_acquiring_terminal(string Url, string Data, CancellationToken cancellationToken)
//        {
//            var tcs = new TaskCompletionSource<CommandResult>();
//            var result = new CommandResult();

//            Task.Factory.StartNew(() =>
//            {
//                try
//                {
//                    var request = (HttpWebRequest)WebRequest.Create(Url);
//                    request.Method = "POST";
//                    request.ContentType = "text/xml; charset=windows-1251";
//                    byte[] byteArray = Encoding.GetEncoding("Windows-1251").GetBytes(Data);
//                    request.ContentLength = byteArray.Length;

//                    using (Stream dataStream = request.GetRequestStream())
//                    {
//                        dataStream.Write(byteArray, 0, byteArray.Length);
//                    }

//                    using (WebResponse response = request.GetResponse())
//                    {
//                        using (Stream responseStream = response.GetResponseStream())
//                        {
//                            using (StreamReader reader = new StreamReader(responseStream))
//                            {
//                                string responseContent = reader.ReadToEnd();

//                                XmlSerializer serializer = new XmlSerializer(typeof(Response));
//                                using (StringReader stringReader = new StringReader(responseContent))
//                                {
//                                    var test = (Response)serializer.Deserialize(stringReader);
//                                    foreach (Field field in test.Field)
//                                    {
//                                        if (field.Id == "39")
//                                        {
//                                            result.AnswerTerminal.сode_response_in_39_field = field.Text;
//                                            result.Status = field.Text.Trim() == "1";
//                                        }
//                                        else if (field.Id == "13")
//                                        {
//                                            result.AnswerTerminal.code_authorization = field.Text.Trim();
//                                        }
//                                        else if (field.Id == "14")
//                                        {
//                                            result.AnswerTerminal.number_reference = field.Text.Trim();
//                                        }
//                                        else if (field.Id == "15")
//                                        {
//                                            result.AnswerTerminal.сode_response_in_15_field = field.Text.Trim();
//                                        }
//                                        else if (field.Id == "90")
//                                        {
//                                            cc.recharge_note = field.Text.Trim();
//                                            int num_pos = cc.recharge_note.IndexOf("(КАССИР)");
//                                            if (num_pos > 0)
//                                            {
//                                                cc.recharge_note = cc.recharge_note.Substring(0, num_pos + 8);
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                    this.DialogResult = DialogResult.OK;
//                }
//                catch (WebException ex)
//                {
//                    result.Status = false;
//                    MessageBox.Show("Ошибка при оплате по карте: " + ex.Message, "Оплата по терминалу");
//                    result.AnswerTerminal.error = true;
//                    this.DialogResult = DialogResult.Cancel;
//                }
//                catch (Exception ex)
//                {
//                    result.Status = false;
//                    MessageBox.Show("Ошибка при оплате по карте: " + ex.Message, "Оплата по терминалу");
//                    result.AnswerTerminal.error = true;
//                    this.DialogResult = DialogResult.Cancel;
//                }

//                tcs.SetResult(result);
//            }, cancellationToken);
//            return tcs.Task;
//        }
//    }
//}

using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;

namespace Cash8
{
    public partial class WaitNonCashPay : Form
    {
        private System.Windows.Forms.Timer timer;
        private int timeElapsed;
        public event Action<bool, Cash8.Pay.AnswerTerminal> CommandCompleted;
        public Cash_check cc;
        public string Url;
        public string Data;
        private CancellationTokenSource cts;
        private int timeout = 80;
        public CommandResult commandResult = null;

        public WaitNonCashPay()
        {
            InitializeComponent();
            this.Load += WaitNonCashPay_Load;
            this.FormClosing += WaitNonCashPay_FormClosing;
        }

        private void WaitNonCashPay_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (!isOperationCompleted)
            //{
            //    e.Cancel = true;
            //}
        }

        private async void WaitNonCashPay_Load(object sender, EventArgs e)
        {
            CenterLabelOverProgressBar();
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += Timer_Tick;
            timer.Start();
            CommandCompleted += WaitNonCashPay_CommandCompleted;
            cts = new CancellationTokenSource();

            // Центрирование labelTimer относительно progressBarNonCashPay
            CenterLabelOverProgressBar();

            // Запуск асинхронной процедуры с таймаутом
            var commandTask = send_command_acquiring_terminal(this.Url, this.Data, cts.Token);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeout+1));

            var completedTask = await Task.WhenAny(commandTask, timeoutTask);

            if (completedTask == commandTask)
            {
                // Команда завершилась до истечения времени
                try
                {
                    var result = await commandTask;
                    this.commandResult = result;
                    HandleTaskCompletion(result.Status, null, result.AnswerTerminal);
                }
                catch (Exception ex)
                {
                    HandleTaskCompletion(false, ex.Message, null);
                }
            }
            else
            {
                // Таймаут истек
                cts.Cancel();
                HandleTaskCompletion(false, "Время ожидания истекло.", null);
            }
        }

        private void CenterLabelOverProgressBar()
        {
            labelTimer.Location = new System.Drawing.Point(
            progressBarNonCashPay.Location.X + (progressBarNonCashPay.Width - labelTimer.Width) / 2,
            progressBarNonCashPay.Location.Y + (progressBarNonCashPay.Height - labelTimer.Height) / 2 + 50
            );
            labelTimer.BringToFront();
        }

        private void HandleTaskCompletion(bool status, string errorMessage, Cash8.Pay.AnswerTerminal answerTerminal)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Ожидание ответа от терминала");
            }
            else
            {
                CommandCompleted?.Invoke(status, answerTerminal);
            }

            // Дополнительная обработка завершения задачи
            // Например, остановка таймера и закрытие формы
            timer.Stop();
            this.Close();
        }

        private void WaitNonCashPay_CommandCompleted(bool arg1, Cash8.Pay.AnswerTerminal arg2)
        {
            this.commandResult = new CommandResult();
            this.commandResult.Status = arg1;
            this.commandResult.AnswerTerminal = arg2;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (timeElapsed < timeout)
            {
                timeElapsed++;
                if (progressBarNonCashPay.IsHandleCreated)
                {
                    progressBarNonCashPay.Invoke((MethodInvoker)delegate
                    {
                        if (timeElapsed <= timeout)
                        {
                            progressBarNonCashPay.Value = timeElapsed;//(int)((double)timeElapsed / timeout * 100);
                            labelTimer.Text = $"{timeElapsed} сек."; // Обновление текста Label
                        }
                        //CenterLabelOverProgressBar(); // Центрирование Label
                    });
                }                
            }
            else
            {
                timer.Stop();
                cts.Cancel();
                this.Close();
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

        public class CommandResult
        {
            public bool Status { get; set; }
            public Cash8.Pay.AnswerTerminal AnswerTerminal { get; set; } = new Pay.AnswerTerminal();
        }

        /// <summary>
        /// Отправляет команду в эквайринг
        /// терминал и возвращает результат
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Data"></param>        
        public async Task<CommandResult> send_command_acquiring_terminal(string Url, string Data, CancellationToken cancellationToken)
        {
            var result = new CommandResult();

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "text/xml; charset=windows-1251";
                byte[] byteArray = Encoding.GetEncoding("Windows-1251").GetBytes(Data);
                request.ContentLength = byteArray.Length;

                using (Stream dataStream = await request.GetRequestStreamAsync())
                {
                    await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
                }

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            string responseContent = await reader.ReadToEndAsync();

                            XmlSerializer serializer = new XmlSerializer(typeof(Response));
                            using (StringReader stringReader = new StringReader(responseContent))
                            {
                                var test = (Response)serializer.Deserialize(stringReader);
                                foreach (Field field in test.Field)
                                {
                                    if (field.Id == "39")
                                    {
                                        result.AnswerTerminal.сode_response_in_39_field = field.Text;
                                        result.Status = field.Text.Trim() == "1";
                                    }
                                    else if (field.Id == "13")
                                    {
                                        result.AnswerTerminal.code_authorization = field.Text.Trim();
                                    }
                                    else if (field.Id == "14")
                                    {
                                        result.AnswerTerminal.number_reference = field.Text.Trim();
                                    }
                                    else if (field.Id == "15")
                                    {
                                        result.AnswerTerminal.сode_response_in_15_field = field.Text.Trim();
                                    }
                                    else if (field.Id == "90")
                                    {
                                        cc.recharge_note = field.Text.Trim();
                                        int num_pos = cc.recharge_note.IndexOf("(КАССИР)");
                                        if (num_pos > 0)
                                        {
                                            cc.recharge_note = cc.recharge_note.Substring(0, num_pos + 8);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (WebException ex)
            {
                result.Status = false;
                MessageBox.Show("Ошибка при оплате по карте: " + ex.Message, "Оплата по терминалу");
                result.AnswerTerminal.error = true;
                this.DialogResult = DialogResult.Cancel;
            }
            catch (Exception ex)
            {
                result.Status = false;
                MessageBox.Show("Ошибка при оплате по карте: " + ex.Message, "Оплата по терминалу");
                result.AnswerTerminal.error = true;
                this.DialogResult = DialogResult.Cancel;
            }

            return result;
        }
    }
}
