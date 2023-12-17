using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TechBlog.Models;

namespace TechBlog.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly TechBlogContext _context;
		public HomeController(ILogger<HomeController> logger, TechBlogContext context)
		{
			_logger = logger;
			_context = context;
		}

		public IActionResult Index()
		{
			var PostByCate = new List<PostByCategory>();
			var category = _context.Categories.Where(m => m.IsActive == true && m.IsDelete == false).ToList();
			var post = _context.Posts.AsNoTracking().OrderByDescending(m => m.CreatedDate).Where(m => m.IsDelete == false && m.IsActive == true).Include(m => m.CreatedByNavigation).Include(m => m.Category).Take(12).ToList();
			/*foreach(var item in category)
			{
				var postByCategory = new PostByCategory();
				postByCategory.category = item;
				postByCategory.posts = post.Where(m => m.CategoryId == item.CategoryId).Take(3).ToList();
				PostByCate.Add(postByCategory);
			}*/
			ViewBag.PostHeader = post.Take(3).ToList();
			return View(post);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}