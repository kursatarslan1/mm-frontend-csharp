public class TextProcessor
{
    private readonly ZemberekService _zemberekService;

    public TextProcessor()
    {
        _zemberekService = new ZemberekService();
    }

    public string FullAuto(string text)
    {
        string withoutPunctuation = TextHelper.RemovePunctuations(text);
        string withoutNumbers = TextHelper.RemoveAllNumbers(withoutPunctuation);
        string lowerCased = TextHelper.ToLowerCase(withoutNumbers);
        string zemberekResult = _zemberekService.AnalyzeText(lowerCased);
        string withoutStopWords = StopWordsService.RemoveStopWords(zemberekResult);
        string uniqueWords = UniqueWordsService.FindUniqueWords(withoutStopWords);

        return uniqueWords;
    }
}
