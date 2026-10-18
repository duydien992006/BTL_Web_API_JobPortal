using BLL.Interfaces;
using Common.Exceptions;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class HoSoBusiness : IHoSoBusiness
    {
        private readonly IHoSoRepository _repo;
        private readonly ISoHuuRepository _soHuuRepo;
        private readonly ILogger<HoSoBusiness> _logger;
        private readonly string _thuMucLuu;
        private const long DungLuongToiDa = 5 * 1024 * 1024;   // 5MB

        public HoSoBusiness(IHoSoRepository repo, ISoHuuRepository soHuuRepo,
            ILogger<HoSoBusiness> logger, IConfiguration configuration)
        {
            _repo = repo;
            _soHuuRepo = soHuuRepo;
            _logger = logger;

            // Đọc từ appsettings.json, nếu chưa cấu hình thì dùng đường dẫn cũ
            _thuMucLuu = configuration["AppSettings:CvDirectory"];
            if (string.IsNullOrWhiteSpace(_thuMucLuu)) _thuMucLuu = @"D:\JobPortal\Files\CV";
        }

        public async Task<PhanHoiModel> TaoAsync(IFormFile file, int ungVienId, int soNamKinhNghiem, List<int> danhSachKyNangId)
        {
            if (file == null || file.Length == 0)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Vui lòng chọn file CV" };

            string duoiFile = Path.GetExtension(file.FileName).ToLower();
            if (!FileHelper.DuoiCV.Contains(duoiFile))
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Chỉ chấp nhận file PDF, DOC, DOCX" };

            if (file.Length > DungLuongToiDa)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "File không được vượt quá 5MB" };

            // MỚI: kiểm tra nội dung thật, chặn file đổi đuôi
            if (!await FileHelper.NoiDungKhopDuoiAsync(file, duoiFile))
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Nội dung file không đúng định dạng " + duoiFile };

            if (soNamKinhNghiem < 0 || soNamKinhNghiem > 60)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Số năm kinh nghiệm phải từ 0 đến 60" };

            if (danhSachKyNangId == null || danhSachKyNangId.Count == 0)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Vui lòng chọn ít nhất 1 kỹ năng" };

            if (danhSachKyNangId.Count > 30)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Tối đa 30 kỹ năng" };

            string tenFileMaHoa = await FileHelper.LuuFileAsync(file, _thuMucLuu, duoiFile);
            try
            {
                var model = new TaoHoSoModel
                {
                    UngVienId = ungVienId,
                    TenFileGoc = FileHelper.LamSachTenFile(file.FileName),
                    DuongDanFile = tenFileMaHoa,
                    SoNamKinhNghiem = soNamKinhNghiem,
                    DanhSachKyNangId = danhSachKyNangId
                };
                int id = await _repo.TaoAsync(model);
                return new PhanHoiModel { ThanhCong = true, ThongDiep = "Tải CV thành công", DuLieu = id };
            }
            catch
            {
                // Ghi DB lỗi thì xóa file vừa lưu, tránh file mồ côi
                FileHelper.XoaFileNeuCo(_thuMucLuu, tenFileMaHoa);
                throw;
            }
        }

        // Chủ CV, Admin, hoặc nhà tuyển dụng đã nhận CV này qua đơn ứng tuyển mới được xem
        private async Task<HoSoModel> LayHoSoCoKiemTraQuyenAsync(int id, NguoiDungHienTai nguoi)
        {
            var hoSo = QuyenHelper.PhaiTonTai(await _repo.LayChiTietAsync(id), "Không tìm thấy hồ sơ");

            bool duocXem = nguoi.LaAdmin
                || hoSo.UngVienId == nguoi.Id
                || (nguoi.VaiTro == "NhaTuyenDung" && await _soHuuRepo.NhaTuyenDungCoQuyenXemHoSoAsync(id, nguoi.Id));

            if (!duocXem) throw new KhongCoQuyenException("Bạn không có quyền xem hồ sơ này");
            return hoSo;
        }

        public async Task<HoSoModel> LayChiTietAsync(int id, NguoiDungHienTai nguoi)
        {
            var hoSo = await LayHoSoCoKiemTraQuyenAsync(id, nguoi);
            hoSo.DuongDanFile = null;
            return hoSo;
        }

        public async Task<FileTaiVeModel> LayFileTaiVeAsync(int id, NguoiDungHienTai nguoi)
        {
            var hoSo = await LayHoSoCoKiemTraQuyenAsync(id, nguoi);

            string tenFile = Path.GetFileName(hoSo.DuongDanFile);
            string duongDan = Path.Combine(_thuMucLuu, tenFile);
            if (!File.Exists(duongDan))
                throw new KhongTimThayException("File CV không còn tồn tại trên máy chủ");

            if (hoSo.UngVienId != nguoi.Id)
            {
                _logger.LogInformation("Người dùng {NguoiDungId} ({VaiTro}) tải CV {HoSoId} của ứng viên {ChuHoSoId}",
                    nguoi.Id, nguoi.VaiTro, id, hoSo.UngVienId);
            }

            return new FileTaiVeModel
            {
                DuongDanVatLy = duongDan,
                TenFileGoc = hoSo.TenFileGoc,
                ContentType = FileHelper.LayContentType(Path.GetExtension(tenFile))
            };
        }

        public async Task<IEnumerable<HoSoModel>> LayDanhSachTheoUngVienAsync(int ungVienId)
        {
            var ds = (await _repo.LayDanhSachTheoUngVienAsync(ungVienId)).ToList();
            foreach (var h in ds) h.DuongDanFile = null;
            return ds;
        }

        public async Task<PhanHoiModel> XoaAsync(int id, int ungVienId)
        {
            var hoSo = await _repo.LayChiTietAsync(id);

            // CV đã dùng nộp đơn thì SQL báo lỗi khóa ngoại (409), file vẫn được giữ nguyên
            bool ok = await _repo.XoaAsync(id, ungVienId);

            if (ok && hoSo != null) FileHelper.XoaFileNeuCo(_thuMucLuu, hoSo.DuongDanFile);

            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã xóa CV" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy CV hoặc bạn không có quyền xóa" };
        }
    }
}
