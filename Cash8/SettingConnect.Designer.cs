namespace Cash8
{
    partial class SettingConnect
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.ipaddrServer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nameDataBase = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.servicePassword = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.repeatPassword = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.repeatPasswordPostgres = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.passwordPostgres = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.portPostgres = new System.Windows.Forms.Label();
            this.postgresUser = new System.Windows.Forms.TextBox();
            this.portServer = new System.Windows.Forms.TextBox();
            this.userPostrgres = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.add_field = new System.Windows.Forms.Button();
            this.btn_delete_old_columns = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP адрес сервера :";
            this.label1.Visible = false;
            // 
            // ipaddrServer
            // 
            this.ipaddrServer.Location = new System.Drawing.Point(142, 25);
            this.ipaddrServer.MaxLength = 15;
            this.ipaddrServer.Name = "ipaddrServer";
            this.ipaddrServer.Size = new System.Drawing.Size(215, 20);
            this.ipaddrServer.TabIndex = 1;
            this.ipaddrServer.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "имя базы данных :";
            this.label2.Visible = false;
            // 
            // nameDataBase
            // 
            this.nameDataBase.Location = new System.Drawing.Point(142, 127);
            this.nameDataBase.MaxLength = 15;
            this.nameDataBase.Name = "nameDataBase";
            this.nameDataBase.Size = new System.Drawing.Size(215, 20);
            this.nameDataBase.TabIndex = 3;
            this.nameDataBase.Visible = false;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(148, 8);
            this.textBox2.Name = "textBox2";
            this.textBox2.PasswordChar = '|';
            this.textBox2.Size = new System.Drawing.Size(212, 20);
            this.textBox2.TabIndex = 4;
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox2_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "сервисный пароль :";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(388, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 21);
            this.button1.TabIndex = 6;
            this.button1.Text = "Ввести";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // servicePassword
            // 
            this.servicePassword.Location = new System.Drawing.Point(148, 58);
            this.servicePassword.Name = "servicePassword";
            this.servicePassword.Size = new System.Drawing.Size(212, 20);
            this.servicePassword.TabIndex = 8;
            this.servicePassword.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(10, 393);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(212, 21);
            this.button2.TabIndex = 9;
            this.button2.Text = "Записать новые параметры";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(281, 422);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(176, 21);
            this.button3.TabIndex = 10;
            this.button3.Text = "Закрыть";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(132, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "нов. сервисный пароль :";
            this.label5.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "подтверждение пароля";
            this.label6.Visible = false;
            // 
            // repeatPassword
            // 
            this.repeatPassword.Location = new System.Drawing.Point(148, 83);
            this.repeatPassword.Name = "repeatPassword";
            this.repeatPassword.Size = new System.Drawing.Size(212, 20);
            this.repeatPassword.TabIndex = 13;
            this.repeatPassword.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Location = new System.Drawing.Point(3, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(462, 73);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "смена сервисного пароля";
            this.groupBox1.Visible = false;
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(364, 20);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(90, 39);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Показывать пароли";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox2);
            this.groupBox2.Controls.Add(this.repeatPasswordPostgres);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.passwordPostgres);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.portPostgres);
            this.groupBox2.Controls.Add(this.postgresUser);
            this.groupBox2.Controls.Add(this.portServer);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.userPostrgres);
            this.groupBox2.Controls.Add(this.ipaddrServer);
            this.groupBox2.Controls.Add(this.nameDataBase);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(6, 125);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(459, 230);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Параметры соединения с базой данных";
            this.groupBox2.Visible = false;
            // 
            // checkBox2
            // 
            this.checkBox2.Location = new System.Drawing.Point(363, 156);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(90, 49);
            this.checkBox2.TabIndex = 22;
            this.checkBox2.Text = "Показывать пароль к серверу";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.Visible = false;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // repeatPasswordPostgres
            // 
            this.repeatPasswordPostgres.Location = new System.Drawing.Point(142, 187);
            this.repeatPasswordPostgres.Name = "repeatPasswordPostgres";
            this.repeatPasswordPostgres.PasswordChar = '*';
            this.repeatPasswordPostgres.Size = new System.Drawing.Size(215, 20);
            this.repeatPasswordPostgres.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 193);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(125, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "подтверждение пароля";
            // 
            // passwordPostgres
            // 
            this.passwordPostgres.Location = new System.Drawing.Point(142, 156);
            this.passwordPostgres.Name = "passwordPostgres";
            this.passwordPostgres.PasswordChar = '*';
            this.passwordPostgres.Size = new System.Drawing.Size(215, 20);
            this.passwordPostgres.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 162);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "пароль к серверу";
            // 
            // portPostgres
            // 
            this.portPostgres.AutoSize = true;
            this.portPostgres.Location = new System.Drawing.Point(8, 98);
            this.portPostgres.Name = "portPostgres";
            this.portPostgres.Size = new System.Drawing.Size(78, 13);
            this.portPostgres.TabIndex = 17;
            this.portPostgres.Text = "порт сервера ";
            this.portPostgres.Visible = false;
            // 
            // postgresUser
            // 
            this.postgresUser.Location = new System.Drawing.Point(142, 59);
            this.postgresUser.MaxLength = 15;
            this.postgresUser.Name = "postgresUser";
            this.postgresUser.Size = new System.Drawing.Size(215, 20);
            this.postgresUser.TabIndex = 1;
            this.postgresUser.Visible = false;
            // 
            // portServer
            // 
            this.portServer.Location = new System.Drawing.Point(142, 93);
            this.portServer.Name = "portServer";
            this.portServer.Size = new System.Drawing.Size(215, 20);
            this.portServer.TabIndex = 16;
            this.portServer.Visible = false;
            // 
            // userPostrgres
            // 
            this.userPostrgres.AutoSize = true;
            this.userPostrgres.Location = new System.Drawing.Point(8, 61);
            this.userPostrgres.Name = "userPostrgres";
            this.userPostrgres.Size = new System.Drawing.Size(121, 13);
            this.userPostrgres.TabIndex = 0;
            this.userPostrgres.Text = "пользователь postgres";
            this.userPostrgres.Visible = false;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(11, 366);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(212, 21);
            this.button4.TabIndex = 16;
            this.button4.Text = "Создать таблицы базы данных";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // add_field
            // 
            this.add_field.Location = new System.Drawing.Point(11, 420);
            this.add_field.Name = "add_field";
            this.add_field.Size = new System.Drawing.Size(211, 23);
            this.add_field.TabIndex = 17;
            this.add_field.Text = "Добавить поля";
            this.add_field.UseVisualStyleBackColor = true;
            this.add_field.Click += new System.EventHandler(this.add_field_Click);
            // 
            // btn_delete_old_columns
            // 
            this.btn_delete_old_columns.Location = new System.Drawing.Point(281, 363);
            this.btn_delete_old_columns.Name = "btn_delete_old_columns";
            this.btn_delete_old_columns.Size = new System.Drawing.Size(176, 23);
            this.btn_delete_old_columns.TabIndex = 18;
            this.btn_delete_old_columns.Text = "Удалить старые колонки";
            this.btn_delete_old_columns.UseVisualStyleBackColor = true;
            this.btn_delete_old_columns.Click += new System.EventHandler(this.btn_delete_old_columns_Click);
            // 
            // SettingConnect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 457);
            this.Controls.Add(this.btn_delete_old_columns);
            this.Controls.Add(this.add_field);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.repeatPassword);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.servicePassword);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "SettingConnect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Параметры соединения с базой данных";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.SettingConnect_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }






        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ipaddrServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nameDataBase;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox servicePassword;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox repeatPassword;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox postgresUser;
        private System.Windows.Forms.Label userPostrgres;
        private System.Windows.Forms.TextBox portServer;
        private System.Windows.Forms.Label portPostgres;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox repeatPasswordPostgres;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox passwordPostgres;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button add_field;
        private System.Windows.Forms.Button btn_delete_old_columns;
    }
}