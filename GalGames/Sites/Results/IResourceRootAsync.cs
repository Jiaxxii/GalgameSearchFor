using System.Text.Json;

namespace GalgameSearchFor.GalGames.Sites.Results;

public interface IResourceRootAsync<TResourceRoot>
    where TResourceRoot : IValid
{
    TResourceRoot Resource { get; }

    Task<TResourceRoot> RequestJsonDeserializeAsync(Stream stream, JsonSerializerOptions? options, CancellationToken cancellationToken = default);
}