using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtService _jwtService;

    public AuthController(UserService userService, JwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(RegisterDto dto)
    {
        try
        {
            var user = await _userService.RegisterAsync(dto.Name, dto.Email, dto.Password);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }
        catch (UserAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred during user registration.");
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto dto)
    {
        try
        {
            var user = await _userService.LoginAsync(dto.Email, dto.Password);
            var token = _jwtService.GenerateToken(user);
            return Ok(token);
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred during user login.");
        }
    }
}