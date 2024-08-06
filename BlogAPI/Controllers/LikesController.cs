using BlogAPI.Data;
using BlogAPI.Entities.DTOs;
using BlogAPI.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        // Dependency injection for database context and user manager
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to initialize the context and user manager
        public LikesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: api/likes
        [HttpPost]
        [Authorize]  // Authorization required for this action
        public async Task<IActionResult> Like([FromBody] LikePostDTO likePostDTO)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();  // Return 401 if the user is not authenticated
            }

            // Validate the request
            if ((likePostDTO.BlogPostId == null && likePostDTO.CommentId == null) ||
                (likePostDTO.BlogPostId != null && likePostDTO.CommentId != null))
            {
                return BadRequest(new { message = "Invalid like request. You must specify either BlogPostId or CommentId, but not both." });
            }

            // Check if the like already exists
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == user.Id &&
                                          (l.BlogPostId == likePostDTO.BlogPostId || l.CommentId == likePostDTO.CommentId));

            if (existingLike != null)
            {
                // If like exists, remove it
                _context.Likes.Remove(existingLike);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Like removed" });
            }

            // Create a new like
            var like = new Like
            {
                UserId = user.Id,
                BlogPostId = likePostDTO.BlogPostId,
                CommentId = likePostDTO.CommentId
            };

            // Add the new like to the database
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Liked" });
        }

        // DELETE: api/likes
        [HttpDelete]
        [Authorize]  // Authorization required for this action
        public async Task<IActionResult> Unlike([FromBody] LikePostDTO likePostDTO)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();  // Return 401 if the user is not authenticated
            }

            // Validate the request
            if ((likePostDTO.BlogPostId == null && likePostDTO.CommentId == null) ||
                (likePostDTO.BlogPostId != null && likePostDTO.CommentId != null))
            {
                return BadRequest(new { message = "Invalid unlike request. You must specify either BlogPostId or CommentId, but not both." });
            }

            // Check if the like exists
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == user.Id &&
                                          (l.BlogPostId == likePostDTO.BlogPostId || l.CommentId == likePostDTO.CommentId));

            if (like == null)
            {
                return NotFound(new { message = "Like not found" });  // Return 404 if the like does not exist
            }

            // Remove the like from the database
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Like removed" });
        }
    }
}
