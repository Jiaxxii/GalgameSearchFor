using GalgameSearchFor.ConsoleStyle.ANSI;
using GalgameSearchFor.GalGames.Sites.Results.TouchGalgame;

namespace GalgameSearchFor.GalGames.Sites;

public partial class TouchGal
{
    private partial class InternalWriteConsole(Func<TouchResult> resourceFunc, Uri baseUri) : IWrieConsole
    {
        async Task IWrieConsole.WriteConsoleAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var resource = resourceFunc.Invoke();
            foreach (var galgameInfo in resource.GalGames)
            {
                await Console.Out.WriteLineAsync($"\uD83C\uDFAE 《\e[1;38;2;255;165;0m{galgameInfo.Name}\e[0m》");
                await Console.Out.WriteLineAsync($"\uD83D\uDD17 下载页面：\e[38;2;96;174;228m\e[4m{new Uri(baseUri, galgameInfo.UniqueId)}\e[0m");
                await Console.Out.WriteLineAsync($"\uD83D\uDDE3\uFE0F 语言：[{string.Join('、', galgameInfo.Languages.Select(ToStings.LanguageColor))}]");
                await Console.Out.WriteLineAsync($"\uD83D\uDCBB 平台：[{string.Join('、', galgameInfo.Platforms.Select(ToStings.TargetPlatform))}]");
                await Console.Out.WriteLineAsync($"\uD83D\uDCE2 类型：[{string.Join('、', galgameInfo.Type.Select(ToStings.TargetPlatform))}]");
                await Console.Out.WriteLineAsync($"\u2728 游戏标签：[{string.Join('、', galgameInfo.GameTags.Select(tag => $"\e[38;2;76;252;167m{tag}\e[0m"))}]");

                await Console.Out.WriteLineAsync($"\uD83D\uDC40 观看人数：\e[38;2;255;165;0m{galgameInfo.View}\e[0m");
                await Console.Out.WriteLineAsync($"\uD83D\uDE80 下载人数：\e[38;2;255;165;0m{galgameInfo.Download}\e[0m");

                await Console.Out.WriteLineAsync($"\uD83D\uDCAC 评价人数：\e[38;2;255;165;0m{galgameInfo.Temperature.Comment}\e[0m");
                await Console.Out.WriteLineAsync($"\u2B50 收藏人数：\e[38;2;255;165;0m{galgameInfo.Temperature.FavoriteBy}\e[0m");
                await Console.Out.WriteLineAsync($"\uD83D\uDCBE 资源数量：\e[38;2;255;165;0m{galgameInfo.Temperature.Resource}\e[0m");
                Console.WriteLine();
            }

            Console.WriteLine($"\uD83C\uDF10 网站名称：\e[48;2;255;255;0m\e[4;38;2;0;100;255m{baseUri}\e[0m");
            await Console.Out.WriteLineAsync($"\uD83D\uDD0D 搜索关键字：[ \e[38;2;255;255;0m{string.Join(' ', keys)}\e[0m ]");
            Console.WriteLine($"\uD83D\uDCCA 相关数量：\e[1m{resource.Total}\e[0m\r\n");
        }

        public override string ToString() => $"\e[1mTouchGalgame（\e[38;2;96;174;228m\e[4m{baseUri}）\e[0m";
    }
}