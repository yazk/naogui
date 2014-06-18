using System;
using System.Text;

namespace Nethack_Online_GUI
{
	public class NetHackController
	{
        ASCIIEncoding encoding;
		TerminalCell[,] termCells;

        public NetHackController()
		{
		    encoding = new ASCIIEncoding();

            termCells = new TerminalCell[TelnetHelper.TERMINAL_COLS, TelnetHelper.TERMINAL_ROWS];

            for (int r = 0; r < TelnetHelper.TERMINAL_ROWS; ++r)
                for (int c = 0; c < TelnetHelper.TERMINAL_COLS; ++c)
				  termCells[c,r] = new TerminalCell(c,r);
		}

		public TerminalCell[,] getTermCells()
		{
		   return termCells;
        }

        public void setTermCells(TerminalCell[,] termCells)
        {
            this.termCells = termCells;
        }
	}
}