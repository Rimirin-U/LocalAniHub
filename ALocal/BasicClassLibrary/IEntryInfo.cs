using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//条目信息集合
namespace BasicClassLibrary
{
    public interface IEntryInfo
    {
        string TranslatedName { get; }//译名
        string OriginalName { get; }//原名
        DateTime ReleaseDate { get; }//首播/上映日期
        string Category { get; }//类别

        //系列键值对，存储从网络上得到的其他信息
        IReadOnlyDictionary<string, string> Metadata { get; }

        //条目关联接口
        int? EntryId { get; }//外键
        Entry? Entry { get; }//导航属性
    }
}
