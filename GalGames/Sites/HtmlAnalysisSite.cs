using System.Text;
using GalgameSearchFor.GalGames.Platform;
using HtmlAgilityPack;
using RegexMatch = GalgameSearchFor.GalGames.Sites.ConstantSettings.RegexMatch;

namespace GalgameSearchFor.GalGames.Sites;

public abstract class HtmlAnalysisSite<TGalgameInfo>(Uri url, TimeSpan? timeout = null) : SearcherFormResult<TGalgameInfo>(url, timeout)
{
    protected readonly HtmlDocument _document = new();
    protected readonly StringBuilder _stringBuilder = new(128);


    protected abstract List<TGalgameInfo> AnalysisHtml(ref string html);


    protected static int Parse(string content)
    {
        var numberString = RegexMatch.MatchSizeNumber().Match(content);

        if (!numberString.Success) return -1;

        var number = float.Parse(numberString.Value);


        if (content.EndsWith("W", StringComparison.OrdinalIgnoreCase))
        {
            return (int)(number * 10000);
        }

        return (int)number;
    }

    protected string Trim(string content)
    {
        _stringBuilder.Clear();
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < content.Length; i++)
        {
            if (content[i] == '\n' || content[i] == '\r' || content[i] == ' ')
                continue;

            _stringBuilder.Append(content[i]);
        }

        return _stringBuilder.ToString();
    }
}