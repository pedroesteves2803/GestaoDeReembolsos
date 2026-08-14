namespace GestaodeReembolsos.Dtos.Shared;

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }

    public ApiResponseDto(bool success, string message, T? data = default)
    {
        Success = success;
        Message = message;
        Data = data;
    }
}