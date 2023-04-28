using System.IO;
using Microsoft.Extensions.Primitives;
using RazorViewTemplateEngine.Core.Interface;

namespace RazorViewTemplateEngine.Core.FileDescriptor {
    public abstract class AbstractFileDescriptor : IFileDescriptor {
        public virtual bool IsPhysical { get; } = true;
        public string RelativeFilePath { get; set; }
        public string Content { get; set; }
        protected AbstractFileDescriptor(string relativeFilePath = null, string content = null) {
            RelativeFilePath = relativeFilePath;
            Content = content;
        }
        public abstract Stream Read();
        public abstract void Dispose();
    }
}