using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public interface IVideoPlay
    {
        public void Play(string videoPath, string playerPath);
    }
}
