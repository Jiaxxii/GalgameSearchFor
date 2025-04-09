using GalgameSearchFor.GalGames.Sites.Results.QiMeng;

namespace GalgameSearchFor.GalGames.Sites;

public partial class QiMeng
{
    private partial class InternalWriteConsole(Uri baseUri, Func<QiMengResult> resultFunc) : IWrieConsole
    {
        public async Task WriteConsoleAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        {
            var qiMengResult = resultFunc.Invoke();

            foreach (var galgameInfo in qiMengResult.Results)
            {
                await Console.Out.WriteLineAsync($"\uD83C\uDFAE \e[1;38;2;0;100;255m游戏名称：\e[0m\e[38;2;50;200;50m{galgameInfo.TitleName}\e[0m");
                await Console.Out.WriteLineAsync($"\uD83D\uDD17 \e[38;2;255;255;0m游戏下载页面：\e[4m{galgameInfo.GamePageLink}\e[0m");
                await Console.Out.WriteLineAsync($"\uD83C\uDFF7 \e[38;2;0;200;200m类型：\e[0m{galgameInfo.Type}");
                await Console.Out.WriteLineAsync($"\uD83D\uDDBC\uFE0F \e[38;2;255;150;0m游戏插画(CG)：\e[0m\n{string.Join("\n", galgameInfo.ImageUrls)}");
                await Console.Out.WriteLineAsync($"\uD83D\uDCC5 \e[38;2;200;0;200m上传日期：\e[0m{galgameInfo.CreatedTime:yyyy年MM月dd日}");
                await Console.Out.WriteLineAsync($"\uD83D\uDC41\uFE0F \e[38;2;0;255;0m浏览：\e[1m{galgameInfo.HotInfo.Watch}\e[0m");
                await Console.Out.WriteLineAsync($"\uD83D\uDCAC \e[38;2;255;255;0m评论：\e[1m{galgameInfo.HotInfo.Evaluate}\e[0m");
                await Console.Out.WriteLineAsync($"\u2764\uFE0F \e[38;2;255;0;0m喜欢：\e[1m{galgameInfo.HotInfo.Like}\e[0m\n");
            }

            Console.WriteLine($"\uD83C\uDF10 \e[1;38;2;0;100;255m网站名称：\e[0m\e[48;2;30;30;30m\e[38;2;0;255;255m{baseUri}\e[0m");
            await Console.Out.WriteLineAsync($"\uD83D\uDD0E \e[38;2;255;150;0m搜索关键字：\e[0m[ \e[3;38;2;255;255;0m{string.Join(' ', keys)}\e[0m ]");
            Console.WriteLine($"\uD83D\uDCCA \e[38;2;200;0;200m搜索结果共：\e[5;1m{qiMengResult.PageCount}\e[0m页\r\n");
        }

        public override string ToString() => $"\e[1m绮梦（\e[38;2;96;174;228m\e[4m{baseUri}）\e[0m";
    }
}