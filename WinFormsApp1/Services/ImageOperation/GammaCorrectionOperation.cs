using System.Drawing;

namespace WinFormsApp1.Services.ImageOperation
{
    public sealed class GammaCorrectionOperation : ColorImageOperationBase
    {
        public override string Name => "Gamma";
        public double Gamma { get; }

        public GammaCorrectionOperation(double gamma)
        {
            Gamma = gamma;
        }

        public override void ApplyPixel(ref Color color, CancellationToken token = default)
        {
            color = Color.FromArgb(
                color.A,
                ApplyGamma(color.R),
                ApplyGamma(color.G),
                ApplyGamma(color.B));
        }

        public override void ApplyPixel(ref byte b, ref byte g, ref byte r, ref byte a)
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
