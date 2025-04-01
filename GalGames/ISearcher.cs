namespace GalgameSearchFor.GalGames;

public interface ISearcher
{
    void Search(string key);
    Task SearchAsync(string key, CancellationToken cancellationToken);
}