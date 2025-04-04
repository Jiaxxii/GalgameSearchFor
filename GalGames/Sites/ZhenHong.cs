using GalgameSearchFor.GalGames.Sites.Results.ZhenHong;
using HtmlAgilityPack;

namespace GalgameSearchFor.GalGames.Sites;

public partial class ZhenHong : HtmlAnalysisSite<GalgameInfo>
{
    public ZhenHong(TimeSpan? timeout = null) : base(new Uri("https://www.shinnku.com"), timeout)
    {
        WriteConsole = new InternalWriteConsole(()=>Results, _baseUri, () => ShowCount);
    }

    private partial class InternalWriteConsole;

    private const string SearchUrlPath = "/search";
    // private const string WikiUrlPath = "api/wiki?name=";

    private const string ResourcesListRootXPath = "//div[@class='p-4']";

    private readonly HtmlDocument _htmlDocument = new();

    public int ShowCount { get; set; } = 10;


    public override IWrieConsole WriteConsole { get; }

    public override IEnumerable<GalgameInfo> SearchResult(string key)
    {
        throw new NotImplementedException();
    }

    public override async Task<IEnumerable<GalgameInfo>> SearchResultAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        var responseMessage = await GetAsync($"{SearchUrlPath}?q={key}", cancellationToken);

        responseMessage.EnsureSuccessStatusCode();

        var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        return Results = AnalysisHtml(ref content);
    }
    

    protected override List<GalgameInfo> AnalysisHtml(ref string html)
    {
        _htmlDocument.LoadHtml(html);

        var gameListCollection = _htmlDocument.DocumentNode.SelectNodes(ResourcesListRootXPath);


        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (gameListCollection == null || gameListCollection.Count == 0) return [];

        var result = new List<GalgameInfo>();
        foreach (var gameNode in gameListCollection)
        {
            var gameName = gameNode.SelectSingleNode("//p[@class='text-lg']/text()").InnerText;

            // var pathName = gameNode.SelectSingleNode("div/div[2]//p[1]/text()[1]").InnerText;
            var path = gameNode.SelectSingleNode("div/div[2]//p[1]/text()[2]").InnerText;

            // var sizeName = gameNode.SelectSingleNode("div/div[2]//p[2]/text()[1]").InnerText;
            var size = gameNode.SelectSingleNode("div/div[2]//p[2]/text()[2]").InnerText;

            var downloadUrl = gameNode.SelectSingleNode("div/div[3]/a").GetAttributeValue("href", string.Empty);

            var gameInfo = new GalgameInfo(gameName, path, size, downloadUrl);
            result.Add(gameInfo);
        }


        return result;
    }

   
}