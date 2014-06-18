using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Text;

namespace Nethack_Online_GUI
{
    public partial class MainWindow : Form
    {
        Thread socketThread;
        TelnetController tc;
        WindowController wc;
        NetHackController nc;
        ASCIIEncoding encoder;

        public MainWindow()
        {
            InitializeComponent();

            nc = new NetHackController();

            tc = new TelnetController();

            wc = new WindowController(nc);

            encoder = new ASCIIEncoding();
        }

        override protected void OnPaint(PaintEventArgs e)
        {
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About myAbout = new About();
            myAbout.Visible = true;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tc.Disconnect();

            this.Close();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (tc.Connected)
            {
                MessageBox.Show("Error", "Already Connected");

                return;
            }

            //tc.Connect("username", "password", "nethack.alt.org", 23);

            socketThread = new Thread(PollTelnetServer);

            socketThread.Start();
        }

        // Game Panel
        private void gamePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics graph = e.Graphics;

            wc.Paint(graph);

            graph.Dispose();
        }

        private void PollTelnetServer()
        {
            List<TerminalCell> updateList = new List<TerminalCell>();
            List<TerminalCell> ret;
           
            // injecting data
            ret = tc.ProcessTerminalCommand(nc, encoder.GetBytes("[7d"), encoder.GetBytes("## Games on this server are recorded for in-progress viewing and playback!"));
            //updateList.AddRange(ret);

            ret = tc.ProcessTerminalCommand(nc, encoder.GetBytes("[9d"), encoder.GetBytes("Not logged in."));
            //updateList.AddRange(ret);

            ret = tc.ProcessTerminalCommand(nc, encoder.GetBytes("[11d"), encoder.GetBytes(" l) Login"));
            //updateList.AddRange(ret);

            ret = tc.ProcessTerminalCommand(nc, encoder.GetBytes("[12d"), encoder.GetBytes(" r) Register new user"));
            //updateList.AddRange(ret);

            ret = tc.ProcessTerminalCommand(nc, encoder.GetBytes("[13d"), encoder.GetBytes(" w) Watch games in progress"));
            //updateList.AddRange(ret);

            ret = tc.ProcessTerminalCommand(nc, encoder.GetBytes("[14d"), encoder.GetBytes(" q) Quit"));
            //updateList.AddRange(ret);

            ret = tc.ProcessTerminalCommand(nc, encoder.GetBytes("[18d"), encoder.GetBytes(" => "));
            //updateList.AddRange(ret);

            ret = tc.ProcessTerminalCommand(nc, encoder.GetBytes("[24;23H"), encoder.GetBytes("Pw:21(21) AC:3  Xp:5/202 T:2555"));
            //updateList.AddRange(ret);

            ret = tc.ProcessTerminalCommand(nc, encoder.GetBytes("[0;0H"), encoder.GetBytes("This text goes at 1,1 -- hello world!1234567890"));
            //updateList.AddRange(ret);

            ret = tc.ProcessTerminalCommand(nc, encoder.GetBytes("[0;1H"), encoder.GetBytes("-----------------------------------------------"));
            //updateList.AddRange(ret);

            wc.updateRender(updateList);
            gamePanel.Invalidate();


            // Normally how we do it
            /*
            while (tc.Connected)
            {
                tc.PollServer();                
            }
            */
        }
    }
}