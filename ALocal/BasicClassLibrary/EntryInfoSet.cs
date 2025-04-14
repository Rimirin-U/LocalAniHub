using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class EntryInfoSet
    {
        public string TranslatedName { get; }//译名
        public string OriginalName { get; }//原名
        public DateTime ReleaseDate { get; }//首播/上映日期
        public string Category { get; }//类别
        //系列键值对，存储从网络上得到的其他信息
        public IReadOnlyDictionary<string, string> Metadata { get; }
        public EntryInfoSet(string translatedName, string originalName,
            DateTime releaseDate, string category, IReadOnlyDictionary<string, string> metadata)
        {
            TranslatedName = translatedName;
            OriginalName = originalName;
            ReleaseDate = releaseDate;
            Category = category;
            Metadata = metadata;
        }
    }
}
