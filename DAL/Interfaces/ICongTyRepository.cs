using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ICongTyRepository
    {
        Task<int> TaoAsync(CongTyModel model);
        Task<bool> CapNhatAsync(CongTyModel model);
        Task<bool> XacMinhAsync(int id, bool daXacMinh);
        Task<CongTyModel> LayTheoIdAsync(int id);
        Task<IEnumerable<CongTyModel>> LayTatCaAsync();
        Task<bool> XoaAsync(int id);
    }
}
