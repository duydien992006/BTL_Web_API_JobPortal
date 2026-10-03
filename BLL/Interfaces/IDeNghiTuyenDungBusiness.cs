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
        Task<PhanHoiModel> GuiAsync(DeNghiTuyenDungModel model);
        Task<PhanHoiModel> PhanHoiAsync(int id, bool chapNhan);
        Task<IEnumerable<DeNghiTuyenDungModel>> LayTheoDonUngTuyenAsync(int donUngTuyenId);
    }
}
