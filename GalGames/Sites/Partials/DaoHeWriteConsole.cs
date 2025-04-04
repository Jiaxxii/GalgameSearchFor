using GalgameSearchFor.ConsoleStyle.ANSI;
using GalgameSearchFor.GalGames.Sites.Results.DaoHe;

namespace GalgameSearchFor.GalGames.Sites;

public partial class DaoHe
{
    private sealed partial class InternalWriteConsole(IEnumerable<GameInfo> results, Uri baseUri) : IWrieConsole
    {
        async Task IWrieConsole.WriteConsoleAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            foreach (var galgameInfo in results)
            {
                await Console.Out.WriteLineAsync($"\uD83C\uDFAE 《\e[1;38;2;255;165;0m{galgameInfo.ShowName}\e[0m》（\e[1;38;2;255;165;0m{galgameInfo.RawName}\e[0m）"); // 游戏手柄
                await Console.Out.WriteLineAsync(
                    $"\uD83D\uDDBC 类型：\e[38;2;229;129;123m{galgameInfo.GameType}\e[0m \uD83D\uDCAC 介绍：{new Uri(baseUri, galgameInfo.GameInfoUrl).AbsoluteUri}"); // 画板+对话框
                foreach (var downloadPair in galgameInfo.Downloads)
                {
                    await Console.Out.WriteLineAsync($"\uD83D\uDCBB 平台：{ToStings.TargetPlatform(downloadPair.Key)}"); // 笔记本电脑
                    foreach (var downloadPath in downloadPair.Value)
                    {
                        await Console.Out.WriteLineAsync($"\uD83D\uDD17 下载地址：\e[38;2;96;174;228m\e[4m{downloadPath.Url}\e[0m"); // 链接符号
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine($"\uD83C\uDF10 网站名称：\e[48;2;255;255;0m\e[4;38;2;0;100;255m{baseUri}\e[0m"); // 地球图标
            await Console.Out.WriteLineAsync($"\uD83D\uDD0D 搜索关键字：[ \e[38;2;255;255;0m{string.Join(' ', keys)}\e[0m ]"); // 放大镜图标
            Console.WriteLine($"\uD83D\uDCC8 相关数量：\e[1m{results.Count()}\e[0m\r\n"); // 上升图表
        }

        public override string ToString() => $"\e[1m蹈荷（\e[38;2;96;174;228m\e[4m{baseUri}）\e[0m";
    }
}