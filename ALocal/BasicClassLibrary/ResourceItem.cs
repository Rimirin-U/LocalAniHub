using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class ResourceItem
    {
        //init:仅在构造时或初始化器中赋初值
        public required string Title { get; init; }//标题
        public DateTime? PubDate { get; init; }  //发布日期
        public required string DownloadUrl { get; init; }//下载URL资源-媒体资源链接（最重要）
    }
}
