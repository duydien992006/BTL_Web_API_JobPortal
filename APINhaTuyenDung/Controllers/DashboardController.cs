using BLL.Interfaces;
using Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APINhaTuyenDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "NhaTuyenDung")]
    public class DashboardController : ControllerBase
    {
        private readonly IBaoCaoBusiness _business;
        public DashboardController(IBaoCaoBusiness business) { _business = business; }

        [HttpGet("nha-tuyen-dung")]
        public async Task<IActionResult> NhaTuyenDung()
        {
            return Ok(await _business.LayDashboardNhaTuyenDungAsync(User.LayNguoiDungHienTai().Id));
        }
    }
}
