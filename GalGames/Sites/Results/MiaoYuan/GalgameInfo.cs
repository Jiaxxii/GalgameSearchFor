namespace GalgameSearchFor.GalGames.Sites.Results.MiaoYuan;

public readonly struct GalgameInfo(string imageUrl, string pageLink, string title, IEnumerable<string> tags, HotInfo hotInfo)
{
    public string ImageUrl { get; } = imageUrl;
    public string PageLink { get; } = pageLink;
    public string Title { get; } = title;
    public IEnumerable<string> Tags { get; } = tags;

    // public Author Author { get; } = author;

    public HotInfo HotInfo { get; } = hotInfo;


    public IEnumerable<string> SplitName(char splitChar = '/')
    {
        return Title.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
    }
}

// public readonly struct Author(string portraitUrl, string name, string link)
// {
//     public string PortraitUrl { get; } = portraitUrl;
//     public string Name { get; } = name;
//     public string Link { get; } = link;
// }

public readonly struct HotInfo(int evaluateCount, int watchCount, int likeCount)
{
    public int EvaluateCount { get; } = evaluateCount;
    public int WatchCount { get; } = watchCount;
    public int LikeCount { get; } = likeCount;
}