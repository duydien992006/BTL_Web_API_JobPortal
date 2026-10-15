using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Exceptions;
namespace BLL
{
    public class DonUngTuyenBusiness : IDonUngTuyenBusiness
    {
        private readonly IDonUngTuyenRepository _repo;
        private readonly ITinTuyenDungRepository _tinRepo;
        private readonly ICongTyRepository _congTyRepo;
        private readonly IThongBaoBusiness _thongBaoBusiness;
        private readonly ILogger<DonUngTuyenBusiness> _logger; // <--- 2. KHAI BÁO BIẾN _logger

        private static readonly Dictionary<string, string[]> LuongHopLe = new Dictionary<string, string[]>
        {
            { "DaNop", new string[] { "DangXemXet", "TuChoi" } },
            { "DangXemXet", new string[] { "PhongVan", "TuChoi" } },
            { "PhongVan", new string[] { "DeNghi", "TuChoi" } },
            { "DeNghi", new string[] { "TuChoi" } }
        };

        // <--- 3. THÊM ILogger<DonUngTuyenBusiness> logger VÀO CONSTRUCTOR
        public DonUngTuyenBusiness(
            IDonUngTuyenRepository repo,
            ITinTuyenDungRepository tinRepo,
            ICongTyRepository congTyRepo,
            IThongBaoBusiness thongBaoBusiness,
            ILogger<DonUngTuyenBusiness> logger)
        {
            _repo = repo;
            _tinRepo = tinRepo;
            _congTyRepo = congTyRepo;
            _thongBaoBusiness = thongBaoBusiness;
            _logger = logger; // <--- 4. GÁN GIÁ TRỊ CHO _logger
        }

        public async Task<PhanHoiModel> NopDonAsync(NopDonModel model, int ungVienId)
        {
            bool daTonTai = await _repo.DaTonTaiAsync(model.TinTuyenDungId, ungVienId);
            if (daTonTai)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Bạn đã ứng tuyển tin này rồi" };

            var tin = await _tinRepo.LayChiTietAsync(model.TinTuyenDungId);
            if (tin == null || tin.TrangThai != "DaDuyet")
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Tin tuyển dụng không còn hiệu lực" };

            int id = await _repo.NopDonAsync(model.TinTuyenDungId, model.HoSoId, ungVienId);

            // <--- 5. LOG SAU KHI NỘP ĐƠN THÀNH CÔNG
            _logger.LogInformation("Ứng viên {UngVienId} nộp đơn {DonUngTuyenId} vào tin {TinTuyenDungId}",
                ungVienId, id, model.TinTuyenDungId);

            var congTy = await _congTyRepo.LayTheoIdAsync(tin.CongTyId);
            if (congTy != null)
            {
                await _thongBaoBusiness.TaoAsync(congTy.NguoiDaiDienId, "Có ứng viên mới ứng tuyển vào tin: " + tin.TieuDe);
            }

            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Nộp đơn thành công", DuLieu = id };
        }

        public async Task<PhanHoiModel> DoiTrangThaiAsync(DoiTrangThaiDonModel model)
        {
            var don = await _repo.LayTheoIdAsync(model.Id);
            if (don == null)
                throw new KhongTimThayException("Không tìm thấy đơn ứng tuyển");

            if (!LuongHopLe.ContainsKey(don.TrangThai) || !LuongHopLe[don.TrangThai].Contains(model.TrangThaiMoi))
            {
                // <--- 6. LOG KHI CHUYỂN TRẠNG THÁI KHÔNG HỢP LỆ
                _logger.LogWarning("Chuyển trạng thái đơn {DonUngTuyenId} không hợp lệ: {TuTrangThai} -> {DenTrangThai}",
                    model.Id, don.TrangThai, model.TrangThaiMoi);

                string thongDiep = "Không thể chuyển từ '" + don.TrangThai + "' sang '" + model.TrangThaiMoi + "'";
                return new PhanHoiModel { ThanhCong = false, ThongDiep = thongDiep };
            }

            bool ok = await _repo.DoiTrangThaiAsync(model.Id, model.TrangThaiMoi);
            if (ok)
            {
                // <--- 7. LOG KHI CHUYỂN TRẠNG THÁI THÀNH CÔNG (Thêm ngoặc nhọn {} cho lệnh if)
                _logger.LogInformation("Đơn {DonUngTuyenId} chuyển trạng thái {TuTrangThai} -> {DenTrangThai}",
                    model.Id, don.TrangThai, model.TrangThaiMoi);

                return new PhanHoiModel { ThanhCong = true, ThongDiep = "Cập nhật trạng thái thành công" };
            }

            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Cập nhật thất bại" };
        }

        public async Task<IEnumerable<dynamic>> LayTheoTinAsync(int tinId) 
        { 
            return await _repo.LayTheoTinAsync(tinId); 
        }
        public async Task<IEnumerable<dynamic>> LayTheoUngVienAsync(int ungVienId) 
        {
            return await _repo.LayTheoUngVienAsync(ungVienId); 
        }
    }
}