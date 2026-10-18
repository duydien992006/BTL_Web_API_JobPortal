using Microsoft.AspNetCore.Http;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IMauCVBusiness
    {
        Task<PhanHoiModel> TaoAsync(IFormFile file, string tenMau, string moTa, int nguoiTaoId);
        Task<IEnumerable<MauCVModel>> LayTatCaAsync();
        Task<FileTaiVeModel> LayFileTaiVeAsync(int id);
        Task<PhanHoiModel> XoaAsync(int id);
    }
}
