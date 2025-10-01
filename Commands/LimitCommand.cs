namespace QuietChatBot.Commands;

using QuietChatBot.Models;
using QuietChatBot.Repositories;
using Telegram.Bot;

public class LimitCommand : IBotCommand
{
    private readonly LimitRepository _limitRepository;
    private readonly ITelegramBotClient _bot;
    private readonly MessageRepository _messageRepository;

    public LimitCommand(
        LimitRepository limitRepository,
        ITelegramBotClient bot,
        MessageRepository messageRepository
    )
    {
        _limitRepository = limitRepository;
        _bot = bot;
        _messageRepository = messageRepository;
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
        var today = DateTime.Today.ToUniversalTime();
        var tommorrow = today.AddDays(1);

        var messages = _messageRepository.GetAll();

        var userTodayMessages = messages
            .Where(m => m.ChatId == chatId
                && m.UserId == userId
                && m.MessageSendDate >= today
                && m.MessageSendDate < tommorrow
            ).ToList();

        var userTodayMessagesCount = userTodayMessages.Count();

        if (userTodayMessagesCount >= count)
        {
            await _bot.SendMessage(
                chatId: chatId,
                text: $"Вы за сегодня отправили {userTodayMessagesCount} сообщений, лимит должен быть больше"
            );
        }
        else
        {
            _limitRepository.Add(new Limit()
            {
                ChatId = chatId,
                UserId = userId,
                Count = count
            });

            await _bot.SendMessage(
                chatId: chatId,
                text: $"Лимит {count} для пользователя {userName} добавлен"
            );
        }
    }
}