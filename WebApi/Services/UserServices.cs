using Microsoft.EntityFrameworkCore;
using WebApi.Data; // Ensure this is your DbContext namespace
using WebApi.Models; // Ensure this is your models namespace
namespace WebApi.Services {


public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateUserAsync(AppUser newUser)
    {
        _context.User.Add(newUser);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<AppUser> AuthenticateUserAsync(string username, string password)
    {
        return await _context.User
            .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password); // Passwords should be hashed
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        return await _context.User.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> UpdateUserAsync(string username, UpdateUserDto updateUserDto)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Username == username);
        if (user != null)
        {
            // Update user fields
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteUserAsync(string username)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Username == username);
        if (user != null)
        {
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}
}