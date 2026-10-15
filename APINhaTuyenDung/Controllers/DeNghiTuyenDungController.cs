using BLL.Interfaces;
using Common.Problems;
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
        public async Task<IActionResult> Gui([FromBody] GuiDeNghiModel model)   // đổi kiểu tham số
        {
            var kq = await _business.GuiAsync(model);
            return this.KetQua(kq);
        }
    }
}
