using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IDeNghiTuyenDungBusiness
    {
        Task<PhanHoiModel> GuiAsync(GuiDeNghiModel model, NguoiDungHienTai nguoi);
        Task<PhanHoiModel> PhanHoiAsync(int id, bool chapNhan, NguoiDungHienTai nguoi);
        Task<IEnumerable<DeNghiTuyenDungModel>> LayTheoDonUngTuyenAsync(int donUngTuyenId, NguoiDungHienTai nguoi);
    }
}
