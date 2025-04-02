namespace GalgameSearchFor.GalGames;

// public interface IWrieConsole<in T>
// {
//     public Task WriteConsoleAsync(T key, int? millisecondsDelay = null, CancellationToken cancellationToken = default);
// }

public interface IWrieConsole
{
    Task WriteConsoleAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);
}