using System.Text.Json.Serialization;

namespace GalgameSearchFor.GalGames.Sites.RequestTable.TouchGalgame;

public readonly struct SearchTable(IEnumerable<string> query, SearchOption searchOption, int page = 1, int limit = 10)
{
    public IEnumerable<string> Query { get; } = query;

    public int Page { get; } = page;

    public int Limit { get; } = limit;

    public SearchOption SearchOption { get; } = searchOption;

    public static SearchTable DefaultSearch(params IEnumerable<string> query)
    {
        return new SearchTable(query, SearchOption.Default, page: 1, limit: 10);
    }
}

public readonly struct SearchOption(bool inIntroduction, bool inAlias, bool inTag)
{
    public SearchOption() : this(false, true, false)
    {
    }


    [JsonPropertyName("searchInIntroduction")]
    public bool InIntroduction { get; } = inIntroduction;

    [JsonPropertyName("searchInAlias")] public bool InAlias { get; } = inAlias;

    [JsonPropertyName("searchInTag")] public bool InTag { get; } = inTag;


    public static SearchOption Default { get; } = new();
}