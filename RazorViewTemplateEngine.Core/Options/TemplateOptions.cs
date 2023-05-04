using System;
using System.Collections.Generic;

namespace RazorViewTemplateEngine.Core.Options {
    public sealed class TemplateOptions {
        /// <summary>
        /// 物理文件夹路径
        /// </summary>
        public string PhysicalDirectoryPath { get; set; }

        private string _fileWildcard = "*";

        /// <summary>
        /// 文件通配符 * 代表所有文件
        /// </summary>
        public string FileWildcard {
            get => _fileWildcard;
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value), "FileWildcard can not be null or empty");
                _fileWildcard = value;
            }
        }

        /// <summary>
        /// 模板字符串集合
        /// </summary>
        public List<TemplateContentCollection> TemplateStringCollections { get; set; } =
            new List<TemplateContentCollection>();
    }
    public class  TemplateContentCollection {
        /// <summary>
        /// 虚拟路径，用于标识模板
        /// </summary>
        public string VirtualPath { get; set; }
        /// <summary>
        /// 模板内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 继承类型名称
        /// <remarks T="T为当前项目所模板对应的实体类类型">
        /// 如果为empty，那么生成的C#代码则继承RazorViewPage，Model为dynamic类型.
        /// 如果填写了继承类型名称，那么生成的C#代码则继承RazorViewPage<T>,Model为T类型.
        /// </remarks>
        /// </summary>
        public Type InheritanceType { get; set; } = null;

    }
}