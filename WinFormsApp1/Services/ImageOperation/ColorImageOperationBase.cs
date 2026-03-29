using System.Drawing;

namespace WinFormsApp1.Services.ImageOperation
{
    public abstract class ColorImageOperationBase : IImageOperation
    {
        public abstract string Name { get; }

        public Bitmap Apply(Bitmap input, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(input);

            var output = new Bitmap(input.Width, input.Height);

            for (int y = 0; y < input.Height; y++)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                for (int x = 0; x < input.Width; x++)
                {
                    Color color = input.GetPixel(x, y);
                    ApplyPixel(ref color, token);
                    output.SetPixel(x, y, color);
                }
            }

            return output;
        }

        public abstract void ApplyPixel(ref Color input, CancellationToken token = default);
        public abstract void ApplyPixel(ref byte b, ref byte g, ref byte r, ref byte a);
    }
}
