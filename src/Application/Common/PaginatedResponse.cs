namespace Application.Common;

/// <summary>
/// Modelo para respuestas paginadas en la capa Application
/// </summary>
public class PaginatedResponse<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public List<T> Items { get; set; }

    public PaginatedResponse()
    {
        Items = new List<T>();
    }
}
