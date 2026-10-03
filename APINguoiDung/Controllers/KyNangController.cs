using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace APINguoiDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KyNangController : ControllerBase
    {
        private readonly IKyNangBusiness _business;
        public KyNangController(IKyNangBusiness business) { _business = business; }

        [HttpPost]
        public async Task<IActionResult> Tao([FromBody] TaoKyNangModel model)
        {
            var kq = await _business.TaoAsync(model);
            if (kq.ThanhCong) return Ok(kq);
            return BadRequest(kq);
        }

        [HttpGet]
        public async Task<IActionResult> LayTatCa()
        {
            var ds = await _business.LayTatCaAsync();
            return Ok(ds);
        }

        [HttpGet("tim-kiem")]
        public async Task<IActionResult> TimKiem([FromQuery] string tuKhoa)
        {
            var ds = await _business.TimKiemAsync(tuKhoa);
            return Ok(ds);
        }
    }
}
