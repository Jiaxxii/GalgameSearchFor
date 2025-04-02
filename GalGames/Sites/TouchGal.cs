using System.Text.Json;
using GalgameSearchFor.ConsoleStyle.ANSI;
using GalgameSearchFor.GalGames.Platform;
using GalgameSearchFor.GalGames.Sites.ConstantSettings;
using GalgameSearchFor.GalGames.Sites.RequestTable.TouchGalgame;
using GalgameSearchFor.GalGames.Sites.Results;
using GalgameSearchFor.GalGames.Sites.Results.TouchGalgame;

namespace GalgameSearchFor.GalGames.Sites;

public class TouchGal(TimeSpan? timeout = null) : SearcherFormResult<GalgameInfo>(new Uri("https://www.touchgal.io"), timeout)
    , ISearchTable<GalgameInfo, SearchTable>
    , IResourceRootAsync<TouchResult>
{
    private const string SearchHttpPath = "/api/search";


    public TouchResult Resource { get; private set; }

    public override IEnumerable<GalgameInfo> SearchResult(string key)
    {
        var searchTable = SearchTable.DefaultSearch(key.Split(' '));
        return Search(searchTable);
    }

    public override Task<IEnumerable<GalgameInfo>> SearchResultAsync(string key, CancellationToken cancellationToken = default)
    {
        var searchTable = SearchTable.DefaultSearch(key.Split(' '));
        return SearchAsync(searchTable, cancellationToken);
    }


    public IEnumerable<GalgameInfo> Search(SearchTable key)
    {
        throw new NotImplementedException();
    }


    public override async Task WriteConsoleAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        foreach (var galgameInfo in Resource.GalGames)
        {
            await Console.Out.WriteLineAsync($"\uD83C\uDFAE 《\e[1;38;2;255;165;0m{galgameInfo.Name}\e[0m》");
            await Console.Out.WriteLineAsync($"\uD83D\uDD17 下载页面：\e[38;2;96;174;228m\e[4m{new Uri(_baseUri, galgameInfo.UniqueId)}\e[0m");
            await Console.Out.WriteLineAsync($"\uD83D\uDDE3\uFE0F 语言：[{string.Join('、', galgameInfo.Languages.Select(ToStings.LanguageColor))}]");
            await Console.Out.WriteLineAsync($"\uD83D\uDCBB 平台：[{string.Join('、', galgameInfo.Platforms.Select(ToStings.TargetPlatform))}]");
            await Console.Out.WriteLineAsync($"\uD83D\uDCE2 类型：[{string.Join('、', galgameInfo.Type.Select(ToStings.TargetPlatform))}]");
            await Console.Out.WriteLineAsync($"\u2728 游戏标签：[{string.Join('、', galgameInfo.GameTags.Select(tag => $"\e[38;2;76;252;167m{tag}\e[0m"))}]");

            await Console.Out.WriteLineAsync($"\uD83D\uDC40 观看人数：\e[38;2;255;165;0m{galgameInfo.View}\e[0m");
            await Console.Out.WriteLineAsync($"\uD83D\uDE80 下载人数：\e[38;2;255;165;0m{galgameInfo.Download}\e[0m");

            await Console.Out.WriteLineAsync($"\uD83D\uDCAC 评价人数：\e[38;2;255;165;0m{galgameInfo.Temperature.Comment}\e[0m");
            await Console.Out.WriteLineAsync($"\u2B50 收藏人数：\e[38;2;255;165;0m{galgameInfo.Temperature.FavoriteBy}\e[0m");
            await Console.Out.WriteLineAsync($"\uD83D\uDCBE 资源数量：\e[38;2;255;165;0m{galgameInfo.Temperature.Resource}\e[0m");
            Console.WriteLine();
        }

        Console.WriteLine($"\uD83C\uDF10 网站名称：\e[48;2;255;255;0m\e[4;38;2;0;100;255m{_baseUri}\e[0m");
        await Console.Out.WriteLineAsync($"\uD83D\uDD0D 搜索关键字：[ \e[38;2;255;255;0m{string.Join(' ', keys)}\e[0m ]");
        Console.WriteLine($"\uD83D\uDCCA 相关数量：\e[1m{Resource.Total}\e[0m\r\n");
    }


    public async Task<IEnumerable<GalgameInfo>> SearchAsync(SearchTable key, CancellationToken cancellationToken = default)
    {
        await using var bodyStream = await BuilderRequestBodyAsync(key, cancellationToken: cancellationToken);

#if DEBUG
        var streamToStringAsync = await StreamToStringAsync(bodyStream);
#endif

        var searchResult = await RequestSearchResultAsync(bodyStream, cancellationToken);


        return Results = searchResult;
    }

    private async Task<IEnumerable<GalgameInfo>> RequestSearchResultAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var httpResponseMessage = await PostAsync(SearchHttpPath, new StreamContent(stream), cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        await using var readStream = await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);

#if DEBUG
        var streamToStringAsync = await StreamToStringAsync(readStream);
#endif


        Resource = await RequestJsonDeserializeAsync(readStream, cancellationToken: cancellationToken);

        return Resource.GalGames;
    }


    public async Task<TouchResult> RequestJsonDeserializeAsync(Stream stream, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        // stream to string
        // using var streamReader = new StreamReader(stream);
        // var json = await streamReader.ReadToEndAsync(cancellationToken);

        var resourcesResult = await JsonSerializer.DeserializeAsync<TouchResult>(
            stream,
            options ?? JsonSerializerSettings.CamelCaseSerializerOptions,
            cancellationToken);

        if (!resourcesResult.IsValid())
        {
            // TODO: 处理无效数据
            throw new JsonException();
        }

        return resourcesResult;
    }

    public override string ToString() => $"\e[1mTouchGalgame（\e[38;2;96;174;228m\e[4m{_baseUri}）\e[0m";
}