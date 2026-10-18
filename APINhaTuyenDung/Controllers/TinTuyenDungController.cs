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
    public class TinTuyenDungController : ControllerBase
    {
        private readonly ITinTuyenDungBusiness _business;
        public TinTuyenDungController(ITinTuyenDungBusiness business) { _business = business; }

        [HttpPost]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> Tao([FromBody] TaoTinTuyenDungModel model)
        {
            var kq = await _business.TaoAsync(model, User.LayNguoiDungHienTai());
            return this.KetQua(kq);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> CapNhat(int id, [FromBody] TaoTinTuyenDungModel model)
        {
            var kq = await _business.CapNhatAsync(id, model, User.LayNguoiDungHienTai());
            return this.KetQua(kq, 404);
        }

        [HttpPut("{id:int}/duyet")]
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> Duyet(int id, [FromQuery] bool duyet)
        {
            var kq = await _business.DuyetAsync(id, duyet);
            return this.KetQua(kq, 404);
        }

        [HttpPut("{id:int}/dong")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> Dong(int id)
        {
            var kq = await _business.DongAsync(id, User.LayNguoiDungHienTai());
            return this.KetQua(kq, 404);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "NhaTuyenDung,QuanTriVien")]
        public async Task<IActionResult> Xoa(int id)
        {
            var kq = await _business.XoaAsync(id, User.LayNguoiDungHienTai());
            return this.KetQua(kq, 404);
        }

        // Endpoint công khai, nhưng nếu người gọi có gửi token thì Business biết họ là ai
        [HttpGet("{id:int}")]
        public async Task<IActionResult> LayChiTiet(int id)
        {
            var tin = await _business.LayChiTietAsync(id, User.LayNguoiDungHienTaiHoacNull());
            if (tin == null) return NotFound();
            return Ok(tin);
        }

        [HttpPost("tim-kiem")]
        public async Task<IActionResult> TimKiem([FromBody] TimKiemTinModel model)
        {
            return Ok(await _business.TimKiemAsync(model));
        }

        [HttpGet("goi-y")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> GoiY()
        {
            var ds = await _business.GoiYAsync(User.LayNguoiDungHienTai().Id);
            return Ok(ds);
        }
        [HttpGet("cua-toi")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> CuaToi()
        {
            return Ok(await _business.LayCuaToiAsync(User.LayNguoiDungHienTai().Id));
        }

        [HttpGet("cho-duyet")]
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> ChoDuyet()
        {
            return Ok(await _business.LayChoDuyetAsync());
        }
    }
}
