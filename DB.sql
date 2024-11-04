CREATE DATABASE ConvenienceStoreDB;
GO
USE ConvenienceStoreDB;
GO

-- Bảng Category (Loại sản phẩm)
CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY,
    CategoryName NVARCHAR(100) NOT NULL
);
GO

-- Bảng Product (Sản phẩm)
CREATE TABLE Product (
    ProductID INT PRIMARY KEY IDENTITY,
    ProductName NVARCHAR(100) NOT NULL,
    CategoryID INT FOREIGN KEY REFERENCES Category(CategoryID),
    Brand NVARCHAR(100),
    QuantityInStock INT DEFAULT 0,
    ReorderLevel INT DEFAULT 0,
    Price DECIMAL(18, 2) NOT NULL
);
GO

-- Bảng Customer (Khách hàng)
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY IDENTITY,
    CustomerName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20),
    Email NVARCHAR(100)
);
GO

-- Bảng Order (Đơn hàng)
CREATE TABLE [Order] (
    OrderID INT PRIMARY KEY IDENTITY,
    CustomerID INT FOREIGN KEY REFERENCES Customer(CustomerID),
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 2) NOT NULL
);
GO

-- Bảng OrderDetail (Chi tiết đơn hàng)
CREATE TABLE OrderDetail (
    OrderDetailID INT PRIMARY KEY IDENTITY,
    OrderID INT FOREIGN KEY REFERENCES [Order](OrderID),
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL
);
GO

-- Bảng Employee (Nhân viên)
CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY,
    EmployeeName NVARCHAR(100) NOT NULL,
    Position NVARCHAR(50),
    HireDate DATE NOT NULL,
    Salary DECIMAL(18, 2) NOT NULL
);
GO

-- Bảng Shift (Ca làm việc)
CREATE TABLE Shift (
    ShiftID INT PRIMARY KEY IDENTITY,
    EmployeeID INT FOREIGN KEY REFERENCES Employee(EmployeeID),
    ShiftDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL
);
GO

-- Bảng Transaction (Giao dịch tài chính)
CREATE TABLE [Transaction] (
    TransactionID INT PRIMARY KEY IDENTITY,
    TransactionType NVARCHAR(50) NOT NULL, -- (thu nhập hoặc chi tiêu)
    Amount DECIMAL(18, 2) NOT NULL,
    TransactionDate DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng Supplier (Nhà cung cấp)
CREATE TABLE Supplier (
    SupplierID INT PRIMARY KEY IDENTITY,
    SupplierName NVARCHAR(100) NOT NULL,
    ContactNumber NVARCHAR(20),
    Address NVARCHAR(200)
);
GO

-- Bảng PurchaseOrder (Đơn nhập hàng)
CREATE TABLE PurchaseOrder (
    PurchaseOrderID INT PRIMARY KEY IDENTITY,
    SupplierID INT FOREIGN KEY REFERENCES Supplier(SupplierID),
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 2) NOT NULL
);
GO

-- Bảng PurchaseOrderDetail (Chi tiết đơn nhập hàng)
CREATE TABLE PurchaseOrderDetail (
    PurchaseOrderDetailID INT PRIMARY KEY IDENTITY,
    PurchaseOrderID INT FOREIGN KEY REFERENCES PurchaseOrder(PurchaseOrderID),
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL
);
GO

-- Bảng Attendance (Chấm công)
CREATE TABLE Attendance (
    AttendanceID INT PRIMARY KEY IDENTITY,
    EmployeeID INT FOREIGN KEY REFERENCES Employee(EmployeeID),
    Date DATE NOT NULL,
    Status NVARCHAR(50) NOT NULL -- (đi làm, nghỉ phép, nghỉ không lương, ...)
);
GO

-- Bảng Bonus (Thưởng phạt)
CREATE TABLE Bonus (
    BonusID INT PRIMARY KEY IDENTITY,
    EmployeeID INT FOREIGN KEY REFERENCES Employee(EmployeeID),
    BonusAmount DECIMAL(18, 2) NOT NULL,
    BonusDate DATETIME NOT NULL DEFAULT GETDATE(),
    BonusType NVARCHAR(50) NOT NULL -- (thưởng hoặc phạt)
);
GO
