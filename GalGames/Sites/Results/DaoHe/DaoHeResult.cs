using System.Text.Json.Serialization;

namespace GalgameSearchFor.GalGames.Sites.Results.DaoHe;

[method: JsonConstructor]
public readonly struct DaoHeResult(IEnumerable<GameInfo> gameInfos) : IValid
{
    [JsonPropertyName("items")] public IEnumerable<GameInfo> GameInfos { get; } = gameInfos;

    public bool IsValid() => GameInfos is not null && GameInfos.Any();
}

[method: JsonConstructor]
public readonly struct GameInfo(string thumbLink, string showName, string rawName, string gameType, string gameInfoUrl, Dictionary<string, DownLoadPath[]> downloads, string ost)
{
    // /// <summary>
    // /// 通过 BaseUrl/info/{Id.SubString(4)} 获取游戏介绍
    // /// </summary>
    // public string Id { get; }

    /// <summary>
    /// 游戏封面
    /// </summary>
    public string ThumbLink { get; } = thumbLink;

    /// <summary>
    /// 中文名称
    /// </summary>
    [JsonPropertyName("alt")]
    public string ShowName { get; } = showName;

    /// <summary>
    /// 原名称 （日文）
    /// </summary>
    [JsonPropertyName("alt-jps")]
    public string RawName { get; } = rawName;

    /// <summary>
    /// 游戏类型
    /// </summary>
    [JsonPropertyName("cop")]
    public string GameType { get; } = gameType;

    /// <summary>
    /// 游戏信息 BaseUrl/{GameInfoUrl}
    /// </summary>
    [JsonPropertyName("postUrl")]
    public string GameInfoUrl { get; } = gameInfoUrl;

    // public string Ai { get; }

    public Dictionary<string, DownLoadPath[]> Downloads { get; } = downloads;


    /// <summary>
    /// 外部链接
    /// </summary>
    public string Ost { get; } = ost;


    public bool GameNameContains(string key) => ShowName.Contains(key, StringComparison.OrdinalIgnoreCase) || RawName.Contains(key, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() => HashCode.Combine(ThumbLink, RawName, GameType, GameInfoUrl, Downloads, Ost);
}

[method: JsonConstructor]
public readonly struct DownLoadPath(string url, string loadName)
{
    public string Url { get; } = url;

    [JsonPropertyName("loadname")] public string LoadName { get; } = loadName;

    public override int GetHashCode() => HashCode.Combine(Url, LoadName);
}