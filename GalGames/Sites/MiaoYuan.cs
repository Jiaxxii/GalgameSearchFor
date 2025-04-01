using GalgameSearchFor.ConsoleStyle.ANSI;
using GalgameSearchFor.GalGames.Sites.Results.MiaoYuan;
using HtmlAgilityPack;

namespace GalgameSearchFor.GalGames.Sites;

public class MiaoYuan(TimeSpan? timeout = null) : HtmlAnalysisSite<GalgameInfo>(new Uri("https://www.nekotaku.me"), timeout)
{
    private const string SearchUriPath = "?s={NAME}&type=post";


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

    public override async Task WriteConsoleAsync(IEnumerable<string> keys, int? millisecondsDelay = null, CancellationToken cancellationToken = default)
    {
        foreach (var galgameInfo in Results)
        {
            await Console.Out.WriteLineAsync($"\uD83C\uDFAE {string.Join('、', galgameInfo.SplitName().Select(name => $"《\e[1;38;2;255;165;0m{name}\e[0m》"))}"); // 🎮 游戏手柄
            await Console.Out.WriteLineAsync($"\uD83D\uDD17 游戏链接：\e[38;2;96;174;228m\e[4m{new Uri(_baseUri, galgameInfo.PageLink).AbsoluteUri}\e[0m"); // 🔗 链接符号
            await Console.Out.WriteLineAsync($"\uD83D\uDCE2 标签：{string.Join(", ", galgameInfo.Tags.Select(ToStings.TargetPlatform))}"); // 📢 喇叭

            await Console.Out.WriteLineAsync($"\uD83D\uDC64 上传作者：\e[38;2;76;252;246m{galgameInfo.Author.Name}\e[0m"); // 👤 人像
            await Console.Out.WriteLineAsync($"\uD83C\uDFE0 作者主页：\e[38;2;96;174;228m\e[4m{galgameInfo.Author.Link}\e[0m"); // 🏠 房屋

            await Console.Out.WriteLineAsync($"\uD83D\uDCAC 评价人数：\e[38;2;255;165;0m{galgameInfo.HotInfo.EvaluateCount}\e[0m \u2605"); // 💬 对话气泡+★
            await Console.Out.WriteLineAsync($"\uD83D\uDC41\uFE0F 观看人数：\e[38;2;255;165;0m{galgameInfo.HotInfo.WatchCount}\e[0m \uD83D\uDC40"); // 👁️ 眼睛
            await Console.Out.WriteLineAsync($"\u2764\uFE0F 收藏人数：\e[38;2;255;165;0m{galgameInfo.HotInfo.LikeCount}\e[0m \u2605"); // ❤️ 爱心+★

            Console.WriteLine();
        }

        Console.WriteLine($"\uD83C\uDF10 网站名称：\e[48;2;255;255;0m\e[4;38;2;0;100;255m{_baseUri}\e[0m"); // 🌐 地球图标
        await Console.Out.WriteLineAsync($"\uD83D\uDD0E 搜索关键字：[ \e[38;2;255;255;0m{string.Join(' ', keys)}\e[0m ]"); // 🔍 放大镜
        Console.WriteLine($"\uD83D\uDCCA 相关数量：\e[1m{Results.Count()}\e[0m\r\n"); // 📊 统计图表
    }

    protected override List<GalgameInfo> AnalysisHtml(ref string html)
    {
        _document.LoadHtml(html);

        var postsHtmlNodeCollection = _document.DocumentNode.SelectNodes("//div[contains(@class,'search-content')]/posts");

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

    public override string ToString() => $"\e[1m喵源领域（\e[38;2;96;174;228m\e[4m{_baseUri}）\e[0m";
}