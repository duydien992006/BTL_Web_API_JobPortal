using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IKyNangRepository
    {
        Task<int> TaoAsync(string tenKyNang);
        Task<IEnumerable<KyNangModel>> LayTatCaAsync();
        Task<IEnumerable<KyNangModel>> TimKiemAsync(string tuKhoa);
    }
}
