namespace BlogAPI.Entities.Models
{
    // Comment model class
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }
        public Guid BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Comment> Replies { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
