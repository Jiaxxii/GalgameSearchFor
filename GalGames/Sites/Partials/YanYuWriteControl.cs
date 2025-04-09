using GalgameSearchFor.GalGames.Sites.Results.ZiLingHome;

namespace GalgameSearchFor.GalGames.Sites;

public partial class YanYu
{
    private partial class InternalWriteConsole(Func<ZiLingResult> resourceFunc, Uri baseUri) : IWrieConsole
    {
        async Task IWrieConsole.WriteConsoleAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var resource = resourceFunc.Invoke();
            foreach (var galgameInfo in resource.Data.Content)
            {
                await Console.Out.WriteLineAsync($"\uD83C\uDFAE 《\e[1;38;2;255;165;0m{galgameInfo.Name}\e[0m》");

                var downloadUrl = new Uri(baseUri, $"{Uri.EscapeDataString(galgameInfo.Parent)}/{galgameInfo.Name}?form=search");
                await Console.Out.WriteLineAsync($"\uD83D\uDD17 游戏链接：\e[38;2;96;174;228m\e[4m{downloadUrl}\e[0m");

                await Console.Out.WriteLineAsync($"\uD83D\uDCC8 大小：\e[38;2;255;165;0m{galgameInfo.GetSizeString()}\e[0m");

                await Console.Out.WriteLineAsync($"\uD83D\uDCC0 是否是文件夹：\e[38;2;255;165;0m{(galgameInfo.IsDir ? '是' : '否')}\e[0m");
                Console.WriteLine();
            }

            Console.WriteLine($"\u2600\uFE0F 网站名称：\e[48;2;255;255;0m\e[4;38;2;0;100;255m{baseUri}\e[0m");
            await Console.Out.WriteLineAsync($"\uD83D\uDD0E 搜索关键字：[ \e[38;2;255;255;0m{string.Join(' ', keys)}\e[0m ]");
            Console.WriteLine($"\uD83D\uDCCA 相关数量：\e[1m{resource.Data.Total}\e[0m\r\n");
        }

        public override string ToString() => $"\e[1m烟郁（\e[38;2;96;174;228m\e[4m{baseUri}）\e[0m";
    }
}