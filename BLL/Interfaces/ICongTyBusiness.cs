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
        Task<PhanHoiModel> TaoAsync(CongTyModel model);
        Task<PhanHoiModel> CapNhatAsync(CongTyModel model);
        Task<PhanHoiModel> XacMinhAsync(int id, bool daXacMinh);
        Task<CongTyModel> LayTheoIdAsync(int id);
        Task<IEnumerable<CongTyModel>> LayTatCaAsync();
    }
}
