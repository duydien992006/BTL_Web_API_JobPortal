using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class TinTuyenDungModel
    {
        public int Id { get; set; }
        public int CongTyId { get; set; }
        public string TieuDe { get; set; }
        public string MoTaCongViec { get; set; }
        public string DiaDiem { get; set; }
        public decimal MucLuongTu { get; set; }
        public decimal MucLuongDen { get; set; }
        public string TrangThai { get; set; }
        public DateTime NgayDang { get; set; }
        public DateTime NgayHetHan { get; set; }
        public List<KyNangModel> DanhSachKyNang { get; set; } = new List<KyNangModel>();
    }

    public class TaoTinTuyenDungModel
    {
        public int CongTyId { get; set; }

        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(200, ErrorMessage = LoiValidate.QuaDai)]
        public string TieuDe { get; set; }

        [StringLength(10000, ErrorMessage = LoiValidate.QuaDai)]
        public string MoTaCongViec { get; set; }

        [StringLength(200, ErrorMessage = LoiValidate.QuaDai)]
        public string DiaDiem { get; set; }

        [Range(0.0, 1e12, ErrorMessage = LoiValidate.KhongAm)]
        public decimal MucLuongTu { get; set; }

        [Range(0.0, 1e12, ErrorMessage = LoiValidate.KhongAm)]
        public decimal MucLuongDen { get; set; }

        public DateTime NgayHetHan { get; set; }

        [MaxLength(30, ErrorMessage = LoiValidate.QuaNhieu)]
        public List<int> DanhSachKyNangId { get; set; } = new List<int>();
    }

    public class TimKiemTinModel
    {
        [StringLength(200, ErrorMessage = LoiValidate.QuaDai)]
        public string TuKhoa { get; set; }

        [StringLength(200, ErrorMessage = LoiValidate.QuaDai)]
        public string DiaDiem { get; set; }

        [Range(0.0, 1e12, ErrorMessage = LoiValidate.KhongAm)]
        public decimal? LuongToiThieu { get; set; }

        [MaxLength(30, ErrorMessage = LoiValidate.QuaNhieu)]
        public List<int> DanhSachKyNangId { get; set; } = new List<int>();

        [Range(1, 100000, ErrorMessage = LoiValidate.Khoang)]
        public int Trang { get; set; } = 1;

        [Range(1, 50, ErrorMessage = LoiValidate.Khoang)]
        public int KichThuoc { get; set; } = 10;
    }
}
