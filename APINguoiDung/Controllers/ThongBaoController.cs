using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APINguoiDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ThongBaoController : ControllerBase
    {
        private readonly IThongBaoBusiness _business;
        public ThongBaoController(IThongBaoBusiness business) { _business = business; }

        [HttpGet]
        public async Task<IActionResult> LayCuaToi()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            int nguoiDungId = claim != null ? int.Parse(claim.Value) : 0;
            var ds = await _business.LayTheoNguoiDungAsync(nguoiDungId);
            return Ok(ds);
        }

        [HttpPut("{id:int}/da-doc")]
        public async Task<IActionResult> DanhDauDaDoc(int id)
        {
            var kq = await _business.DanhDauDaDocAsync(id);
            if (kq.ThanhCong) return Ok(kq);
            return NotFound(kq);
        }
    }
}
