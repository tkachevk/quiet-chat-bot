namespace QuietChatBot.Data;

using Microsoft.Data.Sqlite;

public static class DbInitializer
{
    public static void Initialize()
    {
        using var connection = Database.GetConnection();
        connection.Open();

        string initSqlQuery = @"
            CREATE TABLE IF NOT EXISTS Messages (
                MessageId INTEGER PRIMARY KEY AUTOINCREMENT,
                MessageSendDate TEXT NOT NULL,
                ChatId INTEGER NOT NULL,
                UserId INTEGER NOT NULL
            )
        ";

        using var cmd = new SqliteCommand(initSqlQuery, connection);
        cmd.ExecuteNonQuery();
    }
}