using System.Text.RegularExpressions;

namespace Domain.IService;

public interface ISearchService
{
    public MatchCollection SearchPattern(string fileText, string pattern, bool isRegisterAware);
    public string ReplaceText(string fileText, int startIndex, int length, string patternReplace);
    public string ReplaceAllText(string fileText, string pattern, string patternReplace, bool isRegisterAware);
}