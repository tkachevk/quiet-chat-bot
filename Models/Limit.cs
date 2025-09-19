namespace QuietChatBot.Models;

public class Limit
{
    public int LimitId { get; set; }
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public int Count { get; set; } 
}