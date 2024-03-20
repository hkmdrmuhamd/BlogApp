using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class PostController : Controller
    {
        public IPostRepository _postRepository;
        public PostController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public IActionResult Index()
        {
            return View(
                new PostViewModel()
                {
                    Posts = _postRepository.Posts.ToList()
                }
            );
        }
        public async Task<IActionResult> Details(int id)
        {
            return View(await _postRepository.Posts.FirstOrDefaultAsync(p => p.PostId == id));
        }
    }
}
