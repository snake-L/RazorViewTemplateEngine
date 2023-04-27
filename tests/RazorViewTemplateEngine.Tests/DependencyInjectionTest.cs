using Microsoft.Extensions.DependencyInjection;
using RazorViewTemplateEngine.Core.DependencyInjection;
using RazorViewTemplateEngine.Core.Interface;
using RazorViewTemplateEngine.Core.Options;

namespace RazorViewTemplateEngine.Tests; 

public class DependencyInjectionTest {
    IServiceProvider _serviceProvider;
    public DependencyInjectionTest() {
        var services = new ServiceCollection();
        services.AddRazorViewTemplateEngine(options => {
            options.TemplateStringCollections.Add(new TemplateContentCollection()
            {
            VirtualPath = "/Views/Home/Index.cshtml",
            Content = @"Hello @Model.Name, Welcome to RazorViewTemplateEngine!",
            });
            options.TemplateStringCollections.Add(new TemplateContentCollection()
            { VirtualPath = "/Views/Home/About.cshtml",
              Content = @"Hello @Model.Name, Welcome to RazorViewTemplateEngine!",
              InheritanceType = typeof(Student) });
        },options => { options.AddAssemblyReference(typeof(Student).Assembly); });
        _serviceProvider = services.BuildServiceProvider();
    }
    [Fact]
    public void should_get_service_success() {
        var razorEngine = _serviceProvider.GetService<IRazorEngine>();
        Assert.NotNull(razorEngine);
    }
    [Fact]
    public void should_compile_dynamic_success() {
        var razorEngine = _serviceProvider.GetService<IRazorEngine>();
        var result = razorEngine.CompileDynamicAsync("/Views/Home/Index.cshtml", new
        { Name = "Alx" }).Result;
        Assert.Equal("Hello Alx, Welcome to RazorViewTemplateEngine!", result);
    }
    [Fact]
    public async Task should_compile_stronglyTyped_success() {
        var razorEngine = _serviceProvider.GetService<IRazorEngine>();
        var result = await razorEngine.CompileGenericAsync("/Views/Home/About.cshtml", new
            Student { Name = "Alx" });
        Assert.Equal("Hello Alx, Welcome to RazorViewTemplateEngine!", result);
    }
}