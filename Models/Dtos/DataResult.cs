namespace UserAuthAPI.Models.Dtos
{
    public class DataResult
    {
        public bool Success { get; set; } = false;
        public List<MessageItem> Messages { get; set; }
        public object Data { get; set; }
    }
    public class MessageItem
    {
        public string Message { get; set; } = "";
        public string Field { get; set; } = "";
    }
}
