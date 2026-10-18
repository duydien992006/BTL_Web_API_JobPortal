using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IBaoCaoRepository
    {
        Task<BaoCaoTongQuanModel> LayTongQuanAsync();
        Task<PipelineAdminModel> LayPipelineAdminAsync();
        Task<DashboardNhaTuyenDungModel> LayDashboardNhaTuyenDungAsync(int nhaTuyenDungId);
    }
}
