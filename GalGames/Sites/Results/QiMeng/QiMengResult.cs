using System.Text.Json.Serialization;

namespace GalgameSearchFor.GalGames.Sites.Results.QiMeng;

[method: JsonConstructor]
public class QiMengResult(IReadOnlyList<GalgameItem> results, int pageCount) : IValid
{
    public IReadOnlyList<GalgameItem> Results { get; set; } = results;

    public int PageCount { get; } = pageCount;


    public bool IsValid() => true;
}

[method: JsonConstructor]
public readonly struct GalgameItem(string gamePageLink, string titleName, string imageUrl)
{
    public string GamePageLink { get; } = gamePageLink;

    public string TitleName { get; } = titleName;

    // public string Synopsis { get; } = synopsis;

    // public IEnumerable<string> ImageUrls { get; } = imageUrls;
    public string ImageUrl { get; } = imageUrl;

    // public string Type { get; } = type;
    //
    // public DateTime CreatedTime { get; } = createdTime;
    //
    // public HotInfo HotInfo { get; } = hotInfo;


    public IEnumerable<string> SplitGameName(char separator = '/') => TitleName.Split(separator);
}

// [method: JsonConstructor]
// public readonly struct HotInfo(int watch, int evaluate, int like)
// {
//     public int Watch { get; } = watch;
//     public int Evaluate { get; } = evaluate;
//     public int Like { get; } = like;
// }