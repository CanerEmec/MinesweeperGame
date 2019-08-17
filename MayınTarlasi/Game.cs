using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MayınTarlasi
{
    class Game
    {
        public Game(Difficulty diffLevel, Size boardSize = new Size())
        {
            this.DifficultyLevel = diffLevel;

            if (diffLevel == Difficulty.Custom)
                this.BoardSize = boardSize;
            else
                this.BoardSize = ExtractSizeFromDifficulty(diffLevel);

            CreateBoard(BoardSize.Width, BoardSize.Height);
        }

        public class GameEndEventArgs : EventArgs
        {
            public string Message { get; set; }
            public int Score { get; set; }
        }

        public event EventHandler<GameEndEventArgs> GameEnd = null;

        public Size BoardSize { get; set; }

        public Cell[,] Board { get; set; }

        public Difficulty DifficultyLevel { get; set; }

        private GameParameters Parameters { get; set; }

        private int NumFlags = 0;

        private int NumFlagsOnMines = 0;

        private int NumOpenedCells = 0;

        private GameParameters Easy = new GameParameters
        {
            NumOfMines = 25,
            CellSize = new Size(40, 40)
        };

        private GameParameters Medium = new GameParameters
        {
            NumOfMines = 65,
            CellSize = new Size(28, 28)
        };

        private GameParameters Hard = new GameParameters
        {
            NumOfMines = 100,
            CellSize = new Size(20, 20)
        };

        public struct GameParameters
        {
            public int NumOfMines;
            public Size CellSize;
        }

        public enum Difficulty
        {
            Easy = 0,
            Medium = 1,
            Hard = 2,
            Custom = 3
        }

        public void SetParameters(int numOfMines,  Size cellSize)
        {
            this.Parameters = new GameParameters
            {
                NumOfMines = numOfMines,
                CellSize = cellSize
            };

            CreateBoard(BoardSize.Width, BoardSize.Height);
        }

        public GameParameters GetParameters()
        {
            return this.Parameters;
        }

        /// <summary>
        /// Sets the game parameters according to the difficulty level.
        /// </summary>
        /// <param name="diffLevel"> Selected Difficulty Level</param>
        /// <returns> Returns Number Of Cell.</returns>
        private Size ExtractSizeFromDifficulty(Difficulty diffLevel)
        {
            switch (diffLevel)
            {
                case Difficulty.Easy:
                    this.Parameters = Easy;
                    return new Size(10, 10);

                case Difficulty.Medium:
                    this.Parameters = Medium;
                    return new Size(15, 15);

                case Difficulty.Hard:
                    this.Parameters = Hard;
                    return new Size(20, 20);

                default:
                    this.Parameters = Medium;
                    return new Size(15, 15);
            }
        }

        private void CreateBoard(int width, int height)
        {
            InitialiseBoard(width, height);
            InitialiseMines(width, height);
            InitialiseNumbers(width, height);
        }

        /// <summary>
        /// Fills non-mine cells with appropriate numbers.
        /// Checks the cells around the cell and finds the appropriate number.
        /// </summary>
        /// <param name="width"> How many cells are there in the horizontal?</param>
        /// <param name="height"> How many cells are there in the vertical?</param>
        private void InitialiseNumbers(int width, int height)
        {
            int num = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (this.Board[i,j].Content == Cell.CellContent.Free)
                    {
                        num = 0;
                        if ((i > 0 && j > 0) &&
                            this.Board[i - 1, j - 1].Content == Cell.CellContent.Mine)
                            num++;

                        if ((j > 0) &&
                            this.Board[i, j - 1].Content == Cell.CellContent.Mine)
                            num++;

                        if ((i < height - 1) && (j > 0) &&
                            this.Board[i + 1, j - 1].Content == Cell.CellContent.Mine)
                            num++;

                        if ((i > 0) &&
                            this.Board[i - 1, j].Content == Cell.CellContent.Mine)
                            num++;

                        if ((i > 0) && (j < width - 1) &&
                            this.Board[i - 1, j + 1].Content == Cell.CellContent.Mine)
                            num++;

                        if ((i < height - 1) &&
                            this.Board[i + 1, j].Content == Cell.CellContent.Mine)
                            num++;

                        if ((j < width - 1) &&
                            this.Board[i, j + 1].Content == Cell.CellContent.Mine)
                            num++;

                        if ((i < height - 1) && (j < width - 1) &&
                            this.Board[i + 1, j + 1].Content == Cell.CellContent.Mine)
                            num++;

                        this.Board[i, j].Number = num;
                    }
                }
            }
        }

        private void InitialiseMines(int width, int height)
        {
            int mines = Parameters.NumOfMines;
            int i, j;

            Random rand = new Random();

            // Randomly initialize mines..
            while (mines > 0)
            {
                i = rand.Next(0, height);
                j = rand.Next(0, width);

                if (Board[i, j].Content != Cell.CellContent.Mine)
                {
                    Board[i, j].Content = Cell.CellContent.Mine;
                    mines--;
                }
            }
        }

        /// <summary>
        /// Creates each cell in the game.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void InitialiseBoard(int width, int height)
        {
            Board = new Cell[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    Board[i, j] = new Cell(Cell.CellContent.Free);
                    Board[i, j].State = Cell.CellState.Closed;
                    Board[i, j].CellBody.Size = new Size(Parameters.CellSize.Width - 2, Parameters.CellSize.Height - 2);
                }
        }

        public void DrawBoard(Graphics grp)
        {
            grp.Clear(SystemColors.ActiveCaption);
            Size sizecell = this.GetParameters().CellSize;

            for (int i = 0; i < this.BoardSize.Height; i++)
            {
                for (int j = 0; j < this.BoardSize.Width; j++)
                {
                    var pen = Pens.Black;
                    var brush = Brushes.Silver;
                    this.Board[i, j].CellBody.Location = new Point(3 + j * sizecell.Height,3 + i * sizecell.Width);
                    grp.FillRectangle(brush, this.Board[i, j].CellBody);
                    grp.DrawRectangle(pen, this.Board[i, j].CellBody);
                }
            }
        }

        /// <summary>
        /// Opens all cells when the game is over.
        /// </summary>
        /// <param name="grp"> Graphics object for drawing.</param>
        public void OpenAllCells(Graphics grp)
        {
            Size sizecell = this.GetParameters().CellSize;

            for (int i = 0; i < this.BoardSize.Height; i++)
            {
                for (int j = 0; j < this.BoardSize.Width; j++)
                {
                    if (this.Board[i, j].State == Cell.CellState.Opened)
                        continue;

                    var pen = Pens.Black;
                    var brush = Brushes.Silver;
                    this.Board[i, j].CellBody.Location = new Point(3 + j * sizecell.Height, 3 + i * sizecell.Width);

                    if (this.Board[i, j].Content == Cell.CellContent.Mine)
                    {
                        brush = Brushes.Red;
                        grp.FillRectangle(brush, this.Board[i, j].CellBody);
                        grp.DrawIcon(new Icon(@"..\..\Icons\Be-Os-Be-Box-BeBox-Minesweeper.ico"), this.Board[i, j].CellBody);
                    }
                    else
                    {
                        grp.FillRectangle(brush, this.Board[i, j].CellBody);
                        grp.DrawString(this.Board[i, j].Number.ToString(), new Font(FontFamily.GenericSerif, 12), Brushes.Black, this.Board[i, j].CellBody);
                    }

                    if (this.Board[i,j].State == Cell.CellState.Flag)
                    {
                        grp.DrawIcon(new Icon(@"..\..\Icons\Everaldo-Crystal-Clear-App-kmines-minesweeper.ico"), this.Board[i, j].CellBody);
                    }

                    this.Board[i, j].State = Cell.CellState.Opened;
                    grp.DrawRectangle(pen, this.Board[i, j].CellBody);
                }
            }

            GameEndEventArgs args = new GameEndEventArgs();
            args.Message = "Game Is End! You Dead!";
            args.Score = ComputeScore();

            GameEnd?.Invoke(this, args);
        }

        public void OpenCell(Graphics grp, Point pos)
        {
            int i = pos.Y / (this.Parameters.CellSize.Height);
            int j = pos.X / (this.Parameters.CellSize.Width);

            // To handle click out of the game area.
            if (i > this.BoardSize.Height - 1 || j > this.BoardSize.Width - 1 || i < 0 || j < 0)
                return;

            Brush brush;

            // if cell already opened, return.
            if (this.Board[i, j].State == Cell.CellState.Opened)
                return;

            if (this.Board[i, j].State == Cell.CellState.Closed)
            {
                switch (this.Board[i,j].Content)
                {
                    case Cell.CellContent.Mine:
                        OpenAllCells(grp);
                        return;
                        
                    case Cell.CellContent.Free:
                        brush = Brushes.LightGray;
                        grp.FillRectangle(brush, this.Board[i, j].CellBody);
                        grp.DrawString(this.Board[i, j].Number.ToString(), new Font(FontFamily.GenericSerif, 12), Brushes.Black, this.Board[i, j].CellBody);
                        this.Board[i, j].State = Cell.CellState.Opened;

                        if (this.Board[i, j].Number == 0)
                        {
                            OpenAround(grp, i, j);
                        }

                        break;
                }
                grp.DrawRectangle(Pens.Black, this.Board[i, j].CellBody);

                NumOpenedCells++;
                CheckGameStatus();
            }
        }

        public void OpenCell(Graphics grp, int ii, int jj)
        {
            int i = ii;
            int j = jj;

            Brush brush;

            if (this.Board[i, j].State == Cell.CellState.Opened)
                return;

            if (this.Board[i, j].State == Cell.CellState.Closed)
            {
                switch (this.Board[i, j].Content)
                {
                    case Cell.CellContent.Mine:
                        OpenAllCells(grp);
                        return;

                    case Cell.CellContent.Free:
                        brush = Brushes.LightGray;
                        grp.FillRectangle(brush, this.Board[i, j].CellBody);
                        grp.DrawString(this.Board[i, j].Number.ToString(), new Font(FontFamily.GenericSerif, 12), Brushes.Black, this.Board[i, j].CellBody);
                        this.Board[i, j].State = Cell.CellState.Opened;

                        if (this.Board[i, j].Number == 0)
                        {
                            OpenAround(grp, i, j);
                        }

                        break;
                }
                grp.DrawRectangle(Pens.Black, this.Board[i, j].CellBody);

            }
        }

        /// <summary>
        /// If there are no mines in the vicinity of the opened cell, it opens the cells until there is at least 1 mine in the vicinity.
        /// </summary>
        /// <param name="grp"></param>
        /// <param name="i"> Horizantal index for current cell.</param>
        /// <param name="j"> Vertical index for current cell.</param>
        private void OpenAround(Graphics grp, int i, int j)
        {
            if (i > 0 && j > 0)
                OpenCell(grp, i - 1, j - 1);

            if (i > 0)
                OpenCell(grp, i - 1, j);

            if (i > 0 && j < BoardSize.Width - 1)
                OpenCell(grp, i - 1, j + 1);



            if (j > 0)
                OpenCell(grp, i, j - 1);

            if (i < BoardSize.Height - 1 && j > 0)
                OpenCell(grp, i + 1, j - 1);


            
            if (j < BoardSize.Width - 1)
                OpenCell(grp, i, j + 1);

            if (i < BoardSize.Height - 1)
                OpenCell(grp, i + 1, j);

            if (i < BoardSize.Height - 1 && j < BoardSize.Width - 1)
                OpenCell(grp, i + 1, j + 1);

        }

        private void CheckGameStatus()
        {
            int reqNumOfOpnedCells = this.BoardSize.Height * this.BoardSize.Width - this.Parameters.NumOfMines;

            // Counting how many cells are opened.
            int numOfOpenedCells = 0;
            for (int i = 0; i < this.BoardSize.Height; i++)
            {
                for (int j = 0; j < this.BoardSize.Width; j++)
                {
                    if (this.Board[i, j].State == Cell.CellState.Opened)
                        numOfOpenedCells++;
                }
            }

            if ((reqNumOfOpnedCells + this.Parameters.NumOfMines) == (numOfOpenedCells + NumFlags))
            {
                GameEndEventArgs args = new GameEndEventArgs();
                args.Message = "Game Is End!, You Win!";
                args.Score = ComputeScore();

                GameEnd?.Invoke(this, args);
            }
        }

        public int ComputeScore(bool finalScore = true)
        {
            if(finalScore)
                return NumOpenedCells * 35 + NumFlagsOnMines * 50;
            return NumOpenedCells * 35;
        }

        public void SetFlag(Graphics grp, Point pos)
        {
            int i = pos.Y / (this.Parameters.CellSize.Height);
            int j = pos.X / (this.Parameters.CellSize.Width);

            if (this.Board[i, j].State == Cell.CellState.Opened)
                return;

            if (this.Board[i, j].State == Cell.CellState.Closed)
            {
                grp.DrawIcon(new Icon(@"..\..\Icons\Everaldo-Crystal-Clear-App-kmines-minesweeper.ico"), this.Board[i, j].CellBody);
                this.Board[i, j].State = Cell.CellState.Flag;
                NumFlags++;

                if (this.Board[i, j].Content == Cell.CellContent.Mine)
                    NumFlagsOnMines++;

                return;
            }
            else if (this.Board[i, j].State == Cell.CellState.Flag)
            {
                grp.FillRectangle(Brushes.Silver, this.Board[i, j].CellBody);
                grp.DrawRectangle(Pens.Black, this.Board[i, j].CellBody);
                this.Board[i, j].State = Cell.CellState.Closed;
                NumFlags--;

                if (this.Board[i, j].Content == Cell.CellContent.Mine)
                    NumFlagsOnMines--;

                return;
            }

            CheckGameStatus();
        }

        public int GetFlag()
        {
            return NumFlags;
        }

    }
}
