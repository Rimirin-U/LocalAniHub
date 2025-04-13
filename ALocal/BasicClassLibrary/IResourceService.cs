using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public interface IResourceService
    {
        // 将现有资源加入资源管理，作为指定作品的指定集数
        Task AddResourceAsync(int entryId, int episodeNumber, string filePath, bool hasSubtitle);
        // 下载指定资源为指定作品的指定集数
        Task DownloadResourceAsync(int entryId, int episodeNumber, string downloadUrl, string savePath);
        // 按照设定（如按时间、按大小）清除资源
        Task ClearResourcesAsync(Func<Resource, bool> condition);
    }
}
