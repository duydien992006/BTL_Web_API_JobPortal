using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Range(1, int.MaxValue, ErrorMessage = LoiValidate.IdHopLe)]
        public int DonUngTuyenId { get; set; }

        public DateTime ThoiGianPhongVan { get; set; }

        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(300, ErrorMessage = LoiValidate.QuaDai)]
        public string DiaDiemPhongVan { get; set; }
    }
}
