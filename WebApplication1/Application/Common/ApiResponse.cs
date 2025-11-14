using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public List<string> Errors { get; init; } = new();

    public static ApiResponse<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static ApiResponse<T> Fail(IEnumerable<string> errors) => new()
    {
        Success = false,
        Errors = errors.ToList()
    };

    public static ApiResponse<T> Fail(params string[] errors) => Fail((IEnumerable<string>)errors);
}
