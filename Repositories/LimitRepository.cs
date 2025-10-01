namespace QuietChatBot.Repositories;

using QuietChatBot.Models;
using QuietChatBot.Data;

public class LimitRepository
{
    public List<Limit> GetAll()
    {
        var limits = new List<Limit>();

        using var connection = Database.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Limits;";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            limits.Add(new Limit
            {
                LimitId = reader.GetInt32(0),
                ChatId = reader.GetInt64(1),
                UserId = reader.GetInt64(2),
                Count = reader.GetInt32(3)
            });
        }

        return limits;
    }

    public void Add(Limit limit)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Limits (ChatId, UserId, Count)
            VALUES ($chatId, $userId, $count);
        ";

        cmd.Parameters.AddWithValue("$chatId", limit.ChatId);
        cmd.Parameters.AddWithValue("$userId", limit.UserId);
        cmd.Parameters.AddWithValue("$count", limit.Count);

        cmd.ExecuteNonQuery();
    }

    public void Delete(long userId, long chatId)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            DELETE FROM Limits
            WHERE UserId = $userId AND ChatId = $chatId;
        ";

        cmd.Parameters.AddWithValue("$userId", userId);
        cmd.Parameters.AddWithValue("$chatId", chatId);

        cmd.ExecuteNonQuery();
    }
}