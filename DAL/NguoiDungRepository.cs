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
    public class NguoiDungRepository : INguoiDungRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        public NguoiDungRepository(IDatabaseHelper dbHelper) { _dbHelper = dbHelper; }

        public async Task<int> DangKyAsync(NguoiDungModel model)
        {
            return await _dbHelper.ExecuteScalarAsync<int>("sp_nguoidung_dangky", new
            {
                model.Email,
                model.MatKhauMaHoa,
                model.VaiTro,
                model.HoTen,
                model.SoDienThoai
            });
        }

        public async Task<NguoiDungModel> LayTheoEmailAsync(string email)
        {
            return await _dbHelper.QueryFirstOrDefaultAsync<NguoiDungModel>("sp_nguoidung_layTheoEmail", new { Email = email });
        }

        public async Task<NguoiDungModel> LayTheoIdAsync(int id)
        {
            return await _dbHelper.QueryFirstOrDefaultAsync<NguoiDungModel>("sp_nguoidung_layTheoId", new { Id = id });
        }

        public async Task<bool> CapNhatAsync(CapNhatNguoiDungModel model)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_nguoidung_capNhat", new { model.Id, model.HoTen, model.SoDienThoai });
            return soDong > 0;
        }
    }
}
