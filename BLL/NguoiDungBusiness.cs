using BLL.Interfaces;
using Common.Exceptions;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class NguoiDungBusiness : INguoiDungBusiness
    {
        private readonly INguoiDungRepository _repo;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NguoiDungBusiness> _logger;
        private static readonly string[] VaiTroDuocTuDangKy = { "UngVien", "NhaTuyenDung" };
        private static readonly string[] CacVaiTro = { "UngVien", "NhaTuyenDung", "QuanTriVien" };
        public NguoiDungBusiness(INguoiDungRepository repo, IConfiguration configuration, ILogger<NguoiDungBusiness> logger)
        {
            _repo = repo;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<PhanHoiModel> DangKyAsync(DangKyModel model)
        {
            if (!VaiTroDuocTuDangKy.Contains(model.VaiTro))
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Vai trò không hợp lệ, chỉ được đăng ký UngVien hoặc NhaTuyenDung" };

            if (string.IsNullOrEmpty(model.Email) || !model.Email.Contains("@"))
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Email không hợp lệ" };

            if (string.IsNullOrEmpty(model.MatKhau) || model.MatKhau.Length < 6)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Mật khẩu phải từ 6 ký tự trở lên" };

            var nguoiDung = new NguoiDungModel
            {
                Email = model.Email,
                MatKhauMaHoa = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                VaiTro = model.VaiTro,
                HoTen = model.HoTen,
                SoDienThoai = model.SoDienThoai
            };

            int id = await _repo.DangKyAsync(nguoiDung);
            if (id == -1)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Email đã được sử dụng" };

            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đăng ký thành công", DuLieu = id };
        }

        public async Task<PhanHoiModel> DangNhapAsync(DangNhapModel model)
        {
            var nguoiDung = await _repo.LayTheoEmailAsync(model.Email);
            if (nguoiDung == null)
            {
                _logger.LogWarning("Đăng nhập thất bại: không tồn tại tài khoản {Email}", model.Email);
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Email hoặc mật khẩu không đúng" };
            }

            bool matKhauDung = BCrypt.Net.BCrypt.Verify(model.MatKhau, nguoiDung.MatKhauMaHoa);
            if (!matKhauDung)
            {
                _logger.LogWarning("Đăng nhập thất bại: sai mật khẩu, tài khoản {Email}", model.Email);
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Email hoặc mật khẩu không đúng" };
            }
            if (nguoiDung.BiKhoa)
            {
                _logger.LogWarning("Tài khoản bị khóa cố đăng nhập: người dùng {NguoiDungId}", nguoiDung.Id);
                throw new KhongCoQuyenException("Tài khoản của bạn đã bị khóa"
                    + (string.IsNullOrWhiteSpace(nguoiDung.LyDoKhoa) ? "" : ". Lý do: " + nguoiDung.LyDoKhoa));
            }

            _logger.LogInformation("Đăng nhập thành công: người dùng {NguoiDungId} vai trò {VaiTro}",
                nguoiDung.Id, nguoiDung.VaiTro);

            string token = TaoJwtToken(nguoiDung);
            return new PhanHoiModel
            {
                ThanhCong = true,
                ThongDiep = "Đăng nhập thành công",
                DuLieu = new { Token = token, HoTen = nguoiDung.HoTen, VaiTro = nguoiDung.VaiTro }
            };
        }

        public async Task<PhanHoiModel> CapNhatAsync(CapNhatNguoiDungModel model)
        {
            bool ok = await _repo.CapNhatAsync(model);
            if (ok) return new PhanHoiModel { ThanhCong = true, ThongDiep = "Cập nhật thành công" };
            return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy người dùng" };
        }

        private string TaoJwtToken(NguoiDungModel nguoiDung)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", nguoiDung.Id.ToString()),
                new Claim(ClaimTypes.Email, nguoiDung.Email),
                new Claim(ClaimTypes.Role, nguoiDung.VaiTro)
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Secret"]));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<HoSoCaNhanModel> LayThongTinCaNhanAsync(int id)
        {
            var nguoiDung = await _repo.LayTheoIdAsync(id);
            if (nguoiDung == null) return null;

            return new HoSoCaNhanModel
            {
                Id = nguoiDung.Id,
                Email = nguoiDung.Email,
                VaiTro = nguoiDung.VaiTro,
                HoTen = nguoiDung.HoTen,
                SoDienThoai = nguoiDung.SoDienThoai,
                NgayTao = nguoiDung.NgayTao
            };
        }
        public async Task<PhanTrangModel<NguoiDungQuanLyModel>> LayDanhSachAsync(string tuKhoa, string vaiTro, int trang, int kichThuoc)
        {
            if (!string.IsNullOrWhiteSpace(vaiTro) && !CacVaiTro.Contains(vaiTro.Trim()))
                throw new NghiepVuException("Vai trò lọc không hợp lệ. Chấp nhận: " + string.Join(", ", CacVaiTro));

            return await _repo.LayDanhSachAsync(
                string.IsNullOrWhiteSpace(tuKhoa) ? null : tuKhoa.Trim(),
                string.IsNullOrWhiteSpace(vaiTro) ? null : vaiTro.Trim(),
                PhanTrangHelper.Trang(trang), PhanTrangHelper.KichThuoc(kichThuoc));
        }

        public async Task<PhanHoiModel> DatKhoaAsync(int id, bool khoa, string lyDo, NguoiDungHienTai nguoiThucHien)
        {
            if (id == nguoiThucHien.Id)
                throw new NghiepVuException("Không thể tự khóa tài khoản của chính mình");

            var dich = QuyenHelper.PhaiTonTai(await _repo.LayTheoIdAsync(id), "Không tìm thấy người dùng");
            if (dich.VaiTro == "QuanTriVien")
                throw new KhongCoQuyenException("Không thể khóa tài khoản quản trị viên");

            if (khoa && string.IsNullOrWhiteSpace(lyDo))
                throw new NghiepVuException("Phải nhập lý do khi khóa tài khoản");
            if (lyDo != null && lyDo.Length > 300)
                throw new NghiepVuException("Lý do tối đa 300 ký tự");

            bool ok = await _repo.DatKhoaAsync(id, khoa, lyDo == null ? null : lyDo.Trim());
            if (!ok) return new PhanHoiModel { ThanhCong = false, ThongDiep = "Không tìm thấy người dùng" };

            _logger.LogWarning("Quản trị viên {AdminId} {HanhDong} tài khoản {NguoiDungId}. Lý do: {LyDo}",
                nguoiThucHien.Id, khoa ? "KHÓA" : "MỞ KHÓA", id, lyDo);

            return new PhanHoiModel
            {
                ThanhCong = true,
                ThongDiep = khoa ? "Đã khóa tài khoản (có hiệu lực trong tối đa 30 giây)" : "Đã mở khóa tài khoản"
            };
        }
    }
}
