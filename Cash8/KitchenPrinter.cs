using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using ZXing;
using System.Security;
using System.Drawing.Text;

namespace Cash8
{
    class KitchenPrinter
    {

        // Константы для ограничения длины строк
        private const int MAX_CHARS_PER_LINE = 65;  // Максимальная длина одной строки
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
            public string IsFractional { get; set; }


            public ReceiptItem(string name, string quantityPrice, string total, string vatInfo = "НДС НЕ ОБЛАГАЕТСЯ", string isFractional = "шт.")
            {
                Name = name;
                QuantityPrice = quantityPrice;
                Total = total;
                VATInfo = vatInfo;
                IsFractional = isFractional;
            }
        }

        [SecuritySafeCritical]
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"QR Code Error: {ex.Message}");
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

       


        public async Task PrintReceiptToThermalPrinterAsync(ReceiptData receiptData, string selectedPrinter= "THERMAL Receipt Printer")
        //public async Task PrintReceiptToThermalPrinterAsync(ReceiptData receiptData, string selectedPrinter = "SEWOO-LKT-Series")
        {
            //if (isPrinting)
            //{
            //    MessageBox.Show("Дождитесь завершения текущей печати");
            //    return;
            //}

            //isPrinting = true;
            //test_print.Enabled = false;
            //test_print.Text = "Печатается...";

            try
            {
                // Используем TaskCompletionSource для асинхронного ожидания
                var tcs = new TaskCompletionSource<bool>();

                // Создаем STA-поток для печати
                var thread = new Thread(() =>
                {
                    try
                    {
                        using (var pd = new PrintDocument())
                        {
                            pd.PrinterSettings.PrinterName = selectedPrinter;
                            pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", 280, 1500);                           

                            var printCompleted = new ManualResetEvent(false);
                            Exception printException = null;

                            pd.PrintPage += (sender, e) =>
                            {
                                try
                                {                                   
                                    //PrintReceiptToThermalPrinter(receiptData);
                                    PrintReceiptPage(e, receiptData);
                                }
                                catch (Exception ex)
                                {
                                    printException = ex;
                                }
                            };

                            pd.EndPrint += (sender, e) =>
                            {
                                printCompleted.Set();
                            };

                            pd.Print();

                            if (!printCompleted.WaitOne(10000))
                                throw new TimeoutException("Таймаут печати");

                            if (printException != null)
                                throw printException;

                            tcs.SetResult(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

                // Ждем завершения потока
                await tcs.Task;

                MessageBox.Show("Чек успешно напечатан!", "Успех",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка печати: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //isPrinting = false;
                //test_print.Enabled = true;
                //test_print.Text = "Печать чека";
            }
        }

        //private void PrintReceiptPage(PrintPageEventArgs e, ReceiptData receiptData)
        //{
        //    PrivateFontCollection privateFonts = new PrivateFontCollection();
        //    Font kitchenFont = null;
        //    Font kitchenFontBold = null;

        //    try
        //    {
        //        int currentY = 8;

        //        // Загружаем шрифт из файла
        //        privateFonts.AddFontFile("Kitchen.ttf");
        //        string fontFamilyName = privateFonts.Families[0].Name;

        //        // Создаем шрифты разных размеров и стилей
        //        kitchenFont = new Font(fontFamilyName, 6);
        //        //kitchenFontBold = new Font(fontFamilyName, 12, FontStyle.Bold);
        //        //using (Font itogFont = new Font(fontFamilyName, 8, FontStyle.Bold))
        //        kitchenFontBold = new Font(fontFamilyName, 16, FontStyle.Bold);
        //        using (Font itogFont = new Font(fontFamilyName, 8))
        //        //using (Brush brush = Brushes.Black)
        //        using (Brush brush = new SolidBrush(Color.FromArgb(200, Color.Black)))
        //        {
        //            Graphics g = e.Graphics;
        //            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        //            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

        //            // Настройка межстрочного интервала
        //            int lineSpacing = 2; // Дополнительные пиксели между строками
        //            int sectionSpacing = 4; // Дополнительные пиксели между секциями

        //            // Заголовок: ОНЛАЙН-КАССА
        //            SizeF size1 = g.MeasureString(receiptData.Header, kitchenFont);
        //            float x1 = (e.PageBounds.Width - size1.Width) / 2;
        //            g.DrawString(receiptData.Header, kitchenFont, brush, x1, currentY);
        //            currentY += (int)size1.Height + lineSpacing;

        //            // Логотип АТОЛ
        //            SizeF sizeLogo = g.MeasureString(receiptData.Logo, kitchenFontBold);
        //            float xLogo = (e.PageBounds.Width - sizeLogo.Width) / 2;
        //            g.DrawString(receiptData.Logo, kitchenFontBold, brush, xLogo, currentY);
        //            currentY += (int)sizeLogo.Height + lineSpacing;

        //            // Подзаголовок: Кассовый чек
        //            SizeF sizeSub = g.MeasureString(receiptData.SubHeader, kitchenFont);
        //            float xSub = (e.PageBounds.Width - sizeSub.Width) / 2;
        //            g.DrawString(receiptData.SubHeader, kitchenFont, brush, xSub, currentY);
        //            currentY += (int)sizeSub.Height + sectionSpacing;

        //            // Печать товаров
        //            foreach (ReceiptItem item in receiptData.Items)
        //            {
        //                List<string> nameLines = SplitItemNameByChars(item.Name, MAX_CHARS_PER_LINE);

        //                foreach (string line in nameLines)
        //                {
        //                    g.DrawString(line, kitchenFont, brush, 2, currentY);
        //                    currentY += (int)g.MeasureString(line, kitchenFont).Height + lineSpacing;
        //                }

        //                if (!string.IsNullOrEmpty(item.QuantityPrice))
        //                {
        //                    // Получаем размеры всех элементов
        //                    SizeF qtyPriceSize = g.MeasureString(item.QuantityPrice, kitchenFont);
        //                    SizeF totalSize = g.MeasureString(item.Total, kitchenFont);
        //                    SizeF fractionalSize = g.MeasureString(item.IsFractional, kitchenFont);

        //                    int rightMargin = 5;

        //                    // Позиционируем элементы справа налево
        //                    int totalX = e.PageBounds.Width - rightMargin - (int)totalSize.Width;
        //                    int fractionalX = totalX - (int)fractionalSize.Width - 3;
        //                    int qtyPriceX = fractionalX - (int)qtyPriceSize.Width - 3;

        //                    // Рисуем элементы
        //                    g.DrawString(item.QuantityPrice, kitchenFont, brush, qtyPriceX, currentY);
        //                    g.DrawString(item.IsFractional, kitchenFont, brush, fractionalX, currentY);
        //                    g.DrawString(item.Total, kitchenFont, brush, totalX, currentY);

        //                    currentY += (int)g.MeasureString(item.QuantityPrice, kitchenFont).Height + lineSpacing;
        //                }

        //                if (!string.IsNullOrEmpty(item.VATInfo))
        //                {
        //                    g.DrawString(item.VATInfo, kitchenFont, brush, 2, currentY);
        //                    currentY += (int)g.MeasureString(item.VATInfo, kitchenFont).Height + lineSpacing;
        //                }

        //                // Добавляем отступ между товарами
        //                currentY += 1;
        //            }

        //            // Разделительная линия
        //            currentY += sectionSpacing;
        //            g.DrawLine(Pens.Black, 2, currentY, e.PageBounds.Width - 10, currentY);
        //            currentY += sectionSpacing;

        //            // ИТОГ
        //            string itogLabel = "ИТОГ";
        //            string itogValue = $"={receiptData.TotalAmount:F2}";
        //            SizeF itogLabelSize = g.MeasureString(itogLabel, itogFont);
        //            SizeF itogValueSize = g.MeasureString(itogValue, itogFont);
        //            int itogValueX = e.PageBounds.Width - 5 - (int)itogValueSize.Width;
        //            g.DrawString(itogLabel, itogFont, brush, 2, currentY);
        //            g.DrawString(itogValue, itogFont, brush, itogValueX, currentY);
        //            currentY += (int)itogLabelSize.Height + lineSpacing;

        //            // Разделительная линия
        //            g.DrawLine(Pens.Black, 2, currentY, e.PageBounds.Width - 10, currentY);
        //            currentY += sectionSpacing;

        //            // Остальные элементы с увеличенными отступами...
        //            // Сумма без НДС
        //            string sumNoVATLabel = "СУММА БЕЗ НДС";
        //            string sumNoVATValue = $"={receiptData.AmountWithoutVAT:F2}";
        //            SizeF sumNoVATLabelSize = g.MeasureString(sumNoVATLabel, kitchenFont);
        //            SizeF sumNoVATValueSize = g.MeasureString(sumNoVATValue, kitchenFont);
        //            int sumNoVATValueX = e.PageBounds.Width - 5 - (int)sumNoVATValueSize.Width;
        //            g.DrawString(sumNoVATLabel, kitchenFont, brush, 2, currentY);
        //            g.DrawString(sumNoVATValue, kitchenFont, brush, sumNoVATValueX, currentY);
        //            currentY += (int)sumNoVATLabelSize.Height + lineSpacing;

        //            // Наличными
        //            string cashPaidLabel = "НАЛИЧНЫМИ";
        //            string cashPaidValue = $"={receiptData.CashPaid:F2}";
        //            SizeF cashPaidLabelSize = g.MeasureString(cashPaidLabel, kitchenFont);
        //            SizeF cashPaidValueSize = g.MeasureString(cashPaidValue, kitchenFont);
        //            int cashPaidValueX = e.PageBounds.Width - 5 - (int)cashPaidValueSize.Width;
        //            g.DrawString(cashPaidLabel, kitchenFont, brush, 2, currentY);
        //            g.DrawString(cashPaidValue, kitchenFont, brush, cashPaidValueX, currentY);
        //            currentY += (int)cashPaidLabelSize.Height + lineSpacing;

        //            // Получено наличными
        //            string receivedCashLabel = "ПОЛУЧЕНО НАЛИЧНЫМИ";
        //            string receivedCashValue = $"={receiptData.CashReceived:F2}";
        //            SizeF receivedCashLabelSize = g.MeasureString(receivedCashLabel, kitchenFont);
        //            SizeF receivedCashValueSize = g.MeasureString(receivedCashValue, kitchenFont);
        //            int receivedCashValueX = e.PageBounds.Width - 5 - (int)receivedCashValueSize.Width;
        //            g.DrawString(receivedCashLabel, kitchenFont, brush, 2, currentY);
        //            g.DrawString(receivedCashValue, kitchenFont, brush, receivedCashValueX, currentY);
        //            currentY += (int)receivedCashLabelSize.Height + lineSpacing;

        //            // Кассир
        //            SizeF cashierLabelSize = g.MeasureString(receiptData.CashierLabel, kitchenFont);
        //            SizeF cashierValueSize = g.MeasureString(receiptData.CashierName, kitchenFont);
        //            int cashierValueX = e.PageBounds.Width - 5 - (int)cashierValueSize.Width;
        //            g.DrawString(receiptData.CashierLabel, kitchenFont, brush, 2, currentY);
        //            g.DrawString(receiptData.CashierName, kitchenFont, brush, cashierValueX, currentY);
        //            currentY += (int)cashierLabelSize.Height + sectionSpacing;

        //            // Чистый дом
        //            g.DrawString(receiptData.OrganizationName, kitchenFont, brush, 2, currentY);
        //            currentY += (int)g.MeasureString(receiptData.OrganizationName, kitchenFont).Height + lineSpacing;

        //            // Адрес
        //            g.DrawString(receiptData.AddressShop, kitchenFont, brush, 2, currentY);
        //            currentY += (int)g.MeasureString(receiptData.AddressShop, kitchenFont).Height + lineSpacing;

        //            // Место расчетов
        //            string placeOfCalcLabel = "МЕСТО РАСЧЕТОВ";
        //            SizeF placeLabelSize = g.MeasureString(placeOfCalcLabel, kitchenFont);
        //            SizeF placeValueSize = g.MeasureString(receiptData.PlaceOfCalculation, kitchenFont);
        //            int placeValueX = e.PageBounds.Width - 5 - (int)placeValueSize.Width;
        //            g.DrawString(placeOfCalcLabel, kitchenFont, brush, 2, currentY);
        //            g.DrawString(receiptData.PlaceOfCalculation, kitchenFont, brush, placeValueX, currentY);
        //            currentY += (int)placeLabelSize.Height + sectionSpacing;

        //            // Данные ФН
        //            int dataBlockStartY = currentY;

        //            g.DrawString(receiptData.FNData, kitchenFont, brush, 2, currentY);
        //            currentY += (int)g.MeasureString(receiptData.FNData, kitchenFont).Height + lineSpacing;

        //            g.DrawString(receiptData.RNKKT, kitchenFont, brush, 2, currentY);
        //            currentY += (int)g.MeasureString(receiptData.RNKKT, kitchenFont).Height + lineSpacing;

        //            g.DrawString(receiptData.INN, kitchenFont, brush, 2, currentY);
        //            currentY += (int)g.MeasureString(receiptData.INN, kitchenFont).Height + lineSpacing;

        //            g.DrawString(receiptData.FN, kitchenFont, brush, 2, currentY);
        //            currentY += (int)g.MeasureString(receiptData.FN, kitchenFont).Height + lineSpacing;

        //            g.DrawString(receiptData.FD, kitchenFont, brush, 2, currentY);
        //            currentY += (int)g.MeasureString(receiptData.FD, kitchenFont).Height + lineSpacing;

        //            g.DrawString(receiptData.FP, kitchenFont, brush, 2, currentY);
        //            currentY += (int)g.MeasureString(receiptData.FP, kitchenFont).Height + lineSpacing;

        //            g.DrawString(receiptData.OperationType, kitchenFont, brush, 2, currentY);
        //            currentY += (int)g.MeasureString(receiptData.OperationType, kitchenFont).Height + lineSpacing;

        //            string dateTime = receiptData.ReceiptDateTime.ToString("dd.MM.yy HH:mm");
        //            g.DrawString(dateTime, kitchenFont, brush, 2, currentY);
        //            currentY += (int)g.MeasureString(dateTime, kitchenFont).Height + lineSpacing;

        //            g.DrawString(receiptData.CheckCashNumber, kitchenFont, brush, 2, currentY);
        //            currentY += (int)g.MeasureString(receiptData.CheckCashNumber, kitchenFont).Height + sectionSpacing;

        //            // QR-код
        //            using (Bitmap qrCodeBitmap = GenerateQRCode(receiptData.QRCodeText, 80, 80))
        //            {
        //                if (qrCodeBitmap != null)
        //                {
        //                    int qrX = e.PageBounds.Width - qrCodeBitmap.Width - 5;
        //                    int qrY = dataBlockStartY;
        //                    g.DrawImage(qrCodeBitmap, qrX, qrY);
        //                }
        //            }
        //        }

        //        e.HasMorePages = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка при отрисовке чека: {ex.Message}");
        //        e.HasMorePages = false;
        //    }
        //    finally
        //    {
        //        // Освобождаем ресурсы шрифтов
        //        kitchenFont?.Dispose();
        //        kitchenFontBold?.Dispose();
        //        privateFonts?.Dispose();
        //    }
        //}

        private void PrintReceiptPage(PrintPageEventArgs e, ReceiptData receiptData)
        {
            PrivateFontCollection privateFonts = new PrivateFontCollection();
            Font kitchenFont = null;
            Font kitchenFontBold = null;

            try
            {
                int currentY = 8;

                // Загружаем шрифт из файла
                privateFonts.AddFontFile("Kitchen.ttf");
                string fontFamilyName = privateFonts.Families[0].Name;

                // Создаем шрифты разных размеров и стилей
                kitchenFont = new Font(fontFamilyName, 6);
                kitchenFontBold = new Font(fontFamilyName, 16, FontStyle.Bold);

                using (Brush brush = new SolidBrush(Color.FromArgb(200, Color.Black)))
                using (Pen lightPen = new Pen(Color.FromArgb(200, Color.Black)))
                using (Font itogFont = new Font(fontFamilyName, 7))
                {
                    Graphics g = e.Graphics;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                    // Коэффициенты трансформации
                    float scaleX = 0.8f;  // Уменьшение ширины на 20%
                    float scaleY = 1.2f; // Увеличение высоты на 15%

                    // Настройка межстрочного интервала
                    int lineSpacing = 2;
                    int sectionSpacing = 4;

                    // Применяем трансформацию ко всему графическому контексту
                    g.ScaleTransform(scaleX, scaleY);

                    // Корректируем координаты с учетом трансформации
                    float scaledCurrentY = currentY / scaleY;
                    float pageWidth = e.PageBounds.Width / scaleX; // Ширина страницы в масштабированных координатах

                    // Заголовок: ОНЛАЙН-КАССА
                    SizeF size1 = g.MeasureString(receiptData.Header, kitchenFont);
                    float x1 = (pageWidth - size1.Width) / 2;
                    g.DrawString(receiptData.Header, kitchenFont, brush, x1, scaledCurrentY);
                    scaledCurrentY += (size1.Height / scaleY) + lineSpacing;

                    // Логотип АТОЛ
                    SizeF sizeLogo = g.MeasureString(receiptData.Logo, kitchenFontBold);
                    float xLogo = (pageWidth - sizeLogo.Width) / 2;
                    g.DrawString(receiptData.Logo, kitchenFontBold, brush, xLogo, scaledCurrentY);
                    scaledCurrentY += (sizeLogo.Height / scaleY) + lineSpacing;

                    // Подзаголовок: Кассовый чек
                    SizeF sizeSub = g.MeasureString(receiptData.SubHeader, kitchenFont);
                    float xSub = (pageWidth - sizeSub.Width) / 2;
                    g.DrawString(receiptData.SubHeader, kitchenFont, brush, xSub, scaledCurrentY);
                    scaledCurrentY += (sizeSub.Height / scaleY) + sectionSpacing;

                    // Печать товаров
                    foreach (ReceiptItem item in receiptData.Items)
                    {
                        List<string> nameLines = SplitItemNameByChars(item.Name, MAX_CHARS_PER_LINE);

                        foreach (string line in nameLines)
                        {
                            g.DrawString(line, kitchenFont, brush, 2, scaledCurrentY);
                            scaledCurrentY += (g.MeasureString(line, kitchenFont).Height / scaleY) + lineSpacing;
                        }

                        if (!string.IsNullOrEmpty(item.QuantityPrice))
                        {
                            // Получаем размеры всех элементов
                            SizeF qtyPriceSize = g.MeasureString(item.QuantityPrice, kitchenFont);
                            SizeF totalSize = g.MeasureString(item.Total, kitchenFont);
                            SizeF fractionalSize = g.MeasureString(item.IsFractional, kitchenFont);

                            int rightMargin = 3;

                            // Позиционируем элементы справа налево (в масштабированных координатах)
                            int totalX = (int)pageWidth - rightMargin - (int)totalSize.Width;
                            int fractionalX = totalX - (int)fractionalSize.Width - 3;
                            int qtyPriceX = fractionalX - (int)qtyPriceSize.Width - 33;

                            // Рисуем элементы
                            g.DrawString(item.QuantityPrice, kitchenFont, brush, qtyPriceX, scaledCurrentY);
                            g.DrawString(item.IsFractional, kitchenFont, brush, fractionalX, scaledCurrentY);
                            g.DrawString(item.Total, kitchenFont, brush, totalX, scaledCurrentY);

                            scaledCurrentY += (g.MeasureString(item.QuantityPrice, kitchenFont).Height / scaleY) + lineSpacing;
                        }

                        if (!string.IsNullOrEmpty(item.VATInfo))
                        {
                            g.DrawString(item.VATInfo, kitchenFont, brush, 2, scaledCurrentY);
                            scaledCurrentY += (g.MeasureString(item.VATInfo, kitchenFont).Height / scaleY) + lineSpacing;
                        }

                        // Добавляем отступ между товарами
                        scaledCurrentY += 1;
                    }

                    scaledCurrentY -= 1;

                    // Разделительная линия
                    scaledCurrentY += sectionSpacing;
                    g.DrawLine(lightPen, 2, scaledCurrentY, pageWidth - 10, scaledCurrentY);
                    scaledCurrentY += sectionSpacing;

                    // ИТОГ
                    string itogLabel = "ИТОГ";
                    string itogValue = $"={receiptData.TotalAmount:F2}";
                    SizeF itogLabelSize = g.MeasureString(itogLabel, itogFont);
                    SizeF itogValueSize = g.MeasureString(itogValue, itogFont);
                    int itogValueX = (int)pageWidth - 5 - (int)itogValueSize.Width;
                    g.DrawString(itogLabel, itogFont, brush, 2, scaledCurrentY);
                    g.DrawString(itogValue, itogFont, brush, itogValueX, scaledCurrentY);

                    // ДВОЙНОЙ отступ вниз после ИТОГ
                    scaledCurrentY += (itogLabelSize.Height / scaleY) + (lineSpacing * 2);

                    // Разделительная линия
                    g.DrawLine(lightPen, 2, scaledCurrentY, pageWidth - 10, scaledCurrentY);
                    scaledCurrentY += sectionSpacing;

                    // Сумма без НДС
                    string sumNoVATLabel = "СУММА БЕЗ НДС";
                    string sumNoVATValue = $"={receiptData.AmountWithoutVAT:F2}";
                    SizeF sumNoVATLabelSize = g.MeasureString(sumNoVATLabel, kitchenFont);
                    SizeF sumNoVATValueSize = g.MeasureString(sumNoVATValue, kitchenFont);
                    int sumNoVATValueX = (int)pageWidth - 5 - (int)sumNoVATValueSize.Width;
                    g.DrawString(sumNoVATLabel, kitchenFont, brush, 2, scaledCurrentY);
                    g.DrawString(sumNoVATValue, kitchenFont, brush, sumNoVATValueX, scaledCurrentY);
                    scaledCurrentY += (sumNoVATLabelSize.Height / scaleY) + lineSpacing;

                    // Наличными
                    string cashPaidLabel = "НАЛИЧНЫМИ";
                    string cashPaidValue = $"={receiptData.CashPaid:F2}";
                    SizeF cashPaidLabelSize = g.MeasureString(cashPaidLabel, kitchenFont);
                    SizeF cashPaidValueSize = g.MeasureString(cashPaidValue, kitchenFont);
                    int cashPaidValueX = (int)pageWidth - 5 - (int)cashPaidValueSize.Width;
                    g.DrawString(cashPaidLabel, kitchenFont, brush, 2, scaledCurrentY);
                    g.DrawString(cashPaidValue, kitchenFont, brush, cashPaidValueX, scaledCurrentY);
                    scaledCurrentY += (cashPaidLabelSize.Height / scaleY) + lineSpacing;

                    // Получено наличными
                    string receivedCashLabel = "ПОЛУЧЕНО НАЛИЧНЫМИ";
                    string receivedCashValue = $"={receiptData.CashReceived:F2}";
                    SizeF receivedCashLabelSize = g.MeasureString(receivedCashLabel, kitchenFont);
                    SizeF receivedCashValueSize = g.MeasureString(receivedCashValue, kitchenFont);
                    int receivedCashValueX = (int)pageWidth - 5 - (int)receivedCashValueSize.Width;
                    g.DrawString(receivedCashLabel, kitchenFont, brush, 2, scaledCurrentY);
                    g.DrawString(receivedCashValue, kitchenFont, brush, receivedCashValueX, scaledCurrentY);
                    scaledCurrentY += (receivedCashLabelSize.Height / scaleY) + lineSpacing;

                    // Кассир
                    SizeF cashierLabelSize = g.MeasureString(receiptData.CashierLabel, kitchenFont);
                    SizeF cashierValueSize = g.MeasureString(receiptData.CashierName, kitchenFont);
                    int cashierValueX = (int)pageWidth - 5 - (int)cashierValueSize.Width;
                    g.DrawString(receiptData.CashierLabel, kitchenFont, brush, 2, scaledCurrentY);
                    g.DrawString(receiptData.CashierName, kitchenFont, brush, cashierValueX, scaledCurrentY);
                    scaledCurrentY += (cashierLabelSize.Height / scaleY) + sectionSpacing;

                    // Чистый дом
                    g.DrawString(receiptData.OrganizationName, kitchenFont, brush, 2, scaledCurrentY);
                    scaledCurrentY += (g.MeasureString(receiptData.OrganizationName, kitchenFont).Height / scaleY) + lineSpacing;

                    // Адрес
                    g.DrawString(receiptData.AddressShop, kitchenFont, brush, 2, scaledCurrentY);
                    scaledCurrentY += (g.MeasureString(receiptData.AddressShop, kitchenFont).Height / scaleY) + lineSpacing;

                    // Место расчетов
                    string placeOfCalcLabel = "МЕСТО РАСЧЕТОВ";
                    SizeF placeLabelSize = g.MeasureString(placeOfCalcLabel, kitchenFont);
                    SizeF placeValueSize = g.MeasureString(receiptData.PlaceOfCalculation, kitchenFont);
                    int placeValueX = (int)pageWidth - 5 - (int)placeValueSize.Width;
                    g.DrawString(placeOfCalcLabel, kitchenFont, brush, 2, scaledCurrentY);
                    g.DrawString(receiptData.PlaceOfCalculation, kitchenFont, brush, placeValueX, scaledCurrentY);
                    scaledCurrentY += (placeLabelSize.Height / scaleY) + sectionSpacing;

                    // Данные ФН
                    int dataBlockStartY = (int)scaledCurrentY;

                    g.DrawString(receiptData.FNData, kitchenFont, brush, 2, scaledCurrentY);
                    scaledCurrentY += (g.MeasureString(receiptData.FNData, kitchenFont).Height / scaleY) + lineSpacing;

                    g.DrawString(receiptData.RNKKT, kitchenFont, brush, 2, scaledCurrentY);
                    scaledCurrentY += (g.MeasureString(receiptData.RNKKT, kitchenFont).Height / scaleY) + lineSpacing;

                    g.DrawString(receiptData.INN, kitchenFont, brush, 2, scaledCurrentY);
                    scaledCurrentY += (g.MeasureString(receiptData.INN, kitchenFont).Height / scaleY) + lineSpacing;

                    g.DrawString(receiptData.FN, kitchenFont, brush, 2, scaledCurrentY);
                    scaledCurrentY += (g.MeasureString(receiptData.FN, kitchenFont).Height / scaleY) + lineSpacing;

                    g.DrawString(receiptData.FD, kitchenFont, brush, 2, scaledCurrentY);
                    scaledCurrentY += (g.MeasureString(receiptData.FD, kitchenFont).Height / scaleY) + lineSpacing;

                    g.DrawString(receiptData.FP, kitchenFont, brush, 2, scaledCurrentY);
                    scaledCurrentY += (g.MeasureString(receiptData.FP, kitchenFont).Height / scaleY) + lineSpacing;

                    g.DrawString(receiptData.OperationType, kitchenFont, brush, 2, scaledCurrentY);
                    scaledCurrentY += (g.MeasureString(receiptData.OperationType, kitchenFont).Height / scaleY) + lineSpacing;

                    string dateTime = receiptData.ReceiptDateTime.ToString("dd.MM.yy HH:mm");
                    g.DrawString(dateTime, kitchenFont, brush, 2, scaledCurrentY);
                    scaledCurrentY += (g.MeasureString(dateTime, kitchenFont).Height / scaleY) + lineSpacing;

                    g.DrawString(receiptData.CheckCashNumber, kitchenFont, brush, 2, scaledCurrentY);
                    scaledCurrentY += (g.MeasureString(receiptData.CheckCashNumber, kitchenFont).Height / scaleY) + sectionSpacing;

                    // Сбрасываем трансформацию перед рисованием QR-кода
                    g.ResetTransform();

                    // QR-код рисуем без трансформации
                    using (Bitmap qrCodeBitmap = GenerateQRCode(receiptData.QRCodeText, 80, 80))
                    {
                        if (qrCodeBitmap != null)
                        {
                            int qrX = e.PageBounds.Width - qrCodeBitmap.Width - 5;
                            int qrY = (int)(dataBlockStartY * scaleY); // Конвертируем обратно в обычные координаты
                            g.DrawImage(qrCodeBitmap, qrX, qrY);
                        }
                    }
                }

                e.HasMorePages = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отрисовке чека: {ex.Message}");
                e.HasMorePages = false;
            }
            finally
            {
                // Освобождаем ресурсы шрифтов
                kitchenFont?.Dispose();
                kitchenFontBold?.Dispose();
                privateFonts?.Dispose();
            }
        }
    }
}
