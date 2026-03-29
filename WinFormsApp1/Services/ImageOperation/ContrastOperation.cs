using System.Drawing;

namespace WinFormsApp1.Services.ImageOperation
{
    public sealed class ContrastOperation : IImageOperation
    {
        private readonly double _factor;

        public string Name => "Contrast";
        public int Amount { get; }

        public ContrastOperation(int amount)
        {
            Amount = amount;
            _factor = (100.0 + amount) / 100.0;
        }

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
                    output.SetPixel(
                        x,
                        y,
                        Color.FromArgb(
                            c.A,
                            ApplyContrast(c.R),
                            ApplyContrast(c.G),
                            ApplyContrast(c.B)));
                }
            }

            return output;
        }

        public void ApplyPixel(ref Color color, CancellationToken token = default)
        {
            color = Color.FromArgb(
                color.A,
                ApplyContrast(color.R),
                ApplyContrast(color.G),
                ApplyContrast(color.B));
        }

        public void ApplyPixel(ref byte b, ref byte g, ref byte r, ref byte a)
        {
            r = ApplyContrast(r);
            g = ApplyContrast(g);
            b = ApplyContrast(b);
        }

        private byte ApplyContrast(int channel)
        {
            int adjusted = (int)Math.Round((channel - 128) * _factor + 128);
            return ClampToByte(adjusted);
        }

        private static byte ClampToByte(int value) => (byte)(value < 0 ? 0 : (value > 255 ? 255 : value));
    }
}
