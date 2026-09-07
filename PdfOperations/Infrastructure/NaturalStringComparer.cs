using System.Text.RegularExpressions;

namespace PdfOperations;

public class NaturalStringComparer : IComparer<string?>
{
    public int Compare(string? x, string? y)
    {
        if (x == y)
            return 0;
        
        if (x == null)
            return -1;
        
        if (y == null)
            return 1;
        
        string[] xParts = Regex.Split(x, @"(\d+)");
        string[] yParts = Regex.Split(y, @"(\d+)");

        int count = Math.Min(xParts.Length, yParts.Length);

        for (int i = 0; i < count; i++)
        {
            bool xIsNumber = int.TryParse(xParts[i], out int xNumber);
            bool yIsNumber = int.TryParse(yParts[i], out int yNumber);

            if (xIsNumber && yIsNumber)
            {
                int numberCompare = xNumber.CompareTo(yNumber);

                if (numberCompare != 0)
                    return numberCompare;
            }
            else
            {
                int textCompare = string.Compare(
                    xParts[i], 
                    yParts[i], 
                    StringComparison.OrdinalIgnoreCase
                );
                
                if (textCompare != 0)
                    return textCompare;
            } 
        }
        
        return xParts.Length.CompareTo(yParts.Length);
    }
}