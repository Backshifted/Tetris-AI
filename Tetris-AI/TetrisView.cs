using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris_AI
{
    public partial class TetrisView : UserControl
    {
        TetrisProcessor processor;

        public TetrisView()
        {
            processor = new TetrisProcessor();

            InitializeComponent();
        }


    }
}
