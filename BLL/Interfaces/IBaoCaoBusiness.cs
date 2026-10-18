using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IBaoCaoBusiness
    {
        Task<BaoCaoTongQuanModel> LayTongQuanAsync();
        Task<PipelineAdminModel> LayPipelineAsync();
        Task<DashboardNhaTuyenDungModel> LayDashboardNhaTuyenDungAsync(int nhaTuyenDungId);
    }
}
