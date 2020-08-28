using System;
using System.Windows;

namespace SkyEdge.Source
{
    class MyWindow : Window
    {
        public MyWindow()
        {
            // Constructor
            Width = 300;
            Height = 100;
            Title = "My Simple Window";
            Content = "Hi There!";
        }
    }

    public class SkyEdgeApp : Application
    {
        [STAThread]
        static void Main(string[] args)
        {
            Window myWin = new MyWindow(); // Create the Window object.
            // ToolWindow does not have any max or min buttons.
            myWin.WindowStyle = WindowStyle.ToolWindow;
            myWin.Content += "\nHow are you?"; // add more content
            myWin.Show();
            SkyEdgeApp myApp = new SkyEdgeApp(); // Create an Application object.
            myApp.Run(); // Start application running.
        }
    }
}