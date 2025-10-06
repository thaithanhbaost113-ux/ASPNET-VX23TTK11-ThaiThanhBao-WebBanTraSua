using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    public class UploadController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                // Danh sách các định dạng tệp hợp lệ
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(upload.FileName).ToLower();

                // Kiểm tra định dạng tệp
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { uploaded = false, error = "Tệp không hợp lệ. Vui lòng chọn ảnh với định dạng .jpg, .jpeg, .png, hoặc .gif." });
                }

                // Kiểm tra dung lượng tệp (5MB)
                if (upload.Length > 5 * 1024 * 1024)
                {
                    return Json(new { uploaded = false, error = "Tệp quá lớn. Dung lượng tối đa là 5MB." });
                }

                // Tạo tên tệp và đường dẫn lưu trữ tệp vào thư mục "wwwroot/Image"
                var fileName = Path.GetFileName(upload.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Image"); // Đặt thư mục là Image trong wwwroot
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);  // Tạo thư mục nếu không tồn tại
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu tệp vào thư mục
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }

                // Trả về URL của ảnh đã tải lên
                var fileUrl = $"/Image/{fileName}";
                return Json(new { uploaded = true, url = fileUrl });
            }

            return Json(new { uploaded = false, error = "Không có ảnh được tải lên." });
        }
    }
}
