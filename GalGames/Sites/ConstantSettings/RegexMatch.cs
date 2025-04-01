using System.Text.RegularExpressions;

namespace GalgameSearchFor.GalGames.Sites.ConstantSettings;

public static partial class RegexMatch
{
    [GeneratedRegex(@"\d+(\.\d+)?")]
    public static partial Regex MatchSizeNumber();
}