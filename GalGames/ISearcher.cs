namespace GalgameSearchFor.GalGames;

public interface ISearcher : IObject
{
    void Search(string key);
    Task SearchAsync(string key, CancellationToken cancellationToken);
}