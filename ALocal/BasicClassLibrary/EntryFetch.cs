using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class EntryFetch
    {
        public EntryFetch()
        {
            int sourceType;
            try
            {
                sourceType = int.Parse(GlobalSettingsService.Instance.GetValue("defaultEntryFetchSource"));
            }
            catch (Exception)
            {
                throw new ArgumentException("EntryFetch: wrong sourceType");
            }

            int year = DateTime.Now.Year;//默认当前年份
            int month = GetDefaultMonth();//默认月份
            InitializeEntryFetch(sourceType, year, month);
        }

        //  新增构造函数，允许传递 sourceType、year 和 month
        public EntryFetch(int sourceType, int year, int month)
        {
            InitializeEntryFetch(sourceType, year, month);
        }

        // 初始化 entryFetch 的逻辑
        private void InitializeEntryFetch(int sourceType, int year, int month)
        {
            if (!IsValidMonth(month))
            {
                throw new ArgumentException("仅支持1月、4月、7月和10月作为有效月份。");
            }

            switch (sourceType)
            {
                case 0:
                    entryFetch = new EntryFetchInYucWiki(year, month);
                    break;
                default:
                    entryFetch = new EntryFetchInYucWiki(year, month);
                    break;
            }
        }

        // 验证月份是否有效
        private static bool IsValidMonth(int month)
        {
            return month == 1 || month == 4 || month == 7 || month == 10;
        }


        // 获取默认月份（例如，当前季度的第一个月）
        private static int GetDefaultMonth()
        {
            int currentMonth = DateTime.Now.Month;
            if (currentMonth <= 3) return 1;
            if (currentMonth <= 6) return 4;
            if (currentMonth <= 9) return 7;
            return 10;
        }


        private IEntryFetch entryFetch;

        public async Task<List<EntryInfoSet>> FetchAsync()
        {
            return await entryFetch.FetchAsync();
        }
    }
}
