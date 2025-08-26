using Microsoft.Data.Sqlite;

public static class DatabaseConnection
{
    private static string _connectionString = "Data source=quietchatbot.db";

    public static SqliteConnection GetConnection() =>
        new SqliteConnection(_connectionString);

    public static void Initialize()
    {
        using var connection = GetConnection();
        connection.Open();

        string initSqlQuery = @"
            CREATE TABLE IF NOT EXISTS Messages (
                MessageId INTEGER PRIMARY KEY AUTOINCREMENT,
                MessageSendDate TEXT NOT NULL,
                Message TEXT NOT NULL,
                ChatId INTEGER NOT NULL,
                UserId INTEGER NOT NULL
            )
        ";

        using var cmd = new SqliteCommand(initSqlQuery, connection);
        cmd.ExecuteNonQuery();
    }
}