using System.Diagnostics;
using GalgameSearchFor.ConsoleStyle.Loading;
using GalgameSearchFor.GalGames;
using GalgameSearchFor.GalGames.Sites;
using TouchGal = GalgameSearchFor.GalGames.Sites.TouchGal;

Console.OutputEncoding = System.Text.Encoding.UTF8;


var userCancellation = new CancellationTokenSource();
var searchCompleteCancellation = new CancellationTokenSource();

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    userCancellation.Cancel();
    MainHttpClient.Disposable();
    Environment.Exit(0);
};


ISearcher[] sites =
[
    new DaoHe(),
    new LiangZiACG(TimeSpan.FromSeconds(60)),
    new MiaoYuan(),
    new TouchGal(),
    new ZhenHong(),
    new ZiLing()
];


while (!userCancellation.IsCancellationRequested)
{
    var readUserInput = await User.ReadUserInput();
    var state = await SearchAsync(readUserInput);

    if (state == State.Complete)
    {
        foreach (var wrieConsole in sites.Cast<IWrieConsole>())
        {
            Console.Clear();
            await wrieConsole.WriteConsoleAsync(readUserInput.Split(' '), userCancellation.Token);
            Console.WriteLine($"\ud83e\uddfe来源：{wrieConsole}");

            ConsoleKeyInfo keyInfo;
            Console.WriteLine("按下\e[38;2;255;255;0m回车\e[0m继续>>>");
            do
            {
                keyInfo = Console.ReadKey();
            } while (keyInfo.Key != ConsoleKey.Enter);
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

    var start = Stopwatch.GetTimestamp();
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
            // 用户取消
            Console.WriteLine("\e[43;34mERROR\e[0m任务已经取消");
            return State.UserCanceled;
        }

        // 超时
        Console.WriteLine($"\e[43;34mERROR\e[0m任务超时！{e.Message}");
        return State.Timeout;
    }
    catch (HttpRequestException e)
    {
        searchCompleteCancellation.Cancel();

        Console.WriteLine($"\e[43;34mERROR\e[0m可能网络原因被取消{e.Message}");
        return State.HttpRequestException;
    }
    finally
    {
        var elapsedTime = Stopwatch.GetElapsedTime(start);
        await Task.Delay(1000);
        Console.WriteLine($"本次搜索耗时：\e[38;2;76;252;246m{elapsedTime.TotalSeconds:F4}\e[0m秒");
    }
}

file enum State
{
    Complete,
    UserCanceled,
    Timeout,
    HttpRequestException
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

    internal static async Task<string?> ReadUserInputLine(string? input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            await Console.Out.WriteAsync(input);
        }

        return await Console.In.ReadLineAsync();
    }
}