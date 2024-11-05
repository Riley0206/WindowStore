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
INSERT INTO Product (ProductName, CategoryID, Brand, QuantityInStock, Price, CostPrice, Unit) VALUES 
    (N'Nước ngọt Coca-Cola', 1, N'Coca-Cola', 100, 10.00, 8.00, N'Lon'),
    (N'Nước cam ép Tropicana', 1, N'Tropicana', 50, 15.00, 12.00, N'Hộp'),
    (N'Bánh Oreo', 2, N'Oreo', 150, 5.00, 3.00, N'Gói'),
    (N'Sữa tươi Vinamilk', 3, N'Vinamilk', 80, 12.00, 10.00, N'Hộp'),
    (N'Dầu ăn Tường An', 4, N'Tường An', 30, 25.00, 20.00, N'Chai'),
    (N'Nước rửa chén Sunlight', 5, N'Sunlight', 60, 20.00, 18.00, N'Chai');
GO

-- Thêm dữ liệu mẫu vào bảng Customer (Khách hàng)
INSERT INTO Customer (CustomerName, PhoneNumber, JoinDate, CustomerType) VALUES 
    (N'Nguyễn Văn A', '0123456789', '2023-05-15', N'Bình Thường'),
    (N'Trần Thị B', '0987654321', '2024-01-20', N'Thỉnh thoảng'),
    (N'Phạm Văn C', '0912345678', '2024-03-10', N'Thường xuyên');
GO

-- Thêm dữ liệu mẫu vào bảng Employee (Nhân viên)
INSERT INTO Employee (EmployeeName, Position, HireDate, Salary, PhoneNumber, Address, Email, Birthday, IDNumber) VALUES 
    (N'Lê Thị D', N'Thu ngân', '2022-05-01', 6000000, '0123456789', N'123 Đường ABC, Quận 1', N'lethid@gmail.com', '1990-06-15', '123456789'),
    (N'Nguyễn Văn E', N'Quản lý', '2021-01-15', 10000000, '0987654321', N'456 Đường XYZ, Quận 2', N'nguyenvane@gmail.com', '1985-02-20', '987654321'),
    (N'Trần Văn F', N'Nhân viên kho', '2023-02-20', 7000000, '0912345678', N'789 Đường LMN, Quận 3', N'tranvanf@gmail.com', '1995-12-10', '123123123');
GO

-- Thêm dữ liệu mẫu vào bảng Supplier (Nhà cung cấp)
INSERT INTO Supplier (SupplierName, ContactNumber, Address, Email, TaxCode, ContactPerson, Status) VALUES 
    (N'Công ty TNHH Nước Giải Khát', '0293847563', N'123 Đường A, Quận B, TP HCM', N'contact@nuocgiaikhat.com', '123456789', N'Nguyễn Văn B', N'Hoạt động'),
    (N'Công ty Sữa Vinamilk', '0182398456', N'456 Đường C, Quận D, TP HCM', N'contact@vinamilk.com', '987654321', N'Phạm Thị D', N'Hoạt động');
GO

-- Thêm dữ liệu mẫu vào bảng Order (Đơn hàng)
INSERT INTO [Order] (CustomerID, EmployeeID, OrderDate, TotalAmount, PaymentMethod, Discount, Note) VALUES 
    (1, 1, '2024-10-01', 150.00, N'Tiền mặt', 0, N'Không ghi chú'),
    (2, 2, '2024-10-05', 200.00, N'Thẻ ngân hàng', 10.00, N'Giảm giá cho khách quen'),
    (3, 1, '2024-10-10', 100.00, N'Momo', 5.00, N'Khách quen');
GO

-- Thêm dữ liệu mẫu vào bảng OrderDetail (Chi tiết đơn hàng)
INSERT INTO OrderDetail (OrderID, ProductID, Quantity, UnitPrice, Discount) VALUES 
    (1, 1, 5, 10.00, 0),
    (1, 3, 3, 5.00, 0),
    (2, 2, 6, 15.00, 10.00),
    (3, 4, 10, 12.00, 5.00);
GO

-- Thêm dữ liệu mẫu vào bảng Shift (Ca làm việc)
INSERT INTO Shift (EmployeeID, ShiftDate, StartTime, EndTime, Status, Note) VALUES 
    (1, '2024-10-01', '08:00', '16:00', N'Hoàn thành', N'Ca sáng'),
    (2, '2024-10-01', '14:00', '22:00', N'Hoàn thành', N'Ca chiều'),
    (3, '2024-10-02', '08:00', '16:00', N'Hoàn thành', N'Ca sáng');
GO

-- Thêm dữ liệu mẫu vào bảng Transaction (Giao dịch tài chính)
INSERT INTO [Transaction] (TransactionType, Amount, TransactionDate, Description, ReferenceID, ReferenceType, PaymentMethod) VALUES 
    (N'Thu nhập', 200000, '2024-10-01', N'Doanh thu bán hàng', 1, N'Order', N'Tiền mặt'),
    (N'Chi tiêu', 50000, '2024-10-02', N'Mua nguyên liệu', 2, N'PurchaseOrder', N'Thẻ ngân hàng'),
    (N'Thu nhập', 300000, '2024-10-05', N'Doanh thu bán hàng', 3, N'Order', N'Momo');
GO

-- Thêm dữ liệu mẫu vào bảng PurchaseOrder (Đơn nhập hàng)
INSERT INTO PurchaseOrder (SupplierID, EmployeeID, OrderDate, TotalAmount, Status, PaymentStatus, Note) VALUES 
    (1, 3, '2024-09-25', 1000.00, N'Hoàn thành', N'Đã thanh toán', N'Nhập kho nước giải khát'),
    (2, 2, '2024-09-27', 1500.00, N'Hoàn thành', N'Chưa thanh toán', N'Nhập kho sữa và chế phẩm');
GO

-- Thêm dữ liệu mẫu vào bảng PurchaseOrderDetail (Chi tiết đơn nhập hàng)
INSERT INTO PurchaseOrderDetail (PurchaseOrderID, ProductID, Quantity, UnitPrice) VALUES 
    (1, 1, 50, 10.00),
    (2, 4, 30, 12.00);
GO

-- Thêm dữ liệu mẫu vào bảng Attendance (Chấm công)
INSERT INTO Attendance (EmployeeID, Date, Status, TimeIn, TimeOut, Note) VALUES 
    (1, '2024-10-01', N'Đi làm', '08:00', '16:00', N'Ca sáng hoàn thành'),
    (2, '2024-10-01', N'Đi làm', '14:00', '22:00', N'Ca chiều hoàn thành'),
    (3, '2024-10-02', N'Đi làm', '08:00', '16:00', N'Ca sáng hoàn thành');
GO

-- Thêm dữ liệu mẫu vào bảng Bonus (Thưởng phạt)
INSERT INTO Bonus (EmployeeID, BonusAmount, BonusDate, BonusType, Reason) VALUES 
    (1, 200000, '2024-10-05', N'Thưởng', N'Hoàn thành vượt mức doanh thu'),
    (2, 50000, '2024-10-10', N'Phạt', N'Đi trễ 15 phút');
GO
