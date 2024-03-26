using System.Security.Claims;
using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BlogApp.Controllers
{
    public class PostController : Controller
    {
        private IPostRepository _postRepository;
        private ICommentRepository _commentRepository;
        private ITagRepository _tagRepository;
        public PostController(IPostRepository postRepository, ICommentRepository commentRepository, ITagRepository tagRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
        }

        public async Task<IActionResult> Index(string tag)
        {
            var claims = User.Claims; // Kullanıcı bilgilerini almak için kullandık.
            var posts = _postRepository.Posts.Where(x => x.IsActive);
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
            .Include(x => x.User)
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

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
            var role = User.FindFirstValue(ClaimTypes.Role);

            var post = await _postRepository.Posts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.PostId == id);
            ViewBag.Tags = _tagRepository.Tags.ToList();

            if (post == null)
            {
                return NotFound();
            }
            else
            {
                return View(new PostCreateViewModel
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    Content = post.Content,
                    Description = post.Description,
                    Url = post.Url,
                    Image = post.Image,
                    IsActive = post.IsActive,
                    Tags = post.Tags
                });
            }

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(PostCreateViewModel postCreateViewModel, IFormFile imageFile, int[] tagIds)
        {
            var post = await _postRepository.Posts.FirstOrDefaultAsync(x => x.PostId == postCreateViewModel.PostId);
            if (post != null)
            {
                if (post.UserId == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "") || User.FindFirstValue(ClaimTypes.Role) == "admin")
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
                        }
                    }
                    else
                    {
                        var existingPost = await _postRepository.Posts.FirstOrDefaultAsync(x => x.PostId == postCreateViewModel.PostId);
                        if (existingPost != null)
                        {
                            postCreateViewModel.Image = existingPost.Image;
                        }
                    }
                    var updatePost = new Post
                    {
                        PostId = postCreateViewModel.PostId,
                        Title = postCreateViewModel.Title,
                        Content = postCreateViewModel.Content,
                        Description = postCreateViewModel.Description,
                        Url = postCreateViewModel.Url,
                        Image = postCreateViewModel.Image,
                    };
                    if (User.FindFirstValue(ClaimTypes.Role) == "admin")
                    {
                        updatePost.IsActive = postCreateViewModel.IsActive;
                    }

                    _postRepository.EditPost(updatePost, tagIds);
                    return RedirectToAction("List");
                }
            }
            ViewBag.Tags = _tagRepository.Tags.ToList();
            return View(postCreateViewModel);
        }
    }
}
