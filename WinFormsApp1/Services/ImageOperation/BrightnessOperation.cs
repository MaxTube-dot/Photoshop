using System.Drawing;

namespace WinFormsApp1.Services.ImageOperation
{
    public sealed class BrightnessOperation : ColorImageOperationBase
    {
        public override string Name => "Brightness";
        public int Delta { get; }

        public BrightnessOperation(int delta) => Delta = delta;

        public override void ApplyPixel(ref Color color, CancellationToken token = default)
        {
            int r = Math.Clamp(color.R + Delta, 0, 255);
            int g = Math.Clamp(color.G + Delta, 0, 255);
            int b = Math.Clamp(color.B + Delta, 0, 255);

            color = Color.FromArgb(color.A, r, g, b);
        }

        public override void ApplyPixel(ref byte b, ref byte g, ref byte r, ref byte a)
        {
            r = ClampToByte(r + Delta);
            g = ClampToByte(g + Delta);
            b = ClampToByte(b + Delta);
        }

        private static byte ClampToByte(int value) => (byte)(value < 0 ? 0 : (value > 255 ? 255 : value));
    }
}
