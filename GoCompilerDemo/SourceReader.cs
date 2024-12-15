namespace GoCompilerDemo;

//
// Summary:
//     Provides static methods for reading source code from a file.
internal static class SourceReader
{
    //
    // Summary:
    //     The message text to display when the source file is not found.
    public static string FileNotFoundExceptionMessage =
        "IO.FileNotFoundError: The 'source.go' file should be stored on the path: \"GoCompiler/GoCompilerDemo/bin/Debug/net8.0/Resources/source.go\".";

    //
    // Summary:
    //     Public delegate for writing text to the console.
    public static Action<string?> WriteLine = Console.WriteLine;

    //
    // Summary:
    //     Reads the whole content of the given file (located at '/GoCompilerDemo/bin/Debug/net8.0/...').
    public static string ReadSourceCode(string filePath)
    {
        string sourceCode = File.ReadAllText(filePath);
        return string.IsNullOrWhiteSpace(sourceCode) ? throw new EmptySourceException(filePath) : sourceCode;
    }
}

//
// Summary:
//     Represents an exception thrown when the targeted source file is empty.
internal class EmptySourceException(string filePath) : Exception($"Empty targeted file: {filePath}") { }
