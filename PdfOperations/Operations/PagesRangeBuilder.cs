namespace PdfOperations;

public class PagesRangeBuilder
{
    public static string BuildEvenPages(int pagesCount)
    {
        return BuildPagesByStep(2, pagesCount);
    }

    public static string BuildOddPages(int pagesCount)
    {
        return BuildPagesByStep(1, pagesCount);
    }

    public static string BuildPagesByStep(int start, int end)
    {
        List<string> pages = new List<string>();
        
        for (int page = start; page <= end; page += 2)
        {
            pages.Add(page.ToString());
        }

        return string.Join(",", pages);
    }
}