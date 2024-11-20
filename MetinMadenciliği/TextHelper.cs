using System.Text.RegularExpressions;

public static class TextHelper
{
    // Noktalama işaretleri regex deseni
    public static readonly Regex Punctuations = new Regex(@"[.,;:?!""'()\-\—\/{}\[\]\\]", RegexOptions.Compiled);

    // Sayılarla birleşik kelimeler (ör. 2'li, 5'e)
    public static readonly Regex NumberWithText = new Regex(@"\d+['’][a-zA-ZğüşöçıİĞÜŞÖÇ]+", RegexOptions.Compiled);

    // Sadece sayılar
    public static readonly Regex Numbers = new Regex(@"\d+", RegexOptions.Compiled);

    /// <summary>
    /// Metindeki noktalama işaretlerini kaldırır.
    /// </summary>
    public static string RemovePunctuations(string text)
    {
        return Punctuations.Replace(text, "");
    }

    /// <summary>
    /// Metindeki sayılarla birleşik kelimeleri kaldırır.
    /// </summary>
    public static string RemoveNumbersWithText(string text)
    {
        return NumberWithText.Replace(text, "");
    }

    /// <summary>
    /// Metindeki tüm sayıları kaldırır.
    /// </summary>
    public static string RemoveAllNumbers(string text)
    {
        return Numbers.Replace(text, "");
    }

    public static string ToLowerCase(string text)
    {
        return text.ToLower();
    }
}
