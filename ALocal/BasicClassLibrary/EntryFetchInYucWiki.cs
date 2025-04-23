using HtmlAgilityPack; // 需要安装HtmlAgilityPack NuGet包
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
   

    public class EntryFetchInYucWiki : IEntryFetch
    {
        private const string BaseUrl = "https://yuc.wiki/202504/";//存储目标网站的地址
        private readonly HttpClient _httpClient;//HttpClient 是 .NET 中用于发送 HTTP 请求和接收 HTTP 响应的核心类

        public EntryFetchInYucWiki()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        public async Task<List<EntryInfoSet>> FetchAsync()
        {
            try
            {
                // 1. 获取HTML页面
                HttpResponseMessage response = await _httpClient.GetAsync("");  //调用 GetAsync 发送GET请求
                response.EnsureSuccessStatusCode();
                string htmlContent = await response.Content.ReadAsStringAsync();//检查响应的状态码，若状态码不是成功状态（例如 200），则会抛出HttpRequestException异常。

                // 2. 使用HtmlAgilityPack解析HTML
                var htmlDoc = new HtmlDocument();//创建一个HtmlDocument对象，该对象来自HtmlAgilityPack库，用于解析 HTML 内容。
                htmlDoc.LoadHtml(htmlContent);//把之前获取的 HTML 内容加载到HtmlDocument对象中

                // 3. 创建返回列表
                var entries = new List<EntryInfoSet>();

                // 4. 查找所有条目容器 (以float:left样式的div开始)
                //运用 XPath 表达式选取所有style属性包含float:left的div元素，这些元素被当作条目容器。
                var entryNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@style, 'float:left')]");

                if (entryNodes != null)
                {
                    //对每一个条目节点进行遍历
                    foreach (var entryNode in entryNodes)
                    {
                        // 5. 提取基本信息
                        //使用 XPath 表达式选取当前条目节点的下一个兄弟div元素内的table元素。
                        var table = entryNode.SelectSingleNode("following-sibling::div[1]/table");
                        if (table == null) continue;

                        // 6. 解析发布日期 (从broadcast_r)
                        DateTime? releaseDate = null;
                        //选取table元素内class为broadcast_r的p元素，并获取其去除首尾空格后的文本内容。Trim方法用于去除字符串首尾的空白字符（像空格、制表符、换行符等）
                        var broadcastText = table.SelectSingleNode(".//p[@class='broadcast_r']")?.InnerText.Trim();
                        if (!string.IsNullOrEmpty(broadcastText))
                        {
                            // 简单解析示例，实际可能需要更复杂的日期解析逻辑
                            if (broadcastText.Contains("4/8"))
                            {
                                releaseDate = new DateTime(2025, 4, 8); // 假设是2025年
                            }
                        }

                        // 7. 解析制作人员和声优信息
                        var staffText = table.SelectSingleNode(".//td[@class='staff_r']")?.InnerText.Trim();
                        var castText = table.SelectSingleNode(".//td[@class='cast_r']")?.InnerText.Trim();

                        // 8. 解析链接
                        var links = table.SelectNodes(".//td[@class='link_a_r']/a")?
                            .ToDictionary(
                                a => a.InnerText.Trim(),
                                a => a.GetAttributeValue("href", "")
                            ) ?? new Dictionary<string, string>();

                        // 9. 创建条目
                        var entry = new EntryInfoSet(
                            translatedName: table.SelectSingleNode(".//p[@class='title_cn_r1']")?.InnerText.Trim() ?? "",
                            originalName: table.SelectSingleNode(".//p[@class='title_jp_r']")?.InnerText.Trim() ?? "",
                            releaseDate: releaseDate ?? DateTime.MinValue,
                            category: table.SelectSingleNode(".//td[@class='type_a_r']")?.InnerText.Trim() ?? "",
                            metadata: new Dictionary<string, string>
                            {
                                ["CoverUrl"] = entryNode.SelectSingleNode(".//img")?.GetAttributeValue("data-src", ""),
                                ["TypeTag"] = table.SelectSingleNode(".//td[@class='type_tag_r']")?.InnerText.Trim() ?? "",
                                ["Staff"] = staffText ?? "",
                                ["Cast"] = castText ?? "",
                                ["EpisodeInfo"] = table.SelectSingleNode(".//p[@class='broadcast_ex_r']")?.InnerText.Trim() ?? "",
                                ["Links"] = string.Join(" | ", links.Select(kv => $"{kv.Key}:{kv.Value}"))
                            }
                        );
                        entries.Add(entry);
                    }
                }

                return entries;
            }
            catch (Exception ex)
            {
                throw new EntrySearchException("yuc.wiki", $"获取条目失败: {ex.Message}");
            }
        }
    }

    public class EntrySearchException : Exception
    {
        public string Site { get; }

        public EntrySearchException(string site, string message)
            : base($"[{site}] {message}")
        {
            Site = site;
        }
    }
}