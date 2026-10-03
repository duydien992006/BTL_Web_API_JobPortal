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
    }
}
