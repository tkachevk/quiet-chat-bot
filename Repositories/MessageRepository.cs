namespace QuietChatBot.Repositories;

using QuietChatBot.Models;
using QuietChatBot.Data;

public class MessageRepository
{
    public List<Message> GetAll()
    {
        List<Message> messages = new List<Message>();

        using var connection = Database.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Messages;";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            messages.Add(new Message
            {
                MessageId = reader.GetInt32(0),
                MessageSendDate = DateTime.Parse(reader.GetString(1)),
                ChatId = reader.GetInt64(2),
                UserId = reader.GetInt64(3)
            });
        }

        return messages;
    }

    public void Add(Message message)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Messages (MessageSendDate, ChatId, UserId)
            VALUES ($messageSendDate, $chatId, $userId);
        ";

        cmd.Parameters.AddWithValue("$messageSendDate", message.MessageSendDate.ToString());
        cmd.Parameters.AddWithValue("$chatId", message.ChatId);
        cmd.Parameters.AddWithValue("$userId", message.UserId);

        cmd.ExecuteNonQuery();
    }
}