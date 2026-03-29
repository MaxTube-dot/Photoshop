using System.Drawing;

namespace WinFormsApp1.Services.ImageOperation
{
    public sealed class GammaCorrectionOperation : IImageOperation
    {
        public string Name => "Gamma";
        public double Gamma { get; }

        public GammaCorrectionOperation(double gamma)
        {
            Gamma = gamma;
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
                            ApplyGamma(c.R),
                            ApplyGamma(c.G),
                            ApplyGamma(c.B)));
                }
            }

            return output;
        }

        public void ApplyPixel(ref Color color, CancellationToken token = default)
        {
            color = Color.FromArgb(
                color.A,
                ApplyGamma(color.R),
                ApplyGamma(color.G),
                ApplyGamma(color.B));
        }

        public void ApplyPixel(ref byte b, ref byte g, ref byte r, ref byte a)
        {
            r = ApplyGamma(r);
            g = ApplyGamma(g);
            b = ApplyGamma(b);
        }

        private byte ApplyGamma(int color)
        {
            double result = Math.Pow(color / 255d, Gamma) * 255;

            if (result > 255) result = 255;
            if (result < 0) result = 0;

            return (byte)Convert.ToInt32(result);
        }
    }
}
