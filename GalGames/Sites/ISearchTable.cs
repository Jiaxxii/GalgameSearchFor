namespace GalgameSearchFor.GalGames.Sites;

public interface ISearchTable<TResult, in TTable>
{
    IEnumerable<TResult> Search(TTable key);

    Task<IEnumerable<TResult>> SearchAsync(TTable key, CancellationToken cancellationToken = default);
}