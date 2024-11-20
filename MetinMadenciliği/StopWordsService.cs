using System.Collections.Generic;
using System.Linq;

public static class StopWordsService
{
    private static readonly HashSet<string> StopWords = new HashSet<string>
    {
        "ve", "bir", "bu", "şu", "ile", "de", "da" // Örnek stop words
    };

    public static string RemoveStopWords(string text)
    {
        var words = text.Split(' ');
        var filteredWords = words.Where(word => !StopWords.Contains(word));
        return string.Join(" ", filteredWords);
    }
}
