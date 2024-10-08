using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApi.Data;  // Namespace for your DbContext
using WebApi.Models; // Namespace for your DTOs and Models
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Check if the user already exists
            if (await _context.User.AnyAsync(u => u.Username == registerDto.Username))
            {
                return BadRequest("Username already exists.");
            }

            // Create a new user instance
            var user = new AppUser
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password)  // Store hashed password
            };

            // Add the user to the database
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Registration successful.");
        }
[HttpGet]
public IActionResult Test()
{
    return Ok("Auth Controller is working.");
}





        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials.");
            }

            // Return success message or generate JWT token (if implemented)
            return Ok("Login successful.");
        }

        // Password hashing method (you can use more advanced methods like bcrypt in production)
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Password verification method
        private bool VerifyPassword(string password, string storedHash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == storedHash;
        }
    }
}
