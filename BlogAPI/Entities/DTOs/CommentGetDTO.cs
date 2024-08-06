namespace BlogAPI.Entities.DTOs
{
    // Comment DTO for GET model class
    public class CommentGetDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int LikeCount { get; set; }
        public Guid? ParentCommentId { get; set; }
        public ICollection<CommentGetDTO> Replies { get; set; }
    }

}
