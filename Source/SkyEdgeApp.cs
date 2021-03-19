using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace SkyEdge.Source
{
    public class MyWindow : Window
    {
        readonly WebView2 wv;

        public MyWindow()
        {
            // Constructor
            Width = 800;
            Height = 480;
            Title = "My Simple Window";

            DockPanel panel;
            Content = panel = new DockPanel()
            {
                LastChildFill = true
            };
            panel.Background = new LinearGradientBrush();
            wv = new WebView2();
            wv.BeginInit();
            wv.EndInit();

            var btn = new Button() {Name = "ABC"};
            btn.Click += Init;
            panel.Children.Add(btn);
            panel.Children.Add(wv);
            
        }

        async void InitializeAsync()
        {
            await wv.EnsureCoreWebView2Async(null);
            // wv.CoreWebView2.WebMessageReceived += UpdateAddressBar;
        }

        void UpdateAddressBar(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String uri = args.TryGetWebMessageAsString();
            // addressBar.Text = uri;
            // webView.CoreWebView2.PostWebMessageAsString(uri);
        }

        public async void Init(object o, RoutedEventArgs arts)
        {
            await wv.EnsureCoreWebView2Async();

            wv.Source = new Uri("http://jx.skyiah.com/admly/");
        }
    }

    public class SkyEdgeApp : Application
    {
        [STAThread]
        static void Main(string[] args)
        {
            MyWindow win = new MyWindow
            {
                WindowStyle = WindowStyle.ThreeDBorderWindow
            };

            // ToolWindow does not have any max or min buttons.
            win.Show();

            // win.Init(null, null);

            var myApp = new SkyEdgeApp(); // Create an Application object.
            myApp.Run(); // Start application running.
        }
    }
}