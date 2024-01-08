using RequestsResponses;

public interface IPostRepository
{
    Task CreatePostAsync(PostRequest postData);
    Task<PostRequest> GetPostAsync(int id);
    Task DeletePostAsync(int id);
    Task EditPostAsync(PostRequest post);
}