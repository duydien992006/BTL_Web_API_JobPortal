using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IMatKhauBusiness
    {
        Task<PhanHoiModel> DoiMatKhauAsync(int nguoiDungId, DoiMatKhauModel model);
        Task<PhanHoiModel> QuenMatKhauAsync(QuenMatKhauModel model);
        Task<PhanHoiModel> DatLaiMatKhauAsync(DatLaiMatKhauModel model);
    }
}
