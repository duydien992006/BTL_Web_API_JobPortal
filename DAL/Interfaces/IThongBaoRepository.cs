using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IThongBaoRepository
    {
        Task<int> TaoAsync(int nguoiDungId, string noiDung);
        Task<IEnumerable<ThongBaoModel>> LayTheoNguoiDungAsync(int nguoiDungId);
        Task<bool> DanhDauDaDocAsync(int id);
    }
}
