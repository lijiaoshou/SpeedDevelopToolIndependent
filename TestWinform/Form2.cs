using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestWinform
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://u8dev.yonyou.com/");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
            //File.WriteAllText("F:\\myWebText.txt",webBrowser1.Document.Body.InnerHtml);
            //webBrowser1.Refresh();
            //webBrowser1.CanGoBack
            //webBrowser1.GoBack();
            //webBrowser1.CanGoForward;
            //webBrowser1.GoForward();

            //webBrowser1.Document.InvokeScript("script名称");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }
    }
}
