using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GalgameSearchFor.GalGames.Sites.RequestTable.TouchGalgame;

public readonly struct SearchTable(
    string query,
    SearchOption searchOption,
    IEnumerable<string> selectedYears,
    SelectedLanguage selectedLanguage = SelectedLanguage.All,
    SelectedPlatform selectedPlatform = SelectedPlatform.All,
    SelectedType selectedType = SelectedType.All,
    SortField sortField = SortField.ResourceUpdateTime,
    SortOrder sortOrder = SortOrder.Desc,
    int page = 1,
    int limit = 10)
{
    [JsonPropertyName("queryString")] public string Query { get; } = query;

    public int Page { get; } = page;

    public int Limit { get; } = limit;

    public SearchOption SearchOption { get; } = searchOption;

    public SelectedLanguage SelectedLanguage { get; } = selectedLanguage;

    public SelectedPlatform SelectedPlatform { get; } = selectedPlatform;

    public SelectedType SelectedType { get; } = selectedType;

    public IEnumerable<string> SelectedYears { get; } = selectedYears;

    public SortField SortField { get; } = sortField;

    public SortOrder SortOrder { get; } = sortOrder;

    /// <summary>
    /// [2025年4月2日14:33:23] 目前官网不支持
    /// </summary>
    public IEnumerable<string> SelectedMonths { get; } = ["all"];

    public static IEnumerable<string> AllYears => ["all"];
    public static IEnumerable<string> ContainsUnknown(IEnumerable<int> year) => year.Select(x => x.ToString()).Append("unknown");

    public static SearchTable DefaultSearch(params IEnumerable<string> query)
    {
        return new SearchTable(CreatedQuery(query), SearchOption.Default, AllYears);
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = false, // 禁用缩进
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 保持中文原样输出
    };


    public static string CreatedQuery(IEnumerable<string> keys)
    {
        var keywordList = new List<Dictionary<string, string>>();

        foreach (var query in keys)
        {
            keywordList.Add(new Dictionary<string, string>
            {
                ["type"] = "keyword",
                ["name"] = query
            });
        }

        return JsonSerializer.Serialize(keywordList, SerializerOptions);
    }
}

public readonly struct SearchOption(bool inIntroduction, bool inAlias, bool inTag)
{
    public SearchOption() : this(false, true, true)
    {
    }


    [JsonPropertyName("searchInIntroduction")]
    public bool InIntroduction { get; } = inIntroduction;

    [JsonPropertyName("searchInAlias")] public bool InAlias { get; } = inAlias;

    [JsonPropertyName("searchInTag")] public bool InTag { get; } = inTag;


    public static SearchOption Default { get; } = new();
}

public enum SelectedLanguage
{
    [JsonStringEnumMemberName("all")] All,
    [JsonStringEnumMemberName("zh-Hans")] ZhHans,
    [JsonStringEnumMemberName("zh-Hant")] ZhHant,
    [JsonStringEnumMemberName("ja")] Ja,
    [JsonStringEnumMemberName("en")] En,
    [JsonStringEnumMemberName("other")] Other
}

public enum SelectedPlatform
{
    [JsonStringEnumMemberName("all")] All,
    [JsonStringEnumMemberName("windows")] Windows,
    [JsonStringEnumMemberName("android")] Android,
    [JsonStringEnumMemberName("ios")] Ios,
    [JsonStringEnumMemberName("macos")] MacOs,
    [JsonStringEnumMemberName("linux")] Linux,
    [JsonStringEnumMemberName("other")] Other,
}

public enum SelectedType
{
    [JsonStringEnumMemberName("all")] All,
    [JsonStringEnumMemberName("pc")] Pc,
    [JsonStringEnumMemberName("chinese")] Chinese,
    [JsonStringEnumMemberName("mobile")] Mobile,
    [JsonStringEnumMemberName("emulator")] Emulator,
    [JsonStringEnumMemberName("row")] Row,
    [JsonStringEnumMemberName("app")] App,
    [JsonStringEnumMemberName("patch")] Patch,
    [JsonStringEnumMemberName("tool")] Tool,
    [JsonStringEnumMemberName("notice")] Notice,
    [JsonStringEnumMemberName("other")] Other,
}

public enum SortField
{
    // // "resource_update_time", "created", "view", "download", "favorite"
    [JsonStringEnumMemberName("resource_update_time")]
    ResourceUpdateTime,
    [JsonStringEnumMemberName("created")] Created,
    [JsonStringEnumMemberName("view")] View,
    [JsonStringEnumMemberName("download")] Download,
    [JsonStringEnumMemberName("favorite")] Favorite,
}

public enum SortOrder
{
    [JsonStringEnumMemberName("asc")] Asc,
    [JsonStringEnumMemberName("desc")] Desc
}