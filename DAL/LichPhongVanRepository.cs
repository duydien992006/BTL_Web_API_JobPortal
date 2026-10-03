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
    public class LichPhongVanRepository : ILichPhongVanRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        public LichPhongVanRepository(IDatabaseHelper dbHelper) { _dbHelper = dbHelper; }

        public async Task<int> TaoAsync(LichPhongVanModel model)
        {
            return await _dbHelper.ExecuteScalarAsync<int>("sp_lichphongvan_tao", new
            {
                model.DonUngTuyenId,
                model.ThoiGianPhongVan,
                model.DiaDiemPhongVan
            });
        }

        public async Task<bool> CapNhatKetQuaAsync(int id, string ketQua)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_lichphongvan_capNhatKetQua", new { Id = id, KetQua = ketQua });
            return soDong > 0;
        }

        public async Task<IEnumerable<LichPhongVanModel>> LayTheoDonUngTuyenAsync(int donUngTuyenId)
        {
            return await _dbHelper.QueryAsync<LichPhongVanModel>("sp_lichphongvan_layTheoDonUngTuyen", new { DonUngTuyenId = donUngTuyenId });
        }
    }
}
