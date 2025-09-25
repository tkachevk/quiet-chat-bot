namespace QuietChatBot.Commands;

using Telegram.Bot.Types;

public interface IBotCommand
{
    bool CanHandle(Message message);
    Task HandleAsync(Message message, CancellationToken cancellationToken);
}