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

        public async Task<PhanTrangModel<TinTimKiemModel>> TimKiemAsync(TimKiemTinModel model)
        {
            var bangKyNang = _dbHelper.TaoBangId(model.DanhSachKyNangId);

            var thamSo = new DynamicParameters();
            thamSo.Add("TuKhoa", string.IsNullOrWhiteSpace(model.TuKhoa) ? null : model.TuKhoa.Trim());
            thamSo.Add("DiaDiem", string.IsNullOrWhiteSpace(model.DiaDiem) ? null : model.DiaDiem.Trim());
            thamSo.Add("LuongToiThieu", model.LuongToiThieu);
            thamSo.Add("Trang", model.Trang);
            thamSo.Add("KichThuoc", model.KichThuoc);
            thamSo.Add("DanhSachKyNang", bangKyNang.AsTableValuedParameter("DanhSachIntType"));

            // Tập 1 = tổng số bản ghi (1 số nguyên), tập 2 = các bản ghi của trang
            var kq = await _dbHelper.QueryMultipleAsync<int, TinTimKiemModel>("sp_tintuyendung_timKiem", thamSo);
            return PhanTrangModel<TinTimKiemModel>.Tao(kq.KetQua2, kq.KetQua1, model.Trang, model.KichThuoc);
        }

        public async Task<IEnumerable<TinGoiYModel>> GoiYAsync(int ungVienId, List<int> danhSachKyNangUngVien)
        {
            var bangKyNang = _dbHelper.TaoBangId(danhSachKyNangUngVien);
            var thamSo = new DynamicParameters();
            thamSo.Add("UngVienId", ungVienId);
            thamSo.Add("DanhSachKyNangUngVien", bangKyNang.AsTableValuedParameter("DanhSachIntType"));
            return await _dbHelper.QueryAsync<TinGoiYModel>("sp_tintuyendung_goiY", thamSo);
        }
        public async Task<PhanTrangModel<TinTuyenDungTomTatModel>> LayTheoNhaTuyenDungAsync(int nhaTuyenDungId, string trangThai, int trang, int kichThuoc)
        {
            var kq = await _dbHelper.QueryMultipleAsync<int, TinTuyenDungTomTatModel>("sp_tintuyendung_layTheoNhaTuyenDung",
                new { NhaTuyenDungId = nhaTuyenDungId, TrangThai = trangThai, Trang = trang, KichThuoc = kichThuoc });
            return PhanTrangModel<TinTuyenDungTomTatModel>.Tao(kq.KetQua2, kq.KetQua1, trang, kichThuoc);
        }

        public async Task<IEnumerable<TinTuyenDungTomTatModel>> LayChoDuyetAsync()
        {
            return await _dbHelper.QueryAsync<TinTuyenDungTomTatModel>("sp_tintuyendung_layChoDuyet");
        }
    }
}
