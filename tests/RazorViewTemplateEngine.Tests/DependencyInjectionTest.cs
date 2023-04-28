using Microsoft.Extensions.DependencyInjection;
using RazorViewTemplateEngine.Core;
using RazorViewTemplateEngine.Core.DependencyInjection;
using RazorViewTemplateEngine.Core.FileDescriptor;
using RazorViewTemplateEngine.Core.Interface;
using RazorViewTemplateEngine.Core.Options;

namespace RazorViewTemplateEngine.Tests; 

public class DependencyInjectionTest {
    private IServiceProvider _serviceProvider;
    private IRazorEngine _razorEngine;
    private readonly string _rootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Views");
    public DependencyInjectionTest() {
        var services = new ServiceCollection();
        services.AddRazorViewTemplateEngine(options => { 
            options.PhysicalDirectoryPath = _rootDirectory;
            options.TemplateStringCollections.Add(new TemplateContentCollection()
            {
            VirtualPath = "/Views/Home/Index.cshtml",
            Content = @"Hello @Model.Name, Welcome to RazorViewTemplateEngine!",
            });
            options.TemplateStringCollections.Add(new TemplateContentCollection()
            { VirtualPath = "/Views/Home/About.cshtml",
              Content = @"Hello @Model.Name, Welcome to RazorViewTemplateEngine!",
              InheritanceType = typeof(Student) });
        },
            options => { options.AddAssemblyReference(typeof(Student).Assembly); });
        _serviceProvider = services.BuildServiceProvider();
        var fileSystem = _serviceProvider.GetRequiredService<RazorFileSystem>();
        var compiler = _serviceProvider.GetRequiredService<IRuntimeViewCompiler>();
        var txtPhysicalFileDescriptor = new PhysicalFileDescriptor(fileSystem.FileProvider,
            "/Hello.txt",
            compiler.OnReCompile);
        fileSystem.Add(txtPhysicalFileDescriptor);
        _razorEngine = _serviceProvider.GetRequiredService<IRazorEngine>();
    }
   
    [Theory]
    [InlineData("/Views/Home/Index.cshtml")]
    public void should_compile_dynamic_success(string path) {
        var result = _razorEngine.CompileDynamicAsync(path, new
        { Name = "Alx" }).Result;
        Assert.Equal("Hello Alx, Welcome to RazorViewTemplateEngine!", result);
    }
    [Theory]
    [InlineData("/Views/Home/About.cshtml")]
    public async Task should_compile_stronglyTyped_success(string path) {
        var result = await _razorEngine.CompileGenericAsync(path, new
            Student { Name = "Alx" });
        Assert.Equal("Hello Alx, Welcome to RazorViewTemplateEngine!", result);
    }
    [Theory]
    [InlineData("/Hello.txt")]
    public async Task should_compile_with_physicalfile_success(string path) {
        var result = await _razorEngine.CompileDynamicAsync(path, new
        { Name = "Alx" });
        Assert.Equal("Hello Alx, Welcome to RazorViewTemplateEngine!", result);

        await using FileStream fileStream = File.Open(Path.Combine(_rootDirectory + path), FileMode.Open);
        fileStream.SetLength(0);
        fileStream.Close();
        //更改hello.txt中的文件内容，再次编译，结果应该变化
        await File.AppendAllLinesAsync(Path.Combine(_rootDirectory + path),
            new[]
            { "@inherits RazorViewTemplateEngine.Core.RazorViewPage", "Hello @Model.Name" });

        await Task.Delay(1000 * 2);
        result = await _razorEngine.CompileDynamicAsync(path, new
        { Name = "Alx" });
        Assert.Equal("Hello Alx", result.Replace("\r\n",""));
    }
}