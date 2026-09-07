namespace PdfOperations;

public class FileSorter
{
    public static string[] SortFilesByNumberAndName(string [] inputs)
    {
        return inputs.OrderBy(Path.GetFileName, new NaturalStringComparer()).ToArray();
    }
}