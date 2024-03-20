﻿using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.ViewComponents
{
    public class NewPosts : ViewComponent
    {
        private IPostRepository _postRepository;
        public NewPosts(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _postRepository
                            .Posts
                            .OrderByDescending(p => p.PublishedOn)//OrdeByDecending: Tarihe göre sıralama yapar. En yeni post en üstte olur.
                            .Take(5).
                            ToListAsync()
            );
        }
    }
}
