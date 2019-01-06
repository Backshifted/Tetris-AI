namespace Tetris_AI
{
    partial class TetrisViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tetrisView1 = new Tetris_AI.TetrisView();
            this.SuspendLayout();
            // 
            // tetrisView1
            // 
            this.tetrisView1.BlockColor = System.Drawing.SystemColors.ControlText;
            this.tetrisView1.BlockSize = 30;
            this.tetrisView1.BorderColor = System.Drawing.Color.Empty;
            this.tetrisView1.BorderWidth = 0;
            this.tetrisView1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tetrisView1.Location = new System.Drawing.Point(0, 0);
            this.tetrisView1.Name = "tetrisView1";
            this.tetrisView1.Processor = null;
            this.tetrisView1.Size = new System.Drawing.Size(300, 660);
            this.tetrisView1.TabIndex = 12;
            // 
            // TetrisViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 660);
            this.Controls.Add(this.tetrisView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "TetrisViewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tetris View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TetrisViewForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion
        private TetrisView tetrisView1;
    }
}

