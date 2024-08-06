namespace BlogAPI.Entities.DTOs
{
    // Like DTO for POST model class
    public class LikePostDTO
    {
        public Guid? BlogPostId { get; set; }
        public Guid? CommentId { get; set; }
    }

}
