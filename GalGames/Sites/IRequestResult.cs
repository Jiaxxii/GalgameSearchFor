namespace GalgameSearchFor.GalGames.Sites;

public interface IRequestResult<out T>
{
    IEnumerable<T> Results { get; }
}