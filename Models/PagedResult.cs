namespace WebAPI.Models;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int Total { get; init; }
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
}