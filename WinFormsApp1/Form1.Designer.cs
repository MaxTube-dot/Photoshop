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
            imagePanel = new Panel();
            pictureBox1 = new PictureBox();
            cbGrayscale = new CheckBox();
            tbBrightness = new TrackBar();
            lblBrightness = new Label();
            contrastBar = new TrackBar();
            lbContrast = new Label();
            correctionBox = new ComboBox();
            label1 = new Label();
            lbGamma = new Label();
            gammaCor = new TrackBar();
            lbScale = new Label();
            scaleBar1 = new TrackBar();
            interpolationBox1 = new ComboBox();
            label2 = new Label();
            imagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbBrightness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)contrastBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gammaCor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)scaleBar1).BeginInit();
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
            // imagePanel
            // 
            imagePanel.AutoScroll = true;
            imagePanel.BorderStyle = BorderStyle.FixedSingle;
            imagePanel.Controls.Add(pictureBox1);
            imagePanel.Location = new Point(12, 41);
            imagePanel.Name = "imagePanel";
            imagePanel.Size = new Size(665, 394);
            imagePanel.TabIndex = 2;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(0, 0);
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
            cbGrayscale.Size = new Size(117, 19);
            cbGrayscale.TabIndex = 3;
            cbGrayscale.Text = "Градиент серого";
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
            lblBrightness.Location = new Point(683, 44);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(51, 15);
            lblBrightness.TabIndex = 5;
            lblBrightness.Text = "Яркость";
            // 
            // contrastBar
            // 
            contrastBar.Location = new Point(683, 159);
            contrastBar.Name = "contrastBar";
            contrastBar.Size = new Size(183, 45);
            contrastBar.TabIndex = 6;
            contrastBar.Scroll += contrastBar_Scroll;
            contrastBar.MouseUp += contrastBar_MouseUp;
            // 
            // lbContrast
            // 
            lbContrast.AutoSize = true;
            lbContrast.Location = new Point(683, 141);
            lbContrast.Name = "lbContrast";
            lbContrast.Size = new Size(88, 15);
            lbContrast.TabIndex = 7;
            lbContrast.Text = "Контрастность";
            // 
            // correctionBox
            // 
            correctionBox.AutoCompleteCustomSource.AddRange(new string[] { "Линейная", "Синусоидальная", "Экспоненциальная", "Логарифмическая" });
            correctionBox.FormattingEnabled = true;
            correctionBox.Items.AddRange(new object[] { "Линейная", "Синусоидальная", "Экспоненциальная", "Логирифмическая" });
            correctionBox.Location = new Point(683, 225);
            correctionBox.Name = "correctionBox";
            correctionBox.Size = new Size(183, 23);
            correctionBox.TabIndex = 8;
            correctionBox.SelectedIndexChanged += correctionBox_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(683, 207);
            label1.Name = "label1";
            label1.Size = new Size(67, 15);
            label1.TabIndex = 9;
            label1.Text = "Коррекция";
            // 
            // lbGamma
            // 
            lbGamma.AutoSize = true;
            lbGamma.Location = new Point(683, 268);
            lbGamma.Name = "lbGamma";
            lbGamma.Size = new Size(105, 15);
            lbGamma.TabIndex = 11;
            lbGamma.Text = "Гамма коррекция";
            // 
            // gammaCor
            // 
            gammaCor.Location = new Point(683, 286);
            gammaCor.Name = "gammaCor";
            gammaCor.Size = new Size(183, 45);
            gammaCor.TabIndex = 10;
            gammaCor.Scroll += gammaCor_Scroll;
            gammaCor.MouseUp += gammaCor_MouseUp;
            // 
            // lbScale
            // 
            lbScale.AutoSize = true;
            lbScale.Location = new Point(684, 329);
            lbScale.Name = "lbScale";
            lbScale.Size = new Size(59, 15);
            lbScale.TabIndex = 12;
            lbScale.Text = "Масштаб";
            lbScale.Click += lbScale_Click;
            // 
            // scaleBar1
            // 
            scaleBar1.Location = new Point(684, 347);
            scaleBar1.Maximum = 30;
            scaleBar1.Name = "scaleBar1";
            scaleBar1.Size = new Size(183, 45);
            scaleBar1.TabIndex = 13;
            scaleBar1.Scroll += scaleBar1_Scroll;
            scaleBar1.MouseUp += scaleBar1_MouseUp;
            // 
            // interpolationBox1
            // 
            interpolationBox1.AutoCompleteCustomSource.AddRange(new string[] { "Линейная", "Синусоидальная", "Экспоненциальная", "Логарифмическая" });
            interpolationBox1.FormattingEnabled = true;
            interpolationBox1.Items.AddRange(new object[] { "Метод ближайшего соседа", "Билинейная интерполяция" });
            interpolationBox1.Location = new Point(683, 416);
            interpolationBox1.Name = "interpolationBox1";
            interpolationBox1.Size = new Size(183, 23);
            interpolationBox1.TabIndex = 14;
            interpolationBox1.SelectedIndexChanged += interpolationBox1_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(684, 395);
            label2.Name = "label2";
            label2.Size = new Size(125, 15);
            label2.TabIndex = 15;
            label2.Text = "Метод интерполяции";
            label2.Click += label2_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(889, 619);
            Controls.Add(label2);
            Controls.Add(interpolationBox1);
            Controls.Add(scaleBar1);
            Controls.Add(lbScale);
            Controls.Add(lbGamma);
            Controls.Add(gammaCor);
            Controls.Add(label1);
            Controls.Add(correctionBox);
            Controls.Add(lbContrast);
            Controls.Add(contrastBar);
            Controls.Add(lblBrightness);
            Controls.Add(tbBrightness);
            Controls.Add(cbGrayscale);
            Controls.Add(imagePanel);
            Controls.Add(btnSave);
            Controls.Add(btnOpen);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            imagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)contrastBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)gammaCor).EndInit();
            ((System.ComponentModel.ISupportInitialize)scaleBar1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel imagePanel;
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
        private TrackBar contrastBar;
        private Label lbContrast;
        private ComboBox correctionBox;
        private Label label1;
        private Label lbGamma;
        private TrackBar gammaCor;
        private Label lbScale;
        private TrackBar scaleBar1;
        private ComboBox interpolationBox1;
        private Label label2;
    }
}
