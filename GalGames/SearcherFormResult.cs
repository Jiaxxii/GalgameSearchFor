using System.Text.Json;
using GalgameSearchFor.GalGames.Sites;
using GalgameSearchFor.GalGames.Sites.ConstantSettings;

namespace GalgameSearchFor.GalGames;

public abstract class SearcherFormResult<TResult>(Uri baseUri, TimeSpan? timeout = null) :
    ISearcherFormResult<TResult>,
    ISearcher,
    IRequestResult<TResult>,
    IWrieConsole
{
    private readonly HttpClient _singleClient = MainHttpClient.GetClient();
    public TimeSpan? Timeout { get; set; } = timeout;

    public IEnumerable<TResult> Results { get; protected set; } = [];


    protected readonly Uri _baseUri = baseUri;

    public abstract IWrieConsole WriteConsole { get; }


    /// <summary>
    /// 发起 HTTP POST 请求
    /// </summary>
    /// <param name="urlPath">相对路径</param>
    /// <param name="content">请求体</param>
    /// <param name="cancellationToken">用于任务取消 （*如果有超时需求请调用成员 <see cref="Timeout"/> 来更改）</param>
    /// <exception cref="TaskCanceledException">主要异常：请求超时、任务取消</exception>
    /// <returns></returns>
    protected virtual Task<HttpResponseMessage> PostAsync(string urlPath, HttpContent content, CancellationToken cancellationToken = default)
    {
        var uri = new Uri(_baseUri, urlPath);
        return ExecuteAsync(token => _singleClient.PostAsync(uri, content, token), cancellationToken);
    }

    /// <summary>
    /// 发起 HTTP GET 请求
    /// </summary>
    /// <param name="urlPath">相对路径</param>
    /// <param name="cancellationToken">用于任务取消 （*如果有超时需求请调用成员 <see cref="Timeout"/> 来更改）</param>
    /// <exception cref="TaskCanceledException">主要异常：请求超时、任务取消</exception>
    /// <returns></returns>
    protected virtual Task<HttpResponseMessage> GetAsync(string urlPath, CancellationToken cancellationToken = default)
    {
        var uri = new Uri(_baseUri, urlPath);
        return ExecuteAsync(token => _singleClient.GetAsync(uri, token), cancellationToken);
    }


    private Task<HttpResponseMessage> ExecuteAsync(Func<CancellationToken, Task<HttpResponseMessage>> requestFactory, CancellationToken cancellationToken)
    {
        if (Timeout == null)
        {
#if DEBUG
            if (cancellationToken.IsCancellationRequested)
            {
                // TODO : 
                throw new Exception();
            }
#endif
            return requestFactory.Invoke(cancellationToken);
        }

#if DEBUG
        if (Timeout.Value.TotalSeconds <= 10F || cancellationToken.IsCancellationRequested)
        {
            // TODO : 
            throw new Exception();
        }
#endif

        using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource(Timeout.Value).Token, cancellationToken);
        return requestFactory(linkedTokenSource.Token);
    }

    public abstract IEnumerable<TResult> SearchResult(string key);
    public virtual void Search(string key) => _ = SearchResult(key);

    public abstract Task<IEnumerable<TResult>> SearchResultAsync(string key, CancellationToken cancellationToken = default);

    public virtual Task SearchAsync(string key, CancellationToken cancellationToken) => _ = SearchResultAsync(key, cancellationToken);


    public async Task WriteConsoleAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        await WriteConsole.WriteConsoleAsync(keys, cancellationToken);
    }

    protected static async Task<Stream> BuilderRequestBodyAsync<TTable>(TTable searchTable, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, searchTable, options ?? JsonSerializerSettings.TouchGalBuilderSerializerOptions, cancellationToken);
        stream.Position = 0;

        return stream;
    }

#if DEBUG
    protected static async Task<string> StreamToStringAsync(Stream stream)
    {
        var readToEndAsync = await new StreamReader(stream).ReadToEndAsync();
        stream.Position = 0;
        return readToEndAsync;
    }
#endif

    public override string ToString() => WriteConsole.ToString() ?? $"\e[38;2;96;174;228m\e[4m{_baseUri}\e[0m";
}