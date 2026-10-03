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
    public class ThongBaoBusiness : IThongBaoBusiness
    {
        private readonly IThongBaoRepository _repo;
        public ThongBaoBusiness(IThongBaoRepository repo) { _repo = repo; }

        public async Task TaoAsync(int nguoiDungId, string noiDung) { await _repo.TaoAsync(nguoiDungId, noiDung); }

        public async Task<IEnumerable<ThongBaoModel>> LayTheoNguoiDungAsync(int nguoiDungId)
        {
            return await _repo.LayTheoNguoiDungAsync(nguoiDungId);
        }

        public async Task<PhanHoiModel> DanhDauDaDocAsync(int id)
        {
            bool ok = await _repo.DanhDauDaDocAsync(id);
            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã đánh dấu đã đọc" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy thông báo" };
        }
    }
}
