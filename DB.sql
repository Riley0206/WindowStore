CREATE DATABASE ConvenienceStoreDB;
GO
USE ConvenienceStoreDB;
GO

-- Bảng Category
CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY,
    CategoryName NVARCHAR(100) NOT NULL			
);
GO

-- Bảng Supplier
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

-- Bảng Customer
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY IDENTITY,
    CustomerName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20),
    JoinDate DATE,
    CustomerType NVARCHAR(50)
);
GO

-- Bảng Employee
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

-- Bảng Product
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

-- Bảng Order
CREATE TABLE [Order] (
    OrderID INT PRIMARY KEY IDENTITY,
    CustomerID INT FOREIGN KEY REFERENCES Customer(CustomerID),
    EmployeeID INT FOREIGN KEY REFERENCES Employee(EmployeeID),
    OrderDate DATE,
    TotalAmount DECIMAL(18, 2),
    PaymentMethod NVARCHAR(50),
    Discount DECIMAL(18, 2),
    Note NVARCHAR(255)
);
GO

-- Bảng OrderDetail
CREATE TABLE OrderDetail (
    OrderDetailID INT PRIMARY KEY IDENTITY,
    OrderID INT FOREIGN KEY REFERENCES [Order](OrderID),
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID),
    Quantity INT,
    UnitPrice DECIMAL(18, 2),
    Discount DECIMAL(18, 2)
);
GO


-- Bảng Shift
CREATE TABLE Shift (
    ShiftID INT PRIMARY KEY IDENTITY,
    EmployeeID INT FOREIGN KEY REFERENCES Employee(EmployeeID),
    ShiftDate DATE,
    StartTime TIME,
    EndTime TIME,
    Status NVARCHAR(50),
    Note NVARCHAR(255)
);
GO

-- Bảng Transaction
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

-- Bảng PurchaseOrder
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

-- Bảng PurchaseOrderDetail
CREATE TABLE PurchaseOrderDetail (
    PurchaseOrderDetailID INT PRIMARY KEY IDENTITY,
    PurchaseOrderID INT FOREIGN KEY REFERENCES PurchaseOrder(PurchaseOrderID),
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID),
    Quantity INT,
    UnitPrice DECIMAL(18, 2)
);
GO

-- Bảng Attendance
CREATE TABLE Attendance (
    AttendanceID INT PRIMARY KEY IDENTITY,
    EmployeeID INT FOREIGN KEY REFERENCES Employee(EmployeeID),
    Date DATE,
    Status NVARCHAR(50),
    TimeIn TIME,
    TimeOut TIME,
    Note NVARCHAR(255)
);
GO

-- Bảng Bonus
CREATE TABLE Bonus (
    BonusID INT PRIMARY KEY IDENTITY,
    EmployeeID INT FOREIGN KEY REFERENCES Employee(EmployeeID),
    BonusAmount DECIMAL(18, 2),
    BonusDate DATE,
    BonusType NVARCHAR(50),
    Reason NVARCHAR(255)
);
GO
