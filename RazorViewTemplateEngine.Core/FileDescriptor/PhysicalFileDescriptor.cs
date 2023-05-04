using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using RazorViewTemplateEngine.Core.Internal;

namespace RazorViewTemplateEngine.Core.FileDescriptor {
    public class PhysicalFileDescriptor : AbstractFileDescriptor {
        
        private Action<object> OnPhysicalFileChanged { get; set; }
        
        private PhysicalFileReloadToken _reloadToken = new PhysicalFileReloadToken();
        private IDisposable ChangeTokenRegistration { get; set; }
        private  IFileProvider FileProvider { get; set; }
        private IFileInfo FileInfo { get; set; }
        
        public PhysicalFileDescriptor(IFileProvider fileProvider,string relativeFilePath,
            Action<string> callback = null)
            : base(relativeFilePath, null) {
            FileProvider = fileProvider;
            FileInfo = FileProvider.GetFileInfo(RelativeFilePath);
            if (!FileInfo.Exists) {
                //抛出异常 在根路径下找不到该文件
                throw new FileNotFoundException("Unable to find the file according to the instructions",
                    RelativeFilePath);
            }
            ChangeTokenRegistration = ChangeToken.OnChange(() => FileProvider.Watch(RelativeFilePath),
                () => {
                    Thread.Sleep(200); //延迟200毫秒，等待文件写入完成
                    OnReload();
                });
            if (callback != null) {
                OnPhysicalFileChanged = obj => {
                    callback(RelativeFilePath);
                };
                _reloadToken.RegisterChangeCallback(OnPhysicalFileChanged,null);
            }
        }
        public override Stream Read() {
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(
                       FileInfo.PhysicalPath,
                       FileMode.Open,
                       FileAccess.Read,
                       FileShare.ReadWrite,
                       bufferSize: 1,
                       FileOptions.SequentialScan)) {
                stream.CopyToAsync(memoryStream).Wait();
            }
            return memoryStream;
        }
        private void OnReload() {
            var token = new PhysicalFileReloadToken();
            if (OnPhysicalFileChanged != null) {
                token.RegisterChangeCallback(OnPhysicalFileChanged,null);
            }
            PhysicalFileReloadToken previousToken = Interlocked.Exchange(ref _reloadToken, token);
            previousToken.OnReload();
        }
        public override void Dispose() {
            ChangeTokenRegistration?.Dispose();
            OnPhysicalFileChanged = null;
        }
    }
}