namespace BlogApp.Entity
{
    public class User
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>(); //Her kullanıcının birçok postu olabilir
        public List<Comment> Comments { get; set; } = new List<Comment>(); //Her kullanıcının birçok yorumu olabilir
    }   
}
