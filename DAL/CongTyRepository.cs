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
    public class CongTyRepository : ICongTyRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        public CongTyRepository(IDatabaseHelper dbHelper) { _dbHelper = dbHelper; }

        public async Task<int> TaoAsync(CongTyModel model)
        {
            return await _dbHelper.ExecuteScalarAsync<int>("sp_congty_tao", new
            {
                model.TenCongTy,
                model.MoTa,
                model.LogoUrl,
                model.DiaChi,
                model.NguoiDaiDienId
            });
        }

        public async Task<bool> CapNhatAsync(CongTyModel model)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_congty_capNhat", new
            {
                model.Id,
                model.TenCongTy,
                model.MoTa,
                model.LogoUrl,
                model.DiaChi
            });
            return soDong > 0;
        }

        public async Task<bool> XacMinhAsync(int id, bool daXacMinh)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_congty_xacMinh", new { Id = id, DaXacMinh = daXacMinh });
            return soDong > 0;
        }

        public async Task<CongTyModel> LayTheoIdAsync(int id)
        {
            return await _dbHelper.QueryFirstOrDefaultAsync<CongTyModel>("sp_congty_layTheoId", new { Id = id });
        }

        public async Task<IEnumerable<CongTyModel>> LayTatCaAsync()
        {
            return await _dbHelper.QueryAsync<CongTyModel>("sp_congty_layTatCa");
        }

        public async Task<bool> XoaAsync(int id)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_congty_xoa", new { Id = id });
            return soDong > 0;
        }
    }
}
