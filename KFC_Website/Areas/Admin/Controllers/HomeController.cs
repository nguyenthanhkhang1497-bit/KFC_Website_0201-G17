using System;
using System.Linq;
using System.Web.Mvc;
using KFC_Website.Models; // Namespace Models chính của bạn

namespace KFC_Website.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // 1. Dashboard tổng quan
        public ActionResult Index()
        {
            using (var db = new KFCModel())
            {
                ViewBag.TotalOrders = db.Orders.Count();
                ViewBag.PendingOrders = db.Orders.Count(o => o.Status == "Pending");
                ViewBag.TotalMenuItems = db.MenuItems.Count();
                ViewBag.TotalRevenue = db.Orders.Where(o => o.Status == "Hoàn tất" || o.Status == "Completed")
                                               .Select(o => (decimal?)o.TotalAmount).Sum() ?? 0;

                var recentOrders = db.Orders.OrderByDescending(o => o.OrderDate).Take(5).ToList();
                return View(recentOrders);
            }
        }

        // 2. Danh sách toàn bộ đơn hàng
        public ActionResult Orders()
        {
            using (var db = new KFCModel())
            {
                var list = db.Orders.OrderByDescending(o => o.OrderDate).ToList();
                return View(list);
            }
        }

        // 3. Quản lý danh mục món ăn
        public ActionResult Menu()
        {
            using (var db = new KFCModel())
            {
                var items = db.MenuItems.ToList();
                return View(items);
            }
        }

        // 4. Thêm món mới
        [HttpPost]
        public ActionResult AddMenuItem(string Name, decimal Price, string ImageUrl, string Description)
        {
            if (string.IsNullOrWhiteSpace(Name) || Price <= 0)
            {
                TempData["Error"] = "Tên món và Giá không hợp lệ!";
                return RedirectToAction("Menu");
            }

            using (var db = new KFCModel())
            {
                var newItem = new MenuItem
                {
                    Name = Name,
                    Price = Price,
                    ImageUrl = string.IsNullOrWhiteSpace(ImageUrl) ? "/images/default.jpg" : ImageUrl,
                    Description = Description
                };

                db.MenuItems.Add(newItem);
                db.SaveChanges();
            }

            TempData["Success"] = "Thêm món thành công!";
            return RedirectToAction("Menu");
        }

        // 5. Xóa món
        [HttpPost]
        public ActionResult DeleteMenuItem(int id)
        {
            using (var db = new KFCModel())
            {
                var item = db.MenuItems.FirstOrDefault(m => m.Id == id);
                if (item != null)
                {
                    db.MenuItems.Remove(item);
                    db.SaveChanges();
                    TempData["Success"] = "Đã xóa món ăn!";
                }
            }
            return RedirectToAction("Menu");
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra xem đã đăng nhập và có quyền Admin chưa
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                // Chưa đăng nhập Admin thì đẩy về trang chủ ngoài
                filterContext.Result = new RedirectResult("~/Home/Index");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}