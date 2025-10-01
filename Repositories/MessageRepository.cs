namespace QuietChatBot.Repositories;

using Microsoft.EntityFrameworkCore;
using QuietChatBot.Models;

public class MessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Message>> GetAllAsync()
    {
        return await _context.Messages.ToListAsync();
    }

    public async Task AddAsync(Message message)
    {
        await _context.AddAsync(message);
        await _context.SaveChangesAsync();
    }
}