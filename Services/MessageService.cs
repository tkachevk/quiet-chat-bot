namespace QuietChatBot.Services;

using QuietChatBot.Repositories;

public class MessageService
{
    public int GetTodayMessageCount(long userId, long chatId)
    {
        var messageRepository = new MessageRepository();

        return -1;
    }
}