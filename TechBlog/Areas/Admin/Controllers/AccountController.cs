using TechBlog.Extension;
using TechBlog.Models;
using TechBlog.Ultilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace TechBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly TechBlogContext _context;
        public AccountController(TechBlogContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;

            List<User> listUser = new List<User>();
            listUser = _context.Users.AsNoTracking().OrderByDescending(x => x.UserId).Where(m => m.IsDelete == false).ToList();
           
            PagedList<User> models = new PagedList<User>(listUser.AsQueryable(), pageNumber, pageSize);
            ViewBag.currentPage = pageNumber;

            return View(models);
        }

        public IActionResult Create()
        {
            return View();
        }

        public bool CheckEmail(string email)
        {
            var user = _context.Users.Where(m => m.Email.Trim() == email.Trim()).FirstOrDefault();
            if(user != null) { return false; }
            return true;
        }
        [HttpPost]
        public async Task<IActionResult> Create(User user, IFormFile? avatar)
        {
            if (user == null)
            {
                return NotFound();
            }
            try
            {
                if(user.FullName == null) { TempData["FullNameRequired"] = "Họ và tên không được để trống"; }
                if(user.Address == null) { TempData["AddressRequired"] = "Địa chỉ không được để trống"; }
                if(user.Email== null) { TempData["EmailRequired"] = "Email không được để trống"; }
                if(user.PhoneNumber == null) { TempData["PhoneRequired"] = "Số điện thoại không được để trống"; }
                if(user.Email != null && !CheckEmail(user.Email)) { TempData["EmailExits"] = "Email đã được sử dụng."; }
                if(user.FullName == null || user.Address == null || user.PhoneNumber == null || user.Email == null || !CheckEmail(user.Email))
                {
                    return View(user);
                }
                user.IsActive = true;
                user.IsDelete = false;

                if (avatar != null)
                {
                    string extension = Path.GetExtension(avatar.FileName);
                    string image = Extension.Extensions.ToUrlFriendly(user.FullName) + extension;
                    user.Avatar = await Functions.UploadFile(avatar, @"Users", image.ToLower());
                    user.Avatar = "Users/" + user.Avatar;
                }

                if (string.IsNullOrEmpty(user.Avatar)) user.Avatar = "avatar-default.jpeg";
                user.Password = HashPassword.MD5Password("123123");
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                
            } catch
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null) return NotFound();
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User user, string OldAvatar, IFormFile? avatar)
        {
            if (user == null)
            {
                return NotFound();
            }
            try
            {
                if (user.FullName == null) { TempData["FullNameRequired"] = "Họ và tên không được để trống"; }
                if (user.Address == null) { TempData["AddressRequired"] = "Địa chỉ không được để trống"; }
                if (user.Email == null) { TempData["EmailRequired"] = "Email không được để trống"; }
                if (user.PhoneNumber == null) { TempData["PhoneRequired"] = "Số điện thoại không được để trống"; }
                if (user.FullName == null || user.Address == null || user.PhoneNumber == null || user.Email == null)
                {
                    return View(user);
                }
                if (avatar != null)
                {
                    string extension = Path.GetExtension(avatar.FileName);
                    string image = Extension.Extensions.ToUrlFriendly(user.FullName) + extension;
                    user.Avatar = await Functions.UploadFile(avatar, @"Users", image.ToLower());
                    user.Avatar = "Users/" + user.Avatar;
                }
                else
                {
                    user.Avatar = OldAvatar;
                }
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Delete(int UserID)
        {
            if (UserID == null)
            {
                return new JsonResult(new
                {
                    message = "Error",
                    status = 1
                });
            }
            try
            {
                var user = await _context.Users.FindAsync(UserID);
                if (user == null)
                {
                    return new JsonResult(new
                    {
                        message = "Can not find User",
                        status = 1
                    });
                }
                else
                {
                    _context.Remove(user);
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

        public async Task<IActionResult> Block(int UserID)
        {
            if (UserID == null)
            {
                return new JsonResult(new
                {
                    message = "Error",
                    status = 1
                });
            }
            try
            {
                var user = await _context.Users.FindAsync(UserID);
                if (user == null)
                {
                    return new JsonResult(new
                    {
                        message = "Error",
                        status = 1
                    });
                }
                else
                {
                    if (user.IsActive == true) user.IsActive = false;
                    else user.IsActive = true;

                    _context.Update(user);
                    _context.SaveChanges();
                    if (user.IsActive == true)
                    {
                        return new JsonResult(new
                        {
                            message = "Unblock Acc Success",
                            status = 0
                        });
                    }
                    else
                    {
                        return new JsonResult(new
                        {
                            message = "Block Acc Success",
                            status = 0
                        });
                    }

                }
            }
            catch
            {
                return new JsonResult(new
                {
                    message = "Error",
                    status = 1
                });
            }
        }
    }
}
