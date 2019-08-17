using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MayınTarlasi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            timer1.Tick += Timer1_Tick;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = time.ToString();
            time++;
        }

        Game Game1;
        Game.Difficulty difficulty;
        Graphics gr;
        int time = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            gr = panel1.CreateGraphics(); 
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // set flag
                Game1.SetFlag(gr, e.Location);
                lblFlag.Text = Game1.GetFlag().ToString();
            }
            else
            {
                // open and check
                Game1.OpenCell(gr, e.Location);
                lblScore.Text = Game1.ComputeScore(false).ToString();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((sender as ComboBox).SelectedIndex)
            {
                case 0:
                    difficulty = Game.Difficulty.Easy;

                    tbHeigth.Enabled = false;
                    tbWidth.Enabled = false;
                    tbNOMines.Enabled = false;
                    tbCellH.Enabled = false;
                    tbCellW.Enabled = false;
                    break;
                case 1:
                    difficulty = Game.Difficulty.Medium;

                    tbHeigth.Enabled = false;
                    tbWidth.Enabled = false;
                    tbNOMines.Enabled = false;
                    tbCellH.Enabled = false;
                    tbCellW.Enabled = false;
                    break;
                case 2:
                    difficulty = Game.Difficulty.Hard;

                    tbHeigth.Enabled = false;
                    tbWidth.Enabled = false;
                    tbNOMines.Enabled = false;
                    tbCellH.Enabled = false;
                    tbCellW.Enabled = false;
                    break;

                case 3:
                    difficulty = Game.Difficulty.Custom;

                    tbHeigth.Enabled = true;
                    tbWidth.Enabled = true;
                    tbNOMines.Enabled = true;
                    tbCellH.Enabled = true;
                    tbCellW.Enabled = true;
                    break;

                default:
                    difficulty = Game.Difficulty.Medium;

                    tbHeigth.Enabled = false;
                    tbWidth.Enabled = false;
                    tbNOMines.Enabled = false;
                    tbCellH.Enabled = false;
                    tbCellW.Enabled = false;
                    break;
            }
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            // clear dump values in labels
            lblFlag.Text = "0";
            lblTime.Text = "0";
            lblScore.Text = "0";
            lblMines.Text = "0";
            time = 0;

            if (difficulty == Game.Difficulty.Custom)
            {
                Size size_b = new Size(Convert.ToInt32(tbWidth.Text), Convert.ToInt32(tbHeigth.Text));
                Size size_c = new Size(Convert.ToInt32(tbCellW.Text), Convert.ToInt32(tbCellH.Text));
                Game1 = new Game(difficulty, size_b);
                Game1.GameEnd += Game1_GameEnd;
                Game1.SetParameters(Convert.ToInt32(tbNOMines.Text), size_c);
            }
            else
            {
                Game1 = new Game(difficulty);
                Game1.GameEnd += Game1_GameEnd;
            }

            Game1.DrawBoard(gr);

            lblMines.Text = Game1.GetParameters().NumOfMines.ToString();


            timer1.Start();
        }

        private void Game1_GameEnd(object sender, Game.GameEndEventArgs e)
        {
            timer1.Stop();
            lblScore.Text = e.Score.ToString();

            MessageBox.Show(e.Message + "\n\n Your Score : " + e.Score +
                "\n Your Time : " + time.ToString());
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            Game1.OpenAllCells(gr);
        }

        int flipflop = 0;
        private void btnPauseRes_Click(object sender, EventArgs e)
        {
            if (flipflop++ % 2 == 0)
                timer1.Stop();
            else
                timer1.Start();
        }
    }
}
