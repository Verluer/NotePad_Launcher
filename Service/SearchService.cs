using System.Text.RegularExpressions;
using Domain.IService;

namespace Service;

public class SearchService : ISearchService
{
    public MatchCollection SearchPattern(string fileText, string pattern)
    {
       var regex = new Regex(pattern, RegexOptions.IgnoreCase);
       var matches = regex.Matches(fileText);
        return matches;
    }
}