using System.Drawing;

namespace WinFormsApp1.Services.ImageOperation
{
    public enum GradationCorrectionMode
    {
        Linear = 0,
        Sinusoidal = 1,
        Exponential = 2,
        Logarithmic = 3
    }

    public sealed class GradationCorrectionOperation : ColorImageOperationBase
    {
        private readonly double _correctionFactor = 8 * Math.Log(2) / 255d;

        public override string Name => "GradationCorrection";
        public GradationCorrectionMode Mode { get; }

        public GradationCorrectionOperation(GradationCorrectionMode mode)
        {
            Mode = mode;
        }

        public override void ApplyPixel(ref Color color, CancellationToken token = default)
        {
            color = Color.FromArgb(
                color.A,
                ApplyChannel(color.R),
                ApplyChannel(color.G),
                ApplyChannel(color.B));
        }

        public override void ApplyPixel(ref byte b, ref byte g, ref byte r, ref byte a)
        {
            r = ApplyChannel(r);
            g = ApplyChannel(g);
            b = ApplyChannel(b);
        }

        private byte ApplyChannel(int channel)
        {
            if (Mode == GradationCorrectionMode.Linear)
            {
                return (byte)channel;
            }

            if (Mode == GradationCorrectionMode.Sinusoidal)
            {
                return (byte)SinusoidalCorrection(channel);
            }

            if (Mode == GradationCorrectionMode.Exponential)
            {
                return (byte)ExponentialCorrection(channel);
            }

            return (byte)LogarithmicCorrection(channel);
        }

        private int SinusoidalCorrection(int color)
        {
            double result = (255 / 2d) * Math.Sin(Math.PI / 255 * color - Math.PI / 2) + (255 / 2d);

            if (result < 0) result = 0;
            if (result > 255) result = 255;

            return Convert.ToInt32(result);
        }

        private int ExponentialCorrection(int color)
        {
            double result = Math.Exp(_correctionFactor * color) - 1;

            if (result < 0) result = 0;
            if (result > 255) result = 255;

            return Convert.ToInt32(result);
        }

        private int LogarithmicCorrection(int color)
        {
            double result = Math.Log(color + 1) / _correctionFactor;

            if (result < 0) result = 0;
            if (result > 255) result = 255;

            return Convert.ToInt32(result);
        }
    }
}
