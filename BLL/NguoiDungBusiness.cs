using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;
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

        public NguoiDungBusiness(INguoiDungRepository repo, IConfiguration configuration)
        {
            _repo = repo;
            _configuration = configuration;
        }

        public async Task<PhanHoiModel> DangKyAsync(DangKyModel model)
        {
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
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Email hoặc mật khẩu không đúng" };

            bool matKhauDung = BCrypt.Net.BCrypt.Verify(model.MatKhau, nguoiDung.MatKhauMaHoa);
            if (!matKhauDung)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Email hoặc mật khẩu không đúng" };

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
    }
}
