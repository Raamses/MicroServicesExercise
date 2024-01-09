public class Post
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? ImageId { get; set; }
    public int UserId { get; set; }
}

class UserResponse
{
    public bool isUser { get; set; }
}



