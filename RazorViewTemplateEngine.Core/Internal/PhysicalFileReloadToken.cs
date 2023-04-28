using System;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace RazorViewTemplateEngine.Core.Internal {
    internal class PhysicalFileReloadToken   : IChangeToken {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        public bool ActiveChangeCallbacks => true;
    
        public bool HasChanged => _cts.IsCancellationRequested;
    
        public IDisposable RegisterChangeCallback(Action<object> callback, object state) => _cts.Token.Register(callback, state);
    
        public void OnReload() => _cts.Cancel();
    }
} 

