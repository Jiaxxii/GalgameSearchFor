namespace GalgameSearchFor.GalGames;

public interface ISearcherFormResult<T>
{
    IEnumerable<T> SearchResult(string key);

    Task<IEnumerable<T>> SearchResultAsync(string key, CancellationToken cancellationToken = default);
}