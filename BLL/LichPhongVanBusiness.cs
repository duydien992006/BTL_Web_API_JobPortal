using BLL.Interfaces;
using Common.Exceptions;
using DAL.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class LichPhongVanBusiness : ILichPhongVanBusiness
    {
        private readonly ILichPhongVanRepository _repo;
        private readonly ISoHuuRepository _soHuuRepo;

        // Chỉ chấp nhận 2 giá trị này, không cho gửi chuỗi bất kỳ hay quay lại "ChoKetQua"
        private static readonly string[] KetQuaHopLe = { "Dat", "KhongDat" };

        public LichPhongVanBusiness(ILichPhongVanRepository repo, ISoHuuRepository soHuuRepo)
        {
            _repo = repo;
            _soHuuRepo = soHuuRepo;
        }

        public async Task<PhanHoiModel> TaoAsync(TaoLichPhongVanModel model, NguoiDungHienTai nguoi)
        {
            var don = QuyenHelper.PhaiTonTai(await _soHuuRepo.LayThongTinDonAsync(model.DonUngTuyenId), "Không tìm thấy đơn ứng tuyển");
            QuyenHelper.PhaiLaChuSoHuu(don.NhaTuyenDungId, nguoi);

            if (model.ThoiGianPhongVan <= DateTime.UtcNow)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Thời gian phỏng vấn phải ở tương lai" };

            var lichModel = new LichPhongVanModel
            {
                DonUngTuyenId = model.DonUngTuyenId,
                ThoiGianPhongVan = model.ThoiGianPhongVan,
                DiaDiemPhongVan = model.DiaDiemPhongVan
            };

            // SP sp_lichphongvan_tao tự kiểm tra trạng thái đơn, vi phạm sẽ trả 409
            int id = await _repo.TaoAsync(lichModel);
            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã tạo lịch phỏng vấn", DuLieu = id };
        }

        public async Task<PhanHoiModel> CapNhatKetQuaAsync(int id, string ketQua, NguoiDungHienTai nguoi)
        {
            var lich = QuyenHelper.PhaiTonTai(await _soHuuRepo.LayThongTinLichAsync(id), "Không tìm thấy lịch phỏng vấn");
            QuyenHelper.PhaiLaChuSoHuu(lich.NhaTuyenDungId, nguoi);

            if (!KetQuaHopLe.Contains(ketQua))
                throw new NghiepVuException("Kết quả chỉ nhận 'Dat' hoặc 'KhongDat'");

            // sp_sohuu_lichPhongVan trả cột KetQua dưới tên TrangThai
            if (lich.TrangThai != "ChoKetQua")
                throw new XungDotDuLieuException("Lịch phỏng vấn này đã có kết quả, không thể sửa lại");

            bool ok = await _repo.CapNhatKetQuaAsync(id, ketQua);
            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Cập nhật kết quả thành công" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy lịch phỏng vấn" };
        }

        public async Task<IEnumerable<LichPhongVanModel>> LayTheoDonUngTuyenAsync(int donUngTuyenId, NguoiDungHienTai nguoi)
        {
            var don = QuyenHelper.PhaiTonTai(await _soHuuRepo.LayThongTinDonAsync(donUngTuyenId), "Không tìm thấy đơn ứng tuyển");
            QuyenHelper.PhaiLaBenLienQuan(don, nguoi);

            return await _repo.LayTheoDonUngTuyenAsync(donUngTuyenId);
        }

        // Ứng viên: lịch của mình. Nhà tuyển dụng: lịch của các đơn thuộc công ty mình
        public async Task<IEnumerable<LichPhongVanCuaToiModel>> LayCuaToiAsync(NguoiDungHienTai nguoi)
        {
            return await _repo.LayCuaToiAsync(nguoi.Id, nguoi.VaiTro);
        }
    }
}
