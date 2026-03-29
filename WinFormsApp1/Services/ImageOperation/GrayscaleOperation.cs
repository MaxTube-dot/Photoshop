using System.Drawing;

namespace WinFormsApp1.Services.ImageOperation
{
    public sealed class GrayscaleOperation : ColorImageOperationBase
    {
        public override string Name => "Grayscale";

        public override void ApplyPixel(ref Color color, CancellationToken token = default)
        {
            int gray = (int)(0.299 * color.R + 0.587 * color.G + 0.114 * color.B);
            gray = Math.Clamp(gray, 0, 255);
            color = Color.FromArgb(color.A, gray, gray, gray);
        }

        public override void ApplyPixel(ref byte b, ref byte g, ref byte r, ref byte a)
        {
            int gray = (r * 77 + g * 150 + b * 29) >> 8;
            byte y = (byte)gray;
            r = g = b = y;
        }
    }
}
