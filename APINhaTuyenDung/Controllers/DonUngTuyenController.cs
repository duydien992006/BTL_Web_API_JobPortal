using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace APINhaTuyenDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonUngTuyenController : ControllerBase
    {
        private readonly IDonUngTuyenBusiness _business;
        public DonUngTuyenController(IDonUngTuyenBusiness business) { _business = business; }

        [HttpPut("doi-trang-thai")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> DoiTrangThai([FromBody] DoiTrangThaiDonModel model)
        {
            var kq = await _business.DoiTrangThaiAsync(model);
            if (kq.ThanhCong) return Ok(kq);
            return BadRequest(kq);
        }

        [HttpGet("theo-tin/{tinId:int}")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> LayTheoTin(int tinId)
        {
            var ds = await _business.LayTheoTinAsync(tinId);
            return Ok(ds);
        }
    }
}
