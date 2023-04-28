# RazorViewTemplateEngine
Razor 视图模板引擎

1、普通用法
```csharp
public class RazorViewRunTimeCompileTest {
    readonly IRazorEngine _razorEngine;

    public RazorViewRunTimeCompileTest() {
        _razorEngine = RazorEngine.Create(options => {
            //虚拟路径和模板内容
            options.TemplateStringCollections.Add(new TemplateContentCollection()
            { VirtualPath = "/Views/Home/Index.cshtml",
              Content = @"Hello @Model.Name, Welcome to RazorViewTemplateEngine!" });
            options.TemplateStringCollections.Add(new TemplateContentCollection()
            { VirtualPath = "/Views/Home/About.cshtml",
              Content = @"Hello @Model.Name, Welcome to RazorViewTemplateEngine!",
              //强类型模板
              InheritanceType = typeof(Student) });
        }, options => {
         //添加程序集引用
         options.AddAssemblyReference(typeof(Student).Assembly); });
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
```
2、依赖注入
```csharp
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
```

3、支持物理模板文件
```csharp
@inherits RazorViewTemplateEngine.Core.RazorViewPage
Hello @Model.Name, Welcome to RazorViewTemplateEngine!
```
支持物理模板文件，监听文件变化自动重新编译 ✨

TODO
* [ ] 根据设置的根目录结合通配符筛选模板文件