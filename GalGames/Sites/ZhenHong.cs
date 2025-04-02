using GalgameSearchFor.GalGames.Sites.Results.ZhenHong;
using HtmlAgilityPack;

namespace GalgameSearchFor.GalGames.Sites;

public class ZhenHong(TimeSpan? timeout = null) : HtmlAnalysisSite<GalgameInfo>(new Uri("https://www.shinnku.com"), timeout)
{
    private const string SearchUrlPath = "/search";
    // private const string WikiUrlPath = "api/wiki?name=";

    private const string ResourcesListRootXPath = "//div[@class='p-4']";

    private readonly HtmlDocument _htmlDocument = new();


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

    public override async Task WriteConsoleAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        foreach (var galgameInfo in Results)
        {
            await Console.Out.WriteLineAsync($"\uD83C\uDFAE 《\e[1;38;2;255;165;0m{galgameInfo.Name}\e[0m》"); // 游戏手柄表情
            // await Console.Out.WriteLineAsync($"\uD83D\uDCC2 路径: \e[38;2;99;99;99m\e[4m{Uri.UnescapeDataString(galgameInfo.Path)}\e[0m"); // 文件夹表情
            await Console.Out.WriteLineAsync($"\uD83D\uDD17 游戏链接：\e[38;2;96;174;228m\e[4m{Uri.UnescapeDataString(galgameInfo.DownloadUrl)}\e[0m"); // 链接符号表情
            await Console.Out.WriteLineAsync($"\uD83D\uDCC8 大小：\e[38;2;255;165;0m{galgameInfo.Size}\e[0m"); // 上升图表表情

            Console.WriteLine();
        }

        Console.WriteLine($"\u2600\uFE0F 网站名称：\e[48;2;255;255;0m\e[4;38;2;0;100;255m{_baseUri}\e[0m"); // 太阳表情
        await Console.Out.WriteLineAsync($"\uD83D\uDD0E 搜索关键字：[ \e[38;2;255;255;0m{string.Join(' ', keys)}\e[0m ]"); // 放大镜表情
        Console.WriteLine($"\uD83D\uDCCA 相关数量：\e[1m{Results.Count()}\e[0m\r\n"); // 统计图表表情
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

    public override string ToString() => $"\e[1m真红小站（\e[38;2;96;174;228m\e[4m{_baseUri}）\e[0m";
}