using System.Text.Json;
using System.Text.Json.Serialization;

namespace GalgameSearchFor.GalGames.Sites.ConstantSettings;

public static class JsonSerializerSettings
{
    
    public static JsonSerializerOptions TouchGalBuilderSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = false
    };
    
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