namespace buddyUp.DTOs
{
    public class MessageDto
    {
        public int chatId { get; set; }
        public int senderId { get; set; }
        public string text { get; set; } = string.Empty;
    }
}
