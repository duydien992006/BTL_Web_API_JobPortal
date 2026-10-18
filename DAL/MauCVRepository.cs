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
    public class MauCVRepository : IMauCVRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        public MauCVRepository(IDatabaseHelper dbHelper) { _dbHelper = dbHelper; }

        public async Task<int> TaoAsync(MauCVModel model, int nguoiTaoId)
        {
            return await _dbHelper.ExecuteScalarAsync<int>("sp_maucv_tao", new
            {
                model.TenMau,
                model.MoTa,
                model.TenFileGoc,
                model.DuongDanFile,
                NguoiTaoId = nguoiTaoId
            });
        }

        public async Task<IEnumerable<MauCVModel>> LayTatCaAsync()
        {
            return await _dbHelper.QueryAsync<MauCVModel>("sp_maucv_layTatCa");
        }

        public async Task<MauCVModel> LayTheoIdAsync(int id)
        {
            return await _dbHelper.QueryFirstOrDefaultAsync<MauCVModel>("sp_maucv_layTheoId", new { Id = id });
        }

        public async Task<bool> XoaAsync(int id)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_maucv_xoa", new { Id = id });
            return soDong > 0;
        }
    }
}
