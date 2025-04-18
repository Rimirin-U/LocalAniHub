using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class BtDownload
    {
        public BtDownload()
        {
            try
            {
                downloadPath = GlobalSettingsService.Instance.GetValue("downloadPath");
            }
            catch (Exception)
            {
                throw new ArgumentException("BtDownload: wrong downloadPath");
            }
        }

        private readonly string downloadPath;

        public void Download(string url)
        {
            throw new NotImplementedException();
        }
    }
}
