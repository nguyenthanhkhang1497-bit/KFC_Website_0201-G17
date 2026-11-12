USE KFC_Database;
GO

-- 1. Xóa các bảng con đang giữ khóa ngoại trỏ vào MenuItems trước
IF OBJECT_ID('OrderDetails', 'U') IS NOT NULL DROP TABLE OrderDetails;
IF OBJECT_ID('Reviews', 'U') IS NOT NULL DROP TABLE Reviews;

-- 2. Bây giờ đã có thể xóa MenuItems an toàn
IF OBJECT_ID('MenuItems', 'U') IS NOT NULL DROP TABLE MenuItems;
GO

-- 3. Tạo lại bảng MenuItems với chuẩn cột Category
CREATE TABLE MenuItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Price DECIMAL(18,0) NOT NULL,
    ImageUrl NVARCHAR(250) NULL,
    Category NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    IsAvailable BIT NOT NULL DEFAULT 1
);
GO

-- 4. Tạo lại Reviews và OrderDetails để không mất liên kết
CREATE TABLE Reviews (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MenuItemId INT NOT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    Rating INT NOT NULL,
    Comment NVARCHAR(500) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Reviews_MenuItems FOREIGN KEY (MenuItemId) REFERENCES MenuItems(Id) ON DELETE CASCADE
);

CREATE TABLE OrderDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId NVARCHAR(50) NOT NULL,
    MenuItemId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,0) NOT NULL,
    CONSTRAINT FK_OrderDetails_MenuItems FOREIGN KEY (MenuItemId) REFERENCES MenuItems(Id)
);
GO

-- 5. Nạp lại dữ liệu món ăn
INSERT INTO MenuItems (Name, Price, ImageUrl, Category, Description, IsAvailable) VALUES
(N'Gà Rán Giòn Cay', 39000, '/Content/6.png.png', N'Gà Rán', N'Miếng gà giòn rụm đậm vị cay', 1),
(N'Burger Tôm', 45000, '/Content/6.png.png', N'Burger', N'Burger tôm giòn với xốt mayonnaise', 1),
(N'Khoai Tây Chiên (L)', 25000, '/Content/6.png.png', N'Món Phụ', N'Khoai tây chiên vàng giòn cỡ lớn', 1),
(N'Pepsi Lon', 17000, '/Content/6.png.png', N'Thức Uống', N'Pepsi 330ml ướp lạnh', 1);
GO

-- 6. Kiểm tra lại dữ liệu
SELECT * FROM MenuItems;