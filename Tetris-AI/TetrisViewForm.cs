using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris_AI
{
    public partial class TetrisViewForm : Form
    {
        TetrisProcessor tp = new TetrisProcessor();
        TetrisPiece[] pieces = new TetrisPiece[] { new JPiece(), new LPiece(), new SPiece(), new ZPiece(), new LongPiece(), new SquarePiece() };
        public TetrisViewForm()
        {
            InitializeComponent();
            updateRTB1();
            updateRTB2();
        }

        private void updateRTB1()
        {
            richTextBox1.Text = "   ";


            for (int i = 0; i < TetrisProcessor.GRID_WIDTH; i++)
            {
                richTextBox1.AppendText(i.ToString() + " ");
            }

            richTextBox1.AppendText("\n");

            for (int i = 0; i < TetrisProcessor.GRID_HEIGHT; i++)
            {
                richTextBox1.AppendText(i.ToString() + (i < 10 ? "  " : " "));
                for (int j = 0; j < TetrisProcessor.GRID_WIDTH; j++)
                {
                    richTextBox1.AppendText(tp.Grid.GetBlock(i, j) ? "1 " : "0 ");
                }
                richTextBox1.AppendText(i.ToString() + "\n");
            }

            richTextBox1.AppendText("   ");

            for (int i = 0; i < TetrisProcessor.GRID_WIDTH; i++)
            {
                richTextBox1.AppendText(i.ToString() + " ");
            }
        }

        private void updateRTB2()
        {
            TetrisPiece p = pieces[(int)numericUpDown1.Value].getRotations()[(int)numericUpDown2.Value / 90];

            richTextBox2.Text = "";
            for (int i = 0; i < p.Size; i++)
            {
                for (int j = 0; j < p.Size; j++)
                {
                    richTextBox2.AppendText(p.GetBlock(i, j) ? "1 " : "0 ");
                }
                richTextBox2.AppendText("\n");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            updateRTB1();
            string temp = richTextBox1.Text;

            tp.AddPiece(pieces[(int)numericUpDown1.Value].getRotations()[(int)numericUpDown2.Value / 90], (int)numericUpDown3.Value);

            updateRTB1();
            richTextBox1.Text = strdiff(temp, richTextBox1.Text);
        }

        private string strdiff(string str1, string str2)
        {
            string output = "";

            for (int i = 0; i < str1.Length; i++)
            {
                if (str1[i] == str2[i] || str1[i] == 'X' || str2[i] == 'X')
                {
                    output += str1[i];
                }
                else
                    output += "X";
            }

            return output;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            updateRTB2();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            updateRTB2();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
