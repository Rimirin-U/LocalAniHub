using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    // 条目服务总接口
    public interface IEntryService
    {
        void AddEntryWithEpisodes(Entry entry, List<Episode> episodes); // 新增条目并初始化所有Episode对象
        IEntryFetcher Fetcher { get; }// 获取条目拉取模块
        IEntryManager Manager { get; }// 获取条目管理模块
        IEpisodeManager EpisodeManager { get; }// 获取单集管理模块
    }
}
