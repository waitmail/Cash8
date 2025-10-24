using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using ZXing;

namespace Cash8
{
    class KitchenPrinter
    {

        // Константы для ограничения длины строк
        private const int MAX_CHARS_PER_LINE = 50;  // Максимальная длина одной строки
        private const int MAX_LINES = 2;            // Максимальное количество строк для названия

        public class ReceiptData
        {
            // Заголовок и информация о магазине
            public string Header { get; set; } = "ОНЛАЙН-КАССА";
            public string Logo { get; set; } = "АТОЛ";
            public string SubHeader { get; set; } = "Кассовый чек";
            public string OrganizationName { get; set; } //= "ЧИСТЫЙ ДОМ Р ООО";
            public string AddressShop { get; set; } //= "Республика Крым, г. Симферополь, ул. Киевская, д. 22";
            public string PlaceOfCalculation { get; set; } //= "Магазин";

            // Товары
            public List<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();

            // Итоги и оплата
            public decimal TotalAmount { get; set; }
            public decimal AmountWithoutVAT { get; set; }
            public decimal CashPaid { get; set; }
            public decimal CashReceived { get; set; }            

            // Информация о кассире
            public string CashierLabel { get; set; } = "Кассир";
            public string CashierName { get; set; } //= "Кассир А.А.";

            // ФН данные
            public string FNData { get; set; } //= "ЭН ККТ 00106303010911";
            public string RNKKT { get; set; } //= "РН ККТ 0000000001016090";
            public string INN { get; set; } //= "ИНН 9102000373";
            public string FN { get; set; } //= "ФН 9999078902011982";
            public string FD { get; set; } //= "ФД 20";
            public string FP { get; set; } = "ФП 2437495754";
            public string OperationType { get; set; } = "ПРИХОД";
            public string CheckCashNumber { get; set; } //= "A01-8-73954";

            // QR код
            public string QRCodeText { get; set; } = "https://check.ofo.ru/check?fn=9999078902011982&fp=2437495754&fd=20";

            // Дата и время
            public DateTime ReceiptDateTime { get; set; } = DateTime.Now;
        }

        public class ReceiptItem
        {
            public string Name { get; set; }
            public string QuantityPrice { get; set; }
            public string Total { get; set; }
            public string VATInfo { get; set; }

            public ReceiptItem(string name, string quantityPrice, string total, string vatInfo = "НДС НЕ ОБЛАГАЕТСЯ")
            {
                Name = name;
                QuantityPrice = quantityPrice;
                Total = total;
                VATInfo = vatInfo;
            }
        }

        private Bitmap GenerateQRCode(string text, int width, int height)
        {
            try
            {
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Width = width,
                        Height = height,
                        Margin = 1
                    }
                };
                return writer.Write(text);
            }
            catch
            {
                return null;
            }
        }

        // Метод для разбивки по количеству символов с ограничением в 2 строки
        private List<string> SplitItemNameByChars(string itemName, int maxCharsPerLine)
        {
            List<string> lines = new List<string>();

            string remainingText = itemName;

            for (int i = 0; i < MAX_LINES && !string.IsNullOrEmpty(remainingText); i++)
            {
                string line = remainingText.Length <= maxCharsPerLine ?
                    remainingText :
                    remainingText.Substring(0, maxCharsPerLine).Trim();

                lines.Add(line);

                // Обновляем оставшийся текст
                if (remainingText.Length > maxCharsPerLine)
                {
                    remainingText = remainingText.Substring(maxCharsPerLine).Trim();
                }
                else
                {
                    remainingText = "";
                }
            }

            return lines;
        }

        public void PrintReceiptToThermalPrinter(ReceiptData receiptData)
        {
            try
            {
                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = "THERMAL Receipt Printer";

                // Уменьшаем ширину страницы — чтобы текст не уходил за край
                pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", 280, 1500);
                pd.DefaultPageSettings.Margins = new Margins(2, 2, 2, 2);

                int currentY = 8; // Начальная позиция Y

                pd.PrintPage += (sender, args) =>
                {
                    Graphics g = args.Graphics;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                    using (Font font = new Font("Lucida Console", 7))
                    using (Brush brush = Brushes.Black)
                    {
                        // Заголовок: ОНЛАЙН-КАССА
                        SizeF size1 = g.MeasureString(receiptData.Header, font);
                        float x1 = (args.PageBounds.Width - size1.Width) / 2;
                        g.DrawString(receiptData.Header, font, brush, x1, currentY);
                        currentY += (int)size1.Height + 2;

                        // Логотип АТОЛ
                        SizeF sizeLogo = g.MeasureString(receiptData.Logo, new Font("Atol-4", 12, FontStyle.Bold));
                        float xLogo = (args.PageBounds.Width - sizeLogo.Width) / 2;
                        g.DrawString(receiptData.Logo, new Font("Atol-4", 12, FontStyle.Bold), brush, xLogo, currentY);
                        currentY += (int)sizeLogo.Height + 2;

                        // Подзаголовок: Кассовый чек
                        SizeF sizeSub = g.MeasureString(receiptData.SubHeader, font);
                        float xSub = (args.PageBounds.Width - sizeSub.Width) / 2;
                        g.DrawString(receiptData.SubHeader, font, brush, xSub, currentY);
                        currentY += (int)sizeSub.Height + 6;

                        // Печать товаров
                        foreach (ReceiptItem item in receiptData.Items)
                        {
                            // По количеству символов - максимум 2 строки
                            List<string> nameLines = SplitItemNameByChars(item.Name, MAX_CHARS_PER_LINE);

                            // Печатаем каждую строку названия товара (максимум 2 строки)
                            foreach (string line in nameLines)
                            {
                                g.DrawString(line, font, brush, 2, currentY);
                                currentY += (int)g.MeasureString(line, font).Height - 1;
                            }

                            // Всегда печатаем цены на отдельной строке
                            if (!string.IsNullOrEmpty(item.QuantityPrice))
                            {
                                // Вычисляем позиции для выравнивания цен
                                SizeF qtyPriceSize = g.MeasureString(item.QuantityPrice, font);
                                SizeF totalSize = g.MeasureString(item.Total, font);

                                int rightMargin = 5;
                                int priceX = args.PageBounds.Width - rightMargin - (int)totalSize.Width;
                                int qtyPriceX = priceX - (int)qtyPriceSize.Width - 3;

                                // Печатаем количество и цену на отдельной строке
                                g.DrawString(item.QuantityPrice, font, brush, qtyPriceX, currentY);
                                g.DrawString(item.Total, font, brush, priceX, currentY);

                                currentY += (int)g.MeasureString(item.QuantityPrice, font).Height - 1;
                            }

                            // Печатаем информацию о НДС
                            if (!string.IsNullOrEmpty(item.VATInfo))
                            {
                                g.DrawString(item.VATInfo, font, brush, 2, currentY);
                                currentY += (int)g.MeasureString(item.VATInfo, font).Height - 1;
                            }
                        }

                        // Разделительная линия
                        currentY += 4;
                        g.DrawLine(Pens.Black, 2, currentY, args.PageBounds.Width - 10, currentY);
                        currentY += 4;

                        // ИТОГ — более крупным шрифтом, сумма прижата вправо
                        using (Font itogFont = new Font("Atol-4", 8, FontStyle.Bold))
                        {
                            string itogLabel = "ИТОГ";
                            string itogValue = $"={receiptData.TotalAmount:F2}";
                            SizeF itogLabelSize = g.MeasureString(itogLabel, itogFont);
                            SizeF itogValueSize = g.MeasureString(itogValue, itogFont);
                            int itogValueX = args.PageBounds.Width - 5 - (int)itogValueSize.Width;
                            g.DrawString(itogLabel, itogFont, brush, 2, currentY);
                            g.DrawString(itogValue, itogFont, brush, itogValueX, currentY);
                            currentY += (int)itogLabelSize.Height + 2;
                        }

                        // Разделительная линия
                        g.DrawLine(Pens.Black, 2, currentY, args.PageBounds.Width - 10, currentY);
                        currentY += 4;

                        // Сумма без НДС — выровнена по правому краю
                        string sumNoVATLabel = "СУММА БЕЗ НДС";
                        string sumNoVATValue = $"={receiptData.AmountWithoutVAT:F2}";
                        SizeF sumNoVATLabelSize = g.MeasureString(sumNoVATLabel, font);
                        SizeF sumNoVATValueSize = g.MeasureString(sumNoVATValue, font);
                        int sumNoVATValueX = args.PageBounds.Width - 5 - (int)sumNoVATValueSize.Width;
                        g.DrawString(sumNoVATLabel, font, brush, 2, currentY);
                        g.DrawString(sumNoVATValue, font, brush, sumNoVATValueX, currentY);
                        currentY += (int)sumNoVATLabelSize.Height - 1;

                        // Наличными — выровнено по правому краю
                        string cashPaidLabel = "НАЛИЧНЫМИ";
                        string cashPaidValue = $"={receiptData.CashPaid:F2}";
                        SizeF cashPaidLabelSize = g.MeasureString(cashPaidLabel, font);
                        SizeF cashPaidValueSize = g.MeasureString(cashPaidValue, font);
                        int cashPaidValueX = args.PageBounds.Width - 5 - (int)cashPaidValueSize.Width;
                        g.DrawString(cashPaidLabel, font, brush, 2, currentY);
                        g.DrawString(cashPaidValue, font, brush, cashPaidValueX, currentY);
                        currentY += (int)cashPaidLabelSize.Height - 1;                      

                        // Получено наличными — выровнено по правому краю
                        string receivedCashLabel = "ПОЛУЧЕНО НАЛИЧНЫМИ";
                        string receivedCashValue = $"={receiptData.CashReceived:F2}";
                        SizeF receivedCashLabelSize = g.MeasureString(receivedCashLabel, font);
                        SizeF receivedCashValueSize = g.MeasureString(receivedCashValue, font);
                        int receivedCashValueX = args.PageBounds.Width - 5 - (int)receivedCashValueSize.Width;
                        g.DrawString(receivedCashLabel, font, brush, 2, currentY);
                        g.DrawString(receivedCashValue, font, brush, receivedCashValueX, currentY);
                        currentY += (int)receivedCashLabelSize.Height - 1;

                        // Кассир — на одной строке, с выравниванием по краям
                        SizeF cashierLabelSize = g.MeasureString(receiptData.CashierLabel, font);
                        SizeF cashierValueSize = g.MeasureString(receiptData.CashierName, font);
                        int cashierValueX = args.PageBounds.Width - 5 - (int)cashierValueSize.Width;
                        g.DrawString(receiptData.CashierLabel, font, brush, 2, currentY);
                        g.DrawString(receiptData.CashierName, font, brush, cashierValueX, currentY);
                        currentY += (int)cashierLabelSize.Height + 4;

                        // Чистый дом
                        g.DrawString(receiptData.OrganizationName, font, brush, 2, currentY);
                        currentY += (int)g.MeasureString(receiptData.OrganizationName, font).Height - 1;

                        // Адрес
                        g.DrawString(receiptData.AddressShop, font, brush, 2, currentY);
                        currentY += (int)g.MeasureString(receiptData.AddressShop, font).Height - 1;

                        // Место расчетов — на одной строке, с выравниванием по краям
                        string placeOfCalcLabel = "МЕСТО РАСЧЕТОВ";
                        SizeF placeLabelSize = g.MeasureString(placeOfCalcLabel, font);
                        SizeF placeValueSize = g.MeasureString(receiptData.PlaceOfCalculation, font);
                        int placeValueX = args.PageBounds.Width - 5 - (int)placeValueSize.Width;
                        g.DrawString(placeOfCalcLabel, font, brush, 2, currentY);
                        g.DrawString(receiptData.PlaceOfCalculation, font, brush, placeValueX, currentY);
                        currentY += (int)placeLabelSize.Height + 6;

                        // Сохраняем начальную позицию Y для блока с данными ФН
                        int dataBlockStartY = currentY;

                        // Данные ФН и ФП - рисуем слева
                        g.DrawString(receiptData.FNData, font, brush, 2, currentY);
                        currentY += (int)g.MeasureString(receiptData.FNData, font).Height - 1;

                        g.DrawString(receiptData.RNKKT, font, brush, 2, currentY);
                        currentY += (int)g.MeasureString(receiptData.RNKKT, font).Height - 1;

                        g.DrawString(receiptData.INN, font, brush, 2, currentY);
                        currentY += (int)g.MeasureString(receiptData.INN, font).Height - 1;

                        g.DrawString(receiptData.FN, font, brush, 2, currentY);
                        currentY += (int)g.MeasureString(receiptData.FN, font).Height - 1;

                        g.DrawString(receiptData.FD, font, brush, 2, currentY);
                        currentY += (int)g.MeasureString(receiptData.FD, font).Height - 1;

                        g.DrawString(receiptData.FP, font, brush, 2, currentY);
                        currentY += (int)g.MeasureString(receiptData.FP, font).Height - 1;

                        g.DrawString(receiptData.OperationType, font, brush, 2, currentY);
                        currentY += (int)g.MeasureString(receiptData.OperationType, font).Height - 1;

                        string dateTime = receiptData.ReceiptDateTime.ToString("dd.MM.yy HH:mm");
                        g.DrawString(dateTime, font, brush, 2, currentY);
                        currentY += (int)g.MeasureString(dateTime, font).Height - 1;

                        g.DrawString(receiptData.CheckCashNumber, font, brush, 2, currentY);
                        currentY += (int)g.MeasureString(receiptData.CheckCashNumber, font).Height + 4;

                        // QR-код - рисуем справа напротив блока данных
                        Bitmap qrCodeBitmap = GenerateQRCode(receiptData.QRCodeText, 80, 80);

                        if (qrCodeBitmap != null)
                        {
                            int qrX = args.PageBounds.Width - qrCodeBitmap.Width - 5;
                            int qrY = dataBlockStartY;
                            g.DrawImage(qrCodeBitmap, qrX, qrY);
                        }
                    }

                    args.HasMorePages = false;
                };

                pd.Print();

                MessageBox.Show("Чек успешно напечатан!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка печати: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //System.Diagnostics.Debug.WriteLine($"[PrintReceiptToThermalPrinter] Ошибка: {ex}");
            }
        }

    }
}
