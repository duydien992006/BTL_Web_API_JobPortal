using BLL.Interfaces;
using Common.Extensions;
using Common.Problems;
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
        public async Task<IActionResult> LayCuaToi([FromQuery] int trang = 1, [FromQuery] int kichThuoc = 10)
        {
            return Ok(await _business.LayTheoNguoiDungAsync(User.LayNguoiDungHienTai().Id, trang, kichThuoc));
        }

        [HttpPut("{id:int}/da-doc")]
        public async Task<IActionResult> DanhDauDaDoc(int id)
        {
            var kq = await _business.DanhDauDaDocAsync(id, User.LayNguoiDungHienTai().Id);
            return this.KetQua(kq, 404);
        }
    }
}
