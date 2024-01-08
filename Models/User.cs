public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public bool IsActive { get; set; }
}

namespace RequestsResponses
{
    class UserResponse
    {
        public bool isUser { get; set; }
    }

    public class PostRequest
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? ImageId { get; set; }
        public int UserId { get; set; }
    }
}

