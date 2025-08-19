using System.Text;

namespace Web_CSharp.Helpers
{
    public class MyUntil
    {
        public static string GenerateRamdomKey(int length = 5)
        {
            var pattern = @"qwertyuiopasdfghjklzxcvbnmQWERTYUIOASDFGHJKLZXCVBNM";
            var sb = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[random.Next(0, pattern.Length)]);
            }

            return sb.ToString();
        }

        public static string UploadHinh(IFormFile Hinh, string folder)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(),  // thư mục hiện tại nơi app đang chạy
                "wwwroot",                                      // thư mục chứa static files trong ASP.NET Core
                "Hinh",                                         // thư mục con tên "Hinh"
                folder,                                         // tên thư mục con nữa (do bạn truyền vào biến folder)
                Hinh.FileName);                                 // tên file upload (ví dụ: abc.jpg)
                                                                // Tạo full đường dẫn Vd: C:\Users\DELL\source\repos\Web_CSharp\Web_CSharp\wwwroot\Hinh\SanPham\abc.jpg

                using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
                {
                    // Sử dụng using để Đảm bảo khi xong việc, đối tượng FileStream sẽ được đóng và giải phóng tài nguyên
                    // new FileStream dùng để mở 1 stream đến file ở vị trí fullPath
                    // FileMode.CreateNew là Nếu file chưa tồn tại → tạo file mới
                    // Nếu file đã tồn tại → báo lỗi IOException (không ghi đè).

                    Hinh.CopyTo(myfile); // ghi dữ liệu xuống file
                }
                return Hinh.FileName;
            }
            catch (Exception ex) { 
                return String.Empty;
            
            }
            

        }
    }
}
