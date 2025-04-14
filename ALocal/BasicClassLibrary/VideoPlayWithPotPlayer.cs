using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class VideoPlayWithPotPlayer : IVideoPlay
    {
        public void Play(string videoPath, string playerPath)
        {
            System.Diagnostics.Process.Start(playerPath, playerPath);
        }
    }
}
