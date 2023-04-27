using System.IO;
using System.Text;
using Microsoft.Extensions.Primitives;
using RazorViewTemplateEngine.Core.Interface;

namespace RazorViewTemplateEngine.Core.Internal {
    internal class VirtualFileDescriptor : AbstractFileDescriptor {
        public override bool IsPhysical { get; } = false;
        public VirtualFileDescriptor(string relativeFilePath, string content = null) : base(relativeFilePath, content) { }
        public override Stream Read() {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(Content));
            return stream;
        }
    }
}