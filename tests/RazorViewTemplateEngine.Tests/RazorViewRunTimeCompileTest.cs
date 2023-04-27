using RazorViewTemplateEngine.Core;
using RazorViewTemplateEngine.Core.Interface;
using RazorViewTemplateEngine.Core.Options;

namespace RazorViewTemplateEngine.Tests; 

public class RazorViewRunTimeCompileTest {
    readonly IRazorEngine _razorEngine;

    public RazorViewRunTimeCompileTest() {
        _razorEngine = RazorEngine.Create(options => {
            options.TemplateStringCollections.Add(new TemplateContentCollection()
            { VirtualPath = "/Views/Home/Index.cshtml",
              Content = @"Hello @Model.Name, Welcome to RazorViewTemplateEngine!" });
            options.TemplateStringCollections.Add(new TemplateContentCollection()
            { VirtualPath = "/Views/Home/About.cshtml",
              Content = @"Hello @Model.Name, Welcome to RazorViewTemplateEngine!",
              InheritanceType = typeof(Student) });
        }, options => { options.AddAssemblyReference(typeof(Student).Assembly); });
    }

    [Fact]
    public async Task should_compile_dynamic_success() {
       var result = await _razorEngine.CompileDynamicAsync("/Views/Home/Index.cshtml", new
        { Name = "Alx" });
        Assert.Equal("Hello Alx, Welcome to RazorViewTemplateEngine!", result);
    }
    
    [Fact]
    public async Task should_compile_stronglyTyped_success() {
        var result = await _razorEngine.CompileGenericAsync("/Views/Home/About.cshtml", new
        Student { Name = "Alx" });
        Assert.Equal("Hello Alx, Welcome to RazorViewTemplateEngine!", result);
    }
}

