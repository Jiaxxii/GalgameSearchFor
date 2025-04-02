using System.Globalization;
using GalgameSearchFor.ConsoleStyle.ANSI;
using GalgameSearchFor.GalGames.Sites.Results.LiangZi;
using HtmlAgilityPack;

namespace GalgameSearchFor.GalGames.Sites;

public class LiangZiACG(TimeSpan? timeout = null) : HtmlAnalysisSite<GalgameInfo>(new Uri("https://www.lzacg.org/"), timeout)
{
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

    public override async Task WriteConsoleAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        foreach (var galgameInfo in Results)
        {
            await Console.Out.WriteLineAsync($"\uD83C\uDFAE 《\e[1;38;2;255;165;0m{ExtractByName(galgameInfo.Title)}\e[0m》");  // 🎮 游戏手柄
            await Console.Out.WriteLineAsync($"\uD83D\uDCE2 标签：{string.Join(", ", galgameInfo.Tags.Select(ToStings.TargetPlatform))}");  // 📢 喇叭
            await Console.Out.WriteLineAsync($"\uD83D\uDD17 游戏链接：\e[38;2;96;174;228m\e[4m{new Uri(_baseUri, galgameInfo.PageLink).AbsoluteUri}\e[0m");  // 🔗 链接符号
    
            await Console.Out.WriteLineAsync($"\uD83D\uDCC5 资源上传日期：\e[38;2;76;252;246m{galgameInfo.Created:yy-MM-dd HH:mm:ss}\e[0m");  // 📅 日历
            await Console.Out.WriteLineAsync($"\uD83D\uDCAC 评价人数：\e[38;2;255;165;0m{galgameInfo.EvaluationCount}\e[0m");  // 💬 对话气泡
            await Console.Out.WriteLineAsync($"\uD83D\uDC41\uFE0F 观看人数：\e[38;2;255;165;0m{galgameInfo.WatchCount}\e[0m");  // 👁️ 眼睛
            Console.WriteLine();
        }
        
        Console.WriteLine($"\uD83C\uDF10 网站名称：\e[48;2;255;255;0m\e[4;38;2;0;100;255m{_baseUri}\e[0m");  // 🌐 地球图标
        await Console.Out.WriteLineAsync($"\uD83D\uDD0E 搜索关键字：[ \e[38;2;255;255;0m{string.Join(' ', keys)}\e[0m ]");  // 🔍 放大镜
        Console.WriteLine($"\uD83D\uDCCA 相关数量：\e[1m{Results.Count()}\e[0m\r\n");  // 📊 统计图表
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
    
    public override string ToString() => $"\e[1m量子ACG（\e[38;2;96;174;228m\e[4m{_baseUri}）\e[0m";
}