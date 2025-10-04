namespace AideTool
{
    public sealed class ResponseResult
    {
        public RequestResultStatus Status { get; set; }
        public string Message { get; set; }
        public object Content { get; set; }

        public ResponseResult(RequestResultStatus result, string msg, object content)
        {
            Status = result;
            Message = msg;
            Content = content;
        }

        public static ResponseResult Ok() => new(RequestResultStatus.Ok, "OK", null);
        public static ResponseResult Ok(object model) => new(RequestResultStatus.Ok, "OK", model);
        public static ResponseResult Fail(string message) => new(RequestResultStatus.Failed, message, null); 
        public static ResponseResult Progress(float progress) => new(RequestResultStatus.Progressing, "", progress); 
    }
}
