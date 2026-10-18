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
        private readonly IHoSoRepository _hoSoRepo;
        private readonly ISoHuuRepository _soHuuRepo;
        private readonly ILichPhongVanRepository _lichRepo;          // MỚI
        private readonly IDeNghiTuyenDungRepository _deNghiRepo;     // MỚI
        private readonly IThongBaoBusiness _thongBaoBusiness;
        private readonly ILogger<DonUngTuyenBusiness> _logger;

        // Nhà tuyển dụng chỉ được đặt tay 2 trạng thái này.
        // PhongVan / DeNghi / TrungTuyen / UngVienTuChoi do SP tự đặt (tạo lịch, gửi offer, ứng viên phản hồi)
        private static readonly string[] TrangThaiThuCong = { "DangXemXet", "TuChoi" };

        private static readonly Dictionary<string, string[]> LuongHopLe = new Dictionary<string, string[]>
        {
            { "DaNop", new string[] { "DangXemXet", "TuChoi" } },
            { "DangXemXet", new string[] { "TuChoi" } },
            { "PhongVan", new string[] { "TuChoi" } },
            { "DeNghi", new string[] { "TuChoi" } }
            // TuChoi, TrungTuyen, UngVienTuChoi là trạng thái cuối: không có dòng nào ở đây
        };

        private static readonly Dictionary<string, string> NhanTrangThai = new Dictionary<string, string>
        {
            { "DangXemXet", "đang được nhà tuyển dụng xem xét" },
            { "TuChoi", "đã bị nhà tuyển dụng từ chối" }
        };

        public DonUngTuyenBusiness(
            IDonUngTuyenRepository repo,
            ITinTuyenDungRepository tinRepo,
            ICongTyRepository congTyRepo,
            IHoSoRepository hoSoRepo,
            ISoHuuRepository soHuuRepo,
            ILichPhongVanRepository lichRepo,
            IDeNghiTuyenDungRepository deNghiRepo,
            IThongBaoBusiness thongBaoBusiness,
            ILogger<DonUngTuyenBusiness> logger)
        {
            _repo = repo;
            _tinRepo = tinRepo;
            _congTyRepo = congTyRepo;
            _hoSoRepo = hoSoRepo;
            _soHuuRepo = soHuuRepo;
            _lichRepo = lichRepo;
            _deNghiRepo = deNghiRepo;
            _thongBaoBusiness = thongBaoBusiness;
            _logger = logger;
        }

        public async Task<PhanHoiModel> NopDonAsync(NopDonModel model, int ungVienId)
        {
            var hoSo = QuyenHelper.PhaiTonTai(await _hoSoRepo.LayChiTietAsync(model.HoSoId), "Không tìm thấy hồ sơ");
            if (hoSo.UngVienId != ungVienId)
                throw new KhongCoQuyenException("Hồ sơ này không thuộc về bạn");

            bool daTonTai = await _repo.DaTonTaiAsync(model.TinTuyenDungId, ungVienId);
            if (daTonTai)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Bạn đã ứng tuyển tin này rồi" };

            var tin = await _tinRepo.LayChiTietAsync(model.TinTuyenDungId);
            if (tin == null || tin.TrangThai != "DaDuyet")
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Tin tuyển dụng không còn hiệu lực" };

            if (tin.NgayHetHan <= DateTime.UtcNow)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Tin tuyển dụng đã hết hạn nhận hồ sơ" };

            int id = await _repo.NopDonAsync(model.TinTuyenDungId, model.HoSoId, ungVienId);

            _logger.LogInformation("Ứng viên {UngVienId} nộp đơn {DonUngTuyenId} vào tin {TinTuyenDungId}",
                ungVienId, id, model.TinTuyenDungId);

            var congTy = await _congTyRepo.LayTheoIdAsync(tin.CongTyId);
            if (congTy != null)
            {
                await _thongBaoBusiness.TaoAsync(congTy.NguoiDaiDienId, "Có ứng viên mới ứng tuyển vào tin: " + tin.TieuDe);
            }

            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Nộp đơn thành công", DuLieu = id };
        }

        public async Task<PhanHoiModel> DoiTrangThaiAsync(DoiTrangThaiDonModel model, NguoiDungHienTai nguoi)
        {
            var don = QuyenHelper.PhaiTonTai(await _soHuuRepo.LayThongTinDonAsync(model.Id), "Không tìm thấy đơn ứng tuyển");
            QuyenHelper.PhaiLaChuSoHuu(don.NhaTuyenDungId, nguoi);

            if (!TrangThaiThuCong.Contains(model.TrangThaiMoi))
            {
                return new PhanHoiModel
                {
                    ThanhCong = false,
                    ThongDiep = "Không đặt tay được trạng thái '" + model.TrangThaiMoi + "'. PhongVan được đặt khi tạo lịch phỏng vấn, "
                                + "DeNghi khi gửi đề nghị, TrungTuyen/UngVienTuChoi khi ứng viên phản hồi đề nghị"
                };
            }

            if (!LuongHopLe.ContainsKey(don.TrangThai) || !LuongHopLe[don.TrangThai].Contains(model.TrangThaiMoi))
            {
                _logger.LogWarning("Chuyển trạng thái đơn {DonUngTuyenId} không hợp lệ: {TuTrangThai} -> {DenTrangThai}",
                    model.Id, don.TrangThai, model.TrangThaiMoi);

                string thongDiep = "Không thể chuyển từ '" + don.TrangThai + "' sang '" + model.TrangThaiMoi + "'";
                return new PhanHoiModel { ThanhCong = false, ThongDiep = thongDiep };
            }

            bool ok = await _repo.DoiTrangThaiAsync(model.Id, model.TrangThaiMoi);
            if (ok)
            {
                _logger.LogInformation("Đơn {DonUngTuyenId} chuyển trạng thái {TuTrangThai} -> {DenTrangThai}",
                    model.Id, don.TrangThai, model.TrangThaiMoi);

                // Báo cho ứng viên
                string nhan;
                if (!NhanTrangThai.TryGetValue(model.TrangThaiMoi, out nhan)) nhan = model.TrangThaiMoi;
                await _thongBaoBusiness.TaoAsync(don.UngVienId, "Đơn ứng tuyển #" + model.Id + " của bạn " + nhan);

                return new PhanHoiModel { ThanhCong = true, ThongDiep = "Cập nhật trạng thái thành công" };
            }

            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Cập nhật thất bại" };
        }

        public async Task<IEnumerable<dynamic>> LayTheoTinAsync(int tinId, NguoiDungHienTai nguoi)
        {
            var tin = QuyenHelper.PhaiTonTai(await _soHuuRepo.LayThongTinTinAsync(tinId), "Không tìm thấy tin tuyển dụng");
            QuyenHelper.PhaiLaChuSoHuu(tin.NhaTuyenDungId, nguoi);

            return await _repo.LayTheoTinAsync(tinId);
        }

        public async Task<IEnumerable<dynamic>> LayTheoUngVienAsync(int ungVienId)
        {
            return await _repo.LayTheoUngVienAsync(ungVienId);
        }

        // Chi tiết 1 đơn: thông tin đơn + toàn bộ lịch phỏng vấn + toàn bộ offer
        // Chỉ ứng viên của đơn, nhà tuyển dụng của tin, hoặc Admin được xem
        public async Task<ChiTietDonUngTuyenModel> LayChiTietAsync(int id, NguoiDungHienTai nguoi)
        {
            var soHuu = QuyenHelper.PhaiTonTai(await _soHuuRepo.LayThongTinDonAsync(id), "Không tìm thấy đơn ứng tuyển");
            QuyenHelper.PhaiLaBenLienQuan(soHuu, nguoi);

            var don = QuyenHelper.PhaiTonTai(await _repo.LayChiTietAsync(id), "Không tìm thấy đơn ứng tuyển");

            return new ChiTietDonUngTuyenModel
            {
                Don = don,
                LichPhongVan = (await _lichRepo.LayTheoDonUngTuyenAsync(id)).ToList(),
                DeNghi = (await _deNghiRepo.LayTheoDonUngTuyenAsync(id)).ToList()
            };
        }
    }
}