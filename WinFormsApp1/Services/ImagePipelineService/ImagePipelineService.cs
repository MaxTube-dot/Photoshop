using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using WinFormsApp1.Services.ImageOperation;

namespace WinFormsApp1.Services.ImagePipelineService
{
    public interface IImagePipelineService
    {
        /// <summary>
        /// Применяет операции к оригиналу и возвращает НОВЫЙ Bitmap (caller обязан Dispose()).
        /// </summary>
        Bitmap Render(Bitmap original, IReadOnlyList<IImageOperation> operations, CancellationToken ct = default);
        Bitmap RenderByPixels(Bitmap original, IReadOnlyList<IImageOperation> operations, CancellationToken ct = default);
        Bitmap RenderByPixelsFast(Bitmap original, IReadOnlyList<IImageOperation> operations, CancellationToken ct = default);
    }

    public sealed class ImagePipelineService : IImagePipelineService
    {
        private readonly object _lockRead = new object();
        public Bitmap Render(Bitmap original, IReadOnlyList<IImageOperation> operations, CancellationToken ct = default)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (operations == null) throw new ArgumentNullException(nameof(operations));

            Bitmap current = new Bitmap(original);

            try
            {
                var st = Stopwatch.StartNew();

                foreach (var op in operations)
                {
                    if (op == null) continue;


                    if (ct.IsCancellationRequested)
                        break;

                    Bitmap next = op.Apply(current, ct);
                    current.Dispose();
                    current = next;
                }

                var t2 = st.ElapsedMilliseconds;

                return current;
            }
            catch
            {
                current.Dispose();
                throw;
            }
        }

        public Bitmap RenderByPixels(Bitmap original, IReadOnlyList<IImageOperation> operations, CancellationToken ct = default)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (operations == null) throw new ArgumentNullException(nameof(operations));

            Bitmap current = new Bitmap(original);

            try
            {
                var output = new Bitmap(current.Width, current.Height);
                var Height = current.Height;
                var Width = current.Width;
                Color[,] outputColor = new Color[Width, Height];

               var st =  Stopwatch.StartNew();

                Parallel.For(0, Height, y =>
                {
                    if (ct.IsCancellationRequested)
                        return;

                    for (int x = 0; x < Width; x++)
                    {
                        Color c;
                        lock (_lockRead)
                        {
                             c = current.GetPixel(x, y);
                        }

                        foreach (var op in operations)
                        {
                            op.ApplyPixel(ref c);
                        }

                        outputColor[x, y] = c;
                    }
                });

                var t = st.ElapsedMilliseconds;

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        output.SetPixel(x, y, outputColor[x,y]);
                    }
                }

                var t2 = st.ElapsedMilliseconds;

                return output;
            }
            catch
            {
                current.Dispose();
                throw;
            }
        }


        public Bitmap RenderByPixelsFast(Bitmap original, IReadOnlyList<IImageOperation> operations, CancellationToken ct = default)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (operations == null) throw new ArgumentNullException(nameof(operations));

            // Приводим к 32bppArgb (важно для предсказуемого формата)
            var st = Stopwatch.StartNew();

            using var src = Ensure32bppArgb(original);
            var dst = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);

            var rect = new Rectangle(0, 0, src.Width, src.Height);

            BitmapData srcData = null!;
            BitmapData dstData = null!;

            try
            {
                srcData = src.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                dstData = dst.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                int height = src.Height;
                int width = src.Width;
                int stride = srcData.Stride;
                int bytes = stride * height;

                byte[] buffer = new byte[bytes];
                Marshal.Copy(srcData.Scan0, buffer, 0, bytes);

                var po = new ParallelOptions { CancellationToken = ct };

                Parallel.For(0, height, po, y =>
                {
                    int row = y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int i = row + x * 4; // BGRA

                        byte b = buffer[i + 0];
                        byte g = buffer[i + 1];
                        byte r = buffer[i + 2];
                        byte a = buffer[i + 3];

                        for (int k = 0; k < operations.Count; k++)
                            operations[k].ApplyPixel(ref b, ref g, ref r, ref a);

                        buffer[i + 0] = b;
                        buffer[i + 1] = g;
                        buffer[i + 2] = r;
                        buffer[i + 3] = a;
                    }
                });

                Marshal.Copy(buffer, 0, dstData.Scan0, bytes);

                var t2 = st.ElapsedMilliseconds;
                return dst;
            }
            catch
            {
                dst.Dispose();
                throw;
            }
            finally
            {
                if (srcData != null) src.UnlockBits(srcData);
                if (dstData != null) dst.UnlockBits(dstData);
            }
        }

        private static Bitmap Ensure32bppArgb(Bitmap input)
        {
            if (input.PixelFormat == PixelFormat.Format32bppArgb)
                return (Bitmap)input.Clone();

            var clone = new Bitmap(input.Width, input.Height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(clone);
            g.DrawImage(input, new Rectangle(0, 0, clone.Width, clone.Height));
            return clone;
        }
    }
}
