namespace BlogAPI.Entities.DTOs
{
    // Comment DTO for POST model class
    public class CommentPostDTO
    {
        public string Content { get; set; }
        public Guid? ParentCommentId { get; set; }
        public Guid BlogPostId { get; set; }
    }

}
