using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace APINhaTuyenDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CongTyController : ControllerBase
    {
        private readonly ICongTyBusiness _business;
        public CongTyController(ICongTyBusiness business) { _business = business; }

        [HttpPost]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> Tao([FromBody] CongTyModel model)
        {
            var kq = await _business.TaoAsync(model);
            if (kq.ThanhCong) return Ok(kq);
            return BadRequest(kq);
        }

        [HttpPut("{id:int}/xac-minh")]
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> XacMinh(int id, [FromQuery] bool daXacMinh)
        {
            var kq = await _business.XacMinhAsync(id, daXacMinh);
            if (kq.ThanhCong) return Ok(kq);
            return NotFound(kq);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> LayTheoId(int id)
        {
            var congTy = await _business.LayTheoIdAsync(id);
            if (congTy == null) return NotFound();
            return Ok(congTy);
        }

        [HttpGet]
        public async Task<IActionResult> LayTatCa()
        {
            var ds = await _business.LayTatCaAsync();
            return Ok(ds);
        }
    }
}
