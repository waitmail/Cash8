using System;
using System.Collections.Generic;
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
            numericUpDown_enter_quantity.ValueChanged += NumericUpDown_enter_quantity_ValueChanged;
            TextBox textBox = (TextBox)numericUpDown_enter_quantity.Controls[1];
            textBox.KeyPress += TextBox_KeyPress;
            numericUpDown_enter_quantity.Controls[0].Visible=false;
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if ((numericUpDown_enter_quantity.DecimalPlaces == 0)|| textBox.Text.Contains(","))
            {
                if ((e.KeyChar == '.')||(e.KeyChar == ','))
                {
                    e.Handled = true;
                    return;
                }
            }
                        
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
                    //if ((e.KeyChar == '.') || (e.KeyChar == ','))
                    //{
                    //    e.Handled = true;
                    //    return;
                    //}
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
        
        private void EnterQuantity_Shown(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)numericUpDown_enter_quantity.Controls[1];
            tb.SelectionStart = 0;
            tb.SelectionLength = tb.Text.Length;

            if (MainStaticClass.GetWeightAutomatically == 1)
            {
                if (numericUpDown_enter_quantity.DecimalPlaces == 3)
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
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if ((e.KeyCode == Keys.F12)||(numericUpDown_enter_quantity.DecimalPlaces==0 && e.KeyCode == Keys.Enter))
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

            if (numericUpDown_enter_quantity.Focused && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
            {
                e.Handled = true;
                return;
            }
        }
    }
}
