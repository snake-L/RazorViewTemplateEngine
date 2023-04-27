using System.Text;
using System.Threading.Tasks;
using RazorViewTemplateEngine.Core.Interface;

namespace RazorViewTemplateEngine.Core
{
    /// <summary>
    /// 视图对象
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public abstract class RazorViewPage<T> : RazorViewPage, IRazorViewPage<T> {
        public new T Model { get; set; }
    }

    /// <summary>
    /// 视图对象
    /// </summary>
    public abstract class RazorViewPage : IRazorViewPage {
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private string _attributeSuffix = null;

        public dynamic Model { get; set; }

        public void WriteLiteral(string literal = null)
        {
            WriteLiteralAsync(literal).GetAwaiter().GetResult();
        }

        public virtual Task WriteLiteralAsync(string literal = null)
        {
            _stringBuilder.Append(literal);
            return Task.CompletedTask;
        }

        public void Write(object obj = null)
        {
            WriteAsync(obj).GetAwaiter().GetResult();
        }

        public virtual Task WriteAsync(object obj = null)
        {
            _stringBuilder.Append(obj);
            return Task.CompletedTask;
        }

        public void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset,
            int attributeValuesCount)
        {
            BeginWriteAttributeAsync(name, prefix, prefixOffset, suffix, suffixOffset, attributeValuesCount).GetAwaiter().GetResult();
        }

        public virtual Task BeginWriteAttributeAsync(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
        {
            _attributeSuffix = suffix;
            _stringBuilder.Append(prefix);
            return Task.CompletedTask;
        }

        public void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength,
            bool isLiteral)
        {
            WriteAttributeValueAsync(prefix, prefixOffset, value, valueOffset, valueLength, isLiteral).GetAwaiter().GetResult();
        }

        public virtual Task WriteAttributeValueAsync(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            _stringBuilder.Append(prefix);
            _stringBuilder.Append(value);
            return Task.CompletedTask;
        }

        public void EndWriteAttribute()
        {
            EndWriteAttributeAsync().GetAwaiter().GetResult();
        }

        public virtual Task EndWriteAttributeAsync()
        {
            _stringBuilder.Append(_attributeSuffix);
            _attributeSuffix = null;
            return Task.CompletedTask;
        }

        public void Execute()
        {
            ExecuteAsync().GetAwaiter().GetResult();
        }

        public virtual Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }

        public virtual string Result()
        {
            return ResultAsync().GetAwaiter().GetResult();
        }

        public virtual Task<string> ResultAsync()
        {
            return Task.FromResult<string>(_stringBuilder.ToString());
        }
        
    }
}
