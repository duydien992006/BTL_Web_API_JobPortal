using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace APINhaTuyenDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeNghiTuyenDungController : ControllerBase
    {
        private readonly IDeNghiTuyenDungBusiness _business;
        public DeNghiTuyenDungController(IDeNghiTuyenDungBusiness business) { _business = business; }

        [HttpPost("gui")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> Gui([FromBody] DeNghiTuyenDungModel model)
        {
            var kq = await _business.GuiAsync(model);
            if (kq.ThanhCong) return Ok(kq);
            return BadRequest(kq);
        }
    }
}
