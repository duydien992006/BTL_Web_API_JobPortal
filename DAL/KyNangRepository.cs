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
    public class KyNangRepository : IKyNangRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        public KyNangRepository(IDatabaseHelper dbHelper) { _dbHelper = dbHelper; }

        public async Task<int> TaoAsync(string tenKyNang)
        {
            return await _dbHelper.ExecuteScalarAsync<int>("sp_kynang_tao", new { TenKyNang = tenKyNang });
        }

        public async Task<IEnumerable<KyNangModel>> LayTatCaAsync()
        {
            return await _dbHelper.QueryAsync<KyNangModel>("sp_kynang_layTatCa");
        }

        public async Task<IEnumerable<KyNangModel>> TimKiemAsync(string tuKhoa)
        {
            return await _dbHelper.QueryAsync<KyNangModel>("sp_kynang_timKiem", new { TuKhoa = tuKhoa });
        }
    }
}
