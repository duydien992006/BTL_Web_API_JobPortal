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
        Task<PhanTrangModel<DonTheoTinModel>> LayTheoTinAsync(int tinId, string trangThai, int trang, int kichThuoc, NguoiDungHienTai nguoi);
        Task<PhanTrangModel<DonUngTuyenCuaToiModel>> LayTheoUngVienAsync(int ungVienId, string trangThai, int trang, int kichThuoc);

        Task<ChiTietDonUngTuyenModel> LayChiTietAsync(int id, NguoiDungHienTai nguoi);
        Task<PhanHoiModel> RutDonAsync(int id, NguoiDungHienTai nguoi);
    }
}
