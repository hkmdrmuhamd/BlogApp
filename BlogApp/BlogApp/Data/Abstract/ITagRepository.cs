using BlogApp.Entity;

namespace BlogApp.Data.Abstract
{
    public interface ITagRepository
    {
        IQueryable<Tag> Tags { get; } 
        public void CreateTag(Tag tags);
    }
}
