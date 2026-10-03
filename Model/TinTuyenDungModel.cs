using System;
using System.Collections.Generic;
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
        public string TieuDe { get; set; }
        public string MoTaCongViec { get; set; }
        public string DiaDiem { get; set; }
        public decimal MucLuongTu { get; set; }
        public decimal MucLuongDen { get; set; }
        public DateTime NgayHetHan { get; set; }
        public List<int> DanhSachKyNangId { get; set; } = new List<int>();
    }

    public class TimKiemTinModel
    {
        public string TuKhoa { get; set; }
        public string DiaDiem { get; set; }
        public List<int> DanhSachKyNangId { get; set; } = new List<int>();
    }
}
