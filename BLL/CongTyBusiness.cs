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
    public class CongTyBusiness : ICongTyBusiness
    {
        private readonly ICongTyRepository _repo;
        public CongTyBusiness(ICongTyRepository repo) { _repo = repo; }

        public async Task<PhanHoiModel> TaoAsync(CongTyModel model, NguoiDungHienTai nguoi)
        {
            if (string.IsNullOrWhiteSpace(model.TenCongTy))
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Tên công ty không được để trống" };

            // Người đại diện LUÔN là người đang đăng nhập, bỏ qua giá trị client gửi trong Body
            model.NguoiDaiDienId = nguoi.Id;

            int id = await _repo.TaoAsync(model);
            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Tạo công ty thành công, chờ xác minh", DuLieu = id };
        }

        public async Task<PhanHoiModel> CapNhatAsync(CongTyModel model, NguoiDungHienTai nguoi)
        {
            var congTy = QuyenHelper.PhaiTonTai(await _repo.LayTheoIdAsync(model.Id), "Không tìm thấy công ty");
            QuyenHelper.PhaiLaChuSoHuu(congTy.NguoiDaiDienId, nguoi);

            bool ok = await _repo.CapNhatAsync(model);
            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Cập nhật thành công" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy công ty" };
        }

        public async Task<PhanHoiModel> XacMinhAsync(int id, bool daXacMinh)
        {
            bool ok = await _repo.XacMinhAsync(id, daXacMinh);
            if (ok)
            {
                string thongDiep = daXacMinh ? "Đã xác minh công ty" : "Đã hủy xác minh";
                return new PhanHoiModel { ThanhCong = true, ThongDiep = thongDiep };
            }
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy công ty" };
        }

        public async Task<PhanHoiModel> XoaAsync(int id, NguoiDungHienTai nguoi)
        {
            var congTy = QuyenHelper.PhaiTonTai(await _repo.LayTheoIdAsync(id), "Không tìm thấy công ty");
            QuyenHelper.PhaiLaChuSoHuu(congTy.NguoiDaiDienId, nguoi, choPhepAdmin: true);

            bool ok = await _repo.XoaAsync(id);
            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã xóa công ty" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy công ty" };
        }

        public async Task<CongTyModel> LayTheoIdAsync(int id) { return await _repo.LayTheoIdAsync(id); }
        public async Task<PhanTrangModel<CongTyModel>> LayTatCaAsync(int trang, int kichThuoc)
        {
            return await _repo.LayTrangAsync(PhanTrangHelper.Trang(trang), PhanTrangHelper.KichThuoc(kichThuoc));
        }
        public async Task<IEnumerable<CongTyModel>> LayCuaToiAsync(int nguoiDungId)
        {
            return await _repo.LayTheoNguoiDaiDienAsync(nguoiDungId);
        }

        public async Task<IEnumerable<CongTyModel>> LayChoXacMinhAsync()
        {
            return await _repo.LayChoXacMinhAsync();
        }
    }
}
