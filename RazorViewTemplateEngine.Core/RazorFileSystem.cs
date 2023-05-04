using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.FileProviders;
using RazorViewTemplateEngine.Core.FileDescriptor;
using RazorViewTemplateEngine.Core.Interface;
using RazorViewTemplateEngine.Core.Options;

namespace RazorViewTemplateEngine.Core {
    public sealed class RazorFileSystem {
        private const string DEFAULT_TEMPLATE_ROOT_DIRECTORY = "/";
        private readonly DirectoryNode _root;
        public RazorFileSystem() {
            _root = new DirectoryNode(DEFAULT_TEMPLATE_ROOT_DIRECTORY);
        }
        public  IFileDescriptor GetItem(string path)
        {
            path = NormalizeAndEnsureValidPath(path);
            return _root.GetItem(path) ?? throw new DirectoryNotFoundException("Directory not found: " + path);
        }
        public void Add(IFileDescriptor fileDescriptor)
        {
            if (fileDescriptor == null)
            {
                throw new ArgumentNullException(nameof(fileDescriptor));
            }
            var filePath = NormalizeAndEnsureValidPath(fileDescriptor.RelativeFilePath);
            _root.AddFile(new FileNode(filePath, fileDescriptor));
        }
        private string NormalizeAndEnsureValidPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(path);
            return path[0] == '/' ? path : throw new ArgumentException( $"The path does not meet the standard {nameof (path)}");
        }
    }
}