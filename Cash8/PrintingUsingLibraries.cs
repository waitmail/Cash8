using Atol.Drivers10.Fptr;
using System.Windows.Forms;
using AtolConstants = Atol.Drivers10.Fptr.Constants;
using System;
using System.Data;
using System.Text;
using Npgsql;


namespace Cash8
{
    class PrintingUsingLibraries
    {

        //public bool print_sale(ListView listView)
        //{
        //    bool result = true;


        //    return result;
        //}


        //private void setConnectSetting(IFptr fptr)
        //{
        //    fptr.setSingleSetting(AtolConstants.LIBFPTR_SETTING_MODEL, AtolConstants.LIBFPTR_MODEL_ATOL_AUTO.ToString());
        //    fptr.setSingleSetting(AtolConstants.LIBFPTR_SETTING_PORT, AtolConstants.LIBFPTR_PORT_COM.ToString());
        //    fptr.setSingleSetting(AtolConstants.LIBFPTR_SETTING_COM_FILE, MainStaticClass.FnSreialPort);
        //    fptr.setSingleSetting(AtolConstants.LIBFPTR_SETTING_BAUDRATE, AtolConstants.LIBFPTR_PORT_BR_115200.ToString());
        //    fptr.applySingleSettings();
        //}

        public void getShiftStatus()
        {
            IFptr fptr = MainStaticClass.FPTR;
            //setConnectSetting(fptr);
            if (!fptr.isOpened())
            {
                fptr.open();
            }
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_DATA_TYPE, AtolConstants.LIBFPTR_DT_SHIFT_STATE);
            fptr.queryData();
            uint state = fptr.getParamInt(AtolConstants.LIBFPTR_PARAM_SHIFT_STATE);
            //uint number = fptr.getParamInt(AtolConstants.LIBFPTR_PARAM_SHIFT_NUMBER);
            //DateTime dateTime = fptr.getParamDateTime(AtolConstants.LIBFPTR_PARAM_DATE_TIME);

            if (state == AtolConstants.LIBFPTR_SS_EXPIRED)
            {
                //if ((DateTime.Now - dateTime).TotalHours > 0)
                //{
                MessageBox.Show(" Период открытой смены превысил 24 часа !!!\r\n СНИМИТЕ Z-ОТЧЁТ. ЕСЛИ СОМНЕВАЕТЕСЬ В ЧЁМ-ТО, ТО ВСЁ РАВНО СНИМИТЕ Z-ОТЧЁТ");
                //}
            }
            //fptr.close();
        }

        public string getFiscallInfo()
        {
            string fn_info = "";

            IFptr fptr = MainStaticClass.FPTR;
            //setConnectSetting(fptr);
            if (!fptr.isOpened())
            {
                fptr.open();
            }



            //var parts = "1 2 3 a b c".Split();
            //int inx = 0;

            //var dict = parts.GroupBy(x => inx++ % (parts.Length / 2))
            //                .ToDictionary(x => x.First(), x => x.Last());

            //string settings = fptr.getSettings();
            //bool isOpened = fptr.isOpened();
            //if (!isOpened)
            //{
            //    MessageBox.Show(fptr.errorCode() + " " + fptr.errorDescription());
            //}

            fptr.setParam(AtolConstants.LIBFPTR_PARAM_FN_DATA_TYPE, AtolConstants.LIBFPTR_FNDT_VALIDITY_DAYS);
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_DATE_TIME, DateTime.Now);
            fptr.fnQueryData();
            uint daysRemain = fptr.getParamInt(AtolConstants.LIBFPTR_PARAM_FN_DAYS_REMAIN);
            fn_info = "Ресурс ФН " + daysRemain.ToString() + " дней ";

            fptr.setParam(AtolConstants.LIBFPTR_PARAM_FN_DATA_TYPE, AtolConstants.LIBFPTR_FNDT_FN_INFO);
            fptr.fnQueryData();

            if (fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_FN_NEED_REPLACEMENT))
            {
                fn_info += "\r\n" + "Требуется срочная замена ФН !!!";
            }
            if (fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_FN_RESOURCE_EXHAUSTED))
            {
                fn_info += "\r\n" + "Исчерпан ресурс ФН !!!";
            }
            if (fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_FN_MEMORY_OVERFLOW))
            {
                fn_info += "\r\n" + "Память ФН переполнена !!!";
            }
            if (fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_FN_OFD_TIMEOUT))
            {
                fn_info += "\r\n" + "Превышено время ожидания ответа от ОФД !!!";
            }
            if (fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_FN_CRITICAL_ERROR))
            {
                fn_info += "\r\n" + "Критическая ошибка ФН !!!";
            }

            //fptr.close();

            return fn_info;
        }

        public string ofdExchangeStatus()
        {
            string result = "0";

            IFptr fptr = MainStaticClass.FPTR;
            //setConnectSetting(fptr);
            if (!fptr.isOpened())
            {
                fptr.open();
            }
            //fptr.isOpened

            fptr.setParam(AtolConstants.LIBFPTR_PARAM_FN_DATA_TYPE, AtolConstants.LIBFPTR_FNDT_OFD_EXCHANGE_STATUS);
            fptr.fnQueryData();

            uint exchangeStatus = fptr.getParamInt(AtolConstants.LIBFPTR_PARAM_OFD_EXCHANGE_STATUS);
            uint unsentCount = fptr.getParamInt(AtolConstants.LIBFPTR_PARAM_DOCUMENTS_COUNT);
            if (unsentCount > 0)
            {
                DateTime dateTime = fptr.getParamDateTime(AtolConstants.LIBFPTR_PARAM_DATE_TIME);
                result = "Не отправлено документов " + unsentCount.ToString() + "\r\n" +
                               " начиная с даты " + dateTime.ToString("yyyy-MM-dd HH:mm:ss");

            }

            //fptr.close();
            return result;
        }

        public string getCasheSumm()
        {
            string result = "";
            IFptr fptr = MainStaticClass.FPTR;
            //setConnectSetting(fptr);
            if (!fptr.isOpened())
            {
                fptr.open();
            }
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_DATA_TYPE, AtolConstants.LIBFPTR_DT_CASH_SUM);
            fptr.queryData();
            double cashSum = fptr.getParamDouble(AtolConstants.LIBFPTR_PARAM_SUM);
            if (fptr.errorCode() != 0)
            {
                MessageBox.Show("Ошибка при получении суммы наличных в кассе  " + fptr.errorDescription());
            }


            //fptr.close();

            result = cashSum.ToString().Replace(",", ".");
            return result;
        }

        public void cashOutcome(double sumCashOut)
        {
            IFptr fptr = MainStaticClass.FPTR;
            //setConnectSetting(fptr);
            if (!fptr.isOpened())
            {
                fptr.open();
            }
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_SUM, sumCashOut);
            if (fptr.cashOutcome() != 0)
            {
                MessageBox.Show("Ошибка при инкасации  " + fptr.errorDescription());
            }
            //fptr.close();
        }

        public void cashIncome(double sumCashIn)
        {
            IFptr fptr = MainStaticClass.FPTR;
            //setConnectSetting(fptr);
            if (!fptr.isOpened())
            {
                fptr.open();
            }
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_SUM, sumCashIn);
            if (fptr.cashIncome() != 0)
            {
                MessageBox.Show("Ошибка при внесении  " + fptr.errorDescription());
            }
            //fptr.close();
        }

        public void reportX()
        {
            IFptr fptr = MainStaticClass.FPTR;
            //setConnectSetting(fptr);
            if (!fptr.isOpened())
            {
                fptr.open();
            }
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_REPORT_TYPE, AtolConstants.LIBFPTR_RT_X);
            if (fptr.report() != 0)
            {
                MessageBox.Show(string.Format("Ошибка при X отчете.\nОшибка {0}: {1}", fptr.errorCode(), fptr.errorDescription()),
                    "Ошибка при X отчете", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //fptr.close();
        }

        public void reportZ()
        {
            IFptr fptr = MainStaticClass.FPTR;
            //setConnectSetting(fptr);
            if (!fptr.isOpened())
            {
                fptr.open();
            }
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_REPORT_TYPE, AtolConstants.LIBFPTR_RT_CLOSE_SHIFT);
            if (fptr.report() != 0)
            {
                MessageBox.Show(string.Format("Ошибка при закрытии смены.\nОшибка {0}: {1}", fptr.errorCode(), fptr.errorDescription()),
                    "Ошибка закрытия смены", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //fptr.close();
        }



        private void print_terminal_check(IFptr fptr, Cash_check check)
        {
            if (check.recharge_note != "")
            {
                fptr.beginNonfiscalDocument();
                string s = check.recharge_note.Replace("0xDF^^", "");
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_CENTER);
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                fptr.printText();
                fptr.endNonfiscalDocument();
                check.recharge_note = "";
            }
            //if (recharge_note != "")
            //{
            //    FiscallPrintJason2.NonFiscallDocument nonFiscallDocument = new FiscallPrintJason2.NonFiscallDocument();
            //    nonFiscallDocument.type = "nonFiscal";
            //    nonFiscallDocument.printFooter = false;

            //    FiscallPrintJason2.ItemNonFiscal itemNonFiscal = new FiscallPrintJason2.ItemNonFiscal();
            //    nonFiscallDocument.items = new List<FiscallPrintJason2.ItemNonFiscal>();

            //    itemNonFiscal.type = "text";
            //    itemNonFiscal.text = recharge_note.Replace("0xDF^^", "");
            //    itemNonFiscal.alignment = "center";
            //    nonFiscallDocument.items.Add(itemNonFiscal);
            //    FiscallPrintJason2.print_not_fiscal_document(nonFiscallDocument);
            //    recharge_note = "";
            //}
        }

        public void print_sell_2_or_return_sell(Cash_check check)
        {
            bool error = false;

            //***************************************************************************          


            if (check.to_print_certainly == 1)
            {
                MainStaticClass.delete_document_wil_be_printed(check.numdoc.ToString());
            }

            if (MainStaticClass.get_document_wil_be_printed(check.numdoc.ToString()) != 0)
            {
                MessageBox.Show("Этот чек уже был успешно отправлен на печать");
                return;
            }

            //closing = false;           

            //**************************************************************************


            IFptr fptr = MainStaticClass.FPTR;
            //setConnectSetting(fptr);
            if (!fptr.isOpened())
            {
                fptr.open();
            }

            fptr.setParam(1021, MainStaticClass.Cash_Operator);
            fptr.setParam(1203, MainStaticClass.CashOperatorInn);
            fptr.operatorLogin();

            print_terminal_check(fptr, check);

            if (MainStaticClass.SystemTaxation == 1)
            {
                fptr.setParam(1055, AtolConstants.LIBFPTR_TT_OSN);
            }
            else if (MainStaticClass.SystemTaxation == 2)
            {
                fptr.setParam(1055, AtolConstants.LIBFPTR_TT_USN_INCOME_OUTCOME);
            }

            if ((check.check_type.SelectedIndex == 1) || !check.itsnew)//старый механизм работы с макрировкой, для возвратов так же пока старая схема
            {
                fptr.clearMarkingCodeValidationResult();
                foreach (ListViewItem lvi in check.listView1.Items)
                {
                    if (lvi.SubItems[14].Text.Trim().Length > 13)
                    {
                        byte[] textAsBytes = System.Text.Encoding.Default.GetBytes(lvi.SubItems[14].Text.Trim().Replace("vasya2021", "'"));
                        string mark = Convert.ToBase64String(textAsBytes);
                        if (check.check_type.SelectedIndex == 1 || !check.itsnew)
                        {
                            //string mark = Convert.ToBase64String(textAsBytes);
                            uint status = 2;

                            // Запускаем проверку КМ
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_TYPE, AtolConstants.LIBFPTR_MCT12_AUTO);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE, mark);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_STATUS, status);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_QUANTITY, 1.000);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MEASUREMENT_UNIT, AtolConstants.LIBFPTR_IU_PIECE);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_PROCESSING_MODE, 0);
                            //fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_FRACTIONAL_QUANTITY, "1/2");
                            fptr.beginMarkingCodeValidation();

                            //// Дожидаемся окончания проверки и запоминаем результат
                            //while (true)
                            //{
                            //    fptr.getMarkingCodeValidationStatus();
                            //    if (fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_VALIDATION_READY))
                            //        break;
                            //}
                            //uint validationResult = fptr.getParamInt(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_ONLINE_VALIDATION_RESULT);
                            DateTime start_check = DateTime.Now;
                            uint validationError = 0;
                            while (true)
                            {
                                fptr.getMarkingCodeValidationStatus();
                                if (fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_VALIDATION_READY))
                                {
                                    break;
                                }
                                else
                                {
                                    MainStaticClass.write_event_in_log(fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_VALIDATION_READY).ToString() + " " + mark, "check_marking_code", check.numdoc.ToString());
                                }
                                if ((DateTime.Now - start_check).Milliseconds > 2000)
                                {
                                    //MessageBox.Show("check_marking_code таймаут при проверки qr кода " + mark);
                                    MainStaticClass.write_event_in_log("Таймаут при gроверкt маркировки " + mark, "check_marking_code", check.numdoc.ToString());
                                    validationError = 421;
                                    break;
                                }
                            }
                            //**************************************************************************
                            validationError = fptr.getParamInt(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_ONLINE_VALIDATION_ERROR);
                            if ((validationError != 0) && (validationError != 402) && (validationError != 421))
                            {
                                error = true;
                                MessageBox.Show("Код ошибки = " + validationError + "; " + fptr.getParamString(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_ONLINE_VALIDATION_ERROR_DESCRIPTION), "Проверка кода маркировки");
                            }
                            //**************************************************************************
                            // Подтверждаем реализацию товара с указанным КМ
                            fptr.acceptMarkingCode();
                        }
                    }
                }
            }

            // Открытие чека (с передачей телефона получателя)
            if (check.check_type.SelectedIndex == 0)
            {
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_RECEIPT_TYPE, AtolConstants.LIBFPTR_RT_SELL);
            }
            else if (check.check_type.SelectedIndex == 1)
            {
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_RECEIPT_TYPE, AtolConstants.LIBFPTR_RT_SELL_RETURN);
            }
            else if (check.check_type.SelectedIndex == 2)
            {

                fptr.setParam(1178, check.sale_date);//Дата продажи
                if (check.tax_order.Trim().Length != 0)
                {
                    fptr.setParam(1179, check.tax_order);//Номер предписания
                }

                //fptr.setParam(1179, "2.15-15/00373 от 12.01.2024");//Номер предписания

                //fptr.setParam(1179, "2.15-15/00373 от 12.01.2024");//Номер предписания
                fptr.utilFormTlv();
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_RECEIPT_TYPE, AtolConstants.LIBFPTR_RT_SELL_CORRECTION);

                byte[] correctionInfo = fptr.getParamByteArray(AtolConstants.LIBFPTR_PARAM_TAG_VALUE);
                if (check.tax_order == "")
                {
                    fptr.setParam(1173, 0);
                }
                else
                {
                    fptr.setParam(1173, 1);
                }
                fptr.setParam(1174, correctionInfo);
            }

            if (check.txtB_email_telephone.Text.Trim().Length > 0)
            {
                fptr.setParam(1008, check.txtB_email_telephone.Text);
            }

            if ((check.txtB_inn.Text.Trim().Length > 0) && (check.txtB_name.Text.Trim().Length > 0))
            {
                fptr.setParam(1228, check.txtB_inn.Text);
                fptr.setParam(1227, check.txtB_name.Text);
            }



            if (fptr.openReceipt() != 0)
            {
                MessageBox.Show(string.Format("Ошибка при открытии чека.\nОшибка {0}: {1}", fptr.errorCode(), fptr.errorDescription()),
                        "Ошибка откртия чека", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //fptr.close();
                return;
            }

            foreach (ListViewItem lvi in check.listView1.Items)
            {
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_COMMODITY_NAME, lvi.SubItems[0].Text.Trim() + " " + lvi.SubItems[1].Text.Trim());
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PRICE, lvi.SubItems[5].Text.Replace(",", "."));
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_MEASUREMENT_UNIT, AtolConstants.LIBFPTR_IU_PIECE);
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_QUANTITY, lvi.SubItems[3].Text);
                int stavka_nds = MainStaticClass.get_tovar_nds(lvi.SubItems[0].Text.Trim());
                //nomer_naloga = 0;
                //MainStaticClass.use
                if (MainStaticClass.SystemTaxation == 1)
                {
                    if (stavka_nds == 0)
                    {
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_TAX_TYPE, AtolConstants.LIBFPTR_TAX_VAT0);
                    }
                    else if (stavka_nds == 10)
                    {
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_TAX_TYPE, AtolConstants.LIBFPTR_TAX_VAT10);
                    }
                    else if (stavka_nds == 18)
                    {
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_TAX_TYPE, AtolConstants.LIBFPTR_TAX_VAT20);
                    }
                    else if (stavka_nds == 20)
                    {
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_TAX_TYPE, AtolConstants.LIBFPTR_TAX_VAT20);
                    }
                    else
                    {
                        MessageBox.Show("Неизвестная ставка ндс");
                        error = true;
                    }
                    if (MainStaticClass.its_certificate(lvi.SubItems[0].Text.Trim()))
                    {
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_TAX_TYPE, AtolConstants.LIBFPTR_TAX_VAT120);
                    }
                }
                else
                {
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_TAX_TYPE, AtolConstants.LIBFPTR_TAX_NO);
                }


                if (MainStaticClass.its_excise(lvi.SubItems[0].Text.Trim()) == 0)
                {
                    if (lvi.SubItems[14].Text.Trim().Length <= 13)//код маркировки не заполнен
                    {
                        fptr.setParam(1212, 32);
                    }
                    else
                    {
                        byte[] textAsBytes = System.Text.Encoding.Default.GetBytes(lvi.SubItems[14].Text.Trim().Replace("vasya2021", "'"));
                        string mark = Convert.ToBase64String(textAsBytes);
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE, mark);
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_STATUS, 2);
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_TYPE, AtolConstants.LIBFPTR_MCT12_AUTO);
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_ONLINE_VALIDATION_RESULT, 0);
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_PROCESSING_MODE, 0);
                    }
                }
                else
                {
                    fptr.setParam(1212, 2);//подакцизеый товар
                }
                fptr.registration();
            }

            // Регистрация итога (отбрасываем копейки)
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_SUM, (double)check.calculation_of_the_sum_of_the_document());
            fptr.receiptTotal();



            double[] get_result_payment = MainStaticClass.get_cash_on_type_payment(check.numdoc.ToString());
            if (get_result_payment[0] != 0)//Наличные
            {
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PAYMENT_TYPE, AtolConstants.LIBFPTR_PT_CASH);
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PAYMENT_SUM, get_result_payment[0]);
                fptr.payment();
            }
            if (get_result_payment[1] != 0)
            {
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PAYMENT_TYPE, AtolConstants.LIBFPTR_PT_ELECTRONICALLY);
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PAYMENT_SUM, get_result_payment[1]);
                fptr.payment();
            }
            if (get_result_payment[2] != 0)
            {
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PAYMENT_TYPE, AtolConstants.LIBFPTR_PT_PREPAID);
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PAYMENT_SUM, get_result_payment[2]);
                fptr.payment();
            }
            string s = "";
            if (check.check_type.SelectedIndex == 0)//это продажа
            {
                if (check.Discount != 0)
                {
                    //fptr.beginNonfiscalDocument();
                    s = "Вами получена скидка " + check.calculation_of_the_discount_of_the_document().ToString().Replace(",", ".") + " " + MainStaticClass.get_currency();
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_CENTER);
                    fptr.printText();

                    if (check.client.Tag != null)
                    {
                        if (check.client.Tag == check.user.Tag)
                        {
                            s = "ДК: стандартная скидка";
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_LEFT);
                            fptr.printText();
                        }
                        else
                        {
                            s = "ДК: " + check.client.Tag.ToString();
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_LEFT);
                            fptr.printText();
                        }
                    }
                    s = "ДК: " + MainStaticClass.Nick_Shop + "-" + MainStaticClass.CashDeskNumber.ToString() + "-" + check.numdoc.ToString();// +" кассир " + this.cashier;
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_LEFT);
                    fptr.printText();
                    //fptr.endNonfiscalDocument();
                }
            }

            s = MainStaticClass.Nick_Shop + "-" + MainStaticClass.CashDeskNumber.ToString() + "-" + check.numdoc.ToString();// +" кассир " + this.cashier;
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_DEFER, AtolConstants.LIBFPTR_DEFER_POST);
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_LEFT);
            fptr.printText();
            print_fiscal_advertisement(fptr);
            // Закрытие чека
            fptr.closeReceipt();

            while (fptr.checkDocumentClosed() < 0)
            {
                // Не удалось проверить состояние документа. Вывести пользователю текст ошибки, попросить устранить неполадку и повторить запрос
                MessageBox.Show(fptr.errorDescription());
                continue;
            }

            if ((!fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_DOCUMENT_CLOSED)) || (error))
            {
                // Документ не закрылся. Требуется его отменить (если это чек) и сформировать заново
                fptr.cancelReceipt();
                MessageBox.Show(String.Format("Не удалось напечатать документ (Ошибка \"{0}\"). Устраните неполадку и повторите.", fptr.errorDescription()));
                error = true;
                //fptr.close();
                return;
            }

            if (!fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_DOCUMENT_PRINTED))
            {
                // Можно сразу вызвать метод допечатывания документа, он завершится с ошибкой, если это невозможно
                while (fptr.continuePrint() < 0)
                {
                    // Если не удалось допечатать документ - показать пользователю ошибку и попробовать еще раз.
                    MessageBox.Show(String.Format("Не удалось напечатать документ (Ошибка \"{0}\"). Устраните неполадку и повторите.", fptr.errorDescription()));
                    continue;
                }
            }
            //s = MainStaticClass.Nick_Shop + "-" + MainStaticClass.CashDeskNumber.ToString() + "-" + check.numdoc.ToString();// +" кассир " + this.cashier;
            //fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
            //fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_LEFT);
            //fptr.printText();
            //print_fiscal_advertisement(fptr);

            // Завершение работы
            //fptr.close();
            if (!error)
            {
                MainStaticClass.its_print(check.numdoc.ToString());
                check.closing = false;
                check.Close();
            }
        } 

        private void print_fiscal_advertisement(IFptr fptr)
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            string s = ""; //int length = 0;
            bool first_string = true;
            try
            {
                conn.Open();
                string query = "SELECT advertisement_text  FROM advertisement order by num_str";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (first_string)
                    {
                        first_string = false;                        
                        s = " * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*";
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_DEFER, AtolConstants.LIBFPTR_DEFER_POST);//печатать в конце чека
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_LEFT);                        
                        fptr.printText();
                    }

                    s = reader["advertisement_text"].ToString();
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_DEFER, AtolConstants.LIBFPTR_DEFER_POST);
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_LEFT);
                    fptr.printText();
                }
                if (!first_string)
                {
                    s = " * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*";
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_DEFER, AtolConstants.LIBFPTR_DEFER_POST);
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_LEFT);
                    fptr.printText();                   
                }
                reader.Close();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибки при выводе рекламного текста " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибки при выводе рекламного текста " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        public void print_sell_2_3_or_return_sell(Cash_check check, int variant)
        {
            bool error = false;

            //if (check.itsnew)
            //{
            //    if (variant == 0)
            //    {
            //        if (!check.write_new_document(output, output, "0", "0", true, cash_money, non_cash_money, "0", "0"))
            //        {
            //            return;
            //        }
            //    }
            //}
            //***************************************************************************          

            //if (MainStaticClass.SystemTaxation == 0)
            //{
            //    MessageBox.Show("В константах не опрелена система налогобложения, печать чеков невозможна");
            //    return;
            //}

            IFptr fptr = MainStaticClass.FPTR;
            //setConnectSetting(fptr);
            if (!fptr.isOpened())
            {
                fptr.open();
            }
            fptr.setParam(1021, MainStaticClass.Cash_Operator);
            fptr.setParam(1203, MainStaticClass.CashOperatorInn);
            fptr.operatorLogin();

            print_terminal_check(fptr, check);
            if (variant == 0)
            {
                fptr.setParam(1055, AtolConstants.LIBFPTR_TT_PATENT);
            }
            else if (variant == 1)
            {
                fptr.setParam(1055, AtolConstants.LIBFPTR_TT_USN_INCOME_OUTCOME);
            }
            else
            {
                MessageBox.Show("В печать не передан или передан не верный вариант, печать невозможна");
                return;
            }


            if (variant == 0)
            {

                if (check.to_print_certainly == 1)
                {
                    MainStaticClass.delete_document_wil_be_printed(check.numdoc.ToString(), variant);
                }

                if (MainStaticClass.get_document_wil_be_printed(check.numdoc.ToString()) != 0)
                {
                    MessageBox.Show("Этот чек уже был успешно отправлен на печать");
                    return;
                }
                //здесь проверяем есть ли строки
                int count_string = 0;
                foreach (ListViewItem lvi in check.listView1.Items)
                {
                    if (lvi.SubItems[14].Text.Trim().Length <= 13)
                    {
                        count_string++;
                    }
                }
                if (count_string == 0)
                {
                    //здесь необходимо поставить флаг распечатан
                    check.its_print_p(variant);
                    return;
                }

            }
            else if (variant == 1)
            {
                fptr.openDrawer();
                //здесь проверяем есть ли строки

                if (check.to_print_certainly_p == 1)
                {
                    MainStaticClass.delete_document_wil_be_printed(check.numdoc.ToString(), variant);
                }

                if (MainStaticClass.get_document_wil_be_printed(check.numdoc.ToString(), variant) != 0)
                {
                    MessageBox.Show("Этот чек уже был успешно отправлен на печать");
                    return;
                }
                int count_string = 0;
                foreach (ListViewItem lvi in check.listView1.Items)
                {
                    if (lvi.SubItems[14].Text.Trim().Length > 13)
                    {
                        count_string++;
                    }
                }
                if (count_string == 0)
                {
                    //здесь необходимо поставить флаг распечатан
                    check.its_print_p(variant);
                    return;
                }
                
                if (check.check_type.SelectedIndex == 1 || !check.itsnew) //|| MainStaticClass.SystemTaxation == 3)//старый механизм работы с макрировкой, для возвратов так же пока старая схема
                {
                    fptr.clearMarkingCodeValidationResult();
                    foreach (ListViewItem lvi in check.listView1.Items)
                    {
                        if (lvi.SubItems[14].Text.Trim().Length > 13)
                        {
                            byte[] textAsBytes = System.Text.Encoding.Default.GetBytes(lvi.SubItems[14].Text.Trim().Replace("vasya2021", "'"));
                            string mark = Convert.ToBase64String(textAsBytes);
                            //if ((MainStaticClass.Version2Marking == 0) || (check.check_type.SelectedIndex == 1) || !check.itsnew || MainStaticClass.SystemTaxation==3)
                            //{
                            //string mark = Convert.ToBase64String(textAsBytes);
                            uint status = 2;

                            // Запускаем проверку КМ
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_TYPE, AtolConstants.LIBFPTR_MCT12_AUTO);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE, mark);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_STATUS, status);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_QUANTITY, 1.000);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MEASUREMENT_UNIT, AtolConstants.LIBFPTR_IU_PIECE);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_PROCESSING_MODE, 0);
                            //fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_FRACTIONAL_QUANTITY, "1/2");
                            fptr.beginMarkingCodeValidation();

                            //// Дожидаемся окончания проверки и запоминаем результат
                            //while (true)
                            //{
                            //    fptr.getMarkingCodeValidationStatus();
                            //    if (fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_VALIDATION_READY))
                            //        break;
                            //}
                            //uint validationResult = fptr.getParamInt(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_ONLINE_VALIDATION_RESULT);
                            DateTime start_check = DateTime.Now;
                            uint validationError = 0;
                            while (true)
                            {
                                fptr.getMarkingCodeValidationStatus();
                                if (fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_VALIDATION_READY))
                                {
                                    break;
                                }
                                else
                                {
                                    MainStaticClass.write_event_in_log(fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_VALIDATION_READY).ToString() + " " + mark, "check_marking_code", check.numdoc.ToString());
                                }
                                if ((DateTime.Now - start_check).Milliseconds > 2000)
                                {
                                    //MessageBox.Show("check_marking_code таймаут при проверки qr кода " + mark);
                                    MainStaticClass.write_event_in_log("Таймаут при gроверкt маркировки " + mark, "check_marking_code", check.numdoc.ToString());
                                    validationError = 421;
                                    break;
                                }
                            }
                            //**************************************************************************
                            validationError = fptr.getParamInt(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_ONLINE_VALIDATION_ERROR);
                            if ((validationError != 0) && (validationError != 402) && (validationError != 421))
                            {
                                error = true;
                                MessageBox.Show("Код ошибки = " + validationError + "; " + fptr.getParamString(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_ONLINE_VALIDATION_ERROR_DESCRIPTION), "Проверка кода маркировки");
                            }
                            //**************************************************************************
                            // Подтверждаем реализацию товара с указанным КМ
                            fptr.acceptMarkingCode();
                            //}
                        }
                    }
                }

            }

            if (check.check_type.SelectedIndex == 0)
            {
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_RECEIPT_TYPE, AtolConstants.LIBFPTR_RT_SELL);
            }
            else if (check.check_type.SelectedIndex == 1)
            {
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_RECEIPT_TYPE, AtolConstants.LIBFPTR_RT_SELL_RETURN);
            }

            bool closing = false;

            // Открытие чека (с передачей телефона получателя)
            if (check.check_type.SelectedIndex == 0)
            {
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_RECEIPT_TYPE, AtolConstants.LIBFPTR_RT_SELL);
            }
            else
            {
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_RECEIPT_TYPE, AtolConstants.LIBFPTR_RT_SELL_RETURN);
            }


            if (check.txtB_email_telephone.Text.Trim().Length > 0)
            {
                fptr.setParam(1008, check.txtB_email_telephone.Text);
            }

            if ((check.txtB_inn.Text.Trim().Length > 0) && (check.txtB_name.Text.Trim().Length > 0))
            {
                fptr.setParam(1228, check.txtB_inn.Text);
                fptr.setParam(1227, check.txtB_name.Text);
            }


            if (fptr.openReceipt() != 0)
            {
                MessageBox.Show(string.Format("Ошибка при открытии чека.\nОшибка {0}: {1}", fptr.errorCode(), fptr.errorDescription()),
                        "Ошибка откртия чека", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //fptr.close();
                return;
            }

            foreach (ListViewItem lvi in check.listView1.Items)
            {

                if (variant == 0)
                {
                    if (lvi.SubItems[14].Text.Trim().Length > 13)
                    {
                        continue;
                    }
                }
                else if (variant == 1)
                {
                    if (lvi.SubItems[14].Text.Trim().Length < 14)
                    {
                        continue;
                    }
                }

                fptr.setParam(AtolConstants.LIBFPTR_PARAM_COMMODITY_NAME, lvi.SubItems[0].Text.Trim() + " " + lvi.SubItems[1].Text.Trim());
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PRICE, lvi.SubItems[5].Text.Replace(",", "."));
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_MEASUREMENT_UNIT, AtolConstants.LIBFPTR_IU_PIECE);
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_QUANTITY, lvi.SubItems[3].Text);

                fptr.setParam(AtolConstants.LIBFPTR_PARAM_TAX_TYPE, AtolConstants.LIBFPTR_TAX_NO);

                if (MainStaticClass.its_excise(lvi.SubItems[0].Text.Trim()) == 0)
                {
                    if (lvi.SubItems[14].Text.Trim().Length <= 13)//код маркировки не заполнен
                    {
                        fptr.setParam(1212, 32);
                    }
                    else
                    {
                        byte[] textAsBytes = System.Text.Encoding.Default.GetBytes(lvi.SubItems[14].Text.Trim().Replace("vasya2021", "'"));
                        string mark = Convert.ToBase64String(textAsBytes);
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE, mark);
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_STATUS, 2);
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_TYPE, AtolConstants.LIBFPTR_MCT12_AUTO);
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_ONLINE_VALIDATION_RESULT, 0);
                        fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_PROCESSING_MODE, 0);
                    }
                }
                else
                {
                    fptr.setParam(1212, 2);//подакцизеый товар
                }
                fptr.registration();
            }

            // Регистрация итога (отбрасываем копейки)
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_SUM, (double)check.calculation_of_the_sum_of_the_document());
            fptr.receiptTotal();

            double[] get_result_payment = check.get_summ1_systemtaxation3(variant);//MainStaticClass.get_cash_on_type_payment(check.numdoc.ToString());
            if (get_result_payment[0] != 0)//Наличные
            {
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PAYMENT_TYPE, AtolConstants.LIBFPTR_PT_CASH);
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PAYMENT_SUM, get_result_payment[0]);
                fptr.payment();
            }
            if (get_result_payment[1] != 0)
            {
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PAYMENT_TYPE, AtolConstants.LIBFPTR_PT_ELECTRONICALLY);
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PAYMENT_SUM, get_result_payment[1]);
                fptr.payment();
            }
            if (get_result_payment[2] != 0)
            {
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PAYMENT_TYPE, AtolConstants.LIBFPTR_PT_PREPAID);
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PAYMENT_SUM, get_result_payment[2]);
                fptr.payment();
            }
            string s = "";
            if (check.check_type.SelectedIndex == 0)//это продажа
            {
                if (check.Discount != 0)
                {
                    s = "Вами получена скидка " + check.calculation_of_the_discount_of_the_document().ToString().Replace(",", ".") + " " + MainStaticClass.get_currency();
                    fptr.beginNonfiscalDocument();

                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_CENTER);
                    fptr.printText();

                    if (check.client.Tag != null)
                    {
                        if (check.client.Tag == check.user.Tag)
                        {
                            s = "ДК: стандартная скидка";
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_LEFT);
                            fptr.printText();
                        }
                        else
                        {
                            s = "ДК: " + check.client.Tag.ToString();
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                            fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_LEFT);
                            fptr.printText();
                        }
                    }

                    s = "ДК: " + MainStaticClass.Nick_Shop + "-" + MainStaticClass.CashDeskNumber.ToString() + "-" + check.numdoc.ToString();// +" кассир " + this.cashier;
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
                    fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_LEFT);
                    fptr.printText();
                    fptr.endNonfiscalDocument();

                }
            }

            // Закрытие чека
            fptr.closeReceipt();

            while (fptr.checkDocumentClosed() < 0)
            {
                // Не удалось проверить состояние документа. Вывести пользователю текст ошибки, попросить устранить неполадку и повторить запрос
                MessageBox.Show(fptr.errorDescription());
                continue;
            }

            if ((!fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_DOCUMENT_CLOSED)) || (error))
            {
                // Документ не закрылся. Требуется его отменить (если это чек) и сформировать заново
                fptr.cancelReceipt();
                //fptr.close();
                return;
            }

            if (!fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_DOCUMENT_PRINTED))
            {
                // Можно сразу вызвать метод допечатывания документа, он завершится с ошибкой, если это невозможно
                while (fptr.continuePrint() < 0)
                {
                    // Если не удалось допечатать документ - показать пользователю ошибку и попробовать еще раз.
                    MessageBox.Show(String.Format("Не удалось напечатать документ (Ошибка \"{0}\"). Устраните неполадку и повторите.", fptr.errorDescription()));
                    continue;
                }
            }

            s = MainStaticClass.Nick_Shop + "-" + MainStaticClass.CashDeskNumber.ToString() + "-" + check.numdoc.ToString();// +" кассир " + this.cashier;
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_TEXT, s);
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_LEFT);
            fptr.printText();

            print_fiscal_advertisement(fptr);
            // Завершение работы
            //fptr.close();
            if (!error)
            {

                check.its_print_p(variant);
                if (variant == 1)
                {
                    check.Close();
                }
            }
        }      


        public bool check_marking_code(string mark,string num_doc)
        {
            bool result = true;
            IFptr fptr = MainStaticClass.FPTR;
            //setConnectSetting(fptr);
            if (!fptr.isOpened())
            {
                fptr.open();
            }

            fptr.setParam(1021, MainStaticClass.Cash_Operator);
            fptr.setParam(1203, MainStaticClass.CashOperatorInn);
            fptr.operatorLogin();

            uint status = 2;

            // Запускаем проверку КМ
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_TYPE, AtolConstants.LIBFPTR_MCT12_AUTO);
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE, mark);
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_STATUS, status);
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_QUANTITY, 1.000);
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MEASUREMENT_UNIT, AtolConstants.LIBFPTR_IU_PIECE);
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_PROCESSING_MODE, 0);
            //fptr.setParam(AtolConstants.LIBFPTR_PARAM_MARKING_FRACTIONAL_QUANTITY, "1/2");
            fptr.beginMarkingCodeValidation();

            // Дожидаемся окончания проверки и запоминаем результат
            //Необходимо установить тайм аут проверки
            DateTime start_check = DateTime.Now;
            uint validationError = 0;
            while (true)
            {
                fptr.getMarkingCodeValidationStatus();
                if (fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_VALIDATION_READY))
                {
                    break;
                }
                else
                {
                    MainStaticClass.write_event_in_log(fptr.getParamBool(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_VALIDATION_READY).ToString() + " "+ mark, "check_marking_code", num_doc);
                }
                if ((DateTime.Now - start_check).Seconds > 2)
                {
                    //MessageBox.Show("check_marking_code таймаут при проверки qr кода " + mark);
                    MainStaticClass.write_event_in_log(" Таймаут при проверке маркировки " + mark, "check_marking_code", num_doc);
                    validationError = 421;
                    break;
                }
            }
            if (validationError == 0)
            {
                //uint validationResult = fptr.getParamInt(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_ONLINE_VALIDATION_RESULT);
                validationError = fptr.getParamInt(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_ONLINE_VALIDATION_ERROR);
            }
            if ((validationError != 0) && (validationError != 402) && (validationError != 421))
            {
                result = false;
                string error_decription = "Код ошибки = " + validationError + "; " + fptr.getParamString(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_ONLINE_VALIDATION_ERROR_DESCRIPTION);
                if (error_decription.Trim().Length > 200)
                {
                    error_decription = error_decription.Trim().Substring(0, 199);
                }
                MessageBox.Show("Код ошибки = " + validationError + "; " + fptr.getParamString(AtolConstants.LIBFPTR_PARAM_MARKING_CODE_ONLINE_VALIDATION_ERROR_DESCRIPTION), "Проверка кода маркировки");
                MainStaticClass.write_event_in_log(error_decription, " Проверка маркировки " + mark, num_doc);
                fptr.declineMarkingCode();
            }
            else
            {
                // Подтверждаем реализацию товара с указанным КМ
                fptr.acceptMarkingCode();
            }
            
            //fptr.close();
            return result;
        }
    }
}
