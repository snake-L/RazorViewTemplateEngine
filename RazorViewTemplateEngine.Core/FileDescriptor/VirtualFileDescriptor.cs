using System.IO;
using System.Text;

namespace RazorViewTemplateEngine.Core.FileDescriptor {
    public class VirtualFileDescriptor : AbstractFileDescriptor {
        
        private readonly MemoryStream _stream;
        public override bool IsPhysical { get; } = false;

        public VirtualFileDescriptor(string relativeFilePath, string content = null) :
            base(relativeFilePath, content) {
            _stream = new MemoryStream(Encoding.UTF8.GetBytes(Content));
        }
        public override Stream Read() {
            return _stream;
        }
        public override void Dispose() {
            _stream?.Dispose();
        }
    }
}