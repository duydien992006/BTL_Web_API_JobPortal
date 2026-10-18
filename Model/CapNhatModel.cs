using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        public List<LichSuTrangThaiModel> LichSu { get; set; } = new List<LichSuTrangThaiModel>();
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

    public class DonUngTuyenCuaToiModel
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
        public int? DeNghiId { get; set; }
        public decimal? MucLuongDeNghi { get; set; }
        public DateTime? NgayBatDauLamViec { get; set; }
        public string TrangThaiDeNghi { get; set; }
        public DateTime? LichPhongVanGanNhat { get; set; }
    }

    public class DonTheoTinModel
    {
        public int Id { get; set; }
        public int TinTuyenDungId { get; set; }
        public int HoSoId { get; set; }
        public int UngVienId { get; set; }
        public string TrangThai { get; set; }
        public DateTime NgayUngTuyen { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string TenFileGoc { get; set; }
    }
    // ----- Dashboard -----
    public class MucThongKeModel
    {
        public string Nhan { get; set; }
        public int SoLuong { get; set; }
    }

    public class SoDonTheoNgayModel
    {
        public DateTime Ngay { get; set; }
        public int SoDon { get; set; }
    }

    public class TopTinModel
    {
        public int TinId { get; set; }
        public string TieuDe { get; set; }
        public string TenCongTy { get; set; }
        public int SoDon { get; set; }
    }

    public class PipelineAdminModel
    {
        public List<MucThongKeModel> DonTheoTrangThai { get; set; } = new List<MucThongKeModel>();
        public List<MucThongKeModel> TinTheoTrangThai { get; set; } = new List<MucThongKeModel>();
        public List<MucThongKeModel> NguoiDungTheoVaiTro { get; set; } = new List<MucThongKeModel>();
        public List<SoDonTheoNgayModel> DonTheoNgay { get; set; } = new List<SoDonTheoNgayModel>();
        public List<TopTinModel> TopTin { get; set; } = new List<TopTinModel>();
        public int TongDon { get; set; }
        public double TyLeTrungTuyenPhanTram { get; set; }
    }

    public class DongTinPipelineModel
    {
        public int TinId { get; set; }
        public string TieuDe { get; set; }
        public string TrangThaiTin { get; set; }
        public int TongDon { get; set; }
        public int DaNop { get; set; }
        public int DangXemXet { get; set; }
        public int PhongVan { get; set; }
        public int DeNghi { get; set; }
        public int TrungTuyen { get; set; }
        public int UngVienTuChoi { get; set; }
        public int TuChoi { get; set; }
        public int DaRut { get; set; }
    }

    public class LichSapToiModel
    {
        public int Id { get; set; }
        public int DonUngTuyenId { get; set; }
        public DateTime ThoiGianPhongVan { get; set; }
        public string DiaDiemPhongVan { get; set; }
        public string TieuDe { get; set; }
        public string HoTenUngVien { get; set; }
    }

    public class ViecCanLamModel
    {
        public int DonChoXemXet { get; set; }
        public int LichChuaCoKetQua { get; set; }
        public int DeNghiChoPhanHoi { get; set; }
        public int TinChoDuyet { get; set; }
    }

    public class DashboardNhaTuyenDungModel
    {
        public List<MucThongKeModel> Pipeline { get; set; } = new List<MucThongKeModel>();
        public List<DongTinPipelineModel> TheoTin { get; set; } = new List<DongTinPipelineModel>();
        public List<LichSapToiModel> LichSapToi { get; set; } = new List<LichSapToiModel>();
        public ViecCanLamModel ViecCanLam { get; set; } = new ViecCanLamModel();
        public int TongDon { get; set; }
    }

    // ----- Lịch sử trạng thái đơn -----
    public class LichSuTrangThaiModel
    {
        public string TuTrangThai { get; set; }
        public string DenTrangThai { get; set; }
        public DateTime ThoiGian { get; set; }
        public string GhiChu { get; set; }
    }

    // ----- Mẫu CV -----
    public class MauCVModel
    {
        public int Id { get; set; }
        public string TenMau { get; set; }
        public string MoTa { get; set; }
        public string TenFileGoc { get; set; }
        public DateTime NgayTao { get; set; }

        [JsonIgnore]   // không bao giờ trả đường dẫn thật trên server ra ngoài
        public string DuongDanFile { get; set; }
    }

    // ----- Quản lý người dùng -----
    public class NguoiDungQuanLyModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string VaiTro { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime NgayTao { get; set; }
        public bool BiKhoa { get; set; }
        public string LyDoKhoa { get; set; }
    }

    // ----- Đổi / quên mật khẩu -----
    public class DoiMatKhauModel
    {
        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(72, ErrorMessage = LoiValidate.QuaDai)]
        public string MatKhauCu { get; set; }

        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(72, MinimumLength = 6, ErrorMessage = LoiValidate.DoDai)]
        public string MatKhauMoi { get; set; }
    }

    public class QuenMatKhauModel
    {
        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(100, ErrorMessage = LoiValidate.QuaDai)]
        [EmailAddress(ErrorMessage = LoiValidate.Email)]
        public string Email { get; set; }
    }

    public class DatLaiMatKhauModel
    {
        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(100, ErrorMessage = LoiValidate.QuaDai)]
        public string Email { get; set; }

        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(200, ErrorMessage = LoiValidate.QuaDai)]
        public string Token { get; set; }

        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(72, MinimumLength = 6, ErrorMessage = LoiValidate.DoDai)]
        public string MatKhauMoi { get; set; }
    }
}
