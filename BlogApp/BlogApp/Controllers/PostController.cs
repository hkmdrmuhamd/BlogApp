﻿using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    public class PostController: Controller
    {
        public IPostRepository _repository;
        public PostController(IPostRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var posts = _repository.Posts.ToList();
            return View(posts);
        }
    }
}
