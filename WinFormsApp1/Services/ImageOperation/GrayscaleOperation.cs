using System.Drawing;

namespace WinFormsApp1.Services.ImageOperation
{
    public sealed class GrayscaleOperation : IImageOperation
    {
        public string Name => "Grayscale";

        public Bitmap Apply(Bitmap input, CancellationToken token = default)
        {
            var output = new Bitmap(input.Width, input.Height);
            for (int y = 0; y < input.Height; y++)
            {
                if (token.IsCancellationRequested)
                    break;

                for (int x = 0; x < input.Width; x++)
                {
                    Color c = input.GetPixel(x, y);
                    int gray = (int)(0.299 * c.R + 0.587 * c.G + 0.114 * c.B);
                    gray = Math.Clamp(gray, 0, 255);
                    output.SetPixel(x, y, Color.FromArgb(c.A, gray, gray, gray));
                }
            }
            return output;
        }

        public void ApplyPixel(ref Color color, CancellationToken token = default)
        {
            int gray = (int)(0.299 * color.R + 0.587 * color.G + 0.114 * color.B);
            gray = Math.Clamp(gray, 0, 255);
            color = Color.FromArgb(color.A, gray, gray, gray);
        }

        public void ApplyPixel(ref byte b, ref byte g, ref byte r, ref byte a)
        {
            int gray = (r * 77 + g * 150 + b * 29) >> 8;
            byte y = (byte)gray;
            r = g = b = y;
        }
    }
}
