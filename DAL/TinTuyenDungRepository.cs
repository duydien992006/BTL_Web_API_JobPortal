using DAL.Helper.Interfaces;
using DAL.Interfaces;
using Dapper;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class TinTuyenDungRepository : ITinTuyenDungRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        public TinTuyenDungRepository(IDatabaseHelper dbHelper) { _dbHelper = dbHelper; }

        public async Task<int> TaoAsync(TaoTinTuyenDungModel model)
        {
            var bangKyNang = _dbHelper.TaoBangId(model.DanhSachKyNangId);
            var thamSo = new DynamicParameters();
            thamSo.Add("CongTyId", model.CongTyId);
            thamSo.Add("TieuDe", model.TieuDe);
            thamSo.Add("MoTaCongViec", model.MoTaCongViec);
            thamSo.Add("DiaDiem", model.DiaDiem);
            thamSo.Add("MucLuongTu", model.MucLuongTu);
            thamSo.Add("MucLuongDen", model.MucLuongDen);
            thamSo.Add("NgayHetHan", model.NgayHetHan);
            thamSo.Add("DanhSachKyNang", bangKyNang.AsTableValuedParameter("DanhSachIntType"));
            return await _dbHelper.ExecuteScalarAsync<int>("sp_tintuyendung_tao", thamSo);
        }

        public async Task<bool> CapNhatAsync(int id, TaoTinTuyenDungModel model)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_tintuyendung_capNhat", new
            {
                Id = id,
                model.TieuDe,
                model.MoTaCongViec,
                model.DiaDiem,
                model.MucLuongTu,
                model.MucLuongDen,
                model.NgayHetHan
            });
            return soDong > 0;
        }

        public async Task<bool> DuyetAsync(int id, string trangThai)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_tintuyendung_duyet", new { Id = id, TrangThai = trangThai });
            return soDong > 0;
        }

        public async Task<bool> DongAsync(int id)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_tintuyendung_dong", new { Id = id });
            return soDong > 0;
        }

        public async Task<bool> XoaAsync(int id)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_tintuyendung_xoa", new { Id = id });
            return soDong > 0;
        }

        public async Task<TinTuyenDungModel> LayChiTietAsync(int id)
        {
            var ketQua = await _dbHelper.QueryMultipleAsync<TinTuyenDungModel, KyNangModel>("sp_tintuyendung_layChiTiet", new { Id = id });
            if (ketQua.KetQua1 == null) return null;
            ketQua.KetQua1.DanhSachKyNang = ketQua.KetQua2;
            return ketQua.KetQua1;
        }

        public async Task<IEnumerable<TinTuyenDungModel>> TimKiemAsync(TimKiemTinModel model)
        {
            var bangKyNang = _dbHelper.TaoBangId(model.DanhSachKyNangId);
            var thamSo = new DynamicParameters();
            thamSo.Add("TuKhoa", model.TuKhoa);
            thamSo.Add("DiaDiem", model.DiaDiem);
            thamSo.Add("DanhSachKyNang", bangKyNang.AsTableValuedParameter("DanhSachIntType"));
            return await _dbHelper.QueryAsync<TinTuyenDungModel>("sp_tintuyendung_timKiem", thamSo);
        }

        public async Task<IEnumerable<TinTuyenDungModel>> GoiYAsync(List<int> danhSachKyNangUngVien)
        {
            var bangKyNang = _dbHelper.TaoBangId(danhSachKyNangUngVien);
            var thamSo = new DynamicParameters();
            thamSo.Add("DanhSachKyNangUngVien", bangKyNang.AsTableValuedParameter("DanhSachIntType"));
            return await _dbHelper.QueryAsync<TinTuyenDungModel>("sp_tintuyendung_goiY", thamSo);
        }
    }
}
