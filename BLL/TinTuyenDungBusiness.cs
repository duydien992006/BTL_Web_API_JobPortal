using BLL.Interfaces;
using DAL.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class TinTuyenDungBusiness : ITinTuyenDungBusiness
    {
        private readonly ITinTuyenDungRepository _repo;
        private readonly IHoSoRepository _hoSoRepo;
        private readonly ICongTyRepository _congTyRepo;
        private readonly ISoHuuRepository _soHuuRepo;

        public TinTuyenDungBusiness(ITinTuyenDungRepository repo, IHoSoRepository hoSoRepo,
            ICongTyRepository congTyRepo, ISoHuuRepository soHuuRepo)
        {
            _repo = repo;
            _hoSoRepo = hoSoRepo;
            _congTyRepo = congTyRepo;
            _soHuuRepo = soHuuRepo;
        }

        public async Task<PhanHoiModel> TaoAsync(TaoTinTuyenDungModel model, NguoiDungHienTai nguoi)
        {
            // T3: công ty phải tồn tại, thuộc về người đang đăng nhập và đã được Admin xác minh
            var congTy = QuyenHelper.PhaiTonTai(await _congTyRepo.LayTheoIdAsync(model.CongTyId), "Không tìm thấy công ty");
            QuyenHelper.PhaiLaChuSoHuu(congTy.NguoiDaiDienId, nguoi);
            if (!congTy.DaXacMinh)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Công ty chưa được quản trị viên xác minh, chưa thể đăng tin" };

            if (model.DanhSachKyNangId == null || model.DanhSachKyNangId.Count == 0)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Phải chọn ít nhất 1 kỹ năng yêu cầu" };

            if (model.MucLuongTu > model.MucLuongDen)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Mức lương không hợp lệ" };

            if (model.NgayHetHan <= DateTime.UtcNow)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Ngày hết hạn phải lớn hơn hiện tại" };

            int id = await _repo.TaoAsync(model);
            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Tạo tin thành công, đang chờ duyệt", DuLieu = id };
        }

        public async Task<PhanHoiModel> CapNhatAsync(int id, TaoTinTuyenDungModel model, NguoiDungHienTai nguoi)
        {
            var soHuu = QuyenHelper.PhaiTonTai(await _soHuuRepo.LayThongTinTinAsync(id), "Không tìm thấy tin tuyển dụng");
            QuyenHelper.PhaiLaChuSoHuu(soHuu.NhaTuyenDungId, nguoi);

            bool ok = await _repo.CapNhatAsync(id, model);
            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Cập nhật thành công" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy tin tuyển dụng" };
        }

        public async Task<PhanHoiModel> DuyetAsync(int id, bool duyet)
        {
            string trangThai = duyet ? "DaDuyet" : "TuChoi";
            bool ok = await _repo.DuyetAsync(id, trangThai);
            if (ok)
            {
                string thongDiep = duyet ? "Đã duyệt tin" : "Đã từ chối tin";
                return new PhanHoiModel { ThanhCong = true, ThongDiep = thongDiep };
            }
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy tin tuyển dụng" };
        }

        public async Task<PhanHoiModel> DongAsync(int id, NguoiDungHienTai nguoi)
        {
            var soHuu = QuyenHelper.PhaiTonTai(await _soHuuRepo.LayThongTinTinAsync(id), "Không tìm thấy tin tuyển dụng");
            QuyenHelper.PhaiLaChuSoHuu(soHuu.NhaTuyenDungId, nguoi);

            bool ok = await _repo.DongAsync(id);
            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã đóng tin tuyển dụng" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy tin tuyển dụng" };
        }

        public async Task<PhanHoiModel> XoaAsync(int id, NguoiDungHienTai nguoi)
        {
            var soHuu = QuyenHelper.PhaiTonTai(await _soHuuRepo.LayThongTinTinAsync(id), "Không tìm thấy tin tuyển dụng");
            QuyenHelper.PhaiLaChuSoHuu(soHuu.NhaTuyenDungId, nguoi, choPhepAdmin: true);

            bool ok = await _repo.XoaAsync(id);
            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã xóa tin tuyển dụng" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy tin tuyển dụng" };
        }

        // T10: tin ChoDuyet/TuChoi chỉ chủ tin và Admin xem được. Người khác nhận null -> Controller trả 404
        // (trả 404 thay vì 403 để không lộ việc tin đó có tồn tại)
        public async Task<TinTuyenDungModel> LayChiTietAsync(int id, NguoiDungHienTai nguoi)
        {
            var soHuu = await _soHuuRepo.LayThongTinTinAsync(id);
            if (soHuu == null) return null;

            bool congKhai = soHuu.TrangThai == "DaDuyet" || soHuu.TrangThai == "DaDong";
            if (!congKhai)
            {
                bool duocXem = nguoi != null && (nguoi.LaAdmin || soHuu.NhaTuyenDungId == nguoi.Id);
                if (!duocXem) return null;
            }
            return await _repo.LayChiTietAsync(id);
        }

        // TinTuyenDungBusiness.cs: thay 2 hàm
        public async Task<PhanTrangModel<TinTimKiemModel>> TimKiemAsync(TimKiemTinModel model)
        {
            model.Trang = PhanTrangHelper.Trang(model.Trang);
            model.KichThuoc = PhanTrangHelper.KichThuoc(model.KichThuoc);
            return await _repo.TimKiemAsync(model);
        }

        public async Task<IEnumerable<TinGoiYModel>> GoiYAsync(int ungVienId)
        {
            int hoSoId = await _hoSoRepo.LayIdMoiNhatAsync(ungVienId);
            if (hoSoId == 0) return Enumerable.Empty<TinGoiYModel>();

            var hoSo = await _hoSoRepo.LayChiTietAsync(hoSoId);
            if (hoSo == null || hoSo.DanhSachKyNang.Count == 0) return Enumerable.Empty<TinGoiYModel>();

            var kyNangIds = hoSo.DanhSachKyNang.Select(k => k.Id).ToList();
            return await _repo.GoiYAsync(ungVienId, kyNangIds);
        }
        public async Task<PhanTrangModel<TinTuyenDungTomTatModel>> LayCuaToiAsync(int nhaTuyenDungId, string trangThai, int trang, int kichThuoc)
        {
            string loc = TrangThaiHopLe.ChuanHoa(trangThai, TrangThaiHopLe.Tin);
            return await _repo.LayTheoNhaTuyenDungAsync(nhaTuyenDungId, loc,
                PhanTrangHelper.Trang(trang), PhanTrangHelper.KichThuoc(kichThuoc));
        }

        public async Task<IEnumerable<TinTuyenDungTomTatModel>> LayChoDuyetAsync()
        {
            return await _repo.LayChoDuyetAsync();
        }
    }
}
