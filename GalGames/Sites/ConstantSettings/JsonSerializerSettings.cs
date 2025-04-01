using System.Text.Json;

namespace GalgameSearchFor.GalGames.Sites.ConstantSettings;

public static class JsonSerializerSettings
{
    public static JsonSerializerOptions CamelCaseSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };


    public static JsonSerializerOptions SnakeCaseLowerSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = false
    };
}