using GalgameSearchFor.GalGames.Sites.Results.ZhenHong;

namespace GalgameSearchFor.GalGames.Sites;

public partial class ZhenHong
{
    private partial class InternalWriteConsole(Func<IEnumerable<GalgameInfo>> resultsFunc, Uri baseUri, Func<int> showCount) : IWrieConsole
    {
        async Task IWrieConsole.WriteConsoleAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var infos = resultsFunc.Invoke().ToList();

            if (infos.Count >= showCount.Invoke())
            {
                Console.WriteLine("\uD83D\uDD0E \e[1m>>>结果过多，分页显示\e[0m：");
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

                var pageCount = (int)Math.Round(infos.Count / (double)showCount.Invoke());
                var showItemCount = (int)Math.Round((double)infos.Count / pageCount);

                foreach (var galgameInfos in infos.Chunk(showItemCount).Select((data, index) => new { data, index }))
                {
                    foreach (var galgameInfo in galgameInfos.data)
                    {
                        await Console.Out.WriteLineAsync($"\uD83C\uDFAE 《\e[1;38;2;255;165;0m{galgameInfo.Name}\e[0m》");
                        await Console.Out.WriteLineAsync($"\uD83D\uDD17 游戏链接：\e[38;2;96;174;228m\e[4m{Uri.UnescapeDataString(galgameInfo.DownloadUrl)}\e[0m");
                        await Console.Out.WriteLineAsync($"\uD83D\uDCC8 大小：\e[38;2;255;165;0m{galgameInfo.Size}\e[0m");
                        Console.WriteLine();
                    }


                    await Console.Out.WriteLineAsync($"\uD83D\uDD0E \e[1m>>>第\e[38;2;255;165;0m{galgameInfos.index + 1}\e[0m页，" +
                                                     $"共\e[38;2;255;165;0m{pageCount}\e[0m页");


                    // 最后一轮不需要询问
                    if (galgameInfos.index == pageCount - 1)
                    {
                        break;
                    }

                    ConsoleKeyInfo keyInfo;
                    Console.WriteLine("按下\e[38;2;255;255;0m回车\e[0m继续>>");
                    Console.WriteLine("按下\e[38;2;255;255;0mESC\e[0m切换到下一个站点>");
                    do
                    {
                        keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key == ConsoleKey.Escape)
                        {
                            return;
                        }
                    } while (keyInfo.Key != ConsoleKey.Enter);
                }
            }
            else
            {
                foreach (var galgameInfo in infos)
                {
                    await Console.Out.WriteLineAsync($"\uD83C\uDFAE 《\e[1;38;2;255;165;0m{galgameInfo.Name}\e[0m》");
                    await Console.Out.WriteLineAsync($"\uD83D\uDD17 游戏链接：\e[38;2;96;174;228m\e[4m{Uri.UnescapeDataString(galgameInfo.DownloadUrl)}\e[0m");
                    await Console.Out.WriteLineAsync($"\uD83D\uDCC8 大小：\e[38;2;255;165;0m{galgameInfo.Size}\e[0m");

                    Console.WriteLine();
                }
            }


            Console.WriteLine($"\u2600\uFE0F 网站名称：\e[48;2;255;255;0m\e[4;38;2;0;100;255m{baseUri}\e[0m");
            await Console.Out.WriteLineAsync($"\uD83D\uDD0E 搜索关键字：[ \e[38;2;255;255;0m{string.Join(' ', keys)}\e[0m ]");
            Console.WriteLine($"\uD83D\uDCCA 相关数量：\e[1m{infos.Count}\e[0m\r\n");
        }

        public override string ToString() => $"\e[1m真红小站（\e[38;2;96;174;228m\e[4m{baseUri}）\e[0m";
    }
}