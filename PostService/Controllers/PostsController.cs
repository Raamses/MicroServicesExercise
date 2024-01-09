using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly PostContext _context;
    private readonly ILogger<PostsController> _logger;
    private readonly HttpClient _httpClient;

    public PostsController(PostContext context, ILogger<PostsController> logger, HttpClient httpClient)
    {
        _context = context;
        _logger = logger;
        _httpClient = httpClient;
    }

    [HttpPost("createpost")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> CreatePost([FromBody] Post post)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var token = GetJWTTokenFromHeader();

        var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5108/api/Users/is_user?userId={post.UserId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var userResponse = await _httpClient.SendAsync(request);

        if (!userResponse.IsSuccessStatusCode)
        {
            return BadRequest("Invalid user ID");
        }

        var content = await userResponse.Content.ReadAsStringAsync();
        _logger.LogInformation("response {content}", content);
        var userResponseJson = JsonSerializer.Deserialize<UserResponse>(content);

        _logger.LogInformation("Deserialized UserResponse: {userResponseJson}", userResponseJson);

        if (userResponseJson.isUser == false)
        {
            return NotFound("User not found");
        }

        await _context.AddAsync(post);
        await _context.SaveChangesAsync();

        return Ok("OK");
    }

    private string GetJWTTokenFromHeader()
    {
        return HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    }
}