using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using TechBlog.Models;

namespace TechBlog.Controllers
{
    public class PostController : Controller
    {
        private readonly TechBlogContext _context;
        public PostController(TechBlogContext context)
        {
            _context = context;
        }
        [Route("/{slug}-{id}")]
        public IActionResult Index(int id, int? page)
        {
            var category = _context.Categories.Where(m => m.CategoryId== id).FirstOrDefault();
            var cateParent = _context.Categories.Where( m => m.CategoryId == category.CategoryParrentId).FirstOrDefault();
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 6;
            List<Post> ListPost = new List<Post>();
            ListPost = _context.Posts.AsNoTracking().OrderByDescending(m => m.CreatedDate).Where(m => m.IsDelete == false && m.IsActive == true && m.CategoryId == id).Include(m => m.CreatedByNavigation).Include(m => m.Category).Take(12).ToList();
            PagedList<Post> models = new PagedList<Post>(ListPost.AsQueryable(), pageNumber, pageSize);
            ViewBag.currentPage = pageNumber;
            ViewBag.CategoryId = id;
            ViewBag.Category = category;
            ViewBag.CateParrent = cateParent;
            return View(models);
        }
        [Route("/{CategorySlug}/{PostSlug}-{id}")]
        public IActionResult Detail(int id) { 
            var post = _context.Posts.AsNoTracking().Where(m => m.PostId == id && m.IsDelete == false && m.IsActive == true).Include(m => m.Category).Include(m => m.CreatedByNavigation).FirstOrDefault();
            var recentpost = _context.Posts.OrderByDescending(m => m.ViewNumber).Where(m => m.PostId != post.PostId && m.IsDelete == false && m.IsActive == true && m.CategoryId == post.CategoryId).Include(m => m.CreatedByNavigation).Include(m => m.Category).Take(3).ToList();
            ViewBag.RecentPost = recentpost;
            return View(post); 
        }
        public IActionResult AddComment(string usercomment, string contentcomment, int postid)
        {
            try
            {
                if(usercomment == null || contentcomment == null || postid == null)
                {
                    return Json(new
                    {
                        status = 2,
                        message = "Bạn phải nhập đầy đủ thông tin",
                    });
                }
                var newcomment = new Comment();
                newcomment.PostId = postid;
                newcomment.FullName = usercomment;
                newcomment.CommentDate= DateTime.Now;
                newcomment.CommentContent = contentcomment;
                newcomment.CommentLevels = 1;
                newcomment.CommentParrentId= 0;
                _context.Comments.Add(newcomment);
                _context.SaveChanges();
                return Json(new
                {
                    status = 0,
                    message = "Đã gửi bình luận thành công"
                });
            } catch
            {
                return Json(new
                {
                    status = 1,
                    message = "Server đang bị lỗi"
                }) ;
            }
        }
        public IActionResult LoadComment(int postid)
        {
            try
            {
                if (postid == null)
                {
                    return Json(new
                    {
                        status = 2,
                        message = "Không tìm thấy bài viết",
                    });
                }
                var listCommentByPost = _context.Comments.OrderByDescending(m => m.CommentDate).Where(m => m.PostId == postid).Take(5).ToList();
                var data = "";
                foreach(var item in listCommentByPost)
                {
                    data += "<div class='comment-item d-flex mt-2'>";
                    data += "<img class='comment-avt' src='/images/avatar-default.jpeg' />";
                    data += "<div class='comment-content-box'>";
                    data += "<h4 class='comment-name mb-0'>" +item.FullName + "</h4>";
                    data += "<p class='comment-detail mb-0'>" +item.CommentContent + "</p>";
                    data += "</div> </div>";
                }
                return Json(new
                {
                    status = 0,
                    content = data,
                    message = "Đã load bình luận thành công"
                });
            }
            catch
            {
                return Json(new
                {
                    status = 1,
                    message = "Server đang bị lỗi"
                });
            }
        }
    }
}
