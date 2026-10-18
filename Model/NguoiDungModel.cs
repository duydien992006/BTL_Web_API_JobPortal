using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class NguoiDungModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string MatKhauMaHoa { get; set; }
        public string VaiTro { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime NgayTao { get; set; }
        public bool BiKhoa { get; set; }
        public string LyDoKhoa { get; set; }
    }

    public class DangKyModel
    {
        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(100, ErrorMessage = LoiValidate.QuaDai)]
        [EmailAddress(ErrorMessage = LoiValidate.Email)]
        public string Email { get; set; }

        // Tối đa 72 vì BCrypt chỉ dùng 72 byte đầu của mật khẩu
        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(72, MinimumLength = 6, ErrorMessage = LoiValidate.DoDai)]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(20, ErrorMessage = LoiValidate.QuaDai)]
        public string VaiTro { get; set; }

        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(100, ErrorMessage = LoiValidate.QuaDai)]
        public string HoTen { get; set; }

        [StringLength(20, ErrorMessage = LoiValidate.QuaDai)]
        public string SoDienThoai { get; set; }
    }

    public class DangNhapModel
    {
        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(100, ErrorMessage = LoiValidate.QuaDai)]
        public string Email { get; set; }

        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(72, ErrorMessage = LoiValidate.QuaDai)]
        public string MatKhau { get; set; }
    }

    public class CapNhatNguoiDungModel
    {
        public int Id { get; set; }   // Controller tự gán từ token

        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(100, ErrorMessage = LoiValidate.QuaDai)]
        public string HoTen { get; set; }

        [StringLength(20, ErrorMessage = LoiValidate.QuaDai)]
        public string SoDienThoai { get; set; }
    }

    public class HoSoCaNhanModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string VaiTro { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
