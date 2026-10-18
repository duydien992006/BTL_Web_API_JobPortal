using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IMauCVRepository
    {
        Task<int> TaoAsync(MauCVModel model, int nguoiTaoId);
        Task<IEnumerable<MauCVModel>> LayTatCaAsync();
        Task<MauCVModel> LayTheoIdAsync(int id);
        Task<bool> XoaAsync(int id);
    }
}
