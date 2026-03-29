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

        public Form1()
        {
            InitializeComponent();

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            tbBrightness.Minimum = -100;
            tbBrightness.Maximum = 100;
            tbBrightness.TickFrequency = 10;
            tbBrightness.Value = 0;

            contrastBar.Minimum = -100;
            contrastBar.Maximum = 100;
            contrastBar.TickFrequency = 10;
            contrastBar.Value = 0;

            lblBrightness.Text = $"Яркость: {tbBrightness.Value}";
            lbContrast.Text = $"Контраст: {contrastBar.Value}";
            correctionBox.SelectedIndex = -1;
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

            try
            {
                Bitmap newPreview = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    return _pipelineService.RenderByPixelsFast(_original, ops, token);
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
            return true;
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
    }
}
