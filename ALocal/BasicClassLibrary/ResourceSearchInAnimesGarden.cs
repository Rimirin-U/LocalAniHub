using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BasicClassLibrary
{
    public class ResourceSearchInAnimesGarden : IResourceSearch
    {
        private readonly HttpClient _httpClient;// 用于发送 HTTP 请求的客户端（不可变字段）
        public ResourceSearchInAnimesGarden()
        {
            _httpClient = new HttpClient();// 构造函数中创建新的 HttpClient 实例
        }
        public async Task<List<ResourceItem>> SearchAsync(string keyword, CancellationToken ct = default)
        {
            try
            {
                // 对关键词和固定类型 "动画" 进行 URL 编码
                var encodedKeyword = Uri.EscapeDataString(keyword);
                var encodedType = Uri.EscapeDataString("动画");

                // 构造 API URL
                var url = $"https://api.animes.garden/feed.xml?include={encodedKeyword}&type={encodedType}";
                //Console.WriteLine($"请求的URL: {url}");

                // 设置自定义 User-Agent
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");//模拟浏览器行为，避免被服务器拒绝请求

                // 发送 GET 请求
                var response = await _httpClient.GetAsync(url, ct);
                Console.WriteLine($"响应状态码: {(int)response.StatusCode} {response.ReasonPhrase}");

                // 检查响应状态码
                response.EnsureSuccessStatusCode();

                // 使用流式处理优化内存
                using var responseStream = await response.Content.ReadAsStreamAsync(ct);

                // 异步解析 XML
                var xdoc = await XDocument.LoadAsync(responseStream, LoadOptions.None, ct);

                return xdoc.Descendants("item")
                    .Select(item => new ResourceItem
                    {
                        Title = SanitizeString(item.Element("title")?.Value),
                        PubDate = ParseDate(item.Element("pubDate")?.Value),
                        DownloadUrl = SanitizeString(item.Element("enclosure")?.Attribute("url")?.Value)
                    })
                    .ToList();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Console.WriteLine("服务器拒绝了请求。请检查 API 的访问权限或关键词是否正确。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"搜索发生错误: {ex.Message}");
            }

            return new List<ResourceItem>();
        }
        private static DateTime? ParseDate(string? value) // 修改参数类型为string?
        {
            // 显式处理null和空白值
            if (string.IsNullOrWhiteSpace(value)) return null;

            // 尝试按RFC 822格式（"r"格式说明符）解析日期
            return DateTime.TryParseExact(
             value, // 此时value已被验证为非空
             "r",
             CultureInfo.InvariantCulture,
             DateTimeStyles.AdjustToUniversal,
             out var dt)
             ? dt
             : null;
        }
        public static string SanitizeString(string? value)
        {
            return value?.Trim() ?? string.Empty; // 显式处理null
        }
    }
}
   

  /*  public class ResourceSearchInAnimesGarden : IResourceSearch
    {

        private readonly HttpClient _httpClient;// 用于发送 HTTP 请求的客户端（不可变字段）
        public ResourceSearchInAnimesGarden()
        {
            _httpClient = new HttpClient();// 构造函数中创建新的 HttpClient 实例
        }
        
        public async Task<List<ResourceItem>> SearchAsync(string keyword, CancellationToken ct = default)
        // 搜索关键词（必填） CancellationToken ct = default // 取消令牌（默认无，可以先不管这个参数）
        {
            try
            {
                // 对关键词和固定类型 "动画" 进行 URL 编码
                var encodedKeyword = Uri.EscapeDataString(keyword);
                var encodedType = Uri.EscapeDataString("动画");
                // 硬编码 API 地址，拼接查询参数
                var url = $"https://api.animes.garden/feed.xml?include={encodedKeyword}&type={encodedType}";

                // 使用流式处理优化内存
                // 异步获取 HTTP 响应流（使用 using 自动释放流资源）
                using var responseStream = await _httpClient.GetStreamAsync(url, ct);

                // 异步解析XML
                var xdoc = await XDocument.LoadAsync(responseStream, LoadOptions.None, ct);

                return xdoc.Descendants("item")
                    .Select(item => new ResourceItem
                    {

                        // 清理字符串（去除前后空格，处理null）
                        Title = SanitizeString(item.Element("title")?.Value),
                        // 解析日期（处理RFC 822格式，如"Thu, 01 Jan 2020 12:00:00 GMT"）
                        PubDate = ParseDate(item.Element("pubDate")?.Value),
                        // 提取enclosure标签的url属性
                        DownloadUrl= SanitizeString(item.Element("enclosure")?.Attribute("url")?.Value)
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"搜索发生错误: {ex.Message}");
                return new List<ResourceItem>();
            }
        }

        // 日期解析方法
        // 日期解析方法（允许输入参数为null）
        private static DateTime? ParseDate(string? value) // 修改参数类型为string?
        {
            // 显式处理null和空白值
            if (string.IsNullOrWhiteSpace(value)) return null;

            // 尝试按RFC 822格式（"r"格式说明符）解析日期
            return DateTime.TryParseExact(
                value, // 此时value已被验证为非空
                "r",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal,
                out var dt)
                ? dt
                : null;
        }
        public static string SanitizeString(string? value)
        {
            return value?.Trim() ?? string.Empty; // 显式处理null
        }

        public List<SyndicationFeed> Search(string keyWord)
        {
            throw new NotImplementedException();
        }
    }*/


