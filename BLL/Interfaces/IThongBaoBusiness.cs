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
        Task<PhanTrangThongBaoModel> LayTheoNguoiDungAsync(int nguoiDungId, int trang, int kichThuoc);
        Task<PhanHoiModel> DanhDauDaDocAsync(int id, int nguoiDungId);
    }
}
