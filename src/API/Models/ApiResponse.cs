namespace API.Models;

/// <summary>
/// Modelo de respuesta estándar de la API (reemplaza RespuestasVMR)
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; }

    public ApiResponse()
    {
        Success = true;
        Errors = new List<string>();
    }

    public static ApiResponse<T> Ok(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> Fail(string error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = error,
            Errors = new List<string> { error }
        };
    }

    public static ApiResponse<T> Fail(List<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = errors.Count > 0 ? errors[0] : "La solicitud no pudo procesarse.",
            Errors = errors
        };
    }

    public static ApiResponse<T> Fail(string message, IEnumerable<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors.ToList()
        };
    }
}

/// <summary>
/// Modelo para respuestas paginadas (reemplaza ListadoPaginadoVMR)
/// </summary>
public class PaginatedResponse<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public List<T> Items { get; set; }

    public PaginatedResponse()
    {
        Items = new List<T>();
    }
}

/// <summary>
/// Parámetros de paginación para requests
/// </summary>
public class PaginationParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string? SearchTerm { get; set; }
}
