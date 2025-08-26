using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        DatabaseConnection.Initialize();

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Local.json", optional: true)
            .Build();

        string? token = config["BotToken"];
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Bot token missing in configuration!");

        var botClient = new TelegramBotClient(token);

        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cts.Token
        );

        var me = await botClient.GetMe(cts.Token);
        Console.WriteLine($"Bot @{me.Username} started...");

        await Task.Delay(-1, cts.Token);
    }

    static async Task HandleUpdateAsync(
        ITelegramBotClient bot,
        Update update,
        CancellationToken ct)
    {
        if (update.Message is { Text: var text, Id: var id })
        {
            await bot.SendMessage(
                chatId: update.Message.Chat.Id,
                text: $"Вы написали: {text}, {id}",
                cancellationToken: ct
            );
        }
    }

    static Task HandleErrorAsync(
        ITelegramBotClient bot,
        Exception ex,
        CancellationToken ct)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return Task.CompletedTask;
    }
}
