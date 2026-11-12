using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace KFC_Website.Models
{
    // 1. Quản lý danh mục
    [Table("Categories")]
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    // 2. Quản lý sản phẩm
    [Table("MenuItems")]
    public class MenuItems
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }

    // 3. Quản lý khách hàng
    [Table("Customers")]
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    // 4. Quản lý tài khoản Admin & Nhân viên
    [Table("UserAccounts")]
    public class UserAccounts
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // 'Admin', 'Staff'
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    // 5. Quản lý mã giảm giá
    [Table("Coupons")]
    public class Coupons
    {
        [Key]
        public string Code { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public decimal MinOrderAmount { get; set; }
        public bool IsActive { get; set; }
    }

    // 6. Quản lý đơn hàng
    [Table("Orders")]
    public class OrderResults
    {
        [Key]
        public string OrderId { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string CouponCode { get; set; }
        public string Status { get; set; }
        public DateTime? OrderDate { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    // 8. Quản lý thanh toán
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrderResult Order { get; set; }
    }

    // 9. Quản lý đánh giá
    [Table("Reviews")]
    public class Reviews
    {
        [Key]
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    // ==========================================
    // DBCONTEXT KẾT NỐI VÀO SQL SERVER
    // ==========================================
    public class KfcDbContext : DbContext
    {
        public KfcDbContext() : base("name=KfcDbContext")
        {
            Database.SetInitializer<KfcDbContext>(null);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<OrderResult> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}