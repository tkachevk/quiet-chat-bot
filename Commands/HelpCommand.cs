namespace QuietChatBot.Commands;

using Telegram.Bot;
using Telegram.Bot.Types;

public class HelpCommand : IBotCommand
{
    private readonly ITelegramBotClient _bot;

    public HelpCommand(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public bool CanHandle(Message message) =>
        message.Text?.StartsWith("/help") ?? false;

    public async Task HandleAsync(Message message, CancellationToken ct)
    {
        string helpText =
            "🤖 Привет! Я бот для учета лимитов сообщений в чате. Помогаю следить за активностью и ограничивать количество сообщений\n\n" +
            "📌 Доступные команды:\n" +
            "/help – показать справку\n" +
            "/limit <число> – установить лимит сообщений (например: /limit 10)\n\n" +
            "⚠️ Замечания:\n" +
            "- Команда /limit доступна только обычным пользователям чата\n" +
            "- После исчерпания лимита пользователь блокируется, а его лимиты удаляются";
        
        await _bot.SendMessage(
            chatId: message.Chat.Id,
            text: helpText
        );
    }
}