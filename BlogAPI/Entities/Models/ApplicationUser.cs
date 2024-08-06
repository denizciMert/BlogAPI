using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Entities.Models
{
    // User model class with identity user
    public class ApplicationUser : IdentityUser
    {
        public ICollection<BlogPost> BlogPosts { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
