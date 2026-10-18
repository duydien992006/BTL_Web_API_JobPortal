using BLL.Interfaces;
using Common.Exceptions;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
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
        private readonly string _thuMucLuu = "D:\\JobPortal\\Files\\CV";
        private static readonly string[] DinhDangChoPhep = { ".pdf", ".doc", ".docx" };
        private const long DungLuongToiDa = 5 * 1024 * 1024;

        public HoSoBusiness(IHoSoRepository repo, ISoHuuRepository soHuuRepo, ILogger<HoSoBusiness> logger)
        {
            _repo = repo;
            _soHuuRepo = soHuuRepo;
            _logger = logger;
        }

        public async Task<PhanHoiModel> TaoAsync(IFormFile file, int ungVienId, int soNamKinhNghiem, List<int> danhSachKyNangId)
        {
            if (file == null || file.Length == 0)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Vui lòng chọn file CV" };

            string duoiFile = Path.GetExtension(file.FileName).ToLower();
            if (!DinhDangChoPhep.Contains(duoiFile))
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Chỉ chấp nhận file PDF, DOC, DOCX" };

            if (file.Length > DungLuongToiDa)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "File không được vượt quá 5MB" };

            if (danhSachKyNangId == null || danhSachKyNangId.Count == 0)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Vui lòng chọn ít nhất 1 kỹ năng" };

            string tenFileMaHoa = Guid.NewGuid().ToString() + duoiFile;
            if (!Directory.Exists(_thuMucLuu)) Directory.CreateDirectory(_thuMucLuu);
            string duongDanDayDu = Path.Combine(_thuMucLuu, tenFileMaHoa);

            using (var stream = new FileStream(duongDanDayDu, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var model = new TaoHoSoModel
            {
                UngVienId = ungVienId,
                TenFileGoc = file.FileName,
                DuongDanFile = tenFileMaHoa,
                SoNamKinhNghiem = soNamKinhNghiem,
                DanhSachKyNangId = danhSachKyNangId
            };

            int id = await _repo.TaoAsync(model);
            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Tải CV thành công", DuLieu = id };
        }

        // Dùng chung: lấy CV và kiểm tra quyền xem
        // Được xem: chủ CV, Admin, hoặc nhà tuyển dụng đã nhận CV này qua đơn ứng tuyển
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
            hoSo.DuongDanFile = null;   // không lộ tên file thật trên server
            return hoSo;
        }

        public async Task<FileTaiVeModel> LayFileTaiVeAsync(int id, NguoiDungHienTai nguoi)
        {
            var hoSo = await LayHoSoCoKiemTraQuyenAsync(id, nguoi);

            // Path.GetFileName chống path traversal (../../) dù giá trị này do hệ thống tự sinh
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
                ContentType = LayContentType(Path.GetExtension(tenFile))
            };
        }

        private static string LayContentType(string duoiFile)
        {
            switch (duoiFile.ToLower())
            {
                case ".pdf": return "application/pdf";
                case ".doc": return "application/msword";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                default: return "application/octet-stream";
            }
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
            bool ok = await _repo.XoaAsync(id, ungVienId);

            if (ok && hoSo != null)
            {
                string duongDan = Path.Combine(_thuMucLuu, hoSo.DuongDanFile);
                if (File.Exists(duongDan)) File.Delete(duongDan);
            }

            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã xóa CV" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy CV hoặc bạn không có quyền xóa" };
        }
    }
}
