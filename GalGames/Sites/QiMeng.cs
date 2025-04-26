using GalgameSearchFor.GalGames.Sites.Results;
using GalgameSearchFor.GalGames.Sites.Results.QiMeng;

namespace GalgameSearchFor.GalGames.Sites;

public partial class QiMeng : HtmlAnalysisSite<GalgameItem>, IResourceRoot<QiMengResult>
{
    private partial class InternalWriteConsole;

    public QiMeng(TimeSpan? timeout = null) : base(new Uri("https://game.acgs.one"), timeout)
    {
        WriteConsole = new InternalWriteConsole(_baseUri, () => Resource ?? throw new NullReferenceException($"未进行网络请求导致\"{nameof(Resource)}\"未包含任何有效值！"));
    }

    private const string SearchUriPath = "/search";

    public QiMengResult? Resource { get; private set; }


    public override IWrieConsole WriteConsole { get; }

    public override IEnumerable<GalgameItem> SearchResult(string key)
    {
        throw new NotImplementedException();
    }


    public override async Task<IEnumerable<GalgameItem>> SearchResultAsync(string key, CancellationToken cancellationToken = default)
    {
        using var httpResponseMessage = await GetAsync($"{SearchUriPath}/{Uri.EscapeDataString(key)}/", cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        var content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        return AnalysisHtml(ref content);
    }

    protected override List<GalgameItem> AnalysisHtml(ref string html)
    {
        _document.LoadHtml(html);

        var galgameInfoContainerRoot = _document.DocumentNode.SelectNodes("//div[@id='ajax-list']/article");


        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (galgameInfoContainerRoot == null || galgameInfoContainerRoot.Count == 0)
        {
            return [];
        }

        var galgameItems = new List<GalgameItem>();
        foreach (var galgameInfo in galgameInfoContainerRoot)
        {
            var downloadUrl = galgameInfo.SelectSingleNode("a").GetAttributeValue("href", string.Empty);
            var image = galgameInfo.SelectSingleNode("a/img").GetAttributeValue("src", string.Empty);
            var titleInnerText = galgameInfo.SelectSingleNode("div/a/text()").InnerText;

            var galgameItem = new GalgameItem(downloadUrl, titleInnerText, image);
            galgameItems.Add(galgameItem);
        }

        var pageNode = _document.DocumentNode.SelectNodes("/html/body/div[1]/main/div[2]/div[2]/h2/span[1]/text()");
        var pageCount = pageNode is { Count: > 0 } ? int.Parse(pageNode[0].InnerText.Trim()) : galgameItems.Count;

        Resource = new QiMengResult(galgameItems, pageCount);

        return galgameItems;
    }


    // private static (string href, string title, string synopsis) GetGamePageLink(HtmlNode node, string xPath)
    // {
    //     var titleNode = node.SelectSingleNode(xPath);
    //     var href = titleNode.GetAttributeValue("href", string.Empty).Trim();
    //     var title = titleNode.InnerText.Trim();
    //     var synopsis = node.SelectSingleNode("//div[@class=\"flex-1\"]/div//text()").InnerText.Trim();
    //
    //     return (href, title, synopsis);
    // }
    //
    //
    // private static (DateTime createdTime, int watch, int evaluate, int like) GetHotInfo(HtmlNode node, string xPath)
    // {
    //     var gameHotInfoNode = node.SelectSingleNode(xPath);
    //
    //     var createdTimeStr = gameHotInfoNode.SelectSingleNode("div[2]/span[1]/text()").InnerText.Trim();
    //     // 2025年04月03日
    //     var createdTime = DateTime.ParseExact(createdTimeStr, "yyyy年MM月dd日", CultureInfo.CurrentCulture);
    //
    //     var watch = gameHotInfoNode.SelectSingleNode("div[2]/span[2]/text()").InnerText.Trim();
    //     var evaluate = gameHotInfoNode.SelectSingleNode("div[2]/span[3]/text()").InnerText.Trim();
    //     var like = gameHotInfoNode.SelectSingleNode("div[2]/span[4]/text()").InnerText.Trim();
    //
    //     return (createdTime, int.Parse(watch), int.Parse(evaluate), int.Parse(like));
    // }


    QiMengResult IResourceRoot<QiMengResult>.RequestJsonDeserialize(string json, System.Text.Json.JsonSerializerOptions? options)
    {
        throw new NotSupportedException();
    }
}