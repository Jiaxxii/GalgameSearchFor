using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GalgameSearchFor.GalGames.Sites.ConstantSettings;
using GalgameSearchFor.GalGames.Sites.RequestTable.ZiLingHome;
using GalgameSearchFor.GalGames.Sites.Results;
using GalgameSearchFor.GalGames.Sites.Results.ZiLingHome;

namespace GalgameSearchFor.GalGames.Sites;

public partial class YanYu : SearcherFormResult<GalgameInfo>
    , ISearchTable<GalgameInfo, SearchTable>
    , IResourceRootAsync<ZiLingResult>
{
    private partial class InternalWriteConsole;

    public YanYu(TimeSpan? timeout = null) : base(new Uri("https://yanyugal.top"), timeout)
    {
        WriteConsole = new InternalWriteConsole(() => Resource, _baseUri);
    }


    private const string SearchUrlPath = "api/fs/search";

    public ZiLingResult Resource { get; private set; }


    public override IWrieConsole WriteConsole { get; }

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
}