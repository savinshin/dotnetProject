using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(ILogger<UserController> logger, UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserCreationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(UserCreationResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UserCreationResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserCreationResponse>> CreateUser(
        [FromBody] UserCreationRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
        {
            return Conflict(new UserCreationResponse
            {
                Success = false,
                Errors = new() { "Email is already taken." }
            });
        }

        var user = new IdentityUser
        {
            UserName = string.IsNullOrWhiteSpace(request.UserName) ? request.Email : request.UserName,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("User created successfully: {Email}", request.Email);
            return Created($"/api/users/{user.Id}", new UserCreationResponse
            {
                Success = true,
                UserId = user.Id,
                Email = user.Email
            });
        }

        var errors = result.Errors.Select(e => e.Description).ToList();
        _logger.LogWarning("User creation failed: {Errors}", string.Join("; ", errors));

        return BadRequest(new UserCreationResponse
        {
            Success = false,
            Errors = errors
        });
    }

    public class UserCreationRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
    }

    public class UserCreationResponse
    {
        public bool Success { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public List<string>? Errors { get; set; }
    }
}
