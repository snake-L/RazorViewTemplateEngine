using System.Reflection;
using System.Text;
using Microsoft.Extensions.FileProviders;
using RazorViewTemplateEngine.Core;
using RazorViewTemplateEngine.Core.FileDescriptor;
using RazorViewTemplateEngine.Core.Interface;
using RazorViewTemplateEngine.Core.Options;

namespace RazorViewTemplateEngine.Tests; 

public class RazorViewRunTimeCompileTest {
    readonly IRazorEngine _razorEngine;
    private readonly string _rootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Views");
    public RazorViewRunTimeCompileTest() {
        _razorEngine = RazorEngine.Create((system,compiler) => {
            var txtPhysicalFileDescriptor = new PhysicalFileDescriptor(system.FileProvider,
                "/Hello.txt",
                compiler.OnReCompile);
            system.Add(txtPhysicalFileDescriptor);
        },options => {
            options.PhysicalDirectoryPath = _rootDirectory;
            options.TemplateStringCollections.Add(new TemplateContentCollection()
            { VirtualPath = "/Views/Home/Index.cshtml",
              Content = @"Hello @Model.Name, Welcome to RazorViewTemplateEngine!" });
            options.TemplateStringCollections.Add(new TemplateContentCollection()
            { VirtualPath = "/Views/Home/About.cshtml",
              Content = @"Hello @Model.Name, Welcome to RazorViewTemplateEngine!",
              InheritanceType = typeof(Student) });
        }, options => { options.AddAssemblyReference(typeof(Student).Assembly); });
    }

    [Theory]
    [InlineData("/Views/Home/Index.cshtml")]
    public async Task should_compile_dynamic_success(string path) {
       var result = await _razorEngine.CompileDynamicAsync(path, new
        { Name = "Alx" });
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

