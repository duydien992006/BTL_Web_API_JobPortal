using BLL.Interfaces;
using Common.Extensions;
using Common.Problems;
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
        public async Task<IActionResult> Tao([FromBody] TaoLichPhongVanModel model)
        {
            var kq = await _business.TaoAsync(model, User.LayNguoiDungHienTai());
            return this.KetQua(kq);
        }

        [HttpPut("{id:int}/ket-qua")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> CapNhatKetQua(int id, [FromQuery] string ketQua)
        {
            var kq = await _business.CapNhatKetQuaAsync(id, ketQua, User.LayNguoiDungHienTai());
            return this.KetQua(kq, 404);
        }

        [HttpGet("theo-don/{donUngTuyenId:int}")]
        [Authorize]
        public async Task<IActionResult> LayTheoDon(int donUngTuyenId)
        {
            return Ok(await _business.LayTheoDonUngTuyenAsync(donUngTuyenId, User.LayNguoiDungHienTai()));
        }

        [HttpGet("cua-toi")]
        [Authorize(Roles = "UngVien,NhaTuyenDung")]
        public async Task<IActionResult> CuaToi()
        {
            return Ok(await _business.LayCuaToiAsync(User.LayNguoiDungHienTai()));
        }
    }
}
