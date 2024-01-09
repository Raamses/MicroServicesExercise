using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestsResponses;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserContext _context;
    private readonly ILogger<UsersController> _logger;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly IJWTService _jwtService;
    private readonly PostService _postService;
    public UsersController(UserContext context, ILogger<UsersController> logger, IConfiguration config, HttpClient httpClient, IJWTService jwtService, PostService postService)
    {
        _context = context;
        _logger = logger;
        _config = config;
        _httpClient = httpClient;
        _jwtService = jwtService;
        _postService = postService;
    }


    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (_context.Users.Any(p => p.Username == user.Username))
        {
            return Conflict(user.Username);
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        await _postService.CreateWelcomePostForUserAsync(user);

        // var postData = new PostRequest
        // {
        //     Title = $"Welcome All from {user.Username}",
        //     Description = "Welcome to blafhhabf",
        //     ImageId = 0,
        //     UserId = user.Id
        // };

        // string jsonContent = JsonSerializer.Serialize(postData);
        var tokenString = _jwtService.GenerateToken(user.Id, user.Username);

        // var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:5074/api/Posts/createpost");
        // request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        // request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);

        // var userResponse = await _httpClient.SendAsync(request);

        // if (!userResponse.IsSuccessStatusCode)
        // {
        //     return BadRequest("Post failed to create");
        // }

        return Ok(new { token = tokenString });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized();
        }
        var tokenString = _jwtService.GenerateToken(user.Id, user.Username);
        return Ok(new { token = tokenString });
    }

    [HttpGet("is_user")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> IsUser(int userId)
    {
        var userResponse = new UserResponse();
        userResponse.isUser = await _context.Users.AnyAsync(u => u.Id == userId);

        return Ok(userResponse);
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("testicles")]
    // public IActionResult Testicles(string name)
    public async Task<IActionResult> Testicles()
    {
        var postData = new PostRequest
        {
            Title = "Welcome All from {user.Username}",
            Description = "Welcome to blafhhabf",
            ImageId = 0,
            UserId = 1
        };

        string jsonContent = JsonSerializer.Serialize(postData);
        var tokenString = _jwtService.GenerateToken(1, "tester");

        var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:5074/api/Posts/createpost");
        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);

        var userResponse = await _httpClient.SendAsync(request);
        // var user = _context.Users.FirstOrDefault(u => u.Username == name);
        // if (user != null)
        //     return Ok(new { ssstring = user.Username + " " + user.PasswordHash });
        // else
            return Ok(userResponse);
    }

    private string GetJWTTokenFromHeader()
    {
        return HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    }

}
