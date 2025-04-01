using System.Text.Json;

namespace GalgameSearchFor.GalGames.Sites.Results;

public interface IResourceRoot<out TResourceRoot> where TResourceRoot : IValid
{
    TResourceRoot Resource { get; }

    TResourceRoot RequestJsonDeserialize(string json, JsonSerializerOptions? options = null);
}