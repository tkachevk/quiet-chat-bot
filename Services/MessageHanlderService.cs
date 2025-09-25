namespace QuietChatBot.Services;

using QuietChatBot.Repositories;
using Telegram.Bot.Types;
using QuietChatBot.Helpers;
using Telegram.Bot;
using Microsoft.Extensions.Options;

public class MessageHandlerService
{
    private readonly MessageRepository _messageRepository;
    private readonly LimitRepository _limitRepository;
    private readonly AppConfig _config;
    private readonly ITelegramBotClient _bot;

    public MessageHandlerService(
        MessageRepository messageRepository,
        LimitRepository limitRepository,
        ITelegramBotClient bot,
        IOptions<AppConfig> options
    )
    {
        _messageRepository = messageRepository;
        _limitRepository = limitRepository;
        _config = options.Value;
        _bot = bot;
    }

    public async Task HandleMessageAsync(Message message, CancellationToken ct)
    {
        _messageRepository.Add(new Models.Message()
        {
            ChatId = message.Chat.Id,
            UserId = message.From.Id,
            MessageSendDate = message.Date
        });

        var messages = _messageRepository.GetAll();

        var today = DateTime.Today.ToUniversalTime();
        var tommorrow = today.AddDays(1);

        var userTodayMessages = messages
            .Where(m => m.ChatId == message.Chat.Id
                && m.UserId == message.From.Id
                && m.MessageSendDate >= today
                && m.MessageSendDate < tommorrow
            ).ToList();

        var userTodayMessagesCount = userTodayMessages.Count();

        var limits = _limitRepository.GetAll();
        var currentUserLimit = limits
            .Where(m => m.ChatId == message.Chat.Id
                && m.UserId == message.From.Id).LastOrDefault();

        if (currentUserLimit != null)
        {
            if (userTodayMessagesCount >= currentUserLimit.Count)
            {
                var currentUserChatInfo = await _bot.GetChatMember(message.Chat.Id, message.From.Id);

                if (currentUserChatInfo is not ChatMemberRestricted and not ChatMemberOwner)
                {
                    var timeHelper = new TimeHelper(_config.TimeZone);
                    var untilDate = timeHelper.GetEndOfDayUtc();

                    await _bot.RestrictChatMember(
                        chatId: message.Chat.Id,
                        userId: message.From.Id,
                        permissions: new ChatPermissions() { CanSendMessages = false },
                        untilDate: untilDate
                    );

                    await _bot.SendMessage(
                        chatId: message.Chat.Id,
                        text: $"Пользователь {message.From.Username} исчерпал лимит сообщений!"
                    );
                }
            }
        }
    }
}