using System.IO;

namespace RazorViewTemplateEngine.Core.Internal {
    public class PhysicalFileDescriptor : AbstractFileDescriptor {
        public PhysicalFileDescriptor(string relativeFilePath, string content = null) : base(relativeFilePath, content) { }
        public override Stream Read() {
            throw new System.NotImplementedException();
        }
    }
}