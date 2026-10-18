using DAL.Helper.Interfaces;
using DAL.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class BaoCaoRepository : IBaoCaoRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        public BaoCaoRepository(IDatabaseHelper dbHelper) { _dbHelper = dbHelper; }

        public async Task<BaoCaoTongQuanModel> LayTongQuanAsync()
        {
            return await _dbHelper.QueryFirstOrDefaultAsync<BaoCaoTongQuanModel>("sp_baocao_tongQuan");
        }
        public async Task<PipelineAdminModel> LayPipelineAdminAsync()
        {
            return await _dbHelper.QueryMultipleAsync<PipelineAdminModel>("sp_baocao_pipeline", null, async multi =>
            {
                var kq = new PipelineAdminModel();
                kq.DonTheoTrangThai = (await multi.ReadAsync<MucThongKeModel>()).ToList();
                kq.TinTheoTrangThai = (await multi.ReadAsync<MucThongKeModel>()).ToList();
                kq.NguoiDungTheoVaiTro = (await multi.ReadAsync<MucThongKeModel>()).ToList();
                kq.DonTheoNgay = (await multi.ReadAsync<SoDonTheoNgayModel>()).ToList();
                kq.TopTin = (await multi.ReadAsync<TopTinModel>()).ToList();
                return kq;
            });
        }

        public async Task<DashboardNhaTuyenDungModel> LayDashboardNhaTuyenDungAsync(int nhaTuyenDungId)
        {
            return await _dbHelper.QueryMultipleAsync<DashboardNhaTuyenDungModel>("sp_dashboard_nhatuyendung",
                new { NhaTuyenDungId = nhaTuyenDungId }, async multi =>
                {
                    var kq = new DashboardNhaTuyenDungModel();
                    kq.Pipeline = (await multi.ReadAsync<MucThongKeModel>()).ToList();
                    kq.TheoTin = (await multi.ReadAsync<DongTinPipelineModel>()).ToList();
                    kq.LichSapToi = (await multi.ReadAsync<LichSapToiModel>()).ToList();
                    kq.ViecCanLam = await multi.ReadFirstOrDefaultAsync<ViecCanLamModel>() ?? new ViecCanLamModel();
                    return kq;
                });
        }
    }
}
