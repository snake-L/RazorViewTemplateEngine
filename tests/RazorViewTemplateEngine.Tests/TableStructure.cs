namespace RazorViewTemplateEngine.Tests; 

public class TableStructure {
    public TableStructure(int capacity) {
        Columns = new List<ColumnDescription>(capacity);
    }
    public List<ColumnDescription> Columns { get; set; }
    public string NameSpace { get; set; }
    public string TableName { get; set; }
}
public sealed class ColumnDescription {
    public string ColumnName { get; set; }
    public string ColumnType { get; set; }
    public string Comment { get; set; }
    public bool IsCanNull { get; set; }
}