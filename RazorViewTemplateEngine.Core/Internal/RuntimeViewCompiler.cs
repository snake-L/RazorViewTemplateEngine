using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using RazorViewTemplateEngine.Core.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RazorViewTemplateEngine.Core.Options;

namespace RazorViewTemplateEngine.Core.Internal
{
    /// <summary>
    /// Razor 视图模板引擎
    /// </summary>
    internal class RuntimeViewCompiler : IRuntimeViewCompiler
    {
       private  readonly CSharpCompiler _csharpCompiler;
       private  readonly RazorProjectEngine _projectEngine;
       private  readonly MvcRazorRuntimeCompilationOptions _runtimeCompilationOptions;
       private  readonly ReaderWriterLockSlim _readerLockSlim;
       private  readonly Dictionary<string, CompiledViewDescriptor> _compiledViewDescriptors; 
       private  readonly RazorFileSystem _razorFileSystem;
        public RuntimeViewCompiler(
            RazorProjectEngine razorProjectEngine,
            CSharpCompiler cSharpCompiler,
            RazorFileSystem razorFileSystem,
            MvcRazorRuntimeCompilationOptions mvcRazorRuntimeCompilationOptions) {
            _compiledViewDescriptors = new Dictionary<string, CompiledViewDescriptor>();
            _projectEngine = razorProjectEngine;
            _csharpCompiler = cSharpCompiler;
            _razorFileSystem = razorFileSystem;
            _runtimeCompilationOptions = mvcRazorRuntimeCompilationOptions;
            _readerLockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }
        public Task<CompiledViewDescriptor> CompileAsync(string relativePath)
        {
            TaskCompletionSource<CompiledViewDescriptor> source = new TaskCompletionSource<CompiledViewDescriptor>();
            _readerLockSlim.EnterReadLock();
            try
            {
                if (_compiledViewDescriptors.TryGetValue(relativePath, out var compiledViewDescriptor))
                {
                    source.SetResult(compiledViewDescriptor);
                    return source.Task;
                }
            }
            finally
            {
                _readerLockSlim?.ExitReadLock();
            }
            _readerLockSlim.EnterUpgradeableReadLock();
            try
            {
                if (_compiledViewDescriptors.TryGetValue(relativePath, out var compiledViewDescriptor))
                {
                    source.SetResult(compiledViewDescriptor);
                    return source.Task;
                }
                else
                {
                    var descriptor = Compile(relativePath);
                    _readerLockSlim.EnterWriteLock();
                    try
                    {
                        _compiledViewDescriptors.Add(relativePath, descriptor);
                        source.SetResult(descriptor);
                        return source.Task;
                    }
                    catch (Exception e)
                    {
                        source.SetException(e);
                        return source.Task;
                    }
                    finally
                    {
                        _readerLockSlim.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _readerLockSlim.ExitUpgradeableReadLock();
            }
        }

        public void OnReCompile(string relativePath) {
            _readerLockSlim.EnterReadLock();
            try {
                if (_compiledViewDescriptors.TryGetValue(relativePath, out var compiledViewDescriptor)) {
                    compiledViewDescriptor.Dispose();
                    _compiledViewDescriptors.Remove(relativePath);
                }
            }
            finally {
                _readerLockSlim?.ExitReadLock();
            }

            _readerLockSlim.EnterUpgradeableReadLock();
            try {
                var descriptor = Compile(relativePath);
                _readerLockSlim.EnterWriteLock();
                try {
                    _compiledViewDescriptors.Add(relativePath, descriptor);
                }
                finally {
                    _readerLockSlim.ExitWriteLock();
                }
            }
            finally {
                _readerLockSlim.ExitUpgradeableReadLock();
            }
        }

        internal CompiledViewDescriptor Compile(string relativePath)
        {
             var fileDescriptor = _razorFileSystem.GetItem(relativePath);
             if (fileDescriptor == null)
                 throw new NotSupportedException($"RelativePath not supported:{relativePath}");
             
             string fileName = Path.GetRandomFileName();
             RazorSourceDocument razorSourceDocument = fileDescriptor.IsPhysical ?
                 RazorSourceDocument.ReadFrom(fileDescriptor.Read(),fileName) : 
                 RazorSourceDocument.Create(fileDescriptor.Content,fileName);
             
             RazorCodeDocument  codeDocument = _projectEngine.Process(
                     razorSourceDocument , 
                     null,
                     new List<RazorSourceDocument>(),
                     new List<TagHelperDescriptor>());
             
            var cSharpDocument = codeDocument.GetCSharpDocument();
            Debug.WriteLine(cSharpDocument.GeneratedCode);
            var assembly = CompileAssembly(cSharpDocument.GeneratedCode);
            return new CompiledViewDescriptor(assembly, _runtimeCompilationOptions.TemplateNamespace);
        }

        internal Assembly CompileAssembly(string generatedCode)
        {
            var assemblyName = Path.GetRandomFileName();
            var compilation = CreateCompilation(generatedCode, assemblyName);
            using (var assemblyStream = new MemoryStream())
            {
                var result = compilation.Emit(assemblyStream);
                if (!result.Success)
                {
                    throw new CompilationFailureException(result.Diagnostics.ToList(), generatedCode);
                }
                assemblyStream.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(assemblyStream.ToArray());
                return assembly;
            }
        }

        private CSharpCompilation CreateCompilation(string compilationContent, string assemblyName)
        {
            var sourceText = SourceText.From(compilationContent, Encoding.UTF8);
            var syntaxTree = _csharpCompiler.CreateSyntaxTree(sourceText).WithFilePath(assemblyName);
            return _csharpCompiler
                .CreateCompilation(assemblyName)
                .AddSyntaxTrees(syntaxTree);
        }
    }
}
