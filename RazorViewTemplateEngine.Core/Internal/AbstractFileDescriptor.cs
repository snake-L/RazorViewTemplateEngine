using System.IO;
using Microsoft.Extensions.Primitives;
using RazorViewTemplateEngine.Core.Interface;

namespace RazorViewTemplateEngine.Core.Internal {
    public abstract class AbstractFileDescriptor : IFileDescriptor {
        public virtual bool IsPhysical { get; } = true;
        public IChangeToken ChangeToken { get; }
        public string RelativeFilePath { get; set; }
        public string Content { get; set; }

        protected AbstractFileDescriptor(string relativeFilePath, string content = null) {
            RelativeFilePath = relativeFilePath;
            Content = content;
        }
        public abstract Stream Read();
    }
}