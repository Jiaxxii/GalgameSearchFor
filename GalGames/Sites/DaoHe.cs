using System.Text.Json;
using GalgameSearchFor.GalGames.Sites.ConstantSettings;
using GalgameSearchFor.GalGames.Sites.Results;
using GalgameSearchFor.GalGames.Sites.Results.DaoHe;

namespace GalgameSearchFor.GalGames.Sites;

public sealed partial class DaoHe : SearcherFormResult<GameInfo>, IResourceRootAsync<DaoHeResult>
{
    private partial class InternalWriteConsole;

    public DaoHe(TimeSpan? timeout = null) : base(new Uri("https://amoebi.com"), timeout)
    {
        WriteConsole = new InternalWriteConsole(Results, _baseUri);
    }

    private const string ResourceListPath = "/list";

    // private bool _isRequestGameInfoList;
    private bool _isRequestComplete;

    public DaoHeResult? Resource { get; private set; }


    public override IWrieConsole WriteConsole { get; }

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


    private async Task<IEnumerable<GameInfo>> RequestGameInfoList(CancellationToken cancellationToken = default)
    {
        if (Resource != null)
        {
            Console.WriteLine("\e[38;2;255;255;0m资源列表已请求，本次请求将刷新资源！\e[0m");
        }

        _isRequestComplete = false;
        Stream? stream = null;
        try
        {
            using var httpResponseMessage = await GetAsync(ResourceListPath, cancellationToken);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                stream = await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);
                await WriteLocalJsonFileAsync(stream, "DaoHeResult.json", cancellationToken);
                stream.Position = 0;
            }
            else
            {
                Console.WriteLine("\e[38;2;255;0;0m请求被拒绝，将使用本地数据!\e[0m");
                stream = ReadLocalJsonFileAsync("DaoHeResult.json");
#if DEBUG
                var streamToStringAsync = await StreamToStringAsync(stream);
#endif
            }

            Resource = await RequestJsonDeserializeAsync(stream, cancellationToken: cancellationToken);
            _isRequestComplete = true;


            return Resource.GameInfos;
        }
        catch (TaskCanceledException)
        {
            _isRequestComplete = false;
            throw;
        }
        finally
        {
            if (stream != null)
                await stream.DisposeAsync();
        }
    }

    private static Task WriteLocalJsonFileAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
        Directory.CreateDirectory(AppContext.BaseDirectory);

        Console.WriteLine($"本地存储：\e[38;2;96;174;228m\e[4m{filePath}\e[0m");
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        return stream.CopyToAsync(fileStream, cancellationToken);
    }


    private static FileStream ReadLocalJsonFileAsync(string fileName)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
        Console.WriteLine($"本地存储：\e[38;2;96;174;228m\e[4m{filePath}\e[0m");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"[文件不存在] {filePath}");

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);

        if (fileStream.Length == 0)
            throw new FileLoadException($"[文件内容为空] {filePath}");

        return fileStream;
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
}