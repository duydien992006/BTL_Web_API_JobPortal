using Microsoft.AspNetCore.Http;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IHoSoBusiness
    {
        Task<PhanHoiModel> TaoAsync(IFormFile file, int ungVienId, int soNamKinhNghiem, List<int> danhSachKyNangId);
        Task<HoSoModel> LayChiTietAsync(int id);
        Task<IEnumerable<HoSoModel>> LayDanhSachTheoUngVienAsync(int ungVienId);
        Task<PhanHoiModel> XoaAsync(int id, int ungVienId);
    }
}
