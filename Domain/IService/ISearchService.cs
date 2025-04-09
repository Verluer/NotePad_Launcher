using System.Text.RegularExpressions;

namespace Domain.IService;

public interface ISearchService
{
    public MatchCollection SearchPattern(string fileText, string pattern, bool isRegisterAware);
}