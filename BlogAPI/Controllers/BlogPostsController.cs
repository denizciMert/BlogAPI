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
    public class BlogPostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogPostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/blogpost
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPostGetDTO>>> GetBlogPosts()
        {
            var blogPosts = await _context.BlogPosts
                .Include(bp => bp.User)
                .Include(bp => bp.Comments)
                    .ThenInclude(c => c.Replies)
                .Include(bp => bp.Likes)
                .ToListAsync();

            var blogPostDTOs = blogPosts.Select(bp => new BlogPostGetDTO
            {
                Id = bp.Id,
                Title = bp.Title,
                Content = bp.Content,
                CreatedAt = bp.CreatedAt,
                UserId = bp.UserId,
                UserName = bp.User?.UserName ?? "Unknown",
                LikeCount = bp.Likes?.Count ?? 0,
                Comments = MapComments(bp.Comments.Where(c => c.ParentCommentId == null).ToList())
            }).ToList();

            return Ok(blogPostDTOs);
        }

        // GET: api/blogpost/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPostGetDTO>> GetBlogPost(Guid id)
        {
            var blogPost = await _context.BlogPosts
                .Include(bp => bp.User)
                .Include(bp => bp.Comments)
                    .ThenInclude(c => c.Replies)
                .Include(bp => bp.Likes)
                .FirstOrDefaultAsync(bp => bp.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            var blogPostDTO = new BlogPostGetDTO
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Content = blogPost.Content,
                CreatedAt = blogPost.CreatedAt,
                UserId = blogPost.UserId,
                UserName = blogPost.User?.UserName ?? "Unknown",
                LikeCount = blogPost.Likes?.Count ?? 0,
                Comments = MapComments(blogPost.Comments.Where(c => c.ParentCommentId == null).ToList())
            };

            return Ok(blogPostDTO);
        }

        // POST: api/blogpost
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BlogPost>> PostBlogPost([FromBody] BlogPostPostDTO blogPostDTO)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var blogPost = new BlogPost
            {
                Title = blogPostDTO.Title,
                Content = blogPostDTO.Content,
                UserId = user.Id
            };

            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBlogPost), new { id = blogPost.Id }, blogPost);
        }

        // PUT: api/blogpost/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBlogPost(Guid id, [FromBody] BlogPostPostDTO blogPostDTO)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            if (blogPost.UserId != user.Id)
            {
                return Forbid();
            }

            blogPost.Title = blogPostDTO.Title;
            blogPost.Content = blogPostDTO.Content;

            _context.Entry(blogPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/blogpost/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlogPost(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            if (blogPost.UserId != user.Id)
            {
                return Forbid();
            }

            blogPost.Title = $"{blogPost.Title} - This post has been deleted.";

            _context.Entry(blogPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

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
    }
}
