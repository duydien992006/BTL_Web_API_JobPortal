using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIQuanTriVien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "QuanTriVien")]
    public class BaoCaoController : ControllerBase
    {
        private readonly IBaoCaoBusiness _business;
        public BaoCaoController(IBaoCaoBusiness business) { _business = business; }

        [HttpGet("tong-quan")]
        public async Task<IActionResult> TongQuan()
        {
            var bc = await _business.LayTongQuanAsync();
            return Ok(bc);
        }
    }
}
