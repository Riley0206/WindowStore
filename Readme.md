# Báo cáo nghiệm thu cho milestone 1
## Danh sách thành viên và phân công:
+ 21120252 Võ Hoàng Nam Hưng: code chức năng quản lí kho hàng 
+ 21120251 Trần Quang Hưng: code chức năng quản lí đơn hàng

## UI/UX
Link folder ảnh: https://drive.google.com/drive/folders/1DWDeQyvLGkfn9YpukHUtpBUVcAazgufe?usp=sharing
## Design patterns / architecture
- Sử dụng MVVM pattern: Kiến trúc ứng dụng được thiết kế theo mẫu MVVM (Model-View-ViewModel), tách biệt logic của ứng dụng khỏi giao diện người dùng.
## Advanced topics
- Sử dụng RelayCommand: Trong InventoryViewModel, các phương thức được định nghĩa là các RelayCommand, giúp tách biệt logic điều khiển khỏi giao diện.
- Sử dụng ObservableCollection: Các thuộc tính trong InventoryViewModel sử dụng ObservableCollection, cho phép cập nhật giao diện khi dữ liệu thay đổi.
- Sử dụng ContentDialog: Trong InventoryPage, các hộp thoại ContentDialog được sử dụng để hiển thị các thông báo, yêu cầu nhập liệu từ người dùng.
- Lọc và tìm kiếm sản phẩm: Trong InventoryViewModel, có các phương thức FilterProductsByCategory(), LoadAllProducts() và SearchProductsByName() để lọc và tìm kiếm sản phẩm dựa trên danh mục và tên sản phẩm.
- Sử dụng ScrollViewer: Trong InventoryPage, ScrollViewer được sử dụng để hiển thị các control khi số lượng lớn.
## Teamwork - Git flow
https://github.com/Riley0206/WindowStore.git

## Quality assurance: 
Trong lúc chụp ảnh UI, chúng em đã có test các chức năng và chụp chung rồi nên thầy có thể xem trong folder ảnh của mục UI/UX.