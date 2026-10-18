using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    // Danh sách tin kèm tên công ty và số đơn (dùng cho "tin của tôi" và "tin chờ duyệt")
    public class TinTuyenDungTomTatModel
    {
        public int Id { get; set; }
        public int CongTyId { get; set; }
        public string TenCongTy { get; set; }
        public bool CongTyDaXacMinh { get; set; }
        public string TieuDe { get; set; }
        public string DiaDiem { get; set; }
        public decimal MucLuongTu { get; set; }
        public decimal MucLuongDen { get; set; }
        public string TrangThai { get; set; }
        public DateTime NgayDang { get; set; }
        public DateTime NgayHetHan { get; set; }
        public int SoDonUngTuyen { get; set; }
    }

    public class DonUngTuyenChiTietModel
    {
        public int Id { get; set; }
        public int TinTuyenDungId { get; set; }
        public int HoSoId { get; set; }
        public int UngVienId { get; set; }
        public string TrangThai { get; set; }
        public DateTime NgayUngTuyen { get; set; }
        public string TieuDe { get; set; }
        public string DiaDiem { get; set; }
        public string TenCongTy { get; set; }
        public string HoTenUngVien { get; set; }
        public string EmailUngVien { get; set; }
        public string TenFileGoc { get; set; }
    }

    public class ChiTietDonUngTuyenModel
    {
        public DonUngTuyenChiTietModel Don { get; set; }
        public List<LichPhongVanModel> LichPhongVan { get; set; } = new List<LichPhongVanModel>();
        public List<DeNghiTuyenDungModel> DeNghi { get; set; } = new List<DeNghiTuyenDungModel>();
    }

    public class LichPhongVanCuaToiModel
    {
        public int Id { get; set; }
        public int DonUngTuyenId { get; set; }
        public DateTime ThoiGianPhongVan { get; set; }
        public string DiaDiemPhongVan { get; set; }
        public string KetQua { get; set; }
        public string TieuDe { get; set; }
        public string TenCongTy { get; set; }
        public string HoTenUngVien { get; set; }
    }

    public class FileTaiVeModel
    {
        public string DuongDanVatLy { get; set; }
        public string TenFileGoc { get; set; }
        public string ContentType { get; set; }
    }
}
