using TechBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TechBlog.Components
{
    [ViewComponent(Name = "MenuHeader")]
    public class MenuHeaderComponent : ViewComponent
    {
        private readonly TechBlogContext _context;
        public MenuHeaderComponent(TechBlogContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listCategory = _context.Categories.Where(m => m.IsDelete == false && m.IsActive == true).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", listCategory));
        }
    }
}
