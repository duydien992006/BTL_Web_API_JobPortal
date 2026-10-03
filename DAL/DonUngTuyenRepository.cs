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
    public class DonUngTuyenRepository : IDonUngTuyenRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        public DonUngTuyenRepository(IDatabaseHelper dbHelper) { _dbHelper = dbHelper; }

        public async Task<bool> DaTonTaiAsync(int tinId, int ungVienId)
        {
            int soLuong = await _dbHelper.ExecuteScalarAsync<int>("sp_donungtuyen_kiemTraDaTonTai",
                new { TinTuyenDungId = tinId, UngVienId = ungVienId });
            return soLuong > 0;
        }

        public async Task<int> NopDonAsync(int tinId, int hoSoId, int ungVienId)
        {
            return await _dbHelper.ExecuteScalarAsync<int>("sp_donungtuyen_nopDon",
                new { TinTuyenDungId = tinId, HoSoId = hoSoId, UngVienId = ungVienId });
        }

        public async Task<bool> DoiTrangThaiAsync(int id, string trangThai)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_donungtuyen_doiTrangThai", new { Id = id, TrangThai = trangThai });
            return soDong > 0;
        }

        public async Task<DonUngTuyenModel> LayTheoIdAsync(int id)
        {
            return await _dbHelper.QueryFirstOrDefaultAsync<DonUngTuyenModel>("sp_donungtuyen_layTheoId", new { Id = id });
        }

        public async Task<IEnumerable<dynamic>> LayTheoTinAsync(int tinId)
        {
            return await _dbHelper.QueryAsync<dynamic>("sp_donungtuyen_layTheoTin", new { TinTuyenDungId = tinId });
        }

        public async Task<IEnumerable<dynamic>> LayTheoUngVienAsync(int ungVienId)
        {
            return await _dbHelper.QueryAsync<dynamic>("sp_donungtuyen_layTheoUngVien", new { UngVienId = ungVienId });
        }
    }
}
