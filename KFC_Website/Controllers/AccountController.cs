using System.Linq;
using System.Web.Mvc;
using KFC_Website.Models;

namespace KFC_Website.Controllers
{
    public class AccountController : Controller
    {
        // 1. Xử lý Đăng ký (gọi từ Modal qua Fetch API)
        [HttpPost]
        public ActionResult Register(string username, string password, string fullName, string phone)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return Json(new { Success = false, Message = "Tên đăng nhập và mật khẩu không được để trống!" });
            }

            using (var db = new KFCModel())
            {
                if (db.Users.Any(u => u.Username == username))
                {
                    return Json(new { Success = false, Message = "Tên đăng nhập đã tồn tại!" });
                }

                var newUser = new User
                {
                    Username = username,
                    Password = password,
                    FullName = fullName,
                    Phone = phone,
                    Role = "Customer"
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                return Json(new { Success = true, Message = "Đăng ký tài khoản thành công! Bạn có thể đăng nhập." });
            }
        }

        // 2. Xử lý Đăng nhập
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            using (var db = new KFCModel())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    // Lưu thông tin vào Session
                    Session["UserId"] = user.UserId;
                    Session["Username"] = user.Username;
                    Session["FullName"] = user.FullName;
                    Session["UserRole"] = user.Role;

                    return Json(new { Success = true, Role = user.Role, FullName = user.FullName, Message = "Đăng nhập thành công!" });
                }

                return Json(new { Success = false, Message = "Sai tên đăng nhập hoặc mật khẩu!" });
            }
        }

        // 3. Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}