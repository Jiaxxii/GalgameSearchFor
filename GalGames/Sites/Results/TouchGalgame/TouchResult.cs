using System.Text.Json.Serialization;

namespace GalgameSearchFor.GalGames.Sites.Results.TouchGalgame;

[method: JsonConstructor]
public readonly struct TouchResult(IEnumerable<GalgameInfo> galGames, int total) : IValid
{
    [JsonPropertyName("galgames")] public IEnumerable<GalgameInfo> GalGames { get; } = galGames;

    public int Total { get; } = total;


    public bool IsValid() => GalGames is not null && GalGames.Any();
}

[method: JsonConstructor]
public readonly struct GalgameInfo(
    int id,
    string name,
    string bannerUrl,
    int view,
    int download,
    IEnumerable<string> type,
    IEnumerable<string> languages,
    IEnumerable<string> platforms,
    string created,
    IEnumerable<TagItem> tags,
    Temperature temperature,
    IEnumerable<string> gameTags,
    string uniqueId)
{
    public int Id { get; } = id;
    public string Name { get; } = name;

    [JsonPropertyName("banner")] public string BannerUrl { get; } = bannerUrl;

    public int View { get; } = view;

    public int Download { get; } = download;

    public IEnumerable<string> Type { get; } = type;

    [JsonPropertyName("language")] public IEnumerable<string> Languages { get; } = languages;

    [JsonPropertyName("platform")] public IEnumerable<string> Platforms { get; } = platforms;

    public string Created { get; } = created;

    [JsonPropertyName("tag")] public IEnumerable<TagItem> Tags { get; } = tags;

    [JsonPropertyName("_count")] public Temperature Temperature { get; } = temperature;

    [JsonPropertyName("tags")] public IEnumerable<string> GameTags { get; } = gameTags;

    public string UniqueId { get; } = uniqueId;
}

[method: JsonConstructor]
public readonly struct TagItem(Tag tag)
{
    public Tag Tag { get; } = tag;
}

[method: JsonConstructor]
public readonly struct Tag(string name)
{
    public string Name { get; } = name;
}

[method: JsonConstructor]
public readonly struct Temperature(int favoriteBy, int resource, int comment)
{
    [JsonPropertyName("favorite_by")] public int FavoriteBy { get; } = favoriteBy;
    public int Resource { get; } = resource;
    public int Comment { get; } = comment;
}