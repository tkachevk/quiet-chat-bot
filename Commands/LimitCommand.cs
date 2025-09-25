namespace QuietChatBot.Commands;

using QuietChatBot.Models;
using QuietChatBot.Repositories;

public class LimitCommand : IBotCommand
{
    private readonly LimitRepository _limitRepository;

    public LimitCommand(LimitRepository limitRepository)
    {
        _limitRepository = limitRepository;
    }

    public bool CanHandle(Telegram.Bot.Types.Message message)
    {
        return message.Text?.StartsWith("/limit") ?? false;
    }

    public async Task HandleAsync(Telegram.Bot.Types.Message message, CancellationToken ct)
    {
        var count = message.Text!.Split(' ');

        var newLimit = new Limit()
        {
            ChatId = message.Chat.Id,
            UserId = message.From.Id,
            Count = Int32.Parse(count[1])
        };

        _limitRepository.Add(newLimit);
    }
}