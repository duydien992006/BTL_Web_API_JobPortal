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

        public TinTuyenDungBusiness(ITinTuyenDungRepository repo, IHoSoRepository hoSoRepo)
        {
            _repo = repo;
            _hoSoRepo = hoSoRepo;
        }

        public async Task<PhanHoiModel> TaoAsync(TaoTinTuyenDungModel model)
        {
            if (model.DanhSachKyNangId == null || model.DanhSachKyNangId.Count == 0)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Phải chọn ít nhất 1 kỹ năng yêu cầu" };

            if (model.MucLuongTu > model.MucLuongDen)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Mức lương không hợp lệ" };

            if (model.NgayHetHan <= DateTime.UtcNow)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Ngày hết hạn phải lớn hơn hiện tại" };

            int id = await _repo.TaoAsync(model);
            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Tạo tin thành công, đang chờ duyệt", DuLieu = id };
        }

        public async Task<PhanHoiModel> CapNhatAsync(int id, TaoTinTuyenDungModel model)
        {
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

        public async Task<PhanHoiModel> DongAsync(int id)
        {
            bool ok = await _repo.DongAsync(id);
            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã đóng tin tuyển dụng" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy tin tuyển dụng" };
        }

        public async Task<PhanHoiModel> XoaAsync(int id)
        {
            bool ok = await _repo.XoaAsync(id);
            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã xóa tin tuyển dụng" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy tin tuyển dụng" };
        }

        public async Task<TinTuyenDungModel> LayChiTietAsync(int id) { return await _repo.LayChiTietAsync(id); }

        public async Task<IEnumerable<TinTuyenDungModel>> TimKiemAsync(TimKiemTinModel model)
        {
            return await _repo.TimKiemAsync(model);
        }

        public async Task<IEnumerable<TinTuyenDungModel>> GoiYAsync(int ungVienId)
        {
            int hoSoId = await _hoSoRepo.LayIdMoiNhatAsync(ungVienId);
            if (hoSoId == 0) return Enumerable.Empty<TinTuyenDungModel>();

            var hoSo = await _hoSoRepo.LayChiTietAsync(hoSoId);
            if (hoSo == null || hoSo.DanhSachKyNang.Count == 0) return Enumerable.Empty<TinTuyenDungModel>();

            var kyNangIds = hoSo.DanhSachKyNang.Select(k => k.Id).ToList();
            return await _repo.GoiYAsync(kyNangIds);
        }
    }
}
