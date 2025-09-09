namespace WebAPI.Models;

public class PagedUsersQuery
{
    public int PageIndex { get; init; } = 0;
    public int PageSize { get; init; } = 10;
    public string? SortBy { get; init; } // column
    public string? SortDir { get; init; } //asc or desc
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
}