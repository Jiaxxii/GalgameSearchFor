using System.Diagnostics;
using GalgameSearchFor.ConsoleStyle.Loading;
using GalgameSearchFor.GalGames;
using GalgameSearchFor.GalGames.Sites;
using Serilog;


Console.OutputEncoding = System.Text.Encoding.UTF8;

// https://gal.saop.cc/search.json

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Console.WriteLine(
    "\e[38;2;255;255;0m\uD83D\uDC64\e[0m 作者：" + // 黄色人像
    " \e[38;2;0;255;255m西雨与雨 \e[0m");

Console.WriteLine(
    " \e[38;2;255;255;0m\uD83C\uDF10 \e[0m 活跃站点：" + // 黄色地球
    " \e[38;2;96;174;228;4mhttps://www.douyin.com/user/MS4wLjABAAAAH21NzOsn19X7PJRQH1iDOHGdewaiE3vPWaw35fm1C0Q?from_tab_name=main \e[0m");

var userCancellation = new CancellationTokenSource();
var searchCompleteCancellation = new CancellationTokenSource();

// 取消搜索
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    userCancellation.Cancel();
    MainHttpClient.Disposable();
    Environment.Exit(0);
};


ISearcher[] sites =
[
    new ZhenHong(),
    new TouchGal(),
    new LiangZiACG(TimeSpan.FromSeconds(60)),
    new MiaoYuan(),
    new ZiLing(),
    new YanYu(),
    new DaoHe(),
    new QiMeng()
];

Console.WriteLine($"共 \e[38;2;255;165;0m{sites.Length}\e[0m 个站点");


while (!userCancellation.IsCancellationRequested)
{
    var readUserInput = await User.ReadUserInput();

    var start = Stopwatch.GetTimestamp();

    var state = await SearchAsync(readUserInput);

    var elapsedTime = Stopwatch.GetElapsedTime(start);

    if (state == State.Complete)
    {
        foreach (var instance in sites.Cast<IWrieConsole>().Select((wrieConsole, index) => new { wrieConsole, index }))
        {
            await instance.wrieConsole.WriteConsoleAsync(readUserInput.Split(' '), userCancellation.Token);

            Console.WriteLine($"\ud83e\uddfe来源：{instance.wrieConsole}\t" +
                              $"第\e[38;2;255;165;0m{instance.index + 1}\e[0m个站点共\e[38;2;255;165;0m{sites.Length}\e[0m个");

            if (instance.index == 0)
            {
                Console.WriteLine($"本次搜索耗时：\e[38;2;76;252;246m{elapsedTime.TotalSeconds:F4}\e[0m秒 共访问了" +
                                  $"\e[38;2;255;165;0m{sites.Length}\e[0m个站点\r\n");
            }


            ConsoleKeyInfo keyInfo;
            Console.WriteLine("按下\e[38;2;255;255;0m回车\e[0m继续>>>\r\n");
            do
            {
                keyInfo = Console.ReadKey();
            } while (keyInfo.Key != ConsoleKey.Enter);

            Console.Write("\e[2J\e[H");
            Console.Clear();
            Console.WriteLine("\e[3J");
        }

        Console.Clear();
    }

    // 是否重新搜索
    Console.WriteLine("按下\e[38;2;255;255;0m任意按键\e[0m重新开始搜索>>>");
    _ = Console.Read();
    Console.Clear();
}

MainHttpClient.Disposable();
return 0;

async Task<State> SearchAsync(string key)
{
    Console.Clear();

    var loadingCts = CancellationTokenSource.CreateLinkedTokenSource(userCancellation.Token, searchCompleteCancellation.Token);
    _ = Loder.LoadingFrieAndForget(Loder.DefaultLoadingFrames, cancellationToken: loadingCts.Token);

    try
    {
        var userCancelToken = userCancellation.Token;
        await Task.WhenAll(sites.Select(site => site.SearchAsync(key, userCancelToken)));
        searchCompleteCancellation.Cancel();
        return State.Complete;
    }
    catch (TaskCanceledException e)
    {
        searchCompleteCancellation.Cancel();

        if (e.CancellationToken == userCancellation.Token)
        {
#if DEBUG
            throw;
#else
            // 用户取消
            Console.WriteLine("\e[43;34mERROR\e[0m任务已经取消");
            return State.UserCanceled;
#endif
        }
#if DEBUG
        throw;
#else
        // 超时
        // Console.WriteLine($"\e[43;34mERROR\e[0m任务超时！{e.Message}");
        Log.Error(e, "\e[43;34mERROR\e[0m任务超时！{msg}", e.Message);
        return State.Timeout;
#endif
    }
    catch (HttpRequestException e)
    {
        searchCompleteCancellation.Cancel();

        // Console.WriteLine($"\e[43;34mERROR\e[0m可能网络原因被取消{e.Message}");
        Log.Error(e, "\e[43;34mERROR\e[0m可能网络原因被取消{msg}", e.Message);
#if DEBUG
        throw;
#else
        return State.HttpRequestException;
#endif
    }
    catch (Exception e)
    {
        searchCompleteCancellation.Cancel();
        Log.Error(e, "\e[43;34mERROR\e[0m致命错误{msg}", e.Message);

#if DEBUG
        throw;
#else
        return State.Unknown;
#endif
    }
}

file enum State
{
    Complete,
    UserCanceled,
    Timeout,
    HttpRequestException,
    Unknown
}

file static class User
{
    internal static async Task<string> ReadUserInput()
    {
        string? userInput;
        do
        {
            userInput = await ReadUserInputLine
                ("\e[38;2;233;215;67m>>>\e[0m \e[38;2;255;131;236m请输入关键字：\e[0m\e[38;2;76;252;246m\e[4m");

            Console.Write("\e[0m");
            Console.Clear();
        } while (string.IsNullOrEmpty(userInput));

        return userInput;
    }

    private static async Task<string?> ReadUserInputLine(string? input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            await Console.Out.WriteAsync(input);
        }

        return await Console.In.ReadLineAsync();
    }
}