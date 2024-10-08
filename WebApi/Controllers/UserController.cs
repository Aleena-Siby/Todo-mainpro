using Microsoft.AspNetCore.Mvc;
using WebApi.Services; // Adjust according to your namespace
using WebApi.Models; // Adjust according to your namespace
using Microsoft.EntityFrameworkCore;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // Get user details
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username)
    {
        var user = await _userService.GetUserByUsernameAsync(username);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        return Ok(user);
    }

    // Update user details
    [HttpPut("{username}")]
    public async Task<IActionResult> UpdateUser(string username, [FromBody] UpdateUserDto updateUserDto)
    {
        var result = await _userService.UpdateUserAsync(username, updateUserDto);
        if (!result)
        {
            return BadRequest("Failed to update user.");
        }
        return NoContent();
    }

    // Optional: Delete user
    [HttpDelete("{username}")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var result = await _userService.DeleteUserAsync(username);
        if (!result)
        {
            return NotFound("User not found.");
        }
        return NoContent();
    }
}
