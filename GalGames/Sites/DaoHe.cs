using System.Text.Json;
using GalgameSearchFor.ConsoleStyle.ANSI;
using GalgameSearchFor.GalGames.Sites.ConstantSettings;
using GalgameSearchFor.GalGames.Sites.Results;
using GalgameSearchFor.GalGames.Sites.Results.DaoHe;

namespace GalgameSearchFor.GalGames.Sites;

public sealed class DaoHe(TimeSpan? timeout = null) : SearcherFormResult<GameInfo>(new Uri("https://amoebi.com"), timeout)
    , IResourceRootAsync<DaoHeResult>
{
    private const string ResourceListPath = "/list";

    // private bool _isRequestGameInfoList;
    private bool _isRequestComplete;

    public DaoHeResult? Resource { get; private set; }


    public override IEnumerable<GameInfo> SearchResult(string key)
    {
        if (Resource == null)
        {
            throw new NullReferenceException($"[没有如何资源，请调用{nameof(SearchResultAsync)}]方法获取！");
        }

        if (!_isRequestComplete)
        {
            // 如果正确的 await SearchResultAsync(string,CancellationToken) 方法不会触发此异常
            throw new NullReferenceException($"[确保等待{nameof(SearchResultAsync)}]方法后调用！");
        }

        return Results = Resource.GameInfos.Where(info => info.GameNameContains(key));
    }

    public override async Task<IEnumerable<GameInfo>> SearchResultAsync(string key, CancellationToken cancellationToken = default)
    {
        if (Resource == null)
        {
            _ = await RequestGameInfoList(cancellationToken);
        }

        return SearchResult(key);
    }

    public override async Task WriteConsoleAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        foreach (var galgameInfo in Results)
        {
            await Console.Out.WriteLineAsync($"\uD83C\uDFAE 《\e[1;38;2;255;165;0m{galgameInfo.ShowName}\e[0m》（\e[1;38;2;255;165;0m{galgameInfo.RawName}\e[0m）"); // 游戏手柄
            await Console.Out.WriteLineAsync(
                $"\uD83D\uDDBC 类型：\e[38;2;229;129;123m{galgameInfo.GameType}\e[0m \uD83D\uDCAC 介绍：{new Uri(_baseUri, galgameInfo.GameInfoUrl).AbsoluteUri}"); // 画板+对话框
            foreach (var downloadPair in galgameInfo.Downloads)
            {
                await Console.Out.WriteLineAsync($"\uD83D\uDCBB 平台：{ToStings.TargetPlatform(downloadPair.Key)}"); // 笔记本电脑
                foreach (var downloadPath in downloadPair.Value)
                {
                    await Console.Out.WriteLineAsync($"\uD83D\uDD17 下载地址：\e[38;2;96;174;228m\e[4m{downloadPath.Url}\e[0m"); // 链接符号
                }
            }

            Console.WriteLine();
        }


        Console.WriteLine($"\uD83C\uDF10 网站名称：\e[48;2;255;255;0m\e[4;38;2;0;100;255m{_baseUri}\e[0m"); // 地球图标
        await Console.Out.WriteLineAsync($"\uD83D\uDD0D 搜索关键字：[ \e[38;2;255;255;0m{string.Join(' ', keys)}\e[0m ]"); // 放大镜图标
        Console.WriteLine($"\uD83D\uDCC8 相关数量：\e[1m{Results.Count()}\e[0m\r\n"); // 上升图表
    }


    private async Task<IEnumerable<GameInfo>> RequestGameInfoList(CancellationToken cancellationToken = default)
    {
        if (Resource != null)
        {
            Console.WriteLine("\e[38;2;255;255;0m资源列表已请求，本次请求将刷新资源！\e[0m");
        }

        _isRequestComplete = false;
        try
        {
            using var httpResponseMessage = await GetAsync(ResourceListPath, cancellationToken);

            httpResponseMessage.EnsureSuccessStatusCode();

            var readStream = await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);

            Resource = await RequestJsonDeserializeAsync(readStream, cancellationToken: cancellationToken);
            _isRequestComplete = true;

            return Resource.GameInfos;
        }
        catch (TaskCanceledException)
        {
            _isRequestComplete = false;
            throw;
        }
    }


    public async Task<DaoHeResult> RequestJsonDeserializeAsync(Stream stream, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        var resourcesResult = await JsonSerializer.DeserializeAsync<DaoHeResult>(stream, JsonSerializerSettings.CamelCaseSerializerOptions, cancellationToken);

        if (resourcesResult is null || !resourcesResult.IsValid())
        {
            // TODO : 处理无效数据
            throw new JsonException();
        }

        return resourcesResult;
    }


    public override string ToString() => $"\e[1m蹈荷（\e[38;2;96;174;228m\e[4m{_baseUri}）\e[0m";
}