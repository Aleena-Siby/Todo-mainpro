using WebApi.Models;
public interface IUserService
{
    Task<AppUser> GetUserByUsernameAsync(string username);
    Task<bool> UpdateUserAsync(string username, UpdateUserDto updateUserDto);
    Task<bool> DeleteUserAsync(string username);
    Task<bool> CreateUserAsync(AppUser newUser); // For user registration
    Task<AppUser> AuthenticateUserAsync(string username, string password); // For login
}
