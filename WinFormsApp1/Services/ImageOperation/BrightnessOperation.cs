using System.Drawing;

namespace WinFormsApp1.Services.ImageOperation
{
    public sealed class BrightnessOperation : IImageOperation
    {
        public string Name => "Brightness";
        public int Delta { get; }

        public BrightnessOperation(int delta) => Delta = delta;

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
                    int r = Math.Clamp(c.R + Delta, 0, 255);
                    int g = Math.Clamp(c.G + Delta, 0, 255);
                    int b = Math.Clamp(c.B + Delta, 0, 255);
                    output.SetPixel(x, y, Color.FromArgb(c.A, r, g, b));
                }
            }
            return output;
        }

        public  void ApplyPixel(ref Color color, CancellationToken token = default)
        {
            int r = Math.Clamp(color.R + Delta, 0, 255);
            int g = Math.Clamp(color.G + Delta, 0, 255);
            int b = Math.Clamp(color.B + Delta, 0, 255);

            color = Color.FromArgb(color.A, r, g, b);
        }

        public void ApplyPixel(ref byte b, ref byte g, ref byte r, ref byte a)
        {
            r = ClampToByte(r + Delta);
            g = ClampToByte(g + Delta);
            b = ClampToByte(b + Delta);
        }

        private static byte ClampToByte(int v) => (byte)(v < 0 ? 0 : (v > 255 ? 255 : v));
    }
}
