using Microsoft.WindowsAPICodePack.Shell;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace LocalAniHubFront.Helpers
{
    public static class ThumbnailHelper
    {
        public static BitmapSource GetThumbnail(string filePath, int size = 128)
        {
            try
            {
                using (var shellFile = ShellFile.FromFilePath(filePath))
                {
                    var bitmap = shellFile.Thumbnail.LargeBitmap;
                    return ConvertToBitmapSource(bitmap);
                }
            }
            catch
            {
                return null;
            }
        }

        private static BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                var decoder = new PngBitmapDecoder(memory, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                return decoder.Frames[0];
            }
        }
    }
}
