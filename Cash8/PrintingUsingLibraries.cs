using Atol.Drivers10.Fptr;
using System.Windows.Forms;
using AtolConstants = Atol.Drivers10.Fptr.Constants;
using System;

namespace Cash8
{
    class PrintingUsingLibraries
    {

        public bool print_sale(ListView listView)
        {
            bool result = true;





            return result;
        }


        private void setConnectSetting(IFptr fptr)
        {
            fptr.setSingleSetting(AtolConstants.LIBFPTR_SETTING_MODEL, AtolConstants.LIBFPTR_MODEL_ATOL_AUTO.ToString());
            fptr.setSingleSetting(AtolConstants.LIBFPTR_SETTING_PORT, AtolConstants.LIBFPTR_PORT_COM.ToString());
            fptr.setSingleSetting(AtolConstants.LIBFPTR_SETTING_COM_FILE, "COM13");
            fptr.setSingleSetting(AtolConstants.LIBFPTR_SETTING_BAUDRATE, AtolConstants.LIBFPTR_PORT_BR_115200.ToString());
            fptr.applySingleSettings();
        }

        public void getShiftStatus()
        {
            IFptr fptr = MainStaticClass.FPTR;          
            setConnectSetting(fptr);
            fptr.open();            
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
            fptr.close();
        }

        public string  getFiscallInfo()
        {
            string fn_info = "";

            IFptr fptr = MainStaticClass.FPTR;
            setConnectSetting(fptr);
            fptr.open();
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

            fptr.close();

            return fn_info;
        }

        public string ofdExchangeStatus()
        {
            string result = "";

            IFptr fptr = MainStaticClass.FPTR;
            setConnectSetting(fptr);
            fptr.open();           

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

            fptr.close();

            return result;
        }

        public string getCasheSumm()
        {
            string result = "";
            IFptr fptr = MainStaticClass.FPTR;
            setConnectSetting(fptr);
            fptr.open();
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_DATA_TYPE, AtolConstants.LIBFPTR_DT_CASH_SUM);
            fptr.queryData();
            double cashSum = fptr.getParamDouble(AtolConstants.LIBFPTR_PARAM_SUM);
            if (fptr.errorCode() != 0)
            {
                MessageBox.Show("Ошибка при получении суммы наличных в кассе  " + fptr.errorDescription());
            }
            fptr.close();
            result = cashSum.ToString().Replace(",",".");            
            return result;
        }

        public void cashOutcome(double sumCashOut)
        {
            IFptr fptr = MainStaticClass.FPTR;
            setConnectSetting(fptr);
            fptr.open();
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_SUM,sumCashOut);
            fptr.cashOutcome();
            if (fptr.errorCode() != 0)
            {
                MessageBox.Show("Ошибка при инкасации  "  + fptr.errorDescription());
            }
            fptr.close();
        }

        public void cashIncome(double sumCashIn)
        {
            IFptr fptr = MainStaticClass.FPTR;
            setConnectSetting(fptr);
            fptr.open();
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_SUM, sumCashIn);
            fptr.cashIncome();
            if (fptr.errorCode() != 0)
            {
                MessageBox.Show("Ошибка при внесении  " + fptr.errorDescription());
            }
            fptr.close();
        }

        public void reportX()
        {
            IFptr fptr = MainStaticClass.FPTR;
            setConnectSetting(fptr);
            fptr.open();
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_REPORT_TYPE, AtolConstants.LIBFPTR_RT_X);
            if (fptr.report() != 0)
            {
                MessageBox.Show(string.Format("Ошибка при X отчете.\nОшибка {0}: {1}", fptr.errorCode(), fptr.errorDescription()),
                    "Ошибка при X отчете", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            fptr.close();
        }

        public void reportZ()
        {
            IFptr fptr = MainStaticClass.FPTR;
            setConnectSetting(fptr);
            fptr.open();
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_REPORT_TYPE, AtolConstants.LIBFPTR_RT_CLOSE_SHIFT);
            if (fptr.report() != 0)
            {
                MessageBox.Show(string.Format("Ошибка при закрытии смены.\nОшибка {0}: {1}", fptr.errorCode(), fptr.errorDescription()),
                    "Ошибка закрытия смены", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }                        
            fptr.close();
        }
    }
}
