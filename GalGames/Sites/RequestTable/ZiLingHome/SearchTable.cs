using System.Text.Json.Serialization;

namespace GalgameSearchFor.GalGames.Sites.RequestTable.ZiLingHome;

public readonly struct SearchTable(
    string keywords,
    int page = 1,
    string parent = "/",
    string password = "",
    int perPage = 100,
    SelectMode selectMode = SelectMode.All)
{
    public string Keywords { get; } = keywords;

    public int Page { get; } = page;

    public string Parent { get; } = parent;

    public string Password { get; } = password;

    public int PerPage { get; } = perPage;

    [JsonPropertyName("scope")] public SelectMode SelectMode { get; } = selectMode;


    public static SearchTable DefaultSearch(string keywords, SelectMode selectMode = SelectMode.All)
    {
        return new SearchTable(keywords, selectMode: selectMode);
    }
}

public enum SelectMode
{
    /// <summary>
    /// 文件和文件夹
    /// </summary>
    All,

    /// <summary>
    /// 文件
    /// </summary>
    File,

    /// <summary>
    /// 文件夹
    /// </summary>
    Folder
}