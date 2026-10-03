using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IDeNghiTuyenDungRepository
    {
        Task<int> GuiAsync(DeNghiTuyenDungModel model);
        Task<bool> PhanHoiAsync(int id, string trangThai);
        Task<IEnumerable<DeNghiTuyenDungModel>> LayTheoDonUngTuyenAsync(int donUngTuyenId);
    }
}
