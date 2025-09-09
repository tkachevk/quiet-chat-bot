namespace QuietChatBot.Services;

using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using QuietChatBot.Repositories;
using QuietChatBot.Helpers;

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

            await _bot.SendMessage(chatId: update.Message.Chat.Id, text: $"User {update.Message.From.Username} send messages: {userTodayMessagesCount}");

            if (userTodayMessagesCount > 10)
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