using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ITinTuyenDungRepository
    {
        Task<int> TaoAsync(TaoTinTuyenDungModel model);
        Task<bool> CapNhatAsync(int id, TaoTinTuyenDungModel model);
        Task<bool> DuyetAsync(int id, string trangThai);
        Task<bool> DongAsync(int id);
        Task<bool> XoaAsync(int id);
        Task<TinTuyenDungModel> LayChiTietAsync(int id);
        Task<PhanTrangModel<TinTimKiemModel>> TimKiemAsync(TimKiemTinModel model);
        Task<IEnumerable<TinGoiYModel>> GoiYAsync(int ungVienId, List<int> danhSachKyNangUngVien);
        Task<PhanTrangModel<TinTuyenDungTomTatModel>> LayTheoNhaTuyenDungAsync(int nhaTuyenDungId, string trangThai, int trang, int kichThuoc);
        Task<IEnumerable<TinTuyenDungTomTatModel>> LayChoDuyetAsync();
    }
}
