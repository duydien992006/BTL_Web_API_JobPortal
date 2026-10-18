using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
namespace BLL.Interfaces
{
    public interface INguoiDungBusiness
    {
        Task<PhanHoiModel> DangKyAsync(DangKyModel model);
        Task<PhanHoiModel> DangNhapAsync(DangNhapModel model);
        Task<PhanHoiModel> CapNhatAsync(CapNhatNguoiDungModel model);
        Task<HoSoCaNhanModel> LayThongTinCaNhanAsync(int id);
        Task<PhanTrangModel<NguoiDungQuanLyModel>> LayDanhSachAsync(string tuKhoa, string vaiTro, int trang, int kichThuoc);
        Task<PhanHoiModel> DatKhoaAsync(int id, bool khoa, string lyDo, NguoiDungHienTai nguoiThucHien);
    }
}
