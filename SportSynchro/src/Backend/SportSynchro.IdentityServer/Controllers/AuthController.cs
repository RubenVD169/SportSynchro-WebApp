using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportSynchro.IdentityServer.Dtos;
using SportSynchro.IdentityServer.Models;

namespace SportSynchro.IdentityServer.Controllers;

[ApiController]
[Route("auth")]
[EnableCors("FrontendCors")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // 1. Basic validation
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // 2. Check existing user
        IdentityUser? existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser != null)
        {
            return BadRequest(new
            {
                error = "Username already exists"
            });
        }
        // 3. Check existing email
        ApplicationUser? existingEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
        {
            return BadRequest(new
            {
                error = "Email address is already in use"
            });
        }

        // 4. Create user
        ApplicationUser user = new()
        {
            UserName = request.Username,
            Email = request.Email
        };

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                errors = result.Errors.Select(e => e.Description)
            });
        }

        // 5. Default role
        await _userManager.AddToRoleAsync(user, "User");

        return Created("", new
        {
            message = "User registered successfully"
        });
    }
}