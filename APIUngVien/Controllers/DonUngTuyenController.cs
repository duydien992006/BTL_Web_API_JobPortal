using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace APIUngVien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonUngTuyenController : ControllerBase
    {
        private readonly IDonUngTuyenBusiness _business;
        public DonUngTuyenController(IDonUngTuyenBusiness business) { _business = business; }

        [HttpPost("nop-don")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> NopDon([FromBody] NopDonModel model)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            int ungVienId = claim != null ? int.Parse(claim.Value) : 0;
            var kq = await _business.NopDonAsync(model, ungVienId);
            if (kq.ThanhCong) return Ok(kq);
            return BadRequest(kq);
        }

        [HttpGet("cua-toi")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> DonCuaToi()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            int ungVienId = claim != null ? int.Parse(claim.Value) : 0;
            var ds = await _business.LayTheoUngVienAsync(ungVienId);
            return Ok(ds);
        }
    }
}
