using System.Linq;

public static class UniqueWordsService
{
    public static string FindUniqueWords(string text)
    {
        var words = text.Split(' ');
        var uniqueWords = words.Distinct();
        return string.Join(" ", uniqueWords);
    }
}
