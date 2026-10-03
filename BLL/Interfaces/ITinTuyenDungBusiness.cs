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
        Task<PhanHoiModel> TaoAsync(TaoTinTuyenDungModel model);
        Task<PhanHoiModel> CapNhatAsync(int id, TaoTinTuyenDungModel model);
        Task<PhanHoiModel> DuyetAsync(int id, bool duyet);
        Task<PhanHoiModel> DongAsync(int id);
        Task<PhanHoiModel> XoaAsync(int id);
        Task<TinTuyenDungModel> LayChiTietAsync(int id);
        Task<IEnumerable<TinTuyenDungModel>> TimKiemAsync(TimKiemTinModel model);
        Task<IEnumerable<TinTuyenDungModel>> GoiYAsync(int ungVienId);
    }
}
