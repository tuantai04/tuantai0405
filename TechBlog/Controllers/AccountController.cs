using TechBlog.Extension;
using TechBlog.Models;
using TechBlog.SessionSystem;
using Microsoft.AspNetCore.Mvc;

namespace TechBlog.Controllers
{
    public class AccountController : Controller
    {
        private readonly TechBlogContext _context;
        public AccountController(TechBlogContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (user == null)
            {
                return NotFound();
            }
            try
            {
                string password = HashPassword.MD5Password(user.Password);
                var CheckLogin = _context.Users.Where(m => m.Email.ToLower() == user.Email.ToLower() && m.Password == password).FirstOrDefault();
                if (CheckLogin != null)
                {
                    HttpContext.Session.SetString(SessionKey.FULLNAME, CheckLogin.FullName);
                    HttpContext.Session.SetString(SessionKey.EMAIL, CheckLogin.Email);
                    HttpContext.Session.SetInt32(SessionKey.USERID, CheckLogin.UserId);
                    HttpContext.Session.SetInt32(SessionKey.ROLEID, (int)CheckLogin.RoleId);
                    return Redirect("/Admin");
                }
                else
                {
                    TempData["LoginError"] = "Tài khoản hoặc mật khẩu không chính xác";
                    return View();
                }
            }
            catch
            {
                return NotFound();
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove(SessionKey.FULLNAME);
            HttpContext.Session.Remove(SessionKey.ROLEID);
            HttpContext.Session.Remove(SessionKey.USERID);
            return RedirectToAction("Login", "Account");
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, string AgainPass)
        {
            if (user == null)
            {
                return NotFound();
            }
            try
            {
                var CheckEmail = _context.Users.Where(m => m.Email == user.Email).FirstOrDefault();
                if (user.Email == null) { TempData["EmailRequired"] = "Vui lòng nhập Email của bạn"; }
                if (user.FullName == null) { TempData["FullNameRequired"] = "Vui lòng nhập họ và tên của bạn"; }
                if (user.Password == null) { TempData["PasswordRequired"] = "Vui lòng nhập mật khẩu của bạn"; }
                if (user.Address == null) { TempData["AddressRequired"] = "Vui lòng nhập Địa chỉ của bạn"; }
                if (user.PhoneNumber == null) { TempData["PhoneNumberRequired"] = "Vui lòng nhập điện thoại của bạn"; }
                if (AgainPass == null) { TempData["AgainPassRequired"] = "Trường này không được để trống"; }
                if (user.Email != null && CheckEmail != null) { TempData["EmailExists"] = "Email này đã được sử dụng"; }
                if ((user.Password != null && AgainPass != null) && user.Password.Trim() != AgainPass.Trim()) { TempData["AgainPassError"] = "Mật khẩu nhập lại không trùng khớp"; }

                if (user.Email == null || CheckEmail != null || user.FullName == null || user.Password == null || user.Address == null || user.PhoneNumber == null || AgainPass == null || user.Password.Trim() != AgainPass.Trim())
                {
                    return View(user);
                }

                user.RoleId = 2;
                user.Avatar = "avatar-default.jpeg";
                user.Password = HashPassword.MD5Password(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
