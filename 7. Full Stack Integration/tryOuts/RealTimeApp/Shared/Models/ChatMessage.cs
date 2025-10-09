namespace Shared.Models
{
    public class ChatMessage
    {
        public string User { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString() => $"{Timestamp:HH:mm} {User}: {Message}";
    }
}
