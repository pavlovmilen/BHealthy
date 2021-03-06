﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthSocialMediaApp.Data;
using HealthSocialMediaApp.Models;


namespace HealthSocialMediaApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnv;

        public PostsController(ApplicationDbContext context, IWebHostEnvironment hostingEnv)
        {
            _context = context;
            _hostingEnv = hostingEnv;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<System.Collections.IEnumerable>> GetPosts(string currentUserId, string userId, bool? following)
        {
            var post = await (from p in _context.Posts
                              let amountOfLikes = (from like in _context.Likes where like.PostId == p.Id select like.Id).Count()
                              let isLikedByCurrentUser = currentUserId != null && (from like in _context.Likes where like.PostId == p.Id && like.ApplicationUserId == currentUserId select like.Id).Any()
                              where (userId != null && p.ApplicationUser.Id == userId) || (userId == null)
                              where (following != null && _context.Followers.Where(d => d.FollowerId == currentUserId).Select(r => r.FolloweeId).Contains(p.ApplicationUserId)) || (following == null)
                              select new
                              {
                                  p.Id,
                                  p.ImageLink,
                                  p.Description,
                                  p.CategoryId,
                                  p.Category.Name,
                                  p.CreatedAt,
                                  p.ApplicationUser.UserName,
                                  UserId = p.ApplicationUserId,
                                  amountOfLikes,
                                  isLikedByCurrentUser
                              }).OrderByDescending(d => d.CreatedAt).ToListAsync();
            return post;
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5/like
        [Authorize]
        [HttpPut("{id}/like")]
        public async Task<ActionResult<Post>> PutPostLike(int id, string userId)
        {
            if (!IsAuthenticatedUser(userId))
            {
                return Forbid();
            }

            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return BadRequest();
            }

            bool userAlreadyLikedPost = false;
            foreach (Like like in _context.Likes)
            {
                if (like.ApplicationUserId.Equals(userId) && like.PostId.Equals(id))
                {
                    userAlreadyLikedPost = true;
                }
            }

            if (userAlreadyLikedPost)
            {
                return BadRequest();
            }

            Like correspondingLike = new Like();
            correspondingLike.PostId = id;
            correspondingLike.ApplicationUserId = userId;

            _context.Likes.Add(correspondingLike);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // PUT: api/Posts/5/unlike
        [Authorize]
        [HttpPut("{id}/unlike")]
        public async Task<ActionResult<Post>> PutPostUnlike(int id, string userId)
        {
            if (!IsAuthenticatedUser(userId))
            {
                return Forbid();
            }

            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return BadRequest();
            }

            bool userHasNotLikedPost = true;
            foreach (Like like in _context.Likes)
            {
                if (like.ApplicationUserId.Equals(userId) && like.PostId.Equals(id))
                {
                    _context.Likes.Remove(like);
                    userHasNotLikedPost = false;
                }
            }

            if (userHasNotLikedPost)
            {
                return BadRequest();
            }

            await _context.SaveChangesAsync();

            return Ok();
        }


        // POST: api/Posts
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost([FromBody] Post post)
        {
            if (!IsAuthenticatedUser(post.ApplicationUserId))
            {
                return Forbid();
            }

            if (post.CategoryId == 0)
            {
                post.CategoryId = -7;
            }

            post.CreatedAt = DateTime.Now;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Posts/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Post>> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            if (!IsAuthenticatedUser(post.ApplicationUserId))
            {
                return Forbid();
            }

            ImagesController.DeleteImage(post.ImageLink, _hostingEnv);

            _context.Posts.Remove(post);

            await _context.SaveChangesAsync();

            return post;
        }

        private bool IsAuthenticatedUser(string userId)
        {
            return userId == User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
