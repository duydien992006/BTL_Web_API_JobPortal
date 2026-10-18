using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICongTyBusiness
    {
        Task<PhanHoiModel> TaoAsync(CongTyModel model, NguoiDungHienTai nguoi);
        Task<PhanHoiModel> CapNhatAsync(CongTyModel model, NguoiDungHienTai nguoi);
        Task<PhanHoiModel> XacMinhAsync(int id, bool daXacMinh);
        Task<PhanHoiModel> XoaAsync(int id, NguoiDungHienTai nguoi);
        Task<CongTyModel> LayTheoIdAsync(int id);
        Task<PhanTrangModel<CongTyModel>> LayTatCaAsync(int trang, int kichThuoc);
        Task<IEnumerable<CongTyModel>> LayCuaToiAsync(int nguoiDungId);
        Task<IEnumerable<CongTyModel>> LayChoXacMinhAsync();
    }
}
