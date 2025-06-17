using Npgsql;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cash8
{
    class CreateDB
    {
        public static string Create_constants()
        {
            string result = @"
            CREATE TABLE IF NOT EXISTS public.constants
        (
            cash_desk_number smallint NOT NULL,
            nick_shop character(5) COLLATE pg_catalog.default NOT NULL,
            code_shop character varying(36) COLLATE pg_catalog.default,
            path_for_web_service character varying(200) COLLATE pg_catalog.default,
            unloading_period smallint,
            last_date_download_bonus_clients timestamp without time zone,
            system_taxation smallint NOT NULL DEFAULT 0,
            version_fn smallint,
            id_acquirer_terminal character varying(8) COLLATE pg_catalog.default,
            ip_address_acquiring_terminal character varying(21) COLLATE pg_catalog.default,
            webservice_authorize boolean DEFAULT false,
            printing_using_libraries boolean DEFAULT false,
            fn_serial_port character varying(20) COLLATE pg_catalog.default,
            cdn_token character varying(36) COLLATE pg_catalog.default,
            enable_cdn_markers boolean DEFAULT false,
            scale_serial_port character varying(20) COLLATE pg_catalog.default,
            get_weight_automatically boolean DEFAULT false,
            variant_connect_fn smallint NOT NULL DEFAULT 0,
            fn_ipaddr character varying(20) COLLATE pg_catalog.default,
            acquiring_bank smallint DEFAULT 0,
            constant_conversion_to_kilograms integer NOT NULL DEFAULT 0
        )

        TABLESPACE pg_default;

            ALTER TABLE IF EXISTS public.constants
                OWNER to postgres;

            COMMENT ON COLUMN public.constants.cash_desk_number
                IS 'Номер кассы в магазине';

        COMMENT ON COLUMN public.constants.nick_shop
            IS 'Код магазина';

        COMMENT ON COLUMN public.constants.code_shop
            IS 'Уникальный номер БД';

        COMMENT ON COLUMN public.constants.path_for_web_service
            IS 'Путь к веб сервису';

        COMMENT ON COLUMN public.constants.unloading_period
            IS 'Период выгрузки в минутах';

        COMMENT ON COLUMN public.constants.last_date_download_bonus_clients
            IS 'Дата последней удачной загрузки клиентов';

        COMMENT ON COLUMN public.constants.system_taxation
            IS 'Система налогообложения';

        COMMENT ON COLUMN public.constants.version_fn
            IS 'Версия ФН в ФР';

        COMMENT ON COLUMN public.constants.id_acquirer_terminal
            IS 'Ид банковского терминала';

        COMMENT ON COLUMN public.constants.ip_address_acquiring_terminal
            IS 'Ип адрес банковского терминала';

        COMMENT ON COLUMN public.constants.webservice_authorize
            IS 'Признак того что веб сервис с авторизацией';

        COMMENT ON COLUMN public.constants.printing_using_libraries
            IS 'Печать с использованием библиотек';

        COMMENT ON COLUMN public.constants.fn_serial_port
            IS 'Номер порта ФР';

        COMMENT ON COLUMN public.constants.cdn_token
            IS 'Номер cdn токена для проверки кодов маркировки';

        COMMENT ON COLUMN public.constants.enable_cdn_markers
            IS 'Разрешить проверку в cdn кодов маркировки';

        COMMENT ON COLUMN public.constants.scale_serial_port
            IS 'Номер порта для весов';

        COMMENT ON COLUMN public.constants.get_weight_automatically
            IS 'Считывать вес автоматически для весового товара';

        COMMENT ON COLUMN public.constants.variant_connect_fn
            IS 'Вариант соединения компьютера с ФР';

        COMMENT ON COLUMN public.constants.fn_ipaddr
            IS 'Ип адрес ФР';

        COMMENT ON COLUMN public.constants.acquiring_bank
            IS 'Банк которму принадлежит банковский терминал';

        COMMENT ON COLUMN public.constants.constant_conversion_to_kilograms
            IS 'Множитель  для конвертации значения полученного из весов ';";

            return result;
        }

        public static void delete_inactive_old_column()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            string connectionString = conn.ConnectionString;
            conn.Dispose();

            // Получаем имя таблицы из скрипта
            string tableName = GetTableNameFromScript(Create_constants());

            // Получаем колонки из скрипта
            List<string> scriptColumns = GetScriptColumns(Create_constants());

            // Получаем колонки из БД
            List<string> dbColumns = GetDatabaseColumns(connectionString, tableName);

            // Находим лишние колонки
            List<string> columnsToDrop = new List<string>();
            foreach (string column in dbColumns)
            {
                if (!scriptColumns.Contains(column))
                {
                    columnsToDrop.Add(column);
                }
            }

            // Удаляем лишние колонки
            if (columnsToDrop.Count > 0)
            {
                DropExtraColumns(connectionString, tableName, columnsToDrop);
            }
            else
            {
                Console.WriteLine("Лишних колонок нет.");
            }
        }

        /// <summary>
        /// Извлечение имени таблицы из скрипта
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        static string GetTableNameFromScript(string script)
        {
            string pattern = @"CREATE\s+TABLE\s+IF\s+NOT\s+EXISTS\s+(\w+\.\w+)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = regex.Match(script);

            if (match.Success)
            {
                return match.Groups[1].Value; // Полное имя таблицы (например, "public.constants")
            }

            throw new InvalidOperationException("Не удалось найти имя таблицы в скрипте.");
        }

        /// <summary>
        /// Получение списка колонок из скрипта
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        static List<string> GetScriptColumns(string script)
        {
            string pattern = @"\b(\w+)\s+(smallint|character|varchar|text|timestamp|boolean|integer)\b";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(script);
            List<string> columns = new List<string>();

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    columns.Add(match.Groups[1].Value);
                }
            }

            return columns;
        }

        /// <summary>
        /// Получение списка колонок из БД
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        static List<string> GetDatabaseColumns(string connectionString, string tableName)
        {
            List<string> columns = new List<string>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = $@"
                SELECT column_name
                FROM information_schema.columns
                WHERE table_name = '{tableName.Split('.')[1]}' AND table_schema = '{tableName.Split('.')[0]}';
            ";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return columns;
        }

        /// <summary>
        /// Удаление из БД тех колонок которых нет в скрипте
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <param name="columnsToDrop"></param>
        static void DropExtraColumns(string connectionString, string tableName, List<string> columnsToDrop)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                foreach (string column in columnsToDrop)
                {
                    string query = $@"
                    ALTER TABLE {tableName}
                    DROP COLUMN IF EXISTS {column};
                ";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                            Console.WriteLine($"Удалена колонка: {column}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при удалении колонки {column}: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}