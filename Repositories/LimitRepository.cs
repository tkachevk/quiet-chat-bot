namespace QuietChatBot.Repositories;

using Microsoft.EntityFrameworkCore;
using QuietChatBot.Models;

public class LimitRepository
{
    private readonly AppDbContext _context;

    public LimitRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Limit>> GetAllAsync()
    {
        return await _context.Limits.ToListAsync();
    }

    public async Task AddAsync(Limit limit)
    {
        await _context.AddAsync(limit);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long userId, long chatId)
    {
        var messagesToDelete = await _context.Limits
            .Where(l => l.UserId == userId && l.ChatId == chatId)
            .ExecuteDeleteAsync();
    }
}