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
        public TetrisViewForm()
        {
            InitializeComponent();
            tetrisView1.InitView();
            tetrisView1.Select();
        }

        private void TetrisViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            tetrisView1.Bot.Stop();
        }
    }
}
