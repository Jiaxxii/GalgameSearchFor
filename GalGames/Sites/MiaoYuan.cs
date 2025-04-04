using GalgameSearchFor.GalGames.Sites.Results.MiaoYuan;
using HtmlAgilityPack;

namespace GalgameSearchFor.GalGames.Sites;

public partial class MiaoYuan : HtmlAnalysisSite<GalgameInfo>
{
    private partial class InternalWriteConsole;

    public MiaoYuan(TimeSpan? timeout = null) : base(new Uri("https://www.nekotaku.me"), timeout)
    {
        WriteConsole = new InternalWriteConsole(()=>Results, _baseUri);
    }

    private const string SearchUriPath = "?s={NAME}&type=post";

    public override IWrieConsole WriteConsole { get; }


    public override IEnumerable<GalgameInfo> SearchResult(string key)
    {
        throw new NotImplementedException();
    }

    public override async Task<IEnumerable<GalgameInfo>> SearchResultAsync(string key, CancellationToken cancellationToken = default)
    {
        var httpResponseMessage = await GetAsync(SearchUriPath.Replace("{NAME}", key), cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        var content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);


        return Results = AnalysisHtml(ref content);
    }


    protected override List<GalgameInfo> AnalysisHtml(ref string html)
    {
        _document.LoadHtml(html);

        var postsHtmlNodeCollection = _document.DocumentNode.SelectNodes("//div[contains(@class,'search-content')]/posts");

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (postsHtmlNodeCollection == null || postsHtmlNodeCollection.Count == 0) return [];

        var gameInfos = new List<GalgameInfo>();
        foreach (var posts in postsHtmlNodeCollection)
        {
            var gameImageUrl = posts.SelectSingleNode("div/a/img").GetAttributeValue("data-src", string.Empty);

            var (gamePageLink, title) = GetGamePageLink(posts, "div[2]/h2/a");

            var tags = posts.SelectNodes("div[2]/div[1]/a").Select(a => Trim(a.InnerText));

            var author = GetAuthor(posts.SelectSingleNode("div[2]/div[2]"));
            var hotInfo = GetHot(posts.SelectSingleNode("div[2]/div[2]"));

            var gameInfo = new GalgameInfo(gameImageUrl, gamePageLink, title, tags, author, hotInfo);

            gameInfos.Add(gameInfo);
        }

        return gameInfos;
    }

    private static (string gamePageLink, string title) GetGamePageLink(HtmlNode node, string xPath)
    {
        var aNode = node.SelectSingleNode(xPath);
        var gamePageLink = aNode.GetAttributeValue("href", string.Empty);
        var title = aNode.InnerText.Trim();

        return (gamePageLink, title);
    }

    private static Author GetAuthor(HtmlNode node, string xPath = "item")
    {
        var authorNode = node.SelectSingleNode(xPath);
        var authorLink = authorNode.SelectSingleNode("a").GetAttributeValue("href", string.Empty);

        var headLink = authorNode.SelectSingleNode("a/span/img").GetAttributeValue("data-src", string.Empty);
        var authorName = authorNode.SelectSingleNode("span/text()").InnerText;

        var author = new Author(headLink, authorName, authorLink);
        return author;
    }

    private static HotInfo GetHot(HtmlNode node, string xPath = "div")
    {
        var hotNode = node.SelectSingleNode(xPath);
        var evaluateCount = Parse(hotNode.SelectSingleNode("item[1]").InnerText);
        var watchCount = Parse(hotNode.SelectSingleNode("item[2]").InnerText);
        var likeCount = Parse(hotNode.SelectSingleNode("item[3]").InnerText);

        var hotInfo = new HotInfo(evaluateCount, watchCount, likeCount);
        return hotInfo;
    }
}