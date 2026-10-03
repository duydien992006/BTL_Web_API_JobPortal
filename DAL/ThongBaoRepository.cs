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
    public class ThongBaoRepository : IThongBaoRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        public ThongBaoRepository(IDatabaseHelper dbHelper) { _dbHelper = dbHelper; }

        public async Task<int> TaoAsync(int nguoiDungId, string noiDung)
        {
            return await _dbHelper.ExecuteScalarAsync<int>("sp_thongbao_tao", new { NguoiDungId = nguoiDungId, NoiDung = noiDung });
        }

        public async Task<IEnumerable<ThongBaoModel>> LayTheoNguoiDungAsync(int nguoiDungId)
        {
            return await _dbHelper.QueryAsync<ThongBaoModel>("sp_thongbao_layTheoNguoiDung", new { NguoiDungId = nguoiDungId });
        }

        public async Task<bool> DanhDauDaDocAsync(int id)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_thongbao_danhDauDaDoc", new { Id = id });
            return soDong > 0;
        }
    }
}
