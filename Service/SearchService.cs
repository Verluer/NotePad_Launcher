using System.Text.RegularExpressions;
using Domain.IService;
using static System.Net.Mime.MediaTypeNames;

namespace Service;

public class SearchService : ISearchService
{
    public MatchCollection SearchPattern(string fileText, string pattern, bool isRegisterAware)
    {
        var options = isRegisterAware ? RegexOptions.None : RegexOptions.IgnoreCase;
        var regex = new Regex(pattern, options);
       var matches = regex.Matches(fileText);
        return matches;
    }
    public string ReplaceText(string fileText, int startIndex, int length, string patternReplace)
    {
        string resultReplace = fileText.Substring(0, startIndex) + patternReplace + fileText.Substring(startIndex + length);
        return resultReplace;
    }
    public string ReplaceAllText(string fileText, string pattern, string patternReplace, bool isRegisterAware)
    {
        string resultReplaceAll;

        if (isRegisterAware == false)
        {
           resultReplaceAll = Regex.Replace(fileText, $"{pattern}", $"{patternReplace}", RegexOptions.IgnoreCase);
        }
        else
        {
            resultReplaceAll = fileText.Replace($"{pattern}", $"{patternReplace}");
        }

        return resultReplaceAll;
    }
}