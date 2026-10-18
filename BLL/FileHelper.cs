using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public static class FileHelper
    {
        public static readonly string[] DuoiCV = { ".pdf", ".doc", ".docx" };

        public static string LayContentType(string duoiFile)
        {
            switch ((duoiFile ?? "").ToLower())
            {
                case ".pdf": return "application/pdf";
                case ".doc": return "application/msword";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                default: return "application/octet-stream";
            }
        }

        // Đọc vài byte đầu file để chắc nội dung đúng loại mà đuôi file khai báo
        public static async Task<bool> NoiDungKhopDuoiAsync(IFormFile file, string duoiFile)
        {
            byte[] dau = new byte[4];
            int docDuoc;
            using (var stream = file.OpenReadStream())
            {
                docDuoc = await stream.ReadAsync(dau, 0, dau.Length);
            }
            if (docDuoc < 4) return false;

            switch (duoiFile.ToLower())
            {
                case ".pdf":   // "%PDF"
                    return dau[0] == 0x25 && dau[1] == 0x50 && dau[2] == 0x44 && dau[3] == 0x46;
                case ".docx":  // file ZIP: "PK\x03\x04"
                    return dau[0] == 0x50 && dau[1] == 0x4B && dau[2] == 0x03 && dau[3] == 0x04;
                case ".doc":   // OLE2: D0 CF 11 E0
                    return dau[0] == 0xD0 && dau[1] == 0xCF && dau[2] == 0x11 && dau[3] == 0xE0;
                default:
                    return false;
            }
        }

        // Bỏ phần đường dẫn (nếu có), cắt còn tối đa 255 ký tự nhưng giữ nguyên đuôi file
        public static string LamSachTenFile(string tenFile, int toiDa = 255)
        {
            string ten = Path.GetFileName(tenFile ?? "");
            if (ten.Length > toiDa)
            {
                string duoi = Path.GetExtension(ten);
                ten = ten.Substring(0, toiDa - duoi.Length) + duoi;
            }
            return ten;
        }

        // Lưu file với tên ngẫu nhiên, trả về tên đã mã hóa
        public static async Task<string> LuuFileAsync(IFormFile file, string thuMuc, string duoiFile)
        {
            if (!Directory.Exists(thuMuc)) Directory.CreateDirectory(thuMuc);
            string tenMaHoa = Guid.NewGuid().ToString("N") + duoiFile;
            using (var stream = new FileStream(Path.Combine(thuMuc, tenMaHoa), FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return tenMaHoa;
        }

        public static void XoaFileNeuCo(string thuMuc, string tenFile)
        {
            try
            {
                string duongDan = Path.Combine(thuMuc, Path.GetFileName(tenFile ?? ""));
                if (File.Exists(duongDan)) File.Delete(duongDan);
            }
            catch
            {
                // Dọn file lỗi không được phép làm hỏng luồng chính
            }
        }
    }
}
