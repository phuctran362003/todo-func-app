namespace todo_web_app.Models
{
    public class TodoItem
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateTodoItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateTodoItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public bool IsFailure { get; set; }
        public ApiResponseValue<T>? Value { get; set; }
        public ApiResponseError? Error { get; set; }
    }

    public class ApiResponseValue<T>
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    public class ApiResponseError
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
    }

    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public bool IsFailure { get; set; }
        public ApiResponseValuePlain? Value { get; set; }
        public ApiResponseError? Error { get; set; }
    }

    public class ApiResponseValuePlain
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
    }
}
