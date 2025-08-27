namespace QuietChatBot.Models;

public class Message
{
    public int MessageId { get; set; }
    public DateTime MessageSendDate { get; set; }
    public long ChatId { get; set; }
    public long UserId { get; set; }
}