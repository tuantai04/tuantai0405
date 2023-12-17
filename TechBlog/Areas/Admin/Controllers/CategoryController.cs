using TechBlog.Models;
using TechBlog.Ultilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly TechBlogContext _context;
        public CategoryController(TechBlogContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
			var listCatePost = _context.Categories.Where(m => m.IsActive == true && m.IsDelete == false).ToList();
			return View(listCatePost);
		}
        
        public IActionResult Create()
        {
            var listParrentCate = _context.Categories.Where(m => m.IsActive == true && m.IsDelete == false && m.Levels == 1).ToList();
            ViewBag.ListParrentCate = new SelectList(listParrentCate.AsQueryable(), "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category cate)
        {
            if(cate == null)
            {
                return NotFound();
            }
            try
            {
                if(cate.CategoryName== null) { TempData["CateNameRequired"] = "Bạn phải nhập tên danh mục"; }
                if(cate.CategoryDesc== null) { TempData["CateDescRequired"] = "Bạn phải nhập mô tả danh mục"; }
                if ( cate.CategoryDesc == null || cate.CategoryName== null)
                {
                    return View(cate);
                }
                cate.CategorySlug = Functions.AliasLink(cate.CategoryName);
                cate.Levels = 1;
                if (cate.CategoryParrentId != 0) cate.Levels = 2;

                _context.Categories.Add(cate);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> EditCate(int id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }
            try
            {
                var CateByID = _context.Categories.Where(m => m.CategoryId == id).First();
                if (CateByID == null)
                {
                    return NotFound();
                }
				var listParrentCate = _context.Categories.Where(m => m.IsActive == true && m.IsDelete == false && m.Levels == 1).ToList();
				ViewBag.ListParrentCate = new SelectList(listParrentCate.AsQueryable(), "CategoryId", "CategoryName", CateByID.CategoryParrentId);
				return View(CateByID);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditCate(Category cate)
        {
            if (cate == null) return NotFound();
            try
            {
				if (cate.CategoryName == null) { TempData["CateNameRequired"] = "Bạn phải nhập tên danh mục"; }
				if (cate.CategoryDesc == null) { TempData["CateDescRequired"] = "Bạn phải nhập mô tả danh mục"; }
				if (cate.CategoryDesc == null || cate.CategoryName == null)
				{
					var listParrentCate = _context.Categories.Where(m => m.IsActive == true && m.IsDelete == false && m.Levels == 1).ToList();
					ViewBag.ListParrentCate = new SelectList(listParrentCate.AsQueryable(), "CategoryId", "CategoryName", cate.CategoryParrentId);
					return View(cate);
				}
				
                cate.CategorySlug = Functions.AliasLink(cate.CategoryName);
                cate.Levels = 1;
                if (cate.CategoryParrentId != 0) cate.Levels = 2;

                _context.Categories.Update(cate);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int CategoryId)
        {
            if (CategoryId == null)
            {
                return new JsonResult(new
                {
                    message = "Error",
                    status = 1
                });
            }
            try
            {
                var category = await _context.Categories.FindAsync(CategoryId);
                if (category == null)
                {
                    return new JsonResult(new
                    {
                        message = "Can not find category",
                        status = 1
                    });
                }
                else
                {
                    _context.Remove(category);
                    _context.SaveChanges();
                    return new JsonResult(new
                    {
                        message = "Success",
                        status = 0
                    });
                }
            }
            catch
            {
                return new JsonResult(new
                {
                    message = "Error from server",
                    status = 1
                });
            }
        }
    }
}
