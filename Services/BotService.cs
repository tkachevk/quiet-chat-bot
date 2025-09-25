namespace QuietChatBot.Services;

using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using QuietChatBot.Repositories;
using QuietChatBot.Helpers;
using QuietChatBot.Models;

public class BotService
{
    private readonly ITelegramBotClient _bot;
    private readonly AppConfig _config;
    private readonly LimitRepository _limitRepository;
    private readonly MessageRepository _messageRepository;

    public BotService(
        ITelegramBotClient bot,
        IOptions<AppConfig> options,
        LimitRepository limitRepository,
        MessageRepository messageRepository
    )
    {
        _config = options.Value;
        _bot = bot;
        _limitRepository = limitRepository;
        _messageRepository = messageRepository;
    }

    public async Task HandleUpdateAsync(
        Update update,
        CancellationToken ct)
    {
        if (update.Message != null)
        {
            if (update.Message.Text != null && update.Message.Text.StartsWith("/limit"))
            {
                var count = update.Message.Text.Split(' ');

                var newLimit = new Limit()
                {
                    ChatId = update.Message.Chat.Id,
                    UserId = update.Message.From.Id,
                    Count = Int32.Parse(count[1])
                };
                _limitRepository.Add(newLimit);
            }
            else
            {
                _messageRepository.Add(new Models.Message()
                {
                    ChatId = update.Message.Chat.Id,
                    UserId = update.Message.From.Id,
                    MessageSendDate = update.Message.Date
                });

                var messages = _messageRepository.GetAll();

                var today = DateTime.Today;
                var tommorrow = today.AddDays(1);

                var userTodayMessages = messages
                    .Where(m => m.ChatId == update.Message.Chat.Id
                        && m.UserId == update.Message.From.Id
                        && m.MessageSendDate >= today
                        && m.MessageSendDate < tommorrow
                    ).ToList();

                var userTodayMessagesCount = userTodayMessages.Count();

                var limits = _limitRepository.GetAll();
                var currentUserLimit = limits
                    .Where(m => m.ChatId == update.Message.Chat.Id
                        && m.UserId == update.Message.From.Id).LastOrDefault();

                if (currentUserLimit != null)
                {
                    if (userTodayMessagesCount >= currentUserLimit.Count)
                    {
                        var currentUserChatInfo = await _bot.GetChatMember(update.Message.Chat.Id, update.Message.From.Id);

                        if (currentUserChatInfo is not ChatMemberRestricted and not ChatMemberOwner)
                        {
                            var timeHelper = new TimeHelper(_config.TimeZone);
                            var untilDate = timeHelper.GetEndOfDayUtc();

                            await _bot.RestrictChatMember(
                                chatId: update.Message.Chat.Id,
                                userId: update.Message.From.Id,
                                permissions: new ChatPermissions() { CanSendMessages = false },
                                untilDate: untilDate
                            );

                            await _bot.SendMessage(
                                chatId: update.Message.Chat.Id,
                                text: $"Пользователь {update.Message.From.Username} исчерпал лимит сообщений!"
                            );
                        }
                    }
                }
            }
        }
    }
}