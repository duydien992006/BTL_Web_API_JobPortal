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
        public async Task<PhanTrangModel<NguoiDungQuanLyModel>> LayDanhSachAsync(string tuKhoa, string vaiTro, int trang, int kichThuoc)
        {
            var kq = await _dbHelper.QueryMultipleAsync<int, NguoiDungQuanLyModel>("sp_nguoidung_layDanhSach",
                new { TuKhoa = tuKhoa, VaiTro = vaiTro, Trang = trang, KichThuoc = kichThuoc });
            return PhanTrangModel<NguoiDungQuanLyModel>.Tao(kq.KetQua2, kq.KetQua1, trang, kichThuoc);
        }

        public async Task<bool> DatKhoaAsync(int id, bool biKhoa, string lyDo)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_nguoidung_datKhoa", new { Id = id, BiKhoa = biKhoa, LyDo = lyDo });
            return soDong > 0;
        }

        public async Task<bool> BiKhoaAsync(int id)
        {
            return await _dbHelper.ExecuteScalarAsync<bool>("sp_nguoidung_biKhoa", new { Id = id });
        }

        public async Task<bool> DoiMatKhauAsync(int id, string matKhauMaHoa)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_nguoidung_doiMatKhau", new { Id = id, MatKhauMaHoa = matKhauMaHoa });
            return soDong > 0;
        }

        public async Task<bool> TaoTokenDatLaiAsync(int nguoiDungId, string tokenHash, DateTime hetHan)
        {
            int daTao = await _dbHelper.ExecuteScalarAsync<int>("sp_datlai_tao",
                new { NguoiDungId = nguoiDungId, TokenHash = tokenHash, HetHan = hetHan });
            return daTao == 1;
        }

        public async Task<bool> DatLaiMatKhauAsync(string email, string tokenHash, string matKhauMaHoa)
        {
            int ketQua = await _dbHelper.ExecuteScalarAsync<int>("sp_datlai_xacNhan",
                new { Email = email, TokenHash = tokenHash, MatKhauMaHoa = matKhauMaHoa });
            return ketQua == 1;
        }
    }
}
