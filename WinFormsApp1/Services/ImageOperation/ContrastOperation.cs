using System.Drawing;

namespace WinFormsApp1.Services.ImageOperation
{
    public sealed class ContrastOperation : ColorImageOperationBase
    {
        private readonly double _factor;

        public override string Name => "Contrast";
        public int Amount { get; }

        public ContrastOperation(int amount)
        {
            Amount = amount;
            _factor = (100.0 + amount) / 100.0;
        }

        public override void ApplyPixel(ref Color color, CancellationToken token = default)
        {
            color = Color.FromArgb(
                color.A,
                ApplyContrast(color.R),
                ApplyContrast(color.G),
                ApplyContrast(color.B));
        }

        public override void ApplyPixel(ref byte b, ref byte g, ref byte r, ref byte a)
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
