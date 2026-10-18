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
    public class DeNghiTuyenDungBusiness : IDeNghiTuyenDungBusiness
    {
        private readonly IDeNghiTuyenDungRepository _repo;
        private readonly ISoHuuRepository _soHuuRepo;

        public DeNghiTuyenDungBusiness(IDeNghiTuyenDungRepository repo, ISoHuuRepository soHuuRepo)
        {
            _repo = repo;
            _soHuuRepo = soHuuRepo;
        }

        public async Task<PhanHoiModel> GuiAsync(GuiDeNghiModel model, NguoiDungHienTai nguoi)
        {
            // Chỉ nhà tuyển dụng của đơn này mới được gửi offer
            var don = QuyenHelper.PhaiTonTai(await _soHuuRepo.LayThongTinDonAsync(model.DonUngTuyenId), "Không tìm thấy đơn ứng tuyển");
            QuyenHelper.PhaiLaChuSoHuu(don.NhaTuyenDungId, nguoi);

            if (model.MucLuongDeNghi <= 0)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Mức lương đề nghị không hợp lệ" };

            var deNghiModel = new DeNghiTuyenDungModel
            {
                DonUngTuyenId = model.DonUngTuyenId,
                MucLuongDeNghi = model.MucLuongDeNghi,
                NgayBatDauLamViec = model.NgayBatDauLamViec
            };

            int id = await _repo.GuiAsync(deNghiModel);
            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã gửi đề nghị tuyển dụng", DuLieu = id };
        }

        public async Task<PhanHoiModel> PhanHoiAsync(int id, bool chapNhan, NguoiDungHienTai nguoi)
        {
            // T7: chỉ ứng viên nhận offer này mới được phản hồi
            var deNghi = QuyenHelper.PhaiTonTai(await _soHuuRepo.LayThongTinDeNghiAsync(id), "Không tìm thấy đề nghị tuyển dụng");
            QuyenHelper.PhaiLaChuSoHuu(deNghi.UngVienId, nguoi);
            // MỚI: mỗi đề nghị chỉ được phản hồi 1 lần
            if (deNghi.TrangThai != "DaGui")
                throw new XungDotDuLieuException("Đề nghị này đã được phản hồi, không thể đổi ý");

            string trangThai = chapNhan ? "DaChapNhan" : "DaTuChoi";
            bool ok = await _repo.PhanHoiAsync(id, trangThai);
            if (ok)
            {
                string thongDiep = chapNhan ? "Đã chấp nhận đề nghị" : "Đã từ chối đề nghị";
                return new PhanHoiModel { ThanhCong = true, ThongDiep = thongDiep };
            }
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy đề nghị tuyển dụng" };
        }

        public async Task<IEnumerable<DeNghiTuyenDungModel>> LayTheoDonUngTuyenAsync(int donUngTuyenId, NguoiDungHienTai nguoi)
        {
            var don = QuyenHelper.PhaiTonTai(await _soHuuRepo.LayThongTinDonAsync(donUngTuyenId), "Không tìm thấy đơn ứng tuyển");
            QuyenHelper.PhaiLaBenLienQuan(don, nguoi);

            return await _repo.LayTheoDonUngTuyenAsync(donUngTuyenId);
        }
    }
}
