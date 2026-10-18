using BLL.Interfaces;
using Common.Extensions;
using Common.Problems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanTriVien.Controllers
{
    [Route("api/mau-cv")]
    [ApiController]
    public class MauCVController : ControllerBase
    {
        private readonly IMauCVBusiness _business;
        public MauCVController(IMauCVBusiness business) { _business = business; }

        // Công khai: người chưa đăng nhập cũng xem và tải được, và thẻ <a href> không gửi được token
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LayTatCa()
        {
            return Ok(await _business.LayTatCaAsync());
        }

        [HttpGet("{id:int}/tai-ve")]
        [AllowAnonymous]
        public async Task<IActionResult> TaiVe(int id)
        {
            var file = await _business.LayFileTaiVeAsync(id);
            return PhysicalFile(file.DuongDanVatLy, file.ContentType, file.TenFileGoc);
        }

        [HttpPost]
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> Tao(IFormFile file, [FromForm] string tenMau, [FromForm] string moTa)
        {
            var kq = await _business.TaoAsync(file, tenMau, moTa, User.LayNguoiDungHienTai().Id);
            return this.KetQua(kq);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> Xoa(int id)
        {
            var kq = await _business.XoaAsync(id);
            return this.KetQua(kq, 404);
        }
    }
}
