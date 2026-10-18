using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DeNghiTuyenDungModel
    {
        public int Id { get; set; }
        public int DonUngTuyenId { get; set; }
        public decimal MucLuongDeNghi { get; set; }
        public DateTime NgayBatDauLamViec { get; set; }
        public string TrangThai { get; set; }
    }
    // MỚI THÊM — dùng riêng cho request gửi đề nghị, không cần Id/TrangThai
    public class GuiDeNghiModel
    {
        [Range(1, int.MaxValue, ErrorMessage = LoiValidate.IdHopLe)]
        public int DonUngTuyenId { get; set; }

        [Range(1.0, 1e12, ErrorMessage = LoiValidate.SoDuong)]
        public decimal MucLuongDeNghi { get; set; }

        public DateTime NgayBatDauLamViec { get; set; }
    }
}
