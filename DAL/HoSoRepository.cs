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
    public class HoSoRepository : IHoSoRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        public HoSoRepository(IDatabaseHelper dbHelper) { _dbHelper = dbHelper; }

        public async Task<int> TaoAsync(TaoHoSoModel model)
        {
            var bangKyNang = _dbHelper.TaoBangId(model.DanhSachKyNangId);
            var thamSo = new DynamicParameters();
            thamSo.Add("UngVienId", model.UngVienId);
            thamSo.Add("TenFileGoc", model.TenFileGoc);
            thamSo.Add("DuongDanFile", model.DuongDanFile);
            thamSo.Add("SoNamKinhNghiem", model.SoNamKinhNghiem);
            thamSo.Add("DanhSachKyNang", bangKyNang.AsTableValuedParameter("DanhSachIntType"));
            return await _dbHelper.ExecuteScalarAsync<int>("sp_hoso_tao", thamSo);
        }

        public async Task<HoSoModel> LayChiTietAsync(int id)
        {
            var ketQua = await _dbHelper.QueryMultipleAsync<HoSoModel, KyNangModel>("sp_hoso_layChiTiet", new { Id = id });
            if (ketQua.KetQua1 == null) return null;
            ketQua.KetQua1.DanhSachKyNang = ketQua.KetQua2;
            return ketQua.KetQua1;
        }

        public async Task<int> LayIdMoiNhatAsync(int ungVienId)
        {
            return await _dbHelper.ExecuteScalarAsync<int>("sp_hoso_layIdMoiNhat", new { UngVienId = ungVienId });
        }

        public async Task<IEnumerable<HoSoModel>> LayDanhSachTheoUngVienAsync(int ungVienId)
        {
            return await _dbHelper.QueryAsync<HoSoModel>("sp_hoso_layDanhSachTheoUngVien", new { UngVienId = ungVienId });
        }

        public async Task<bool> XoaAsync(int id, int ungVienId)
        {
            int soDong = await _dbHelper.ExecuteAsync("sp_hoso_xoa", new { Id = id, UngVienId = ungVienId });
            return soDong > 0;
        }
    }
}
