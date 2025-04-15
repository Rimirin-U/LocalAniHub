using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace BasicClassLibrary
{
    public sealed class GlobalSettingsService//sealed防止被继承
    {
        // 借助Lazy<T>实现延迟加载的单例实例
        //Lazy<T> 能够确保实例是线程安全的，并且在首次使用时才会被创建
        private static readonly Lazy<GlobalSettingsService> _instance =
            new Lazy<GlobalSettingsService>(() => new GlobalSettingsService());
        // 全局访问点（唯一入口）
        public static GlobalSettingsService Instance => _instance.Value;

        // 线程安全锁，避免多个线程同时访问和修改设置。
        private readonly object _lock = new object();

        // 设置项存储
        private readonly Dictionary<string, string> _settings;

        // 数据库连接字符串，定义 SQLite 数据库的连接字符串，指定数据库文件名为 settings.db。
        private const string ConnectionString = "Data Source=settings.db";

        // 硬编码的默认设置项
        private readonly Dictionary<string, string> _defaultSettings = new Dictionary<string, string>
        {
            // 视频播放相关设置
            {"defaultPlayerType", "0"},      // 0: PotPlayer（默认值）
            {"defaultPlayerPath", ""},       // 路径（默认值：空）
       
            // 条目拉取相关设置
            {"defaultEntryFetchSource", "0"}, // 0: 自yuc.wiki（默认值）
            
            // 资源管理相关设置
            {"defaultparentFolderPath", ""},  // 路径（默认值：还没想好）
            
            // 下载相关设置
            //在代码中通过该键名（如 GetValue("downloadPath")）获取或设置对应的路径值。
            {"downloadPath", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)} // 默认值：MyVideos文件夹
        };

        // 私有构造函数：禁止外部 new 创建实例
        private GlobalSettingsService()
        {
            _settings = new Dictionary<string, string>();
            InitializeDatabase();//初始化数据库
            LoadSettings();//从数据库加载设置
        }

        // 初始化数据库
        private void InitializeDatabase()
        {
            // 使用锁来确保线程安全，防止多个线程同时初始化数据库
            lock (_lock)
            {
                // 创建一个 SQLite 数据库连接对象，使用预先定义好的连接字符串
                using (var conn = new SqliteConnection(ConnectionString))
                {
                    // 打开数据库连接
                    conn.Open();
                    // 创建一个 SQL 命令对象，用于执行 SQL 语句
                    var command = conn.CreateCommand();
                   // 设置 SQL 命令的文本内容，这里使用的是一个多行字符串
                   //CREATE TABLE IF NOT EXISTS：这是一个 SQL 语句的标准语法，意思是如果指定的表不存在，就创建该表；如果已经存在，则不进行任何操作。
                   //Settings：是要创建的表的名称。
                   //Key TEXT PRIMARY KEY：定义了一个名为 Key 的列，数据类型为 TEXT，并将其设置为主键。主键是表中唯一标识每一行的字段，不允许有重复值。
                   //Value TEXT：定义了一个名为 Value 的列，数据类型为 TEXT，用于存储设置项的值。
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Settings (
                            Key TEXT PRIMARY KEY,
                            Value TEXT
                        )";
                    // 执行SQL命令，ExecuteNonQuery方法通常用于执行不返回结果集的SQL语句，
                    // 如 INSERT、UPDATE、DELETE 以及创建表的语句（此处执行创建表的语句）
                    command.ExecuteNonQuery();
                }
            }
        }

        //从数据库中加载设置信息到内存中的 _settings 字典里
        private void LoadSettings()
        {
            lock (_lock)
            {
                // 首先加载默认值
                foreach (var setting in _defaultSettings)
                {
                    //将默认设置项的键值对添加到 _settings 字典中
                    _settings[setting.Key] = setting.Value;
                }

                // 然后从数据库覆盖
                using (var conn = new SqliteConnection(ConnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = "SELECT Key, Value FROM Settings";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //将从数据库中读取到的 Key 和 Value 存储到 _settings 字典中
                            //reader.GetString(0)获取当前行的第一个字段（即 Key），reader.GetString(1)获取当前行的第二个字段（即 Value）
                            //如果 _settings 字典中已经存在该 Key，则会用数据库中的值覆盖之前设置的默认值
                            _settings[reader.GetString(0)] = reader.GetString(1);
                        }
                    }
                }
            }
        }

        //从 _settings 字典中获取指定键对应的值
        public string GetValue(string key)
        {
            lock (_lock)
            {
                if (_settings.TryGetValue(key, out var value))
                {
                    return value;
                }
                throw new KeyNotFoundException($"设置项 {key} 不存在");
            }
        }

        // 设置值
        public void SetValue(string key, string value)
        {
            lock (_lock)
            {
                if (!_defaultSettings.ContainsKey(key))
                    throw new KeyNotFoundException($"设置项 {key} 不是有效的配置项");

                _settings[key] = value;
                // 持久化到数据库
                using (var conn = new SqliteConnection(ConnectionString))
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = @"
                        INSERT OR REPLACE INTO Settings (Key, Value)
                        VALUES (@key, @value)";
                    command.Parameters.AddWithValue("@key", key);
                    command.Parameters.AddWithValue("@value", value);
                    command.ExecuteNonQuery();
                }
            }
        }

        // 获取所有设置项的快照
        public Dictionary<string, string> GetAllSettings()
        {
            lock (_lock)
            {
                return new Dictionary<string, string>(_settings);
            }
        }
    }
}