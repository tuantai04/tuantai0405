using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using TechBlog.Models;
using TechBlog.SessionSystem;

namespace TechBlog.Controllers
{
    public class ContactController : Controller
    {
        private readonly TechBlogContext _context;
        private readonly INotyfService _otyfService;
        public ContactController(TechBlogContext context, INotyfService notyfService)
        {
            _context = context;
            _otyfService = notyfService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("/lien-he")]
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SendRequest(Contact contact)
        {
            if (contact == null)
            {
                return View();
            }
            try
            {
                
                if (contact.FullName == null || contact.Gmail == null || contact.PhoneNumber == null || contact.ContactDetail == null)
                {
                    _otyfService.Error("Vui lòng nhập đầy đủ thông tin");
                    return View("Contact", contact);
                }
                _context.Contacts.Add(contact);
                _context.SaveChanges();
                _otyfService.Success("Bạn đã gửi tin nhắn thành công");
                return RedirectToAction("Contact");
            }
            catch
            {
                return View("Index");
            }
        }
    }
}
