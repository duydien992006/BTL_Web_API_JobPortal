using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface INguoiDungRepository
    {
        Task<int> DangKyAsync(NguoiDungModel model);
        Task<NguoiDungModel> LayTheoEmailAsync(string email);
        Task<NguoiDungModel> LayTheoIdAsync(int id);
        Task<bool> CapNhatAsync(CapNhatNguoiDungModel model);
        Task<PhanTrangModel<NguoiDungQuanLyModel>> LayDanhSachAsync(string tuKhoa, string vaiTro, int trang, int kichThuoc);
        Task<bool> DatKhoaAsync(int id, bool biKhoa, string lyDo);
        Task<bool> BiKhoaAsync(int id);
        Task<bool> DoiMatKhauAsync(int id, string matKhauMaHoa);
        Task<bool> TaoTokenDatLaiAsync(int nguoiDungId, string tokenHash, DateTime hetHan);
        Task<bool> DatLaiMatKhauAsync(string email, string tokenHash, string matKhauMaHoa);
    }
}
