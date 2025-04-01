using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GalgameSearchFor.GalGames.Platform;
using GalgameSearchFor.GalGames.Sites.ConstantSettings;
using GalgameSearchFor.GalGames.Sites.RequestTable.ZiLingHome;
using GalgameSearchFor.GalGames.Sites.Results;
using GalgameSearchFor.GalGames.Sites.Results.ZiLingHome;

namespace GalgameSearchFor.GalGames.Sites;

public class ZiLing(TimeSpan? timeout = null) : SearcherFormResult<GalgameInfo>(new Uri("https://zi0.cc"), timeout)
    , ISearchTable<GalgameInfo, SearchTable>
    , IResourceRootAsync<ZiLingResult>
{
    private const string SearchUrlPath = "api/fs/search";

    public ZiLingResult Resource { get; private set; }


    public override IEnumerable<GalgameInfo> SearchResult(string key)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<GalgameInfo>> SearchResultAsync(string key, CancellationToken cancellationToken = default)
    {
        var searchTable = SearchTable.DefaultSearch(key);
        return SearchAsync(searchTable, cancellationToken);
    }

    public IEnumerable<GalgameInfo> Search(SearchTable key)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<GalgameInfo>> SearchAsync(SearchTable key, CancellationToken cancellationToken = default)
    {
        await using var bodyStream = await BuilderRequestBodyAsync(key, JsonSerializerSettings.SnakeCaseLowerSerializerOptions, cancellationToken);

        return Results = await RequestSearchResultAsync(bodyStream, cancellationToken);
    }


    public async Task<ZiLingResult> RequestJsonDeserializeAsync(Stream stream, JsonSerializerOptions? options, CancellationToken cancellationToken = default)
    {
        var result = await JsonSerializer.DeserializeAsync<ZiLingResult>(stream, options, cancellationToken);

        if (!result.IsValid())
        {
            // TODO: 处理异常
            throw new JsonException();
        }

        return result;
    }

    public override async Task WriteConsoleAsync(IEnumerable<string> keys, int? millisecondsDelay = null, CancellationToken cancellationToken = default)
    {
        foreach (var galgameInfo in Resource.Data.Content)
        {
            await Console.Out.WriteLineAsync($"\uD83C\uDFAE 《\e[1;38;2;255;165;0m{galgameInfo.Name}\e[0m》"); // 游戏手柄表情

            var downloadUrl = new Uri(_baseUri, $"{Uri.EscapeDataString(galgameInfo.Parent)}/{galgameInfo.Name}?form=search");
            await Console.Out.WriteLineAsync($"\uD83D\uDD17 游戏链接：\e[38;2;96;174;228m\e[4m{downloadUrl}\e[0m"); // 链接符号表情

            await Console.Out.WriteLineAsync($"\uD83D\uDCC8 大小：\e[38;2;255;165;0m{galgameInfo.GetSizeString()}\e[0m"); // 上升图表表情

            await Console.Out.WriteLineAsync($"\uD83D\uDCC0 是否是文件夹：\e[38;2;255;165;0m{(galgameInfo.IsDir ? '是' : '否')}\e[0m"); // 文件夹表情
            Console.WriteLine();
        }

        Console.WriteLine($"\u2600\uFE0F 网站名称：\e[48;2;255;255;0m\e[4;38;2;0;100;255m{_baseUri}\e[0m"); // 太阳表情
        await Console.Out.WriteLineAsync($"\uD83D\uDD0E 搜索关键字：[ \e[38;2;255;255;0m{string.Join(' ', keys)}\e[0m ]"); // 放大镜表情
        Console.WriteLine($"\uD83D\uDCCA 相关数量：\e[1m{Resource.Data.Total}\e[0m\r\n"); // 统计图表表情
    }


    private async Task<IEnumerable<GalgameInfo>> RequestSearchResultAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json")
        {
            CharSet = Encoding.UTF8.WebName
        };

        using var httpResponseMessage = await PostAsync(SearchUrlPath, streamContent, cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        var readStream = await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);

        Resource = await RequestJsonDeserializeAsync(readStream, JsonSerializerSettings.SnakeCaseLowerSerializerOptions, cancellationToken: cancellationToken);

        return Resource.Data.Content;
    }

    public override string ToString() => $"\e[1m梓澪の妙妙屋（\e[38;2;96;174;228m\e[4m{_baseUri}）\e[0m";
}