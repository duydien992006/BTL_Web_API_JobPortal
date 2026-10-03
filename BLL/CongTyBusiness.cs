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

        public async Task<PhanHoiModel> TaoAsync(CongTyModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TenCongTy))
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Tên công ty không được để trống" };

            int id = await _repo.TaoAsync(model);
            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Tạo công ty thành công, chờ xác minh", DuLieu = id };
        }

        public async Task<PhanHoiModel> CapNhatAsync(CongTyModel model)
        {
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

        public async Task<CongTyModel> LayTheoIdAsync(int id) { return await _repo.LayTheoIdAsync(id); }
        public async Task<IEnumerable<CongTyModel>> LayTatCaAsync() { return await _repo.LayTatCaAsync(); }
    }
}
