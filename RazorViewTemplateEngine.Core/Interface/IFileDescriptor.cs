using System.IO;
using Microsoft.Extensions.Primitives;

namespace RazorViewTemplateEngine.Core.Interface {
    internal interface IFileDescriptor {
        /// <summary>
        /// 是否是物理文件
        /// </summary>
        bool IsPhysical { get; }
        //TODO: 物理文件监听变化,暂时先不实现
        /// <summary>
        /// 文件变化监听
        /// </summary>
        IChangeToken ChangeToken { get; }
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