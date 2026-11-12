using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KFC_Website.Models;

namespace KFC_MVC5.Controllers
{
    public class HomeController : Controller
    {
        // Khởi tạo DbContext kết nối SQL Server
        private KfcDbContext db = new KfcDbContext();

        // 1. Trả về giao diện trang chủ KFC
        public ActionResult Index()
        {
            return View();
        }

        // 2. LẤY MENU TỪ DATABASE SQL
        [HttpGet]
        public ActionResult GetMenu()
        {
            var menu = db.MenuItems.ToList();
            return Json(menu, JsonRequestBehavior.AllowGet);
        }

        // 3. TÍNH TIỀN GIỎ HÀNG DỰA TRÊN GIÁ TỪ SQL
        [HttpPost]
        public ActionResult CalculateTotal(List<CartItem> items)
        {
            decimal subTotal = 0;
            if (items != null)
            {
                var allItems = db.MenuItems.ToList();
                foreach (var it in items)
                {
                    var dish = allItems.FirstOrDefault(m => m.Id == it.MenuItemId);
                    if (dish != null) subTotal += dish.Price * it.Quantity;
                }
            }

            decimal shippingFee = subTotal > 0 ? 15000 : 0;
            return Json(new { SubTotal = subTotal, ShippingFee = shippingFee, GrandTotal = subTotal + shippingFee });
        }

        // 4. LƯU ĐƠN HÀNG VÀO DATABASE SQL
        [HttpPost]
        public ActionResult Checkout(OrderRequest request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
            {
                return Json(new { Success = false, Message = "Giỏ hàng rỗng." });
            }

            var allItems = db.MenuItems.ToList();
            decimal total = 0;

            foreach (var item in request.Items)
            {
                var dish = allItems.FirstOrDefault(m => m.Id == item.MenuItemId);
                if (dish != null) total += dish.Price * item.Quantity;
            }

            var order = new OrderResult
            {
                OrderId = "KFC" + DateTime.Now.ToString("yyMMddHHmmss"),
                CustomerName = request.CustomerName,
                Phone = request.Phone,
                Address = request.Address,
                TotalAmount = total + 15000,
                Status = "Đã tiếp nhận",
                OrderDate = DateTime.Now
            };

            db.Orders.Add(order);
            db.SaveChanges(); // Lưu trực tiếp xuống SQL Server

            return Json(new { Success = true, Message = "Đặt món thành công!", Order = order });
        }

        // 5. KIỂM TRA MÃ GIẢM GIÁ TỪ SQL (Quân)
        [HttpGet]
        public ActionResult CheckCoupon(string code)
        {
            var coupon = db.Coupons.FirstOrDefault(c => c.Code.ToLower() == code.ToLower() && c.IsActive);
            if (coupon == null)
            {
                return Json(new { Success = false, Message = "Mã giảm giá không hợp lệ hoặc đã hết hạn!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = true, Coupon = coupon }, JsonRequestBehavior.AllowGet);
        }

        // 6. GỬI ĐÁNH GIÁ LƯU XUỐNG SQL (Quân)
        [HttpPost]
        public ActionResult AddReview(Review review)
        {
            if (review == null || review.Rating < 1 || review.Rating > 5)
            {
                return Json(new { Success = false, Message = "Điểm đánh giá phải từ 1 đến 5 sao." });
            }

            review.CreatedAt = DateTime.Now;
            db.Reviews.Add(review);
            db.SaveChanges(); // Lưu vào SQL

            return Json(new { Success = true, Message = "Đã gửi đánh giá thành công!" });
        }

        // 7. THỐNG KÊ DOANH THU TỪ DATABASE (Hoàng)
        [HttpGet]
        public ActionResult GetRevenue()
        {
            var orders = db.Orders.ToList();
            var totalOrders = orders.Count;
            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var avg = totalOrders > 0 ? (totalRevenue / totalOrders) : 0;

            var report = new RevenueReport
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                AverageOrderValue = Math.Round(avg, 0)
            };

            return Json(report, JsonRequestBehavior.AllowGet);
        }

        // 8. ĐĂNG NHẬP PHÂN QUYỀN TRÊN SQL (Hoàng)
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = db.UserAccounts.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                return Json(new { Success = false, Message = "Sai tên đăng nhập hoặc mật khẩu." });
            }

            return Json(new
            {
                Success = true,
                User = new { user.Id, user.Username, user.FullName, user.Role }
            });
        }

        // Đóng kết nối DbContext khi hoàn thành request
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpPost]
        public ActionResult RegisterCustomer(Customer model)
        {
            if (string.IsNullOrWhiteSpace(model.FullName) ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password) ||
                string.IsNullOrWhiteSpace(model.Phone))
            {
                return Json(new { Success = false, Message = "Vui lòng nhập đầy đủ thông tin!" });
            }

            // Kiểm tra trùng email
            bool isExist = db.Customers.Any(c => c.Email.ToLower() == model.Email.ToLower());
            if (isExist)
            {
                return Json(new { Success = false, Message = "Email này đã được sử dụng!" });
            }

            model.CreatedAt = DateTime.Now;
            db.Customers.Add(model);
            db.SaveChanges();

            return Json(new { Success = true, Message = "Đăng ký tài khoản thành công!" });
        }

        // 2. ĐĂNG NHẬP KHÁCH HÀNG
        [HttpPost]
        public ActionResult LoginCustomer(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return Json(new { Success = false, Message = "Vui lòng nhập email và mật khẩu!" });
            }

            var customer = db.Customers.FirstOrDefault(c => c.Email.ToLower() == email.ToLower() && c.Password == password);
            if (customer == null)
            {
                return Json(new { Success = false, Message = "Sai email hoặc mật khẩu!" });
            }

            // Lưu phiên đăng nhập vào Session
            Session["CustomerId"] = customer.Id;
            Session["CustomerName"] = customer.FullName;

            return Json(new
            {
                Success = true,
                Message = "Đăng nhập thành công!",
                Customer = new { customer.Id, customer.FullName, customer.Email, customer.Phone, customer.Address }
            });
        }

        // 3. ĐĂNG XUẤT
        [HttpPost]
        public ActionResult LogoutCustomer()
        {
            Session.Clear();
            return Json(new { Success = true, Message = "Đã đăng xuất!" });
        }
        [HttpGet]
        public JsonResult GetMenuItems()
        {
            using (var db = new KfcDbContext())
            {
                var items = db.MenuItems
                    .Where(m => m.IsAvailable)
                    .Select(m => new
                    {
                        m.Id,
                        m.Name,
                        m.Price,
                        m.ImageUrl,
                        m.Category,
                        m.Description
                    }).ToList();

                return Json(new { Success = true, Data = items }, JsonRequestBehavior.AllowGet);
            }
        }

        // 2. Class phụ để hứng dữ liệu đơn hàng từ JavaScript
        public class CheckoutItemDto
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }

        public class CheckoutRequestDto
        {
            public string CustomerName { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string CouponCode { get; set; }
            public decimal TotalAmount { get; set; }
            public List<CheckoutItemDto> Items { get; set; }
        }

        // 3. Xử lý tạo đơn hàng vào CSDL
        [HttpPost]
        public JsonResult CreateOrder(CheckoutRequestDto model)
        {
            if (model == null || model.Items == null || !model.Items.Any())
            {
                return Json(new { Success = false, Message = "Giỏ hàng của bạn đang trống!" });
            }

            try
            {
                using (var db = new KfcDbContext())
                {
                    // Sinh mã đơn hàng ngẫu nhiên: KFC + ngày giờ
                    string orderId = "KFC" + DateTime.Now.ToString("yyyyMMddHHmmss");

                    // Tạo đối tượng Order chính
                    var order = new OrderResult
                    {
                        OrderId = orderId,
                        CustomerName = string.IsNullOrWhiteSpace(model.CustomerName) ? "Khách vãng lai" : model.CustomerName,
                        Phone = model.Phone,
                        Address = model.Address,

                        SubTotal = model.TotalAmount,     // Tiền hàng
                        DiscountAmount = 0,               // Số tiền giảm (để 0 nếu chưa áp mã)
                        ShippingFee = 15000,              // Phí ship (hoặc để 0)
                        TotalAmount = model.TotalAmount,  // Tổng thanh toán

                        Status = "Pending",
                        OrderDate = DateTime.Now
                    };

                    db.Orders.Add(order);

                    // Duyệt từng món trong giỏ để lưu vào OrderDetails
                    foreach (var item in model.Items)
                    {
                        // Tìm Id món ăn từ tên món
                        var menuItem = db.MenuItems.FirstOrDefault(m => m.Name == item.Name);
                        int menuItemId = menuItem != null ? menuItem.Id : 1;

                        var detail = new OrderDetail
                        {
                            OrderId = orderId,
                            MenuItemId = menuItemId,
                            Quantity = item.Quantity,
                            UnitPrice = item.Price
                        };
                        db.OrderDetails.Add(detail);
                    }

                    db.SaveChanges();

                    return Json(new
                    {
                        Success = true,
                        Message = "Đặt hàng thành công! Đang chuyển đến đơn hàng của bạn...",
                        OrderId = orderId
                    });
                }
            }
            catch (Exception ex)
            {
                // Lấy chi tiết lỗi sâu nhất từ SQL Server
                var inner = ex;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }

                return Json(new
                {
                    Success = false,
                    Message = "Lỗi SQL chi tiết: " + inner.Message
                });
            }
        }
        public ActionResult OrderList()
        {
            using (var db = new KFCModel())
            {
                // Lấy danh sách đơn hàng xếp theo thời gian mới nhất lên đầu
                var listOrders = db.Orders.OrderByDescending(o => o.OrderDate).ToList();
                return View(listOrders);
            }
        }

        // 2. Xem chi tiết món ăn trong một đơn hàng cụ thể
        public ActionResult OrderDetails(string id)
        {
            using (var db = new KFCModel())
            {
                var order = db.Orders.FirstOrDefault(o => o.OrderId == id);
                if (order == null) return HttpNotFound();

                ViewBag.Order = order;

                // Lấy chi tiết đơn kèm thông tin món ăn từ bảng MenuItems
                var detailsWithMenu = (from d in db.OrderDetails
                                       join m in db.MenuItems on d.MenuItemId equals m.Id into menuGroup
                                       from m in menuGroup.DefaultIfEmpty()
                                       where d.OrderId == id
                                       select new OrderItemViewModel
                                       {
                                           MenuItemId = d.MenuItemId,
                                           ItemName = m != null ? m.Name : "Món đã ngừng bán",
                                           ImageUrl = m != null ? m.ImageUrl : "",
                                           Quantity = d.Quantity,
                                           UnitPrice = d.UnitPrice
                                       }).ToList();

                return View(detailsWithMenu);
            }
        }

        // Action xử lý đổi trạng thái đơn hàng (Pending -> Confirmed / Cancelled)
        [HttpPost]
        public ActionResult UpdateOrderStatus(string orderId, string status)
        {
            using (var db = new KFCModel())
            {
                var order = db.Orders.FirstOrDefault(o => o.OrderId == orderId);
                if (order != null)
                {
                    order.Status = status;
                    db.SaveChanges();
                    return Json(new { Success = true, Message = "Đã cập nhật trạng thái đơn hàng!" });
                }
                return Json(new { Success = false, Message = "Không tìm thấy đơn hàng!" });
            }
        }
        // 1. Xem danh sách toàn bộ món ăn
        public ActionResult ManageMenu()
        {
            using (var db = new KFCModel())
            {
                var items = db.MenuItems.ToList();
                return View(items);
            }
        }

        // 2. Thêm món ăn mới vào Menu
        [HttpPost]
        public ActionResult AddMenuItem(string Name, decimal Price, string ImageUrl, string Description)
        {
            if (string.IsNullOrWhiteSpace(Name) || Price <= 0)
            {
                TempData["Error"] = "Tên món và Giá không được để trống!";
                return RedirectToAction("ManageMenu");
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

            TempData["Success"] = "Thêm món ăn thành công!";
            return RedirectToAction("ManageMenu");
        }

        // 3. Xóa món ăn khỏi Menu
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
                else
                {
                    TempData["Error"] = "Không tìm thấy món ăn!";
                }
            }
            return RedirectToAction("ManageMenu");
        }
        public ActionResult AdminDashboard()
        {
            // Kiểm tra đăng nhập (nếu đã lưu Session khi đăng nhập)
            // if (Session["Role"] == null || Session["Role"].ToString() != "Admin") 
            // { 
            //     return RedirectToAction("Index", "Home"); 
            // }

            using (var db = new KFCModel())
            {
                ViewBag.TotalOrders = db.Orders.Count();
                ViewBag.PendingOrders = db.Orders.Count(o => o.Status == "Pending");
                ViewBag.TotalMenuItems = db.MenuItems.Count();
                ViewBag.TotalRevenue = db.Orders.Where(o => o.Status == "Hoàn tất" || o.Status == "Completed")
                                               .Select(o => (decimal?)o.TotalAmount).Sum() ?? 0;

                // Lấy 5 đơn hàng mới nhất
                var recentOrders = db.Orders.OrderByDescending(o => o.OrderDate).Take(5).ToList();
                return View(recentOrders);
            }
        }
    }
}