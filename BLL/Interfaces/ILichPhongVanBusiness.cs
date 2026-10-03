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
        Task<PhanHoiModel> TaoAsync(LichPhongVanModel model);
        Task<PhanHoiModel> CapNhatKetQuaAsync(int id, string ketQua);
        Task<IEnumerable<LichPhongVanModel>> LayTheoDonUngTuyenAsync(int donUngTuyenId);
    }
}
