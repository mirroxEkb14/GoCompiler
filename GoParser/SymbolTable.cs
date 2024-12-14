namespace GoParser;

public class SymbolTable
{
    private readonly Dictionary<string, (string Type, object? Value)> _table = new();

    public void AddVariable(string name, string type, object? value = null)
    {
        if (_table.ContainsKey(name))
            throw new Exception($"Variable '{name}' is already defined.");

        _table[name] = (type, value);
        Console.WriteLine($"Added variable to symbol table: Name = {name}, Type = {type}, Value = {value}");
    }

    public (string Type, object? Value) GetVariable(string name)
    {
        if (!_table.ContainsKey(name))
            throw new Exception($"Variable '{name}' is not defined.");

        return _table[name];
    }

    public bool Contains(string name) => _table.ContainsKey(name);
}
