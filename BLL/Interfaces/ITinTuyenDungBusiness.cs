using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ITinTuyenDungBusiness
    {
        Task<PhanHoiModel> TaoAsync(TaoTinTuyenDungModel model, NguoiDungHienTai nguoi);
        Task<PhanHoiModel> CapNhatAsync(int id, TaoTinTuyenDungModel model, NguoiDungHienTai nguoi);
        Task<PhanHoiModel> DuyetAsync(int id, bool duyet);
        Task<PhanHoiModel> DongAsync(int id, NguoiDungHienTai nguoi);
        Task<PhanHoiModel> XoaAsync(int id, NguoiDungHienTai nguoi);
        Task<TinTuyenDungModel> LayChiTietAsync(int id, NguoiDungHienTai nguoi);   // nguoi = null nếu chưa đăng nhập
        Task<IEnumerable<TinTuyenDungModel>> TimKiemAsync(TimKiemTinModel model);
        Task<IEnumerable<TinTuyenDungModel>> GoiYAsync(int ungVienId);
        Task<IEnumerable<TinTuyenDungTomTatModel>> LayCuaToiAsync(int nhaTuyenDungId);
        Task<IEnumerable<TinTuyenDungTomTatModel>> LayChoDuyetAsync();

    }
}
