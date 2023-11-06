using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using System.Security.Cryptography;
using System.IO;
using Npgsql;
using System.Security.Cryptography;


namespace Cash8
{
    public partial class SettingConnect : Form
    {
        private bool fileSettinConnect = false;

        //private byte[] EncryptedSymmetricKey = { 214, 46, 220, 83, 160, 73, 40, 39, 201, 155, 19, 202, 3, 11, 191, 178, 56, 74, 90, 36, 248, 103, 18, 144, 170, 163, 145, 87, 54, 61, 34, 220 };
        //private byte[] EncryptedSymmetricIV  = { 207, 137, 149, 173, 14, 92, 120, 206, 222, 158, 28, 40, 24, 30, 16, 175 };

        public SettingConnect()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }


        void textBox2_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {

            if (e.KeyChar == 13)
            {
                if (fileSettinConnect == false)
                {
                    //if (textBox2.Text.Trim().ToUpper() == "SERVICEMODE")//прошли авторизацию
                    if (textBox2.Text.Trim().ToUpper() == "1")//прошли авторизацию
                    {
                        //сделать видимыми реквизиты 
                        changeVisibleBeforeWrite();
                    }
                    else
                    {
                        MessageBox.Show("Пароль неправильный");
                    }
                }
                else
                {
                    if (servicePassword.Text.Trim() == textBox2.Text.Trim())
                    {
                        changeVisibleBeforeWrite();
                    }
                    else
                    {
                        MessageBox.Show("Неправильный пароль");
                    }

                }
                textBox2.Text = "";
            }
        }
        private void changeVisibleBeforeWrite()
        {
            button4.Visible = true;
            button2.Visible = true;
            label1.Visible = true;
            ipaddrServer.Visible = true;
            label2.Visible = true;
            nameDataBase.Visible = true;
            textBox2.Visible = false;
            label3.Visible = false;
            button1.Visible = false;
            servicePassword.Visible = true;
            label5.Visible = true;
            label6.Visible = true;
            repeatPassword.Visible = true;
            checkBox1.Visible = true;
            checkBox1.Checked = true;
            groupBox1.Visible = true;
            groupBox2.Visible = true;
            postgresUser.Visible = true;
            portServer.Visible = true;
            portPostgres.Visible = true;
            userPostrgres.Visible = true;
            checkBox2.Visible = true;
            checkBox2.Checked = true;
        }
        private void changeVisibleAfterWrite()
        {
            button4.Visible = false;
            button2.Visible = false;
            label1.Visible = false;
            ipaddrServer.Visible = false;
            label2.Visible = false;
            nameDataBase.Visible = false;
            textBox2.Visible = true;
            label3.Visible = true;
            button1.Visible = true;
            servicePassword.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            repeatPassword.Visible = false;
            checkBox1.Visible = false;
            checkBox1.Checked = false;
            groupBox1.Visible = false;
            groupBox2.Visible = false;
            postgresUser.Visible = false;
            portServer.Visible = false;
            portPostgres.Visible = false;
            userPostrgres.Visible = false;
        }
        private void button1_Click(object sender, EventArgs e)//Сверить с паролем
        {
            if (fileSettinConnect == false)
            {
                //if (textBox2.Text.Trim().ToUpper() == "SERVICEMODE")//прошли авторизацию
                if (textBox2.Text.Trim().ToUpper() == "1")//прошли авторизацию
                {
                    //сделать видимыми реквизиты 
                    changeVisibleBeforeWrite();
                }
                else
                {
                    MessageBox.Show("Пароль неправильный");
                }
            }
            else
            {
                if (servicePassword.Text.Trim() == textBox2.Text.Trim())
                {
                    changeVisibleBeforeWrite();
                }
                else
                {
                    MessageBox.Show("Неправильный пароль");
                }

            }
            textBox2.Text = "";
        }
        private Byte[] ConvertStringToByteArray(String s)
        {
            return (new UnicodeEncoding()).GetBytes(s);
        }
        private string ConvertByteArrayToString(Byte[] bytes)
        {
            return (new UnicodeEncoding()).GetString(bytes);
        }
        void SettingConnect_Load(object sender, System.EventArgs e)
        {           
            fileSettinConnect = File.Exists(Application.StartupPath + "/Setting.gaa");
            if (fileSettinConnect == true)//файл с параметрами есть заполнить реквизиты
            {
                StringReader stReader = Cash8.MainStaticClass.DecryptData(Application.StartupPath + "/Setting.gaa");
                fillDialog(stReader);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void fillDialog(StringReader sr)
        {
            string line; int etap = 0;
            while ((line = sr.ReadLine()) != null)
            {
                if (line == "[ip адрес сервера]")
                {
                    etap = 1;
                    continue;
                }
                if (line == "[имя базы данных]")
                {
                    etap = 2;
                    continue;
                }
                if (line == "[сервисный пароль]")
                {
                    etap = 3;
                    continue;
                }
                if (line == "[порт сервера]")
                {
                    etap = 4;
                    continue;
                }
                if (line == "[пароль postgres]")
                {
                    etap = 5;
                    continue;
                }
                if (line == "[пользователь postgres]")
                {
                    etap = 6;
                    continue;
                }


                if (etap == 1)
                {
                    ipaddrServer.Text = line;
                    etap = 0;
                }
                if (etap == 2)
                {
                    nameDataBase.Text = line;
                    etap = 0;
                }
                if (etap == 3)
                {
                    servicePassword.Text = line;
                    repeatPassword.Text = line;
                    etap = 0;
                }
                if (etap == 4)
                {
                    portServer.Text = line;
                    etap = 0;
                }
                if (etap == 5)
                {
                    passwordPostgres.Text = line;
                    repeatPasswordPostgres.Text = line;
                    etap = 0;
                }
                if (etap == 6)
                {
                    postgresUser.Text = line;
                    etap = 0;
                }
            }
        }
        private bool getValidateDialogControl()
        {
            if (ipaddrServer.Text.Trim().Length == 0)
            {
                MessageBox.Show("ip адрес сервера пустой");
                return false;
            }
            if (nameDataBase.Text.Trim().Length == 0)
            {
                MessageBox.Show("имя базы данных пустое");
                return false;
            }
            if (servicePassword.Text.Trim().Length == 0)
            {
                MessageBox.Show("сервисный пароль не заполнен");
                return false;
            }
            if (repeatPassword.Text.Trim().Length == 0)
            {
                MessageBox.Show("подтверждение сервисного пароля не заполнено");
                return false;
            }
            if (servicePassword.Text.Trim() != repeatPassword.Text.Trim())
            {
                MessageBox.Show("сервисный пароль не совпадает с подтверждением");
                return false;
            }
            if (postgresUser.Text.Trim().Length == 0)
            {
                MessageBox.Show("пользователь postgres пустой");
                return false;
            }
            if (portServer.Text.Trim().Length == 0)
            {
                MessageBox.Show("порт сервера пустой");
                return false;
            }
            if (passwordPostgres.Text.Trim().Length == 0)
            {
                MessageBox.Show("пароль к серверу пустой");
                return false;
            }
            if (repeatPasswordPostgres.Text.Trim().Length == 0)
            {
                MessageBox.Show("подтверждение пароля к серверу пустое");
                return false;
            }
            if (passwordPostgres.Text.Trim() != repeatPasswordPostgres.Text.Trim())
            {
                MessageBox.Show("пароль к серверу не совпадает с подтверждением");
                return false;
            }

            //passwordPostgres
            //repeatPasswordPostgres



            return true;
        }
        private void button2_Click(object sender, EventArgs e)//записать параметры в файл
        {

            if (!getValidateDialogControl())
            {
                return;
            }

            string f = "";
            f += "[ip адрес сервера]";
            f += "\r\n";
            f += ipaddrServer.Text.Trim();
            f += "\r\n";
            f += "[имя базы данных]";
            f += "\r\n";
            f += nameDataBase.Text.Trim();
            f += "\r\n";
            f += "[сервисный пароль]";
            f += "\r\n";
            f += servicePassword.Text.Trim();
            f += "\r\n";
            f += "[порт сервера]";
            f += "\r\n";
            f += portServer.Text.Trim();
            f += "\r\n";
            f += "[пароль postgres]";
            f += "\r\n";
            f += passwordPostgres.Text.Trim();
            f += "\r\n";
            f += "[пользователь postgres]";
            f += "\r\n";
            f += postgresUser.Text.Trim();

            Cash8.MainStaticClass.EncryptData(Application.StartupPath + "/Setting.gaa", f);
            //using (StreamWriter sw = new StreamWriter(Application.StartupPath + "/SettingCopy.gaa"))
            //{
            //    sw.WriteLine(f);
            //}
            changeVisibleAfterWrite();

            //Cash8.MainStaticClass.loadConfig(Application.StartupPath + "/Setting.gaa");
            if (fileSettinConnect == true)//файл с параметрами есть заполнить реквизиты
            {
                StringReader stReader = Cash8.MainStaticClass.DecryptData(Application.StartupPath + "/Setting.gaa");
                fillDialog(stReader);
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                servicePassword.PasswordChar = Convert.ToChar("*");
                repeatPassword.PasswordChar = Convert.ToChar("*");
            }
            else
            {
                servicePassword.PasswordChar = Convert.ToChar(0);
                repeatPassword.PasswordChar = Convert.ToChar(0);
            }
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                passwordPostgres.PasswordChar = Convert.ToChar("*");
                repeatPasswordPostgres.PasswordChar = Convert.ToChar("*");
            }
            else
            {
                passwordPostgres.PasswordChar = Convert.ToChar(0);
                repeatPasswordPostgres.PasswordChar = Convert.ToChar(0);
            }

        }
        private void CreateDataTables()
        {
            NpgsqlConnection conn = Cash8.MainStaticClass.NpgsqlConn();
            NpgsqlCommand command = null;
            NpgsqlTransaction trans = null;
                        
            try
            {
                string[] query=new string[25];
                //query[0]  = "CREATE TABLE action_clients(num_doc integer,code_client character(10))WITH (OIDS=FALSE);ALTER TABLE action_clients  OWNER TO postgres";
                query[0] = "CREATE TABLE public.action_clients(num_doc integer, code_client character(10) COLLATE pg_catalog.default)WITH(OIDS = FALSE)TABLESPACE pg_default; ALTER TABLE public.action_clients OWNER to postgres; CREATE UNIQUE INDEX code_client_num_doc    ON public.action_clients USING btree(code_client COLLATE pg_catalog.default ASC NULLS LAST, num_doc ASC NULLS LAST)    TABLESPACE pg_default; ALTER TABLE public.action_clients CLUSTER ON code_client_num_doc;";
                query[1] = "CREATE TABLE public.action_header(    date_started date,    date_end date,    num_doc integer NOT NULL,    tip smallint,    barcode character(13) COLLATE pg_catalog.default,    persent numeric(5,2),    sum numeric(10,2),    comment character(100) COLLATE pg_catalog.default,    code_tovar bigint,    marker smallint,    action_by_discount boolean DEFAULT false,    time_start integer,    time_end integer,    bonus_promotion smallint,    with_old_promotion smallint,    monday smallint,    tuesday smallint,    wednesday smallint,    thursday smallint,    friday smallint,    saturday smallint,    sunday smallint,    promo_code smallint,    sum_bonus smallint,    execution_order smallint,    gift_price numeric(10,2) NOT NULL,    kind smallint NOT NULL DEFAULT 0)WITH(    OIDS = FALSE)TABLESPACE pg_default;        ALTER TABLE public.action_header            OWNER to postgres;        COMMENT ON COLUMN public.action_header.code_tovar            IS 'код товара';        COMMENT ON COLUMN public.action_header.marker            IS 'если значение заполнено в 1 то будет показано окно ввода штрихкода товара';        COMMENT ON COLUMN public.action_header.time_start             IS 'Время начала действия акции';        COMMENT ON COLUMN public.action_header.time_end            IS 'Время окончания действия акции';        COMMENT ON COLUMN public.action_header.bonus_promotion             IS '1 - это бонусная акция, иначе - моментальная (старого типа)';         COMMENT ON COLUMN public.action_header.with_old_promotion             IS 'пересечение с моментальными акциями: 1 - да, иначе - нет';        COMMENT ON COLUMN public.action_header.monday            IS 'акция работает по понедельникам';        COMMENT ON COLUMN public.action_header.tuesday            IS 'акция работает по вторникам';        COMMENT ON COLUMN public.action_header.wednesday            IS 'акция работает по средам';        COMMENT ON COLUMN public.action_header.thursday             IS 'акция работает по четвергам';         COMMENT ON COLUMN public.action_header.friday             IS 'акция работает по пятницам';        COMMENT ON COLUMN public.action_header.saturday            IS 'акция работает по субботам';        COMMENT ON COLUMN public.action_header.sunday            IS 'акция работает по воскресеньям';        COMMENT ON COLUMN public.action_header.promo_code            IS 'промо-код при озвучивании которого может сработать бонусная акция, если не заполнен, то не ограничивать';        COMMENT ON COLUMN public.action_header.kind            IS '0 - обычная автоматическая 1 - активируемая штрихкодом 2 - обычная автоматическая по картам';";
                //query[1]  = "CREATE TABLE action_header(date_started date,date_end date,num_doc integer NOT NULL, tip smallint,  barcode character(13),  persent numeric(5,2),  sum numeric(10,2),  comment character(100),  code_tovar integer,  marker smallint,  action_by_discount boolean DEFAULT false,  time_start integer,  time_end integer,  bonus_promotion smallint,  with_old_promotion smallint,  monday smallint,  tuesday smallint,  wednesday smallint,  thursday smallint,  friday smallint,  saturday smallint,  sunday smallint,  promo_code smallint,sum_bonus smallint)WITH (  OIDS=FALSE);ALTER TABLE action_header  OWNER TO postgres;COMMENT ON COLUMN action_header.code_tovar IS 'код товара';COMMENT ON COLUMN action_header.marker IS 'если значение заполнено в 1 то будет показано окно ввода штрихкода товара';COMMENT ON COLUMN action_header.time_start IS 'Время начала действия акции';COMMENT ON COLUMN action_header.time_end IS 'Время окончания действия акции';COMMENT ON COLUMN action_header.bonus_promotion IS '1 - это бонусная акция, иначе - моментальная (старого типа)';COMMENT ON COLUMN action_header.with_old_promotion IS 'пересечение с моментальными акциями: 1 - да, иначе - нет';COMMENT ON COLUMN action_header.monday IS 'акция работает по понедельникам';COMMENT ON COLUMN action_header.tuesday IS 'акция работает по вторникам';COMMENT ON COLUMN action_header.wednesday IS 'акция работает по средам';COMMENT ON COLUMN action_header.thursday IS 'акция работает по четвергам';COMMENT ON COLUMN action_header.friday IS 'акция работает по пятницам';COMMENT ON COLUMN action_header.saturday IS 'акция работает по субботам';COMMENT ON COLUMN action_header.sunday IS 'акция работает по воскресеньям';COMMENT ON COLUMN action_header.promo_code IS 'промо-код при озвучивании которого может сработать бонусная акция, если не заполнен, то не ограничивать';";
                //query[2] = "CREATE TABLE action_table(num_doc integer NOT NULL,  num_list integer,  code_tovar integer, CONSTRAINT action_table_code_tovar_fkey FOREIGN KEY (code_tovar)      REFERENCES tovar (code) MATCH SIMPLE      ON UPDATE NO ACTION ON DELETE NO ACTION)WITH (  OIDS=FALSE);ALTER TABLE action_table  OWNER TO postgres;COMMENT ON COLUMN action_table.code_tovar IS 'Код товара из таблицы товары';";
                query[2] = "CREATE TABLE public.action_table(    num_doc integer NOT NULL,    num_list integer,    code_tovar bigint,    price numeric(10,2),    CONSTRAINT action_table_code_tovar_fkey FOREIGN KEY(code_tovar)        REFERENCES public.tovar(code) MATCH SIMPLE        ON UPDATE NO ACTION        ON DELETE NO ACTION)WITH(    OIDS = FALSE)TABLESPACE pg_default;        ALTER TABLE public.action_table            OWNER to postgres;        COMMENT ON COLUMN public.action_table.code_tovar            IS 'Код товара из таблицы товары';";
                //query[3] = "CREATE TABLE advertisement(advertisement_text character varying(1000), num_str smallint)WITH (  OIDS=FALSE);ALTER TABLE advertisement  OWNER TO postgres;";
                query[3] = "CREATE TABLE public.advertisement(    advertisement_text character varying(1000) COLLATE pg_catalog.default,    num_str smallint)WITH(    OIDS = FALSE)TABLESPACE pg_default;        ALTER TABLE public.advertisement            OWNER to postgres;";
                //query[5]  = "CREATE TABLE barcode(  barcode character(13) NOT NULL,  tovar_code integer NOT NULL,  CONSTRAINT barcode_tovar_code_fkey FOREIGN KEY (tovar_code)      REFERENCES tovar (code) MATCH SIMPLE      ON UPDATE NO ACTION ON DELETE NO ACTION)WITH (  OIDS=FALSE);ALTER TABLE barcode  OWNER TO postgres;CREATE INDEX _barcode_tovar_code_  ON barcode  USING btree  (barcode COLLATE pg_catalog.default, tovar_code);";
                query[4] = "CREATE TABLE public.barcode(barcode character(13) COLLATE pg_catalog.default NOT NULL, tovar_code bigint NOT NULL, CONSTRAINT barcode_tovar_code_fkey FOREIGN KEY (tovar_code) REFERENCES public.tovar(code) MATCH SIMPLE        ON UPDATE NO ACTION        ON DELETE NO ACTION)WITH(OIDS = FALSE)TABLESPACE pg_default; ALTER TABLE public.barcode OWNER to postgres; CREATE INDEX _barcode_tovar_code_ ON public.barcode USING btree(barcode COLLATE pg_catalog.default ASC NULLS LAST, tovar_code ASC NULLS LAST)    TABLESPACE pg_default;";
                //CREATE TABLE public.bonus_cards(    code character varying(10) COLLATE pg_catalog."default",    pin character varying(11) COLLATE pg_catalog."default")WITH(    OIDS = FALSE)TABLESPACE pg_default;        ALTER TABLE public.bonus_cards            OWNER to postgres;
                query[5] = "CREATE TABLE characteristic(tovar_code integer,name character varying(100),guid character varying(36),retail_price_characteristic numeric(10,2),opt_price numeric(10,2))WITH (OIDS=FALSE);ALTER TABLE characteristic OWNER TO postgres;";
                                                                            
                //query[7] = "CREATE TABLE checks_header(  document_number bigint,  client character(10),  cash_desk_number smallint NOT NULL,  comment character(50),  cash numeric(10,2) NOT NULL,  remainder numeric(10,2),  date_time_write timestamp without time zone NOT NULL,  date_time_start timestamp without time zone NOT NULL,  discount numeric(10,2) DEFAULT 0,  its_deleted numeric(1,0) NOT NULL,  action_num_doc integer,  check_type smallint,  have_action boolean,  discount_it_is_sent boolean,  its_print boolean,  is_sent smallint,  cash_money numeric(10,2),  non_cash_money numeric(10,2),  sertificate_money numeric(10,2),  sales_assistant character varying(11),  bonus_is_on smallint,  autor integer,  bonuses_it_is_written_off numeric(8,2) DEFAULT 0,  bonuses_it_is_counted numeric(8,2) DEFAULT 0,  rouble boolean,  type_pay smallint)WITH (  OIDS=FALSE);ALTER TABLE checks_header  OWNER TO postgres;COMMENT ON COLUMN checks_header.discount_it_is_sent IS 'Данные (если начислен или списан этим документом) успешно отправлены в центр';COMMENT ON COLUMN checks_header.its_print IS 'Признак того что чек был нормально напечатан на фискальном принтере';COMMENT ON COLUMN checks_header.is_sent IS '0 - не отправлен 1 - отправлен';CREATE INDEX _checks_header_date_time_write_  ON checks_header  USING btree  (date_time_write);CREATE UNIQUE INDEX checks_header_document_number_idx  ON checks_header  USING btree  (document_number);                ALTER TABLE checks_header CLUSTER ON checks_header_document_number_idx;CREATE UNIQUE INDEX cheks_header_date_time_start  ON checks_header  USING btree  (date_time_start);";
                query[6] = "CREATE TABLE checks_header(  document_number bigint,  client character(10),  cash_desk_number smallint NOT NULL,  comment character(50),  cash numeric(10,2) NOT NULL,  remainder numeric(10,2),  date_time_write timestamp without time zone NOT NULL,  date_time_start timestamp without time zone NOT NULL,  discount numeric(10,2) DEFAULT 0,  its_deleted numeric(1,0) NOT NULL,  action_num_doc integer,  check_type smallint,  have_action boolean,  discount_it_is_sent boolean,  its_print boolean,  is_sent smallint,  cash_money numeric(10,2),  non_cash_money numeric(10,2),  sertificate_money numeric(10,2),  sales_assistant character varying(11),  bonus_is_on smallint,  autor integer,  bonuses_it_is_written_off numeric(8,2) DEFAULT 0,  bonuses_it_is_counted numeric(8,2) DEFAULT 0)WITH (  OIDS=FALSE);ALTER TABLE checks_header  OWNER TO postgres;COMMENT ON COLUMN checks_header.discount_it_is_sent IS 'Данные (если начислен или списан этим документом) успешно отправлены в центр';COMMENT ON COLUMN checks_header.its_print IS 'Признак того что чек был нормально напечатан на фискальном принтере';COMMENT ON COLUMN checks_header.is_sent IS '0 - не отправлен 1 - отправлен';CREATE INDEX _checks_header_date_time_write_  ON checks_header  USING btree  (date_time_write);CREATE UNIQUE INDEX checks_header_document_number_idx  ON checks_header  USING btree  (document_number);ALTER TABLE checks_header CLUSTER ON checks_header_document_number_idx;CREATE UNIQUE INDEX cheks_header_date_time_start  ON checks_header  USING btree  (date_time_start);";
                query[7]  = "CREATE TABLE checks_table(  document_number bigint NOT NULL,  tovar_code integer NOT NULL,  quantity integer NOT NULL,  price numeric(10,2) NOT NULL,  price_at_a_discount numeric(10,2),  sum numeric(10,2) NOT NULL,  sum_at_a_discount numeric(10,2) NOT NULL,  numstr integer NOT NULL,  action_num_doc integer,   action_num_doc1 integer,  action_num_doc2 integer,  characteristic character varying(36),  bonus_standard numeric(8,2),  bonus_promotion numeric(8,2),  promotion_b_mover integer,  CONSTRAINT checks_table_tovar_code_fkey FOREIGN KEY (tovar_code)      REFERENCES tovar (code) MATCH SIMPLE      ON UPDATE NO ACTION ON DELETE NO ACTION)WITH (  OIDS=FALSE);ALTER TABLE checks_table  OWNER TO postgres;COMMENT ON COLUMN checks_table.action_num_doc IS 'сюда пишется номер акционного документа, маркер чтобы не был начислен дисконт';COMMENT ON COLUMN checks_table.action_num_doc1 IS 'когда выдается подарок сюда пишется номер акционного документа';COMMENT ON COLUMN checks_table.action_num_doc2 IS 'сюда пишется номер акционного документа, маркер того чтобы данный товар участвовал в одной акции';COMMENT ON COLUMN checks_table.bonus_standard IS 'бонусы, начисленные по стандартным значениям для производителей/марок/товаров';COMMENT ON COLUMN checks_table.bonus_promotion IS 'бонусы, начисленные по бонусным акциям';COMMENT ON COLUMN checks_table.promotion_b_mover IS 'номер акции, указывается для товаров-инициаторов сработки';";
                query[8] = "CREATE TABLE discount_types(  code integer NOT NULL,  discount_percent numeric(10,2) NOT NULL,  transition_sum numeric(10,2) NOT NULL,  name character(10) NOT NULL)WITH (  OIDS=FALSE);ALTER TABLE discount_types  OWNER TO postgres;CREATE UNIQUE INDEX _discount_types_  ON discount_types  USING btree  (code);";
                query[9]  = "CREATE TABLE clients(  name character(100) NOT NULL,  sum numeric(10,2),  discount_types_code integer NOT NULL,  code character(13) NOT NULL,  date_of_birth date,  its_work smallint,  phone character varying(13),  attribute character varying(1),  bonus_is_on smallint,  CONSTRAINT clients_discount_types_code_fkey FOREIGN KEY (discount_types_code)      REFERENCES discount_types (code) MATCH SIMPLE      ON UPDATE NO ACTION ON DELETE NO ACTION)WITH (  OIDS=FALSE);ALTER TABLE clients  OWNER TO postgres;COMMENT ON COLUMN clients.bonus_is_on IS 'Бонусная система включена - 1, выключена - иначе.';CREATE UNIQUE INDEX _clients_code_  ON clients  USING btree  (code COLLATE pg_catalog.default);";                
                query[10] = "CREATE TABLE document_wil_be_printed(  document_number bigint)WITH (  OIDS=FALSE);ALTER TABLE document_wil_be_printed  OWNER TO postgres;CREATE UNIQUE INDEX document_wil_be_printed_document_number_idx  ON document_wil_be_printed  USING btree  (document_number);";
                query[11] = "CREATE TABLE failed_input_phone(client_code character varying(10),datetime_input timestamp without time zone)WITH (  OIDS=FALSE);ALTER TABLE failed_input_phone  OWNER TO postgres;";
                query[12] = "CREATE TABLE logs(time_event timestamp without time zone,description character(200), metadata character(50),  document_number bigint)WITH (  OIDS=FALSE);ALTER TABLE logs  OWNER TO postgres;";
                query[13] = "CREATE TABLE rights_of_users(  code smallint NOT NULL,  name character(30) NOT NULL,  CONSTRAINT _code_rights_of_users_ PRIMARY KEY (code))WITH (  OIDS=FALSE);ALTER TABLE rights_of_users  OWNER TO postgres;";
                query[14] = "CREATE TABLE sertificates(  code integer,  code_tovar integer,  rating integer,  is_active smallint,  CONSTRAINT sertificates_code_tovar_fkey FOREIGN KEY (code_tovar)      REFERENCES tovar (code) MATCH SIMPLE      ON UPDATE NO ACTION ON DELETE NO ACTION)WITH (  OIDS=FALSE);ALTER TABLE sertificates  OWNER TO postgres;";
                query[15] = "CREATE TABLE temp_code_clients(  old_code_client character(10),  new_code_client character(10),  date_time_change timestamp without time zone)WITH (  OIDS=FALSE);ALTER TABLE temp_code_clients  OWNER TO postgres;";
                query[16] = "CREATE TABLE temp_phone_clients(  barcode character varying(10),  phone character varying(13))WITH (  OIDS=FALSE);ALTER TABLE temp_phone_clients  OWNER TO postgres;";                
                query[17] = "CREATE TABLE tovar_action( code integer NOT NULL,  retail_price numeric(10,2) NOT NULL,  quantity integer,  characteristic_name character varying(100),  characteristic_guid character varying(36))WITH (  OIDS=FALSE);ALTER TABLE tovar_action  OWNER TO postgres;";
                query[18] = "CREATE TABLE users(  code integer NOT NULL,  name character varying(50) NOT NULL,  rights smallint NOT NULL,  shop character(3),  password_m character(47) NOT NULL,  password_b character(47),  inn character varying(12))WITH (  OIDS=FALSE);ALTER TABLE users  OWNER TO postgres;CREATE UNIQUE INDEX users_code  ON users  USING btree  (code);CREATE UNIQUE INDEX users_code_idx ON users  USING btree  (code);";
                //query[20] = "CREATE TABLE constants(  cash_desk_number smallint NOT NULL,  nick_shop character(5) NOT NULL,  width_of_symbols numeric(3,0),  use_usb_to_com_barcode_scaner boolean,  name_com_port character(20),  quantity_of_empty_lines integer DEFAULT 0,  use_fiscal_print boolean,  size_font_listview integer NOT NULL DEFAULT 12,  num_text_printer character varying,  change_path_for_main_computer character varying(255),  code_shop character varying(36),  use_debug boolean,  rate numeric(9,6),  two_currencies boolean,  path_for_web_service character varying(200),  currency character(4),  fiscal_num_port smallint,  fiscal_type_port character varying(3),  unloading_period smallint,  firma character varying(200),  inn character varying(12),  use_trassir smallint,  ip_addr_trassir character varying(15),  ip_port_trassir integer,  show_before_payment_window smallint,  execute_addcolumn smallint,  start_sum_opt_price integer,  envd boolean,  last_date_download_bonus_clients timestamp without time zone)WITH (  OIDS=FALSE);ALTER TABLE constants  OWNER TO postgres;COMMENT ON COLUMN constants.num_text_printer IS 'Номер принтера для текстовой печати';COMMENT ON COLUMN constants.change_path_for_main_computer IS 'Путь к папке обмена с главным компьютером';COMMENT ON COLUMN constants.code_shop IS 'Уникальный номер БД';COMMENT ON COLUMN constants.use_trassir IS '0. Не импользуется трассир.1. Используется трассирю';";
                query[19] = "CREATE TABLE constants(cash_desk_number smallint NOT NULL,  nick_shop character(5) NOT NULL,  code_shop character varying(36),   use_debug boolean,  path_for_web_service character varying(200),  currency character(4),  unloading_period smallint,  execute_addcolumn smallint,  envd boolean,  last_date_download_bonus_clients timestamp without time zone)WITH (  OIDS=FALSE);ALTER TABLE constants  OWNER TO postgres;COMMENT ON COLUMN constants.code_shop IS 'Уникальный номер БД';";
                query[20] = "CREATE SEQUENCE checks_header_document_number_seq  INCREMENT 1  MINVALUE 0  MAXVALUE 9223372036854775807  START 1  CACHE 1;ALTER TABLE checks_header_document_number_seq  OWNER TO postgres;";                
                query[21] = "INSERT INTO constants(use_debug, currency,unloading_period, execute_addcolumn,envd,last_date_download_bonus_clients,cash_desk_number,nick_shop,path_for_web_service)VALUES('false','руб.',0,1,false,'01.01.2000',9,'A00','http://ch.sd2.com.ua/DiscountSystem/Ds.asmx');";
                query[22] = "INSERT INTO discount_types(code, discount_percent, transition_sum, name) VALUES (1, 5, 5, '1000000');";
                //query[24] = "CREATE TABLE failed_input_phone(client_code character varying(10),  datetime_input timestamp without time zone)WITH (OIDS=FALSE);ALTER TABLE failed_input_phone  OWNER TO postgres";
                query[23] = "CREATE TABLE date_sync(tovar date,client date)WITH(OIDS = FALSE); ALTER TABLE date_sync OWNER TO postgres";
                query[24] = "CREATE TABLE tovar(code integer NOT NULL,  name character(100) NOT NULL,  retail_price numeric(10,2) NOT NULL, its_deleted numeric(1,0) NOT NULL,  nds integer,  its_certificate smallint,   percent_bonus numeric(8,2) DEFAULT 0,  tnved character varying(10))WITH (  OIDS=FALSE);ALTER TABLE tovar  OWNER TO postgres;CREATE UNIQUE INDEX _tovar_code_  ON tovar  USING btree  (code);";

                conn.Open();               
                trans = conn.BeginTransaction();

                foreach(string str in query)
                {
                    command = new NpgsqlCommand(str, conn);
                    command.Transaction = trans;
                    command.ExecuteNonQuery();
                }
                trans.Commit();
                conn.Close();
                MessageBox.Show("Таблицы для базы данных созданы успешно");
            }
            catch (Exception ex)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }
                MessageBox.Show("Неудачная попытка создать таблицы БД " + ex.ToString());
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CreateDataTables();
            add_field_Click(null, null);
        }
        
        /// <summary>
        /// Проверка и вставка
        /// 13 кода прав
        /// </summary>
        private void check_and_insert_data()
        {
            NpgsqlConnection conn = null;
            string query = "SELECT COUNT(*) FROM rights_of_users WHERE code=13;";
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (Convert.ToInt16(command.ExecuteScalar()) == 0)
                {
                    query = "INSERT INTO rights_of_users(code, name)VALUES (13, 'Заблокирован')";
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                //MessageBox.Show(ex.Message,query);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, query);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        private void append_column(string query)
        {

            NpgsqlConnection conn = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                //MessageBox.Show(ex.Message,query);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, query);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        private string get_max_document_number()
        {
            string result = "0";

            NpgsqlConnection conn = null;
            string query = " SELECT MAX(document_number)+1 FROM checks_header";

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                object result_query = command.ExecuteScalar();
                if (result_query.GetType().FullName != "System.DBNull")
                {
                    result = (Convert.ToInt64(result_query) + 1).ToString();
                }
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message, " Макс. номер документа ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " Макс. номер документа ");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return result;
        }



        /// <summary>
        /// Эта функция проверяет наличие колонки в таблице, если ее нет выполняет запрос
        /// который создает эту колонку и выполняет первончальное заполнение
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="add_query"></param>
        /// <returns></returns>
        //private int exists_coulumn(string table, string field)
        //{
        //    int result = 0;

        //    NpgsqlConnection conn = null;
        //    NpgsqlTransaction tran = null;

        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        string query = "SELECT COUNT(*) FROM information_schema.columns where table_name = '" + table + "' AND column_name = '" + field + "'";
        //        NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //        result = Convert.ToInt16(command.ExecuteScalar());

        //        if (result == 0)
        //        {
        //            tran = conn.BeginTransaction();
        //            command = new NpgsqlCommand(add_query, conn);
        //            command.Transaction = tran;
        //            command.ExecuteNonQuery();
        //            tran.Commit();
        //        }

        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        if (tran != null)
        //        {
        //            tran.Rollback();
        //        }
        //        MyMessageBox mmb = new MyMessageBox(ex.Message + " | " + ex.Detail, " Проверка существования колонки ");
        //        mmb.ShowDialog();
        //        result = 2;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (tran != null)
        //        {
        //            tran.Rollback();
        //        }
        //        MyMessageBox mmb = new MyMessageBox(ex.Message, " Проверка существования колонки ");
        //        mmb.ShowDialog();
        //        result = 2;
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }

        //    return result;

        //}




        //
        private void add_column_currency()
        {

        }



        private int get_count_columns_in_dssl_log()
        {
            int result = 0;
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_NAME='dssl_log';";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                object result_query = command.ExecuteScalar();
                if (result_query != null)
                {
                    result = Convert.ToInt32(result_query);
                }
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException)
            {

            }
            catch (Exception)
            {

            }
            finally
            {
                if(conn.State== ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return result;
        }


        private void change_type_column_constants()
        {
            DateTime old_value = new DateTime(2000, 1, 1);
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            NpgsqlTransaction tran = null;
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                string query = " SELECT last_date_download_bonus_clients FROM constants ";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.Transaction = tran;
                object result_query = command.ExecuteScalar();
                if (result_query != null)
                {
                    old_value = Convert.ToDateTime(result_query);
                }
                query = " ALTER TABLE constants DROP COLUMN last_date_download_bonus_clients; ";
                command = new NpgsqlCommand(query, conn);
                command.Transaction = tran;
                command.ExecuteNonQuery();

                query = " ALTER TABLE constants ADD COLUMN last_date_download_bonus_clients timestamp without time zone; ";
                command = new NpgsqlCommand(query, conn);
                command.Transaction = tran;
                command.ExecuteNonQuery();

                query = " UPDATE constants SET last_date_download_bonus_clients ='"+old_value.ToString("yyyy-MM-dd HH:mm:ss")+"'";
                command = new NpgsqlCommand(query, conn);
                command.Transaction = tran;
                command.ExecuteNonQuery();
                tran.Commit();
                conn.Close();
                command.Dispose();
            }
            catch
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        private void check_and_chage_data_type_persent_bonus()
        {
             NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
             NpgsqlTransaction tran = null;
             try
             {
                 conn.Open();
                 tran = conn.BeginTransaction();
                 string query = "SELECT data_type FROM information_schema.columns where table_name = 'tovar' AND column_name = 'percent_bonus'";
                 NpgsqlCommand command = new NpgsqlCommand(query, conn);
                 if (command.ExecuteScalar().ToString().Trim() == "smallint")//старый тип колонки в бд, меняем на новый
                 {
                     query = "ALTER TABLE tovar DROP COLUMN percent_bonus";
                     command = new NpgsqlCommand(query, conn);
                     command.Transaction = tran;
                     command.ExecuteNonQuery();
                     query = "ALTER TABLE tovar ADD COLUMN percent_bonus numeric(8,2);ALTER TABLE tovar ALTER COLUMN percent_bonus SET DEFAULT 0;UPDATE tovar SET percent_bonus=0";
                     command = new NpgsqlCommand(query, conn);
                     command.Transaction = tran;
                     command.ExecuteNonQuery(); 
                 }
                 tran.Commit();
                 conn.Close();
                 command.Dispose();
             }
             catch
             {
                 if (tran != null)
                 {
                     tran.Rollback();
                 }
             }
             finally
             {
                 if (conn.State == ConnectionState.Open)
                 {
                     conn.Close();
                 }
             }
        }

        
        private void check_and_chage_data_type_bonuses_it_is_written_off()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            NpgsqlTransaction tran = null;
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                string query = "SELECT data_type FROM information_schema.columns where table_name = 'checks_header' AND column_name = 'bonuses_it_is_written_off'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (command.ExecuteScalar().ToString().Trim() == "integer")//старый тип колонки в бд, меняем на новый
                {
                    query = "ALTER TABLE checks_header DROP COLUMN bonuses_it_is_written_off";
                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                    query = "ALTER TABLE checks_header ADD COLUMN bonuses_it_is_written_off numeric(8,2);ALTER TABLE checks_header ALTER COLUMN bonuses_it_is_written_off SET DEFAULT 0;UPDATE checks_header SET bonuses_it_is_written_off=0";
                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                }
                tran.Commit();
                conn.Close();
                command.Dispose();
            }
            catch
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void check_and_chage_data_type_bonuses_it_is_counted()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            NpgsqlTransaction tran = null;
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                string query = "SELECT data_type FROM information_schema.columns where table_name = 'checks_header' AND column_name = 'bonuses_it_is_counted'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (command.ExecuteScalar().ToString().Trim() == "integer")//старый тип колонки в бд, меняем на новый
                {
                    query = "ALTER TABLE checks_header DROP COLUMN bonuses_it_is_counted";
                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                    query = "ALTER TABLE checks_header ADD COLUMN bonuses_it_is_counted numeric(8,2);ALTER TABLE checks_header ALTER COLUMN bonuses_it_is_counted SET DEFAULT 0;UPDATE checks_header SET bonuses_it_is_counted=0";
                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                }
                tran.Commit();
                conn.Close();
                command.Dispose();
            }
            catch
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        /// <summary>
        /// Исправление старого типа автор
        /// в колонке
        /// </summary>
        private void check_and_correct()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            NpgsqlTransaction tran = null;
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                string query = "SELECT data_type FROM information_schema.columns where table_name = 'checks_header' AND column_name = 'autor'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (command.ExecuteScalar().ToString().Trim() != "integer")//старый тип колонки в бд, меняем на новый
                {
                    query = "ALTER TABLE checks_header ADD COLUMN autor_temp integer;UPDATE checks_header SET autor_temp = CAST(autor AS Integer);ALTER TABLE checks_header DROP COLUMN autor;ALTER TABLE checks_header RENAME COLUMN autor_temp TO autor;";
                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                }
                tran.Commit();
                conn.Close();
                command.Dispose();
            }
            catch
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void check_and_correct_date_sync()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            NpgsqlTransaction tran = null;
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                string query = "SELECT COUNT(*) FROM date_sync";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (Convert.ToInt16(command.ExecuteScalar())==0)//в таблицен нет записей, далее там только обновление, поэтому сделаем 1 запись
                {
                    query = "INSERT INTO date_sync(tovar, client) VALUES ('01.01.2000', '01.01.2000');";
                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                }
                tran.Commit();
                conn.Close();
                command.Dispose();
            }
            catch
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        /// <summary>
        /// Если не заполнена новая система 
        /// налогообложения то попытаться заполнить 
        /// его значением из старой схемы
        /// если считывается истина то оогда магазин работает по усн
        /// </summary>
        //private void check_system_taxation()
        //{
        //    if (MainStaticClass.SystemTaxation == 0)//Система налообложения не определена
        //    {
        //        NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //        try
        //        {
        //            conn.Open();
        //            string query = "SELECT usn_income_out_come FROM constants";
        //            NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //            bool result = Convert.ToBoolean(command.ExecuteScalar());
        //            if (!result)
        //            {
        //                query = "UPDATE constants SET system_taxation = 1";
        //            }
        //            else
        //            {
        //                query = "UPDATE constants SET system_taxation = 2";
        //            }

        //            command = new NpgsqlCommand(query, conn);
        //            command.ExecuteNonQuery();
        //            command.Dispose();
        //        }
        //        catch (NpgsqlException ex)
        //        {
        //            MessageBox.Show("Ошибка при считывании системы налогообложения по старой схеме, проверьте константы, в частности ситстему налогообложения  "+ex.Message );
        //        }
        //        finally
        //        {
        //            if (conn.State == ConnectionState.Open)
        //            {
        //                conn.Close();
        //            }
        //        }
        //    }                        
        //}




        public void add_field_Click(object sender, EventArgs e)
        {
            List<string> queries = new List<string>();
            queries.Add("CREATE TABLE failed_input_phone(client_code character varying(10),  datetime_input timestamp without time zone)WITH (OIDS=FALSE);ALTER TABLE failed_input_phone  OWNER TO postgres");
            queries.Add("ALTER TABLE tovar DROP COLUMN purchase_price");
            queries.Add("ALTER TABLE tovar DROP COLUMN opt_price");
            queries.Add("DROP TABLE public.date_sync; CREATE TABLE public.date_sync (tovar timestamp without time zone,    client date)WITH(    OIDS = FALSE)");
            queries.Add("ALTER TABLE action_header ADD COLUMN execution_order smallint");
            queries.Add("ALTER TABLE action_table ADD COLUMN price numeric(10, 2)");
            queries.Add("UPDATE constants SET use_debug = false");
            queries.Add("ALTER TABLE constants ADD COLUMN pass_promo character varying(100)");
            //queries.Add("CREATE TABLE bonus_cards(code character varying(10), pin character varying(11))WITH(OIDS = FALSE); ALTER TABLE bonus_cards OWNER TO postgres;");
            queries.Add("ALTER TABLE constants ADD COLUMN threshold integer;COMMENT ON COLUMN constants.threshold IS 'Порог срабатывания по выдаче бонусной карты';UPDATE public.constants	SET threshold=0;");
            queries.Add("ALTER TABLE constants ADD COLUMN last_date_download_bonus_cards timestamp without time zone;COMMENT ON COLUMN constants.last_date_download_bonus_cards IS 'Дата последнего удачного получения данных бонусных карт';UPDATE constants   SET last_date_download_bonus_cards='01-01-2000';");
            queries.Add("ALTER TABLE checks_header ADD COLUMN sent_to_processing_center smallint;  ALTER TABLE checks_header ALTER COLUMN sent_to_processing_center SET DEFAULT 0;");
            queries.Add("ALTER TABLE public.checks_header ADD COLUMN id_transaction character varying(10) COLLATE pg_catalog.default; COMMENT ON COLUMN public.checks_header.id_transaction IS 'Номер транзакции в процессинговом центре бонусной программы';");
            queries.Add("ALTER TABLE public.checks_header ADD COLUMN id_transaction_sale character varying(10) COLLATE pg_catalog.default;COMMENT ON COLUMN public.checks_header.id_transaction_sale IS 'Колонка ид транзакции документа продажи на основании которого вводится возврат'; ");
            queries.Add("ALTER TABLE checks_header ADD COLUMN clientInfo_vatin character varying(12);COMMENT ON COLUMN checks_header.clientInfo_vatin IS 'Инн покупателя при возврате';");
            queries.Add("ALTER TABLE checks_header ADD COLUMN clientInfo_name character varying(200);COMMENT ON COLUMN checks_header.clientInfo_name IS 'Наименования покупателя при возврате';");
            //queries.Add("ALTER TABLE public.checks_header ADD COLUMN cardTrack2 character varying(36) COLLATE pg_catalog.default; COMMENT ON COLUMN public.checks_header.cardTrack2    IS 'Номер бонусной карты'");
            //queries.Add("ALTER TABLE public.checks_header ADD COLUMN phone character varying(12) COLLATE pg_catalog.default; COMMENT ON COLUMN public.checks_header.phone IS 'Номер телефона бонусной карты'");
            queries.Add("ALTER TABLE checks_header ALTER COLUMN client TYPE character varying(36)");
            queries.Add("ALTER TABLE public.tovar    ADD COLUMN its_marked smallint;UPDATE public.tovar SET its_marked=0;");
            queries.Add("ALTER TABLE public.checks_table    ADD COLUMN item_marker character varying(100)");
            queries.Add("ALTER TABLE public.checks_header   ADD COLUMN id_sale bigint");
            queries.Add("ALTER TABLE public.constants    ADD COLUMN print_m boolean;UPDATE public.constants	SET print_m=true;");
            queries.Add("CREATE TABLE public.client_with_changed_status_to_send(client character varying(10) COLLATE pg_catalog.default NOT NULL,date_change timestamp without time zone NOT NULL,new_phone_number character varying(10) COLLATE pg_catalog.default,   CONSTRAINT client_with_changed_status_to_send_pkey PRIMARY KEY (client) )WITH(    OIDS = FALSE)TABLESPACE pg_default;        ALTER TABLE public.client_with_changed_status_to_send OWNER to postgres");
            queries.Add("ALTER TABLE action_header ADD COLUMN gift_price numeric(10, 2); ALTER TABLE action_header ALTER COLUMN gift_price SET NOT NULL;");
            queries.Add("ALTER TABLE action_header ADD COLUMN gift_price numeric(10, 2);UPDATE public.action_header SET gift_price=0; ALTER TABLE action_header ALTER COLUMN gift_price SET NOT NULL;");
            //queries.Add("ALTER TABLE constants ADD COLUMN usn_income_out_come boolean; UPDATE public.constants SET usn_income_out_come=false;");
            queries.Add("ALTER TABLE checks_table ALTER COLUMN item_marker TYPE character varying(200)");
            queries.Add("CREATE TABLE deleted_items(num_doc bigint NOT NULL,num_cash smallint NOT NULL,date_time_start timestamp without time zone NOT NULL,date_time_action timestamp without time zone NOT NULL,tovar integer NOT NULL,quantity integer NOT NULL,type_of_operation smallint NOT NULL)WITH(    OIDS = FALSE)TABLESPACE pg_default;        ALTER TABLE public.deleted_items            OWNER to postgres");
            queries.Add("ALTER TABLE public.constants ADD COLUMN system_taxation smallint NOT NULL DEFAULT 0;");
            queries.Add("ALTER TABLE public.constants ADD COLUMN work_schema smallint NOT NULL DEFAULT 1; COMMENT ON COLUMN public.constants.work_schema IS 'Варианты работы программы 1-ЧД 2-ЕВА';");
            queries.Add("ALTER TABLE public.constants ADD COLUMN login_promo character varying(100) COLLATE pg_catalog.default;");
            queries.Add("ALTER TABLE clients ALTER COLUMN code TYPE varchar(13)");
            queries.Add("ALTER TABLE tovar ALTER COLUMN code TYPE bigint");
            queries.Add("ALTER TABLE sertificates ALTER COLUMN code TYPE bigint");
            queries.Add("ALTER TABLE sertificates ALTER COLUMN code_tovar TYPE bigint");
            queries.Add("ALTER TABLE checks_table ALTER COLUMN tovar_code TYPE bigint");
            queries.Add("ALTER TABLE barcode ALTER COLUMN tovar_code TYPE bigint");
            queries.Add("ALTER TABLE action_table ALTER COLUMN code_tovar TYPE bigint");
            queries.Add("ALTER TABLE action_header ALTER COLUMN code_tovar TYPE bigint");
            queries.Add("ALTER TABLE public.constants ADD COLUMN version_fn smallint; UPDATE public.constants SET version_fn=2");
            queries.Add("ALTER TABLE public.checks_header ADD COLUMN requisite smallint;");
            queries.Add("ALTER TABLE deleted_items ALTER COLUMN tovar TYPE bigint");
            queries.Add("ALTER TABLE public.checks_header ADD COLUMN viza_d smallint;");
            queries.Add("CREATE UNIQUE INDEX _clients_code_  ON clients  USING btree(code COLLATE pg_catalog.default);");
            queries.Add("CREATE TABLE public.roll_up_temp(code_tovar bigint,name_tovar character varying(200) COLLATE pg_catalog.default,    characteristic_guid character varying(36) COLLATE pg_catalog.default," +
                "characteristic_name character varying(200) COLLATE pg_catalog.default,quantity integer,price numeric(10,2),price_at_a_discount numeric(10,2),sum numeric(10,2),sum_at_a_discount numeric(10,2)," +
                "action_num_doc integer,action_num_doc1 integer,action_num_doc2 integer,item_marker character varying(200) COLLATE pg_catalog.default)WITH(OIDS = FALSE)TABLESPACE pg_default;" +
                "ALTER TABLE public.roll_up_temp OWNER to postgres;");
            queries.Add("ALTER TABLE public.constants ADD COLUMN Enable_stock_processing_in_memory boolean;UPDATE public.constants	SET Enable_stock_processing_in_memory=false;");
            queries.Add("ALTER TABLE public.constants ADD COLUMN id_acquirer_terminal character varying(8) COLLATE pg_catalog.default;UPDATE public.constants	SET id_acquirer_terminal=''");
            queries.Add("ALTER TABLE public.constants ADD COLUMN ip_address_acquiring_terminal character varying(21) COLLATE pg_catalog.default;UPDATE public.constants	SET ip_address_acquiring_terminal=''");
            queries.Add("ALTER TABLE public.constants ADD COLUMN self_service_kiosk boolean;COMMENT ON COLUMN public.constants.self_service_kiosk IS 'Это киоск самообслуживания'; UPDATE public.constants	SET self_service_kiosk=false");
            queries.Add("ALTER TABLE public.constants ADD COLUMN one_monitors_connected boolean;UPDATE public.constants	SET one_monitors_connected=false");
            queries.Add("ALTER TABLE public.checks_header ADD COLUMN id_transaction_terminal character varying(18) COLLATE pg_catalog.default;");
            queries.Add("ALTER TABLE public.checks_header ADD COLUMN system_taxation smallint DEFAULT 0; COMMENT ON COLUMN public.checks_header.system_taxation IS 'Система налогообложения';");
            queries.Add("ALTER TABLE public.checks_header ADD COLUMN code_authorization_terminal character varying(8) COLLATE pg_catalog.default;COMMENT ON COLUMN public.checks_header.code_authorization_terminal IS 'Это служебное поле в ответе от терминала при оплате, его необходимо указывать при возврате ';");
            queries.Add("ALTER TABLE public.checks_header ADD COLUMN its_print_p boolean;COMMENT ON COLUMN public.checks_header.its_print_p IS 'Признако того что чек был нормально рапечатан на фискальном принтере по налогообложению патент.';");
            queries.Add("ALTER TABLE public.document_wil_be_printed ADD COLUMN tax_type smallint; COMMENT ON COLUMN public.document_wil_be_printed.tax_type IS 'Реквизит добавлен для 3 типа налогообложения';");
            queries.Add("DROP INDEX public.document_wil_be_printed_document_number_idx;");
            queries.Add("CREATE INDEX document_wil_be_printed_document_number_idx2    ON public.document_wil_be_printed USING btree    (document_number ASC NULLS LAST, tax_type ASC NULLS LAST)    TABLESPACE pg_default;");
            queries.Add("ALTER TABLE public.action_header ADD COLUMN kind smallint NOT NULL DEFAULT 0;");
            queries.Add("CREATE UNIQUE INDEX code_client_num_doc ON public.action_clients USING btree (code_client, num_doc); ALTER TABLE public.action_clients CLUSTER ON code_client_num_doc;");
            queries.Add("ALTER TABLE public.tovar ADD COLUMN its_excise smallint NOT NULL DEFAULT 0;COMMENT ON COLUMN public.tovar.its_excise IS '0 - обычный товар 1 - подакцизный товар';");
            queries.Add("UPDATE public.constants SET enable_stock_processing_in_memory=true");
            queries.Add("ALTER TABLE public.constants ADD COLUMN version2_marking boolean  DEFAULT true");
            queries.Add("ALTER TABLE public.checks_header ADD COLUMN cash_money1 numeric(10,2) DEFAULT 0");
            queries.Add("ALTER TABLE public.checks_header ADD COLUMN non_cash_money1 numeric(10,2) DEFAULT 0");
            queries.Add("ALTER TABLE public.checks_header    ADD COLUMN sertificate_money1 numeric(10,2) DEFAULT 0");
            queries.Add("ALTER TABLE public.checks_header    ADD COLUMN guid character varying(36) COLLATE pg_catalog.default NOT NULL DEFAULT ''::character varying");
            queries.Add("ALTER TABLE public.checks_table    ADD COLUMN guid character varying(36) COLLATE pg_catalog.default NOT NULL DEFAULT ''::character varying");
            queries.Add("ALTER TABLE public.clients ADD COLUMN notify_security smallint DEFAULT 0;");
            queries.Add("ALTER TABLE public.clients ADD COLUMN reason_for_blocking character varying(500) COLLATE pg_catalog.default;");
            queries.Add("ALTER TABLE public.constants ADD COLUMN webservice_authorize boolean  DEFAULT false;");


            foreach (string str in queries)
            {
                append_column(str);
            }

            check_and_correct();
            check_and_correct_date_sync();
            //if (MainStaticClass.CashDeskNumber != 9)
            //{
            //check_system_taxation();
            //}

            MessageBox.Show(" Дополнительные колонки добавлены ");
        }

    }
}