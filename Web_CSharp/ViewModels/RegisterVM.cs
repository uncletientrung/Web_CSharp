using System.ComponentModel.DataAnnotations;

namespace Web_CSharp.ViewModels
{
    public class RegisterVM
    {
        [Display(Name ="Tên đăng nhập")]
        [Required(ErrorMessage="*")] // Đây là thông báo lỗi nếu người dùng không nhập đủ tt yêu cầu
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string MaKh { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)] // Khi nhập dữ liệu nó sẽ ẩn dữ liệu nhập thành ***
        public string MatKhau { get; set; }

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string HoTen { get; set; }

        public bool GioiTinh { get; set; } = true;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)] // Set kiểu định dạng nhập vào là datet
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Địa chỉ")]
        [MaxLength(60, ErrorMessage = "Tối đa 60 ký tự")]
        public string DiaChi { get; set; }

        [Display(Name = "Số điện thoại")]
        [MaxLength(10, ErrorMessage = "Tối đa 10 ký tự")]
        [RegularExpression(@"0[9875]\d{8}", ErrorMessage ="Chưa đúng định dạng di động Việc Nam")]
        public string DienThoai { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage="Chưa đúng định dạng")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Thiếu hình")]
        public string Hinh { get; set; }
    }
}
