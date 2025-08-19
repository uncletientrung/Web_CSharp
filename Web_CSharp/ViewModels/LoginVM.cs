
using System.ComponentModel.DataAnnotations;

namespace Web_CSharp.ViewModels
{
    public class LoginVM
    {
        [Display(Name ="Tên đăng nhập")]
        [Required(ErrorMessage ="Chưa nhập tài khoản")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string UserName {  get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Chưa nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
