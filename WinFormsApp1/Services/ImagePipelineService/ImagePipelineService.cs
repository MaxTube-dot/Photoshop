using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using WinFormsApp1.Services.ImageOperation;

namespace WinFormsApp1.Services.ImagePipelineService
{
    public enum ImageInterpolationMethod
    {
        NearestNeighbor = 0,
        Bilinear = 1
    }

    public interface IImagePipelineService
    {
        /// <summary>
        /// Применяет операции к оригиналу и возвращает НОВЫЙ Bitmap (caller обязан Dispose()).
        /// </summary>
        Bitmap Render(Bitmap original, IReadOnlyList<IImageOperation> operations, CancellationToken ct = default);
        Bitmap RenderByPixels(Bitmap original, IReadOnlyList<IImageOperation> operations, CancellationToken ct = default);
        Bitmap RenderByPixelsFast(Bitmap original, IReadOnlyList<IImageOperation> operations, CancellationToken ct = default);
        Bitmap Resize(Bitmap input, double scale, ImageInterpolationMethod interpolationMethod, CancellationToken ct = default);
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

        public Bitmap Resize(Bitmap input, double scale, ImageInterpolationMethod interpolationMethod, CancellationToken ct = default)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (scale <= 0) throw new ArgumentOutOfRangeException(nameof(scale));

            int sourceWidth = input.Width;
            int sourceHeight = input.Height;
            int targetWidth = Math.Max(1, (int)Math.Round(sourceWidth * scale));
            int targetHeight = Math.Max(1, (int)Math.Round(sourceHeight * scale));

            if (targetWidth == sourceWidth && targetHeight == sourceHeight)
                return new Bitmap(input);

            using var src = Ensure32bppArgb(input);
            var dst = new Bitmap(targetWidth, targetHeight, PixelFormat.Format32bppArgb);
            var srcRect = new Rectangle(0, 0, sourceWidth, sourceHeight);
            var dstRect = new Rectangle(0, 0, dst.Width, dst.Height);

            BitmapData srcData = null!;
            BitmapData dstData = null!;

            try
            {
                srcData = src.LockBits(srcRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                dstData = dst.LockBits(dstRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                int srcStride = srcData.Stride;
                int dstStride = dstData.Stride;
                int srcBytes = srcStride * src.Height;
                int dstBytes = dstStride * dst.Height;

                byte[] srcBuffer = new byte[srcBytes];
                byte[] dstBuffer = new byte[dstBytes];
                Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBytes);

                var po = new ParallelOptions { CancellationToken = ct };

                switch (interpolationMethod)
                {
                    case ImageInterpolationMethod.Bilinear:
                        ResizeBilinear(srcBuffer, dstBuffer, sourceWidth, sourceHeight, targetWidth, targetHeight, srcStride, dstStride, po);
                        break;
                    case ImageInterpolationMethod.NearestNeighbor:
                    default:
                        ResizeNearestNeighbor(srcBuffer, dstBuffer, sourceWidth, sourceHeight, targetWidth, targetHeight, srcStride, dstStride, po);
                        break;
                }

                Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBytes);
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

        private static void ResizeNearestNeighbor(
            byte[] srcBuffer,
            byte[] dstBuffer,
            int sourceWidth,
            int sourceHeight,
            int targetWidth,
            int targetHeight,
            int srcStride,
            int dstStride,
            ParallelOptions po)
        {
            double xRatio = sourceWidth / (double)targetWidth;
            double yRatio = sourceHeight / (double)targetHeight;

            Parallel.For(0, targetHeight, po, y =>
            {
                int srcY = Clamp((int)Math.Round((y + 0.5d) * yRatio - 0.5d), 0, sourceHeight - 1);
                int srcRow = srcY * srcStride;
                int dstRow = y * dstStride;

                for (int x = 0; x < targetWidth; x++)
                {
                    int srcX = Clamp((int)Math.Round((x + 0.5d) * xRatio - 0.5d), 0, sourceWidth - 1);
                    int srcIndex = srcRow + srcX * 4;
                    int dstIndex = dstRow + x * 4;

                    dstBuffer[dstIndex + 0] = srcBuffer[srcIndex + 0];
                    dstBuffer[dstIndex + 1] = srcBuffer[srcIndex + 1];
                    dstBuffer[dstIndex + 2] = srcBuffer[srcIndex + 2];
                    dstBuffer[dstIndex + 3] = srcBuffer[srcIndex + 3];
                }
            });
        }

        private static void ResizeBilinear(
            byte[] srcBuffer,
            byte[] dstBuffer,
            int sourceWidth,
            int sourceHeight,
            int targetWidth,
            int targetHeight,
            int srcStride,
            int dstStride,
            ParallelOptions po)
        {
            double xRatio = sourceWidth / (double)targetWidth;
            double yRatio = sourceHeight / (double)targetHeight;

            Parallel.For(0, targetHeight, po, y =>
            {
                double sourceY = Clamp((y + 0.5d) * yRatio - 0.5d, 0d, sourceHeight - 1d);
                int y0 = (int)Math.Floor(sourceY);
                int y1 = Math.Min(y0 + 1, sourceHeight - 1);
                double wy = sourceY - y0;

                int srcRow0 = y0 * srcStride;
                int srcRow1 = y1 * srcStride;
                int dstRow = y * dstStride;

                for (int x = 0; x < targetWidth; x++)
                {
                    double sourceX = Clamp((x + 0.5d) * xRatio - 0.5d, 0d, sourceWidth - 1d);
                    int x0 = (int)Math.Floor(sourceX);
                    int x1 = Math.Min(x0 + 1, sourceWidth - 1);
                    double wx = sourceX - x0;

                    int topLeft = srcRow0 + x0 * 4;
                    int topRight = srcRow0 + x1 * 4;
                    int bottomLeft = srcRow1 + x0 * 4;
                    int bottomRight = srcRow1 + x1 * 4;
                    int dstIndex = dstRow + x * 4;

                    dstBuffer[dstIndex + 0] = InterpolateChannel(srcBuffer[topLeft + 0], srcBuffer[topRight + 0], srcBuffer[bottomLeft + 0], srcBuffer[bottomRight + 0], wx, wy);
                    dstBuffer[dstIndex + 1] = InterpolateChannel(srcBuffer[topLeft + 1], srcBuffer[topRight + 1], srcBuffer[bottomLeft + 1], srcBuffer[bottomRight + 1], wx, wy);
                    dstBuffer[dstIndex + 2] = InterpolateChannel(srcBuffer[topLeft + 2], srcBuffer[topRight + 2], srcBuffer[bottomLeft + 2], srcBuffer[bottomRight + 2], wx, wy);
                    dstBuffer[dstIndex + 3] = InterpolateChannel(srcBuffer[topLeft + 3], srcBuffer[topRight + 3], srcBuffer[bottomLeft + 3], srcBuffer[bottomRight + 3], wx, wy);
                }
            });
        }

        private static byte InterpolateChannel(byte topLeft, byte topRight, byte bottomLeft, byte bottomRight, double wx, double wy)
        {
            double top = topLeft + (topRight - topLeft) * wx;
            double bottom = bottomLeft + (bottomRight - bottomLeft) * wx;
            double value = top + (bottom - top) * wy;
            return (byte)Math.Round(value);
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
