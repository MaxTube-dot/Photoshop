namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnOpen = new Button();
            btnSave = new Button();
            pictureBox1 = new PictureBox();
            cbGrayscale = new CheckBox();
            tbBrightness = new TrackBar();
            lblBrightness = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbBrightness).BeginInit();
            SuspendLayout();
            // 
            // btnOpen
            // 
            btnOpen.Location = new Point(12, 12);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(75, 23);
            btnOpen.TabIndex = 0;
            btnOpen.Text = "Открыть";
            btnOpen.UseVisualStyleBackColor = true;
            btnOpen.Click += btnOpen_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(93, 12);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 1;
            btnSave.Text = "Сохранить";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 41);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(665, 394);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // cbGrayscale
            // 
            cbGrayscale.AutoSize = true;
            cbGrayscale.Location = new Point(683, 113);
            cbGrayscale.Name = "cbGrayscale";
            cbGrayscale.Size = new Size(60, 19);
            cbGrayscale.TabIndex = 3;
            cbGrayscale.Text = "Серое";
            cbGrayscale.UseVisualStyleBackColor = true;
            cbGrayscale.CheckedChanged += cbGrayscale_CheckedChanged;
            // 
            // tbBrightness
            // 
            tbBrightness.Location = new Point(683, 62);
            tbBrightness.Name = "tbBrightness";
            tbBrightness.Size = new Size(183, 45);
            tbBrightness.TabIndex = 4;
            tbBrightness.Scroll += tbBrightness_Scroll;
            tbBrightness.MouseUp += tbBrightness_MouseUp;
            // 
            // lblBrightness
            // 
            lblBrightness.AutoSize = true;
            lblBrightness.Location = new Point(762, 41);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(38, 15);
            lblBrightness.TabIndex = 5;
            lblBrightness.Text = "label1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(889, 619);
            Controls.Add(lblBrightness);
            Controls.Add(tbBrightness);
            Controls.Add(cbGrayscale);
            Controls.Add(pictureBox1);
            Controls.Add(btnSave);
            Controls.Add(btnOpen);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbBrightness).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem менюToolStripMenuItem;
        private ToolStripMenuItem открытьКартинкуToolStripMenuItem;
        private ToolStripMenuItem информацияToolStripMenuItem;
        private ToolStripMenuItem оПрограммеToolStripMenuItem;
        private Button btnOpen;
        private Button btnSave;
        private CheckBox cbGrayscale;
        private TrackBar tbBrightness;
        private Label lblBrightness;
    }
}
