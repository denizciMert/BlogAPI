namespace BlogAPI.Entities.DTOs
{
    // BlogPost DTO for GET model class
    public class BlogPostGetDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int LikeCount { get; set; }
        public ICollection<CommentGetDTO> Comments { get; set; }
    }
}
