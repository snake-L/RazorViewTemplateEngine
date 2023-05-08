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
            options.TemplateStringCollections.Add(new TemplateContentCollection()
            {
            VirtualPath = "/Views/Home/Entity.cshtml",
            Content = @"using System;
                        using System.Collections.Generic;
                        namespace @Model.NameSpace
                        {
                            public class @Model.TableName
                            {
                                @foreach (var item in Model.Columns)
                                {
                                    @:public @item.ColumnType @item.ColumnName { get; set; }
                                }
                            }
                        }",
              InheritanceType = typeof(TableStructure)
            });
        },
            options => { options.AddAssemblyReference(typeof(Student).Assembly); });
        _serviceProvider = services.BuildServiceProvider();
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

    [Fact]
    public async Task should_compile_entity_success() {
        var model = new
            TableStructure(2)
            { NameSpace = "RazorViewTemplateEngine.Tests",
              TableName = "Student" };
        model.Columns.Add(new ColumnDescription()
        {
         ColumnName = "Id",
         ColumnType = "int",
            Comment = "主键",
         IsCanNull = false
        }); model.Columns.Add(new ColumnDescription()
        {
        ColumnName = "Name",
        ColumnType = "string",
        Comment = "姓名",
        IsCanNull = false
        });
        var result = await _razorEngine.CompileGenericAsync("/Views/Home/Entity.cshtml",model);
        Assert.NotEmpty(result);
    }
}