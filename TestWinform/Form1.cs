using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WinForm_Test;

namespace TestWinform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string res;
            frmWaitingBox f = new frmWaitingBox((obj, args) =>
            {
                //f.WaitMessage = "";
                Thread.Sleep(5*1000);
                Add();
            }, 3, "处理中，请稍后...", false, true);
            //给等待框也换肤，保持界面统一
            //SkinSE.SkinSE_Net skinWaitBox = new SkinSE.SkinSE_Net();
            //skinWaitBox.Init_NET(f, 1);
            f.ShowDialog(this);
            //res = f.Message;
            //if (!string.IsNullOrEmpty(res))
            //    MessageBox.Show(res);
        }

        private void Add()
        {
            int i = 3;
            int j = 5;
            MessageBox.Show((i + j).ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (BackgroundWorker bw = new BackgroundWorker())
            {
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerAsync("Tank");
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            // 这里是后台线程， 是在另一个线程上完成的
            // 这里是真正做事的工作线程
            // 可以在这里做一些费时的，复杂的操作
            Thread.Sleep(5000);
            e.Result = e.Argument + "工作线程完成";
            //disableCachingBindingFailures
                
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //这时后台线程已经完成，并返回了主线程，所以可以直接使用UI控件了 
            //this.label4.Text = e.Result.ToString();
            string str = e.Result.ToString();
        }
    }
}
