namespace BlogApp.Entity
{
    public class Tag
    {
        public int TagId { get; set; }
        public string? Text { get; set; }

        public List<Post> Posts { get; set; } = new List<Post>(); //Her tagın birçok postu olabilir
    }
}
