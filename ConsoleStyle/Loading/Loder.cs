namespace GalgameSearchFor.ConsoleStyle.Loading;

public static class Loder
{
    public static string[] DefaultLoadingFrames => ["|", "/", "-", "\\"];
    public static string[] UnicodeLoadingFrames => ["◜", "◠", "◝", "◞", "◡", "◟"];
    public static string[] SquareLoadingFrames => ["◢", "◣", "◤", "◥"];
    public static string[] CircleLoadingFrames => ["◯", "◔", "◑", "◒", "◐"];
    public static string[] DiamondLoadingFrames => ["◈", "◇", "◆", "◇"];
    public static string[] HeartLoadingFrames => ["❤", "❥", "❣", "♥"];
    public static string[] StarLoadingFrames => ["★", "☆", "✮", "✯"];
    public static string[] ArrowLoadingFrames => ["➤", "➥", "➦", "➧"];
    public static string[] SpiralLoadingFrames => ["⟳", "⟲", "⟳", "⟲"];
    public static string[] WaveLoadingFrames => ["~", "≈", "≃", "≅"];
    public static string[] CustomLoadingFrames => ["▙", "▛", "▜", "▟"];


    public static async Task LoadingFrieAndForget(string[] frames, string text = "LOADING ", int speed = 200, CancellationToken cancellationToken = default)
    {
        try
        {
            Console.CursorVisible = false;
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var frame in frames.Select(f => $"\e[38;2;{Random.Shared.Next(150, 255)};{Random.Shared.Next(150, 255)};{Random.Shared.Next(150, 255)}m{f}\e[0m"))
                {
                    Console.Write($"\r\e[38;2;20;209;104m{text} {frame}"); // \r实现原地刷新
                    await Task.Delay(speed, cancellationToken); // 控制动画速度
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            Console.Write("\r\e[K");
            Console.CursorVisible = true;
        }
    }
}