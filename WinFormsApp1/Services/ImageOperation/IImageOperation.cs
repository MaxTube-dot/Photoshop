namespace WinFormsApp1.Services.ImageOperation
{
    public interface IImageOperation
    {
        string Name { get; }
        Bitmap Apply(Bitmap input, CancellationToken token = default);
        void ApplyPixel(ref Color input, CancellationToken token = default);
        void ApplyPixel(ref byte b, ref byte g, ref byte r, ref byte a);
    }
}
