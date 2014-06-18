using System;
using System.Windows.Forms;
using System.Drawing;

namespace Nethack_Online_GUI
{
    class Controller
    {
        static void Main()
        {
            //Console.ForegroundColor = ConsoleColor.Red;
            
            //NethackController nhControl;
            //Form myLoginWindow;
            Form myMainWindow;

            //nhControl = new NethackController();
            //myLoginWindow = new Login(nhControl);

            //Application.EnableVisualStyles();
            //Application.Run(myLoginWindow);

            //if (nhControl.Connected())
            //{
            //    myMainWindow = new MainWindow(nhControl);

            //    Application.Run(myMainWindow);
            //}

            //else
            //{
            //    MessageBox.Show("Error", "Log-in Error");
            //}

            myMainWindow = new MainWindow();
            Application.Run(myMainWindow);
        }
    }
}