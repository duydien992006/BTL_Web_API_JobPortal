using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Model;
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
        private readonly string _thuMucLuu = "D:\\JobPortal\\Files\\CV";
        private static readonly string[] DinhDangChoPhep = { ".pdf", ".doc", ".docx" };
        private const long DungLuongToiDa = 5 * 1024 * 1024;

        public HoSoBusiness(IHoSoRepository repo) { _repo = repo; }

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

        public async Task<HoSoModel> LayChiTietAsync(int id) { return await _repo.LayChiTietAsync(id); }

        public async Task<IEnumerable<HoSoModel>> LayDanhSachTheoUngVienAsync(int ungVienId)
        {
            return await _repo.LayDanhSachTheoUngVienAsync(ungVienId);
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
