using BLL.Interfaces;
using DAL.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BaoCaoBusiness : IBaoCaoBusiness
    {
        private readonly IBaoCaoRepository _repo;
        public BaoCaoBusiness(IBaoCaoRepository repo) { _repo = repo; }
        public async Task<BaoCaoTongQuanModel> LayTongQuanAsync() { return await _repo.LayTongQuanAsync(); }
        public async Task<PipelineAdminModel> LayPipelineAsync()
        {
            var kq = await _repo.LayPipelineAdminAsync();
            kq.TongDon = kq.DonTheoTrangThai.Sum(x => x.SoLuong);
            int trungTuyen = kq.DonTheoTrangThai.Where(x => x.Nhan == "TrungTuyen").Sum(x => x.SoLuong);
            kq.TyLeTrungTuyenPhanTram = kq.TongDon == 0 ? 0 : Math.Round(trungTuyen * 100.0 / kq.TongDon, 1);
            return kq;
        }

        public async Task<DashboardNhaTuyenDungModel> LayDashboardNhaTuyenDungAsync(int nhaTuyenDungId)
        {
            var kq = await _repo.LayDashboardNhaTuyenDungAsync(nhaTuyenDungId);
            kq.TongDon = kq.Pipeline.Sum(x => x.SoLuong);
            return kq;
        }
    }
}
