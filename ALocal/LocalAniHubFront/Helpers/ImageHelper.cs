using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System;

namespace LocalAniHubFront.Helpers
{
    public static class ImageHelper
    {
        // 加载图片
        public static ImageSource LoadImage(string filePath)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze(); // 跨线程安全
                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"图片加载失败: {ex.Message}");
                return null;
            }
        }

        // 计算图片平均颜色
        public static Color CalculateAverageColor(ImageSource imageSource)
        {
            if (imageSource is not BitmapSource bitmapSource)
                return Colors.DarkSlateGray;

            try
            {
                // 转换为32bpp格式
                var formatConverted = new FormatConvertedBitmap(
                    bitmapSource,
                    PixelFormats.Bgra32,
                    null,
                    0);

                int stride = formatConverted.PixelWidth * 4;
                byte[] pixels = new byte[stride * formatConverted.PixelHeight];
                formatConverted.CopyPixels(pixels, stride, 0);

                long r = 0, g = 0, b = 0;
                int totalPixels = pixels.Length / 4;

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    b += pixels[i];
                    g += pixels[i + 1];
                    r += pixels[i + 2];
                }

                return Color.FromRgb(
                    (byte)(r / totalPixels),
                    (byte)(g / totalPixels),
                    (byte)(b / totalPixels));
            }
            catch
            {
                return Colors.DarkSlateGray;
            }
        }

        // System.Drawing.Image 转换为 ImageSource
        public static BitmapSource ToImageSource(System.Drawing.Image image)
        {
            if (image == null)
                return null;

            using var ms = new MemoryStream();

            // 深拷贝：重新生成一份干净的 Bitmap
            using var cleanBitmap = new System.Drawing.Bitmap(image); // 防止原 Bitmap 被锁定或不完整

            cleanBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            bitmap.Freeze(); // WPF 跨线程安全

            return bitmap;
        }
    }
}