using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Data;
using Npgsql;
using System.Windows.Forms;
//using System.Threading;


namespace Cash8
{
    class Read_Data_From_Com_Port
    {
        //public Cash_check cc = null;
        public System.IO.Ports.SerialPort mySerial;

        //        public Read_Data_From_Com_Port()
        //        {
        ////            to_read_the_data_from_a_port(); 
        //        }

        public void ReadData()
        {

            byte tmpByte;
            string rxString = "";
            try
            {
                tmpByte = (byte)mySerial.ReadByte();

                while ((char)tmpByte != '\r')
                {
                    if (char.IsDigit(((char)tmpByte)))
                    {
                        rxString += ((char)tmpByte);
                    }
                    tmpByte = (byte)mySerial.ReadByte();
                }
                if (rxString.Length > 0)
                {
                    //lock (MainStaticClass.Barcode)
                    //{
                    MainStaticClass.Barcode = rxString;
                    //}
                }
            }
            catch //(Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //public void insert_barcode_in_database(string barcode)
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        string query = "DELETE FROM temp_barcode;INSERT INTO temp_barcode(barcode)VALUES('" + barcode + "');";
        //        command = new NpgsqlCommand(query, conn);
        //        command.ExecuteNonQuery();
        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {

        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}

        public void to_read_the_data_from_a_port()
        {
            if (mySerial != null)
                if (mySerial.IsOpen)
                    mySerial.Close();
            mySerial = new SerialPort(MainStaticClass.Name_Com_Port.Trim(), 9600);
            mySerial.ReadTimeout = 400;
            try
            {
                mySerial.Open();
            }
            catch//(Exception ex)
            {

            }
            while (MainStaticClass.continue_to_read_the_data_from_a_port)
            {
                try
                {
                    MainStaticClass.Last_Answer_Barcode_Scaner = DateTime.Now;
                    //while (MainStaticClass.continue_to_read_the_data_from_a_port)
                    //{
                    ReadData();
                    //}
                }
                catch
                {
                    //
                }
            }
            //}         

            if (mySerial != null)
                if (mySerial.IsOpen)
                    mySerial.Close();
            mySerial = null;
            GC.Collect();
        }
    }
}
