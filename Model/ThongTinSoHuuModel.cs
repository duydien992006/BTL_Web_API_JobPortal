using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    // Người đang gọi API, lấy từ JWT
    public class NguoiDungHienTai
    {
        public int Id { get; set; }
        public string VaiTro { get; set; }
        public bool LaAdmin { get { return VaiTro == "QuanTriVien"; } }
    }

    // Kết quả tra cứu chủ sở hữu. SP nào không trả cột nào thì cột đó giữ giá trị mặc định (0/null)
    public class ThongTinSoHuuModel
    {
        public int Id { get; set; }
        public int UngVienId { get; set; }
        public int NhaTuyenDungId { get; set; }
        public int DonUngTuyenId { get; set; }
        public string TrangThai { get; set; }
    }
}
