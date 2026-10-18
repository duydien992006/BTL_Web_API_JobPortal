using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class MatKhauBusiness : IMatKhauBusiness
    {
        private const int PhutHieuLuc = 15;

        private readonly INguoiDungRepository _repo;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _env;
        private readonly ILogger<MatKhauBusiness> _logger;

        public MatKhauBusiness(INguoiDungRepository repo, IEmailSender emailSender, IConfiguration config,
            IHostEnvironment env, ILogger<MatKhauBusiness> logger)
        {
            _repo = repo;
            _emailSender = emailSender;
            _config = config;
            _env = env;
            _logger = logger;
        }

        public async Task<PhanHoiModel> DoiMatKhauAsync(int nguoiDungId, DoiMatKhauModel model)
        {
            var nguoiDung = QuyenHelper.PhaiTonTai(await _repo.LayTheoIdAsync(nguoiDungId), "Không tìm thấy người dùng");

            if (!BCrypt.Net.BCrypt.Verify(model.MatKhauCu, nguoiDung.MatKhauMaHoa))
            {
                _logger.LogWarning("Đổi mật khẩu thất bại: sai mật khẩu hiện tại, người dùng {NguoiDungId}", nguoiDungId);
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Mật khẩu hiện tại không đúng" };
            }

            if (model.MatKhauCu == model.MatKhauMoi)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Mật khẩu mới phải khác mật khẩu hiện tại" };

            await _repo.DoiMatKhauAsync(nguoiDungId, BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi));
            _logger.LogInformation("Người dùng {NguoiDungId} đã đổi mật khẩu", nguoiDungId);
            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đổi mật khẩu thành công" };
        }

        public async Task<PhanHoiModel> QuenMatKhauAsync(QuenMatKhauModel model)
        {
            // Luôn trả cùng 1 thông điệp dù email có tồn tại hay không, để không ai dò được email nào đã đăng ký
            var ketQua = new PhanHoiModel
            {
                ThanhCong = true,
                ThongDiep = "Nếu email tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu đã được gửi"
            };

            var nguoiDung = await _repo.LayTheoEmailAsync(model.Email);
            if (nguoiDung == null || nguoiDung.BiKhoa) return ketQua;

            string token = TaoToken();
            bool daTao = await _repo.TaoTokenDatLaiAsync(nguoiDung.Id, BamSha256(token), DateTime.UtcNow.AddMinutes(PhutHieuLuc));
            if (!daTao)
            {
                _logger.LogWarning("Người dùng {NguoiDungId} yêu cầu đặt lại mật khẩu quá nhiều lần trong 1 giờ", nguoiDung.Id);
                return ketQua;
            }

            string baseUrl = _config["AppSettings:FrontendBaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl)) baseUrl = "http://localhost:3000";
            string link = baseUrl.TrimEnd('/') + "/dat-lai-mat-khau?email=" + Uri.EscapeDataString(nguoiDung.Email) + "&token=" + token;

            string noiDung = "<p>Bạn vừa yêu cầu đặt lại mật khẩu JobPortal.</p>"
                           + "<p><a href=\"" + link + "\">Bấm vào đây để đặt mật khẩu mới</a> (có hiệu lực " + PhutHieuLuc + " phút).</p>"
                           + "<p>Nếu không phải bạn yêu cầu, hãy bỏ qua email này.</p>";

            bool daGui = await _emailSender.GuiAsync(nguoiDung.Email, "Đặt lại mật khẩu JobPortal", noiDung);

            // Chỉ khi chạy Development và chưa cấu hình SMTP mới ghi link vào log để tiện test
            if (!daGui && _env.IsDevelopment())
                _logger.LogInformation("[DEV] Link đặt lại mật khẩu cho {Email}: {Link}", nguoiDung.Email, link);

            return ketQua;
        }

        public async Task<PhanHoiModel> DatLaiMatKhauAsync(DatLaiMatKhauModel model)
        {
            bool ok = await _repo.DatLaiMatKhauAsync(model.Email, BamSha256(model.Token),
                BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi));

            if (!ok)
                return new PhanHoiModel { ThanhCong = false, ThongDiep = "Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn" };

            _logger.LogInformation("Đã đặt lại mật khẩu cho {Email}", model.Email);
            return new PhanHoiModel { ThanhCong = true, ThongDiep = "Đặt lại mật khẩu thành công, vui lòng đăng nhập" };
        }

        // 32 byte ngẫu nhiên mã hóa Base64 an toàn cho URL
        private static string TaoToken()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        // DB chỉ lưu bản băm: lộ DB cũng không dùng được token
        private static string BamSha256(string token)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        }
    }
}
