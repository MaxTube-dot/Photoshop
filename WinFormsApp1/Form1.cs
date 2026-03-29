using System.Drawing.Imaging;
using WinFormsApp1.Services.ImageOperation;
using WinFormsApp1.Services.ImagePipelineService;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private Bitmap? _original;
        private Bitmap? _preview;
        private readonly IImagePipelineService _pipelineService = new ImagePipelineService();
        private CancellationTokenSource? _renderCts;
        private bool _isResponsiveLayoutReady;

        public Form1()
        {
            InitializeComponent();
            ConfigureResponsiveLayout();

            pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
            MinimumSize = new Size(980, 700);

            tbBrightness.Minimum = -100;
            tbBrightness.Maximum = 100;
            tbBrightness.TickFrequency = 10;
            tbBrightness.Value = 0;

            contrastBar.Minimum = -100;
            contrastBar.Maximum = 100;
            contrastBar.TickFrequency = 10;
            contrastBar.Value = 0;

            gammaCor.Minimum = 10;
            gammaCor.Maximum = 500;
            gammaCor.TickFrequency = 10;
            gammaCor.Value = 100;

            scaleBar1.Minimum = 10;
            scaleBar1.Maximum = 1100;
            scaleBar1.TickFrequency = 10;
            scaleBar1.Value = 100;

            lblBrightness.Text = $"Яркость: {tbBrightness.Value}";
            lbContrast.Text = $"Контраст: {contrastBar.Value}";
            lbGamma.Text = $"Гамма: {GetGammaValue():0.00}";
            lbScale.Text = $"Масштаб: {scaleBar1.Value}%";
            correctionBox.SelectedIndex = (int)GradationCorrectionMode.Linear;
            interpolationBox1.SelectedIndex = (int)ImageInterpolationMethod.NearestNeighbor;
            imagePanel.SizeChanged += imagePanel_SizeChanged;
            _isResponsiveLayoutReady = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.OpenFile();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files|*.*"
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            // Важно: открываем без блокировки файла
            using var tmp = new Bitmap(ofd.FileName);
            _original?.Dispose();
            _original = new Bitmap(tmp);

            ApplyPipelineAndShow();
        }

        private async void ApplyPipelineAndShow()
        {
            if (_original == null) return;

            _renderCts?.Cancel();
            _renderCts = new CancellationTokenSource();
            var token = _renderCts.Token;

            var ops = BuildOperationsFromUi();
            var viewportSize = imagePanel.ClientSize;
            var scaleMultiplier = GetScaleFactor();
            var interpolationMethod = GetInterpolationMethod();

            try
            {
                Bitmap newPreview = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    var rendered = _pipelineService.RenderByPixelsFast(_original, ops, token);
                    var scaleFactor = GetPreviewScaleFactor(rendered.Size, viewportSize, scaleMultiplier);

                    if (Math.Abs(scaleFactor - 1.0d) < double.Epsilon)
                        return rendered;

                    try
                    {
                        return _pipelineService.Resize(rendered, scaleFactor, interpolationMethod, token);
                    }
                    finally
                    {
                        rendered.Dispose();
                    }
                }, token);

                if (token.IsCancellationRequested)
                    return;

                _preview?.Dispose();
                _preview = newPreview;
                SetPicture(_preview);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetPicture(Bitmap bmp)
        {
            var old = pictureBox1.Image;
            pictureBox1.Image = bmp;
            pictureBox1.Size = bmp.Size;
            old?.Dispose();
        }

        private List<IImageOperation> BuildOperationsFromUi()
        {
            var ops = new List<IImageOperation>();

            if (cbGrayscale.Checked)
                ops.Add(new GrayscaleOperation());

            if (tbBrightness.Value != 0)
                ops.Add(new BrightnessOperation(tbBrightness.Value));

            if (contrastBar.Value != 0)
                ops.Add(new ContrastOperation(contrastBar.Value));

            double gamma = GetGammaValue();
            if (Math.Abs(gamma - 1.0) > double.Epsilon)
                ops.Add(new GammaCorrectionOperation(gamma));

            if (TryGetGradationCorrectionMode(out var correctionMode))
                ops.Add(new GradationCorrectionOperation(correctionMode));

            return ops;
        }

        private bool TryGetGradationCorrectionMode(out GradationCorrectionMode mode)
        {
            mode = GradationCorrectionMode.Linear;

            if (correctionBox.SelectedIndex < 0)
                return false;

            if (!Enum.IsDefined(typeof(GradationCorrectionMode), correctionBox.SelectedIndex))
                return false;

            mode = (GradationCorrectionMode)correctionBox.SelectedIndex;
            return mode != GradationCorrectionMode.Linear;
        }

        private double GetGammaValue()
        {
            return gammaCor.Value / 100d;
        }

        private double GetScaleFactor()
        {
            return scaleBar1.Value / 100d;
        }

        private ImageInterpolationMethod GetInterpolationMethod()
        {
            if (!Enum.IsDefined(typeof(ImageInterpolationMethod), interpolationBox1.SelectedIndex))
                return ImageInterpolationMethod.NearestNeighbor;

            return (ImageInterpolationMethod)interpolationBox1.SelectedIndex;
        }

        private static double GetPreviewScaleFactor(Size imageSize, Size viewportSize, double scaleMultiplier)
        {
            if (imageSize.Width <= 0 || imageSize.Height <= 0)
                return scaleMultiplier;

            if (viewportSize.Width <= 0 || viewportSize.Height <= 0)
                return scaleMultiplier;

            double fitScaleX = viewportSize.Width / (double)imageSize.Width;
            double fitScaleY = viewportSize.Height / (double)imageSize.Height;
            double fitScale = Math.Min(fitScaleX, fitScaleY);

            return fitScale * scaleMultiplier;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _original?.Dispose();
            _preview?.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_preview == null)
            {
                MessageBox.Show("Нечего сохранять. Сначала откройте изображение.");
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Filter = "PNG|*.png|JPEG|*.jpg;*.jpeg|BMP|*.bmp",
                FileName = "result.png"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            ImageFormat format = ImageFormat.Png;
            string ext = Path.GetExtension(sfd.FileName).ToLowerInvariant();
            if (ext == ".jpg" || ext == ".jpeg") format = ImageFormat.Jpeg;
            else if (ext == ".bmp") format = ImageFormat.Bmp;

            _preview.Save(sfd.FileName, format);
        }

        private void cbGrayscale_CheckedChanged(object sender, EventArgs e)
        {
            ApplyPipelineAndShow();
        }

        private void tbBrightness_Scroll(object sender, EventArgs e)
        {
            lblBrightness.Text = $"Яркость: {tbBrightness.Value}";
        }

        private void tbBrightness_MouseUp(object sender, MouseEventArgs e)
        {
            lblBrightness.Text = $"Яркость: {tbBrightness.Value}";
            ApplyPipelineAndShow();
        }

        private void contrastBar_Scroll(object sender, EventArgs e)
        {
            lbContrast.Text = $"Контраст: {contrastBar.Value}";
        }

        private void contrastBar_MouseUp(object sender, MouseEventArgs e)
        {
            lbContrast.Text = $"Контраст: {contrastBar.Value}";
            ApplyPipelineAndShow();
        }

        private void correctionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyPipelineAndShow();
        }

        private void gammaCor_Scroll(object sender, EventArgs e)
        {
            lbGamma.Text = $"Гамма: {GetGammaValue():0.00}";
        }

        private void gammaCor_MouseUp(object sender, MouseEventArgs e)
        {
            lbGamma.Text = $"Гамма: {GetGammaValue():0.00}";
            ApplyPipelineAndShow();
        }

        private void scaleBar1_Scroll(object sender, EventArgs e)
        {
            lbScale.Text = $"Масштаб: {scaleBar1.Value}%";
        }

        private void scaleBar1_MouseUp(object sender, MouseEventArgs e)
        {
            lbScale.Text = $"Масштаб: {scaleBar1.Value}%";
            ApplyPipelineAndShow();
        }

        private void lbScale_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void interpolationBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyPipelineAndShow();
        }

        private void ConfigureResponsiveLayout()
        {
            SuspendLayout();

            var toolbarPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                WrapContents = false
            };

            var contentLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                RowCount = 1
            };
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72F));
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var settingsLayout = CreateSettingsLayout();

            var mainLayout = new TableLayoutPanel
            {
                ColumnCount = 1,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = new Padding(12),
                RowCount = 2
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            btnOpen.AutoSize = true;
            btnSave.AutoSize = true;
            btnOpen.Margin = new Padding(0, 0, 8, 0);
            btnSave.Margin = Padding.Empty;

            imagePanel.Dock = DockStyle.Fill;
            imagePanel.Margin = new Padding(0, 0, 12, 0);

            toolbarPanel.Controls.Add(btnOpen);
            toolbarPanel.Controls.Add(btnSave);

            contentLayout.Controls.Add(imagePanel, 0, 0);
            contentLayout.Controls.Add(settingsLayout, 1, 0);

            mainLayout.Controls.Add(toolbarPanel, 0, 0);
            mainLayout.Controls.Add(contentLayout, 0, 1);

            Controls.Add(mainLayout);

            ResumeLayout(true);
        }

        private TableLayoutPanel CreateSettingsLayout()
        {
            var settingsLayout = new TableLayoutPanel
            {
                AutoScroll = true,
                ColumnCount = 1,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            settingsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            ConfigureSettingsControl(lblBrightness, new Padding(0, 0, 0, 4));
            ConfigureSettingsControl(tbBrightness, new Padding(0, 0, 0, 12));
            ConfigureSettingsControl(cbGrayscale, new Padding(0, 0, 0, 12));
            ConfigureSettingsControl(lbContrast, new Padding(0, 0, 0, 4));
            ConfigureSettingsControl(contrastBar, new Padding(0, 0, 0, 12));
            ConfigureSettingsControl(label1, new Padding(0, 0, 0, 4));
            ConfigureSettingsControl(correctionBox, new Padding(0, 0, 0, 12));
            ConfigureSettingsControl(lbGamma, new Padding(0, 0, 0, 4));
            ConfigureSettingsControl(gammaCor, new Padding(0, 0, 0, 12));
            ConfigureSettingsControl(lbScale, new Padding(0, 0, 0, 4));
            ConfigureSettingsControl(scaleBar1, new Padding(0, 0, 0, 12));
            ConfigureSettingsControl(label2, new Padding(0, 0, 0, 4));
            ConfigureSettingsControl(interpolationBox1, Padding.Empty);

            AddSettingsRow(settingsLayout, lblBrightness, SizeType.AutoSize, 0);
            AddSettingsRow(settingsLayout, tbBrightness, SizeType.Absolute, 45F);
            AddSettingsRow(settingsLayout, cbGrayscale, SizeType.AutoSize, 0);
            AddSettingsRow(settingsLayout, lbContrast, SizeType.AutoSize, 0);
            AddSettingsRow(settingsLayout, contrastBar, SizeType.Absolute, 45F);
            AddSettingsRow(settingsLayout, label1, SizeType.AutoSize, 0);
            AddSettingsRow(settingsLayout, correctionBox, SizeType.AutoSize, 0);
            AddSettingsRow(settingsLayout, lbGamma, SizeType.AutoSize, 0);
            AddSettingsRow(settingsLayout, gammaCor, SizeType.Absolute, 45F);
            AddSettingsRow(settingsLayout, lbScale, SizeType.AutoSize, 0);
            AddSettingsRow(settingsLayout, scaleBar1, SizeType.Absolute, 45F);
            AddSettingsRow(settingsLayout, label2, SizeType.AutoSize, 0);
            AddSettingsRow(settingsLayout, interpolationBox1, SizeType.AutoSize, 0);

            settingsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            settingsLayout.RowCount++;

            return settingsLayout;
        }

        private static void ConfigureSettingsControl(Control control, Padding margin)
        {
            control.Dock = DockStyle.Fill;
            control.Margin = margin;
        }

        private static void AddSettingsRow(TableLayoutPanel layout, Control control, SizeType sizeType, float size)
        {
            layout.RowStyles.Add(new RowStyle(sizeType, size));
            layout.Controls.Add(control, 0, layout.RowCount);
            layout.RowCount++;
        }

        private void imagePanel_SizeChanged(object? sender, EventArgs e)
        {
            if (!_isResponsiveLayoutReady || _original == null)
                return;

            ApplyPipelineAndShow();
        }
    }
}
