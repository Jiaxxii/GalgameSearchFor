using RegexMatch = GalgameSearchFor.GalGames.Sites.ConstantSettings.RegexMatch;

namespace GalgameSearchFor.GalGames.Sites.Results.ZhenHong;

public readonly struct GalgameInfo(string name, string path, string size, string downloadUrl)
{
    public string Name { get; } = name;
    public string Path { get; } = path;
    public string Size { get; } = size;
    public string DownloadUrl { get; } = downloadUrl;


    public long GetByteSize()
    {
        var matchNumber = RegexMatch.MatchSizeNumber().Match(Size);

        if (!matchNumber.Success) return -1;

        var size = float.Parse(matchNumber.Value);

        if (Size.EndsWith("GB", StringComparison.OrdinalIgnoreCase))
        {
            return (long)(size * 1024 * 1024 * 1024);
        }

        if (Size.EndsWith("MB", StringComparison.OrdinalIgnoreCase))
        {
            return (long)(size * 1024 * 1024);
        }

        if (Size.EndsWith("KB", StringComparison.OrdinalIgnoreCase))
        {
            return (long)(size * 1024);
        }

        return -1;
    }
}