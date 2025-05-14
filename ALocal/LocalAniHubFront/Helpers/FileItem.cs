using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LocalAniHubFront.Helpers
{
    public class FileItem
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public ImageSource Thumbnail { get; set; }
        public long Size { get; set; }
        public BitmapImage PreviewImage
        {
            get
            {
                if (IsImage && File.Exists(FullPath))
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(FullPath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad; // 关键点：立即加载并释放文件锁
                        bitmap.EndInit();
                        bitmap.Freeze(); // 线程安全
                        return bitmap;
                    }
                    catch { return null; }
                }
                return null;
            }
        }


        public bool IsImage { get; set; }

        public string SizeText => $"{Size / 1024.0:F2} KB";
    }
}
