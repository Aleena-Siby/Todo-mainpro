using WebApi.Models; 
namespace WebApi.Services{
    public interface IPasswordHasher
{
    string HashPassword(string password);
}
}