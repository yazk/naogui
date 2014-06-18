using System;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Nethack_Online_GUI
{
    public class TerminalCell
    {
        public int row, col;
        //public Color color;
        //public FontStyle fontStyle;
        public char character;
        public bool isTile;
        public int tileNum;

        public TerminalCell(int col, int row)
        {
            this.col = col;
            this.row = row;

            //color = Color.White;
            //fontStyle = FontStyle.Regular;
            character = (char)0;
            isTile = true;
            tileNum = 1057; // first blank tile
        }

        public TerminalCell(int col, int row, char character)
        {
            this.col = col;
            this.row = row;

            //color = Color.White;
            //fontStyle = FontStyle.Regular;
            this.character = character;
            this.isTile = false;
            this.tileNum = -1;
        }

        public TerminalCell(int col, int row, int tileNum)
        {
            this.col = col;
            this.row = row;

            //color = Color.White;
            //fontStyle = FontStyle.Regular;

            this.character = (char)0;
            this.isTile = true;
            this.tileNum = tileNum;
        }

        public Location term
        {
            get
            {
                return new Location(col, row);
            }
        }
    }
}