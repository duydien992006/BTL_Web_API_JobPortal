using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IDonUngTuyenBusiness
    {
        Task<PhanHoiModel> NopDonAsync(NopDonModel model, int ungVienId);
        Task<PhanHoiModel> DoiTrangThaiAsync(DoiTrangThaiDonModel model, NguoiDungHienTai nguoi);
        Task<IEnumerable<dynamic>> LayTheoTinAsync(int tinId, NguoiDungHienTai nguoi);
        Task<IEnumerable<dynamic>> LayTheoUngVienAsync(int ungVienId);
        Task<ChiTietDonUngTuyenModel> LayChiTietAsync(int id, NguoiDungHienTai nguoi);
    }
}
