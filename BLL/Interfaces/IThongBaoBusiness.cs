using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IThongBaoBusiness
    {
        Task TaoAsync(int nguoiDungId, string noiDung);
        Task<IEnumerable<ThongBaoModel>> LayTheoNguoiDungAsync(int nguoiDungId);
        Task<PhanHoiModel> DanhDauDaDocAsync(int id);
    }
}
