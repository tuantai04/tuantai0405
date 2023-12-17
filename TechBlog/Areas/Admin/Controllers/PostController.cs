using TechBlog.Models;
using TechBlog.Ultilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PagedList.Core;
using TechBlog.SessionSystem;

namespace TechBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostController : Controller
    {
        private readonly TechBlogContext _context;
        public PostController(TechBlogContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;

            List<Post> listPost = new List<Post>();
            listPost = _context.Posts.AsNoTracking().OrderByDescending(x => x.PostId).Where(m => m.IsDelete == false).Include(m => m.Category).ToList();

            PagedList<Post> models = new PagedList<Post>(listPost.AsQueryable(), pageNumber, pageSize);
            ViewBag.currentPage = pageNumber;
            return View(models);
        }
        public IActionResult GoToTrash(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;

            List<Post> listPost = new List<Post>();
            listPost = _context.Posts.AsNoTracking().OrderByDescending(x => x.PostId).Where(m => m.IsDelete == true).Include(m => m.Category).ToList();

            PagedList<Post> models = new PagedList<Post>(listPost.AsQueryable(), pageNumber, pageSize);
            ViewBag.currentPage = pageNumber;
            return View(models);
        }
        public IActionResult Create()
        {
            var listCategory = _context.Categories.Where(m => m.IsActive == true && m.IsDelete == false).ToList();
            ViewBag.ListCategory = new SelectList(listCategory.AsQueryable(), "CategoryId", "CategoryName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Post post, IFormFile? PostImage)
        {
            if (post == null)
            {
                return NotFound();
            }
            try
            {

                if (post.PostName == null) TempData["PostName"] = "Bạn phải nhập tên bài viết";
                if (post.PostTitle == null) TempData["PostTitle"] = "Bạn phải nhập tiêu đề bài viết";
                if (post.CategoryId == 0) TempData["CategoryId"] = "Bạn chưa chọn danh mục bài viết";
                if (post.PostDetail == null) TempData["PostDetail"] = "Bạn phải nhập nội dung cho bài viết";
                
                if (post.PostName == null ||
                    post.PostTitle == null ||
                    post.CategoryId == 0 ||
                    post.PostDetail == null)
                {
                    var listCategory = _context.Categories.Where(m =>  m.IsActive == true && m.IsDelete == false).ToList();
                    ViewBag.ListCategory = new SelectList(listCategory.AsQueryable(), "CategoryId", "CategoryName", post.CategoryId);
                    return View();
                }
                var UserId = HttpContext.Session.GetInt32(SessionKey.USERID);
                post.PostSlug = Functions.AliasLink(post.PostName);
                post.IsActive = true;
                post.IsDelete = false;
                post.CreatedDate = DateTime.Now;
                post.ModifiedDate = DateTime.Now;
                post.CreatedBy = UserId;
                post.ModifiedBy = UserId;
                if (PostImage != null)
                {
                    string extension = Path.GetExtension(PostImage.FileName);
                    string image = Extension.Extensions.ToUrlFriendly(post.PostName) + extension;
                    post.PostImage = await Functions.UploadFile(PostImage, @"Posts", image.ToLower());
                    post.PostImage = "Posts/" + post.PostImage;
                }

                if (string.IsNullOrEmpty(post.PostImage)) post.PostImage = "image-default.png";
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                return NotFound();
            }
        }
        public IActionResult Edit(int id)
        {
            var EditPostById = _context.Posts.Where(m => m.PostId == id && m.IsDelete == false).FirstOrDefault();
            var listCategory = _context.Categories.Where(m => m.IsActive == true && m.IsDelete == false).ToList();
            ViewBag.ListCategory = new SelectList(listCategory.AsQueryable(), "CategoryId", "CategoryName", EditPostById.CategoryId);
            return View(EditPostById);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Post post, IFormFile? PostImage, string OldImage)
        {
            if (post == null)
            {
                return NotFound();
            }
            try
            {
                if (post.PostName == null) TempData["PostName"] = "Bạn phải nhập tên bài viết";
                if (post.PostTitle == null) TempData["PostTitle"] = "Bạn phải nhập tiêu đề bài viết";
                if (post.CategoryId == 0) TempData["CategoryId"] = "Bạn chưa chọn danh mục bài viết";
                if (post.PostDetail == null) TempData["PostDetail"] = "Bạn phải nhập nội dung cho bài viết";
                if (post.PostName == null ||
                post.PostTitle == null ||
                post.CategoryId == 0 ||
                    post.PostDetail == null)
                {
                    var listCategory = _context.Categories.Where(m => m.IsActive == true && m.IsDelete == false).ToList();
                    ViewBag.ListCategory = new SelectList(listCategory.AsQueryable(), "CategoryId", "CategoryName", post.CategoryId);
                    return View();
                }
                var UserId = HttpContext.Session.GetInt32(SessionKey.USERID);
                post.ModifiedBy = UserId;
                post.PostSlug = Functions.AliasLink(post.PostName);
                if (PostImage != null)
                {
                    string extension = Path.GetExtension(PostImage.FileName);
                    string image = Extension.Extensions.ToUrlFriendly(post.PostName) + extension;
                    post.PostImage = await Functions.UploadFile(PostImage, @"Posts", image.ToLower());
                    post.PostImage = "Posts/" + post.PostImage;
                }
                else
                {
                    post.PostImage = OldImage;
                }

                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> Delete(int IdToDelete)
        {
            if (IdToDelete == null)
            {
                return new JsonResult(new
                {
                    message = "Error",
                    status = 1
                });
            }
            try
            {
                var ItemById = await _context.Posts.FindAsync(IdToDelete);
                if (ItemById == null)
                {
                    return new JsonResult(new
                    {
                        message = "Can not find User",
                        status = 1
                    });
                }
                else
                {
                    ItemById.IsDelete = true;
                    _context.Update(ItemById);
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
        public async Task<IActionResult> KhoiPhuc(int IdKhoiPhuc)
        {
            if (IdKhoiPhuc == null)
            {
                return new JsonResult(new
                {
                    message = "Error",
                    status = 1
                });
            }
            try
            {
                var khoiphuc = await _context.Posts.FindAsync(IdKhoiPhuc);
                if (khoiphuc == null)
                {
                    return new JsonResult(new
                    {
                        message = "Can not find",
                        status = 1
                    });
                }
                else
                {
                    khoiphuc.IsDelete = false;
                    _context.Update(khoiphuc);
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
        public async Task<IActionResult> UpdateActiveStatus(int IdToUpdate)
        {
            if (IdToUpdate == null)
            {
                return new JsonResult(new
                {
                    message = "Error",
                    status = 1
                });
            }
            try
            {
                var ItemById = await _context.Posts.Where(m => m.PostId == IdToUpdate && m.IsDelete == false).FirstOrDefaultAsync();
                if (ItemById == null) return Json(new
                {
                    status = 2,
                    message = "Cannot find Product"
                });
                ItemById.IsActive = !ItemById.IsActive;
                _context.Posts.Update(ItemById);
                _context.SaveChanges();
                return Json(new
                {
                    status = 0,
                    currentValue = ItemById.IsActive,
                    message = "Success"
                });
            }
            catch
            {
                return Json(new
                {
                    status = 3,
                    message = "Error from server"
                });
            }

        }
    }
}
