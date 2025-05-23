﻿using GalgameSearchFor.ConsoleStyle.ANSI;
using GalgameSearchFor.GalGames.Sites.Results.MiaoYuan;

namespace GalgameSearchFor.GalGames.Sites;

public partial class MiaoYuan
{
    private partial class InternalWriteConsole(Func<IEnumerable<GalgameInfo>> resultsFunc, Uri baseUri) : IWrieConsole
    {
        async Task IWrieConsole.WriteConsoleAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var results = resultsFunc.Invoke().ToArray();
            foreach (var galgameInfo in results)
            {
                await Console.Out.WriteLineAsync($"\uD83C\uDFAE {string.Join('、', galgameInfo.SplitName().Select(name => $"《\e[1;38;2;255;165;0m{name}\e[0m》"))}");
                await Console.Out.WriteLineAsync($"\uD83D\uDD17 游戏链接：\e[38;2;96;174;228m\e[4m{new Uri(baseUri, galgameInfo.PageLink).AbsoluteUri}\e[0m");
                await Console.Out.WriteLineAsync($"\uD83D\uDCE2 标签：{string.Join(", ", galgameInfo.Tags.Select(ToStings.TargetPlatform))}");
                await Console.Out.WriteLineAsync($"\uD83D\uDCE2 插画：{galgameInfo.ImageUrl}");

                // await Console.Out.WriteLineAsync($"\uD83D\uDC64 上传作者：\e[38;2;76;252;246m{galgameInfo.Author.Name}\e[0m");
                // await Console.Out.WriteLineAsync($"\uD83C\uDFE0 作者主页：\e[38;2;96;174;228m\e[4m{galgameInfo.Author.Link}\e[0m");

                await Console.Out.WriteLineAsync($"\uD83D\uDCAC 评价人数：\e[38;2;255;165;0m{galgameInfo.HotInfo.EvaluateCount}\e[0m");
                await Console.Out.WriteLineAsync($"\uD83D\uDC41\uFE0F 观看人数：\e[38;2;255;165;0m{galgameInfo.HotInfo.WatchCount}\e[0m");
                await Console.Out.WriteLineAsync($"\u2764\uFE0F 收藏人数：\e[38;2;255;165;0m{galgameInfo.HotInfo.LikeCount}\e[0m");

                Console.WriteLine();
            }

            Console.WriteLine($"\uD83C\uDF10 网站名称：\e[48;2;255;255;0m\e[4;38;2;0;100;255m{baseUri}\e[0m");
            await Console.Out.WriteLineAsync($"\uD83D\uDD0E 搜索关键字：[ \e[38;2;255;255;0m{string.Join(' ', keys)}\e[0m ]");
            Console.WriteLine($"\uD83D\uDCCA 相关数量：\e[1m{results.Length}\e[0m\r\n");
        }

        public override string ToString() => $"\e[1m喵源领域（\e[38;2;96;174;228m\e[4m{baseUri}）\e[0m";
    }
}