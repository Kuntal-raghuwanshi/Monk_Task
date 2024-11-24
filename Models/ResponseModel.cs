namespace Monk_Task.Models
{
    public class ResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
        public ResponseModel()
        {
            IsSuccess = false;
            Message = "Warning";
            Data = null;
        }
    }
}
