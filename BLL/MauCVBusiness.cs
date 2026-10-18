using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class MauCVBusiness : IMauCVBusiness
    {
        private const long DungLuongToiDa = 5 * 1024 * 1024;

        private readonly IMauCVRepository _repo;
        private readonly ILogger<MauCVBusiness> _logger;
        private readonly string _thuMucLuu;

        public MauCVBusiness(IMauCVRepository repo, ILogger<MauCVBusiness> logger, IConfiguration configuration)
        {
            _repo = repo;
            _logger = logger;
            _thuMucLuu = configuration["AppSettings:TemplateDirectory"];
            if (string.IsNullOrWhiteSpace(_thuMucLuu)) _thuMucLuu = @"D:\JobPortal\Files\Templates";
        }

        public async Task<PhanHoiModel> TaoAsync(IFormFile file, string tenMau, string moTa, int nguoiTaoId)
        {
            if (string.IsNullOrWhiteSpace(tenMau) || tenMau.Trim().Length > 100)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Tên mẫu là bắt buộc, tối đa 100 ký tự" };
            if (moTa != null && moTa.Length > 500)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Mô tả tối đa 500 ký tự" };
            if (file == null || file.Length == 0)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Vui lòng chọn file mẫu CV" };

            string duoiFile = Path.GetExtension(file.FileName).ToLower();
            if (!FileHelper.DuoiCV.Contains(duoiFile))
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Chỉ chấp nhận file PDF, DOC, DOCX" };
            if (file.Length > DungLuongToiDa)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "File không được vượt quá 5MB" };
            if (!await FileHelper.NoiDungKhopDuoiAsync(file, duoiFile))
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Nội dung file không đúng định dạng " + duoiFile };

            string tenMaHoa = await FileHelper.LuuFileAsync(file, _thuMucLuu, duoiFile);
            try
            {
                int id = await _repo.TaoAsync(new MauCVModel
                {
                    TenMau = tenMau.Trim(),
                    MoTa = string.IsNullOrWhiteSpace(moTa) ? null : moTa.Trim(),
                    TenFileGoc = FileHelper.LamSachTenFile(file.FileName),
                    DuongDanFile = tenMaHoa
                }, nguoiTaoId);

                _logger.LogInformation("Quản trị viên {AdminId} thêm mẫu CV {MauCVId}", nguoiTaoId, id);
                return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã thêm mẫu CV", DuLieu = id };
            }
            catch
            {
                FileHelper.XoaFileNeuCo(_thuMucLuu, tenMaHoa);
                throw;
            }
        }

        public async Task<IEnumerable<MauCVModel>> LayTatCaAsync()
        {
            return await _repo.LayTatCaAsync();
        }

        public async Task<FileTaiVeModel> LayFileTaiVeAsync(int id)
        {
            var mau = QuyenHelper.PhaiTonTai(await _repo.LayTheoIdAsync(id), "Không tìm thấy mẫu CV");

            string tenFile = Path.GetFileName(mau.DuongDanFile);
            string duongDan = Path.Combine(_thuMucLuu, tenFile);
            if (!File.Exists(duongDan))
                throw new Common.Exceptions.KhongTimThayException("File mẫu không còn tồn tại trên máy chủ");

            return new FileTaiVeModel
            {
                DuongDanVatLy = duongDan,
                TenFileGoc = mau.TenFileGoc,
                ContentType = FileHelper.LayContentType(Path.GetExtension(tenFile))
            };
        }

        public async Task<PhanHoiModel> XoaAsync(int id)
        {
            var mau = QuyenHelper.PhaiTonTai(await _repo.LayTheoIdAsync(id), "Không tìm thấy mẫu CV");
            bool ok = await _repo.XoaAsync(id);
            if (ok) FileHelper.XoaFileNeuCo(_thuMucLuu, mau.DuongDanFile);

            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đã xóa mẫu CV" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy mẫu CV" };
        }
    }
}
