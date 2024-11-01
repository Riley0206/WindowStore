-- Sử dụng cơ sở dữ liệu
USE ConvenienceStoreDB;
GO

-- Thêm dữ liệu mẫu vào bảng Category (Loại sản phẩm)
INSERT INTO Category (CategoryName) VALUES 
    (N'Đồ uống'),
    (N'Bánh kẹo'),
    (N'Sữa và chế phẩm từ sữa'),
    (N'Gia vị và gia dụng'),
    (N'Hóa mỹ phẩm');
GO

-- Thêm dữ liệu mẫu vào bảng Product (Sản phẩm)
INSERT INTO Product (ProductName, CategoryID, Brand, QuantityInStock, ReorderLevel, Price) VALUES 
    (N'Nước ngọt Coca-Cola', 1, N'Coca-Cola', 50, 10, 10.00),
    (N'Nước cam ép', 1, N'Tropicana', 30, 5, 15.00),
    (N'Bánh Oreo', 2, N'Oreo', 100, 20, 5.00),
    (N'Sữa tươi Vinamilk', 3, N'Vinamilk', 70, 10, 12.00),
    (N'Dầu ăn Tường An', 4, N'Tường An', 20, 5, 25.00),
    (N'Nước rửa chén Sunlight', 5, N'Sunlight', 40, 10, 20.00);
GO

-- Thêm dữ liệu mẫu vào bảng Customer (Khách hàng)
INSERT INTO Customer (CustomerName, PhoneNumber, Email) VALUES 
    (N'Nguyễn Văn A', '0123456789', 'vana@gmail.com'),
    (N'Trần Thị B', '0987654321', 'thib@gmail.com'),
    (N'Phạm Văn C', '0912345678', 'vanc@gmail.com');
GO

-- Thêm dữ liệu mẫu vào bảng Employee (Nhân viên)
INSERT INTO Employee (EmployeeName, Position, HireDate, Salary) VALUES 
    (N'Lê Thị D', N'Thu ngân', '2022-05-01', 6000000),
    (N'Nguyễn Văn E', N'Quản lý', '2021-01-15', 10000000),
    (N'Trần Văn F', N'Nhân viên kho', '2023-02-20', 7000000);
GO

-- Thêm dữ liệu mẫu vào bảng Supplier (Nhà cung cấp)
INSERT INTO Supplier (SupplierName, ContactNumber, Address) VALUES 
    (N'Công ty TNHH Nước Giải Khát', '0293847563', N'123 Đường A, Quận B, TP HCM'),
    (N'Công ty Sữa Vinamilk', '0182398456', N'456 Đường C, Quận D, TP HCM');
GO

-- Thêm dữ liệu mẫu vào bảng Order (Đơn hàng)
INSERT INTO [Order] (CustomerID, OrderDate, TotalAmount) VALUES 
    (1, '2024-10-01', 50.00),
    (2, '2024-10-05', 100.00),
    (3, '2024-10-10', 60.00);
GO

-- Thêm dữ liệu mẫu vào bảng OrderDetail (Chi tiết đơn hàng)
INSERT INTO OrderDetail (OrderID, ProductID, Quantity, UnitPrice) VALUES 
    (1, 1, 5, 10.00),
    (2, 2, 6, 15.00),
    (3, 3, 10, 5.00);
GO

-- Thêm dữ liệu mẫu vào bảng Shift (Ca làm việc)
INSERT INTO Shift (EmployeeID, ShiftDate, StartTime, EndTime) VALUES 
    (1, '2024-10-01', '08:00', '16:00'),
    (2, '2024-10-01', '08:00', '16:00'),
    (3, '2024-10-01', '08:00', '16:00');
GO

-- Thêm dữ liệu mẫu vào bảng Transaction (Giao dịch tài chính)
INSERT INTO [Transaction] (TransactionType, Amount, TransactionDate) VALUES 
    (N'Thu nhập', 200000, '2024-10-01'),
    (N'Chi tiêu', 50000, '2024-10-02'),
    (N'Thu nhập', 300000, '2024-10-05');
GO

-- Thêm dữ liệu mẫu vào bảng PurchaseOrder (Đơn nhập hàng)
INSERT INTO PurchaseOrder (SupplierID, OrderDate, TotalAmount) VALUES 
    (1, '2024-09-25', 1000.00),
    (2, '2024-09-27', 1500.00);
GO

-- Thêm dữ liệu mẫu vào bảng PurchaseOrderDetail (Chi tiết đơn nhập hàng)
INSERT INTO PurchaseOrderDetail (PurchaseOrderID, ProductID, Quantity, UnitPrice) VALUES 
    (1, 1, 50, 10.00),
    (2, 4, 30, 12.00);
GO

-- Thêm dữ liệu mẫu vào bảng Attendance (Chấm công)
INSERT INTO Attendance (EmployeeID, Date, Status) VALUES 
    (1, '2024-10-01', N'Đi làm'),
    (2, '2024-10-01', N'Đi làm'),
    (3, '2024-10-01', N'Đi làm');
GO

-- Thêm dữ liệu mẫu vào bảng Bonus (Thưởng phạt)
INSERT INTO Bonus (EmployeeID, BonusAmount, BonusDate, BonusType) VALUES 
    (1, 200000, '2024-10-05', N'Thưởng'),
    (2, 50000, '2024-10-10', N'Phạt');
GO
