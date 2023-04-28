using System.Diagnostics;
using Microsoft.AspNetCore.Razor.Language;
using RazorViewTemplateEngine.Core.Interface;

namespace RazorViewTemplateEngine.Core {

    // Internal for testing
    [DebuggerDisplay("{Path}")]
    internal class FileNode
    {
        public FileNode(string path, IFileDescriptor file)
        {
            Path = path;
            FileInfo = file;
        }

        public DirectoryNode Directory { get; set; }

        public string Path { get; }

        public IFileDescriptor FileInfo { get; set; }
    }
}