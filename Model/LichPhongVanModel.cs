using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class LichPhongVanModel
    {
        public int Id { get; set; }
        public int DonUngTuyenId { get; set; }
        public DateTime ThoiGianPhongVan { get; set; }
        public string DiaDiemPhongVan { get; set; }
        public string KetQua { get; set; }
    }
    // MỚI THÊM — dùng riêng cho request tạo lịch phỏng vấn, không cần Id/KetQua
    public class TaoLichPhongVanModel
    {
        public int DonUngTuyenId { get; set; }
        public DateTime ThoiGianPhongVan { get; set; }
        public string DiaDiemPhongVan { get; set; }
    }
}
