using System;
using System.Collections.Generic;
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
    }

    public class DangKyModel
    {
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public string VaiTro { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
    }

    public class DangNhapModel
    {
        public string Email { get; set; }
        public string MatKhau { get; set; }
    }

    public class CapNhatNguoiDungModel
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
    }
}
