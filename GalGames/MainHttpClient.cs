using System.Net;
using GalgameSearchFor.GalGames.Sites.ConstantSettings;

namespace GalgameSearchFor.GalGames;

public static class MainHttpClient
{
    private static HttpClient MainClient { get; }


    // private static ConcurrentDictionary<string, byte> GetClientFilePathSet { get; } = new();

    static MainHttpClient()
    {
        var handler = new SocketsHttpHandler
        {
            // 连接池策略
            PooledConnectionLifetime = TimeSpan.FromSeconds(30), // 短生命周期，快速切换连接
            EnableMultipleHttp2Connections = true, // 强制多路复用

            // 超时控制
            // ConnectTimeout = TimeSpan.FromSeconds(3), // 快速失败
            // ResponseDrainTimeout = TimeSpan.FromSeconds(5),

            // 绕过反爬
            AutomaticDecompression = DecompressionMethods.All, // 解压缩节省带宽
            UseCookies = false // 禁用 Cookie 保持
        };

        // 创建全局 HttpClient 实例，设置默认超时为 30 秒
        // 注意：实际请求可能通过 CancellationToken 使用更短的超时
        MainClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        MainClient.DefaultRequestHeaders.UserAgent.ParseAdd(HttpRequestSettings.DefaultEdgeUserAgent);
    }


    public static HttpClient GetClient()
    {
        return MainClient;
    }

    public static void Disposable() => MainClient.Dispose();
}