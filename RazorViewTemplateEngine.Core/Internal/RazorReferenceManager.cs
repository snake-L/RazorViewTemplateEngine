using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using RazorViewTemplateEngine.Core.Options;

namespace RazorViewTemplateEngine.Core.Internal {
    internal sealed class RazorReferenceManager {
        private readonly MvcRazorRuntimeCompilationOptions _options;
        public RazorReferenceManager(MvcRazorRuntimeCompilationOptions options) {
            _options = options;
        }
        public IReadOnlyList<MetadataReference> CompilationReferences {
            get {
                return _options.ReferenceAssemblies
                   .Select(ass => {
#if NETSTANDARD2_0
                            return  MetadataReference.CreateFromFile(ass.Location); 
#else
                       unsafe {
                           ass.TryGetRawMetadata(out byte* blob, out int length);
                           ModuleMetadata moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)blob, length);
                           AssemblyMetadata assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
                           PortableExecutableReference metadataReference = assemblyMetadata.GetReference();

                           return metadataReference;
                       }
#endif
                   })
                    .Concat(_options.MetadataReferences)
                    .ToList();
            }
        }
    }
}
