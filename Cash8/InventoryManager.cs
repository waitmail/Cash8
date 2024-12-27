using System;
using System.Collections.Generic;
using Npgsql;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cash8
{
    public static class InventoryManager
    {
        private static Dictionary<long, ProductData> dictionaryProductData = new Dictionary<long, ProductData>();
        public static bool complete = false;
        //public static int rowCount = 0;
        //public static int rowCountCurrent = 0;

        public static Dictionary<long, ProductData> DictionaryProductData
        {
            get => dictionaryProductData;
        }

        public static void ClearDictionaryProductData()
        {
            complete = false;
            dictionaryProductData.Clear();
        }
        
        public static async Task FillDictionaryProductDataAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    FillDictionaryProductData();
                }
                catch (Exception ex)
                {
                    // Перехват исключения и передача его в основной поток
                    if (Application.OpenForms.Count > 0)
                    {
                        var mainForm = Application.OpenForms[0];
                        mainForm.Invoke(new MethodInvoker(() =>
                        {
                            MessageBox.Show($"Произошла ошибка: {ex.Message}", "Работа с кешем товаров");
                        }));
                    }
                }
            });
        }
        
        public static bool FillDictionaryProductData()
        {
            bool result = true;

            dictionaryProductData.Clear();

            using (var conn = MainStaticClass.NpgsqlConn())
            {
                try
                {                    
                    //string countQuery = "SELECT COUNT(*) FROM tovar " +
                    //    "LEFT JOIN barcode ON tovar.code=barcode.tovar_code " +
                    //    "WHERE tovar.its_deleted = 0 AND tovar.retail_price<>0";

                    conn.Open();
                    //using (var countCommand = new NpgsqlCommand(countQuery, conn))
                    //{
                    //    rowCount = Convert.ToInt32(countCommand.ExecuteScalar());
                    //}

                    string query = "SELECT tovar.code, tovar.name, tovar.retail_price, tovar.its_certificate, tovar.its_marked, tovar.cdn_check, tovar.fractional, barcode.barcode FROM tovar " +
                    "LEFT JOIN barcode ON tovar.code=barcode.tovar_code WHERE tovar.its_deleted = 0 AND tovar.retail_price<>0";

                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            //rowCountCurrent = 0;
                            while (reader.Read())
                            {
                                //rowCountCurrent++;
                                long code = Convert.ToInt64(reader["code"]);

                                ProductFlags flags = ProductFlags.None;
                                // Создаем флаги на основе значений из базы данных                            
                                if (Convert.ToBoolean(reader["its_certificate"])) flags |= ProductFlags.Certificate;
                                if (Convert.ToBoolean(reader["its_marked"])) flags |= ProductFlags.Marked;
                                if (Convert.ToBoolean(reader["cdn_check"])) flags |= ProductFlags.CDNCheck;
                                if (Convert.ToBoolean(reader["fractional"])) flags |= ProductFlags.Fractional;

                                if (!dictionaryProductData.TryGetValue(code, out _))
                                {
                                    var productData = new ProductData(code, reader["name"].ToString().Trim(), Convert.ToDecimal(reader["retail_price"]), flags);

                                    AddItem(code, productData);
                                }

                                string barcode = reader["barcode"].ToString().Trim();
                                if (!(string.IsNullOrEmpty(barcode) || dictionaryProductData.TryGetValue(Convert.ToInt64(barcode), out _)))
                                {
                                    var productData = new ProductData(Convert.ToInt64(code), reader["name"].ToString().Trim(), Convert.ToDecimal(reader["retail_price"]), flags);
                                    AddItem(Convert.ToInt64(barcode), productData);
                                }
                            }
                        }
                    }
                }
                catch (NpgsqlException ex)
                {
                    //MessageBox.Show($"Произошли ошибки при заполнении словаря данными о товарах: {ex.Message}", "Заполнение кеша товаров");
                    throw new Exception($"При заполнении словаря данными о товарах: {ex.Message}", ex);
                    //result = false;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Произошли ошибки при заполнении словаря данными о товарах: {ex.Message}", "Заполенние кеша товаров");
                    throw new Exception($"При заполнении словаря данными о товарах: {ex.Message}", ex);
                    //result = false;
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
                complete = result;
                return result;
            }
        }       
        
        public static void AddItem(long id, ProductData data)
        {
            if (!dictionaryProductData.ContainsKey(id))
            {
                dictionaryProductData.Add(id, data);
            }
            else
            {
                throw new ArgumentException($"Товар с идентификатором {id} уже существует.");
            }
        }
        
        public static ProductData GetItem(long id)
        {
            if (dictionaryProductData.TryGetValue(id, out var data))
            {
                return data;
            }
            else
            {
                return new ProductData(0, string.Empty, 0, ProductFlags.None);
            }
        }
    }
}
