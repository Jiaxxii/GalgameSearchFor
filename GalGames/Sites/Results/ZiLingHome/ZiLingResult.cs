using System.Text.Json.Serialization;

namespace GalgameSearchFor.GalGames.Sites.Results.ZiLingHome;

[method: JsonConstructor]
public readonly struct ZiLingResult(int code, string message, ResourcesResultData data) : IValid
{
    public int Code { get; } = code;

    public string Message { get; } = message;

    public ResourcesResultData Data { get; } = data;


    public bool IsValid()
    {
        return Code == 200 && Message.Equals("success", StringComparison.CurrentCultureIgnoreCase);
    }
}

[method: JsonConstructor]
public readonly struct ResourcesResultData(int total, IEnumerable<GalgameInfo> content)
{
    public int Total { get; } = total;

    public IEnumerable<GalgameInfo> Content { get; } = content;
}

[method: JsonConstructor]
public readonly struct GalgameInfo(string parent, string name, bool isDir, long size, int type)
{
    public string Parent { get; } = parent;
    public string Name { get; } = name;
    public bool IsDir { get; } = isDir;
    public long Size { get; } = size;
    public int Type { get; } = type;

    public string GetSizeString()
    {
        return Size < 1024 * 1024 * 1024 ? $"{Size / 1024F / 1024:F2} MB" : $"{Size / 1024f / 1024 / 1024:F2}GB";
    }
}