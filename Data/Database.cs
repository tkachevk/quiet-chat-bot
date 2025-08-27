namespace QuietChatBot.Data;

using Microsoft.Data.Sqlite;

public static class Database
{
    private static string _connectionString = "Data source=quietchatbot.db";

    public static SqliteConnection GetConnection() =>
        new SqliteConnection(_connectionString);
}