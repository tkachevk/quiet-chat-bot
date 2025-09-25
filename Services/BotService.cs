namespace QuietChatBot.Services;

using Telegram.Bot.Types;
using Serilog;
using QuietChatBot.Commands;

public class BotService
{
    private readonly IEnumerable<IBotCommand> _commands;
    private readonly MessageHandlerService _messageHandlerService;

    public BotService(
        IEnumerable<IBotCommand> commands,
        MessageHandlerService messageHandlerService
    )
    {
        _commands = commands;
        _messageHandlerService = messageHandlerService;
    }

    public async Task HandleUpdateAsync(
        Update update,
        CancellationToken ct)
    {
        if (update.Message == null) return;

        Log.Information("New update");

        var command = _commands.FirstOrDefault(c => c.CanHandle(update.Message));
        if (command != null)
        {
            await command.HandleAsync(update.Message, ct);
            return;
        }

        await _messageHandlerService.HandleMessageAsync(update.Message, ct);
    }
}