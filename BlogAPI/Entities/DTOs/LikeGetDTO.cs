namespace BlogAPI.Entities.DTOs
{
    // Like DTO for GET model class
    public class LikeGetDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid? BlogPostId { get; set; }
        public Guid? CommentId { get; set; }
    }

}
