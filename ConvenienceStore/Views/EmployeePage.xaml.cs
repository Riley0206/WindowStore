using Microsoft.UI.Xaml.Controls;
using ConvenienceStore.ViewModels;
using Microsoft.UI.Xaml;
using System;
using ConvenienceStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Input;

namespace ConvenienceStore.Views
{
    public sealed partial class EmployeePage : Page
    {
        public EmployeeViewModel ViewModel { get; }

        public EmployeePage()
        {
            this.InitializeComponent();
            ViewModel = new EmployeeViewModel();
            this.DataContext = ViewModel;
        }

        private async void ShowAddShiftDialog(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.XamlRoot == null)
                {
                    throw new InvalidOperationException("XamlRoot chưa được khởi tạo.");
                }

                if (ViewModel.SelectedEmployee == null)
                {
                    await ShowErrorDialog("Lỗi", "Vui lòng chọn nhân viên trước khi thực hiện thao tác.");
                    return;
                }

                var datePicker = new DatePicker { Date = DateTimeOffset.Now };
                var shiftComboBox = new ComboBox
                {
                    ItemsSource = new List<string> { "Ca 1 (7:00 - 11:00)", "Ca 2 (11:00 - 15:00)", "Ca 3 (15:00 - 19:00)", "Ca 4 (19:00 - 23:00)" },
                    PlaceholderText = "Chọn ca làm việc"
                };

                var statusComboBox = new ComboBox
                {
                    ItemsSource = new List<string> { "Đã lên lịch", "Bị Hủy" },
                    SelectedIndex = 0
                };

                var dialog = new ContentDialog
                {
                    Title = "Thêm Ca Làm",
                    PrimaryButtonText = "Lưu",
                    CloseButtonText = "Hủy",
                    XamlRoot = this.XamlRoot,
                    Content = new StackPanel
                    {
                        Children = {
                    new TextBlock { Text = "Ngày Làm:" },
                    datePicker,
                    new TextBlock { Text = "Ca Làm:" },
                    shiftComboBox,
                    new TextBlock { Text = "Trạng Thái:" },
                    statusComboBox
                }
                    }
                };

                if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    if (shiftComboBox.SelectedItem == null)
                    {
                        await ShowErrorDialog("Lỗi", "Vui lòng chọn ca làm việc.");
                        return;
                    }

                    var newShift = new Shift
                    {
                        ShiftDate = datePicker.Date.DateTime,
                        Status = statusComboBox.SelectedItem.ToString(),
                        Note = shiftComboBox.SelectedItem.ToString().Split('(')[0].Trim()
                    };

                    try
                    {
                        await ViewModel.AssignShiftAsync(newShift);
                        await ShowSuccessDialog("Thành Công", "Đã thêm ca làm mới thành công.");
                    }
                    catch (Exception ex)
                    {
                        await ShowErrorDialog("Lỗi", $"Không thể thêm ca làm: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowErrorDialog("Lỗi", $"Đã xảy ra lỗi: {ex.Message}");
            }
        }


        private async Task ShowErrorDialog(string title, string content)
        {
            await new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            }.ShowAsync();
        }

        private async Task ShowSuccessDialog(string title, string content)
        {
            await new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            }.ShowAsync();
        }

        private async void ShowAttendanceDialog(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra XamlRoot
                if (this.XamlRoot == null)
                {
                    throw new InvalidOperationException("XamlRoot chưa được khởi tạo.");
                }

                // Kiểm tra xem có nhân viên được chọn không
                var selectedEmployee = ViewModel.SelectedEmployee;
                if (selectedEmployee == null)
                {
                    await ShowErrorDialog("Lỗi", "Hãy chọn một nhân viên trước khi điểm danh!");
                    return;
                }

                var noteTextBox = new TextBox { PlaceholderText = "Ghi chú (nếu có)", Text = "" };
                var datePicker = new DatePicker { Date = DateTimeOffset.Now };
                var shiftComboBox = new ComboBox
                {
                    ItemsSource = new List<string> {
                        "Ca 1 (7:00 - 11:00)",
                        "Ca 2 (11:00 - 15:00)",
                        "Ca 3 (15:00 - 19:00)",
                        "Ca 4 (19:00 - 23:00)"
            },
                    PlaceholderText = "Chọn ca làm việc"
                };
                var statusComboBox = new ComboBox
                {
                    ItemsSource = new List<string> { "Có Mặt", "Vắng Mặt", "Đi Muộn" }
                };
                var dialog = new ContentDialog
                {
                    Title = "Điểm Danh",
                    PrimaryButtonText = "Lưu",
                    CloseButtonText = "Hủy",
                    XamlRoot = this.XamlRoot,
                    Content = new StackPanel
                    {
                        Children = {
                    new TextBlock { Text = "Ngày:" },
                    datePicker,
                    new TextBlock { Text = "Ca Làm:" },
                    shiftComboBox,
                    new TextBlock { Text = "Trạng Thái:" },
                    statusComboBox,
                    new TextBlock { Text = "Ghi Chú:" },
                    noteTextBox
                }
                    }
                };

                if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    if (shiftComboBox.SelectedItem == null)
                    {
                        await ShowErrorDialog("Lỗi", "Vui lòng chọn ca làm việc.");
                        return;
                    }

                    if (statusComboBox.SelectedItem == null)
                    {
                        await ShowErrorDialog("Lỗi", "Vui lòng chọn trạng thái.");
                        return;
                    }

                    // Xác định giờ bắt đầu và kết thúc ca
                    TimeSpan timeIn = TimeSpan.Zero;
                    TimeSpan timeOut = TimeSpan.Zero;

                    string selectedShift = shiftComboBox.SelectedItem.ToString().Split('(')[0].Trim();
                    switch (selectedShift)
                    {
                        case "Ca 1":
                            timeIn = TimeSpan.FromHours(7);
                            timeOut = TimeSpan.FromHours(11);
                            break;
                        case "Ca 2":
                            timeIn = TimeSpan.FromHours(11);
                            timeOut = TimeSpan.FromHours(15);
                            break;
                        case "Ca 3":
                            timeIn = TimeSpan.FromHours(15);
                            timeOut = TimeSpan.FromHours(19);
                            break;
                        case "Ca 4":
                            timeIn = TimeSpan.FromHours(19);
                            timeOut = TimeSpan.FromHours(23);
                            break;
                        default:
                            throw new InvalidOperationException("Chọn ca làm không hợp lệ");
                    }

                    var newAttendance = new Attendance
                    {
                        Date = datePicker.Date.DateTime,
                        Status = statusComboBox.SelectedItem.ToString(),
                        EmployeeID = selectedEmployee.EmployeeID,
                        TimeIn = timeIn,     // Giờ vào ca theo ca làm
                        TimeOut = timeOut,   // Giờ kết thúc ca theo ca làm
                        Note = $"{shiftComboBox.SelectedItem} {noteTextBox.Text}"
                    };

                    try
                    {
                        await ViewModel.MarkAttendanceAsync(newAttendance);
                        await ShowSuccessDialog("Thành Công", "Điểm danh thành công.");
                    }
                    catch (Exception ex)
                    {
                        await ShowErrorDialog("Lỗi", $"Không thể điểm danh: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowErrorDialog("Lỗi", $"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        private async void ShowAddEmployeeDialog(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.XamlRoot == null)
                {
                    throw new InvalidOperationException("XamlRoot chưa được khởi tạo.");
                }

                var nameTextBox = new TextBox { PlaceholderText = "Tên nhân viên" };
                var positionTextBox = new TextBox { PlaceholderText = "Chức vụ" };
                var hireDatePicker = new DatePicker { Date = DateTimeOffset.Now };
                var salaryTextBox = new TextBox { PlaceholderText = "Lương", InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } } };
                var phoneTextBox = new TextBox { PlaceholderText = "Số điện thoại" };
                var addressTextBox = new TextBox { PlaceholderText = "Địa chỉ" };
                var emailTextBox = new TextBox { PlaceholderText = "Email" };
                var birthdayPicker = new DatePicker { Date = DateTimeOffset.Now };
                var idNumberTextBox = new TextBox { PlaceholderText = "Số CMND/CCCD" };

                var scrollViewer = new ScrollViewer
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Content = new StackPanel
                    {
                        Spacing = 10,
                        Children =
                {
                    new TextBlock { Text = "Tên nhân viên:" },
                    nameTextBox,
                    new TextBlock { Text = "Chức vụ:" },
                    positionTextBox,
                    new TextBlock { Text = "Ngày nhận việc:" },
                    hireDatePicker,
                    new TextBlock { Text = "Lương:" },
                    salaryTextBox,
                    new TextBlock { Text = "Số điện thoại:" },
                    phoneTextBox,
                    new TextBlock { Text = "Địa chỉ:" },
                    addressTextBox,
                    new TextBlock { Text = "Email:" },
                    emailTextBox,
                    new TextBlock { Text = "Ngày sinh:" },
                    birthdayPicker,
                    new TextBlock { Text = "Số CMND/CCCD:" },
                    idNumberTextBox
                }
                    }
                };

                var dialog = new ContentDialog
                {
                    Title = "Thêm nhân viên mới",
                    PrimaryButtonText = "Lưu",
                    CloseButtonText = "Hủy",
                    XamlRoot = this.XamlRoot,
                    Content = scrollViewer,
                    FullSizeDesired = false
                };

                if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    if (string.IsNullOrWhiteSpace(nameTextBox.Text) ||
                        string.IsNullOrWhiteSpace(positionTextBox.Text) ||
                        string.IsNullOrWhiteSpace(salaryTextBox.Text) ||
                        string.IsNullOrWhiteSpace(phoneTextBox.Text) ||
                        string.IsNullOrWhiteSpace(emailTextBox.Text) ||
                        string.IsNullOrWhiteSpace(idNumberTextBox.Text))
                    {
                        await ShowErrorDialog("Lỗi", "Vui lòng nhập đầy đủ thông tin.");
                        return;
                    }

                    var newEmployee = new Employee
                    {
                        EmployeeName = nameTextBox.Text,
                        Position = positionTextBox.Text,
                        HireDate = hireDatePicker.Date.DateTime,
                        Salary = decimal.Parse(salaryTextBox.Text),
                        PhoneNumber = phoneTextBox.Text,
                        Address = addressTextBox.Text,
                        Email = emailTextBox.Text,
                        Birthday = birthdayPicker.Date.DateTime,
                        IDNumber = idNumberTextBox.Text
                    };

                    try
                    {
                        await ViewModel.AddEmployeeAsync(newEmployee);
                        await ShowSuccessDialog("Thành công", "Nhân viên mới đã được thêm.");
                    }
                    catch (Exception ex)
                    {
                        await ShowErrorDialog("Lỗi", $"Không thể thêm nhân viên: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowErrorDialog("Lỗi", $"Đã xảy ra lỗi: {ex.Message}");
            }
        }


    }
}
