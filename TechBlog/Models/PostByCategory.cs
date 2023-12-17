namespace TechBlog.Models
{
    public class PostByCategory
    {
        public Category? category { get; set; }
        public IList<Post>? posts { get; set; }
    }
}
