using RequestsResponses;

public class PostService
{
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task CreateWelcomePostForUserAsync(User user)
    {
        var postData = new PostRequest
        {
            Title = $"Welcome All from {user.Username}",
            Description = "Welcome to blafhhabf",
            ImageId = 0,
            UserId = user.Id
        };

        await _postRepository.CreatePostAsync(postData);
    }

    public async Task<PostRequest> GetPostAsync(int id)
    {
        return await _postRepository.GetPostAsync(id);
    }

    public async Task DeletePostAsync(int id)
    {
        await _postRepository.DeletePostAsync(id);
    }

    public async Task EditPostAsync(PostRequest post)
    {
        await _postRepository.EditPostAsync(post);
    }
}