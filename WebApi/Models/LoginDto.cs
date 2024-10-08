namespace WebApi.Models
{
    public class LoginDto
    {
        public string Username { get; set; } =string.Empty; // The username or email of the user
        public string Password { get; set; }=string.Empty;  // The password for authentication
    }
}
