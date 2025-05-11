using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalAniHubFront.Models
{
    public class EntryLine
    {
        public string LineText { get; set; } // 示例: "原名 (译名) - 上映时间"
        public int Id { get; set; } // 条目 ID
    }
}
