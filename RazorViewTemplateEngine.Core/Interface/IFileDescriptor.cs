using System;
using System.IO;
using Microsoft.Extensions.Primitives;

namespace RazorViewTemplateEngine.Core.Interface {
    public interface IFileDescriptor : IDisposable {
        /// <summary>
        /// 是否是物理文件
        /// </summary>
        bool IsPhysical { get; }
        /// <summary>
        /// 文件相对路径
        /// </summary>
        string RelativeFilePath { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        string Content { get; set; }
        /// <summary>
        /// 流数据
        /// </summary>
        Stream Read();
    }
}