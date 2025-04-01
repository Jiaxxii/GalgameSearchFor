using System.Diagnostics;
using GalgameSearchFor.ConsoleStyle.Loading;
using GalgameSearchFor.GalGames;
using GalgameSearchFor.GalGames.Sites;

Console.OutputEncoding = System.Text.Encoding.UTF8;

const string taskCancelled = "TASK CANCELED";
const string taskComplete = "TASK COMPLETE";
const string httpRequestError = "HTTP REQUEST ERROR";

var userCancellation = new CancellationTokenSource();
var searchCompleteCancellation = new CancellationTokenSource();


object[] sites = [new DaoHe(), new LiangZiACG(), new MiaoYuan(), new TouchGal(), new ZhenHong(), new ZiLing()];
foreach (var site in sites)
{
    bool isNext;
    do
    {
        Console.WriteLine(site.ToString() + '\n');
        var timestamp = Stopwatch.GetTimestamp();
        var state = await Search(site);
        var elapsedTime = Stopwatch.GetElapsedTime(timestamp);

        Console.WriteLine($"本次搜索耗时：\e[38;2;76;252;246m{elapsedTime.TotalSeconds:F4}\e[0m秒");

        if (state != taskComplete)
        {
            Console.WriteLine($"\e[38;2;255;0;0m请求失败！{state}\e[0m");
        }

        var userInput = await ReadUserInput("\r\n是否使用下一个网站搜索？（y/n）\e[38;2;76;252;246m\e[4m");
        Console.Write("\e[0m");

        isNext = userInput is not null && userInput.Contains('y', StringComparison.CurrentCultureIgnoreCase);
        Console.Clear();
    } while (!isNext);
}


MainHttpClient.Disposable();
return;

async Task<string> Search(object result)
{
    string? userInput;
    do
    {
        userInput = await ReadUserInput("\e[38;2;233;215;67m>>> \e[0m\e[38;2;255;131;236m请输入关键字：\e[0m\e[38;2;76;252;246m\e[4m");
        Console.WriteLine("\e[0m");
    } while (string.IsNullOrWhiteSpace(userInput));

    var loadingCts = CancellationTokenSource.CreateLinkedTokenSource(userCancellation.Token, searchCompleteCancellation.Token);
    _ = Loder.LoadingFrieAndForget(Loder.DefaultLoadingFrames, cancellationToken: loadingCts.Token);

    try
    {
        await ((ISearcher)result).SearchAsync(userInput, userCancellation.Token);
    }
    catch (OperationCanceledException e)
    {
        return string.Concat(taskCancelled, e.Message);
    }
    catch (HttpRequestException e)
    {
        return string.Concat(httpRequestError, e.Message);
    }
    finally
    {
        searchCompleteCancellation.Cancel();
        searchCompleteCancellation = new CancellationTokenSource();
    }


    await ((IWrieConsole)result).WriteConsoleAsync(userInput.Split(' '), cancellationToken: userCancellation.Token);
    return taskComplete;
}

async Task<string?> ReadUserInput(string? input)
{
    if (!string.IsNullOrEmpty(input))
    {
        await Console.Out.WriteAsync(input);
    }

    return await Console.In.ReadLineAsync();
}