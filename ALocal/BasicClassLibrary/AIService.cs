using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace BasicClassLibrary
{
    // API 响应数据结构（与接口文档对应）
    public class ApiResponse
    {
        public int Code { get; set; }    // 响应代码 0=成功
        public string Msg { get; set; } // 消息提示
        public string Data { get; set; }// 实际数据内容
    }

    /// <summary>
    /// 音频转换核心处理器
    /// </summary>
    public class AudioConverter : IDisposable
    {
        // 预定义合法参数值（根据接口文档定义）
        private static readonly HashSet<string> ValidLanguages = new HashSet<string>
            { "zh", "en", "fr", "de", "ja", "ko", "ru", "es", "th", "it", "pt", "vi", "ar", "tr" };

        private static readonly HashSet<string> ValidModels = new HashSet<string>
            { "base", "small", "medium", "large-v3" };

        private readonly HttpClient _httpClient;

        /// <summary>
        /// 构造函数初始化HttpClient
        /// </summary>
        public AudioConverter()
        {
            _httpClient = new HttpClient
            {
                // 设置与Python示例相同的600秒超时
                Timeout = TimeSpan.FromSeconds(600)
            };
        }

        /// <summary>
        /// 执行音频转换的主方法
        /// </summary>
        /// <param name="filePath">音视频文件路径</param>
        /// <param name="language">语言代码</param>
        /// <param name="model">模型名称</param>
        /// <param name="responseFormat">响应格式</param>
        /// <returns>识别结果的保存路径</returns>
        public async Task<string> ConvertAsync(
            string filePath,
            string language = "zh",
            string model = "base",
            string responseFormat = "text")
        {
            // 参数验证
            ValidateInput(filePath, language, model, responseFormat);

            // 构建请求内容
            using var formData = new MultipartFormDataContent();
            await AddFileContent(formData, filePath);
            AddTextParameters(formData, language, model, responseFormat);

            // 发送请求
            var response = await _httpClient.PostAsync("http://127.0.0.1:9977/api", formData);

            // 处理响应
            var result = await ParseResponse(response);
            return SaveResult(result.Data, responseFormat);
        }

        #region 私有方法
        /// <summary>
        /// 验证所有输入参数的合法性
        /// </summary>
        private void ValidateInput(
            string filePath,
            string language,
            string model,
            string responseFormat)
        {
            // 文件存在性检查
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"文件不存在: {filePath}");

            // 语言代码验证
            if (!ValidLanguages.Contains(language))
                throw new ArgumentException($"无效语言代码: {language}");

            // 模型验证
            if (!ValidModels.Contains(model))
                throw new ArgumentException($"无效模型名称: {model}");

            // 响应格式验证
            if (!new[] { "text", "json", "srt" }.Contains(responseFormat))
                throw new ArgumentException($"无效响应格式: {responseFormat}");
        }

        /// <summary>
        /// 添加文件内容到请求体
        /// </summary>
        private async Task AddFileContent(MultipartFormDataContent formData, string filePath)
        {
            try
            {
                // 使用FileStream自动处理文件锁定
                using var fileStream = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);

                var fileContent = new StreamContent(fileStream);
                formData.Add(fileContent, "file", Path.GetFileName(filePath));
            }
            catch (IOException ex)
            {
                throw new IOException($"文件读取失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 添加文本参数到请求体
        /// </summary>
        private void AddTextParameters(
            MultipartFormDataContent formData,
            string language,
            string model,
            string responseFormat)
        {
            formData.Add(new StringContent(language), "language");
            formData.Add(new StringContent(model), "model");
            formData.Add(new StringContent(responseFormat), "response_format");
        }

        /// <summary>
        /// 解析API响应
        /// </summary>
        private async Task<ApiResponse> ParseResponse(HttpResponseMessage response)
        {
            // 读取响应内容
            var content = await response.Content.ReadAsStringAsync();

            // 强制检查HTTP状态码
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"HTTP请求失败: {response.StatusCode} - {response.ReasonPhrase}");
            }

            try
            {
                // 反序列化JSON
                var result = JsonSerializer.Deserialize<ApiResponse>(content);

                // 检查业务状态码
                if (result.Code != 0)
                {
                    throw new InvalidOperationException(
                        $"API业务错误: [{result.Code}] {result.Msg}");
                }

                return result;
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("响应JSON解析失败", ex);
            }
        }

        /// <summary>
        /// 保存结果到文件
        /// </summary>
        private string SaveResult(string data, string format)
        {
            var extension = format switch
            {
                "json" => ".json",
                "srt" => ".srt",
                _ => ".txt"
            };

            var outputPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                $"result_{DateTime.Now:yyyyMMddHHmmss}{extension}");

            File.WriteAllText(outputPath, data);
            return outputPath;
        }

        /// <summary>
        /// 释放HttpClient资源
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
        #endregion

        // 使用示例
        //class Program
        //{
        //    static async Task Main(string[] args)
        //    {
        //        try
        //        {
        //            // 示例参数（实际应从配置或命令行获取）
        //            var filePath = @"C:\audio\test.wav";
        //            var language = "zh";
        //            var model = "large-v3";
        //            var format = "srt";

        //            using var converter = new AudioConverter();
        //            var outputPath = await converter.ConvertAsync(filePath, language, model, format);

        //            // 此处可添加其他处理逻辑
        //        }
        //        catch (Exception ex)
        //        {
        //            // 集中处理所有异常
        //            Environment.ExitCode = 1; // 设置非零退出码
        //            throw; // 抛出异常供上层捕获
        //        }
        //    }
        //}
    }
}
