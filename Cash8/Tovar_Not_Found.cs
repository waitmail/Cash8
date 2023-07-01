using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Cash8
{
    public partial class Tovar_Not_Found : Form
    {
        public delegate void show_tovar_not_found();

        private System.Timers.Timer timer = new System.Timers.Timer(1000);

        public Tovar_Not_Found()
        {
            InitializeComponent();
            //this.Load += new EventHandler(Tovar_Not_Found_Load);
            timer.Start();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                timer.Stop();
                this.Close();
            }
        }

        private void set_show_tovar_not_found()
        {
            if (this.BackColor == Color.Yellow)
            {
                this.BackColor = Color.Red;
            }
            else
            {
                this.BackColor = Color.Yellow;
            } 
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Invoke(new show_tovar_not_found(set_show_tovar_not_found));
            if (MainStaticClass.SelfServiceKiosk == 1)
            {
                System.Threading.Thread.Sleep(2000);
                KeyEventArgs args = new KeyEventArgs(Keys.Escape);
                OnKeyDown(args);
            }
        }
    }
}
