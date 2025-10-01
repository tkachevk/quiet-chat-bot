namespace QuietChatBot.Commands;

using QuietChatBot.Models;
using QuietChatBot.Repositories;
using Telegram.Bot;

public class LimitCommand : IBotCommand
{
    private readonly LimitRepository _limitRepository;
    private readonly ITelegramBotClient _bot;

    public LimitCommand(LimitRepository limitRepository, ITelegramBotClient bot)
    {
        _limitRepository = limitRepository;
        _bot = bot;
    }

    public bool CanHandle(Telegram.Bot.Types.Message message)
    {
        return message.Text?.StartsWith("/limit") ?? false;
    }

    public async Task HandleAsync(Telegram.Bot.Types.Message message, CancellationToken ct)
    {
        var splitCommand = message.Text!.Split(' ');

        var chatId = message.Chat.Id;
        var userId = message.From.Id;
        var userName = message.From.Username;
        var count = Int32.Parse(splitCommand[1]);

        var newLimit = new Limit()
        {
            ChatId = chatId,
            UserId = userId,
            Count = count
        };

        _limitRepository.Add(newLimit);

        await _bot.SendMessage(
            chatId: chatId,
            text: $"Лимит {count} для пользователя {userName} добавлен"
        );
    }
}