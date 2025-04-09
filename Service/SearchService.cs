using System.Text.RegularExpressions;
using Domain.IService;

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
}