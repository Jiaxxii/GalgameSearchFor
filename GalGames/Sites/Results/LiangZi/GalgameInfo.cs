namespace GalgameSearchFor.GalGames.Sites.Results.LiangZi;

public readonly struct GalgameInfo(string imageUrl, string pageLink, string title, IEnumerable<string> tags, DateTime created, int evaluationCount, int watchCount)
{
    public string ImageUrl { get; } = imageUrl;
    public string PageLink { get; } = pageLink;
    public string Title { get; } = title;

    public IEnumerable<string> Tags { get; } = tags;

    public DateTime Created { get; } = created;

    public int EvaluationCount { get; } = evaluationCount;
    public int WatchCount { get; } = watchCount;
}