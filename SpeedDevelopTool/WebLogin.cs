using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpeedDevelopTool
{
    public partial class WebLogin : Form
    {
        private string NavigateUrl { get; set; }

        public WebLogin()
        {
            InitializeComponent();
        }

        public WebLogin(string navigateUrl)
        {
            InitializeComponent();

            this.NavigateUrl = navigateUrl;
        }

        private void WebLogin_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(NavigateUrl);
            //HtmlElement registButton=
        }

        private void WebDocumentClick(object sender,HtmlElementEventArgs e)
        {
            //HtmlElement loginButton= webBrowser1.Document.All["submit"];
            
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //HtmlElement loginButton;
            //HtmlElementCollection inputCollection = this.webBrowser1.Document.GetElementsByTagName("input");
            //for (int i = 0; i < inputCollection.Count; i++)
            //{
            //    if (inputCollection[i].GetAttribute("type").ToLower().Equals("submit"))
            //    {
            //        loginButton = inputCollection[i];
            //        if (loginButton != null)
            //        {
            //            loginButton.Click += new HtmlElementEventHandler(WebDocumentClick);
            //        }
            //        break;
            //    }
            //}

            if (webBrowser1.Url.ToString() == "http://u8dev.yonyou.com/home/ask/index.aspx?r=my")
            {
                //刷新主窗体中的两个webbrowser控件
                Middle.DoSendMessage("");

                this.Close();
            }
        }
    }
}
