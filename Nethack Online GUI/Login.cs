using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Nethack_Online_GUI
{
    public partial class Login : Form
    {
        public Login(/*NetHackHelper nhControl*/)
        {
            InitializeComponent();

            //nhControl = new NethackController();
        }
       
        private void okButton_Click(object sender, EventArgs e)
        {
            //if (usernameBox.Text == "" || passwordBox.Text == "")
            //{
            //    MessageBox.Show("Username / Password fields not complete");
            //    return;
            //}

            //// Good data, lets connect...
            ////if (false)
            ////{
            ////    nhControl.Connect(usernameBox.Text, passwordBox.Text, "nethack.alt.org", 23);

            ////    this.Close(); // We're done with this form...                
            ////}
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
