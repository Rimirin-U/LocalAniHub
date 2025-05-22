using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class EntryRatingManager : Manager<EntryRating>
    {
        private readonly LogManager _logManager;
        public EntryRatingManager() : base(new AppDbContext())
        {
            _logManager = new LogManager(new AppDbContext());
        }

        public static readonly Func<int, Func<EntryRating, bool>> ByEntryId = (entryId => (o => o.EntryId == entryId));

        // 重写 Add 方法
        public override void Add(EntryRating entryRating)
        {
            if (entryRating == null)
            {
                throw new ArgumentNullException(nameof(entryRating));
            }

            // 调用基类的 Add 方法
            base.Add(entryRating);

            // 添加日志
            if (entryRating.EntryId.HasValue)
            {
                _logManager.AddRatingLog(entryRating.EntryId.Value, (int)(entryRating.Score ?? 0));
            }
        }
    }
}
