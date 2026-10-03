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
    public class LichPhongVanBusiness : ILichPhongVanBusiness
    {
        private readonly ILichPhongVanRepository _repo;
        public LichPhongVanBusiness(ILichPhongVanRepository repo) { _repo = repo; }

        public async Task<PhanHoiModel> TaoAsync(LichPhongVanModel model)
        {
            if (model.ThoiGianPhongVan <= DateTime.UtcNow)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Thời gian phỏng vấn phải ở tương lai" };

            int id = await _repo.TaoAsync(model);
            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã tạo lịch phỏng vấn", DuLieu = id };
        }

        public async Task<PhanHoiModel> CapNhatKetQuaAsync(int id, string ketQua)
        {
            bool ok = await _repo.CapNhatKetQuaAsync(id, ketQua);
            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Cập nhật kết quả thành công" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy lịch phỏng vấn" };
        }

        public async Task<IEnumerable<LichPhongVanModel>> LayTheoDonUngTuyenAsync(int donUngTuyenId)
        {
            return await _repo.LayTheoDonUngTuyenAsync(donUngTuyenId);
        }
    }
}
