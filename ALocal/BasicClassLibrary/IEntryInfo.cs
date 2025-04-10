using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public interface IEntryInfo
    {
        public int Id { get; set; }         // 内键
        public int EntryId { get; set; }    // 外键（唯一）
        public Entry Entry { get; set; }    // 导航属性
    }
}
