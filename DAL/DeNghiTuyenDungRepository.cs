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
    public class DeNghiTuyenDungRepository : IDeNghiTuyenDungRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        public DeNghiTuyenDungRepository(IDatabaseHelper dbHelper) { _dbHelper = dbHelper; }

        public async Task<int> GuiAsync(DeNghiTuyenDungModel model)
        {
            return await _dbHelper.ExecuteScalarAsync<int>("sp_denghi_gui", new
            {
                model.DonUngTuyenId,
                model.MucLuongDeNghi,
                model.NgayBatDauLamViec
            });
        }

        public async Task<bool> PhanHoiAsync(int id, string trangThai)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_denghi_phanHoi", new { Id = id, TrangThai = trangThai });
            return soDong > 0;
        }

        public async Task<IEnumerable<DeNghiTuyenDungModel>> LayTheoDonUngTuyenAsync(int donUngTuyenId)
        {
            return await _dbHelper.QueryAsync<DeNghiTuyenDungModel>("sp_denghi_layTheoDonUngTuyen", new { DonUngTuyenId = donUngTuyenId });
        }
    }
}
