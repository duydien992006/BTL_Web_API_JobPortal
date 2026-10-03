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
        Task<IEnumerable<dynamic>> LayTheoTinAsync(int tinId);
        Task<IEnumerable<dynamic>> LayTheoUngVienAsync(int ungVienId);
    }
}
