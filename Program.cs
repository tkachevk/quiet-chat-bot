using Telegram.Bot;
using Telegram.Bot.Polling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuietChatBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using QuietChatBot.Repositories;
using Serilog;
using QuietChatBot.Commands;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("bot.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var builder = Host.CreateApplicationBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true);
        builder.Services.Configure<AppConfig>(builder.Configuration);
        builder.Services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var cfg = sp.GetRequiredService<IOptions<AppConfig>>().Value;
            return new TelegramBotClient(cfg.BotToken);
        });
        builder.Services.AddSingleton<BotService>();
        builder.Services.AddSingleton<MessageHandlerService>();
        builder.Services.AddSingleton<IUpdateHandler, BotUpdateHandler>();
        builder.Services.AddScoped<LimitRepository>();
        builder.Services.AddScoped<MessageRepository>();
        builder.Services.AddSingleton<IBotCommand, LimitCommand>();
        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=app.db"));

        var app = builder.Build();

        // db init
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        var botClient = app.Services.GetRequiredService<ITelegramBotClient>();
        var updateHandler = app.Services.GetRequiredService<IUpdateHandler>();

        var cts = new CancellationTokenSource();

        botClient.StartReceiving(
            updateHandler: updateHandler,
            receiverOptions: new ReceiverOptions(),
            cancellationToken: cts.Token
        );

        Log.Information("Bot started");

        await app.RunAsync();
    }
}