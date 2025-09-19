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

    public BotService(ITelegramBotClient bot, IOptions<AppConfig> options)
    {
        _config = options.Value;
        _bot = bot;
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
                var limitRepository = new LimitRepository();
                limitRepository.Add(newLimit);
            }
            else
            {
                var messageRepository = new MessageRepository();
                messageRepository.Add(new QuietChatBot.Models.Message()
                {
                    ChatId = update.Message.Chat.Id,
                    UserId = update.Message.From.Id,
                    MessageSendDate = update.Message.Date
                });

                var messages = messageRepository.GetAll();

                var today = DateTime.Today;
                var tommorrow = today.AddDays(1);

                var userTodayMessages = messages
                    .Where(m => m.ChatId == update.Message.Chat.Id
                        && m.UserId == update.Message.From.Id
                        && m.MessageSendDate >= today
                        && m.MessageSendDate < tommorrow
                    ).ToList();

                var userTodayMessagesCount = userTodayMessages.Count();

                var limitRepository = new LimitRepository();
                var limits = limitRepository.GetAll();
                var currentUserLimit = limits
                    .Where(m => m.ChatId == update.Message.Chat.Id
                        && m.UserId == update.Message.From.Id).LastOrDefault();

                if (currentUserLimit != null)
                {
                    if (userTodayMessagesCount >= currentUserLimit.Count)
                    {
                        var timeHelper = new TimeHelper(_config.TimeZone);
                        var untilDate = timeHelper.GetEndOfDayUtc();

                        await _bot.RestrictChatMember(
                            chatId: update.Message.Chat.Id,
                            userId: update.Message.From.Id,
                            permissions: new ChatPermissions() { CanSendMessages = false },
                            untilDate: untilDate
                        );

                        await _bot.SendMessage(chatId: update.Message.Chat.Id, text: $"User {update.Message.From.Username} reached message limit");
                    }
                }
            }
        }
    }
}