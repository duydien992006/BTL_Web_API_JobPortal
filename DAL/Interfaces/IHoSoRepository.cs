using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IHoSoRepository
    {
        Task<int> TaoAsync(TaoHoSoModel model);
        Task<HoSoModel> LayChiTietAsync(int id);
        Task<int> LayIdMoiNhatAsync(int ungVienId);
        Task<IEnumerable<HoSoModel>> LayDanhSachTheoUngVienAsync(int ungVienId);
        Task<bool> XoaAsync(int id, int ungVienId);
    }
}
