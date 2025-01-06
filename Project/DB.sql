-- 1. Tạo cơ sở dữ liệu
CREATE DATABASE ConvenienceStoreDB;
GO
USE ConvenienceStoreDB;
GO

-- 2. Bảng liên quan đến danh mục và sản phẩm
CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY,
    CategoryName NVARCHAR(100) NOT NULL			
);
GO

CREATE TABLE Product (
    ProductID INT PRIMARY KEY IDENTITY,
    ProductName NVARCHAR(100) NOT NULL,
    CategoryID INT FOREIGN KEY REFERENCES Category(CategoryID),
    Brand NVARCHAR(50),
    QuantityInStock INT,
    Price DECIMAL(18, 2),
    CostPrice DECIMAL(18, 2),
    Unit NVARCHAR(20)
);
GO

-- 3. Bảng nhà cung cấp và các bảng liên quan
CREATE TABLE Supplier (
    SupplierID INT PRIMARY KEY IDENTITY,
    SupplierName NVARCHAR(100) NOT NULL,
    ContactNumber NVARCHAR(20),
    Address NVARCHAR(255),
    Email NVARCHAR(100),
    TaxCode NVARCHAR(50),
    ContactPerson NVARCHAR(50),
    Status NVARCHAR(20)
);
GO

-- 4. Bảng nhân viên và các bảng liên quan
CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY,
    EmployeeName NVARCHAR(100) NOT NULL,
    Position NVARCHAR(50),
    HireDate DATE,
    Salary DECIMAL(18, 2),
    PhoneNumber NVARCHAR(20),
    Address NVARCHAR(255),
    Email NVARCHAR(100),
    Birthday DATE,
    IDNumber NVARCHAR(20)
);
GO

-- 5. Bảng đơn hàng và chi tiết đơn hàng
CREATE TABLE [Order] (
    OrderID INT PRIMARY KEY IDENTITY,
    EmployeeID INT FOREIGN KEY REFERENCES Employee(EmployeeID),
    OrderDate DATE,
    TotalAmount DECIMAL(18, 2),
    PaymentMethod NVARCHAR(50),
    Discount DECIMAL(18, 2),
    Note NVARCHAR(255)
);
GO

CREATE TABLE OrderDetail (
    OrderDetailID INT PRIMARY KEY IDENTITY,
    OrderID INT FOREIGN KEY REFERENCES [Order](OrderID),
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID),
    Quantity INT,
    UnitPrice DECIMAL(18, 2),
    Discount DECIMAL(18, 2)
);
GO

-- 6. Bảng ca làm việc (Shift)
CREATE TABLE Shift (
    ShiftID INT PRIMARY KEY IDENTITY,
    EmployeeID INT FOREIGN KEY REFERENCES Employee(EmployeeID),
    ShiftDate DATE,
    StartTime TIME,
    EndTime TIME,
    Status NVARCHAR(50) DEFAULT 'Scheduled',
    Note NVARCHAR(255) DEFAULT '',
    EmployeeName NVARCHAR(100)
);
GO

-- Ràng buộc kiểm tra StartTime < EndTime
ALTER TABLE Shift
ADD CONSTRAINT CK_Shift_StartTime_EndTime CHECK (StartTime < EndTime);

-- Trigger cập nhật EmployeeName trong bảng Shift
CREATE TRIGGER TR_Shift_Insert_Update_EmployeeName
ON Shift
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE Shift
    SET EmployeeName = e.EmployeeName
    FROM Shift s
    INNER JOIN Employee e ON s.EmployeeID = e.EmployeeID
    WHERE s.ShiftID IN (SELECT ShiftID FROM inserted);
END;
GO

-- 7. Bảng giao dịch
CREATE TABLE [Transaction] (
    TransactionID INT PRIMARY KEY IDENTITY,
    TransactionType NVARCHAR(50),
    Amount DECIMAL(18, 2),
    TransactionDate DATE,
    Description NVARCHAR(255),
    ReferenceID INT,
    ReferenceType NVARCHAR(50),
    PaymentMethod NVARCHAR(50)
);
GO

-- 8. Bảng đặt hàng từ nhà cung cấp và chi tiết
CREATE TABLE PurchaseOrder (
    PurchaseOrderID INT PRIMARY KEY IDENTITY,
    SupplierID INT FOREIGN KEY REFERENCES Supplier(SupplierID),
    EmployeeID INT FOREIGN KEY REFERENCES Employee(EmployeeID),
    OrderDate DATE,
    TotalAmount DECIMAL(18, 2),
    Status NVARCHAR(50),
    PaymentStatus NVARCHAR(50),
    Note NVARCHAR(255)
);
GO

CREATE TABLE PurchaseOrderDetail (
    PurchaseOrderDetailID INT PRIMARY KEY IDENTITY,
    PurchaseOrderID INT FOREIGN KEY REFERENCES PurchaseOrder(PurchaseOrderID),
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID),
    Quantity INT,
    UnitPrice DECIMAL(18, 2)
);
GO
