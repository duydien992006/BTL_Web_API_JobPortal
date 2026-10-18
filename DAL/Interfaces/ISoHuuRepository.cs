using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ISoHuuRepository
    {
        Task<ThongTinSoHuuModel> LayThongTinTinAsync(int tinId);
        Task<ThongTinSoHuuModel> LayThongTinDonAsync(int donId);
        Task<ThongTinSoHuuModel> LayThongTinLichAsync(int lichId);
        Task<ThongTinSoHuuModel> LayThongTinDeNghiAsync(int deNghiId);
        Task<bool> NhaTuyenDungCoQuyenXemHoSoAsync(int hoSoId, int nhaTuyenDungId);
    }
}
