using System;
using System.Drawing;
using System.Collections.Generic;

namespace Nethack_Online_GUI
{
    public class WindowController
    {
        const int NUM_TILES_ACROSS = 40; // number of tiles across tileset
        const int NUM_TILES_DOWN = 30;
        int tileSize; // n x n pixel tile

        Font font;       //
        Bitmap tilesRaw; // our tile set
        Bitmap render;   // final image to be put on screen
        NetHackController nc;
        Bitmap[,] tiles;

        public WindowController(NetHackController nc)
        {
            font = new Font(FontFamily.GenericMonospace, 16, GraphicsUnit.Pixel);

            tilesRaw = new Bitmap(@"..\..\nhtiles.bmp"); // Hard coded for now

            tileSize = tilesRaw.Width / NUM_TILES_ACROSS;

            render = new Bitmap(tilesRaw.Width, tilesRaw.Height);

            this.nc = nc;

            tiles = new Bitmap[NUM_TILES_ACROSS, NUM_TILES_DOWN];

            // Create tile array
            for (int col = 0; col < NUM_TILES_ACROSS; ++col)
            {
                for (int row = 0; row < NUM_TILES_DOWN; ++row)
                {
                    tiles[col, row] = CopyBitmap(tilesRaw, new Rectangle(col * tileSize, row * tileSize, tileSize, tileSize));
                }
            }
        }

        // Copies a part of a bitmap
        // http://msdn.microsoft.com/en-us/library/ms172505.aspx
        private Bitmap CopyBitmap(Bitmap source, Rectangle part)
        {
            Bitmap bmp = new Bitmap(part.Width, part.Height);
            Graphics g = Graphics.FromImage(bmp);

            g.DrawImage(source, 0, 0, part, GraphicsUnit.Pixel);
            g.Dispose();

            return bmp;
        }

        public void Paint(Graphics graph)
        {
            graph.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, 1280, 384)); //Dungeon Area

            //drawText("hello world", 0, graph);
            //drawText("Line 2", 1, graph);
            //drawText("Line 23", 23, graph);

            graph.DrawImage(render,0,0);

            // Draw random tiles
            for (int row = 0; row < TelnetHelper.TERMINAL_ROWS; ++row)
                for (int col = 0; col < TelnetHelper.TERMINAL_COLS; ++col)
                    drawTile((new Random()).Next(100), new Location(col, row), graph);
        }


        public void drawTile(Location tile, Location term, Graphics graph)
        {
            // Method 1
            //graph.DrawImage(tileSet, new Rectangle(0, 0, TILE_SIZE, TILE_SIZE), new Rectangle(tile.col * TILE_SIZE, tile.row * TILE_SIZE, TILE_SIZE, TILE_SIZE), GraphicsUnit.Pixel);
            
            // Method 2
            //graph.DrawImage(tileSet,
            //    new Rectangle(term.col * tileSize, term.row * tileSize, tileSize,
            //    tileSize),
            //    new Rectangle(tile.col * tileSize, tile.row * tileSize, tileSize, tileSize),
            //    GraphicsUnit.Pixel);

            // Method 3
            graph.DrawImage(tiles[tile.col, tile.row], term.col * tileSize, term.row * tileSize);
        }

        public void drawTile(int tileNum, Location term, Graphics graph)
        {
            int row = tileNum / NUM_TILES_ACROSS;
            int col = tileNum - (row * NUM_TILES_ACROSS);

            drawTile(new Location(col, row), term, graph);
        }

        public void drawText(string text, Location term, Graphics graph)
        {
            drawText(text, term.col, term.row, graph);
        }

        public void drawText(string text, int row, Graphics graph)
        {
            drawText(text, 0, row, graph);
        }

        public void drawText(string text, int col, int row, Graphics graph)
        {
            graph.DrawString(text, font, Brushes.White, col * tileSize, row * tileSize);
        }

        public void drawCharacter(char c, Location term, Graphics graph)
        {
            graph.DrawString(c.ToString(), font, Brushes.White, term.col * tileSize, term.row * tileSize);
        }

        public void updateRender(List<TerminalCell> updateList)
        {
            TerminalCell[,] termCells = nc.getTermCells();
            Bitmap buffer = new Bitmap(render); //THREAD UNSAFE
            Graphics graph = Graphics.FromImage(buffer);

            foreach (TerminalCell cell in termCells)//updateList
            {
                if (cell.isTile)
                {
                    drawTile(cell.tileNum, cell.term, graph);
                    Console.WriteLine("drawTile= " + cell.tileNum + " (" + cell.col + "," + cell.row + ")");
                }
                else
                {
                    drawCharacter(cell.character, cell.term, graph);
                    Console.WriteLine("drawText= " + cell.character + " (" + cell.col + "," + cell.row + ")");
                }
            }

            // THREAD UNSAFE
            render = buffer;

            graph.Dispose();
        }
    }
}