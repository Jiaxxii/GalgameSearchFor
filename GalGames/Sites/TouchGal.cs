using System.Text.Json;
using GalgameSearchFor.GalGames.Sites.ConstantSettings;
using GalgameSearchFor.GalGames.Sites.RequestTable.TouchGalgame;
using GalgameSearchFor.GalGames.Sites.Results;
using GalgameSearchFor.GalGames.Sites.Results.TouchGalgame;

namespace GalgameSearchFor.GalGames.Sites;

public partial class TouchGal : SearcherFormResult<GalgameInfo>
    , ISearchTable<GalgameInfo, SearchTable>
    , IResourceRootAsync<TouchResult>
{
    public TouchGal(TimeSpan? timeout = null) : base(new Uri("https://www.touchgal.io"), timeout)
    {
        WriteConsole = new InternalWriteConsole(() => Resource, _baseUri);
    }

    private partial class InternalWriteConsole;

    private const string SearchHttpPath = "/api/search";


    public TouchResult Resource { get; private set; }

    public override IWrieConsole WriteConsole { get; }

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

#if DEBUG
        var streamToStringAsync = await StreamToStringAsync(stream);
#endif

        var resourcesResult = await JsonSerializer.DeserializeAsync<TouchResult>(
            stream,
            options ?? JsonSerializerSettings.CamelCaseSerializerOptions,
            cancellationToken);

        // if (resourcesResult is null)
        // {
        //     // TODO: 处理无效数据
        //     throw new JsonException();
        // }

        return resourcesResult;
    }
}