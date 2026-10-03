using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace APINhaTuyenDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichPhongVanController : ControllerBase
    {
        private readonly ILichPhongVanBusiness _business;
        public LichPhongVanController(ILichPhongVanBusiness business) { _business = business; }

        [HttpPost("tao-lich")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> Tao([FromBody] LichPhongVanModel model)
        {
            var kq = await _business.TaoAsync(model);
            if (kq.ThanhCong) return Ok(kq);
            return BadRequest(kq);
        }

        [HttpPut("{id:int}/ket-qua")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> CapNhatKetQua(int id, [FromQuery] string ketQua)
        {
            var kq = await _business.CapNhatKetQuaAsync(id, ketQua);
            if (kq.ThanhCong) return Ok(kq);
            return NotFound(kq);
        }

        [HttpGet("theo-don/{donUngTuyenId:int}")]
        [Authorize]
        public async Task<IActionResult> LayTheoDon(int donUngTuyenId)
        {
            var ds = await _business.LayTheoDonUngTuyenAsync(donUngTuyenId);
            return Ok(ds);
        }
    }
}
