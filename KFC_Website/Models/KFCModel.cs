using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KFC_Website.Models
{
    [Table("MenuItems")]
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; } 
        public string Description { get; set; }
        public bool IsAvailable { get; set; } = true;
    }

    public class CartItem
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderRequest
    {
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public List<CartItem> Items { get; set; }
    }

    [Table("Coupons")]
    public class Coupon
    {
        [Key]
        public string Code { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("Orders")]
    public class OrderResult
    {
        [Key]
        public string OrderId { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string CouponCode { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
    }

    [Table("Reviews")]
    public class Review
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public string CustomerName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [Table("UserAccounts")]
    public class UserAccount
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }

    public class RevenueReport
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    // 2. Lớp DbContext kết nối SQL
    public class KFCModel : DbContext
    {
        // Tên chuỗi kết nối trong Web.config
        public KFCModel() : base("name=KfcDbContext")
        {
        }

        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OrderResult> Orders { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
    public class Order
    {
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    [Table("OrderDetails")]
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
    public class OrderItemViewModel
    {
        public int MenuItemId { get; set; }
        public string ItemName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(20)]
        public string Role { get; set; }
    }
}