namespace Nethack_Online_GUI
{
    public class Location
    {
        public int col, row;

        public Location()
        {
            col = -1;
            row = -1;
        }

        public Location(int col, int row)
        {
            this.col = col;
            this.row = row;
        }    
    }
}