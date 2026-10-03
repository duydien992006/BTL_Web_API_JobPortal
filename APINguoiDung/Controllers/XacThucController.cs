using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace APINguoiDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XacThucController : ControllerBase
    {
        private readonly INguoiDungBusiness _business;
        public XacThucController(INguoiDungBusiness business) { _business = business; }

        [HttpPost("dang-ky")]
        public async Task<IActionResult> DangKy([FromBody] DangKyModel model)
        {
            var kq = await _business.DangKyAsync(model);
            if (kq.ThanhCong) return Ok(kq);
            return BadRequest(kq);
        }

        [HttpPost("dang-nhap")]
        public async Task<IActionResult> DangNhap([FromBody] DangNhapModel model)
        {
            var kq = await _business.DangNhapAsync(model);
            if (kq.ThanhCong) return Ok(kq);
            return Unauthorized(kq);
        }
    }
}
