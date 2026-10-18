using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ILichPhongVanBusiness
    {
        Task<PhanHoiModel> TaoAsync(TaoLichPhongVanModel model, NguoiDungHienTai nguoi);
        Task<PhanHoiModel> CapNhatKetQuaAsync(int id, string ketQua, NguoiDungHienTai nguoi);
        Task<IEnumerable<LichPhongVanModel>> LayTheoDonUngTuyenAsync(int donUngTuyenId, NguoiDungHienTai nguoi);
        Task<IEnumerable<LichPhongVanCuaToiModel>> LayCuaToiAsync(NguoiDungHienTai nguoi);
    }
}
