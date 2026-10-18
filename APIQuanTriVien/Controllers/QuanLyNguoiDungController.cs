using BLL.Interfaces;
using Common.Extensions;
using Common.Problems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanTriVien.Controllers
{
    [Route("api/quan-ly-nguoi-dung")]
    [ApiController]
    [Authorize(Roles = "QuanTriVien")]
    public class QuanLyNguoiDungController : ControllerBase
    {
        private readonly INguoiDungBusiness _business;
        public QuanLyNguoiDungController(INguoiDungBusiness business) { _business = business; }

        [HttpGet]
        public async Task<IActionResult> DanhSach([FromQuery] string tuKhoa = null, [FromQuery] string vaiTro = null,
            [FromQuery] int trang = 1, [FromQuery] int kichThuoc = 10)
        {
            return Ok(await _business.LayDanhSachAsync(tuKhoa, vaiTro, trang, kichThuoc));
        }

        [HttpPut("{id:int}/khoa")]
        public async Task<IActionResult> Khoa(int id, [FromQuery] bool khoa, [FromQuery] string lyDo = null)
        {
            var kq = await _business.DatKhoaAsync(id, khoa, lyDo, User.LayNguoiDungHienTai());
            return this.KetQua(kq, 404);
        }
    }
}
