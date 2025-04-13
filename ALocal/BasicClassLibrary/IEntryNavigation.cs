using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//来自branch4 条目关联接口
namespace BasicClassLibrary
{
    internal interface IEntryNavigation
    {
        public int? EntryId { get; set; }    // 外键
        public Entry? Entry { get; set; }    // 导航属性
    }
}
