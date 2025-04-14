using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//后续合并后需要删去的部分
public class ResourceSearch;
public class ResourceDownloader;
public class SyndicationFeed;



namespace BasicClassLibrary
{
    public class ResourceFetcher
    {
        private readonly ResourceSearch _search;
        private readonly ResourceDownloader _downloader;
        public ResourceFetcher(ResourceSearch search, ResourceDownloader downloader)
        {
            _search = search;
            _downloader = downloader;
        } 
        public void FetchResource(string keyword, string destinationPath)
        {
            // 第一步：通过资源搜索模块获取 SyndicationFeed 列表
            List<SyndicationFeed> feeds = _search.SearchResources(keyword);//类中搜索资源函数的调用
            // 第二步：遍历 SyndicationFeed 列表并下载资源
            foreach (var feed in feeds)
            {
                _downloader.DownloadResource(feed, destinationPath);
            }
        }
    }
}
