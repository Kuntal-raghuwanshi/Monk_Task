namespace Monk_Task.Models
{
    public class ResponseModel
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
