using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.WinForms;
using System.Windows.Forms;

namespace WebClient
{
    public partial class Browser : Form
    {
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern IntPtr GetActiveWindow();//获取当前窗体的活动状态
        // 判断当前窗口是否处于活动状态的方法
        private bool ThisIsActive() { return (GetActiveWindow() == this.Handle); }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!ThisIsActive())
            {
                this.Activate();
            }
            this.WindowState = FormWindowState.Normal;
        }

        public ChromiumWebBrowser browser;
        public void InitBrowser()
        {
            var settings = new CefSettings
            {
                Locale = "zh-CN"
            };
            settings.CefCommandLineArgs.Add("-enable-system-flash", "1");//使用系统flash
            //settings.CefCommandLineArgs.Add("ppapi-flash-version", "32.0.0.321");
            //settings.CefCommandLineArgs.Add("ppapi-flash-path", @"plugins\pepflashplayer.dll");
            settings.CefCommandLineArgs.Add("disable-gpu", "1");//去掉gpu，否则chrome显示有问题
            settings.CefCommandLineArgs.Add("proxy-auto-detect", "0"); //去掉代理，增加加载网页速度
            settings.CefCommandLineArgs.Add("no-proxy-serve", "1"); //去掉代理，增加加载网页速度


            Cef.Initialize(settings);
            browser = new ChromiumWebBrowser("http://q.zsyyjs2018.com")
            {
                KeyboardHandler = new KeyBoardHander(),
                LifeSpanHandler = new CefLifeSpanHandler()
            };
            this.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
        }

        public Browser()
        {
            InitializeComponent();
            InitBrowser();
            this.SetVisibleCore(false);
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.SetVisibleCore(true);
            //this.TopMost = true;
        }

    }
    class KeyBoardHander : IKeyboardHandler
    {

        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            if(type == KeyType.KeyUp)
            {
                if (windowsKeyCode == (int)Keys.F5)
                {
                    browser.Reload(true); //无视缓存刷新
                }
                else if (windowsKeyCode == (int)Keys.F12)
                {
                    browser.ShowDevTools(); //开发者模式
                }
            }
            return false;
        }

        public bool OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            return false;
        }
    }

    class CefLifeSpanHandler : ILifeSpanHandler
    {
        public CefLifeSpanHandler()
        {

        }

        public bool DoClose(IWebBrowser browserControl, CefSharp.IBrowser browser)
        {
            if (browser.IsDisposed || browser.IsPopup)
            {
                return false;
            }

            return true;
        }

        public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {

        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {
        }

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;
            chromiumWebBrowser.Load(targetUrl);
            return true; //Return true to cancel the popup creation
        }
    }

}
