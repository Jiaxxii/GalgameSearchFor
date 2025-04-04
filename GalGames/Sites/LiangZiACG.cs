using System.Globalization;
using GalgameSearchFor.GalGames.Sites.Results.LiangZi;
using HtmlAgilityPack;

namespace GalgameSearchFor.GalGames.Sites;

public partial class LiangZiACG : HtmlAnalysisSite<GalgameInfo>
{
    private partial class InternalWireConsole;

    public LiangZiACG(TimeSpan? timeout = null) : base(new Uri("https://www.lzacg.org/"), timeout)
    {
        WriteConsole = new InternalWireConsole(()=>Results, _baseUri);
    }

    public override IWrieConsole WriteConsole { get; }

    public override IEnumerable<GalgameInfo> SearchResult(string key)
    {
        throw new NotImplementedException();
    }

    public override async Task<IEnumerable<GalgameInfo>> SearchResultAsync(string key, CancellationToken cancellationToken = default)
    {
        var httpResponseMessage = await GetAsync(string.Concat("?s=", key), cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        var content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        return Results = AnalysisHtml(ref content);
    }

    private static string ExtractByName(string gameName)
    {
        // 【Gal】【PC】寻爱或赴死
        var start = gameName.LastIndexOf('】') + 1;
        return gameName.Substring(start);
    }

    protected override List<GalgameInfo> AnalysisHtml(ref string html)
    {
        _document.LoadHtml(html);

        var postsHtmlNodeCollection = _document.DocumentNode.SelectNodes("//div[contains(@class,'overflow-hidden')]/posts");

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (postsHtmlNodeCollection == null || postsHtmlNodeCollection.Count == 0)
        {
            return [];
        }

        var result = new List<GalgameInfo>();
        foreach (var posts in postsHtmlNodeCollection)
        {
            var gameImageUrl = posts.SelectSingleNode("div/a/img").GetAttributeValue("data-src", string.Empty);

            var (gamePageLink, title) = GetGamePageLink(posts, "div[2]/h2/a");

            var tags = posts.SelectNodes("div[2]/div[1]/a").Select(a => Trim(a.InnerText));

            var (creationData, evaluateCount, watchCount) = GetGameHot(posts, "div[2]/div[2]");

            var gameInfo = new GalgameInfo(gameImageUrl, gamePageLink, title, tags, creationData, evaluateCount, watchCount);
            result.Add(gameInfo);
        }

        return result;
    }

    private static (string gamePageLink, string title) GetGamePageLink(HtmlNode node, string xPath)
    {
        var aNode = node.SelectSingleNode(xPath);
        var gamePageLink = aNode.GetAttributeValue("href", string.Empty);
        var title = aNode.InnerText.Trim();

        return (gamePageLink, title);
    }

    private (DateTime creationData, int evaluateCount, int watchCount ) GetGameHot(HtmlNode node, string xPath)
    {
        var divNode = node.SelectSingleNode(xPath);
        var creationDate = divNode.SelectSingleNode("item").GetAttributeValue("title", string.Empty);

        // 2025-03-22 16:21:49
        var dateTime = DateTime.ParseExact(creationDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);

        var evaluateCount = Trim(divNode.SelectSingleNode("div/item[1]").InnerText);
        var watchCount = Trim(divNode.SelectSingleNode("div/item[2]").InnerText);

        return (dateTime, Parse(evaluateCount), Parse(watchCount));
    }
}