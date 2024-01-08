using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RequestsResponses;

public class PostsServiceRepository : IPostRepository
{
    private readonly HttpClient _httpClient;
    private readonly IJWTService _jwtService;

    public PostsServiceRepository(HttpClient httpClient, IJWTService jwtService)
    {
        _httpClient = httpClient;
        _jwtService = jwtService;
    }

    public async Task CreatePostAsync(PostRequest postData)
    {
        string jsonContent = JsonSerializer.Serialize(postData);

        var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:5074/api/Posts");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwtService.GetJWTTokenFromHeader());
        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Post creation failed");
        }
    }

    public Task DeletePostAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task EditPostAsync(PostRequest post)
    {
        throw new NotImplementedException();
    }

    public Task<PostRequest> GetPostAsync(int id)
    {
        throw new NotImplementedException();
    }

    // Implement GetPostAsync, DeletePostAsync, and EditPostAsync using _httpClient to make requests to the microservice
}

