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
    public class CongTyController : ControllerBase
    {
        private readonly ICongTyBusiness _business;
        public CongTyController(ICongTyBusiness business) { _business = business; }

        [HttpPost]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> Tao([FromBody] CongTyModel model)
        {
            var kq = await _business.TaoAsync(model, User.LayNguoiDungHienTai());
            return this.KetQua(kq);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> CapNhat(int id, [FromBody] CongTyModel model)
        {
            model.Id = id;   // Id lấy từ URL, không tin Id trong Body
            var kq = await _business.CapNhatAsync(model, User.LayNguoiDungHienTai());
            return this.KetQua(kq, 404);
        }

        [HttpPut("{id:int}/xac-minh")]
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> XacMinh(int id, [FromQuery] bool daXacMinh)
        {
            var kq = await _business.XacMinhAsync(id, daXacMinh);
            return this.KetQua(kq, 404);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "NhaTuyenDung,QuanTriVien")]
        public async Task<IActionResult> Xoa(int id)
        {
            var kq = await _business.XoaAsync(id, User.LayNguoiDungHienTai());
            return this.KetQua(kq, 404);
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
            return Ok(await _business.LayTatCaAsync());
        }
        [HttpGet("cua-toi")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> CuaToi()
        {
            return Ok(await _business.LayCuaToiAsync(User.LayNguoiDungHienTai().Id));
        }

        [HttpGet("cho-xac-minh")]
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> ChoXacMinh()
        {
            return Ok(await _business.LayChoXacMinhAsync());
        }
    }
}
