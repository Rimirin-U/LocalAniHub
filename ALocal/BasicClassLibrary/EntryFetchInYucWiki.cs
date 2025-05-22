using HtmlAgilityPack; // 需要安装HtmlAgilityPack NuGet包
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Drawing;
using Image = System.Drawing.Image;
using System.Text.RegularExpressions;

namespace BasicClassLibrary
{
   

    public class EntryFetchInYucWiki : IEntryFetch
    {

        private readonly HttpClient _httpClient;//HttpClient 是 .NET 中用于发送 HTTP 请求和接收 HTTP 响应的核心类
        private readonly int _year;
       
        public EntryFetchInYucWiki(int year,int month)
        {
            if (!IsValidMonth(month))
            {
                throw new ArgumentException("仅支持1月、4月、7月和10月作为有效月份。");
            }
            _year = year;
            string baseUrl = GenerateBaseUrl(year, month);
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }


        private static bool IsValidMonth(int month)
        {
            // 仅支持1月、4月、7月和10月
            return month == 1 || month == 4 || month == 7 || month == 10;
        }
        private static string GenerateBaseUrl(int year, int month)
        {
            // 动态生成 BaseUrl
            return $"https://yuc.wiki/{year}{month.ToString("D2", CultureInfo.InvariantCulture)}/";
        }

        // 新增：下载图片为Image对象的工具方法
        private async Task<Image?> DownloadImageAsImageAsync(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;
            try
            {
                //使用注入的HttpClient实例（_httpClient）的GetByteArrayAsync方法异步发送 HTTP 请求，
                //并将响应内容作为字节数组返回。
                byte[] imageBytes = await _httpClient.GetByteArrayAsync(url);
                //将下载的字节数组包装在MemoryStream中，以便Image类可以读取。
                //使用using语句确保流资源在使用后被正确释放，避免内存泄漏。
                using MemoryStream ms = new MemoryStream(imageBytes);
                return Image.FromStream(ms);
            }
            catch
            {
                return null;
            }
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

                if (entryNodes == null)
                {
                    throw new EntrySearchException("yuc.wiki", "未找到任何条目节点");
                }
               
                    //对每一个条目节点进行遍历
                    foreach (var entryNode in entryNodes)
                    {
                        // 5. 提取基本信息
                        //使用 XPath 表达式选取当前条目节点的下一个兄弟div元素内的table元素。
                        var table = entryNode.SelectSingleNode("following-sibling::div[1]/table");
                        if (table == null) continue;

                    // 译名：class="title_cn" + 其他
                    var translatedName = table.SelectSingleNode(".//p[starts-with(@class, 'title_cn')]")?.InnerText.Trim() ?? "";
                    // 原名：class="title_jp" + 其他
                    var originalName = table.SelectSingleNode(".//p[starts-with(@class, 'title_jp')]")?.InnerText.Trim() ?? "";
                    var category = table.SelectSingleNode(@"
                    .//td[
                    starts-with(@class, 'type_') 
                    and string-length(@class) >= 7
                    and contains(
                    '_a_b_c_d_e_f_g_h_i_j_k_l_m_n_o_p_q_r_s_t_u_v_w_x_y_z_', 
                    concat('_', substring(@class, 6, 1), '_')
                     )
                    ]")?.InnerText.Trim() ?? "";

                    // 6. 解析发布日期 (从broadcast_r)
                    DateTime? releaseDate = null;
                        //选取table元素内class为broadcast_r的p元素，并获取其去除首尾空格后的文本内容。Trim方法用于去除字符串首尾的空白字符（像空格、制表符、换行符等）
                        var broadcastText = table.SelectSingleNode(".//p[@class='broadcast_r']")?.InnerText.Trim();
                        if (!string.IsNullOrEmpty(broadcastText))
                        {
                            // 匹配如 4/12 或 4/12周六深夜 这样的格式
                            var match = System.Text.RegularExpressions.Regex.Match(broadcastText, @"(\d{1,2})/(\d{1,2})");
                            if (match.Success)
                            {
                                int month = int.Parse(match.Groups[1].Value);
                                int day = int.Parse(match.Groups[2].Value);
                                // 用构造函数参数 year
                                releaseDate = new DateTime(_year, month, day);
                            }

                           // 7. 解析制作人员和声优信息
                           // 支持 staff_r 和 staff_r1 两种 class
                           var staffNode = table.SelectSingleNode(".//td[@class='staff_r']") ?? table.SelectSingleNode(".//td[@class='staff_r1']");
                           var staffText = staffNode?.InnerText.Trim();
                           var castText = table.SelectSingleNode(".//td[@class='cast_r']")?.InnerText.Trim();

                        // 7.1 解析制作人员为Key-Value对
                        var staffDict = new Dictionary<string, string>();
                        if (!string.IsNullOrEmpty(staffText))
                        {
                            // 先按 <br> 或换行分割
                            var staffLines = staffText
                                .Replace("<br>", "\n", StringComparison.OrdinalIgnoreCase)
                                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                            string? lastKey = null;
                            foreach (var line in staffLines)
                            {
                                var trimmed = line.Trim();
                                if (string.IsNullOrEmpty(trimmed)) continue;
                                var parts = trimmed.Split(new[] { '：', ':' }, 2);
                                if (parts.Length == 2)
                                {
                                    // 新的职位
                                    lastKey = parts[0].Trim();
                                    staffDict[lastKey] = string.Join(" ", parts[1].Trim().Split(new[] { ' ', '　' }, StringSplitOptions.RemoveEmptyEntries));
                                }
                                else if (lastKey != null)
                                {
                                    // 没有冒号，且有上一个职位，视为追加到上一个职位
                                    staffDict[lastKey] += " " + trimmed;
                                }
                
                            }
                        }

                        // 解析声优 cast，处理带空格和<br>分隔的名字
                        string castValue = "";
                        if (!string.IsNullOrWhiteSpace(castText))
                        {
                            // 1. 替换<br>为空格（而不是换行）
                            // 2. 合并所有空白字符为单个空格
                            castValue = Regex.Replace(
                                castText
                                    .Replace("<br>", " ", StringComparison.OrdinalIgnoreCase) // 先替换<br>为空格
                                    .Replace("\n", " ")  // 替换换行为空格
                                    .Replace("\r", " "), // 替换回车为空格
                                @"\s+", " ")            // 合并所有空白字符
                                .Trim();
                        }


                        //// 8. 解析链接
                        //var links = table.SelectNodes(".//td[@class='link_a_r']/a")?
                        //        .ToDictionary(
                        //            a => a.InnerText.Trim(),
                        //            a => a.GetAttributeValue("href", "")
                        //        ) ?? new Dictionary<string, string>();

                            // 获取图片URL
                            string? coverUrl = entryNode.SelectSingleNode(".//img")?.GetAttributeValue("data-src", "");
                            // 下载图片为Image对象
                            Image? keyVisualImage = await DownloadImageAsImageAsync(coverUrl);

                        // 类型标签：class="type_tag" + 其他
                        var typeTag = table.SelectSingleNode(".//td[starts-with(@class, 'type_tag')]")?.InnerText.Trim() ?? "";

                        // 9. 创建条目
                        var metadata = new Dictionary<string, string>
                        {
                            ["类型标签"] = typeTag??"",
                            ["声优"] = castValue ?? "",
                            ["集数信息"] = table.SelectSingleNode(".//p[@class='broadcast_ex_r']")?.InnerText.Trim() ?? "",
                            //["相关链接"] = string.Join(" | ", links.Select(kv => $"{kv.Key}:{kv.Value}"))
                        };
                        // 插入所有制作人员项
                        foreach (var kv in staffDict)
                        {
                            metadata[$"{kv.Key}"] = kv.Value;
                        }
                        var entry = new EntryInfoSet(
                                translatedName: translatedName,
                                originalName: originalName,
                                releaseDate: releaseDate ?? DateTime.MinValue,
                                category: category,
                                keyVisualImage: keyVisualImage,
                                metadata:metadata
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