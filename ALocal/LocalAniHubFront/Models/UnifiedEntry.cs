using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalAniHubFront.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;

    //在对象的属性值发生改变时通知绑定的客户端
    public class UnifiedEntry : INotifyPropertyChanged
    {
        //用于存储对象的属性，键是属性名（字符串类型），值是属性值（对象类型）
        private readonly Dictionary<string, object> _properties = new();

        //属性变更事件，用于通知 UI 更新
        public event PropertyChangedEventHandler? PropertyChanged;

        //定义了一个索引器，允许通过属性名作为索引来访问和设置对象的属性值
        public object this[string propertyName]
        {
            //如果 _properties 字典中包含指定的属性名，则返回该属性的值；否则返回 null。
            get => _properties.ContainsKey(propertyName) ? _properties[propertyName] : null;
            //如果 _properties 字典中已经包含指定的属性名，则更新该属性的值；
            //否则添加一个新的属性。
            //无论哪种情况，都会调用 OnPropertyChanged 方法触发 PropertyChanged 事件。
            set
            {
                if (_properties.ContainsKey(propertyName))
                {
                    _properties[propertyName] = value;
                    OnPropertyChanged(propertyName);
                }
                else
                {
                    _properties.Add(propertyName, value);
                    OnPropertyChanged(propertyName);
                }
            }
        }

        public void AddProperty(string propertyName, object value)
        {
            //实际上是调用了索引器的 set 访问器。
            this[propertyName] = value;
        }

        //定义了一个受保护的方法 OnPropertyChanged，用于触发 PropertyChanged 事件。
        //使用 ?. 空条件运算符确保在 PropertyChanged 事件有订阅者时才调用 Invoke 方法。
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
