using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cash8
{
    public partial class EnterQuantity : Form
    {
        public Cash_check check = null;


        public EnterQuantity()
        {
            InitializeComponent();
            this.Shown += EnterQuantity_Shown;
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

        private void NumericUpDown_enter_quantity_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown_enter_quantity.Value = Decimal.Round(numericUpDown_enter_quantity.Value, 3);
        }

        private void NumericUpDown_enter_quantity_KeyPress(object sender, KeyPressEventArgs e)
        {

            //if (e.KeyChar == 27)
            //{
            //    this.numericUpDown_enter_quantity.Visible = false;
            //    this.panel1.Visible = false;
            //    return;
            //}

            if (e.KeyChar == (char)(Keys.F1))
            {
                if (this.numericUpDown_enter_quantity.Value == 0)
                {
                    MessageBox.Show("Количество не может быть пустым");
                    return;
                }

                if (Convert.ToDecimal(check.listView1.SelectedItems[0].SubItems[3].Text) > this.numericUpDown_enter_quantity.Value)
                {
                    //MessageBox.Show("Не администраторам запрещено менять количество на меньшее");
                    ///////////////////////////////////////////////////////////////
                    if (MainStaticClass.Code_right_of_user != 1)
                    {
                        check.enable_delete = false;
                        Interface_switching isw = new Interface_switching();
                        isw.caller_type = 3;
                        isw.cc = check;
                        isw.not_change_Cash_Operator = true;
                        isw.ShowDialog();
                        isw.Dispose();

                        if (!check.enable_delete)
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
                        check.insert_incident_record(check.listView1.SelectedItems[0].SubItems[0].Text, Math.Round(Convert.ToDecimal(check.listView1.SelectedItems[0].SubItems[3].Text) - this.numericUpDown_enter_quantity.Value, 2, MidpointRounding.ToEven).ToString().Replace(",", "."), "1", reasons.reason);
                    }
                    else
                    {
                        return;
                    }

                }
                else if (Convert.ToDecimal(check.listView1.SelectedItems[0].SubItems[3].Text) < this.numericUpDown_enter_quantity.Value)
                {

                    //Проверка на сертификат 
                    if (check.its_sertificate(check.listView1.SelectedItems[0].SubItems[0].Text.Trim()))
                    {
                        MessageBox.Show("Каждый сертификат продается отдельной строкой");
                        return;
                    }
                }

                check.listView1.SelectedItems[0].SubItems[3].Text = Convert.ToDecimal(this.numericUpDown_enter_quantity.Value).ToString();
                //check.recalculate_all();
                //check.calculation_of_the_sum_of_the_document();
                //this.numericUpDown_enter_quantity.Visible = false;
                //this.panel1.Visible = false;
                //this.listView1.Select();
                //this.listView1.Items[this.listView1.SelectedIndices[0]].Selected = true;
                check.write_new_document("0", "0", "0", "0", false, "0", "0", "0", "0");
            }

            //check.SendDataToCustomerScreen(1, 0, 1);
        }


        private void EnterQuantity_Shown(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)numericUpDown_enter_quantity.Controls[1];
            tb.SelectionStart = 0;
            tb.SelectionLength = 1;

            if (MainStaticClass.GetWeightAutomatically == 1)
            {
                Dictionary<double, int> frequencyMap = new Dictionary<double, int>();
                if (MessageBox.Show("Ввод веса будет из весов ? ", "Истоник веса", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    double weigt = 0;
                    weigt = MainStaticClass.GetWeight();
                    if (weigt > 0)
                    {
                        this.numericUpDown_enter_quantity.Value = Convert.ToDecimal(weigt);
                    }
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                if (numericUpDown_enter_quantity.Value == 0)
                {
                    MessageBox.Show(" Необходимо ввести количество ");
                    return;
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
        }
    }
}
