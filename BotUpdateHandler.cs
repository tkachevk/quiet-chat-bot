using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using QuietChatBot.Services;
using Serilog;

public class BotUpdateHandler : IUpdateHandler
{
    private readonly BotService _botService;

    public BotUpdateHandler(BotService botService)
    {
        _botService = botService;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        await _botService.HandleUpdateAsync(update, ct);
    }

    public Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, HandleErrorSource hes, CancellationToken ct)
    {
        Log.Error(ex.Message);
        return Task.CompletedTask;
    }
}
