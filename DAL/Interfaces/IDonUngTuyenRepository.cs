using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IDonUngTuyenRepository
    {
        Task<bool> DaTonTaiAsync(int tinId, int ungVienId);
        Task<int> NopDonAsync(int tinId, int hoSoId, int ungVienId);
        Task<bool> DoiTrangThaiAsync(int id, string trangThai);
        Task<DonUngTuyenModel> LayTheoIdAsync(int id);
        Task<PhanTrangModel<DonTheoTinModel>> LayTheoTinAsync(int tinId, string trangThai, int trang, int kichThuoc);
        Task<PhanTrangModel<DonUngTuyenCuaToiModel>> LayTheoUngVienAsync(int ungVienId, string trangThai, int trang, int kichThuoc);
        Task<DonUngTuyenChiTietModel> LayChiTietAsync(int id);
        Task<bool> RutAsync(int id, int ungVienId);
        Task<List<LichSuTrangThaiModel>> LayLichSuAsync(int donUngTuyenId);
    }
}
