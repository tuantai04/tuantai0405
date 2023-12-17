using TechBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TechBlog.Components
{
    [ViewComponent(Name = "TrendPost")]
    public class TrendPostComponent : ViewComponent
    {
        private readonly TechBlogContext _context;
        public TrendPostComponent(TechBlogContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listPost = _context.Posts.AsNoTracking().OrderByDescending(m => m.ViewNumber).Where(m => m.IsDelete == false && m.IsActive == true).Include(m => m.Category).Take(5).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", listPost));
        }
    }
}
