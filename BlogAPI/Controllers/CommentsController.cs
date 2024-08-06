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
    public class CommentsController : ControllerBase
    {
        // Dependency injection for database context and user manager
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to initialize the context and user manager
        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentGetDTO>>> GetComments()
        {
            // Retrieve comments with related entities
            var comments = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Replies)
                .Include(c => c.Likes)
                .ToListAsync();

            // Map comments to DTOs and filter root comments
            var commentDTOs = comments
                .Where(c => c.ParentCommentId == null)
                .Select(c => MapComment(c))
                .ToList();

            return Ok(commentDTOs);
        }

        // GET: api/comments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentGetDTO>> GetComment(Guid id)
        {
            // Retrieve comment by id with related entities
            var comment = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Replies)
                .Include(c => c.Likes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            // Map comment to DTO
            var commentDTO = new CommentGetDTO
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UserId = comment.UserId,
                UserName = comment.User?.UserName ?? "Unknown",
                LikeCount = comment.Likes?.Count ?? 0,
                ParentCommentId = comment.ParentCommentId,
                Replies = null
            };

            return Ok(commentDTO);
        }

        // POST: api/comments
        [HttpPost]
        [Authorize]  // Authorization required for this action
        public async Task<ActionResult<Comment>> PostComment([FromBody] CommentPostDTO commentDTO)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Create a new comment
            var comment = new Comment
            {
                Content = commentDTO.Content,
                BlogPostId = commentDTO.BlogPostId,
                ParentCommentId = commentDTO.ParentCommentId,
                UserId = user.Id
            };

            // Add the new comment to the database
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        // PUT: api/comments/{id}
        [HttpPut("{id}")]
        [Authorize]  // Authorization required for this action
        public async Task<IActionResult> PutComment(Guid id, [FromBody] CommentPostDTO commentDTO)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Retrieve comment by id
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            // Check if the user is authorized to update the comment
            if (comment.UserId != user.Id)
            {
                return Forbid();
            }

            // Update the comment properties
            comment.Content = commentDTO.Content;
            comment.BlogPostId = commentDTO.BlogPostId;
            comment.ParentCommentId = commentDTO.ParentCommentId;

            // Mark the comment as modified and save changes
            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/comments/{id}
        [HttpDelete("{id}")]
        [Authorize]  // Authorization required for this action
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Retrieve comment by id with related entities
            var comment = await _context.Comments
                .Include(c => c.Replies)
                .Include(c => c.Likes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            // Check if the user is authorized to delete the comment
            if (comment.UserId != user.Id)
            {
                return Forbid();
            }

            // Mark the comment as deleted and clear likes
            comment.Content = "This comment has been deleted.";
            comment.Likes.Clear();

            // Mark the comment as modified and save changes
            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Maps a list of comments to CommentGetDTOs
        private List<CommentGetDTO> MapComments(IEnumerable<Comment> comments)
        {
            return comments.Select(c => new CommentGetDTO
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UserId = c.UserId,
                UserName = c.User?.UserName ?? "Unknown",
                LikeCount = c.Likes?.Count ?? 0,
                ParentCommentId = c.ParentCommentId,
                Replies = MapComments(c.Replies)
            }).ToList();
        }

        // Maps a single comment to CommentGetDTO
        private CommentGetDTO MapComment(Comment comment)
        {
            return new CommentGetDTO
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UserId = comment.UserId,
                UserName = comment.User?.UserName ?? "Unknown",
                LikeCount = comment.Likes?.Count ?? 0,
                ParentCommentId = comment.ParentCommentId,
                Replies = MapComments(comment.Replies.Where(r => r.ParentCommentId == comment.Id))
            };
        }
    }
}
