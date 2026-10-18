using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Model
{
    public class PhanTrangModel<T>
    {
        public List<T> DanhSach { get; set; } = new List<T>();
        public int TongSo { get; set; }
        public int Trang { get; set; }
        public int KichThuoc { get; set; }
        public int TongSoTrang
        {
            get { return KichThuoc > 0 ? (int)Math.Ceiling(TongSo / (double)KichThuoc) : 0; }
        }

        public static PhanTrangModel<T> Tao(List<T> danhSach, int tongSo, int trang, int kichThuoc)
        {
            return new PhanTrangModel<T> { DanhSach = danhSach, TongSo = tongSo, Trang = trang, KichThuoc = kichThuoc };
        }
    }

    public class PhanTrangThongBaoModel : PhanTrangModel<ThongBaoModel>
    {
        public int SoChuaDoc { get; set; }
    }

    // Tập kết quả đầu tiên của sp_thongbao_layTheoNguoiDung
    public class TongThongBaoModel
    {
        public int TongSo { get; set; }
        public int SoChuaDoc { get; set; }
    }

    // 1 dòng trong danh sách tìm việc
    public class TinTimKiemModel
    {
        public int Id { get; set; }
        public int CongTyId { get; set; }
        public string TenCongTy { get; set; }
        public string LogoUrl { get; set; }
        public bool CongTyDaXacMinh { get; set; }
        public string TieuDe { get; set; }
        public string DiaDiem { get; set; }
        public decimal MucLuongTu { get; set; }
        public decimal MucLuongDen { get; set; }
        public DateTime NgayDang { get; set; }
        public DateTime NgayHetHan { get; set; }

        // SP trả kỹ năng thành 1 chuỗi "C#|SQL Server", đoạn dưới tách thành mảng cho frontend
        [JsonIgnore]
        public string KyNangChuoi { get; set; }

        public List<string> KyNang
        {
            get
            {
                return string.IsNullOrEmpty(KyNangChuoi)
                    ? new List<string>()
                    : KyNangChuoi.Split('|').ToList();
            }
        }
    }

    public class TinGoiYModel : TinTimKiemModel
    {
        public int SoKyNangTrungKhop { get; set; }
    }
}
