using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class VideoPlayWithPotPlayer : IVideoPlay
    {
        public void Play(string path)
        {
            // get player path
            string playerPath = GlobalSettingsService.Instance
                                .GetValue("playerPath");
            if (string.IsNullOrEmpty(playerPath)) throw new ArgumentException("VideoPlayer: wrong player path");

            // play
            System.Diagnostics.Process.Start(playerPath,path);
        }
    }
}
