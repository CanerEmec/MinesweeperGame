using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MayınTarlasi
{
    class Cell
    {
        public Cell(CellContent content, CellState state = 0)
        {
            this.State = state;
            this.Content = content;
            this.CellBody = new Rectangle();
        }

        public CellState State { get; set; }

        public CellContent Content { get; set; }

        public int Number { get; set; }

        public Rectangle CellBody;

        public enum CellContent
        {
            Mine = 0,
            Free = 1,
            //Number = 2
        }

        public enum CellState
        {
            Closed = 0,
            Opened = 1,
            Flag = 2
        }
    }
}
