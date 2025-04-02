using System.Text.Json;

namespace GalgameSearchFor.GalGames.Sites.Results;

public interface IResourceRootAsync<TResourceRoot>
    where TResourceRoot : IValid
{
    // 资源在调用 RequestJsonDeserializeAsync 之前可能为空
    TResourceRoot? Resource { get; }


    Task<TResourceRoot> RequestJsonDeserializeAsync(Stream stream, JsonSerializerOptions? options, CancellationToken cancellationToken = default);
}