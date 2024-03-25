using System.Security.Claims;
using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize] //Authorize olmamış bir kullanıcının bu sayfayı görüntülemeini engeller
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync(PostCreateViewModel postCreateViewModel, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var extension = Path.GetExtension(imageFile.FileName).ToLower();
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Sadece jpg, jpeg ve png uzantılı doslayaları seçebilirsiniz.");
                    }
                    else
                    {
                        var randomFileName = $"{Guid.NewGuid().ToString()}{extension}";
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        postCreateViewModel.Image = randomFileName;
                        var entity = new Post
                        {
                            Title = postCreateViewModel.Title,
                            Content = postCreateViewModel.Content,
                            Description = postCreateViewModel.Description,
                            Url = postCreateViewModel.Url,
                            PublishedOn = DateTime.Now,
                            IsActive = false,
                            Image = postCreateViewModel.Image,
                            UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "")
                        };
                        _postRepository.CreatePost(entity);
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Lütfen bir resim seçiniz.");
                }
            }
            return View(postCreateViewModel);
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
            var role = User.FindFirstValue(ClaimTypes.Role);

            var posts = _postRepository.Posts;

            if (string.IsNullOrEmpty(role))
            {
                posts = posts.Where(x => x.UserId == userId);
            }
            return View(await posts.ToListAsync());
        }
    }
}
