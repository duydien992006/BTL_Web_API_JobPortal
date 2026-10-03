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
    public class DeNghiTuyenDungBusiness : IDeNghiTuyenDungBusiness
    {
        private readonly IDeNghiTuyenDungRepository _repo;
        public DeNghiTuyenDungBusiness(IDeNghiTuyenDungRepository repo) { _repo = repo; }

        public async Task<PhanHoiModel> GuiAsync(DeNghiTuyenDungModel model)
        {
            if (model.MucLuongDeNghi <= 0)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Mức lương đề nghị không hợp lệ" };

            int id = await _repo.GuiAsync(model);
            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã gửi đề nghị tuyển dụng", DuLieu = id };
        }

        public async Task<PhanHoiModel> PhanHoiAsync(int id, bool chapNhan)
        {
            string trangThai = chapNhan ? "DaChapNhan" : "DaTuChoi";
            bool ok = await _repo.PhanHoiAsync(id, trangThai);
            if (ok)
            {
                string thongDiep = chapNhan ? "Đã chấp nhận đề nghị" : "Đã từ chối đề nghị";
                return new PhanHoiModel { ThanhCong = true, ThongDiep = thongDiep };
            }
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy đề nghị tuyển dụng" };
        }

        public async Task<IEnumerable<DeNghiTuyenDungModel>> LayTheoDonUngTuyenAsync(int donUngTuyenId)
        {
            return await _repo.LayTheoDonUngTuyenAsync(donUngTuyenId);
        }
    }
}
