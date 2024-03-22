using System.Security.Claims;
using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class PostController : Controller
    {
        private IPostRepository _postRepository;
        private ICommentRepository _commentRepository;
        public PostController(IPostRepository postRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
        }

        public async Task<IActionResult> Index(string tag)
        {
            var claims = User.Claims; // Kullanıcı bilgilerini almak için kullandık.
            var posts = _postRepository.Posts;
            if (!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(x => x.Tags.Any(t => t.Url == tag));
            }
            return View(new PostViewModel { Posts = await posts.ToListAsync() });
        }
        public async Task<IActionResult> Details(string url)
        {
            return View(await _postRepository
            .Posts
            .Include(x => x.Tags)
            .Include(x => x.Comments)//Comments'e git
            .ThenInclude(x => x.User)//orada ki user'a git
            .FirstOrDefaultAsync(p => p.Url == url)
            );
        }

        [HttpPost]
        public JsonResult AddComment(int PostId, string Text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);//ClaimTypes cookie'ler içerisinden NameIdentifier(userId) değerini alır. String tipindedir.
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var avatar = User.FindFirstValue(ClaimTypes.UserData);

            var entity = new Comment
            {
                PostId = PostId,
                Text = Text,
                PublishedOn = DateTime.Now,
                UserId = int.Parse(userId ?? ""),
            };
            _commentRepository.CreateComment(entity);

            return Json(new
            {
                userName,
                Text,
                entity.PublishedOn,
                avatar
            });
        }
    }
}
