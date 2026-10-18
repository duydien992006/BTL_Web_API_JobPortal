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
    public class SoHuuRepository : ISoHuuRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        public SoHuuRepository(IDatabaseHelper dbHelper) { _dbHelper = dbHelper; }

        public async Task<ThongTinSoHuuModel> LayThongTinTinAsync(int tinId)
        {
            return await _dbHelper.QueryFirstOrDefaultAsync<ThongTinSoHuuModel>("sp_sohuu_tinTuyenDung", new { Id = tinId });
        }

        public async Task<ThongTinSoHuuModel> LayThongTinDonAsync(int donId)
        {
            return await _dbHelper.QueryFirstOrDefaultAsync<ThongTinSoHuuModel>("sp_sohuu_donUngTuyen", new { Id = donId });
        }

        public async Task<ThongTinSoHuuModel> LayThongTinLichAsync(int lichId)
        {
            return await _dbHelper.QueryFirstOrDefaultAsync<ThongTinSoHuuModel>("sp_sohuu_lichPhongVan", new { Id = lichId });
        }

        public async Task<ThongTinSoHuuModel> LayThongTinDeNghiAsync(int deNghiId)
        {
            return await _dbHelper.QueryFirstOrDefaultAsync<ThongTinSoHuuModel>("sp_sohuu_deNghi", new { Id = deNghiId });
        }

        public async Task<bool> NhaTuyenDungCoQuyenXemHoSoAsync(int hoSoId, int nhaTuyenDungId)
        {
            int soLuong = await _dbHelper.ExecuteScalarAsync<int>("sp_sohuu_hoSoChoNhaTuyenDung",
                new { HoSoId = hoSoId, NhaTuyenDungId = nhaTuyenDungId });
            return soLuong > 0;
        }
    }
}
