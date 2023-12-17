using Microsoft.AspNetCore.Mvc;
using TechBlog.Models;
using TechBlog.SessionSystem;

namespace TechBlog.Components
{
    [ViewComponent(Name = "Sidebar")]
    [Area("Admin")]
    public class HeaderComponent : ViewComponent
    {
        private readonly TechBlogContext _context;
        public HeaderComponent(TechBlogContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var RoleId = HttpContext.Session.GetInt32(SessionKey.ROLEID);
            ViewBag.RoleId = RoleId;
            return await Task.FromResult((IViewComponentResult)View("Default"));
        }
    }
}
