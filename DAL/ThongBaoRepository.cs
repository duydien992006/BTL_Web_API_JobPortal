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

        public async Task<PhanTrangThongBaoModel> LayTheoNguoiDungAsync(int nguoiDungId, int trang, int kichThuoc)
        {
            var kq = await _dbHelper.QueryMultipleAsync<TongThongBaoModel, ThongBaoModel>(
                "sp_thongbao_layTheoNguoiDung",
                new { NguoiDungId = nguoiDungId, Trang = trang, KichThuoc = kichThuoc });

            return new PhanTrangThongBaoModel
            {
                DanhSach = kq.KetQua2,
                TongSo = kq.KetQua1 == null ? 0 : kq.KetQua1.TongSo,
                SoChuaDoc = kq.KetQua1 == null ? 0 : kq.KetQua1.SoChuaDoc,
                Trang = trang,
                KichThuoc = kichThuoc
            };
        }

        public async Task<bool> DanhDauDaDocAsync(int id, int nguoiDungId)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_thongbao_danhDauDaDoc", new { Id = id, NguoiDungId = nguoiDungId });
            return soDong > 0;
        }
    }
}
