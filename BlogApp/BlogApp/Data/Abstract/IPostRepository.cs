using BlogApp.Entity;

namespace BlogApp.Data.Abstract
{
    public interface IPostRepository
    {
        IQueryable<Post> Posts { get; } //Post tablosuna erişim sağlar. IQueryable => Veritabanı üzerinde belirli bir sorgu yapmak için kullanılır.
        public void CreatePost(Post post);
        public void EditPost(Post post);
    }
}
