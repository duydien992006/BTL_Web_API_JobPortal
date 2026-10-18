using Common.Exceptions;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Dùng cho endpoint có [Authorize]. Thay cho đoạn int.Parse(claim.Value) lặp ở mọi Controller
        public static NguoiDungHienTai LayNguoiDungHienTai(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("Id");
            int id;
            if (claim == null || !int.TryParse(claim.Value, out id) || id <= 0)
                throw new AppException("Token không chứa thông tin người dùng hợp lệ", 401, "TOKEN_THIEU_ID");

            string vaiTro = null;
            foreach (var r in new[] { "QuanTriVien", "NhaTuyenDung", "UngVien" })
            {
                if (user.IsInRole(r)) { vaiTro = r; break; }
            }
            return new NguoiDungHienTai { Id = id, VaiTro = vaiTro };
        }

        // Dùng cho endpoint công khai nhưng muốn biết người gọi là ai nếu họ có gửi token
        public static NguoiDungHienTai LayNguoiDungHienTaiHoacNull(this ClaimsPrincipal user)
        {
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated) return null;
            return user.LayNguoiDungHienTai();
        }
    }
}
