using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.Language;
using RazorViewTemplateEngine.Core.Interface;
using RazorViewTemplateEngine.Core.Options;

namespace RazorViewTemplateEngine.Core.Internal {
    internal sealed class RazorFileSystem {
        private readonly DirectoryNode _root;
 
        private readonly TemplateOptions _templateOptions;
        private readonly MvcRazorRuntimeCompilationOptions _mvcRazorRuntimeCompilationOptions;
        public RazorFileSystem(TemplateOptions templateOptions,
            MvcRazorRuntimeCompilationOptions mvcRazorRuntimeCompilationOptions) {
            _templateOptions = templateOptions;
            _mvcRazorRuntimeCompilationOptions = mvcRazorRuntimeCompilationOptions;
            _root = new DirectoryNode(_templateOptions.TemplateDirectoryPath);
            //TODO 读取该路径下的文件，添加到root中。
            
            
            if (_templateOptions.TemplateStringCollections == null) return;
            foreach (var template in _templateOptions.TemplateStringCollections) {
                _root.AddFile(new FileNode(template.VirtualPath, 
                    new VirtualFileDescriptor(template.VirtualPath,WriteDirectives(template))));
            }
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
        private string WriteDirectives(TemplateContentCollection template)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (template.InheritanceType != null) {
                string studentName = template.InheritanceType.DeclaringType == null
                    ? template.InheritanceType.Name
                    : template.InheritanceType.DeclaringType.Name + "." + template.InheritanceType.Name;
                string inheritType = template.InheritanceType != null ? $"<{studentName}>" : "";
                stringBuilder.AppendLine($"@inherits {_mvcRazorRuntimeCompilationOptions.Inherits}{inheritType}");
                stringBuilder.AppendLine($"@using {template.InheritanceType.Namespace}");
            } else {
                stringBuilder.AppendLine($"@inherits {_mvcRazorRuntimeCompilationOptions.Inherits}");
            }
            foreach (string entry in _mvcRazorRuntimeCompilationOptions.Usings)
            {
                stringBuilder.AppendLine($"@using {entry}");
            }
            stringBuilder.Append(template.Content);
            return stringBuilder.ToString();
        }
        
        private string NormalizeAndEnsureValidPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(path);
            return path[0] == '/' ? path : throw new ArgumentException( $"The path does not meet the standard {nameof (path)}");
        }
    }
}